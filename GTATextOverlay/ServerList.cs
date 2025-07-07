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

/// <summary>
/// Holds a static list of predefined WebSocket server locations for the chat client.
/// These are selectable by the user in the settings window. Each server would be it's own chat instance.
/// </summary>
public static class ServerList
{
    /// <summary>
    /// The list of available server locations and their corresponding WebSocket addresses.
    /// The server locations can either be local or external, depending where the server is ran.
    /// </summary>
    public static readonly List<ServerInfo> Servers = new List<ServerInfo>
    {
        new ServerInfo("Phoenix, AZ", "ws://127.0.0.1:8181")
        // Add more
        // new ServerInfo("Some, Place", "ws://1.2.3.4:8181")
    };
}