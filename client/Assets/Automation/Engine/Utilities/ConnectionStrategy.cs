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
		public static string Strategy { get; set; }
		public static int MaxMessageLength {
			get { 
				switch(Strategy) {
					case "pubnub":
						return PubnubConnectionStrategy.MAX_MESSAGE_LENGTH;
					case "socket":
					default:
						return SocketConnectionStrategy.MAX_MESSAGE_LENGTH;
				}
			}
		}
	
		//Don't forget to update all locations that consider strategies with any custom-created ones.
		public static SocketConnectionStrategy SocketConnectionStrategy { get; private set; }
		public static PubnubConnectionStrategy PubnubConnectionStrategy { get; private set; }

		void Start () {

			Strategy = string.Empty;
 			ChangeConnectionStrategy(ConfigReader.GetString("CONNECTION_STRATEGY").ToLower());

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

		public void ChangeConnectionStrategy(string newStrategy) {

			if(newStrategy == Strategy) {

				return;

			}

			switch(Strategy.ToLower()) {
				case "pubnub":
				Destroy(PubnubConnectionStrategy);
					break;
				case "socket":
				default:
					Destroy(SocketConnectionStrategy);
					break;
			}

			Strategy = newStrategy;
			switch(Strategy.ToLower()) {
				case "pubnub":
					TrilleonConnectionStrategy = ConnectionStrategyType.Pubnub;
					PubnubConnectionStrategy = gameObject.AddComponent<PubnubConnectionStrategy>();
					break;
				case "socket":
				default:
					TrilleonConnectionStrategy = ConnectionStrategyType.Socket;
					SocketConnectionStrategy = gameObject.AddComponent<SocketConnectionStrategy>();
					break;
			}

		}

		public void UpdateChannelIdentity(string identity) {

			switch(Strategy) {
				case "pubnub":
					PubnubConnectionStrategy.Unsubscribe();
					PubnubConnectionStrategy.Subscribe(identity);
					AutomationMaster.Arbiter.SendCommunication("checking_in", AutomationMaster.Arbiter.GridIdentity);
					break;
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