using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrilleonAutomation {

	public enum ConnectionStrategyType {
		Pubnub,
		Socket
	};
	public class ConnectionStrategy : MonoBehaviour {

		public static ConnectionStrategyType TrilleonConnectionStrategy;
		public static List<string> IncomingCommandQueue = new List<string>();
		public static List<string> OutgoingCommandQueue = new List<string>();

		public static int MaxMessageLength {
			get { 
				switch(ConfigReader.GetString("CONNECTION_STRATEGY").ToLower()) {
					case "pubnub":
						return SocketConnectionStrategy.MAX_MESSAGE_LENGTH;
					case "socket":
					default:
						return PubnubConnectionStrategy.MAX_MESSAGE_LENGTH;
				}
			}
		}

		public SocketConnectionStrategy SocketConnectionStrategy {
			get { 
				return AutomationMaster.StaticSelf.GetComponent<SocketConnectionStrategy>();
			}
		}

		public PubnubConnectionStrategy PubnubConnectionStrategy {
			get { 
				return AutomationMaster.StaticSelf.GetComponent<PubnubConnectionStrategy>();
			}
		}

		void Start () {

			switch(ConfigReader.GetString("CONNECTION_STRATEGY").ToLower()) {
				case "pubnub":
					TrilleonConnectionStrategy = ConnectionStrategyType.Pubnub;
					gameObject.AddComponent<PubnubConnectionStrategy>();
					break;
				case "socket":
				default:
					TrilleonConnectionStrategy = ConnectionStrategyType.Socket;
					gameObject.AddComponent<SocketConnectionStrategy>();
					break;
			}

		}


		void Update() {

			//Listen for commands until coroutine is killed.
			if(IncomingCommandQueue.Any()) {

				ReceiveMessageActual(IncomingCommandQueue.First());
				IncomingCommandQueue.RemoveAt(0);

			} 
			if(OutgoingCommandQueue.Any()) {

				SendCommunicationActual(OutgoingCommandQueue.First());
				OutgoingCommandQueue.RemoveAt(0);

			} 
				
		}

		public static void SendCommunication(string json) {

			OutgoingCommandQueue.Add(json);

		}

		public static void ReceiveMessage(string result) {

			IncomingCommandQueue.Add(result);

		}

		void ReceiveMessageActual(string result) {

			AutomationMaster.Arbiter.StartCoroutine(AutomationMaster.Arbiter.HandleMessage(result));

		}

		void SendCommunicationActual(string json) {

			switch(TrilleonConnectionStrategy) {
				case ConnectionStrategyType.Pubnub:
					PubnubConnectionStrategy.SendCommunication(json.ToString());
					break;
				case ConnectionStrategyType.Socket:
					SocketConnectionStrategy.SendCommunication(json.ToString());
					break;
			}

		}
	
	}

}