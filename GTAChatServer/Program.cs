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

using Fleck;
using System;
using System.Collections.Generic;

namespace GTAChatServer
{
    class Program
    {
        static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();

        static void Main(string[] args)
        {
            FleckLog.Level = LogLevel.Info;

            var server = new WebSocketServer("ws://0.0.0.0:8181");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    string ip = socket.ConnectionInfo.ClientIpAddress;
                    string host = socket.ConnectionInfo.Host;
                    int port = socket.ConnectionInfo.ClientPort;
                    Console.WriteLine($"Client connected from ->\n`- {ip}({host}):{port}");
                    allSockets.Add(socket);
                };

                socket.OnClose = () =>
                {
                    string ip = socket.ConnectionInfo.ClientIpAddress;
                    string host = socket.ConnectionInfo.Host;
                    int port = socket.ConnectionInfo.ClientPort;
                    Console.WriteLine($"Client disconnected from ->\\n`- {{ip}}({{host}}):{{port}}\"");
                    allSockets.Remove(socket);
                };

                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);

                    foreach (var s in allSockets)
                    {
                        if (s.IsAvailable)
                            s.Send(message);
                    }
                };
            });

            string publicIP = GetPublicIPAddress();
            Console.WriteLine($"Server running on ws://{publicIP}:8181");
            Console.ReadLine();
        }

        static string GetPublicIPAddress()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    return client.DownloadString("https://api.ipify.org").Trim();
                }
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}