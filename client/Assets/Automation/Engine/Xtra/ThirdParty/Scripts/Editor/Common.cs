#define USE_MiniJSON
using System;
using PubNubMessaging.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

#if (USE_DOTNET_SERIALIZATION)
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

#elif (USE_MiniJSON)
using MiniJSON;

#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
#endif
namespace PubNubMessaging.Tests
{
	public class ActionMaster:MonoBehaviour{
	}
    public class Common
    {
        public static string Origin = "ps.pndsn.com";
        public static string PublishKey = "demo-36";
        public static string SubscribeKey = "demo-36";
        public static string SecretKey = "demo-36";
        public static float WaitTimeBetweenCalls = 5;
        public static float WaitTimeToReadResponse = 15;


        public object Response { get; set; }
        public string ErrorResponse { get; set; }

        public bool DeliveryStatus  { get; set; }

        public static string GetRandomChannelName()
        {
            System.Random r = new System.Random ();
            return "UnityUnitTests_" + r.Next (100);
        }

        public static List<ChannelEntity> CreateListOfChannelEntities<T>(bool channelGroup, bool channel, 
            bool presence, bool awaitingConnectCallback,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
            ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");


            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>("ch1", false, false, dictSM, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            var dictSM2 = new Dictionary<string, object>();
            dictSM2.Add("k3","v3");
            dictSM2.Add("k4","v4");

            ChannelEntity ce2 = Helpers.CreateChannelEntity<T>("ch2", false, false, dictSM2, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            var dictSM3 = new Dictionary<string, object>();
            dictSM3.Add("k5","v5");
            dictSM3.Add("k6","v6");

            ChannelEntity ce3 = Helpers.CreateChannelEntity<T>("cg1", false, true, dictSM3, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            var dictSM4 = new Dictionary<string, object>();
            dictSM4.Add("k7","v7");
            dictSM4.Add("k8","v8");

            ChannelEntity ce4 = Helpers.CreateChannelEntity<T>("cg2", false, true, dictSM4, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            var dictSM5 = new Dictionary<string, object>();
            dictSM5.Add("k7","v7");
            dictSM5.Add("k8","v8");

            ChannelEntity ce5 = Helpers.CreateChannelEntity<T>("cg2-pnpres", false, true, dictSM5, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);


            var dictSM6 = new Dictionary<string, object>();
            dictSM6.Add("k7","v7");
            dictSM6.Add("k8","v8");

            ChannelEntity ce6 = Helpers.CreateChannelEntity<T>("ch2-pnpres", false, false, dictSM6, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);


            var dictSM7 = new Dictionary<string, object>();
            dictSM7.Add("k7","v7");
            dictSM7.Add("k8","v8");

            ChannelEntity ce7 = Helpers.CreateChannelEntity<T>("ch7", true, false, dictSM7, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            var dictSM8 = new Dictionary<string, object>();
            dictSM8.Add("k7","v7");
            dictSM8.Add("k8","v8");

            ChannelEntity ce8 = Helpers.CreateChannelEntity<T>("cg8", true, true, dictSM8, 
            userCallback, connectCallback,
            Common.ErrorCallback, disconnectCallback, 
            wildcardPresenceCallback);

            List<ChannelEntity> lstCE = new List<ChannelEntity>();
            if(channel){
                lstCE.Add(ce1);
                lstCE.Add(ce2);
                if(presence){
                    lstCE.Add(ce6);
                }

                if(awaitingConnectCallback){
                    lstCE.Add(ce7);
                }

            }
            if(channelGroup){
                lstCE.Add(ce3);
                lstCE.Add(ce4);
                if(presence){
                    lstCE.Add(ce5);
                }

                if(awaitingConnectCallback){
                    lstCE.Add(ce8);
                }

            }
            return lstCE;
        }

        public static Dictionary<string, object> CreateSubscribeDictionary(){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("a", "1");
            dictSM.Add("b", "SM");
            dictSM.Add("c", "Channel");
            dictSM.Add("d", "Message");
            dictSM.Add("f", "flags");
            dictSM.Add("i", "issuingClientId");
            dictSM.Add("k", "subscribeKey");
            dictSM.Add("s", "10");

            var dictOT = new Dictionary<string, object>(); 
            dictOT.Add("t", 14685037252884276);
            dictOT.Add("r", "west");
            dictSM.Add("o", dictOT);

            var dictPM = new Dictionary<string, object>(); 
            dictPM.Add("t", 14685037252884348);
            dictPM.Add("r", "east");
            dictSM.Add("p", dictPM);

            var dictU = new Dictionary<string, object>(); 
            dictU.Add("region", "north");
            dictSM.Add("u", dictU);
            return dictSM;
        }

        public static void LogAndCompare(string expected, string received)
        {
            string expNRec = string.Format("Expected: {0}\nReceived: {1} ", expected, received);
            UnityEngine.Debug.Log(expNRec + expected.Equals (received));
            Assert.IsTrue (expected.Equals (received), expNRec);
        }

        /// <summary>
        /// Blocks the current thread unit the response is received
        /// or timeout occurs
        /// </summary>
        /// <param name="countdownSeconds">seconds to timeout</param>
        public void WaitForResponse (int countdownSeconds = 30)
        {
            Timer timer = new Timer ();
            DateTime start = DateTime.UtcNow; 
            DateTime endTime = start.AddSeconds (countdownSeconds); 
            timer.Enabled = true;
            timer.Start ();
            timer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
                TimeSpan remainingTime = endTime - DateTime.UtcNow;
                if (remainingTime < TimeSpan.Zero) {
                    timer.Enabled = false; 
                    DeliveryStatus = true;
                }
            };
            while (!DeliveryStatus)
                ;
            timer.Stop ();
        }

        public PubnubUnitTest CreateUnitTestInstance (string testClassName, string testCaseName)
        {
            PubnubUnitTest unitTest = new PubnubUnitTest ();
            unitTest.TestClassName = testClassName;
            unitTest.TestCaseName = testCaseName;
            return unitTest;
        }

        public void DisplayErrorMessage2 (PubnubClientError result)
        {
            Response = result.ToString();
            DeliveryStatus = true;
            UnityEngine.Debug.Log ("DisplayErrorMessage1:" + result.ToString ());
        }

        public void DisplayErrorMessage (PubnubClientError result)
        {
            ErrorResponse = result.Description;
            //DeliveryStatus = true;
            UnityEngine.Debug.Log ("DisplayErrorMessage:" + result.ToString ());
        }

        public void DisplayReturnMessageDummy (object result)
        {
            //deliveryStatus = true;
            //Response = result;
            ErrorResponse = result.ToString();
            UnityEngine.Debug.Log ("DisplayReturnMessageDummy:" + result.ToString ());
        }

        public void DisplayReturnMessage (object result)
        {
            UnityEngine.Debug.Log ("DisplayReturnMessageO:" + result.ToString ());
            Response = result;
            DeliveryStatus = true;
        }

        public void DisplayReturnMessage (string result)
        {
            UnityEngine.Debug.Log ("DisplayReturnMessageS:" + result.ToString ());
            Response = (object)result.ToString ();
            DeliveryStatus = true;
        }

        public long Timestamp (Pubnub pubnub)
        {
            DeliveryStatus = false;

            pubnub.Time (DisplayReturnMessage, DisplayReturnMessage);
            while (!DeliveryStatus)
                ;

            IList<object> fields = Response as IList<object>;
            return Convert.ToInt64 (fields [0].ToString ());
        }

        /// <summary>
        /// Deserialize the specified message using either JSONFX or NEWTONSOFT.JSON.
        /// The functionality is based on the pre-compiler flag
        /// </summary>
        /// <param name="message">Message.</param>
        public static T Deserialize<T> (string message)
        {
            object retMessage;
            #if (USE_JSONFX) || (USE_JSONFX_UNITY)
            var reader = new JsonFx.Json.JsonReader ();
            retMessage = reader.Read<T> (message);
            #elif (USE_JSONFX_UNITY_IOS)
            UnityEngine.Debug.Log ("message: " + message);
            retMessage = JsonReader.Deserialize<T> (message);
            #elif (USE_MiniJSON)
            UnityEngine.Debug.Log("message: " + message);
            object retMessage1 = Json.Deserialize(message) as object;
            Type type = typeof(T);
            var expectedType2 = typeof(object[]);
            if(expectedType2.IsAssignableFrom(type)){
                retMessage = ((System.Collections.IEnumerable)retMessage1).Cast<object> ().ToArray ();
            } else {
                retMessage    = retMessage1;
            }
            #else
            retMessage = JsonConvert.DeserializeObject<T> (message);
            #endif
            return (T)retMessage;
        }

        #if (USE_MiniJSON)
        /// <summary>
        /// Deserialize the specified message using either JSONFX or NEWTONSOFT.JSON.
        /// The functionality is based on the pre-compiler flag
        /// </summary>
        /// <param name="message">Message.</param>
        public static T DeserializeMiniJson<T> (string message)
        {
                object retMessage;
                UnityEngine.Debug.Log("message: " + message);
                retMessage = MiniJSON.Json.Deserialize(message) as object;
                return (T)retMessage;
        }
        #endif

        /// <summary>
        /// Serialize the specified message using either JSONFX or NEWTONSOFT.JSON.
        /// The functionality is based on the pre-compiler flag
        /// </summary>
        /// <param name="message">Message.</param>
        public static string Serialize (object message)
        {
            string retMessage;
            #if (USE_JSONFX) || (USE_JSONFX_UNITY)
            var writer = new JsonFx.Json.JsonWriter ();
            retMessage = writer.Write (message);
            retMessage = ConvertHexToUnicodeChars (retMessage);
            #elif (USE_JSONFX_UNITY_IOS)
            retMessage = JsonWriter.Serialize (message);
            retMessage = ConvertHexToUnicodeChars (retMessage);
            #elif (USE_MiniJSON)
            retMessage = Json.Serialize(message);
            UnityEngine.Debug.Log("retMessage: " + retMessage);
            #else
            retMessage = JsonConvert.SerializeObject (message);
            #endif
            return retMessage;
        }
                
        #if (USE_MiniJSON)
        /// <summary>
        /// Serialize the specified message using either JSONFX or NEWTONSOFT.JSON.
        /// The functionality is based on the pre-compiler flag
        /// </summary>
        /// <param name="message">Message.</param>
        public static string SerializeMiniJson (object message)
        {
                string retMessage;
                retMessage = MiniJSON.Json.Serialize(message);
                UnityEngine.Debug.Log("retMessage: " + retMessage);
                return retMessage;
        }
        #endif

        /// <summary>
        /// Converts the upper case hex to lower case hex.
        /// </summary>
        /// <returns>The lower case hex.</returns>
        /// <param name="value">Hex Value.</param>
        private static string ConvertHexToUnicodeChars (string value)
        {
            //if(;
            return Regex.Replace (
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse (m.Groups ["Value"].Value, NumberStyles.HexNumber)).ToString ();
                }     
            );
        }

        public static void WildcardPresenceCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("WildcardPresenceCallback CALLBACK LOG: {0}", result.ToString()));
        }

