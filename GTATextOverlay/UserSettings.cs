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

using System.Collections.Generic;
using System.IO;

public class UserSettings
{
    public string Nickname { get; set; } = "Player";
    public int ServerIndex { get; set; } = 0;
    public int MessageDurationSeconds { get; set; } = 300;
    public int FadeDurationSeconds { get; set; } = 10;

    public static string SettingsPath => "settings.ini";

    public void Save()
    {
        var lines = new List<string>
        {
            "[Settings]",
            $"nickname={Nickname}",
            $"server={ServerIndex}",
            $"messageDuration={MessageDurationSeconds}",
            $"fadeDuration={FadeDurationSeconds}"
        };
        File.WriteAllLines(SettingsPath, lines);
    }

    public static UserSettings Load()
    {
        var settings = new UserSettings();

        if (!File.Exists(SettingsPath))
        {
            settings.Save();
            return settings;
        }

        foreach (var line in File.ReadAllLines(SettingsPath))
        {
            if (line.StartsWith("nickname="))
                settings.Nickname = line.Substring("nickname=".Length);
            else if (line.StartsWith("server="))
                settings.ServerIndex = int.Parse(line.Substring("server=".Length));
            else if (line.StartsWith("messageDuration="))
                settings.MessageDurationSeconds = int.Parse(line.Substring("messageDuration=".Length));
            else if (line.StartsWith("fadeDuration="))
                settings.FadeDurationSeconds = int.Parse(line.Substring("fadeDuration=".Length));
        }

        return settings;
    }
}