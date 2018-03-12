using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class BuildDetailedHistoryRequestUnitTests
    {
        #if DEBUG    
        [Test]
        public void TestBuildDetailedHistoryRequest ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSL ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTT ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverse ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSL ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSL ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSL ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLEnd ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStart ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartEnd ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLEndNoAuth ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartNoAuth ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartEndNoAuth ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, 90);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLEndNoCount  ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartNoCount  ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartEndNoCount  ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "authKey", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = 14498416799269095;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartNoAuthNoCount ()
        {
            long startTime = 14498416434364941;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (false, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTSSLStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, false, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestSSLReverseSSLStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, false, true, "", startTime, endTime, -1);
        }

        [Test]
        public void TestBuildDetailedHistoryRequestIncludeTTReverseSSLStartEndNoAuthNoCount ()
        {
            long startTime = -1;
            long endTime = -1;

            TestBuildDetailedHistoryRequestCommon (true, true, true, "", startTime, endTime, -1);
        }

        public void TestBuildDetailedHistoryRequestCommon(bool ssl, bool reverse, bool includeTimetoken, 
            string authKey, long startTime, long endTime, int count){
            string channel = "history_channel";
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

            string startTimeString = "";
            string endTimeString = "";
            if (startTime != -1) {
                startTimeString = string.Format ("&start={0}", startTime.ToString ());
            }
            if (endTime != -1) {
                endTimeString = string.Format ("&end={0}", endTime.ToString ());
            }


            Uri uri = BuildRequests.BuildDetailedHistoryRequest (channel, startTime, endTime, count, reverse, 
                includeTimetoken, uuid, ssl, pubnub.Origin, pubnub.AuthenticationKey, Common.SubscribeKey
            );

            if (count == -1) {
                count = 100;
            }
            //Received:http://ps.pndsn.com/v2/history/sub-key/demo-36/channel/history_channel?count=90&reverse=true&start=14498416434364941&end=14498416799269095&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            //Received:https://ps.pndsn.com/v2/history/sub-key/demo-36/channel/history_channel?count=90&include_token=true&start=14498416434364941&end=14498416799269095&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            //http://ps.pndsn.com/v2/history/sub-key/demo-36/channel/publish_channel?count=90&start=14498416434364941&end=14498416799269095&auth=authKey&uuid=customuuid&pnsdk=PubNub-CSharp-UnityOSX/3.6.9.0
            string expected = string.Format ("http{0}://{1}/v2/history/sub-key/{2}/channel/{3}?count={4}{5}{6}{7}{8}{9}&uuid={10}&pnsdk={11}",
                ssl?"s":"", pubnub.Origin, Common.SubscribeKey, channel, count,
                includeTimetoken?"&include_token=true":"", reverse?"&reverse=true":"",
                startTimeString, endTimeString,    authKeyString, uuid, 
                Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.DetailedHistory, false, true)
            );
            string received = uri.OriginalString;
            Common.LogAndCompare (expected, received);
        }
        #endif
    }
}

