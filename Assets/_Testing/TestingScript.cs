using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
using UnityEngine.UI;

public class TestingScript : MonoBehaviour {





}

//namespace HD {
//	public class UDPChat : MonoBehaviour {
//		#region Data
//		public static UDPChat instance;
//		public int port = 8080;
//		public bool isServer;

//		public IPAddress serverIp;

//		List<IPEndPoint> clientList = new List<IPEndPoint>();

//		public static string messageToDisplay;
//		public Text text;

//		UdpConnectedClient connection;
//		#endregion

//		#region Unity Events
//		public void Awake() {
//			instance = this;

//			if (serverIp == null) {
//				this.isServer = true;
//				connection = new UdpConnectedClient();
//			} else {
//				connection = new UdpConnectedClient(ip: serverIp);
//				AddClient(new IPEndPoint(serverIp, port));
//			}
//		}

//		internal static void AddClient(
//		  IPEndPoint ipEndpoint) {
//			if (instance.clientList.Contains(ipEndpoint) == false) { // If it's a new client, add to the client list
//				UnityEngine.MonoBehaviour.print($"Connect to {ipEndpoint}");
//				instance.clientList.Add(ipEndpoint);
//			}
//		}

//		internal static void RemoveClient(
//		IPEndPoint ipEndpoint) {
//			instance.clientList.Remove(ipEndpoint);
//		}

//		private void OnApplicationQuit() {
//			connection.Close();
//		}

//		protected void Update() {
//			text.text = messageToDisplay;
//		}
//		#endregion

//		#region API
//		public void Send(
//		  string message) {
//			if (isServer) {
//				messageToDisplay += message + Environment.NewLine;
//			}

//			BroadcastChatMessage(message);
//		}

//		internal static void BroadcastChatMessage(string message) {
//			foreach (var ip in instance.clientList) {
//				instance.connection.Send(message, ip);
//			}
//		}
//		#endregion
//	}



//	public class UdpConnectedClient {
//		#region Data
//		readonly UdpClient connection;
//		readonly int port = 8080;
//		#endregion

//		#region Init
//		public UdpConnectedClient(IPAddress ip = null) {
//			if (UDPChat.instance.isServer) {
//				connection = new UdpClient(port);
//			} else {
//				connection = new UdpClient(); // Auto-bind port
//			}
//			connection.BeginReceive(OnReceive, null);
//		}

//		public void Close() {
//			connection.Close();
//		}
//		#endregion

//		#region API
//		void OnReceive(IAsyncResult ar) {
//			try {
//				IPEndPoint ipEndpoint = null;
//				byte[] data = connection.EndReceive(ar, ref ipEndpoint);

//				UDPChat.AddClient(ipEndpoint);

//				string message = System.Text.Encoding.UTF8.GetString(data);
//				UDPChat.messageToDisplay += message + Environment.NewLine;

//				if (UDPChat.instance.isServer) {
//					UDPChat.BroadcastChatMessage(message);
//				}
//			} catch (SocketException e) { 
//				// This happens when a client disconnects, as we fail to send to that port.
//			}
//			connection.BeginReceive(OnReceive, null);
//		}

//		internal void Send(string message, IPEndPoint ipEndpoint) {
//			byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
//			connection.Send(data, data.Length, ipEndpoint);
//		}
//		#endregion
//	}
//}