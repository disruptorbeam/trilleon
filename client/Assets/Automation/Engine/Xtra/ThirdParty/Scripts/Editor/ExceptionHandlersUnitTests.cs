using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class ExceptionHandlersUnitTests
    {
        #if DEBUG
        string ExceptionMessage ="";
        string ExceptionChannel = "";
        string ExceptionChannelGroups = "";
        int ExceptionStatusCode = 0;

        ResponseType CRequestType;
        bool ResumeOnReconnect = false;
        bool resultPart1 = false;

        bool IsTimeout = false;
        bool IsError = false;

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedTOCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe400CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe404CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe414CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe504CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe503CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe500CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe403CGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeNameResolutionFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeConnectFailureCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeServerProtocolViolationCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeProtocolErrorCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFNFCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFailedDLCGnCH()
        {
            string[] channels = {"testSubscribe","test2Subscribe"};string[] channelGroups = {"testSubscribeCG","test2SubscribeCG"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timed out", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "timedout", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "404 test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "414 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "504 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "503 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "500 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "403 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "NameResolutionFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ConnectFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ServerProtocolViolation 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "ProtocolError 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "java.io.FileNotFoundException 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> (channelGroups, "Failed downloading UnityWeb", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timed out", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timedout", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedTOCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "timedout", null, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe400CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe404CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "404 test message", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe414CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "414 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe504CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "504 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe503CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "503 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe500CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "500 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe403CG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "403 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeNameResolutionFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "NameResolutionFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeConnectFailureCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ConnectFailure 400", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeServerProtocolViolationCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ServerProtocolViolation 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeProtocolErrorCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "ProtocolError 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFNFCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "java.io.FileNotFoundException 400 response", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFailedDLCG()
        {
            string[] channelGroups = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channelGroups, "Failed downloading UnityWeb", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;
            
            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timed out", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test] 
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("timedout", channels, false,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe400 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe404 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("404 test message", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe414 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("414 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe504 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("504 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe503 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("503 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe500 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("500 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribe403 ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("403 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeNameResolutionFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("NameResolutionFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeConnectFailure ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ConnectFailure 400", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeServerProtocolViolation ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeProtocolError ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("ProtocolError 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFNF ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFailedDL ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> ("Failed downloading UnityWeb", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORSubscribeFailedTO ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 0;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 127;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerRORPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, false, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 0;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, false,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub400 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub404 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("404 test message", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub414 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 414;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("414 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub504 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 504;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("504 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub503 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 503;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("503 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub500 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 500;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("500 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPub403 ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("403 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubNameResolutionFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("NameResolutionFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubConnectFailure ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ConnectFailure 400", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubServerProtocolViolation ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ServerProtocolViolation 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubProtocolError ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 122;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("ProtocolError 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFNF ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 403;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("java.io.FileNotFoundException 400 response", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Critical
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFailedDL ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("Failed downloading UnityWeb", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }

        [Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerErrRORPubFailedTO ()
        {
            string[] channels = {"testNonSub"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<string> ("timedout", channels, true,
                ResponseType.Publish, CurrentRequestType.NonSubscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                true, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        /*[Test]
        public void TestResponseCallbackErrorOrTimeoutHandlerObjErrRORSubscribeFailedDLCG ()
        {
            string[] channels = {"testSubscribe","test2Subscribe"}; 
            ExceptionStatusCode = 400;

            TestResponseCallbackErrorOrTimeoutHandler<object> (channels, "Failed downloading UnityWeb", null, true,
                ResponseType.SubscribeV2, CurrentRequestType.Subscribe, UserCallbackCommonExceptionHandler, 
                ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, 
                false, true, 0, false, PubnubErrorFilter.Level.Warning
            );
        }*/

        public void TestResponseCallbackErrorOrTimeoutHandler<T>(string message, string[] channels,
            bool resumeOnReconnect, ResponseType responseType, CurrentRequestType crt, Action<T> userCallback,
            Action<T> connectCallback, Action<PubnubClientError> errorCallback,
            bool isTimeout, bool isError, long timetoken, bool ssl, PubnubErrorFilter.Level errorLevel
        ){
            TestResponseCallbackErrorOrTimeoutHandler(null, message, channels, resumeOnReconnect, responseType,
                crt, userCallback, connectCallback, errorCallback, isTimeout, isError, timetoken, ssl, errorLevel);
        }


        public void TestResponseCallbackErrorOrTimeoutHandler<T>( 
            string[] channelGroups, string message, string[] channels,
            bool resumeOnReconnect, ResponseType responseType, CurrentRequestType crt, Action<T> userCallback,
            Action<T> connectCallback, Action<PubnubClientError> errorCallback,
            bool isTimeout, bool isError, long timetoken, bool ssl, PubnubErrorFilter.Level errorLevel
        ){
            ExceptionMessage = message;
            ExceptionChannel = (channels!=null)?string.Join (",", channels):"";
            ExceptionChannelGroups = (channelGroups!=null)?string.Join (",", channelGroups):"";

            if (isTimeout) { 
                ExceptionMessage = "Operation Timeout";
                IsTimeout = true;
            } else {
                IsTimeout = false;
            }

            if (isError) {
                IsError = true;
            } else {
                IsError = false;
            }

            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(channels, 
                true, false, null, userCallback, connectCallback, errorCallback, null, null);  

            List<ChannelEntity> channelGroupEntities = Helpers.CreateChannelEntity<T>(channelGroups, 
                true, true, null, userCallback, connectCallback, errorCallback, null, null);  

            if((channelEntities != null) && (channelGroupEntities != null)){
                channelEntities.AddRange(channelGroupEntities);
            } else if(channelEntities == null) {
                channelEntities = channelGroupEntities;
            }

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, responseType, 
                resumeOnReconnect, 0, isTimeout, timetoken, typeof(T));

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );

            CustomEventArgs<T> cea = new CustomEventArgs<T> ();
            cea.PubnubRequestState = requestState;
            cea.Message = message;
            cea.IsError = isError;
            cea.IsTimeout = isTimeout;
            cea.CurrRequestType = crt;

            CRequestType = responseType;
            if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                ExceptionHandlers.MultiplexException += HandleMultiplexException<T>;
                resultPart1 = false;
            }
            ExceptionHandlers.ResponseCallbackErrorOrTimeoutHandler<T> (cea, requestState, 
                errorLevel);

            /*if (responseType == ResponseType.PresenceV2 || responseType == ResponseType.SubscribeV2) {
                DateTime dt = DateTime.Now;
                while (dt.AddSeconds(2) > DateTime.Now) {
                    UnityEngine.Debug.Log ("waiting");
                }
            }*/
        }    

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2.0f);
        }

        private void HandleMultiplexException<T> (object sender, EventArgs ea)
        {
            ExceptionHandlers.MultiplexException -= HandleMultiplexException<T>;
            MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
            bool channelMatch = false;
            bool channelGroupMatch = false;
            if (mea.channelEntities != null) {
                foreach (ChannelEntity c in mea.channelEntities) {
                    channelMatch = ExceptionChannel.Contains(c.ChannelID.ChannelOrChannelGroupName);
                    channelGroupMatch = ExceptionChannelGroups.Contains(c.ChannelID.ChannelOrChannelGroupName);
                    if(channelMatch || channelGroupMatch)
                        continue;
                }
            }
            string channels = Helpers.GetNamesFromChannelEntities(mea.channelEntities, false);

            UnityEngine.Debug.Log (string.Format("mea.responseType.Equals (CRequestType) {0}\n" +
                "channelMatch {1}\n" +
                "mea.resumeOnReconnect.Equals(ResumeOnReconnect) {2}\n"
                , mea.responseType.Equals (CRequestType),
            channelMatch,
                mea.resumeOnReconnect.Equals(ResumeOnReconnect)));

            UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: \n" +
                "mea.responseType.Equals (CRequestType): {0} \n" +
                "channelMatch: {1} \n" +
                "mea.resumeOnReconnect.Equals(ResumeOnReconnect): {2} \n" +
                "CRequestType:{3} \n" +
                "ExceptionChannel: {4} \n" +
                "ResumeOnReconnect: {5} \n" +
                "mea.responseType: {6} \n" +
                "channels: {7} \n" +
                "mea.resumeOnReconnect: {8} \n" +
                "resultPart1: {9} \n" +
                "channelGroupMatch: {10}\n",
                "channelGroups: {11}\n",
                mea.responseType.Equals (CRequestType),
                channelMatch,
                mea.resumeOnReconnect.Equals(ResumeOnReconnect), CRequestType.ToString(), 
                ExceptionChannel, ResumeOnReconnect, mea.responseType,
                channels, mea.resumeOnReconnect, resultPart1,
                channelGroupMatch,
                ExceptionChannelGroups
                ));
            bool resultPart2 = false;
            if (mea.responseType.Equals (CRequestType)
                && (string.IsNullOrEmpty(ExceptionChannel))?true:channelMatch
                && (string.IsNullOrEmpty(ExceptionChannelGroups))?true:channelGroupMatch
                && mea.resumeOnReconnect.Equals (ResumeOnReconnect)) {
                resultPart2 = true;
            }
            Assert.IsTrue (resultPart1 && resultPart2);
        }
            
        void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: \n" +
                "result: {0} \n" +
                "result.Message.Equals (ExceptionMessage): {1} \n" +
                "ExceptionChannel.Contains(result.ChannelGroup): {2} \n" +
                "result.StatusCode.Equals(ExceptionStatusCode): {3} \n" +
                "result.StatusCode.ToString(): {4} \n" +
                "ExceptionMessage: {5} \n" +
                "ExceptionChannel: {6} \n" +
                "ExceptionStatusCode: {7} \n" +
                "IsTimeout: {8} \n" +
                "result.Channel: {9} \n" +
                "ExceptionChannelGroups.Contains(result.ChannelGroup): {10}\n",
                "ExceptionChannelGroups: {11}\n",
                result, result.Message.Equals (ExceptionMessage),
                ExceptionChannel.Contains(result.ChannelGroup),
                result.StatusCode.Equals(ExceptionStatusCode), result.StatusCode.ToString(), ExceptionMessage,
                ExceptionChannel, ExceptionStatusCode, IsTimeout, result.Channel,
                ExceptionChannelGroups.Contains(result.ChannelGroup),
                ExceptionChannelGroups
            ));

            bool statusCodeCheck = false;
            //TODO: Check why isError and isTimeout status codes dont match
            if (IsTimeout || IsError) {
                //statusCodeCheck = result.StatusCode.Equals (400);
                statusCodeCheck = true;
            } else {
                statusCodeCheck = result.StatusCode.Equals (ExceptionStatusCode);
            }

            if ((result.Channel.Contains ("Subscribe")) || (result.Channel.Contains ("Presence"))
                || (result.ChannelGroup.Contains ("Subscribe")) || (result.ChannelGroup.Contains ("Presence"))
            ) {
                if ((result.Message.Equals (ExceptionMessage)
                    && (string.IsNullOrEmpty(ExceptionChannel))?true:ExceptionChannel.Contains(result.Channel)
                    && (string.IsNullOrEmpty(ExceptionChannelGroups))?true:ExceptionChannelGroups.Contains(result.ChannelGroup)
                    && statusCodeCheck)) {
                    resultPart1 = true;
                } else {
                    resultPart1 = false;
                }
                UnityEngine.Debug.Log ("Subscribe || Presence " + resultPart1);
            } else {
                //if (IsChannelGroup){
                    Assert.IsTrue (result.Message.Equals (ExceptionMessage)
                    && (string.IsNullOrEmpty(ExceptionChannel))?true:ExceptionChannel.Contains(result.Channel)
                    && (string.IsNullOrEmpty(ExceptionChannelGroups))?true:ExceptionChannelGroups.Contains(result.ChannelGroup)
                    && statusCodeCheck);
                /*}else{
                    Assert.IsTrue (result.Message.Equals (ExceptionMessage)
                        && ExceptionChannel.Contains(result.ChannelGroup)
                        && statusCodeCheck);
                    
                }*/
            }
        }

        void UserCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
        }

        void UserCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
        }

        void DisconnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
        }

        void ConnectCallbackCommonExceptionHandler (string result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
        }

        void ConnectCallbackCommonExceptionHandler (object result)
        {
            UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
        }

        #endif
    }
}

