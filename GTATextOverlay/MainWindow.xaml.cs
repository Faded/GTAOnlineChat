// ---------------------------------------------------------------------------------------------
// Project: GTA Overlay Chat
// Description: Simple p2p text chat using WebSocket to overlay on GTA5 Online (or any game)
// Author: HypeFawx (LittyGames.net)
// License: GNU General Public License v2.0 (GPL-2.0)

// Rockstar's Policy: This project does not infringe on Rockstar's online modding policy. It's only
// an application that overlays a text chat. It is the same thing as the Discord and Steam overlays.
// ---------------------------------------------------------------------------------------------
//
// This file is part of the GTA Overlay Chat Client.
// 
// GTA Overlay Chat Client is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
//
// GTA Overlay Chat Client is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using WebSocketSharp;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows.Media;
using Gma.System.MouseKeyHook;
using System.Windows.Media.Animation;

namespace GTATextOverlay
{
    /// <summary>
    /// MainWindow is the core overlay window for the GTA Chat Client.
    /// It connects to the selected WebSocket server, displays incoming messages with fade effects,
    /// handles global hotkeys for showing the chat input, quitting the app, and opening the settings window.
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserSettings _settings;
        private WebSocket _wsClient;

        private string _nickname;
        private List<string> _messageHistory = new List<string>();

        private IKeyboardMouseEvents _globalHook;
        private IntPtr _lastFocusedWindow = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();

            _settings = UserSettings.Load();
            _nickname = _settings.Nickname;

            WindowState = WindowState.Maximized;
            ConnectToServer();

            Loaded += MakeClickThrough;
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        /// <summary>
        /// Establishes the WebSocket connection and registers message/close handlers.
        /// </summary>
        private void ConnectToServer()
        {
            _wsClient = new WebSocket(ServerList.Servers[_settings.ServerIndex].Address);
            _wsClient.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Connected to server");
            };

            _wsClient.OnMessage += (sender, e) =>
            {
                var messageData = JsonSerializer.Deserialize<Dictionary<string, string>>(e.Data);
                string message = messageData["message"];
                Dispatcher.Invoke(() => AddChatMessage(message));
            };

            _wsClient.OnClose += (sender, e) =>
            {
                Console.WriteLine("Disconnected from server. Attempting to reconnect...");
                ReconnectToServer();
            };

            _wsClient.Connect();
        }

        private void ReconnectToServer()
        {
            var reconnectTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            reconnectTimer.Tick += (s, e) =>
            {
                if (!_wsClient.IsAlive)
                {
                    Console.WriteLine("Reconnecting...");
                    _wsClient.Connect();
                }
            };
            reconnectTimer.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetupGlobalKeyHook();
        }

        /// <summary>
        /// Hooks into global key events using a low level listener.
        /// </summary>
        private void SetupGlobalKeyHook()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalKeyDown;
        }

        /// <summary>
        /// Handles global hotkeys:
        /// - Ctrl+Y: open chat
        /// - Ctrl+Alt+Y: open settings
        /// - Ctrl+K: quit application
        /// </summary>
        private void GlobalKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Y && e.Control && ChatInput.Visibility != Visibility.Visible)
            {
                Dispatcher.Invoke(() =>
                {
                    _lastFocusedWindow = GetForegroundWindow();
                    ChatInput.Visibility = Visibility.Visible;
                    ChatStack.Children.Clear();

                    var historyCopy = new List<string>(_messageHistory);
                    foreach (var message in historyCopy)
                    {
                        AddChatMessage(message);
                    }

                    ForceFocus();
                    ChatInput.Focus();

                });
                e.Handled = true;
            }

            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.K)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }

            if (e.Control && e.Alt && e.KeyCode == System.Windows.Forms.Keys.Y)
            {
                Dispatcher.Invoke(() =>
                {
                    var settingsWindow = new SettingsWindow(_settings);
                    settingsWindow.Owner = this;
                    settingsWindow.ShowDialog();

                    _settings = UserSettings.Load();
                    _nickname = _settings.Nickname;
                });
            }
        }

        /// <summary>
        /// Fallback key handler for app focus. Can open chat with Ctrl+Y inside the app itself.
        /// </summary>
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ChatInput.Visibility == Visibility.Visible)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Y)
            {
                ChatInput.Visibility = Visibility.Visible;
                ChatInput.Focus();
                ChatStack.Children.Clear();

                var historyCopy = new List<string>(_messageHistory);
                foreach (var message in historyCopy)
                {
                    AddChatMessage(message);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles Enter or Escape inside the chat input. Sends message or cancels input.
        /// </summary>
        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(ChatInput.Text))
                {
                    string time = DateTime.Now.ToShortTimeString();
                    string formatted = $"[{time}] {_nickname}: {ChatInput.Text}";

                    var message = new { nickname = _nickname, message = formatted };
                    _wsClient.Send(JsonSerializer.Serialize(message));

                    ChatInput.Text = string.Empty;

                    if (_lastFocusedWindow != IntPtr.Zero)
                    {
                        SetForegroundWindow(_lastFocusedWindow);
                        _lastFocusedWindow = IntPtr.Zero;
                    }
                }

                ChatInput.Visibility = Visibility.Collapsed;
            }
            else if (e.Key == Key.Escape)
            {
                ChatInput.Text = string.Empty;
                ChatInput.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Displays a new chat message and triggers fade-out timers.
        /// </summary>
        private void AddChatMessage(string message)
        {
            _messageHistory.Add(message);
            if (_messageHistory.Count > 10)
                _messageHistory.RemoveAt(0);

            var text = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 16,
                Margin = new Thickness(0, 2, 0, 2),
                Opacity = 1,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300 
            };

            ChatStack.Children.Add(text);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();

                var fadeOut = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_settings.MessageDurationSeconds) };
                fadeOut.Tick += (fs, fe) =>
                {
                    fadeOut.Stop();
                    var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(_settings.FadeDurationSeconds));
                    text.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
                };
                fadeOut.Start();

            };
            timer.Start();
        }


        protected override void OnClosed(EventArgs e)
        {
            _globalHook.KeyDown -= GlobalKeyDown;
            _globalHook.Dispose();
            base.OnClosed(e);
        }

        /// <summary>
        /// Brings the overlay window to the front to allow text input.
        /// </summary>
        private void ForceFocus()
        {
            var hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var foregroundWindow = GetForegroundWindow();

            uint thisThread = GetCurrentThreadId();
            uint foregroundThread = GetWindowThreadProcessId(foregroundWindow, out _);

            AttachThreadInput(foregroundThread, thisThread, true);
            ShowWindow(hWnd, 5);
            SetForegroundWindow(hWnd);
            AttachThreadInput(foregroundThread, thisThread, false);
        }

        /// <summary>
        /// Applies transparent click-through style to the window so it doesn't block mouse input.
        /// </summary>
        private void MakeClickThrough(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE);
        }

        // WinAPI imports and constants
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}