using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class HelpersUnitTests
    {
        #if DEBUG
        int ExceptionCode =0;
        bool readCallback = false;

        List<object> resultList = new List<object>();
        string ExpectedConnectResponse = "";
        string ExpectedRegularResponse = "";
        bool ExpectedCallback = false;
        bool ExpectedConnect = false;

        string MessageToCheck = "";
        bool CheckMessage = false;
        bool CheckMultiple = false;
        int MessageCount =0;
        int MessageReceivedCount =0;
        /*[Test]
        public void TestCheckChannelsInMultiChannelSubscribeRequestFalse2 (){

            string[] multiChannel = {"testChannel", "testChannel2"};

            TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel", false);
        }

        [Test]
        public void TestCheckChannelsInMultiChannelSubscribeRequestTrue (){

            string[] multiChannel = {};

            TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel", true);
        }

        [Test]
        public void TestCheckChannelsInMultiChannelSubscribeRequestFalse (){ 

            string[] multiChannel = {"testChannel", "testChannel2"}; 
            TestCheckChannelsInMultiChannelSubscribeRequestCommon (multiChannel, "testChannel3", false);
        }

        void TestCheckChannelsInMultiChannelSubscribeRequestCommon(string[] multiChannel, string channel, bool isTrue){
            SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
            foreach (string currentChannel in multiChannel) {
                multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
            }

            SafeDictionary<string, PubnubWebRequest> channelRequest = new SafeDictionary<string, PubnubWebRequest> ();
            PubnubWebRequest pnwr = new PubnubWebRequest(new UnityEngine.WWW("ps.pndsn.com"));
            foreach (string currentChannel in multiChannel) {
                channelRequest.AddOrUpdate (currentChannel, pnwr, (key, oldState) => pnwr);
            }

            if (isTrue) {
                Assert.IsTrue (Helpers.CheckChannelsInMultiChannelSubscribeRequest (channel, 
                    multiChannelSubscribe, channelRequest));
            } else {
                Assert.IsFalse(Helpers.CheckChannelsInMultiChannelSubscribeRequest(channel, 
                    multiChannelSubscribe, channelRequest));
            }
        }*/

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.PresenceV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsPresNoNetNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.PresenceV2);
        }
            
        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemove (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribed (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNew (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<string> (channel, multiChannel, false, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetRemoveObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetAlreadySubscribedObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel";
            ExceptionCode = 112;
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, true, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsNoNetNewObj (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string channel = "testChannel3";
            TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<object> (channel, multiChannel, false, false, ResponseType.SubscribeV2);
        }

        public void TestRemoveDuplicateChannelsAndCheckForAlreadySubscribedChannelsCommon<T> (string channel,
            string[] multiChannel, bool fireCallback, bool networkConnection, ResponseType responseType
        ){ 

            //Remove duplicate channels
            //Already subscribed
            //New channel
            readCallback = fireCallback;
            List<string> validChannels = new List<string> ();
            validChannels.Add (channel);

            PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;

            Action<PubnubClientError> errorcb = ErrorCallbackCommonExceptionHandler;
            if (!readCallback) {
                errorcb = null;
            }
            List<ChannelEntity> channelEntities;

            Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T> (responseType, null, null, 
                errorcb, null, null, multiChannel, null, errorLevel, false, out channelEntities
            );

            List<ChannelEntity> channelEntities2;

            Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels<T> (responseType, null, null, 
                errorcb, null, null, new string[] {channel}, null, errorLevel, false, out channelEntities2
            );

            if (!readCallback) {
                string channels2 = Helpers.GetNamesFromChannelEntities(channelEntities2, false);

                bool channelMatch = false;
                if (channelEntities != null) {
                    foreach (ChannelEntity c in channelEntities2) {
                        string ch2= c.ChannelID.ChannelOrChannelGroupName;
                        if(c.ChannelID.IsPresenceChannel){
                            channel = channel + Utility.PresenceChannelSuffix;
                        }
                        channelMatch = channel.Equals(ch2);
                        if(channelMatch)
                            break;
                    }
                }
                UnityEngine.Debug.Log ("not fireCallback:" +channelMatch + channels2 + channel);
                Assert.IsTrue (channelMatch);

            }
        }

        void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
        {
            UnityEngine.Debug.Log ("in fireCallback");
            UnityEngine.Debug.Log (result.StatusCode);
            UnityEngine.Debug.Log (ExceptionCode);

            if (readCallback) {
                Assert.IsTrue (result.StatusCode.Equals (ExceptionCode));
            }
        }

        void ErrorCallbackCommonExceptionHandler2 (PubnubClientError result)
        {
            UnityEngine.Debug.Log ("in fireCallback");
            UnityEngine.Debug.Log (result.StatusCode);
            UnityEngine.Debug.Log (ExceptionCode);

            if (readCallback) {
                Assert.IsTrue (result.StatusCode.Equals (ExceptionCode));
                if (CheckMessage) {
                    UnityEngine.Debug.Log (MessageToCheck);
                    Assert.IsTrue (result.Message.Contains (MessageToCheck));
                }
                if (CheckMultiple) {
                    MessageReceivedCount++;
                    UnityEngine.Debug.Log ("MessageReceivedCount:" + MessageReceivedCount);
                    if (MessageCount.Equals (MessageReceivedCount)) {
                        Assert.True (true, "MessageCount equals MessageReceivedCount");
                    }
                }
            }
        }

        /*[Test]
        public void TestGetValidChannels (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string[] rawChannels = {"testChannel", "testChannel2"}; 

            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestGetValidChannelsEmpty (){ 
            string[] multiChannel = {"testChannel3", "testChannel"}; 
            string[] rawChannels = {" "}; 
            ExceptionCode = 117;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestGetValidChannelsNotSub (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
            ExceptionCode = 118;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.SubscribeV2);
        }

        [Test]
        public void TestGetValidChannelsNotSubUnsub (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
            ExceptionCode = 118;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Unsubscribe);
        }

        [Test]
        public void TestGetValidChannelsUnsub (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string[] rawChannels = {"testChannel", "testChannel2"}; 

            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.Unsubscribe);
        }

        [Test]
        public void TestGetValidChannelsEmptyUnsub (){ 
            string[] multiChannel = {"testChannel3", "testChannel"}; 
            string[] rawChannels = {" "}; 
            ExceptionCode = 117;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.Unsubscribe);
        }

        [Test]
        public void TestGetValidChannelsNotSubPUnsub (){ 
            string[] multiChannel = {"testChannel", "testChannel2"}; 
            string[] rawChannels = {"testChannel3", "testChannel", "testChannel2"}; 
            ExceptionCode = 119;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.PresenceUnsubscribe);
        }

        [Test]
        public void TestGetValidChannelsPUnsub (){ 
            string[] multiChannel = {"testChannel-pnpres", "testChannel2-pnpres"}; 
            string[] rawChannels = {"testChannel", "testChannel2"}; 

            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, false, ResponseType.PresenceUnsubscribe);
        }

        [Test]
        public void TestGetValidChannelsEmptyPUnsub (){ 
            string[] multiChannel = {"testChannel3", "testChannel"}; 
            string[] rawChannels = {" "}; 
            ExceptionCode = 117;
            TestGetValidChannelsCommon<string> (rawChannels, multiChannel, true, ResponseType.PresenceUnsubscribe);
        }

        public void TestGetValidChannelsCommon<T> (string[] rawChannels,
            string[] multiChannel, bool fireCallback, ResponseType responseType
        ){ 
            readCallback = fireCallback;

            PubnubErrorFilter.Level errorLevel = PubnubErrorFilter.Level.Info;
            SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
            foreach (string currentChannel in multiChannel) {
                multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
            }


            Action<PubnubClientError> errorcb = ErrorCallbackCommonExceptionHandler;
            if (!readCallback) {
                errorcb = null;
            }

            List<string> validChannels2 = Helpers.GetValidChannels<T> (responseType,
                errorcb, rawChannels, multiChannelSubscribe, errorLevel
            );
            if (!readCallback) {
                UnityEngine.Debug.Log ("not fireCallback" + validChannels2.Count);
                foreach (string channel in validChannels2) {
                    UnityEngine.Debug.Log ("ch:" + channel);
                }
                if (responseType.Equals (ResponseType.PresenceUnsubscribe)) {
                    Assert.IsTrue (validChannels2.Contains (rawChannels [0] + "-pnpres"));
                } else {
                    Assert.IsTrue (validChannels2.Contains (rawChannels [0]));
                }
            }
        }

        [Test]
        public void TestGetCurrentSubscriberChannels (){ 
            string[] multiChannel = {"testChannel3", "testChannel"}; 

            SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
            foreach (string currentChannel in multiChannel) {
                multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
            }

            string[] channels = Helpers.GetCurrentSubscriberChannels (multiChannelSubscribe);
            bool bFound = false;
            foreach (string ch in channels) {
                bool bFound1 = false;
                foreach (string ch2 in multiChannel) {
                    if (ch.Equals (ch2)) {
                        bFound1 = true;
                        break;
                    }
                }
                if (bFound1) {
                    bFound = true;
                } else {
                    bFound = false;
                }
            }
            Assert.IsTrue (bFound);
        }*/

        public void UserCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
            if (ExpectedCallback) {
                bool bRes = result.Equals (ExpectedRegularResponse);
                UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK: {0}", bRes));

                Assert.IsTrue (bRes);
            }

        }

        public void UserCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
            if (ExpectedCallback) {
                Pubnub pubnub = new Pubnub (
                    Common.PublishKey,
                    Common.SubscribeKey,
                    "",
                    "",
                    true
                );
                bool bRes = pubnub.JsonPluggableLibrary.SerializeToJsonString (result).Equals (ExpectedRegularResponse);
                UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK Obj: {0}", bRes));

                Assert.IsTrue (bRes);
            }
        }

        void DisconnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        public void ConnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
            if (ExpectedConnect) {
                bool bRes = result.Equals (ExpectedConnectResponse);
                UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK: {0}", bRes));
                Assert.IsTrue (bRes);
            }
        }

        public void ConnectCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
            if (ExpectedConnect) {
                Pubnub pubnub = new Pubnub (
                                   Common.PublishKey,
                                   Common.SubscribeKey,
                                   "",
                                   "",
                                   true
                               );
                bool bRes = pubnub.JsonPluggableLibrary.SerializeToJsonString (result).Equals (ExpectedConnectResponse);
                UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK Obj: {0}", bRes));
                Assert.IsTrue (bRes);
            }
        }

        [Test]
        public void TestProcessResponseCallbacksConnectedSub (){ 
            string[] multiChannel = {"testChannel"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            //result2.Add ("[]");
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            ExpectedConnectResponse = "[1,\"Connected\",\"testChannel\"]";
            ExpectedConnect = true;
            ExpectedCallback = false;
            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksConnectedPres (){ 
            string[] multiChannel = {"testChannel"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            //result2.Add ("[]");
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            ExpectedConnectResponse = "[1,\"Presence Connected\",\"testChannel\"]";
            ExpectedConnect = true;
            ExpectedCallback = false;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.PresenceV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksConnectedSubObj (){ 
            string[] multiChannel = {"testChannel"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            //result2.Add ("[]");
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            ExpectedConnectResponse = "[1,\"Connected\",\"testChannel\"]";
            ExpectedConnect = true;
            ExpectedCallback = false;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksConnectedPresObj (){ 
            string[] multiChannel = {"testChannel"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            //result2.Add ("[]");
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            ExpectedConnectResponse = "[1,\"Presence Connected\",\"testChannel\"]";
            ExpectedConnect = true;
            ExpectedCallback = false;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.PresenceV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksSub (){ 
            string[] multiChannel = {"testChannel"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            object[] obj = {"test message"};
            result2.Add (obj);
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            ExpectedRegularResponse = "[[\"test message\"],\"14498416434364941\",\"testChannel\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksPres (){ 
            string[] multiChannel = {"testChannel"};

            Dictionary<string, object> dictionary = new Dictionary<string, object> ();
            dictionary.Add ("action", "join");
            dictionary.Add ("timestamp", 1451284248);
            dictionary.Add ("uuid", "0c52695b-7dbf-4643-8b90-e0e1cbc19071");
            dictionary.Add ("occupancy", 1);
            Dictionary<string, object>[] dict = new Dictionary<string, object>[] {
                new Dictionary<string, object>()
            };
            dict[0] = dictionary;
            List<object> result = new List<object> ();
            result.Add (dict);
            result.Add ("14380891444409649");
            result.Add ("testChannel");

            ExpectedRegularResponse = "[{\"action\":\"join\",\"timestamp\":1451284248,\"uuid\":\"0c52695b-7dbf-4643-8b90-e0e1cbc19071\",\"occupancy\":1},\"14380891444409649\",\"testChannel\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksHereNow (){ 
            string[] multiChannel = {"testChannel"};

            Dictionary<string, object> dictionary = new Dictionary<string, object> ();
            string[] strarr = { "0c52695b-7dbf-4643-8b90-e0e1cbc19071", "123123234t234f34fq3dq" };
            dictionary.Add ("occupancy", 4);
            dictionary.Add ("uuid", strarr);

            Dictionary<string, object>[] dict = new Dictionary<string, object>[] {
                new Dictionary<string, object>()
            };
            dict[0] = dictionary;
            List<object> result = new List<object> ();
            result.Add (dict);
            result.Add ("14380891444409649");
            result.Add ("testChannel");
            ExpectedRegularResponse = "[{\"occupancy\":4,\"uuid\":[\"0c52695b-7dbf-4643-8b90-e0e1cbc19071\",\"123123234t234f34fq3dq\"]},\"14380891444409649\",\"testChannel\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 14498416434364941, false, false, ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksPushRegister (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Modified Channels");
            result.Add ("");

            ExpectedRegularResponse = "[1,\"Modified Channels\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 0, false, false, ResponseType.PushRegister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksPushGet (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add ("push_channel");
            result.Add ("");
            //["push_channel"]

            ExpectedRegularResponse = "[\"push_channel\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 0, false, false, ResponseType.PushGet,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksPushUnregister (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Modified Channels");
            result.Add ("");

            ExpectedRegularResponse = "[1,\"Modified Channels\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 0, false, false, ResponseType.PushUnregister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestProcessResponseCallbacksPushRemove (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Removed Device");
            result.Add ("");
            //[1, "Removed Device"]

            ExpectedRegularResponse = "[1,\"Removed Device\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<string> (multiChannel, result, "", 0, false, false, ResponseType.PushRemove,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void  TestProcessResponseCallbacksObjPushRegister (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Modified Channels");
            result.Add ("");

            ExpectedRegularResponse = "[1,\"Modified Channels\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 0, false, false, ResponseType.PushRegister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void  TestProcessResponseCallbacksObjPushGet (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add ("push_channel");
            result.Add ("");
            //["push_channel"]

            ExpectedRegularResponse = "[\"push_channel\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 0, false, false, ResponseType.PushGet,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void  TestProcessResponseCallbacksObjPushUnregister (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Modified Channels");
            result.Add ("");

            ExpectedRegularResponse = "[1,\"Modified Channels\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 0, false, false, ResponseType.PushUnregister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void  TestProcessResponseCallbacksObjPushRemove (){ 
            string[] multiChannel = {"push_channel"};
            List<object> result = new List<object> ();
            result.Add (1);
            result.Add ("Removed Device");
            result.Add ("");
            //[1, "Removed Device"]

            ExpectedRegularResponse = "[1,\"Removed Device\",\"\"]";
            ExpectedConnect = false;
            ExpectedCallback = true;

            TestProcessResponseCallbacksCommon<object> (multiChannel, result, "", 0, false, false, ResponseType.PushRemove,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        public void TestProcessResponseCallbacksCommon<T>(string [] multiChannel, List<object> result, string cipherKey, long timetoken, bool isTimeout,
            bool resumeOnReconnect, ResponseType responseType,
            Action<T> userCallback, Action<T> connectCallback, Action<PubnubClientError> errorCallback 
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                cipherKey,
                true
            );

            SafeDictionary<string, long> multiChannelSubscribe = new SafeDictionary<string, long> ();
            foreach (string currentChannel in multiChannel) {
                if (ExpectedConnect) {
                    multiChannelSubscribe.AddOrUpdate (currentChannel, 0, (key, oldValue) => Convert.ToInt64 (0));
                } else {
                    multiChannelSubscribe.AddOrUpdate (currentChannel, 14498416434364941, (key, oldValue) => Convert.ToInt64 (14498416434364941));
                }

            }
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(multiChannel, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                resumeOnReconnect, 0, isTimeout, timetoken, typeof(T), "", userCallback, errorCallback);

            Helpers.ProcessResponseCallbacks <T> (ref result, requestState, 
                cipherKey, pubnub.JsonPluggableLibrary);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjSubscribeConnectedCipher (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjSubscribeMessageCipher (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[\"test\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"test\"], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjSubscribeMessageCipherYay (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[\"q/xJqqN6qbiZMXYmiQC1Fw==\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"q/xJqqN6qbiZMXYmiQC1Fw==\"], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjSubscribe2MessageCipher (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("[\"test\",\"test2\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[[\"test\",\"test2\"], 14498416434364941, \"testChannel3\"]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPublishCipher (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Sent");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "enigma", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPublish (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Sent");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjDH (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("[\"test1\",\"test2\"]");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[[\"test1\",\"test2\"], \"testChannel3\"]", ResponseType.DetailedHistory,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjHereNow (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", 
                "{\"status\": 200, \"message\": \"OK\", \"service\": \"Presence\", \"occupancy\": 0}", ResponseType.HereNow,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjGlobalHereNow (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("{\"status\":200,\"message\":\"OK\",\"payload\":{\"channels\":{\"e\":{\"occupancy\":1},\"sentiment\":{\"occupancy\":1},\"testChannel\":{\"occupancy\":3}},\"total_channels\":3,\"total_occupancy\":5},\"service\":\"Presence\"}");

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", 
                "{\"status\": 200, \"message\": \"OK\", \"payload\": {\"channels\": {\"e\": {\"occupancy\": 1}, \"sentiment\": {\"occupancy\": 1}, \"testChannel\": {\"occupancy\": 3}}, \"total_channels\": 3, \"total_occupancy\": 5}, \"service\": \"Presence\"}",
                ResponseType.GlobalHereNow,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjTime(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add ("14510493331774269");

            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[14510493331774269]", ResponseType.Time,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesSubscribeConnectedCipher (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesSubscribeMessageCipher (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[\"test\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"test\"], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesSubscribeMessageCipherYay (){ 
            string[] multiChannel = {"testChannel3", "testChannel"};
            resultList = new List<object> ();
            resultList.Add ("[\"q/xJqqN6qbiZMXYmiQC1Fw==\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"q/xJqqN6qbiZMXYmiQC1Fw==\"], 14498416434364941]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesSubscribe2MessageCipher (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("[\"test\",\"test2\"]");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[[\"test\",\"test2\"], 14498416434364941, \"testChannel3\"]", ResponseType.SubscribeV2,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPublishCipher (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Sent");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));
                            
            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "enigma", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPublish (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Sent");
            resultList.Add ("14498416434364941");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[1, \"Sent\", \"14498416434364941\"]", ResponseType.Publish,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesDH (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("[\"test1\",\"test2\"]");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[[\"test1\",\"test2\"], \"testChannel3\"]", ResponseType.DetailedHistory,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesHereNow (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}");
            resultList.Add (string.Join(",", multiChannel));

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", 
                "{\"status\": 200, \"message\": \"OK\", \"service\": \"Presence\", \"occupancy\": 0}", ResponseType.HereNow,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesGlobalHereNow (){ 
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            resultList.Add ("{\"status\":200,\"message\":\"OK\",\"payload\":{\"channels\":{\"e\":{\"occupancy\":1},\"sentiment\":{\"occupancy\":1},\"testChannel\":{\"occupancy\":3}},\"total_channels\":3,\"total_occupancy\":5},\"service\":\"Presence\"}");

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", 
                "{\"status\": 200, \"message\": \"OK\", \"payload\": {\"channels\": {\"e\": {\"occupancy\": 1}, \"sentiment\": {\"occupancy\": 1}, \"testChannel\": {\"occupancy\": 3}}, \"total_channels\": 3, \"total_occupancy\": 5}, \"service\": \"Presence\"}",
                ResponseType.GlobalHereNow,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesTime(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add ("14510493331774269");

            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[14510493331774269]", ResponseType.Time,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPushRegister(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Modified Channels");
            resultList.Add ("");
            //[1, "Modified Channels"]
            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[1, \"Modified Channels\"]", ResponseType.PushRegister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPushRemove(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Modified Channels");
            resultList.Add ("");

            //[1, "Modified Channels"]
            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[1, \"Modified Channels\"]", ResponseType.PushRemove,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPushGet(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add ("push_channel");
            resultList.Add ("");
            //["push_channel"]
            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[\"push_channel\"]", ResponseType.PushGet,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesObjPushUnregister(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Removed Device");
            resultList.Add ("");
            //[1, "Removed Device"]
            TestWrapResultBasedOnResponseTypeCommon<object> (multiChannel, "", "[1, \"Removed Device\"]", ResponseType.PushUnregister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPushRegister(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Modified Channels");
            resultList.Add ("");
            //[1, "Modified Channels"]
            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[1, \"Modified Channels\"]", ResponseType.PushRegister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPushRemove(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Modified Channels");
            resultList.Add ("");

            //[1, "Modified Channels"]
            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[1, \"Modified Channels\"]", ResponseType.PushRemove,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPushGet(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add ("push_channel");
            resultList.Add ("");
            //["push_channel"]
            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[\"push_channel\"]", ResponseType.PushGet,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        [Test]
        public void TestWrapResultBasedOnResponseTypesPushUnregister(){ 
            string[] multiChannel = {};
            resultList = new List<object> ();
            resultList.Add (1);
            resultList.Add ("Removed Device");
            resultList.Add ("");
            //[1, "Removed Device"]
            TestWrapResultBasedOnResponseTypeCommon<string> (multiChannel, "", "[1, \"Removed Device\"]", ResponseType.PushUnregister,
                UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler
            );

        }

        public void TestWrapResultBasedOnResponseTypeCommon<T>(string [] channels, string cipherKey, string jsonString,
            ResponseType responseType, Action<T> userCallback, 
            Action<T> connectCallback, Action<PubnubClientError> errorCallback ){

            //here now {"status": 200, "message": "OK", "service": "Presence", "occupancy": 0}
            //global herenow {"status": 200, "message": "OK", "payload": {"channels": {"e": {"occupancy": 1}, "sentiment": {"occupancy": 1}, "testChannel": {"occupancy": 3}}, "total_channels": 3, "total_occupancy": 5}, "service": "Presence"}
            //time [14510493331774269]
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                cipherKey,
                true
            );

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(channels, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                false, 0, false, 0, typeof(T), "", userCallback, errorCallback);

            List<object> list = new List<object>();

            Helpers.WrapResultBasedOnResponseType<T> (requestState, jsonString, 
                pubnub.JsonPluggableLibrary, PubnubErrorFilter.Level.Info, cipherKey, ref list);

            bool bResult = false;
            bResult = MatchList (bResult, responseType, list, resultList, pubnub.JsonPluggableLibrary);

            Assert.IsTrue (bResult);
        }

        bool MatchList(bool bResult, ResponseType responseType, List<object> list, List<object> resList, IJsonPluggableLibrary jsonPluggableLibrary){
            UnityEngine.Debug.Log ("list:" + jsonPluggableLibrary.SerializeToJsonString(list));  
            UnityEngine.Debug.Log ("resList:" + jsonPluggableLibrary.SerializeToJsonString(resList));  
            foreach (object obj in list) {
                UnityEngine.Debug.Log ("obj:" + obj.ToString ());    
                bool bResult1 = false;
                Type valueType = obj.GetType();
                var expectedType = typeof(String);
                var expectedType2 = typeof(Object);
                var expectedType3 = typeof(String[]);
                var expectedType4 = typeof(Object[]);
                foreach (object obj2 in resList) {
                    UnityEngine.Debug.Log ("obj2:" + obj2.ToString ());

                    if (valueType.IsArray && (expectedType.IsAssignableFrom (valueType.GetElementType ())
                        || expectedType2.IsAssignableFrom (valueType.GetElementType ())
                        || expectedType3.IsAssignableFrom (valueType.GetElementType ())
                        || expectedType4.IsAssignableFrom (valueType.GetElementType ())
                        )) {
                        UnityEngine.Debug.Log ("obj2:" + obj2.ToString () + valueType.IsArray
                        + expectedType.IsAssignableFrom (valueType.GetElementType ())
                        + expectedType2.IsAssignableFrom (valueType.GetElementType ())
                        + expectedType3.IsAssignableFrom (valueType.GetElementType ())
                        + expectedType4.IsAssignableFrom (valueType.GetElementType ())
                        + valueType.GetElementType ().ToString ());    

                        if (expectedType.IsAssignableFrom (valueType.GetElementType ())) {
                            string[] arrObj = ((System.Collections.IEnumerable)obj).Cast<object> ().Select (x => x.ToString ()).ToArray ();
                            foreach (string str in arrObj) {
                                UnityEngine.Debug.Log ("array:" + str);    
                            }
                            bResult1 = jsonPluggableLibrary.SerializeToJsonString (arrObj).Equals (obj2.ToString ());
                            UnityEngine.Debug.Log ("array" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (arrObj));
                        } else {
                            object[] arrObj = ((System.Collections.IEnumerable)obj).Cast<object> ().Select (x => x.ToString ()).ToArray ();
                            foreach (string str in arrObj) {
                                UnityEngine.Debug.Log ("obj array:" + str);    
                            }
                            bResult1 = jsonPluggableLibrary.SerializeToJsonString (arrObj).Equals (obj2.ToString ());
                            UnityEngine.Debug.Log ("obj array" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (arrObj));
                        }
                        if (bResult1) {
                            break;
                        }
                    } else if (obj.GetType().IsGenericType){
                        UnityEngine.Debug.Log ("generic:" + obj2.ToString());
                        bResult1 = jsonPluggableLibrary.SerializeToJsonString (obj).Equals (obj2.ToString ());
                        UnityEngine.Debug.Log ("obj generic" + bResult1 + jsonPluggableLibrary.SerializeToJsonString (obj));
                        if (bResult1) {
                            break;
                        }
                    } else {
                        UnityEngine.Debug.Log ("non array/generic:" + obj2.ToString());
                        if (obj.ToString().Equals (obj2.ToString())) {
                            UnityEngine.Debug.Log ("obj-obj2:" + obj.ToString () + "-" + obj2.ToString ());
                            bResult1 = true;
                            break;
                        } 
                    }
                }
                if (bResult1) {
                    bResult = true;
                } else {
                    bResult = false;
                    UnityEngine.Debug.Log ("obj not found:" + obj.ToString ());
                    break;
                }
            }
            return bResult;
        }

        [Test]
        public void TestJsonEncodePublishMsg(){ 
            string str = "test message";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );
            string encoded = Helpers.JsonEncodePublishMsg (str, "", pubnub.JsonPluggableLibrary);
            bool res = encoded.Equals ("\"test message\"");
            UnityEngine.Debug.Log ("res1:" + res + encoded);
            Assert.IsTrue(res);
        }
            
        [Test]
        public void TestJsonEncodePublishMsgCipher(){ 
            string str = "test message";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "enigma",
                true
            );
            string encoded = Helpers.JsonEncodePublishMsg (str, "enigma", pubnub.JsonPluggableLibrary);
            bool res = encoded.Equals ("\"UXgV6VPqJ7WI04csguMrqw==\"");
            UnityEngine.Debug.Log ("res2:" + res + encoded);
            Assert.IsTrue(res);
        }

        [Test]
        public void TestDecodeDecryptLoopCipher(){ 
            TestDecodeDecryptLoopCommon ("enigma", "test message", "UXgV6VPqJ7WI04csguMrqw==", false);
        }

        [Test]
        public void TestDecodeDecryptLoop(){ 
            TestDecodeDecryptLoopCommon ("", "test message", "test message", false);
        }
        [Test]
        public void TestDecodeDecryptLoopCipherMultiple(){ 
            TestDecodeDecryptLoopCommon ("enigma", "test message", "UXgV6VPqJ7WI04csguMrqw==", true);
        }

        [Test]
        public void TestDecodeDecryptLoopMultiple(){ 
            TestDecodeDecryptLoopCommon ("", "test message", "test message", true);
        }

        public void TestDecodeDecryptLoopCommon(string cipherKey, string resultExpected, string inputMessage, bool multiple){
            string[] multiChannel = {"testChannel3"};
            resultList = new List<object> ();
            List<object> resultList2 = new List<object> ();
            resultList2.Add (resultExpected);
            if (multiple) {
                resultList2.Add (resultExpected);
            }
            resultList.Add (resultList2);
            resultList.Add (string.Join(",", multiChannel));

            List<object> inputList = new List<object> ();
            List<object> inputList2 = new List<object> ();
            inputList2.Add (inputMessage);
            if (multiple) {
                inputList2.Add (inputMessage);
            }

            inputList.Add (inputList2);
            inputList.Add (string.Join(",", multiChannel));

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                cipherKey,
                true
            );

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<string>(multiChannel, 
                true, false, null, null, null, ErrorCallbackCommonExceptionHandler, null, null);  

            List<object> response = Helpers.DecodeDecryptLoop (inputList, channelEntities, cipherKey,
                pubnub.JsonPluggableLibrary, PubnubErrorFilter.Level.Info);
                
            bool bResult = false;
            string ser1 = pubnub.JsonPluggableLibrary.SerializeToJsonString (response);
            string ser2 = pubnub.JsonPluggableLibrary.SerializeToJsonString (resultList);
            UnityEngine.Debug.Log ("ser2:" + ser2);
            UnityEngine.Debug.Log ("ser1:" + ser1);
            bResult = (ser1.Equals (ser2));
            Assert.IsTrue (bResult);
        }

        [Test]
        public void TestDecodeMessage(){ 
            string str = "UXgV6VPqJ7WI04csguMrqw==";
            TestDecodeMessageCommon (str, "test message");
        }

        [Test]
        public void TestDecodeMessageError(){ 
            string str = "UXgV6VPqJ7WI04csguMrqw=";
            ExceptionCode = 0;
            readCallback = true;
            TestDecodeMessageCommon (str, "**DECRYPT ERROR**");
        }

        void TestDecodeMessageCommon(object inputMessage, string resultExpected){
            string[] multiChannel = {"testChannel3"};
            string cipherKey = "enigma";
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                cipherKey,
                true
            );

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<string>(multiChannel, 
                true, false, null, null, null, ErrorCallbackCommonExceptionHandler, null, null);  

            PubnubCrypto aes = new PubnubCrypto (cipherKey);
            object resp= Helpers.DecodeMessage(aes, inputMessage, channelEntities,
                pubnub.JsonPluggableLibrary, PubnubErrorFilter.Level.Info);

            UnityEngine.Debug.Log ("ser2:" + resultExpected.ToString());
            UnityEngine.Debug.Log ("ser1:" + resp.ToString());
            Assert.IsTrue (resp.Equals(resultExpected));
        }

        [Test]
        public void TestDeserializeAndAddToResult(){
            string str = "{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}";
            TestDeserializeAndAddToResultCommon (false, str, "status");
        }

        [Test]
        public void TestDeserializeAndAddToResultAddChannel(){
            string str = "{\"status\":200,\"message\":\"OK\",\"service\":\"Presence\",\"occupancy\":0}";
            TestDeserializeAndAddToResultCommon (true, str, "status");
        }

        public void TestDeserializeAndAddToResultCommon(bool addChannel, string str, string expectedResult){ 
            string channel = "testChannel3";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );

            List<object> lstObj = Helpers.DeserializeAndAddToResult (str, channel, 
                pubnub.JsonPluggableLibrary, addChannel);

            bool bRes = false;
            foreach (object obj in lstObj) {
                if (obj.GetType ().IsGenericType) {
                    UnityEngine.Debug.Log ("generic:" + obj.ToString ());
                    Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
                    bRes = dictionary.ContainsKey(expectedResult);
                    break;
                }
            }

            UnityEngine.Debug.Log ("ser1:" + bRes );
            Assert.IsTrue(bRes);
            if (addChannel) {
                bRes = lstObj.Contains (channel);
                UnityEngine.Debug.Log ("ser2:" + bRes);
                Assert.IsTrue (bRes);

            } else {
                bRes = lstObj.Contains (channel);
                UnityEngine.Debug.Log ("ser2:" + bRes);
                Assert.IsFalse (bRes);
            }
        }

        [Test]
        public void TestCreatePubnubClientError(){
            string[] multiChannel = {"testChannel3"};
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<string>(multiChannel, 
                true, false, null, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, 
                ErrorCallbackCommonExceptionHandler, null, null);  

            RequestState<string> requestState = BuildRequests.BuildRequestState<string> (channelEntities, ResponseType.SubscribeV2, 
                true, 0, true, 0, typeof(string), "", UserCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler);
            
            PubnubClientError error = Helpers.CreatePubnubClientError<string> ("Timed out", requestState, channelEntities, 
                PubnubErrorCode.OperationTimeout, PubnubErrorSeverity.Critical);
            
            UnityEngine.Debug.Log ("Timed out:" + error.Message);
            UnityEngine.Debug.Log (error.StatusCode + ":" + PubnubErrorCode.OperationTimeout);
            UnityEngine.Debug.Log (error.Severity + ":" + PubnubErrorSeverity.Critical);

            Assert.IsTrue (error.Message.Contains("Timed out"));
            Assert.IsTrue (error.StatusCode.Equals(138));
            Assert.IsTrue (error.Severity.Equals (PubnubErrorSeverity.Critical));
        }

        [Test]
        public void TestCreatePubnubClientErrorEx(){
            string[] multiChannel = {"testChannel3"};
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<string>(multiChannel, 
                true, false, null, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, 
                ErrorCallbackCommonExceptionHandler, null, null);  

            RequestState<string> requestState = BuildRequests.BuildRequestState<string> (channelEntities, ResponseType.SubscribeV2, 
                true, 0, true, 0, typeof(string), "", UserCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler);
            
            Exception ex = new Exception ("Test Exception");

            PubnubClientError error = Helpers.CreatePubnubClientError<string> (ex, requestState, channelEntities, 
                PubnubErrorCode.PubnubObjectDisposedException, PubnubErrorSeverity.Info);

            UnityEngine.Debug.Log (ex.Message + ":" + error.Message + error.Message.Contains(ex.Message));
            UnityEngine.Debug.Log (error.StatusCode + ":" + PubnubErrorCode.PubnubObjectDisposedException + error.StatusCode.Equals(107));
            UnityEngine.Debug.Log (error.Severity + ":" + PubnubErrorSeverity.Info + error.Severity.Equals (PubnubErrorSeverity.Info));

            Assert.IsTrue (error.Message.Contains(ex.Message));
            Assert.IsTrue (error.StatusCode.Equals(107));
            Assert.IsTrue (error.Severity.Equals (PubnubErrorSeverity.Info));
        }

        [Test]
        public void TestCreatePubnubClientErrorWebEx(){
            string[] multiChannel = {"testChannel3"};
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<string>(multiChannel, 
                true, false, null, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, 
                ErrorCallbackCommonExceptionHandler, null, null);  

            RequestState<string> requestState = BuildRequests.BuildRequestState<string> (channelEntities, ResponseType.SubscribeV2, 
                true, 0, true, 0, typeof(string), "", UserCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler);
            
            WebException wex = new WebException ("Test Web Exception");

            PubnubClientError error = Helpers.CreatePubnubClientError<string> (wex, requestState, string.Join(",", multiChannel), 
                PubnubErrorSeverity.Warn);

            UnityEngine.Debug.Log (wex.Message + ":" + error.Message);
            UnityEngine.Debug.Log (error.StatusCode + ":" + PubnubErrorCode.None);
            UnityEngine.Debug.Log (error.Severity + ":" + PubnubErrorSeverity.Warn);
            Assert.IsTrue (error.Message.Contains(wex.Message));
            Assert.IsTrue (error.StatusCode.Equals(0));
            Assert.IsTrue (error.Severity.Equals (PubnubErrorSeverity.Warn));

        }

        [Test]
        public void TestGetTimeOutErrorCodePresence(){
            TestGetTimeOutErrorCodeCommon (ResponseType.PresenceV2, PubnubErrorCode.OperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeAuditAccess(){
            TestGetTimeOutErrorCodeCommon (ResponseType.AuditAccess, PubnubErrorCode.PAMAccessOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeDetailedHistory(){
            TestGetTimeOutErrorCodeCommon (ResponseType.DetailedHistory, PubnubErrorCode.DetailedHistoryOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeGetUserState(){
            TestGetTimeOutErrorCodeCommon (ResponseType.GetUserState, PubnubErrorCode.GetUserStateTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeGlobalHereNow(){
            TestGetTimeOutErrorCodeCommon (ResponseType.GlobalHereNow, PubnubErrorCode.GlobalHereNowOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeSetUserState(){
            TestGetTimeOutErrorCodeCommon (ResponseType.SetUserState, PubnubErrorCode.SetUserStateTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeHereNow(){
            TestGetTimeOutErrorCodeCommon (ResponseType.HereNow, PubnubErrorCode.HereNowOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeTime(){
            TestGetTimeOutErrorCodeCommon (ResponseType.Time, PubnubErrorCode.TimeOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeWhereNow(){
            TestGetTimeOutErrorCodeCommon (ResponseType.WhereNow, PubnubErrorCode.WhereNowOperationTimeout);
        }

        [Test]
        public void TestGetTimeOutErrorCodeSubscribe(){
            TestGetTimeOutErrorCodeCommon (ResponseType.SubscribeV2, PubnubErrorCode.OperationTimeout);
        }

        void TestGetTimeOutErrorCodeCommon(ResponseType responseType, PubnubErrorCode expErrorCode){
            PubnubErrorCode errCode = Helpers.GetTimeOutErrorCode (responseType);
            Assert.IsTrue (errCode.Equals (expErrorCode));
        }
            
        [Test]
        public void TestCreateJsonResponsePresence(){
            TestCreateJsonResponseCommon ("Presence Connected");
        }

        [Test]
        public void TestCreateJsonResponseConnected(){
            TestCreateJsonResponseCommon ("Connected");
        }

        public void TestCreateJsonResponseCommon(string message){
            string channel = "testChannel3";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );

            List<object> lstObj = Helpers.CreateJsonResponse (message, channel, pubnub.JsonPluggableLibrary);
            object obj0 = lstObj [0];
            object obj1 = lstObj [1];
            object obj2 = lstObj [2];
            UnityEngine.Debug.Log (obj0.ToString() + obj1.ToString() + obj2.ToString());
            Assert.IsTrue (obj0.ToString ().Equals ("1"));
            Assert.IsTrue (obj1.ToString ().Equals (message));
            Assert.IsTrue (obj2.ToString ().Equals (channel));
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksUnsubscribe(){
            
            string[] multiChannel = {"testChannel3"};
            readCallback = true;
            ExceptionCode = 123;
            CheckMultiple = false;
            MessageCount = 0;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<string> (false, multiChannel, ResponseType.Unsubscribe, 
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksPresenceUnsubscribe(){
            
            string[] multiChannel = {"testChannel3"};
            readCallback = true;
            ExceptionCode = 124;
            CheckMultiple = false;
            MessageCount = 0;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<string> (true, multiChannel, ResponseType.PresenceUnsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksUnsubscribeMultiple(){
            
            string[] multiChannel = {"testChannel3", "testChannel4"};
            readCallback = true;
            ExceptionCode = 123;
            CheckMultiple = true;
            MessageReceivedCount = 0;
            MessageCount = 2;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<string> (false, multiChannel, ResponseType.Unsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksPresenceUnsubscribeMultiple(){
            
            string[] multiChannel = {"testChannel3", "testChannel4"};
            readCallback = true;
            ExceptionCode = 124;
            CheckMultiple = true;
            MessageReceivedCount = 0;
            MessageCount = 2;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<string> (true, multiChannel, ResponseType.PresenceUnsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksUnsubscribeObj(){
            
            string[] multiChannel = {"testChannel3"};
            readCallback = true;
            ExceptionCode = 123;
            CheckMultiple = false;
            MessageCount = 0;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<object> (false, multiChannel, ResponseType.Unsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksPresenceUnsubscribeObj(){
            
            string[] multiChannel = {"testChannel3"};
            readCallback = true;
            ExceptionCode = 124;
            CheckMultiple = false;
            MessageCount = 0;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<object> (true, multiChannel, ResponseType.PresenceUnsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksUnsubscribeMultipleObj(){
            
            string[] multiChannel = {"testChannel3", "testChannel4"};
            readCallback = true;
            ExceptionCode = 123;
            CheckMultiple = true;
            MessageReceivedCount = 0;
            MessageCount = 2;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<object> (false, multiChannel, ResponseType.Unsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestCheckSubscribedChannelsAndSendCallbacksPresenceUnsubscribeMultipleObj(){
            
            string[] multiChannel = {"testChannel3", "testChannel4"};
            readCallback = true;
            ExceptionCode = 124;
            CheckMultiple = true;
            MessageReceivedCount = 0;
            MessageCount = 2;
            TestCheckSubscribedChannelsAndSendCallbacksCommon<object> (true, multiChannel, ResponseType.PresenceUnsubscribe,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        public void TestCheckSubscribedChannelsAndSendCallbacksCommon<T>(bool isPresence, string[] multiChannel, 
            ResponseType responseType, Action<T> userCallback, Action<T> connectCallback

        ){
            Action<PubnubClientError> errorCallback = ErrorCallbackCommonExceptionHandler2;


            MessageToCheck = "Unsubscribed after 10";
            if (isPresence) {
                MessageToCheck = "Presence Unsubscribed after 10";
            }
            CheckMessage = true;
            if(isPresence){
                for(int i=0; i<multiChannel.Length; i++ ){
                    multiChannel[i] += Utility.PresenceChannelSuffix;
                }
            }

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(multiChannel, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                false, 0, false, 0, typeof(T), "", userCallback, errorCallback);
            

            Helpers.CheckSubscribedChannelsAndSendCallbacks<T>    (channelEntities, responseType, 10, 
                PubnubErrorFilter.Level.Critical);
        }

        internal void UserCallbackCommonExceptionHandler2 (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
            if (ExpectedCallback) {
                bool bRes = result.Equals (ExpectedRegularResponse);
                UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK: {0}", bRes));

                Assert.IsTrue (bRes);
            }

        }

        internal void UserCallbackCommonExceptionHandler2 (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
            if (ExpectedCallback) {
                bool bRes = result.Equals (ExpectedRegularResponse);
                UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK Obj: {0}", bRes));

                Assert.IsTrue (bRes);
            }
        }

        public void ConnectCallbackCommonExceptionHandler2 (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
            if (ExpectedConnect) {
                bool bRes = result.Equals (ExpectedConnectResponse);
                UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK: {0}", bRes));
                Assert.IsTrue (bRes);
            }
        }

        public void ConnectCallbackCommonExceptionHandler2 (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
            if (ExpectedConnect) {
                Pubnub pubnub = new Pubnub (
                    Common.PublishKey,
                    Common.SubscribeKey,
                    "",
                    "",
                    true
                );
                bool bRes = pubnub.JsonPluggableLibrary.SerializeToJsonString (result).Equals (ExpectedConnectResponse);
                UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK Obj: {0}", bRes));
                Assert.IsTrue (bRes);
            }
        }

        [Test]
        public void TestProcessWrapResultBasedOnResponseTypeException(){
            string[] multiChannel = {"testChannel3"};
            TestProcessWrapResultBasedOnResponseTypeExceptionCommon<string> (multiChannel, ResponseType.PresenceV2,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestProcessWrapResultBasedOnResponseTypeExceptionDH(){
            string[] multiChannel = {"testChannel3"};
            TestProcessWrapResultBasedOnResponseTypeExceptionCommon<string> (multiChannel, ResponseType.DetailedHistory,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestProcessWrapResultBasedOnResponseTypeExceptionObj(){
            string[] multiChannel = {"testChannel3"};
            TestProcessWrapResultBasedOnResponseTypeExceptionCommon<object> (multiChannel, ResponseType.PresenceV2,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        [Test]
        public void TestProcessWrapResultBasedOnResponseTypeExceptionDHObj(){
            string[] multiChannel = {"testChannel3"};
            TestProcessWrapResultBasedOnResponseTypeExceptionCommon<object> (multiChannel, ResponseType.DetailedHistory,
                UserCallbackCommonExceptionHandler2, ConnectCallbackCommonExceptionHandler2);
        }

        public void TestProcessWrapResultBasedOnResponseTypeExceptionCommon<T>(string[] multiChannel, 
            ResponseType responseType, Action<T> userCallback, Action<T> connectCallback){
            Action<PubnubClientError> errorCallback = ErrorCallbackCommonExceptionHandler2;

            readCallback = true;
            ExceptionCode = 0;
            MessageToCheck = "Test exception";
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(multiChannel, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                false, 0, false, 0, typeof(T), "", userCallback, errorCallback);
            
            Helpers.ProcessWrapResultBasedOnResponseTypeException<T> (requestState, PubnubErrorFilter.Level.Critical, 
                new Exception (MessageToCheck));
        }

        /*[Test]
        public void TestCreateMessageListString(){
            TestCreateMessageListCommon ("test message");
        }

        [Test]
        public void TestCreateMessageListInt(){
            TestCreateMessageListCommon (1);
        }

        [Test]
        public void TestCreateMessageListDouble(){
            TestCreateMessageListCommon (1.2);
        }

        [Test]
        public void TestCreateMessageListInt64(){
            TestCreateMessageListCommon (14498416434364941);
        }

        [Test]
        public void TestCreateMessageListBool(){
            TestCreateMessageListCommon (true);
        }

        [Test]
        public void TestCreateMessageListArr(){
            object[] obj = new object[]{};
            TestCreateMessageListCommon (obj);
        }

        public void TestCreateMessageListCommon(object input){
            string[] multiChannel = {"testChannel3"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            object[] obj = new object[1];
            obj [0] = input;
            result2.Add (obj);
            result.Add (result2);
            result.Add (14498416434364941);
            result.Add (string.Join(",", multiChannel));

            var messages = (from item in result
                select item as object).ToArray ();
            
            var messageList = messages [0] as object[];

            messageList = Helpers.CreateMessageList (result, messageList);
            for (int messageIndex = 0; messageIndex < messageList.Length; messageIndex++)
            {
                object[] objList = messageList[messageIndex] as object[];
                for(int messageIndex1 = 0; messageIndex1 < objList.Length; messageIndex1++){
                    string message = objList [messageIndex1].ToString ();
                    UnityEngine.Debug.Log (message);
                    if (message.Equals (input)) {
                        Assert.IsTrue (true);
                        break;
                    }
                }
            }

        }

        [Test]
        public void TestAddMessageToList(){
            string cipherKey= "";
            TestAddMessageToListCommon (cipherKey, "test message", "test message");
        }

        //[Test]
        public void TestAddMessageToListCipher(){
            string cipherKey= "enigma";
            TestAddMessageToListCommon (cipherKey, "test message", "UXgV6VPqJ7WI04csguMrqw==");
        }
            
        public void TestAddMessageToListCommon(string cipherKey, string expected, string input){
            
            string[] multiChannel = {"testChannel3"};
            List<object> result = new List<object> ();
            List<object> result2 = new List<object> ();
            string[] obj = new string[1];
            obj [0] = input;
            result2.Add (obj);
            result.Add (result2);
            result.Add (14498416434364941);
            string currentChannel = string.Join (",", multiChannel);
            result.Add (currentChannel);

            var messages = (from item in result
                select item as object).ToArray ();

            var messageList = messages [0] as object[];

            messageList = Helpers.CreateMessageList (result, messageList);
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                cipherKey,
                true
            );

            List<object> resp = Helpers.AddMessageToList (cipherKey, pubnub.JsonPluggableLibrary, messages, 0, currentChannel, messageList);
            object obj0 = resp [0];
            object obj1 = resp [1];
            object obj2 = resp [2];
            UnityEngine.Debug.Log (obj0.ToString() + obj1.ToString() + obj2.ToString());
            object[] objList = obj0 as object[];
            UnityEngine.Debug.Log (objList.Length);
            for (int messageIndex1 = 0; messageIndex1 < objList.Length; messageIndex1++) {
                string message = objList [messageIndex1].ToString ();
                UnityEngine.Debug.Log (message);
                if (message.Equals (expected)) {
                    Assert.IsTrue (true);
                    break;
                }
            }
            Assert.IsTrue (obj1.ToString ().Equals ("14498416434364941"));
            Assert.IsTrue (obj2.ToString ().Equals (currentChannel));

        }*/


        #endif
    }
}
