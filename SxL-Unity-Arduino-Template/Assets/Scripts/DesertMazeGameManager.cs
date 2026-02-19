using UnityEngine;

/// <summary>
/// Unity side of the desert maze + ESP32 protocol (no Arduino code changes needed).
/// Matches the Arduino script that receives on port 5005 and responds to:
///   "c" = LED on (game running), "d" = LED off (game stopped), "b" = buzzer (water reached fountain).
/// Unity must send to: ESP32's IP address, port 5005 (udpReceiverPort on Arduino).
/// </summary>
public class DesertMazeGameManager : MonoBehaviour
{
    public static DesertMazeGameManager Instance { get; private set; }

    [Header("Wireless Arduino (ESP32) UDP")]
    [Tooltip("UDPSender used to send to ESP32. If not set, will try to find one in scene.")]
    public UDPSender udpSender;
    [Tooltip("Start UDP sender when game starts. Set UDPSender IP = ESP32 IP, Port = 5005.")]
    public bool autoStartUdp = true;

    [Header("When Arduino is not connected (e.g. USB issues)")]
    [Tooltip("If true: no UDP is sent; game still runs and logs what would be sent. Use when you can't use the Arduino.")]
    public bool simulateArduino = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // No need to drag-and-drop: find UDPSender on this GameObject first, then anywhere in scene
        if (udpSender == null)
            udpSender = GetComponent<UDPSender>();
        if (udpSender == null)
            udpSender = FindObjectOfType<UDPSender>();
    }

    private void Start()
    {
        if (simulateArduino)
        {
            Debug.Log("[DesertMaze] Simulate Arduino ON — no UDP sent. Game works without the ESP32.");
            LogArduinoCommand("c");
            return;
        }

        if (udpSender == null)
        {
            Debug.LogWarning("[DesertMaze] No UDPSender found. Add a UDPSender component to this same GameObject (or any in the scene). Enable 'Simulate Arduino' to play without the ESP32.");
            return;
        }

        // UDPSender must have IP = ESP32's IP and Port = 5005 (see Inspector)
        if (autoStartUdp)
            udpSender.startSenderThread();

        // Arduino expects "c" = LED on (game running)
        SendToArduino("c");
        Debug.Log("[DesertMaze] Sent 'c' (LED on) to " + udpSender.IP + ":" + udpSender.senderPort + ". If the LED did not turn on, check that IP is your ESP32's IP and Port is 5005.");
    }

    /// <summary>Send a command to the Arduino. Protocol: "c" = LED on, "d" = LED off, "b" = buzzer.</summary>
    public void SendToArduino(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (simulateArduino)
        {
            LogArduinoCommand(message);
            return;
        }

        if (udpSender != null)
            udpSender.sendMessage(message);
    }

    private void LogArduinoCommand(string message)
    {
        string meaning = message switch
        {
            "c" => "LED ON (game running)",
            "d" => "LED OFF (game stopped)",
            "b" => "BUZZER (water reached fountain)",
            _ => "command"
        };
        Debug.Log("[DesertMaze] Would send to Arduino: '" + message + "' (" + meaning + ")");
    }

    /// <summary>Called when the water ball reaches the fountain. Sends "b" to trigger the buzzer.</summary>
    public void SendBuzzer()
    {
        SendToArduino("b");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (!simulateArduino && udpSender != null)
            udpSender.sendMessage("d");
    }

    private void OnApplicationQuit()
    {
        if (!simulateArduino && udpSender != null)
            udpSender.sendMessage("d");
    }
}
