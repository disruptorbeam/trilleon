using System;
using PubNubMessaging.Core;
using NUnit.Framework;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class PubnubChannelsUnitTests
    {
        #if DEBUG
        [Test]
        public void TestGetPubnubChannelCallback(){ 
            TestGetPubnubChannelCallbackCommon<string> (UserCallback, ConnectCallback, ErrorCallback, 
                DisconnectCallback, WildcardCallback);
        }

        [Test]
        public void TestGetPubnubChannelCallbackObj(){ 
            TestGetPubnubChannelCallbackCommon<object> (UserCallback, ConnectCallback, ErrorCallback, 
                DisconnectCallback, WildcardCallback);
        }

        public void TestGetPubnubChannelCallbackCommon<T>(Action<T> userCallback, Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback, Action<T> disconnectCallback, Action<T> wildcardPresenceCallback){
            PubnubChannelCallback<T> cb = PubnubCallbacks.GetPubnubChannelCallback(userCallback, connectCallback, 
                errorCallback, disconnectCallback, wildcardPresenceCallback);
            Assert.True(
                cb.ConnectCallback.Equals(connectCallback)
                && cb.SuccessCallback.Equals(userCallback)
                && cb.ErrorCallback.Equals(errorCallback)
                && cb.DisconnectCallback.Equals(disconnectCallback)
                && cb.WildcardPresenceCallback.Equals(wildcardPresenceCallback)
            );
        }

        void ErrorCallback (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format ("Exception handler: {0}", result));
        }

        void UserCallback<T> (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
        }

        void UserCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
        }

        void DisconnectCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        void WildcardCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        void DisconnectCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result.ToString()));
        }

        void WildcardCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("Wildcard CALLBACK LOG: {0}", result.ToString()));
        }

        void ConnectCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
        }

        void ConnectCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
        }
        /*public void TestGetPubnubChannelCallbackKey (){ 
            string channel = "testchannel";
            ResponseType respType = ResponseType.SubscribeV2;
            /*PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey ();
            callbackKey.Channel = channel;
            callbackKey.Type = respType;

            PubnubChannelCallbackKey callbackKey2 = PubnubCallbacks.GetPubnubChannelCallbackKey(channel, respType);
            Assert.IsTrue (callbackKey.Channel.Equals(callbackKey2.Channel) && callbackKey.Type.Equals(callbackKey2.Type));

        }*/
        #endif
    }
}

