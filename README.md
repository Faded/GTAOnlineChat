# GTA Overlay Chat Client

A lightweight, transparent, in-game text chat overlay for **GTA V Online** (or any game), inspired by Rockstar's now removed in-game chat.

This project allows friends to run a standalone chat app that mimics GTA V's UI style without injecting into the game. It uses WebSockets for communication and runs externally as a transparent overlay. 

This project is in its most basic form and you are free to modify/use at your pleasure. More updates to follow on it and pull requests are taken in to consideration.

---

## 📸 Features

- ✅ Transparent WPF overlay that works in fullscreen/borderless games  
- ✅ Global hotkeys to open chat, open settings, and quit  
- ✅ Fade-in/out message display with customizable timing  
- ✅ Simple, no-port-forwarding WebSocket server  
- ✅ Configurable nickname, message timing, and server list  
- ✅ Saves settings to a local `.ini` file  
- ✅ External networking support (not LAN-only)  

---

## 🛠 Requirements

### Client (Overlay App)
- .NET 6 SDK
- [WebSocketSharp](https://github.com/sta/websocket-sharp)
- [Gma.System.MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook)

### Server (WebSocket Server)
- .NET 6 Console App
- [Fleck WebSocket Server](https://github.com/statianzo/Fleck)

---

# 💬 Usage
### tl;dr -> Build the server, run it where you want to host it. Edit ServerList.cs and change ServerInfo to the external IP of your server.
---
## 🔧 1. Run the Server
```
cd GTAChatServer
dotnet run
```
This will start the WebSocket server at:
```
ws://0.0.0.0:8181
```
You can add more server locations in ServerList.cs.

## 🖥️ 2. Run the Client
Open the GTATextOverlay project in Visual Studio and run.
```
Default Hotkeys
Key Combo	Action
Ctrl + Y	Open Chat Input
Ctrl + Alt + Y	Open Settings Window
Ctrl + K	Quit App Immediately
```

## ⚙ Settings
Use Ctrl + Alt + Y to open the settings window.

Settings include:
```
Nickname

Server Location (editable in ServerList.cs)

Message Display Duration

Fade-Out Duration
```

These settings are saved to:
```
%APPDATA%\GTAChatOverlay\settings.ini
```

## ✍ Customization
Add or Change Server IPs
Edit ServerList.cs:
```
public static readonly List<ServerInfo> Servers = new List<ServerInfo>
{
    new ServerInfo("Phoenix, AZ", "ws://<ip>:8181"),
    new ServerInfo("England, UK", "ws://<ip>:8181")
};
```

### Adjust Chat Style
Edit the AddChatMessage() method in MainWindow.xaml.cs:
```
FontSize = 16,
Foreground = Brushes.White,
TextWrapping = TextWrapping.Wrap,
MaxWidth = 300,
```

## ⚠ Notes
Run the client as administrator to allow hotkeys in protected apps (like Steam, Discord, GTA, etc).

Does not inject or modify GTA in any way — 100% external overlay.

Intended for personal/private use with friends.

## 📄 License
This project is licensed under the GNU General Public License v2.0.
See the LICENSE file for more information.

## 🙌 Credits
Made by HypeFawx (LittyGames.net)
Inspired by Rockstar's original GTA V Online chat UI.

---