using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using UnityEngine;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildAccessManagerRequestsUnitTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildGrantRequestSSL ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSL ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipher ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequest ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuth ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuth ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuth ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuth ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoRead ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoReadWrite ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoReadTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, true, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, true, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestSSLNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestCipherNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestSSLAuthNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherSSLAuthNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,true, false, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestCipherAuthNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestAuthNoReadWriteTTL0 ()
        {
            string channel = "access_manager_channel"; TestBuildGrantRequestCommon (channel,false, false, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSL ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSL ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipher ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannel ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuth ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuth ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuth ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuth ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoRead ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 10, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 10, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 10, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoReadWrite ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 10, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoReadTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, true, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, true, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, true, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 0, "enigma", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 0, "", "");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelSSLAuthNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 0, "", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherSSLAuthNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,true, false, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelCipherAuthNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 0, "enigma", "authkey");
        }

        [Test]
        public void TestBuildGrantRequestNoChannelAuthNoReadWriteTTL0 ()
        {
            string channel = "";  TestBuildGrantRequestCommon (channel,false, false, false, 0, "", "authkey");
        }

        public void TestBuildGrantRequestCommon(string channel, bool ssl, bool read, bool write, int ttl, 
            string cipherKey, string authKey){
            string uuid = "customuuid";
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            Uri uri = BuildRequests.BuildGrantAccessRequest (channel, read, write, ttl, uuid, ssl, pubnub.Origin,
                authKey, Common.PublishKey, Common.SubscribeKey, cipherKey, Common.SecretKey
            );

            //https://ps.pndsn.com/v1/auth/grant/sub-key/demo-36?signature=RlJ5QMGPMxNj9C2J6emNaXUymQ8pbsUM3y8_Wz25Zdg=&channel=access_manager_channel&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0&r=1&timestamp=1450074813&ttl=10&uuid=customuuid&w=1
            string expected1 = string.Format ("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=",
                                  ssl ? "s" : "", pubnub.Origin, Common.SubscribeKey);

            string expected2 = string.Format ("{0}{1}&pnsdk={2}&r={3}&timestamp=",
                (channel=="")?"":"&channel=", channel, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.GrantAccess, false, true), 
                Convert.ToInt32 (read).ToString());
            
            string expected3 = string.Format ("&ttl={0}&uuid={1}&w={2}",
                ttl, uuid, Convert.ToInt32 (write).ToString()
            );

            string received = uri.OriginalString;
            UnityEngine.Debug.Log("Expected:" + expected1);
            UnityEngine.Debug.Log("Expected:" + expected2);
            UnityEngine.Debug.Log("Expected:" + expected3);
            UnityEngine.Debug.Log("Received:" + received);

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
        public void TestBuildAuditRequest ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, false, "", "");
        }

        [Test]
        public void TestBuildAuditRequestAuth ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, false, "", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestSSL ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, true, "", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthSSL ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, true, "", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestCipher ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, false, "enigma", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthCipher ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, false, "enigma", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestSSLCipher ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, true, "enigma", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthSSLCipher ()
        {
            string channel = "access_manager_channel";
            TestBuildAuditRequestCommon (channel, true, "enigma", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, false, "", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, false, "", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestSSLNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, true, "", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthSSLNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, true, "", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestCipherNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, false, "enigma", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthCipherNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, false, "enigma", "authkey");
        }

        [Test]
        public void TestBuildAuditRequestSSLCipherNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, true, "enigma", "");
        }

        [Test]
        public void TestBuildAuditRequestAuthSSLCipherNoChannel ()
        {
            string channel = "";
            TestBuildAuditRequestCommon (channel, true, "enigma", "authkey");
        }

        public void TestBuildAuditRequestCommon(string channel, bool ssl, string cipherKey, string authKey){
            string uuid = "customuuid";
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            Uri uri = BuildRequests.BuildAuditAccessRequest (channel, uuid, ssl, pubnub.Origin,
                authKey, Common.PublishKey, Common.SubscribeKey, cipherKey, Common.SecretKey
            );

            //http://ps.pndsn.com/v1/auth/audit/sub-key/demo-36?signature=z3fwOXFHyyaSfEbt8QwAvVzCviLgxxefRmPMDv7Ipns=&channel=access_manager_channel&pnsdk=PubNub-CSharp-UnityIOS/3.6.9.0&timestamp=1450102082&uuid=customuuid
            string expected1 = string.Format ("http{0}://{1}/v1/auth/audit/sub-key/{2}?signature=",
                ssl ? "s" : "", pubnub.Origin, Common.SubscribeKey);

            string expected2 = string.Format ("{0}{1}&pnsdk={2}&timestamp=",
                (channel=="")?"":"&channel=", channel, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.AuditAccess, false, true));

            string expected3 = string.Format ("&uuid={0}", uuid);

            string received = uri.OriginalString;
            UnityEngine.Debug.Log("Expected:" + expected1);
            UnityEngine.Debug.Log("Expected:" + expected2);
            UnityEngine.Debug.Log("Expected:" + expected3);
            UnityEngine.Debug.Log("Received:" + received);

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

