# **DualAudioShare**

DualAudioShare is a lightweight Windows desktop app that lets you **mirror audio from one playback device to another** – for example, listen to the same content on **two pairs of headphones** (Bluetooth, USB, jack, etc.) at once.

It’s inspired by “sound sharing” features found in OEM audio suites (like Nahimic), but implemented as an open-source **C# / .NET / WPF** application.

> ⚠️ This project is **not** affiliated with or endorsed by Nahimic, A-Volute, MSI, or any other vendor. It’s an independent learning/open-source tool.

* * *

## **✨ Demo**

<figure class="image"><img style="aspect-ratio:819/503;" src="DualShare.md_image.png" width="819" height="503"></figure>

* * *

## **✨ Features**

*   🎧 **Dual Device Sharing**  
    Select a **source device** (your main output) and a **mirror device** (second output). Audio from the source is mirrored to the second device.
*   🎚 **Per-Device Volume Control**
    *   Control **system volume** for the source device.
    *   Control **mirror volume** independently inside the app.
*   🌓 **Modern Dark UI**
    *   Clean, minimal layout inspired by gaming audio apps.
    *   Built using **WPF** + **ModernWpf** for a polished look.
*   🪟 **Pure User-Space Implementation**  
    Uses **WASAPI loopback** via **NAudio** – no custom drivers or kernel components required.

* * *

## **🧱 Tech Stack**

*   **Language:** C#
*   **Framework:** .NET (e.g., .NET 8)
*   **UI:** WPF + [ModernWpf](https://github.com/Kinnara/ModernWpf)
*   **Audio:** [NAudio](https://github.com/naudio/NAudio) (WASAPI loopback + playback)

* * *

## **🚀 Getting Started**

### **Prerequisites**

*   **Windows 10 / 11**
*   **.NET SDK** (e.g., .NET 8)
*   **Visual Studio 2022** (or later) with:
    *   .NET desktop development workload

### **Clone the Repository**

```
git clone https://github.com/HarshitRaja1999/DualAudioShare.git
cd DualAudioShare
```

### Restore & Build

1.  Open `DualAudioShare.sln` in Visual Studio.
2.  Visual Studio will restore NuGet packages automatically (NAudio, ModernWPF).
3.  Set build configuration to **Release** or **Debug**.
4.  Press **F5** (Run) or **Ctrl+ Shift+ B** (Build).

## 📖 Usage

1.  **Launch the app.**
2.  In the main window:
    *   **Device 1 – Source device**  
        Select the playback device that is **already receiving system audio** (e.g., your main headphones/speakers).
    *   **Device 2 – Mirror device**  
        Select the second playback device you want to share the audio with.
3.  Adjust:
    *   **Source Volume** – changes the **system volume** of the source playback device.
    *   **Mirror Volume** – changes only the **mirrored output** volume from the app.
4.  Click **“Start Sharing”**.
    *   The status text will show something like:  
        `Sharing audio from "<Source>" to "<Mirror>"...`
5.  To stop, click **“Stop Sharing”**.

> 💡 Tip: This feature is optimised for **headphones** (wired or Bluetooth), but can also be used with other playback devices.

* * *

## ⚠️ Limitations

*   **Windows-only** (WASAPI-based).
*   **Some latency** between the main and mirrored device (user-space loopback, not a driver).
*   Source & mirror devices **must be different** – mirroring a device to itself is blocked.
*   Relies on devices being active and enabled in **Windows Sound Settings**.

* * *

## 🗺 Roadmap / Future Goals

Planned enhancements and ideas:

*   🔁 **Multi-Device Mirroring**
    *   Current version: **1 source → 1 mirror device**.
    *   Future goal: **1 source → N mirror devices** (e.g., share audio to 2–3 headphones/speakers at once).
*   ⏱ **Latency & Buffer Controls**
    *   Expose buffer size / latency presets (Low / Normal / Safe) for smoother playback.
*   🧠 **Profiles & Presets**
    *   Save preferred device pairs and volume settings as profiles.
*   🧰 **Tray Mode & Auto-Start**
    *   Minimise to system tray.
    *   Optional “start with Windows” + auto-start sharing with last used devices.
*   🎚 **Per-Device Effects (Stretch Goal)**
    *   Simple EQ / limiter for mirrored output.

If you’re interested in contributing, check out the **Contributing** section below.

* * *

## 🤝 Contributing

Contributions, bug reports, and feature requests are welcome!

1.  Fork the repo
2.  Create a feature branch:  
    `git checkout -b feature/my-new-feature`
3.  Commit your changes:  
    `git commit -m "Add my new feature"`
4.  Push to the branch:  
    `git push origin feature/my-new-feature`
5.  Open a Pull Request

Please try to keep code style consistent and include a short description of what you changed and why.

* * *

## 📄 License – MIT

This project is licensed under the **MIT License**.

## Acknowledgements

*   [NAudio](https://github.com/naudio/NAudio) for making Windows audio APIs nice to work with in .NET.
*   [ModernWpf](https://github.com/Kinnara/ModernWpf) for the modern Fluent-style UI.
*   Inspiration from OEM audio suites that offer “sound sharing” / dual audio output features.