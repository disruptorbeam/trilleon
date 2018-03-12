//#define USE_MiniJSON
using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class WhenAMessageIsPublished
    {
        [Test]
        public void ThenItShouldGenerateUniqueIdentifier ()
        {
            Pubnub pubnub = new Pubnub (
                          "demo",
                          "demo",
                          "",
                          "",
                          false
                      );

            Assert.IsNotNull (pubnub.GenerateGuid ());
        }

    }
}

