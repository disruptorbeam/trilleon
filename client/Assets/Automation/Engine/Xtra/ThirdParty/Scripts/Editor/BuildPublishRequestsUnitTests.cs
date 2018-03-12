using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildPublishRequestsUnitTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildPublishRequest ()
        {
            TestBuildPublishRequestCommon (false, true, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSL ()
        {
            TestBuildPublishRequestCommon (true, true, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestNoStore ()
        {
            TestBuildPublishRequestCommon (false, false, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStore ()
        {
            TestBuildPublishRequestCommon (true, false, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStore ()
        {
            TestBuildPublishRequestCommon (false, false, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStore ()
        {
            TestBuildPublishRequestCommon (true, false, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSL ()
        {
            TestBuildPublishRequestCommon (true, true, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipher ()
        {
            TestBuildPublishRequestCommon (false, true, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestSecret ()
        {
            TestBuildPublishRequestCommon (false, true, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLSecret ()
        {
            TestBuildPublishRequestCommon (true, true, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreSecret ()
        {
            TestBuildPublishRequestCommon (false, false, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreSecret ()
        {
            TestBuildPublishRequestCommon (true, false, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreSecret ()
        {
            TestBuildPublishRequestCommon (false, false, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreSecret ()
        {
            TestBuildPublishRequestCommon (true, false, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLSecret ()
        {
            TestBuildPublishRequestCommon (true, true, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSecret ()
        {
            TestBuildPublishRequestCommon (false, true, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestAuth ()
        {
            TestBuildPublishRequestCommon (false, true, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLAuth ()
        {
            TestBuildPublishRequestCommon (true, true, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreAuth ()
        {
            TestBuildPublishRequestCommon (false, false, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreAuth ()
        {
            TestBuildPublishRequestCommon (true, false, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreAuth ()
        {
            TestBuildPublishRequestCommon (false, false, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreAuth ()
        {
            TestBuildPublishRequestCommon (true, false, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLAuth ()
        {
            TestBuildPublishRequestCommon (true, true, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherAuth ()
        {
            TestBuildPublishRequestCommon (false, true, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSecretAuth ()
        {
            TestBuildPublishRequestCommon (false, true, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLSecretAuth ()
        {
            TestBuildPublishRequestCommon (true, true, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreSecretAuth ()
        {
            TestBuildPublishRequestCommon (false, false, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreSecretAuth ()
        {
            TestBuildPublishRequestCommon (true, false, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreSecretAuth ()
        {
            TestBuildPublishRequestCommon (false, false, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreSecretAuth ()
        {
            TestBuildPublishRequestCommon (true, false, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLSecretAuth ()
        {
            TestBuildPublishRequestCommon (true, true, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuth ()
        {
            TestBuildPublishRequestCommon (false, true, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "", "", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "Secret", "", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "Secret", "enigma", "");
        }

        [Test]
        public void TestBuildPublishRequestAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestNoStoreSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestSSLNoStoreSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "Secret", "", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherNoStoreSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, false, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLNoStoreSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, false, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSSLSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, true, true, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuthMeta ()
        {
            TestBuildPublishRequestCommon (true, false, true, "Secret", "enigma", "authKey");
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuthMetaStoreTrueTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, true, "Secret", "enigma", "authKey", 10);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuthMetaStoreTrueNoTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, true, "Secret", "enigma", "authKey", -1);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuthMetaStoreFalseNoTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, false, "Secret", "enigma", "authKey", -1);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretAuthMetaStoreFalseTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, false, "Secret", "enigma", "authKey", 10);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretMetaStoreTrueTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, true, "Secret", "enigma", "", 10);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretMetaStoreTrueNoTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, true, "Secret", "enigma", "", -1);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretMetaStoreFalseNoTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, false, "Secret", "enigma", "", -1);
        }

        [Test]
        public void TestBuildPublishRequestCipherSecretMetaStoreFalseTTL ()
        {
            TestBuildPublishRequestCommonWithTTL (true, false, false, "Secret", "enigma", "", 10);
        }

        public void TestBuildPublishRequestCommon(bool ssl, bool storeInHistory, string secretKey, 
            string cipherKey, string authKey){
            TestBuildPublishRequestCommon(false, ssl, storeInHistory, secretKey, cipherKey, authKey);
        }

        public void TestBuildPublishRequestCommonWithTTL(bool sendMeta, bool ssl, bool storeInHistory, string secretKey, 
            string cipherKey, string authKey, int ttl){
            string channel = "publish_channel";
            string message = "Test message";
            string uuid = "customuuid";
            string signature = "0";
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                secretKey,
                cipherKey,
                ssl
            );
            pubnub.AuthenticationKey = authKey;
            string authKeyString = "";
            if (!string.IsNullOrEmpty(authKey)) {
                authKeyString = string.Format ("&auth={0}", pubnub.AuthenticationKey);
            }

            string meta = "";
            string metadata = "{\"region\":\"east\"}";
            if (sendMeta) {
                meta =  string.Format ("&meta={0}", Utility.EncodeUricomponent (metadata, ResponseType.Publish, false, false));
            } else {
                metadata = "";
            }

            string originalMessage = Helpers.JsonEncodePublishMsg (message, cipherKey, pubnub.JsonPluggableLibrary);
            if (secretKey.Length > 0) {
                StringBuilder stringToSign = new StringBuilder ();
                stringToSign
                    .Append (Common.PublishKey)
                    .Append ('/')
                    .Append (Common.SubscribeKey)
                    .Append ('/')
                    .Append (secretKey)
                    .Append ('/')
                    .Append (channel)
                    .Append ('/')
                    .Append (originalMessage); // 1

                // Sign Message
                signature = Utility.Md5 (stringToSign.ToString ());
            }

            Uri uri = BuildRequests.BuildPublishRequest (channel, originalMessage, storeInHistory, uuid, ssl, 
                pubnub.Origin, pubnub.AuthenticationKey, Common.PublishKey, Common.SubscribeKey,
                cipherKey, secretKey, metadata, 0, ttl
            );

            string ttlStr = (ttl == -1) ? "" : string.Format("&ttl={0}", ttl.ToString());

            //http://ps.pndsn.com/publish/demo-36/demo-36/0/publish_channel/0?uuid=customuuid&auth=authKey&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            string expected = string.Format ("http{0}://{1}/publish/{2}/{3}/{4}/{5}/0/{6}?uuid={7}&seqn=0{8}{12}{11}{9}&pnsdk={10}",
                ssl?"s":"", pubnub.Origin, Common.PublishKey, Common.SubscribeKey, signature, channel, 
                Utility.EncodeUricomponent(originalMessage, ResponseType.Publish, false, false), 
                uuid, storeInHistory?"":"&store=0", authKeyString,
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.Publish, false, false),
                meta,
                ttlStr
            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }

        public void TestBuildPublishRequestCommon(bool sendMeta, bool ssl, bool storeInHistory, string secretKey, 
            string cipherKey, string authKey){
            TestBuildPublishRequestCommonWithTTL(false, ssl, storeInHistory, secretKey, cipherKey, authKey, -1);
        }

        #endif
    }
}

