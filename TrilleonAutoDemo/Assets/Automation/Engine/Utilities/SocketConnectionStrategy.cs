using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redisU.framework;
using redisU.events;

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

        public static int SOCKET_PORT { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Should remain identical to the port used by pubsub master scripts.
        public static int MAX_MESSAGE_LENGTH { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Keep reasonably small for sake of performance.
        public static string SOCKET_CHANNEL_NAME { get; set; }
        public static List<KeyValuePair<RedisSubscription, RedisConnection>> Subscriptions = new List<KeyValuePair<RedisSubscription, RedisConnection>>();
        public static bool SubscriptionsComplete { get; private set; } //CUSTOMIZE: Change this value in TrilleonConfig.txt. Should remain identical to the port used by pubsub master scripts.
        static bool CheckedIn { get; set; }

        public static void Set() {

            SOCKET_PORT = AutomationMaster.ConfigReader.GetInt("SOCKET_MASTER_PUBSUB_PORT");
            MAX_MESSAGE_LENGTH = AutomationMaster.ConfigReader.GetInt("SOCKET_MAX_MESSAGE_LENGTH");
            SOCKET_CHANNEL_NAME = AutomationMaster.ConfigReader.GetString("PUBNUB_CHANNEL_NAME");

        }

        void Start() {

            //For WebGL games, use WebGLBroker.cs and javascript execution on the client to communicate with your game.
            #if !UNITY_WEBGL
            StartCoroutine(Subscribe());
            #endif

        }

        void Update() {

            if(!CheckedIn && SubscriptionsComplete) {

                ConnectionStrategy.Ready();
                CheckedIn = true;
                    
            }

        }

        public IEnumerator Subscribe() {

            yield return StartCoroutine(AutomationMaster.GameMaster.WaitForGameLoadingComplete());
            Thread t = new Thread(new ThreadStart(ConnectAll));
            t.Start();
            yield return null;

        }

        /// <summary>
        /// Async connection. Main thread will stall without this if connection cannot instantly be made.
        /// </summary>
        public void ConnectAll() {

            List<string> hosts = AutomationMaster.ConfigReader.GetString("SOCKET_HOSTS_PIPE_SEPERATED").Split('|').ToList();
            for(int h = 0; h < hosts.Count; h++) {

                try {

                    RedisSubscription r = new RedisSubscription(hosts[h], SOCKET_PORT);
                    r.OnSubscribe += OnSubscribe;
                    r.OnUnsubscribe += OnSubscribe;
                    r.Subscribe(SOCKET_CHANNEL_NAME);
                    r.OnMessageReceived += ReceiveCallback;
                    RedisConnection c = new RedisConnection(hosts[h], SOCKET_PORT);
                    Subscriptions.Add(new KeyValuePair<RedisSubscription, RedisConnection>(r, c));
                    ConnectionStrategy.OutgoingCommandQueue.Add(string.Format("SocketConnectionStrategy Subscription Phase: {0} : SUCCESS", hosts[h]));

                } catch(System.Net.Sockets.SocketException e) {

                    ConnectionStrategy.OutgoingCommandQueue.Add(string.Format("SocketConnectionStrategy Subscription Phase: {0} : {1}", hosts[h], e.Message));

                }

            }
            SubscriptionsComplete = true;

        }

        void OnSubscribe(object sender, SubscriptionEventArgs args) { /* Do nothing */ }

        void ReceiveCallback(object sender, MessageEventArgs args) {

            ConnectionStrategy.ReceiveMessage(args.GetMessage());

        }

        public void SendCommunication(string message) {

            for(int s = 0; s < Subscriptions.Count; s++) {

                Subscriptions[s].Value.Publish(SOCKET_CHANNEL_NAME, message);

            }

        }

        public void Stop() {

            for(int s = 0; s < Subscriptions.Count; s++) {

                Subscriptions[s].Key.Unsubscribe(SOCKET_CHANNEL_NAME);

            }

        }

    }

}