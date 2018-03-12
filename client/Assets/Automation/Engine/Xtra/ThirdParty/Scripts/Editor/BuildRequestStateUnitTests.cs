using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using System.Collections.Generic;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildRequestStateUnitTests
    {
        #if DEBUG    
        [Test]
        public void BuildAddChannelsToChannelGroupRequestCB ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<string> (false, 1234, false, 12345,
                UserCallback, ErrorCallback, true, false);
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCEnCB ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<string> (false, 1234, true, 12345,
                UserCallback, ErrorCallback, true, true);
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCE ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<string> (true, 1234, false, 12345,
                UserCallback, ErrorCallback, false, true);
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCBObj ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<object> (false, 1234, false, 12345,
                UserCallback, ErrorCallback, true, false);
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCEnCBObj ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<object> (false, 1234, true, 12345,
                UserCallback, ErrorCallback, true, true);
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCEObj ()
        {
            TestBuildAddChannelsToChannelGroupRequestCommon<object> (true, 1234, false, 12345,
                UserCallback, ErrorCallback, false, true);
        }


        public void TestBuildAddChannelsToChannelGroupRequestCommon<T>(bool reconnect, long id, bool timeout, 
            long timetoken, Action<T> userCallback, Action<PubnubClientError>  
            errorCallback, bool testCallbacks, bool testChannelEntities){

            string [] channels = {"addChannel1, addChannel2"};
            string uuid = "CustomUUID";
            bool testUUID = false;

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(channels, 
                true, false, null, userCallback, 
                null, errorCallback, null, null);  

            RequestState<T> requestState = null;
            if(testChannelEntities && testCallbacks){
                requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                    ResponseType.GrantAccess, reconnect, id, timeout, timetoken, typeof(T), uuid,
                    userCallback, errorCallback
                );
                testUUID = true;
            } else if (testChannelEntities){
                requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                    ResponseType.GrantAccess, reconnect, id, timeout, timetoken, typeof(T));
                
            } else {
                requestState = BuildRequests.BuildRequestState<T> (userCallback, errorCallback, 
                    ResponseType.GrantAccess, reconnect, id, timeout, timetoken, typeof(T), uuid);
                testUUID = true;
            }

            bool channelEntitiesMatch = false;

            if(!testChannelEntities){
                channelEntitiesMatch = true;
            }else {
                if (channelEntities != null) {
                    foreach (ChannelEntity c in channelEntities) {
                        channelEntitiesMatch = requestState.ChannelEntities.Contains(c);
                        if(!channelEntitiesMatch)
                            break;
                    }
                }
            }

            string str = string.Format(
                "{0}\n" +
                "{1}\n" +
                "{2}\n" +
                "{3}\n" +
                "{4}\n" +
                "{5}\n" +
                "{6}\n" +
                "{7}\n" +
                "{8}\n" +
                "{9}\n", 
                requestState.ID.Equals(id), requestState.Reconnect.Equals(reconnect),
                requestState.Timeout.Equals(timeout), requestState.Timetoken.Equals(timetoken),
                (testCallbacks)?requestState.SuccessCallback.Equals(userCallback):true,
                (testCallbacks)?requestState.ErrorCallback.Equals(errorCallback):true,
                requestState.Timetoken.Equals(timetoken),
                channelEntitiesMatch,
                typeof(T).Equals(requestState.TypeParameterType),
                (testUUID)?uuid.Equals(requestState.UUID):true
            );

            UnityEngine.Debug.Log(str);

            Assert.True(requestState.ID.Equals(id) && requestState.Reconnect.Equals(reconnect)
                && requestState.Timeout.Equals(timeout) && requestState.Timetoken.Equals(timetoken)
                && (testCallbacks)?requestState.SuccessCallback.Equals(userCallback):true
                && (testCallbacks)?requestState.ErrorCallback.Equals(errorCallback):true
                && requestState.Timetoken.Equals(timetoken)
                && channelEntitiesMatch
                && typeof(T).Equals(requestState.TypeParameterType)
                && (testUUID)?uuid.Equals(requestState.UUID):true
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

        void DisconnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        void ConnectCallback (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
        }

        void ConnectCallback (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
        }

        #endif
    }
}

