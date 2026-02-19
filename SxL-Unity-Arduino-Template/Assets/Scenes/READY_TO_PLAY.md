# You're Ready to Play — Desert Maze with Both Arduinos

Both Arduinos are working. Here’s how to run the full game.

---

## Before you press Play

**In Unity (scene: "homework 4 and 5"):**

1. **One GameObject** has:
   - **Desert Maze Game Manager** (script)
   - **UDP Sender** (script)
2. **UDP Sender** settings:
   - **IP** = your ESP32’s IP (from Serial Monitor: “Arduino IP address: …”)
   - **Sender Port** = **5005**
3. **Desert Maze Game Manager**:
   - **Simulate Arduino** = **unchecked**
4. **Water** object has the **Desert Maze Water Controller** script.

---

## What happens when you play

| When | Wired Arduino (joystick) | Wireless Arduino (ESP32) |
|------|---------------------------|---------------------------|
| You press **Play** | — | LED turns **on** (Unity sends `"c"`) |
| You move the joystick | Ball moves in the maze (joystick = mouse) | — |
| Ball touches the **fountain** | — | **Buzzer** sounds for ~2 seconds (Unity sends `"b"`) |
| You stop **Play** | — | LED turns **off** (Unity sends `"d"`) |

---

## Quick test

1. Open the **"homework 4 and 5"** scene.
2. Press **Play**.
3. Check the ESP32: the **LED should be on**.
4. Move the **joystick**: the **water ball** should move.
5. Roll the ball to the **fountain**: the **buzzer** should go off.
6. Stop Play: the **LED** should turn off.

---

## If something doesn’t work

- **LED doesn’t turn on when you press Play** → Check UDP Sender IP (ESP32’s IP) and port 5005. Same WiFi for Mac and ESP32.
- **Ball doesn’t move** → Joystick should be moving the system cursor; focus the Game view (click inside it) so Unity gets mouse input.
- **Buzzer doesn’t go off at the fountain** → Make sure the fountain has a collider and that its name (or a parent’s name) is `fountain` (or matches the “Fountain Object Name” field on the water controller).

That’s it — you’re set up. Have fun with the maze.
