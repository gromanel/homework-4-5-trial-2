#include <WiFi.h>
#include <WiFiUdp.h>

//Install Board Libraries for "esp32 by expressif".
//Make board type NodeMCU-32S
//Sample board supplier: https://www.amazon.com/HiLetgo-ESP-WROOM-32-Development-Microcontroller-Integrated/dp/B0718T232Z

// Replace with your network credentials
const char *ssid = "NETGEAR39";//"TP-Link_5E30";
const char *password = "freshbreeze181";//"18506839";

// Set up UDP
WiFiUDP udp;
const char *udpAddress = "172.168.10.6";  // Mac IP address 
const int udpSenderPort = 5006;  // Port for sending data to Unity
const int udpReceiverPort = 5005;  // Port for receiving data from Unity
String sentMessage;
String receivedMessage;

//Char LED + Buzzer (Desert Maze Game)
const int ledPin = 2;   // LED: on while Unity game is running
const int buzzerPin = 4;  // Buzzer: goes off when water touches fountain (change to your pin)

unsigned long buzzerStartTime = 0;
const unsigned long buzzerDurationMs = 3000;  // How long to buzz when win (ms)
bool buzzerActive = false;

int charsSent = 0;
int sendTimer = 0;
int sendTime = 100;

void setup() {
  // initialize the serial communication:
  Serial.begin(9600);

  // Init UDP
  initUDP();

  // initialize the ledPin and buzzer as outputs:
  pinMode(ledPin, OUTPUT);
  pinMode(buzzerPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  digitalWrite(buzzerPin, LOW);

  // Quick hardware test: LED on + short beep so you know output works
  digitalWrite(ledPin, HIGH);
  digitalWrite(buzzerPin, HIGH);
  delay(200);
  digitalWrite(buzzerPin, LOW);
  delay(200);
  digitalWrite(ledPin, LOW);
  Serial.println("Output test done. LED and buzzer should have flashed/beeped.");
}

void loop() {
  // Check WiFi connection status
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("WiFi disconnected! Attempting to reconnect...");
    digitalWrite(ledPin, LOW);
    WiFi.begin(ssid, password);
    delay(5000);
    return;
  }

  // Listen for a message from UDP (UDP -> Arduino ESP32)
  receivedMessage = receiveUDP();

  // Trim whitespace/newline so "c", "c\n", "c\r" all work
  receivedMessage.trim();

  // Use first character so we're robust to extra bytes from Unity
  char cmd = receivedMessage.length() > 0 ? receivedMessage.charAt(0) : '\0';

  // Control the LED and buzzer based on the received message
  if (cmd == 'c') {
    digitalWrite(ledPin, HIGH);  // Turn LED on (game running)
  } else if (cmd == 'd') {
    digitalWrite(ledPin, LOW);   // Turn LED off (game stopped)
  } else if (cmd == 'b') {
    // Buzzer: water reached fountain (win)
    buzzerActive = true;
    buzzerStartTime = millis();
    digitalWrite(buzzerPin, HIGH);
  }

  // Turn off buzzer after duration
  if (buzzerActive && (millis() - buzzerStartTime >= buzzerDurationMs)) {
    buzzerActive = false;
    digitalWrite(buzzerPin, LOW);
  }

  //Sending data via UDP on a repeating timer (only if WiFi connected):
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;

    //Switch between sending chars 'a' or 'b':
    charsSent += 1;
    if(charsSent % 2 == 0) {
      sentMessage = "a";
    } else {
      sentMessage = "b";
    }

    // Send message to UDP
    sendUDP(sentMessage);
  }

  //Quick delay:
  delay(10);
}

void initUDP() {
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.println("Connecting to WiFi...");
  Serial.print("SSID: ");
  Serial.println(ssid);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  
  Serial.println("");
  Serial.println("Connected to WiFi!");
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  Serial.print("Arduino IP address: ");
  Serial.println(WiFi.localIP());
  Serial.print("Subnet Mask: ");
  Serial.println(WiFi.subnetMask());
  Serial.print("Gateway IP: ");
  Serial.println(WiFi.gatewayIP());
  Serial.print("DNS IP: ");
  Serial.println(WiFi.dnsIP());
  Serial.print("MAC Address: ");
  Serial.println(WiFi.macAddress());

  // Start listening for UDP on the receive port
  udp.begin(udpReceiverPort);
  Serial.println("UDP Initialized");
}

String receiveUDP() {
  int packetSize = udp.parsePacket();
  if (packetSize > 0) {
    char incomingPacket[255];
    int len = udp.read(incomingPacket, 255);
    if (len > 0) {
      incomingPacket[len] = 0; //Null-terminate string for formatting.
      String message = String(incomingPacket);
      Serial.println("← Received: '" + message + "' from " + udp.remoteIP().toString() + ":" + String(udp.remotePort()) + " (" + String(len) + " bytes)");
      return message;
    } else {
      Serial.println("ERROR: Receive failed - read 0 bytes");
      return "";
    }
  }
  return "";
}

void sendUDP(String message) {
  int beginResult = udp.beginPacket(udpAddress, udpSenderPort);
  if (beginResult == 0) {
    Serial.println("ERROR: Send failed - beginPacket");
    return;
  }
  
  size_t written = udp.write((const uint8_t *)message.c_str(), message.length());
  int endResult = udp.endPacket();
  
  if (endResult == 1) {
    Serial.println("→ Sent: '" + message + "' to " + String(udpAddress) + ":" + String(udpSenderPort) + " (" + String(written) + " bytes)");
  } else {
    Serial.println("ERROR: Send failed - endPacket (result: " + String(endResult) + ")");
  }
}
