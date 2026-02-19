# Troubleshooting: LED and Buzzer Not Working

Follow these steps in order. They tell you whether the problem is hardware, Arduino, network, or Unity.

---

## Step 1: Confirm hardware (LED + buzzer) work

1. Upload the updated **UDPReadWriteChars** sketch to the ESP32 (it now has a short boot test).
2. Open **Serial Monitor** (9600 baud) and reset the board.
3. You should see: `Output test done. LED and buzzer should have flashed/beeped.`
4. **Did the LED flash and the buzzer beep once?**
   - **No** → Check wiring. LED: long leg (+) to pin 2, short leg (-) to GND (use a 220Ω–330Ω resistor). Buzzer: positive to pin 4, negative to GND. Then re-upload and test again.
   - **Yes** → Outputs are fine. Go to Step 2.

---

## Step 2: Confirm ESP32 receives UDP from Unity

1. Keep **Serial Monitor** open (9600 baud).
2. In Unity, set **UDP Sender**: **IP** = your ESP32’s IP (from Serial Monitor at boot: “Arduino IP address: 192.168.x.x”), **Sender Port** = **5005**.
3. Press **Play** in Unity.
4. Watch the Serial Monitor. When Unity sends, you should see a line like:  
   `← Received: 'c' from 172.168.x.x:xxxxx`

**Do you see “← Received” when you press Play?**
- **No** → The ESP32 is not getting the packet. Go to Step 3.
- **Yes** → The ESP32 is receiving. If the LED still doesn’t turn on, re-upload the sketch (it now uses the first character only, so "c" is always recognized). Then test again. If it still doesn’t work, check that **Simulate Arduino** is unchecked on the Desert Maze Game Manager.

---

## Step 3: Fix Unity → ESP32 (no “← Received” on ESP32)

If the ESP32 never prints “← Received” when you press Play:

1. **Same WiFi**  
   Mac and ESP32 must be on the same network (e.g. same NETGEAR39).

2. **Correct IP in Unity**  
   In the Inspector, on the **UDP Sender** component, **IP** must be exactly the ESP32’s IP (the one printed in Serial Monitor as “Arduino IP address: …”). Not your Mac’s IP, not 127.0.0.1.

3. **Port 5005**  
   **Sender Port** must be **5005** (same as `udpReceiverPort` in the Arduino sketch).

4. **Firewall (Mac)**  
   System Settings → Network → Firewall (or Security & Privacy → Firewall). Temporarily turn it off or allow Unity/your build. Sometimes the firewall blocks outgoing UDP.

5. **Unity Console**  
   When you press Play you should see:  
   `[DesertMaze] Sent 'c' (LED on) to 192.168.x.x:5005`  
   If you see that but the ESP32 still doesn’t print “← Received”, the packet is not reaching the ESP32 (wrong IP, different network, or firewall).

---

## Step 4: Quick recap

| What you see | What to do |
|--------------|------------|
| No LED/beep at boot | Fix wiring and re-upload (Step 1). |
| LED/beep at boot, no “← Received” when Play | Fix IP, port, WiFi, firewall (Step 3). |
| “← Received: 'c'” but LED doesn’t turn on | Re-upload sketch (first-char fix). Uncheck Simulate Arduino. |
| “← Received” and LED on when Play, buzzer not on win | Move ball to fountain; buzzer triggers on "b". Check fountain name/collider. |

After any change, upload the sketch again, then test Unity (Play, then reach fountain).
