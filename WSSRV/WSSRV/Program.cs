/*
 * Created by SharpDevelop.
 * User: Edward
 * Date: 3/20/2016
 * Time: 1:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Fleck;

namespace WSSRV
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(@" __          _______  _____ ______ _  __          _   _ _  ________ _____  ");  
			Console.WriteLine(@" \ \        / / ____|/ ____|  ____| |/ /    /\   | \ | | |/ /  ____|  __ \ ");
			Console.WriteLine(@"  \ \  /\  / / (___ | |  __| |__  | ' /    /  \  |  \| | ' /| |__  | |__) |");
			Console.WriteLine(@"   \ \/  \/ / \___ \| | |_ |  __| |  <    / /\ \ | . ` |  < |  __| |  _  / ");
			Console.WriteLine(@"    \  /\  /  ____) | |__| | |____| . \  / ____ \| |\  | . \| |____| | \ \ ");
			Console.WriteLine(@"     \/  \/  |_____/ \_____|______|_|\_\/_/    \_\_| \_|_|\_\______|_|  \_\");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Omdat het kan! -VIS000\n");
			Console.ForegroundColor = ConsoleColor.Gray;
			var cs = new Program();
			cs.InitServer();
		}
		
		public List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
		public Dictionary<string, string> users = new Dictionary<string, string>();
		
		public void InitServer()
		{
			
			Console.WriteLine("Starting Server...");
			Console.Title = "Booting up";
			
			var server = new WebSocketServer("ws://0.0.0.0:909/server");

			server.Start(socket =>
			             {
			             	socket.OnOpen = () => 
			             	{
			             		Console.ForegroundColor = ConsoleColor.Green;
			             		Console.WriteLine("New Connection from: " + socket.ConnectionInfo.ClientIpAddress);
			             		allSockets.Add(socket);
			             		users.Add(socket.ConnectionInfo.Id.ToString(), "Anonymous");
			             		Console.Title = "Connections: " + allSockets.Count.ToString();
			             		Console.ForegroundColor = ConsoleColor.Gray;
			             	};
			             	socket.OnClose = () => 
			             	{
			             		Console.ForegroundColor = ConsoleColor.DarkYellow;
			             		Console.WriteLine(socket.ConnectionInfo.ClientIpAddress + " went away");
			             		MessageHandler("Left the chatroom", socket);
			             		allSockets.Remove(socket);
			             		Console.Title = "Connections: " + allSockets.Count.ToString();
			             		Console.ForegroundColor = ConsoleColor.Gray;
			             	};
			             	socket.OnMessage = message => MessageHandler(message, socket);

			             });
			Console.WriteLine("");
			Console.Title = "Connections: " + allSockets.Count.ToString();
			KeepAlive();			
		}
		
		void MessageHandler(string message, IWebSocketConnection sck)
		{
			Console.WriteLine(sck.ConnectionInfo.ClientIpAddress + " Data: " + message);
			if(message.Contains("/setusername"))
			{
				string userid = sck.ConnectionInfo.Id.ToString();
				string username = message.Split(' ')[1];
				users[sck.ConnectionInfo.Id.ToString()] = username;
				sck.Send("Username changed to: "+ username);
			}
			else
			{
				string username = users[sck.ConnectionInfo.Id.ToString()];
				allSockets.ToList().ForEach(s => s.Send(username + ": " + message));
			}
		}
		
		void KeepAlive()
		{
			
			string cmd = Console.ReadLine();
			while(cmd != "exit")
			{
				if(cmd.Contains("say"))
				{
					string[] message = cmd.Split('/');
					
					allSockets.ToList().ForEach(s => s.Send("Server: " + message[1].Trim()));
					Console.ForegroundColor = ConsoleColor.DarkMagenta;
					Console.WriteLine("Server: " + message[1].Trim());
					Console.ForegroundColor = ConsoleColor.Gray;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Error: Unknown command "+ cmd);
					Console.ForegroundColor = ConsoleColor.Gray;
				}
				Console.Write("CMD: ");
				Console.Title = "Connections: " + allSockets.Count.ToString();
				cmd = Console.ReadLine();
			}
		}
	}
}