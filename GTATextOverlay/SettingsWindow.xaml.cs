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

using System.Windows;

namespace GTATextOverlay
{
    /// <summary>
    /// Allows the user to configure chat overlay settings, including name, 
    /// server location, and message display durations.
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly UserSettings _settings;

        /// <summary>
        /// Initializes the settings window with the current user settings.
        /// Populates all fields with existing values.
        /// </summary>
        /// <param name="settings">The loaded user settings.</param>
        public SettingsWindow(UserSettings settings)
        {
            InitializeComponent();
            _settings = settings;

            NicknameBox.Text = _settings.Nickname;
            DisplayDurationBox.Text = _settings.MessageDurationSeconds.ToString();
            FadeDurationBox.Text = _settings.FadeDurationSeconds.ToString();

            ServerDropdown.ItemsSource = ServerList.Servers;
            ServerDropdown.SelectedIndex = _settings.ServerIndex;
        }

        /// <summary>
        /// Called when the Save & Close button is clicked.
        /// Reads values from the UI, updates the settings, saves to ini, and closes the window.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var settings = new UserSettings
            {
                Nickname = NicknameBox.Text,
                ServerIndex = ServerDropdown.SelectedIndex,
                MessageDurationSeconds = int.TryParse(DisplayDurationBox.Text, out var dur) ? dur : 300,
                FadeDurationSeconds = int.TryParse(FadeDurationBox.Text, out var fade) ? fade : 10
            };

            settings.Save();
            this.Close();
        }
    }
}
