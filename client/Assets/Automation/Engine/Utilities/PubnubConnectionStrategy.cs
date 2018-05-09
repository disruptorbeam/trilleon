using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubNubMessaging.Core;

namespace TrilleonAutomation {
	
	public class PubnubConnectionStrategy : MonoBehaviour {

		public static string PUBSUB_CHANNEL { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The name of pubsub channel that hosts this app's automation.
		public static string PUBLISH_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
		public static string SUBSCRIBE_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
		public static int MAX_MESSAGE_LENGTH { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The maximum number of characters (32KB) that can successfully go in a single Pubnub (pubsub) message. Make this number a couple dozen short of the actual maximum. Or, whatever limit your custom implementation requires.
		Pubnub pubnub { get; set; }

		void Start () {

			PUBLISH_KEY = ConfigReader.GetString("PUBNUB_PUBLISH_KEY");
			SUBSCRIBE_KEY = ConfigReader.GetString("PUBNUB_SUBSCRIBE_KEY");
			PUBSUB_CHANNEL = ConfigReader.GetString("PUBNUB_CHANNEL_NAME");
			MAX_MESSAGE_LENGTH = ConfigReader.GetInt("PUBNUB_MAX_MESSAGE_LENGTH");

			pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);
			pubnub.Subscribe<string>(
				PUBSUB_CHANNEL, 
				ReceiveMessagePubsub, 
				ReturnMessage, 
				Error); 

		}

		public void SendCommunication(string json) {

			pubnub.Publish<string>(
				PUBSUB_CHANNEL, 
				json.ToString(), 
				ReturnMessage, 
				Error); 
			
		}

		public void ReceiveMessagePubsub(string result) {

			Arbiter.LocalRunLaunch = false;
			ConnectionStrategy.ReceiveMessage(result);

		}

		public void ReturnMessage(string result) {

			return; //This is a success-type message. It's a required implementation, but is of no use outside of debugging; so do nothing.

		}

		private void Error(PubnubClientError pubnubError) {

			AutoConsole.PostMessage(pubnubError.Description, MessageLevel.Verbose);

		}

	}

}