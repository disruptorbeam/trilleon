using UnityEngine;
using PubNubMessaging.Core;

namespace TrilleonAutomation {

    public class PubnubConnectionStrategy : MonoBehaviour {

        public static string PUBSUB_CHANNEL { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The name of pubsub channel that hosts this app's automation.
        public static string PUBLISH_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
        public static string SUBSCRIBE_KEY { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt.
        public static int MAX_MESSAGE_LENGTH { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. The maximum number of characters (32KB) that can successfully go in a single Pubnub (pubsub) message. Make this number a couple dozen short of the actual maximum. Or, whatever limit your custom implementation requires.
        Pubnub pubnub { get; set; }

        public static void Set() {

            PUBLISH_KEY = AutomationMaster.ConfigReader.GetString("PUBNUB_PUBLISH_KEY");
            SUBSCRIBE_KEY = AutomationMaster.ConfigReader.GetString("PUBNUB_SUBSCRIBE_KEY");
            PUBSUB_CHANNEL = AutomationMaster.ConfigReader.GetString("PUBNUB_CHANNEL_NAME");
            MAX_MESSAGE_LENGTH = AutomationMaster.ConfigReader.GetInt("PUBNUB_MAX_MESSAGE_LENGTH");

        }

        void Start() {

            //For WebGL games, use WebGLBroker.cs and javascript execution on the client to communicate with your game.
            pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);
            Subscribe(PUBSUB_CHANNEL);
            ConnectionStrategy.Ready();

        }

		public void Subscribe(string customChannel) {

			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
			PUBSUB_CHANNEL = customChannel;
			pubnub.Subscribe<string>(
				PUBSUB_CHANNEL, 
				ReceiveMessagePubsub, 
				ReturnMessage, 
				Error); 

		}

		public void Unsubscribe() {

			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
			pubnub.Unsubscribe<string>(
				PUBSUB_CHANNEL, 
				ReturnMessage,
				ReturnMessage,
				ReturnMessage, 
				Error); 

		}

		public void SendCommunication(string json) {

			if(pubnub == null) {

				pubnub = new Pubnub(PUBLISH_KEY, SUBSCRIBE_KEY);

			}
			pubnub.Publish<string>(
				PUBSUB_CHANNEL, 
				json, 
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