# Desert Maze – Arduino Setup (Homework 4 & 5)

This guide explains how to use **two Arduinos** with the **homework 4 and 5** desert maze scene: one **wired** (joystick to move the water ball) and one **wireless** (LED + buzzer).

**Unity is written to match your existing Arduino script.** No Arduino code changes are required. If you can’t upload to the ESP32 (e.g. USB issues), you can still play the game using **Simulate Arduino** in Unity (see below).

---

## Overview

| Arduino | Connection | Role |
|--------|------------|------|
| **1. Wired** | USB to computer | Joystick acts as **mouse** → moves the water ball in Unity |
| **2. Wireless (ESP32)** | WiFi (UDP) to same network as computer | **LED** on while game is running; **buzzer** when water touches the fountain |

---

## 1. Wired Arduino (Joystick → Mouse)

- You already have code that makes the **joystick control the mouse**.
- Unity reads **Mouse X** and **Mouse Y** to move the water ball, so no change is needed on this Arduino.
- Ensure the joystick is plugged in and moving the system cursor when you run the game.

**If you don’t have joystick-as-mouse code yet:**  
Use the Arduino **Mouse** library (e.g. on Leonardo/Pro Micro) or a sketch that reads the joystick and sends relative mouse movement. The important part is that the OS sees **mouse movement**; Unity will use it automatically.

---

## 2. Wireless Arduino (ESP32 – LED + Buzzer)

### Hardware

- **LED** on pin **2** (on when the game is running).
- **Buzzer** on pin **4** (change `buzzerPin` in the sketch if you use another pin).
- **Active buzzer:** `digitalWrite(buzzerPin, HIGH)` is enough.
- **Passive piezo:** For a clear tone, you can use `tone(buzzerPin, 2000, 500);` in the `"b"` branch and turn off with `noTone(buzzerPin)` after a delay.

### Code (use your existing sketch — no changes required)

Your Arduino script already does the right thing:

- **Receives** UDP on port **5005** (`udpReceiverPort`).
- **Responds to:** `"c"` = LED on, `"d"` = LED off, `"b"` = buzzer.
- Optionally **sends** to your Mac at `udpAddress` port **5006** (Unity can ignore this for the game).

Unity sends to the **ESP32’s IP** on port **5005**. Set **UDPSender** in Unity: **IP** = ESP32’s IP, **Port** = **5005**.

### Finding the ESP32’s IP

- Open the **Arduino Serial Monitor** (9600 baud) after uploading. The sketch prints the ESP32’s IP (e.g. `192.168.1.xxx`).
- Use that IP in Unity (see below).

---

## 3. Unity Scene Setup (Homework 4 and 5)

### A. Objects to add

1. **Game manager (for UDP to wireless Arduino)**  
   - Create an **empty GameObject** (e.g. name: `DesertMazeGameManager`).  
   - Add **DesertMazeGameManager** script.  
   - Add **UDPSender** component (or use an existing one in the scene).  
   - On **UDPSender**:  
     - **IP** = your **ESP32’s IP** (e.g. `192.168.1.xxx`).  
     - **Sender Port** = **5005** (same as `udpReceiverPort` in the Arduino sketch).  
   - On **DesertMazeGameManager**:  
     - Assign the **UDPSender** reference.  
     - Leave **Auto Start Udp** checked so it sends **"c"** (LED on) when Play is pressed.

2. **Water ball**  
   - Select the **water** GameObject.  
   - Add **DesertMazeWaterController** (a **Rigidbody** will be added automatically if missing).  
   - Adjust **Move Force**, **Sensitivity**, and **Max Speed** to taste.  
   - **Movement Plane**: use **XZ** for a top-down style maze, **XY** for a side view.  
   - **Fountain Object Name** should be `fountain` (default); change only if your fountain has another name.

### B. Fountain collision

- The script detects when the **water** object (with a **Collider**) hits something named **fountain** (or a child of an object named **fountain**).
- The **fountain** (or a child) must have a **Collider** (trigger or non-trigger). If the fountain is only in a child, the child’s name can be different as long as a parent is named `fountain`, or you can set **Fountain Object Name** to the exact name you use.

### C. Summary

- **DesertMazeGameManager** (with **UDPSender**):  
  - Sends **"c"** when the game starts → ESP32 turns **LED on**.  
  - Sends **"d"** when the game stops → ESP32 turns **LED off**.  
  - When the water reaches the fountain, **DesertMazeWaterController** calls the manager, which sends **"b"** → ESP32 turns **buzzer on** for 2 seconds.

- **DesertMazeWaterController** on **water**:  
  - Moves the ball with **Mouse X / Mouse Y** (your wired joystick-as-mouse).  
  - On collision with **fountain**, triggers the buzzer via the manager.

---

## 4. Playing without the Arduino (Simulate mode)

If the ESP32 isn’t connected or you can’t upload code (e.g. USB port issues):

1. Select the GameObject that has **DesertMazeGameManager**.
2. In the Inspector, enable **Simulate Arduino**.
3. The game runs as normal; no UDP is sent. The Console will log what would be sent (`"c"`, `"d"`, `"b"`). You can still move the ball (mouse/joystick) and “win” at the fountain.

---

## 5. Quick checklist

- [ ] Wired Arduino: joystick controlling the mouse; no Unity code change needed.  
- [ ] ESP32: same WiFi as computer; your existing sketch is fine (receives on 5005; LED pin 2, buzzer pin 4).  
- [ ] Unity: **UDPSender** IP = ESP32 IP, port = **5005**.  
- [ ] Unity: **DesertMazeGameManager** on an empty GameObject with **UDPSender** assigned (or use **Simulate Arduino** if the ESP32 isn’t available).  
- [ ] Unity: **DesertMazeWaterController** on the **water** object; **fountain** has a collider and name `fountain` (or matching **Fountain Object Name**).

After that, pressing Play in Unity should turn the LED on (or log in Simulate mode), moving the joystick should move the water ball, and reaching the fountain should trigger the buzzer (or log in Simulate mode).
