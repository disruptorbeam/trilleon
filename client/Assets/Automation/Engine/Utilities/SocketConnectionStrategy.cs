using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrilleonAutomation {

	/* IMPORTANT!
	 	Be careful customizing this, particularly when using the "recieve message" callbacks. This is multi-threaded,
	 	and Unity does not currently handle multi-threading well. Much of the UnityEngine API's cannot handle usage
	 	outside of the main thread. The default state is to queue messages in the arbiter, which picks them up on the 
	 	main thread in an IEnumerator, and handles their parsing and reactions as normal commands.
	*/

	/// <summary>
	/// A local-run pubsub system hosted by the machine(s) that launch automation onto devices. Eliminates need for a third party system and internet access (if game under test does not require internet access).
	/// </summary>
	public class SocketConnectionStrategy : MonoBehaviour {  

		/* Console Test Commands:
		   brew install netcat
		   echo "[{"automation_command":"rt all"}]" | netcat localhost 9595
		*/

		public static int MASTER_TRILLEON_PUBSUB_PORT { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Should remain identical to the port used by pubsub master scripts.
		public static int MAX_MESSAGE_LENGTH { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Keep reasonably small for sake of performance.
		public static string TRILLEON_SOCKET_IP { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Default is Localhost.
		Socket listener;

		class StateObject {

			public Socket workSocket = null;  
			public const int BufferSize = 1024;  
			public byte[] buffer = new byte[BufferSize];  
			public StringBuilder sb = new StringBuilder();  

		}

		void Start() {

			MASTER_TRILLEON_PUBSUB_PORT = ConfigReader.GetInt("SOCKET_MASTER_PUBSUB_PORT");
			MAX_MESSAGE_LENGTH = ConfigReader.GetInt("SOCKET_MAX_MESSAGE_LENGTH");
			TRILLEON_SOCKET_IP = ConfigReader.GetString("SOCKET_IP_ADDRESS");

			IPAddress ipAddress = IPAddress.Parse(TRILLEON_SOCKET_IP);  
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, MASTER_TRILLEON_PUBSUB_PORT);  
			listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
			listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			listener.BeginConnect(localEndPoint, new AsyncCallback(ConnectCallback), listener);  
			StateObject state = new StateObject();  
			state.workSocket = listener; 
			listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,  
				new AsyncCallback(ReceiveCallback), state);  
			
		}

		public void Stop() {

			listener.Close();

		}

		private void ReceiveCallback(IAsyncResult ar) {  

			StateObject state = (StateObject)ar.AsyncState;  
			Socket client = state.workSocket;  
			int bytesRead = client.EndReceive(ar);  

			if(bytesRead > 0) {  

				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));  
				client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);  
				
			}

			if(!string.IsNullOrEmpty(state.sb.ToString())) {
				
				ConnectionStrategy.ReceiveMessage(state.sb.ToString());

			}

		}   

		public void SendCommunication(string message) {
			
			byte[] byteData = Encoding.ASCII.GetBytes(message); 
			listener.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), listener);  
			
		}  

		void SendCallback(IAsyncResult ar) {  
			
			return;

		}  

		private static void ConnectCallback(IAsyncResult ar) {  

			return;

		}

	}  

}