        public static void WildcardPresenceCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("WildcardPresenceCallback CALLBACK LOG: {0}", result));
        }

        public static void UserCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
        }

        public static void UserCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
        }

        public static void DisconnectCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        public static void DisconnectCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        public static void ConnectCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
        }

        public static void ConnectCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
        }

        public static void ErrorCallback (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format ("Error CALLBACK LOG: {0}", result.ToString()));
        }

    }

    /// <summary>
    /// Custom class for testing the encryption and decryption 
    /// </summary>
    class CustomClass
    {
        public string foo = "hi!";
        public int[] bar = { 1, 2, 3, 4, 5 };
    }

    [Serializable]
    class PubnubDemoObject
    {
        public double VersionID = 3.4;
        public long Timetoken = 13601488652764619;
        public string OperationName = "Publish";
        public string[] Channels = { "ch1" };
        public PubnubDemoMessage DemoMessage = new PubnubDemoMessage ();
        public PubnubDemoMessage CustomMessage = new PubnubDemoMessage ("This is a demo message");
        public XmlDocument SampleXml = new PubnubDemoMessage ().TryXmlDemo ();
    }

    [Serializable]
    class PubnubDemoMessage
    {
        public string DefaultMessage = "~!@#$%^&*()_+ `1234567890-= qwertyuiop[]\\ {}| asdfghjkl;' :\" zxcvbnm,./ <>? ";
        //public string DefaultMessage = "\"";
        public PubnubDemoMessage ()
        {
        }

        public PubnubDemoMessage (string message)
        {
            DefaultMessage = message;
        }

        public XmlDocument TryXmlDemo ()
        {
            XmlDocument xmlDocument = new XmlDocument ();
            xmlDocument.LoadXml ("<DemoRoot><Person ID='ABCD123'><Name><First>John</First><Middle>P.</Middle><Last>Doe</Last></Name><Address><Street>123 Duck Street</Street><City>New City</City><State>New York</State><Country>United States</Country></Address></Person><Person ID='ABCD456'><Name><First>Peter</First><Middle>Z.</Middle><Last>Smith</Last></Name><Address><Street>12 Hollow Street</Street><City>Philadelphia</City><State>Pennsylvania</State><Country>United States</Country></Address></Person></DemoRoot>");

            return xmlDocument;
        }
    }
}

