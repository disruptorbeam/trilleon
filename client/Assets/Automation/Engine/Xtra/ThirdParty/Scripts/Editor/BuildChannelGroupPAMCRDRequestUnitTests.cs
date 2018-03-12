using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildChannelGroupPAMCRDRequestUnitTests
    {
       
        #if DEBUG    
        [Test]
        public void BuildAddChannelsToChannelGroupRequest ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSL ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestNoAuth ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLNoAuth ()
        {
            string [] channels = {"addChannel1, addChannel2"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestNoAuthCH ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildAddChannelsToChannelGroupRequestSSLNoAuthCh ()
        {
            string [] channels = {"addChannel1"};
            TestBuildAddChannelsToChannelGroupRequestCommon (channels, true, "");
        }

        public void TestBuildAddChannelsToChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildAddChannelsToChannelGroupRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = string.Join(",", channels);

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}?add={4}&uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupAdd, true, true),
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAdd, false, true)

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestSSL ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSL ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestSSLAuth ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSLAuthMulti ()
        {
            string [] channels = {"addChannel1", "addChannel2"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestSSLAuth ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, true, "authKey");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequest ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequest ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "");
        }

        [Test]
        public void BuildBuildRemoveAllChannelsFromChannelGroupRequestAuth ()
        {
            string [] channels = null;
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "authKey");
        }


        [Test]
        public void BuildBuildRemoveChannelsFromChannelGroupRequestAuth ()
        {
            string [] channels = {"addChannel1"};
            TestBuildRemoveChannelsFromChannelGroupRequestCommon (channels, false, "authKey");
        }

        //remove channels
        //remove cg
        public void TestBuildRemoveChannelsFromChannelGroupRequestCommon(string[] channels, bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl


            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest (channels, "", channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            string ch = "";
            string chStr = "";
            string chStr2 = "/remove";
            if (channels != null && channels.Length > 0) {
                ch = string.Join(",", channels);
                chStr = string.Format ("remove={0}&", Utility.EncodeUricomponent(ch, ResponseType.ChannelGroupRemove, true, false));
                chStr2 = "";
            }

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/channel-group/{3}{8}?{4}uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                chStr, uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupRemove, false, true),
                chStr2

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequest ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, false, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSL ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, true, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAuth ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, false, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAuth ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (false, true, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, false, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, true, "");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestAuthAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, false, "authKey");
        }

        [Test]
        public void TestBuildGetChannelsForChannelGroupRequestSSLAuthAll ()
        {
            TestBuildGetChannelsForChannelGroupRequestCommon (true, true, "authKey");
        }

        //GetChannels
        //Get All CG
        public void TestBuildGetChannelsForChannelGroupRequestCommon(bool allCg, 
            bool ssl, string authKey){

            string channelGroup = "channelGroup";
            string channelGroupStr ="channel-group/";
            if(allCg){
                channelGroup = "";
                channelGroupStr= "channel-group";
            }
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildGetChannelsForChannelGroupRequest ("", channelGroup, allCg,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            //http://ps.pndsn.com/v1/channel-registration/sub-key/demo-36/channel-group/channelGroup?add=addChannel1,%20addChannel2&uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            string expected = string.Format ("http{0}://{1}/v1/channel-registration/sub-key/{2}/{8}{3}?uuid={5}{6}&pnsdk={7}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channelGroup, 
                "",
                uuid, authKeyString, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupGet, false, true),
                channelGroupStr

            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequest()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSL()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestAuth()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLAuth()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSubKey()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLSubKey()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestAuthSubKey()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLAuthSubKey()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestAuthCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLAuthCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (false, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSubKeyCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLSubKeyCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestAuthSubKeyCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupAuditAccessRequestSSLAuthSubKeyCipher()
        {
            TestBuildChannelGroupAuditAccessRequestCommon (true, true, true, "authKey");
        }

        public void TestBuildChannelGroupAuditAccessRequestCommon(bool subKeyLevel, bool ssl,  bool useCipher, string authKey){

            string channelGroup = "channelGroup";
            if(subKeyLevel){
                channelGroup ="";
            }
            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );

            string cipher = "";
            if(useCipher){
                cipher = "enigma";
            }
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildChannelGroupAuditAccessRequest (channelGroup,
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.PublishKey, 
                Common.SubscribeKey, cipher, Common.SecretKey
            );

            //http://ps.pndsn.com/v1/auth/audit/sub-key/demo-36?
            //signature=5WXdNx4N-ahcZg1ouczRy0_8iww0o3cS1hBIgdd3BR8
            //=&channel-group=channelGroup&pnsdk=PubNub-CSharp-UnityOSX%2F3.7
            //&timestamp=1468320915&uuid=customuuid
            string expected1 = string.Format ("http{0}://{1}/v1/auth/audit/sub-key/{2}?signature=",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey

            );

            string expected2 = string.Format ("{3}{0}{1}&pnsdk={2}&timestamp=",
                (channelGroup=="")?"":"&channel-group=", channelGroup, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupGrantAccess, false, true),
                authKeyString
                );

            string expected3 = string.Format ("&uuid={0}",
                uuid
            );

            string received = uri.OriginalString;
            UnityEngine.Debug.Log(string.Format("Expected1: {0}\nExpected2: {1}\nExpected3: {2}\nReceived: {3}\n", 
                expected1, expected2, expected3, received));
        
            if (!received.Contains (expected1)) {
                Assert.Fail ("expected1 doesn't match");
            }
            if (!received.Contains (expected2)) {
                Assert.Fail ("expected2 doesn't match");
            }
            if (!received.Contains (expected3)) {
                Assert.Fail ("expected3 doesn't match");
            }

            Assert.IsTrue (true);
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequest()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuth()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKey()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuth()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestR()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthR()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyR()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthR()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRM()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, false, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMTTL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, true, false, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMTTLCipher()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, true, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestCommonAuth2()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, true, true, 0, false, false, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeySSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, false, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMTTLSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, false, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 0, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, false, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, false, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, false, false, true, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, false, false, true, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, false, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, false, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestRMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestAuthRMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (false, true, false, true, 10, true, true, "authKey");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyRMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, true, true, "");
        }

        [Test]
        public void TestBuildChannelGroupGrantAccessRequestSubKeyAuthRMTTLCipherSSL()
        {
            TestBuildChannelGroupGrantAccessRequestCommon (true, true, false, true, 10, true, true, "authKey");
        }

        public void TestBuildChannelGroupGrantAccessRequestCommon(bool subKeyLevel, 
            bool read, bool write, bool manage, int ttl, bool useCipher,
            bool ssl, string authKey){

            string channelGroup = "channelGroup";
            if(subKeyLevel){
                channelGroup ="";
            }

            string uuid = "customuuid";

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            string cipher = "";
            if(useCipher){
                cipher = "enigma";
            }

            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            Uri uri = BuildRequests.BuildChannelGroupGrantAccessRequest (channelGroup, 
                read, write, manage, ttl, 
                uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, 
                Common.PublishKey, Common.SubscribeKey, cipher, Common.SecretKey 
            );


            //http://ps.pndsn.com/v1/auth/grant/sub-key/demo-36?signature=EW8fnNVSpUYDEKKC_iPGyc7Bjq4zOyjKMAkCcQ3islM=
            //&auth=authKey&m=1&pnsdk=PubNub-CSharp-UnityOSX%2F3.7&r=1&timestamp=1468324416&ttl=0&uuid=customuuid
            string expected1 = string.Format ("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=",
                ssl ? "s" : "", pubnub.Origin, Common.SubscribeKey);

            string expected2 = string.Format ("{4}{0}{1}&m={5}&pnsdk={2}&r={3}&timestamp=",
                (channelGroup=="")?"":"&channel-group=", channelGroup, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupGrantAccess, false, true), 
                Convert.ToInt32 (read).ToString(),
                authKeyString,
                Convert.ToInt32 (manage).ToString()
            );

            string expected3 = string.Format ("&ttl={0}&uuid={1}",
                ttl, uuid
            );

            string received = uri.OriginalString;
            UnityEngine.Debug.Log(string.Format("Expected1: {0}\nExpected2: {1}\nExpected3: {2}\nReceived: {3}\n", 
                expected1, expected2, expected3, received));

            if (!received.Contains (expected1)) {
                Assert.Fail ("expected1 doesn't match");
            }
            if (!received.Contains (expected2)) {
                Assert.Fail ("expected2 doesn't match");
            }
            if (!received.Contains (expected3)) {
                Assert.Fail ("expected3 doesn't match");
            }

            Assert.IsTrue (true);
        }

        #endif
    }
}

