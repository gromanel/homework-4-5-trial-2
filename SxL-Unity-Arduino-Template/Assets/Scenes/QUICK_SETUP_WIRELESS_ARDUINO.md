# Quick setup: Wireless Arduino (ESP32) LED + buzzer

You **do not need to drag-and-drop** anything. The game manager finds the UDPSender automatically.

## Steps (in "homework 4 and 5" scene)

### 1. One GameObject with both scripts

- In the **Hierarchy**, select the GameObject that has **DesertMaze Game Manager** (or create an empty GameObject and add it).
- With that object selected, in the **Inspector** click **Add Component**.
- Search for **UDPSender** and add it.

You should now see on the **same** GameObject:
- **Desert Maze Game Manager** (script)
- **UDP Sender** (script)

No assignment needed: the manager finds the UDPSender on the same object.

### 2. Set the ESP32’s IP and port

Still on that GameObject, in the **UDP Sender** component:

- **IP** = your ESP32’s IP address (e.g. `192.168.1.xxx`).  
  Get it from the Arduino Serial Monitor when the ESP32 boots (it prints “Arduino IP address: …”).
- **Sender Port** = **5005** (must be 5005 to match the Arduino).

Leave **Simulate Arduino** unchecked on the Desert Maze Game Manager.

### 3. Press Play

When you enter Play mode, the manager will send `"c"` to the ESP32 and the LED should turn on. In the Console you should see something like:

`[DesertMaze] Sent 'c' (LED on) to 192.168.1.xxx:5005 ...`

If the LED still doesn’t turn on:

- Confirm the ESP32 is on the **same WiFi network** as your computer.
- Confirm **IP** in UDPSender is exactly the ESP32’s IP (from Serial Monitor).
- Confirm **Sender Port** is **5005**.
- Check the ESP32 Serial Monitor: when Unity sends, it should print “← Received: 'c' …”.

### If you don’t have an ESP32 / want to test without it

On **Desert Maze Game Manager**, check **Simulate Arduino**. The game will run and the Console will log what would be sent; no UDP is used.
