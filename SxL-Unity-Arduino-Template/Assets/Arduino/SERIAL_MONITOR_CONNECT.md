# Serial Monitor Says "Not Connected" (Board and Port Are Selected)

If upload works but Serial Monitor says it’s not connected or asks you to select a board and port, try these in order.

---

## 1. Close Serial Monitor, then upload again

- Close the **Serial Monitor** window completely.
- Click **Upload** and wait until you see “Done uploading”.
- Wait **2–3 seconds** (ESP32 resets after upload).
- Open **Serial Monitor** again (Tools → Serial Monitor or Ctrl+Shift+M / Cmd+Shift+M).
- Set baud rate to **9600** (dropdown in Serial Monitor).

Often the port is busy right after upload; closing and reopening Serial Monitor fixes it.

---

## 2. Re-select the port after upload

- After a successful upload, go to **Tools → Port**.
- If your port is already selected, choose **a different port** once, then switch **back** to the correct one (e.g. `/dev/cu.usbserial-0001` or `COM3`).
- Open **Serial Monitor** (9600 baud).

On Mac, the port can disappear and reappear after reset; re-selecting it helps the IDE “see” it again.

---

## 3. Confirm the correct port (Mac)

- Unplug the ESP32, then **Tools → Port**. Note which port(s) are listed.
- Plug the ESP32 back in. Wait a few seconds.
- **Tools → Port** again. The **new** port that appeared is usually the ESP32 (e.g. `/dev/cu.usbserial-xxxx` or `/dev/cu.SLAB_USBtoUART`).
- Select that port, then open Serial Monitor at **9600**.

---

## 4. Confirm the correct port (Windows)

- In **Device Manager** (under “Ports (COM & LPT)”), see which COM port appears when you plug in the ESP32 (e.g. “USB-SERIAL CH340 (COM3)”).
- In Arduino IDE: **Tools → Port** → select that COM port.
- Open Serial Monitor at **9600**.

---

## 5. Board and baud rate

- **Tools → Board** → choose your ESP32 board (e.g. **ESP32 Arduino → NodeMCU-32S**).
- In Serial Monitor, set the baud rate to **9600** (must match `Serial.begin(9600)` in the sketch).

---

## 6. One app at a time

- Don’t open the same serial port in two places (e.g. Arduino Serial Monitor and another terminal app).
- Close any other program that might be using the port (PuTTY, screen, etc.), then try Serial Monitor again.

---

## 7. Cable and USB

- Use a **data** USB cable (some cables are charge-only and don’t provide a serial connection).
- Try another USB port, preferably directly on the computer (not through a hub).
- If you have another USB cable, try it.

---

**Quick sequence that often works:** Close Serial Monitor → Upload → wait 2–3 seconds → Tools → Port → select the ESP32 port → Open Serial Monitor → 9600 baud.
