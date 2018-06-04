//TODO: Enable if you add PubNub library!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: ENABLE using PubNubMessaging.Core;

namespace TrilleonAutomation {
	
	public class PubnubConnectionStrategy : MonoBehaviour {
		
		public static string PUBSUB_CHANNEL { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The name of pubsub channel that hosts this app's automation.
		public static string PUBLISH_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
		public static string SUBSCRIBE_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
		public static int MAX_MESSAGE_LENGTH { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The maximum number of characters (32KB) that can successfully go in a single Pubnub (pubsub) message. Make this number a couple dozen short of the actual maximum. Or, whatever limit your custom implementation requires.
		//TODO: ENABLE Pubnub pubnub { get; set; }
		
		void Start () {
			
			PUBLISH_KEY = ConfigReader.GetString("PUBNUB_PUBLISH_KEY");
			SUBSCRIBE_KEY = ConfigReader.GetString("PUBNUB_SUBSCRIBE_KEY");
			PUBSUB_CHANNEL = ConfigReader.GetString("PUBNUB_CHANNEL_NAME");
			MAX_MESSAGE_LENGTH = ConfigReader.GetInt("PUBNUB_MAX_MESSAGE_LENGTH");
			//TODO: ENABLE pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);
			Subscribe(PUBSUB_CHANNEL);
			
			if(!Application.isEditor || ConfigReader.GetBool("SEND_COMMUNICATIONS_IN_EDITOR")) {
				
				AutomationMaster.Arbiter.SendCommunication("checking_in", AutomationMaster.Arbiter.GridIdentity);
				
			}
			
		}
		
		public void Subscribe(string customChannel) {
			
			/*TODO: ENABLE
			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
			PUBSUB_CHANNEL = customChannel;
			pubnub.Subscribe<string>(
				PUBSUB_CHANNEL, 
				ReceiveMessagePubsub, 
				ReturnMessage, 
				Error); 
			*/
			
		}
		
		public void Unsubscribe() {
			
			/*TODO: ENABLE
			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
			pubnub.Unsubscribe<string>(
				PUBSUB_CHANNEL, 
				ReturnMessage,
				ReturnMessage,
				ReturnMessage, 
				Error); 
			*/
			
		}
		
		public void SendCommunication(string json) {
			
			/*TODO: ENABLE
			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
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
		
		/*TODO: ENABLE
		private void Error(PubnubClientError pubnubError) {

			AutoConsole.PostMessage(pubnubError.Description, MessageLevel.Verbose);

		}
		*/
		
	}
	
}