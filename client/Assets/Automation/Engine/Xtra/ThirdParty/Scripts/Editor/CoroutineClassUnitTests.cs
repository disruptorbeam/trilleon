//#define REDUCE_PUBNUB_COROUTINES
using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class CoroutineClassUnitTests
    {
        #if DEBUG && REDUCE_PUBNUB_COROUTINES
        [Test]
        public void TestSetGetCoroutineParamsSub(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Subscribe;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.SubscribeV2;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            //http://ps.pndsn.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}
            string url = string.Format ("http://ps.pndsn.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, 0, pubnub.SessionUUID, pubnub.Version
            );

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.SubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsNonSub(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.NonSubscribe;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Time;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = "https://ps.pndsn.com/time/0";

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsHB(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.Heartbeat;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.Heartbeat;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = "https://ps.pndsn.com/time/0";

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        [Test]
        public void TestSetGetCoroutineParamsPHB(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.PresenceHeartbeat;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.PresenceHeartbeat;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = string.Format ("http://ps.pndsn.com/v2/presence/sub_key/{0}/channel/{1}/heartbeat?uuid={2}&heartbeat=62&pnsdk={3}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, pubnub.SessionUUID, pubnub.Version
            );

            SetGetCoroutineParams<string> (multiChannel, url, crt, respType, pubnub.NonSubscribeTimeout, false, false);

        }

        public void SetGetCoroutineParams<T>(string[] multiChannel, string url, CurrentRequestType crt, ResponseType respType,
            int timeout, bool resumeOnReconnect, bool isTimeout
        ){
            List<ChannelEntity> channelEntities = Helpers.CreateChannelEntity<T>(multiChannel, 
                true, false, null, null, 
                null, null, null, null);  

            RequestState<T> pubnubRequestState = BuildRequests.BuildRequestState<T> (channelEntities, respType, 
                resumeOnReconnect, 0, isTimeout, 0, typeof(T));

            CoroutineParams<T> cp = new CoroutineParams<T> (url, timeout, 0, crt, typeof(T), pubnubRequestState);

            GameObject go = new GameObject ("PubnubUnitTestCoroutine");
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();

            cc.SetCoroutineParams<T>(crt, cp);

            CoroutineParams<T> cp2 = cc.GetCoroutineParams<T>(crt) as CoroutineParams<T>;
            Assert.True (cp.crt.Equals (cp2.crt));
            Assert.True (cp.pause.Equals (cp2.pause));
            Assert.True (cp.timeout.Equals (cp2.timeout));
            Assert.True (cp.url.Equals (cp2.url));
        }

        [Test]
        public void TestCheckElapsedTimeSubTimeElapsed(){
            string[] multiChannel = {"testChannel"};

            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.SubscribeV2;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            //http://ps.pndsn.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}
            string url = string.Format ("http://ps.pndsn.com/subscribe/{0}/{1}/0/{2}?uuid={3}&pnsdk={4}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, 0, pubnub.SessionUUID, pubnub.Version
            );
            WWW www = new WWW (url);
            CheckElapsedTime<string> (CurrentRequestType.Subscribe, 0, www, false);
        }

        [Test]
        public void TestCheckElapsedTimeSubTimeNotElapsedWWWNull(){

            CheckElapsedTime<string> (CurrentRequestType.Subscribe, 10, null, false);
        }

        [Test]
        public void TestCheckElapsedTimeSubTimeNotElapsedWWWNullComplete(){

            CheckElapsedTime<string> (CurrentRequestType.Subscribe, 10, null, true);
        }

        [Test]
        public void TestCheckElapsedTimeHBTimeElapsed(){
            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );
            string url = "https://ps.pndsn.com/time/0";
            WWW www = new WWW (url);
            CheckElapsedTime<string> (CurrentRequestType.Heartbeat, 0, www, false);
        }

        [Test]
        public void TestCheckElapsedTimeHBTimeNotElapsedWWWNull(){

            CheckElapsedTime<string> (CurrentRequestType.Heartbeat, 10, null, false);
        }

        [Test]
        public void TestCheckElapsedTimeHBTimeNotElapsedWWWNullComplete(){

            CheckElapsedTime<string> (CurrentRequestType.Heartbeat, 10, null, true);
        }

        [Test]
        public void TestCheckElapsedTimeNonSubTimeElapsed(){
            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );
            string url = "https://ps.pndsn.com/time/0";
            WWW www = new WWW (url);
            CheckElapsedTime<string> (CurrentRequestType.NonSubscribe, 0, www, false);
        }

        [Test]
        public void TestCheckElapsedTimeNonSubTimeNotElapsedWWWNull(){

            CheckElapsedTime<string> (CurrentRequestType.NonSubscribe, 10, null, false);
        }

        [Test]
        public void TestCheckElapsedTimeNonSubTimeNotElapsedWWWNullComplete(){

            CheckElapsedTime<string> (CurrentRequestType.NonSubscribe, 10, null, true);
        }

        [Test]
        public void TestCheckElapsedTimePHBTimeElapsed(){
            string[] multiChannel = {"testChannel"};

            CurrentRequestType crt = CurrentRequestType.PresenceHeartbeat;
            string expectedMessage = "[[]";
            string expectedChannels = string.Join (",", multiChannel);
            ResponseType respType =  ResponseType.PresenceHeartbeat;

            Pubnub pubnub = new Pubnub (
                CommonIntergrationTests.PublishKey,
                CommonIntergrationTests.SubscribeKey,
                "",
                "",
                true
            );

            string url = string.Format ("http://ps.pndsn.com/v2/presence/sub_key/{0}/channel/{1}/heartbeat?uuid={2}&heartbeat=62&pnsdk={3}", CommonIntergrationTests.SubscribeKey, 
                expectedChannels, pubnub.SessionUUID, pubnub.Version
            );
            WWW www = new WWW (url);
            CheckElapsedTime<string> (CurrentRequestType.PresenceHeartbeat, 0, www, false);
        }

        [Test]
        public void TestCheckElapsedTimePHBTimeNotElapsedWWWNull(){

            CheckElapsedTime<string> (CurrentRequestType.PresenceHeartbeat, 10, null, false);
        }

        [Test]
        public void TestCheckElapsedTimePHBTimeNotElapsedWWWNullComplete(){

            CheckElapsedTime<string> (CurrentRequestType.PresenceHeartbeat, 10, null, true);
        }

        CurrentRequestType sCrt;
        bool responseHandled = false;

        void CheckElapsedTime<T>(CurrentRequestType crt, float timer, WWW www, bool completeFlag){


            GameObject go = new GameObject ("PubnubUnitTestCoroutine");
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();

            if(crt.Equals(CurrentRequestType.Subscribe)){
                cc.isSubscribeComplete = completeFlag;
                cc.SubCompleteOrTimeoutEvent += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.NonSubscribe)){
                cc.isNonSubscribeComplete = completeFlag;
                cc.NonsubCompleteOrTimeoutEvent += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.PresenceHeartbeat)){
                cc.isPresenceHeartbeatComplete = completeFlag;
                cc.PresenceHeartbeatCompleteOrTimeoutEvent += CcCoroutineComplete<T>;
            } else if (crt.Equals(CurrentRequestType.Heartbeat)){
                cc.isHearbeatComplete = completeFlag;
                cc.HeartbeatCompleteOrTimeoutEvent += CcCoroutineComplete<T>;
            }
            sCrt = crt;
            responseHandled = false;

            cc.CheckElapsedTime (crt, timer, www);
            UnityEngine.Debug.Log ("waiting");
            //Wait ();
            //Assert.True (responseHandled);
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.5f);
        }

        void CcCoroutineComplete<T> (object sender, EventArgs e)
        {
            responseHandled = true;
            UnityEngine.Debug.Log ("Event handler fired");
            CurrentRequestTypeEventArgs crtEa = e as CurrentRequestTypeEventArgs;

            if (crtEa != null) {
                UnityEngine.Debug.Log ("cea.IsTimeout:" + crtEa.IsTimeout);
                UnityEngine.Debug.Log ("cea.IsTimeout:" + crtEa.CurrRequestType);
                Assert.True (crtEa.CurrRequestType.Equals (sCrt), "All good");
            } else {
                Assert.True (false, "Event handler args empty");
            }
        }

        [Test]
        public void TestCheckIfRequestIsRunningSub(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.Subscribe, false);
        }

        [Test]
        public void TestCheckIfRequestIsRunningTrueSub(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.Subscribe, true);
        }

        [Test]
        public void TestCheckIfRequestIsRunningNonSub(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.NonSubscribe, false);
        }

        [Test]
        public void TestCheckIfRequestIsRunningTrueNonSub(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.NonSubscribe, true);
        }

        [Test]
        public void TestCheckIfRequestIsRunningHB(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.Heartbeat, false);
        }

        [Test]
        public void TestCheckIfRequestIsRunningTrueHB(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.Heartbeat, true);
        }

        [Test]
        public void TestCheckIfRequestIsRunningPHB(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.PresenceHeartbeat, false);
        }

        [Test]
        public void TestCheckIfRequestIsRunningTruePHB(){
            CheckIfRequestIsRunning<string> (CurrentRequestType.PresenceHeartbeat, true);
        }

        void CheckIfRequestIsRunning<T>(CurrentRequestType crt, bool completeFlag){
            GameObject go = new GameObject ("PubnubUnitTestCoroutine");
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();
            if(crt.Equals(CurrentRequestType.Subscribe)){
                cc.isSubscribeComplete = completeFlag;
            } else if (crt.Equals(CurrentRequestType.NonSubscribe)){
                cc.isNonSubscribeComplete = completeFlag;
            } else if (crt.Equals(CurrentRequestType.PresenceHeartbeat)){
                cc.isPresenceHeartbeatComplete = completeFlag;
            } else if (crt.Equals(CurrentRequestType.Heartbeat)){
                cc.isHearbeatComplete = completeFlag;
            }
            if (completeFlag) {
                Assert.False (cc.CheckIfRequestIsRunning (crt));
            } else {
                Assert.True (cc.CheckIfRequestIsRunning (crt));
            }
        }
    
        [Test]
        public void TestStopTimeoutsSubSubscribe(){
            StopTimeouts (CurrentRequestType.Subscribe);
        }

        [Test]
        public void TestStopTimeoutsNonSubscribe(){
            StopTimeouts (CurrentRequestType.NonSubscribe);
        }

        [Test]
        public void TestStopTimeoutsHeartbeat(){
            StopTimeouts (CurrentRequestType.Heartbeat);
        }

        [Test]
        public void TestStopTimeoutsPresenceHeartbeat(){
            StopTimeouts (CurrentRequestType.PresenceHeartbeat);
        }

        void StopTimeouts(CurrentRequestType crt){
            GameObject go = new GameObject ("PubnubUnitTestCoroutine");
            CoroutineClass cc = go.AddComponent<CoroutineClass> ();
            if(crt.Equals(CurrentRequestType.Subscribe)){
                cc.runSubscribeTimer = true;
            } else if (crt.Equals(CurrentRequestType.NonSubscribe)){
                cc.runNonSubscribeTimer = true;
            } else if (crt.Equals(CurrentRequestType.PresenceHeartbeat)){
                cc.runPresenceHeartbeatTimer = true;
            } else if (crt.Equals(CurrentRequestType.Heartbeat)){
                cc.runHeartbeatTimer = true;
            }
            cc.StopTimeouts (crt);
            if(crt.Equals(CurrentRequestType.Subscribe)){
                Assert.True (!cc.runSubscribeTimer);
                Assert.True (cc.subscribeTimer.Equals(0));
            } else if (crt.Equals(CurrentRequestType.NonSubscribe)){
                Assert.True (!cc.runNonSubscribeTimer);
                Assert.True (cc.nonSubscribeTimer.Equals(0));
            } else if (crt.Equals(CurrentRequestType.PresenceHeartbeat)){
                Assert.True (!cc.runPresenceHeartbeatTimer);
                Assert.True (cc.presenceHeartbeatTimer.Equals(0));
                Assert.True (!cc.runPresenceHeartbeatPauseTimer);
                Assert.True (cc.presenceHeartbeatPauseTimer.Equals(0));
            } else if (crt.Equals(CurrentRequestType.Heartbeat)){
                Assert.True (!cc.runHeartbeatTimer);
                Assert.True (cc.heartbeatTimer.Equals(0));
                Assert.True (!cc.runPresenceHeartbeatPauseTimer);
                Assert.True (cc.heartbeatPauseTimer.Equals(0));
            }
        }
        #endif
    }
}

