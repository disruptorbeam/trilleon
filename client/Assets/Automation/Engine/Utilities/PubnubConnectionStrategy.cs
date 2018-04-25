using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Activate if you have downloaded the Pubnub SDK! using PubNubMessaging.Core;

namespace TrilleonAutomation {
	
	public class PubnubConnectionStrategy : MonoBehaviour {

		public const string PUBSUB_CHANNEL = "Trilleon-Automation"; //CUSTOMIZE: The name of pubsub channel that hosts this app's automation.
		public const string PUBLISH_KEY = "YOUR_PUBNUB_PUBLISH_KEY_HERE"; //Recommend you store the publish key on a server if it is used outside of automation.
		public const string SUBSCRIBE_KEY = "YOUR_PUBNUB_SUBSCRIBE_KEY_HERE";
		public const int MAX_MESSAGE_LENGTH = 4000; //CUSTOMIZE: The maximum number of characters (32KB) that can successfully go in a single Pubnub (pubsub) message. Make this number a couple dozen short of the actual maximum. Or, whatever limit your custom implementation requires.
		//TODO: Activate if you have downloaded the Pubnub SDK! Pubnub pubnub { get; set; }

		void Start () {
			
			/*TODO: Activate if you have downloaded the Pubnub SDK! 
			pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);
			pubnub.Subscribe<string>(
				PUBSUB_CHANNEL, 
				ReceiveMessagePubsub, 
				ReturnMessage, 
				Error); 
			*/

		}

		public void SendCommunication(string json) {

			/*TODO: Activate if you have downloaded the Pubnub SDK! 
			pubnub.Publish<string>(
				PUBSUB_CHANNEL, 
				json.ToString(), 
				ReturnMessage, 
				Error); 
			*/

		}

		public void ReceiveMessagePubsub(string result) {

			Arbiter.LocalRunLaunch = false;
			ConnectionStrategy.ReceiveMessage(result);

		}

		public void ReturnMessage(string result) {

			return; //This is a success-type message. It's a required implementation, but is of no use outside of debugging; so do nothing.

		}

		/*TODO: Activate if you have downloaded the Pubnub SDK! 
		private void Error(PubnubClientError pubnubError) {

			AutoConsole.PostMessage(pubnubError.Description, MessageLevel.Verbose);

		}
		*/

	}

}