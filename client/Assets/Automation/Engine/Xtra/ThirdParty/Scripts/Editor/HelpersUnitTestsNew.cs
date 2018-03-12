using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class HelpersUnitTestsNew
    {
        #if DEBUG
        [Test]
        public void TestCounterClassNextValue(){
            Counter publishMessageCounter = new Counter ();
            publishMessageCounter.NextValue();
            Assert.True(publishMessageCounter.NextValue().Equals(2));
        }

        [Test]
        public void TestCounterClassReset(){
            Counter publishMessageCounter = new Counter ();
            publishMessageCounter.NextValue();
            publishMessageCounter.NextValue();
            publishMessageCounter.Reset();
            Assert.True(publishMessageCounter.NextValue().Equals(1));
        }

        [Test]
        public void TestCreateTimetokenMetadata(){
            var dict = new Dictionary<string, object>(); 
            dict.Add("t", 14685037252884276);
            dict.Add("r", "east");
            TimetokenMetadata ttm= Helpers.CreateTimetokenMetadata(dict, "orig");
            Assert.True(
                ttm.Region.Equals("east")
                && ttm.Timetoken.Equals(14685037252884276));
        }

        [Test]
        public void TestCreateTimetokenMetadataWithoutRegion(){
            var dict = new Dictionary<string, object>(); 
            dict.Add("t", 14685037252884276);
            TimetokenMetadata ttm= Helpers.CreateTimetokenMetadata(dict, "orig");
            Assert.True(
                ttm.Region.Equals("")
                && ttm.Timetoken.Equals(14685037252884276));
        }

        [Test]
        public void TestAddToSubscribeMessageList(){
            List<SubscribeMessage> lsm = new List<SubscribeMessage>();

            Helpers.AddToSubscribeMessageList(Common.CreateSubscribeDictionary(), ref lsm);

            if(lsm!=null){
                ParseSubscribeMessageList(lsm);
            }
            else {
                Assert.True(false, "Lsm null");
            }
        }

        internal void ParseSubscribeMessageList(List<SubscribeMessage> lsm){
            var dictUR = lsm[0].UserMetadata as Dictionary<string, object>;
            string log=
                String.Format(" " 
                    +"\n lsm[0].Channel.Equals('Channel') {0} "
                    +"\n lsm[0].Flags.Equals('flags') {1} "
                    +"\n lsm[0].IssuingClientId.Equals('issuingClientId') {2} "
                    +"\n lsm[0].OriginatingTimetoken.Region.Equals('west') {3} "
                    +"\n lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276) {4} "
                    +"\n lsm[0].Payload.Equals('Message') {5} "
                    +"\n lsm[0].PublishTimetokenMetadata.Region.Equals('east') {6} "
                    +"\n lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348) {7} "
                    +"\n lsm[0].SequenceNumber.Equals('10') {8} "
                    +"\n lsm[0].Shard.Equals('1') {9} "
                    +"\n lsm[0].SubscribeKey.Equals('subscribeKey') {10} "
                    +"\n lsm[0].SubscriptionMatch.Equals('SM') {11} "
                    +"\n dictUR.ContainsKey('region')  {12} "
                    +"\n dictUR.ContainsValue('north')  {13} ",
                    lsm[0].Channel.Equals("Channel")
                    , lsm[0].Flags.Equals("flags")
                    , lsm[0].IssuingClientId.Equals("issuingClientId")
                    , lsm[0].OriginatingTimetoken.Region.Equals("west")
                    , lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276)
                    , lsm[0].Payload.Equals("Message")
                    , lsm[0].PublishTimetokenMetadata.Region.Equals("east")
                    , lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348)
                    , lsm[0].SequenceNumber.Equals(10)
                    , lsm[0].Shard.Equals("1")
                    , lsm[0].SubscribeKey.Equals("subscribeKey")
                    , lsm[0].SubscriptionMatch.Equals("SM")
                    , dictUR.ContainsKey("region")
                    , dictUR.ContainsValue("north") 
                );
            UnityEngine.Debug.Log(log);
            Assert.True(
                lsm[0].Channel.Equals("Channel")
                && lsm[0].Flags.Equals("flags")
                && lsm[0].IssuingClientId.Equals("issuingClientId")
                && lsm[0].OriginatingTimetoken.Region.Equals("west")
                && lsm[0].OriginatingTimetoken.Timetoken.Equals(14685037252884276)
                && lsm[0].Payload.Equals("Message")
                && lsm[0].PublishTimetokenMetadata.Region.Equals("east")
                && lsm[0].PublishTimetokenMetadata.Timetoken.Equals(14685037252884348)
                && lsm[0].SequenceNumber.Equals(10)
                && lsm[0].Shard.Equals("1")
                && lsm[0].SubscribeKey.Equals("subscribeKey")
                && lsm[0].SubscriptionMatch.Equals("SM")
                && dictUR.ContainsKey("region")
                && dictUR.ContainsValue("north"), log);

        }

        public static Dictionary<string, object> CreatePresenceDictionary(bool joinNull, bool leaveNull, bool timeoutNull){

            Dictionary<string, object> pnPresenceEventDict = new Dictionary<string, object> ();

            pnPresenceEventDict["action"] = "interval";
            pnPresenceEventDict["uuid"] = "a7acb27c-f1da-4031-a2cc-58656196b06d";
            pnPresenceEventDict["occupancy"] = 3;
            pnPresenceEventDict["timestamp"] = 1490700797;
            pnPresenceEventDict["state"] = null;
            if (joinNull) {
                pnPresenceEventDict ["join"] = null;
            } else {
                pnPresenceEventDict ["join"] = new List<string> (){ "Client-odx4y", "test" };
            }
            if (leaveNull) {
                pnPresenceEventDict ["leave"] = null;
            } else {
                pnPresenceEventDict ["leave"] = new List<string> (){ "Client-2", "test2" };
            }
            if (timeoutNull) {
                pnPresenceEventDict ["timeout"] = null;
            } else {
                pnPresenceEventDict["timeout"] = new List<string> (){ "Client-3", "test3"};
            }

            var dictSM = new Dictionary<string, object>();
            dictSM.Add("a", "1");
            dictSM.Add("b", "SM");
            dictSM.Add("c", "Channel");
            dictSM.Add("d", (object)pnPresenceEventDict);
            dictSM.Add("f", "flags");
            dictSM.Add("i", "issuingClientId");
            dictSM.Add("k", "subscribeKey");
            dictSM.Add("s", "10");

            var dictOT = new Dictionary<string, object>(); 
            dictOT.Add("t", 14685037252884276);
            dictOT.Add("r", "west");
            dictSM.Add("o", dictOT);

            var dictPM = new Dictionary<string, object>(); 
            dictPM.Add("t", 14685037252884348);
            dictPM.Add("r", "east");
            dictSM.Add("p", dictPM);

            var dictU = new Dictionary<string, object>(); 
            dictU.Add("region", "north");
            dictSM.Add("u", dictU);
            return dictSM;
        }

        [Test]
        public void TestCreatePNPresenceEventResult(){
            bool joinNull = false, leaveNull = false, timeoutNull = false;
            TestCreatePNPresenceEventResultCommon (joinNull, leaveNull, timeoutNull);
        }

        [Test]
        public void TestCreatePNPresenceEventResultTimeoutNull(){
            bool joinNull = false, leaveNull = false, timeoutNull = true;
            TestCreatePNPresenceEventResultCommon (joinNull, leaveNull, timeoutNull);
        }

        [Test]
        public void TestCreatePNPresenceEventResultLeaveNull(){
            bool joinNull = false, leaveNull = true, timeoutNull = false;
            TestCreatePNPresenceEventResultCommon (joinNull, leaveNull, timeoutNull);
        }

        [Test]
        public void TestCreatePNPresenceEventResultJoinNull(){
            bool joinNull = true, leaveNull = false, timeoutNull = false;
            TestCreatePNPresenceEventResultCommon (joinNull, leaveNull, timeoutNull);
        }

        [Test]
        public void TestCreatePNPresenceEventResultAllNull(){
            bool joinNull = true, leaveNull = true, timeoutNull = true;
            TestCreatePNPresenceEventResultCommon (joinNull, leaveNull, timeoutNull);
        }

        public void TestCreatePNPresenceEventResultCommon(bool joinNull, bool leaveNull, bool timeoutNull){
            List<SubscribeMessage> lsm = new List<SubscribeMessage>();
            Helpers.AddToSubscribeMessageList(CreatePresenceDictionary(joinNull, leaveNull, timeoutNull), ref lsm);

            PNPresenceEventResult subMessageResult; 
            Helpers.CreatePNPresenceEventResult(lsm[0], out subMessageResult);

            Assert.IsTrue (subMessageResult.Channel.Equals("Channel"));
            Assert.IsTrue (subMessageResult.Event.Equals("interval"));
            Assert.IsTrue (subMessageResult.Occupancy.Equals(3));
            Assert.IsTrue (subMessageResult.UUID.Equals("a7acb27c-f1da-4031-a2cc-58656196b06d"));
            Assert.IsTrue (subMessageResult.Timestamp.Equals(1490700797));
            if (!joinNull) {
                Assert.IsTrue (subMessageResult.Join != null);
                Assert.IsTrue ((subMessageResult.Join != null) ? subMessageResult.Join.Contains ("Client-odx4y") && subMessageResult.Join.Contains ("test") : false);
            } else {
                Assert.IsTrue (subMessageResult.Join == null);
            }
            if (!leaveNull) {
                Assert.IsTrue (subMessageResult.Leave != null);
                Assert.IsTrue ((subMessageResult.Leave != null) ? subMessageResult.Leave.Contains ("Client-2") && subMessageResult.Leave.Contains ("test2") : false);
            } else {
                Assert.IsTrue (subMessageResult.Leave == null);
            }
            if (!timeoutNull) {
                Assert.IsTrue (subMessageResult.Timeout != null);
                Assert.IsTrue ((subMessageResult.Timeout != null) ? subMessageResult.Timeout.Contains ("Client-3") && subMessageResult.Timeout.Contains ("test3") : false);
            } else {
                Assert.IsTrue (subMessageResult.Timeout == null);
            }
        }

        [Test]
        public void TestCreateListOfSubscribeMessage(){
            object[] obj = {Common.CreateSubscribeDictionary()}; 
            List<SubscribeMessage> lsm = Helpers.CreateListOfSubscribeMessage(obj);

            if(lsm!=null){
                ParseSubscribeMessageList(lsm);
            }
            else {
                Assert.True(false, "Lsm null");
            }

        }

        [Test]
        public void TestBuildJsonUserState(){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");
            string ret = Helpers.BuildJsonUserState(dictSM);
            Assert.AreEqual(ret, "\"k\":\"v\",\"k2\":\"v2\"", ret);
        }

        [Test]
        public void TestBuildJsonUserStateCEObj(){
            TestBuildJsonUserStateCommon<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGObj(){
            TestBuildJsonUserStateCommon<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCHObj(){
            TestBuildJsonUserStateCommon<object>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCE(){
            TestBuildJsonUserStateCommon<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECG(){
            TestBuildJsonUserStateCommon<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestBuildJsonUserStateCECGnCH(){
            TestBuildJsonUserStateCommon<string>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestBuildJsonUserStateCommon<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback,  
                wildcardPresenceCallback,
                disconnectCallback);
            string ret = Helpers.BuildJsonUserState(lstCE);
            if(channel && channelGroup){
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"},\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else if (channelGroup){
                Assert.AreEqual(ret, "{\"cg1\":{\"k5\":\"v5\",\"k6\":\"v6\"},\"cg2\":{\"k7\":\"v7\",\"k8\":\"v8\"}}", ret);
            } else {
                Assert.AreEqual(ret, "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k3\":\"v3\",\"k4\":\"v4\"}}", ret);
            }
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCGObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCNObj(){
            TestGetNamesFromChannelEntitiesCommon<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntities(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG(){
            TestGetNamesFromChannelEntitiesCommon<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN(){
            TestGetNamesFromChannelEntitiesCommon<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }


        public void TestGetNamesFromChannelEntitiesCommon<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback, 
                wildcardPresenceCallback,
                disconnectCallback
            );    
            string ces = Helpers.GetNamesFromChannelEntities(lstCE);

            bool ceFound = true;
            foreach(ChannelEntity ch in lstCE){
                if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)){
                    ceFound = false;
                }
            }
            UnityEngine.Debug.Log(ces);
            Assert.True(ceFound, ces);
        }
        [Test]
        public void TestGetNamesFromChannelEntitiesCG2Obj(){
            TestGetNamesFromChannelEntitiesCommon2<object>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2Obj(){
            TestGetNamesFromChannelEntitiesCommon2<object>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCG2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestGetNamesFromChannelEntitiesCN2(){
            TestGetNamesFromChannelEntitiesCommon2<string>(false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestGetNamesFromChannelEntitiesCommon2<T>(bool channelGroup, bool channel,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback){
            List<ChannelEntity> lstCE= Common.CreateListOfChannelEntities<T>(channelGroup, channel,
                false, false,
                userCallback, connectCallback,  
                wildcardPresenceCallback,
                disconnectCallback
            );    
            string ces = Helpers.GetNamesFromChannelEntities(lstCE, channelGroup);

            bool ceFound = true;
            foreach(ChannelEntity ch in lstCE){
                if(channelGroup){
                    if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)
                        && ch.ChannelID.IsChannelGroup)
                    {
                        ceFound = false;
                    }
                } else {
                    if(!ces.Contains(ch.ChannelID.ChannelOrChannelGroupName)
                        && !ch.ChannelID.IsChannelGroup)
                    {
                        ceFound = false;
                    }
                }
            }
            UnityEngine.Debug.Log(ces);
            Assert.True(ceFound, ces);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditObj(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallback(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntity(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEdit(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, false);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackObjOther(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityObjOther(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditObjOther(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditObjOther(){
            TestUpdateOrAddUserStateOfEntityCommon<object>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackOther(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityOther(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityErrorCallbackEditOther(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        [Test]
        public void TestUpdateOrAddUserStateOfEntityEditOther(){
            TestUpdateOrAddUserStateOfEntityCommon<string>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, true);
        }

        public void TestUpdateOrAddUserStateOfEntityCommon<T>(bool isChannelGroup, bool edit, 
            bool checkErrorCallback, bool ssl, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback, bool isForOtherUUID){

            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                ssl
            );
            string state = pubnub.JsonPluggableLibrary.SerializeToJsonString(dictSM);

            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>("ch1", false, isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);

            List<ChannelEntity> lstCe = new List<ChannelEntity>();
            lstCe.Add(ce1);
            string channelName = "ch1";

            if(checkErrorCallback || edit){
                var dictSM2 = new Dictionary<string, object>();
                dictSM2.Add("k2","v3");

                List<ChannelEntity> lstCe2 = new List<ChannelEntity>();
                lstCe2.Add(ce1);

                Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM2, edit,
                    userCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info, 
                    isForOtherUUID, ref lstCe2);
                string ustate = pubnub.JsonPluggableLibrary.SerializeToJsonString(lstCe2[0].ChannelParams.UserState);
                string state2 = pubnub.JsonPluggableLibrary.SerializeToJsonString(dictSM2);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state2, ustate));
            }

            if(Helpers.UpdateOrAddUserStateOfEntity<T>(channelName, isChannelGroup, dictSM, edit,
                userCallback, ErrorCallbackUserState, PubnubErrorFilter.Level.Info, 
                isForOtherUUID, ref lstCe)){
                string ustate = pubnub.JsonPluggableLibrary.SerializeToJsonString(lstCe[0].ChannelParams.UserState);
                UnityEngine.Debug.Log(string.Format("{0}\n{1}", state, ustate));
                Assert.AreEqual(ustate, state, 
                    string.Format ("{0}\n{1}", ustate, state));
            } else {
                UnityEngine.Debug.Log(state);
                if(!checkErrorCallback){
                    Assert.True(false, "UpdateOrAddUserStateOfEntity returned false");
                }
            }
        }

        void ErrorCallbackUserState (PubnubClientError result)
        {
            UnityEngine.Debug.Log (string.Format("{0}", result.StatusCode));
            Assert.True(result.StatusCode.Equals(PubnubErrorCode.UserStateUnchanged), result.StatusCode.ToString());
        }


        [Test]
        public void TestCheckAndAddExistingUserStateEdit(){
            TestCheckAndAddExistingUserStateCommon<string>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "", "");
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditObj(){
            TestCheckAndAddExistingUserStateCommon<object>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "", "");
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditOther(){
            TestCheckAndAddExistingUserStateCommon<string>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "uuid1", "uuid2");
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditObjOther(){
            TestCheckAndAddExistingUserStateCommon<object>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "uuid1", "uuid2");
        }

        [Test]
        public void TestCheckAndAddExistingUserStateEditObjBoth(){
            TestCheckAndAddExistingUserStateCommon<object>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "", "");
            TestCheckAndAddExistingUserStateCommon<object>(false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback, "uuid1", "uuid2");
        }

        public void TestCheckAndAddExistingUserStateCommon<T>( bool edit, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback, string uuid, string sessionUUID
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");
            List<ChannelEntity> lstCE;

            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );
            string state = "{\"ch1\":{\"k\":\"v\",\"k2\":\"v2\"},\"ch2\":{\"k\":\"v\",\"k2\":\"v2\"},\"cg1\":{\"k\":\"v\",\"k2\":\"v2\"},\"cg2\":{\"k\":\"v\",\"k2\":\"v2\"}}";

            string userstate;

            string[] ch = {"ch1", "ch2"};
            string[] cg = {"cg1", "cg2"};
            bool stateChanged = Helpers.CheckAndAddExistingUserState<T>(string.Join(",",ch), 
                string.Join(",",cg), dictSM,  userCallback, 
                ErrorCallbackUserState,PubnubErrorFilter.Level.Info
                , edit, uuid, sessionUUID, out userstate, out lstCE);

            bool ceFound = true;
            foreach(ChannelEntity ch2 in lstCE){
                bool chFound = false;
                bool cgFound = false;
                if(!ch2.ChannelID.IsChannelGroup){
                    foreach(string channel in ch){
                        if(channel.Equals(ch2.ChannelID.ChannelOrChannelGroupName)
                        )
                        {
                            UnityEngine.Debug.Log (string.Format("{0} found", channel));
                            chFound = true;
                            break;
                        }
                    }
                } else {
                    foreach(string channel in cg){
                        if(channel.Equals(ch2.ChannelID.ChannelOrChannelGroupName)
                        )
                        {
                            UnityEngine.Debug.Log (string.Format("{0} found", channel));
                            cgFound = true;
                            break;
                        }
                    }
                }

                if(!chFound && !cgFound){
                    ceFound = false;
                    break;
                }
            }
            bool userStateMatch = userstate.Equals(state);
            string resp = string.Format("{0} {1} {2} {3} {4}", ceFound, userStateMatch, 
                stateChanged, userstate, state);
            UnityEngine.Debug.Log(resp);
            Assert.True(ceFound & userStateMatch & stateChanged , resp);

        }

        [Test]
        public void TestCreateChannelEntity(){
            TestCreateChannelEntityCommon<string>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObj(){
            TestCreateChannelEntityCommon<object>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCG(){
            TestCreateChannelEntityCommon<string>(true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCG(){
            TestCreateChannelEntityCommon<object>(true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPres(){
            TestCreateChannelEntityCommon<string>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPres(){
            TestCreateChannelEntityCommon<object>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCGPres(){
            TestCreateChannelEntityCommon<string>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPres(){
            TestCreateChannelEntityCommon<object>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            string channelName = "ch1";
            if(isPresence){
                channelName = "ch1-pnpres";
            }

            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>(channelName, isAwaitingConnectCallback, 
                isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1, isChannelGroup, isAwaitingConnectCallback, isPresence, channelName,
                userCallback,
                connectCallback,
                disconnectCallback, 
                wildcardPresenceCallback);

        }

        [Test]
        public void TestCreateChannelEntityCGPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPresMulti(){
            TestCreateChannelEntityMultiCommon<string>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMulti(){
            TestCreateChannelEntityMultiCommon<object>(false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<string>(true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityObjCGPresMultiCb(){
            TestCreateChannelEntityMultiCommon<object>(true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityMultiCommon<T>( bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            var dictSM = new Dictionary<string, object>();
            dictSM.Add("k","v");
            dictSM.Add("k2","v2");

            string channelName = "ch1";

            if(isPresence){
                channelName = "ch1-pnpres";
            }
            string[] channelArr = {channelName};


            List<ChannelEntity> ce1 = Helpers.CreateChannelEntity<T>(channelArr, isAwaitingConnectCallback, 
                isChannelGroup, dictSM, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);
            CreateChannelEntityMatch<T>(ce1[0], isChannelGroup, isAwaitingConnectCallback, isPresence, channelName,
                userCallback,
                connectCallback,
                disconnectCallback, 
                wildcardPresenceCallback);

        }

        void CreateChannelEntityMatch<T>(ChannelEntity ce1, bool isChannelGroup, bool isAwaitingConnectCallback,
            bool isPresence, string channelName, Action<T> userCallback, Action<T> connectCallback,
            Action<T> disconnectCallback, Action<T> wildcardPresenceCallback){

            bool chMatch = ce1.ChannelID.ChannelOrChannelGroupName.Equals(channelName);
            bool isAwaitingConnectCallbackMatch = ce1.ChannelParams.IsAwaitingConnectCallback.Equals(isAwaitingConnectCallback);
            bool isPresenceMatch = ce1.ChannelID.IsPresenceChannel.Equals(isPresence);
            bool isChannelGroupMatch = ce1.ChannelID.IsChannelGroup.Equals(isChannelGroup);
            bool typeMatch = ce1.ChannelParams.TypeParameterType.Equals(typeof(T));

            PubnubChannelCallback<T> channelCallbacks = ce1.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            bool UserCallbackMatch = channelCallbacks.SuccessCallback == userCallback;
            bool ConnectCallbackMatch = channelCallbacks.ConnectCallback == connectCallback;
            bool ErrorCallbackUserStateMatch = channelCallbacks.ErrorCallback == ErrorCallbackUserState;
            bool DisconnectCallbackMatch = channelCallbacks.DisconnectCallback == disconnectCallback;
            bool WildcardPresenceCallbackMatch = channelCallbacks.WildcardPresenceCallback == wildcardPresenceCallback;

            var userState = ce1.ChannelParams.UserState as Dictionary<string, object>;
            bool userStateMatch = false;
            if(userState["k"].Equals("v") && userState["k2"].Equals("v2")){
                userStateMatch = true;
            }

            string resp = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", chMatch, 
                isAwaitingConnectCallbackMatch, 
                isPresenceMatch, isChannelGroupMatch, typeMatch,
                UserCallbackMatch, ConnectCallbackMatch, ErrorCallbackUserStateMatch,
                DisconnectCallbackMatch, WildcardPresenceCallbackMatch
            );
            UnityEngine.Debug.Log(resp);
            Assert.True(chMatch &
                isAwaitingConnectCallbackMatch &
                isPresenceMatch & isChannelGroupMatch & typeMatch &
                UserCallbackMatch & ConnectCallbackMatch & ErrorCallbackUserStateMatch &
                DisconnectCallbackMatch & WildcardPresenceCallbackMatch, resp);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribe(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObj(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCG(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCG(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPres(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, false, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresUnsub(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, true, false,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, false, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribePresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(false, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(false, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeCGPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<string>(true, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreateChannelEntityAndAddToSubscribeObjCGPresUnsubErrorCB(){
            TestCreateChannelEntityAndAddToSubscribeCommon<object>(true, true, true, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreateChannelEntityAndAddToSubscribeCommon<T>(bool isChannelGroup,
            bool isPresence, bool isUnsubscribe, bool testErrorCB, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            //subscribe
            //resubscribe -> Already sub

            //Subscribe

            //unsubscribe -> not subscribed

            //subscribe
            //unsubscrube

            //presence
            Subscription.Instance.CleanUp();
            ResponseType resp = ResponseType.SubscribeV2;
            if(isPresence && isUnsubscribe){
                resp = ResponseType.PresenceUnsubscribe;
            } else if(isPresence) {
                resp = ResponseType.PresenceV2;
            } else if(isUnsubscribe) {
                resp = ResponseType.Unsubscribe;
            }

            Action<PubnubClientError> errcb = Common.ErrorCallback;;

            string[] ch = {"ch1", "ch2"};
            string[] chpres = new string[ch.Length];
            for(int i=0; i<ch.Length; i++){
                chpres[i] = string.Format("{0}{1}", ch[i], Utility.PresenceChannelSuffix);
            }
            List<ChannelEntity> lstCE = new List<ChannelEntity>();
            if(testErrorCB && isUnsubscribe){
                errcb = ErrorCallbackCENotSub;
            } else if(testErrorCB){
                Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                    userCallback, connectCallback, 
                    Common.ErrorCallback, 
                    wildcardPresenceCallback,
                    disconnectCallback, PubnubErrorFilter.Level.Info, false, ref lstCE);
                errcb = ErrorCallbackCEAlreadySub;
                Subscription.Instance.Add (lstCE);
                lstCE.Clear();
            } else if(isUnsubscribe){
                Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                    userCallback, connectCallback, 
                    Common.ErrorCallback, 
                    wildcardPresenceCallback,
                    disconnectCallback, PubnubErrorFilter.Level.Info, false, ref lstCE);
                errcb = Common.ErrorCallback;
                Subscription.Instance.Add (lstCE);
                lstCE.Clear();
            }

            bool retBool = Helpers.CreateChannelEntityAndAddToSubscribe<T>(resp, ch, isChannelGroup, 
                userCallback, connectCallback, 
                errcb, 
                wildcardPresenceCallback,
                disconnectCallback, PubnubErrorFilter.Level.Info, isUnsubscribe, ref lstCE);

            bool ceFound = ParseListCE(lstCE, ch, chpres, isPresence, isChannelGroup, testErrorCB);
            string logStr = string.Format("{0} {1}", ceFound, retBool);
            UnityEngine.Debug.Log(logStr);
            if(!testErrorCB){
                Assert.True(ceFound & retBool, logStr);
            } else {
                Assert.True(ceFound & !retBool, logStr);
            }
        }

        public static bool ParseListCE(List<ChannelEntity> lstCE, string[] ch, string[] chpres,
            bool isPresence, bool isChannelGroup, bool testErrorCB
        ){
            bool ceFound = true;
            foreach(ChannelEntity ch2 in lstCE){
                if(isChannelGroup && !ch2.ChannelID.IsChannelGroup){
                    continue;
                }
                if(!isChannelGroup && ch2.ChannelID.IsChannelGroup){
                    continue;
                }

                bool chFound = false;
                for(int i=0; i<ch.Length; i++){
                    if(((!isPresence) && ch[i].Equals(ch2.ChannelID.ChannelOrChannelGroupName)) 
                        || ((isPresence) && chpres[i].Equals(ch2.ChannelID.ChannelOrChannelGroupName)))
                    {
                        bool presenceMatch = (isPresence)?ch2.ChannelID.IsPresenceChannel:true;
                        bool cgMatch = (isChannelGroup)?ch2.ChannelID.IsChannelGroup:true;
                        chFound = cgMatch & presenceMatch;
                        UnityEngine.Debug.Log (string.Format("{0} found, {1}, {2}, {3}", ch[i],
                            chFound, presenceMatch, cgMatch
                        ));
                        break;
                    }
                }
                if(!chFound){
                    UnityEngine.Debug.Log (string.Format("{0} not found", ch2.ChannelID.ChannelOrChannelGroupName

                    ));
                    ceFound = false;
                    break;
                }
            }
            return ceFound;
        }

        void ErrorCallbackCENotSub (PubnubClientError result)
        {
            PubnubErrorCode errorType =  PubnubErrorCode.NotSubscribed;
            if(result.Channel.Contains(Utility.PresenceChannelSuffix) 
                || result.ChannelGroup.Contains(Utility.PresenceChannelSuffix) ){
                errorType = PubnubErrorCode.NotPresenceSubscribed;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);
        }

        void ErrorCallbackCEAlreadySub (PubnubClientError result)
        {
            
            PubnubErrorCode errorType =  PubnubErrorCode.AlreadySubscribed;
            if(result.Channel.Contains(Utility.PresenceChannelSuffix) 
                || result.ChannelGroup.Contains(Utility.PresenceChannelSuffix) ){
                errorType = PubnubErrorCode.AlreadyPresenceSubscribed;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannels(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObj(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, false, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCG(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(false, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCG(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(false, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGnCH(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGnCH(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, true, false, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, false, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(false, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(false, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCGnCHPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<string>(true, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsObjCGnCHPres(){
            TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<object>(true, true, true, false, true,
                Common.UserCallback, Common.ConnectCallback, Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestRemoveDuplicatesCheckAlreadySubscribedAndGetChannelsCommon<T>(bool testChannelGroup,
            bool testChannel,
            bool isPresence, bool isUnsubscribe, bool testErrorCB, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            string[] ch = {"ch1", "ch2", "ch1", "ch2"};
            string[] chpres = new string[ch.Length];
            for(int i=0; i<ch.Length; i++){
                chpres[i] = string.Format("{0}{1}", ch[i], Utility.PresenceChannelSuffix);
            }
            string[] cg = {"cg1", "cg2", "cg1", "cg2"};
            string[] cgpres = new string[cg.Length];
            for(int i=0; i<cg.Length; i++){
                cgpres[i] = string.Format("{0}{1}", cg[i], Utility.PresenceChannelSuffix);
            }

            if(!testChannel){
                ch = null;
            }

            if(!testChannelGroup){
                cg = null;
            }

            List<ChannelEntity> lstCE;
            ResponseType resp = ResponseType.SubscribeV2;
            if(isPresence && isUnsubscribe){
                resp = ResponseType.PresenceUnsubscribe;
            } else if(isPresence) {
                resp = ResponseType.PresenceV2;
            } else if(isUnsubscribe) {
                resp = ResponseType.Unsubscribe;
            }

            bool retBool = Helpers.RemoveDuplicatesCheckAlreadySubscribedAndGetChannels(resp, userCallback,
                connectCallback, ErrorCallbackDup, wildcardPresenceCallback, disconnectCallback, ch, cg,
                PubnubErrorFilter.Level.Info, isUnsubscribe, out lstCE);

            bool ceFound = true;
            if(testChannel){
                ceFound = ParseListCE(lstCE, ch, chpres, isPresence, false, false);
            }
            bool cgFound = true;
            if(testChannelGroup){
                cgFound = ParseListCE(lstCE, cg, cgpres, isPresence, true, false);
            }

            string logStr = string.Format("{0} {1} {2}", ceFound, cgFound, retBool);
            UnityEngine.Debug.Log(logStr);
            //if(!testErrorCB){
                Assert.True(ceFound & cgFound & retBool, logStr);
            //}


        } 

        void ErrorCallbackDup (PubnubClientError result)
        {

            PubnubErrorCode errorType =  PubnubErrorCode.DuplicateChannel;
            if(!string.IsNullOrEmpty(result.ChannelGroup)){
                errorType = PubnubErrorCode.DuplicateChannelGroup;
            }
            string logstr = string.Format("{0} {1}", result.StatusCode, (int)errorType);
            UnityEngine.Debug.Log (logstr);
            Assert.True(result.StatusCode.Equals((int)errorType), logstr);

        }

        [Test]
        public void TestProcessResponseCallbacksV2(){
            TestProcessResponseCallbacksV2Common<string>(false, true, false, true, false, true,
                UserCallbackProcessReponse, ConnectCallbackProcessReponse, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2WC(){
            TestProcessResponseCallbacksV2Common<string>(false, false, true, false, false, true,
                UserCallbackProcessReponseWC, ConnectCallbackProcessReponseWC, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2WCPres(){
            TestProcessResponseCallbacksV2Common<string>(false, false, true, true, false, true,
                UserCallbackProcessReponseWC, ConnectCallbackProcessReponseWC, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestProcessResponseCallbacksV2CG(){
            TestProcessResponseCallbacksV2Common<string>(true, false, false, true, false, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestProcessResponseCallbacksV2Common<T>(bool testChannelGroup,
            bool testChannel, bool testwc,
            bool isPresence, bool isUnsubscribe, bool connectcallback, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );

            List<ChannelEntity> channelEntities = Common.CreateListOfChannelEntities(true, true, 
                true, true, userCallback, connectCallback, wildcardPresenceCallback, disconnectCallback);
            
            string uuid = "CustomUUID";
            bool testUUID = false;

            ChannelEntity ce8 = Helpers.CreateChannelEntity<T>("ch8-pnpres", true, true, null, 
                userCallback, 
                connectCallback, 
                Common.ErrorCallback, 
                disconnectCallback, 
                wildcardPresenceCallback
                );
            channelEntities.Add(ce8);

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                ResponseType.SubscribeV2, false, 0, false, 0, typeof(T), uuid,
                userCallback, Common.ErrorCallback
            );
              
            TimetokenMetadata ott = new TimetokenMetadata(14691896187882984, "");
            TimetokenMetadata ptt = new TimetokenMetadata(14691896542063327, "");
            SubscribeMessage sm = new SubscribeMessage("0", "cg2", "ch2", "test-cg", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm2 = new SubscribeMessage("0", "ch2", "ch2", "test", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm3 = new SubscribeMessage("0", "ch2.*", "ch2", "test-wc", "", "", "", 2, ott, ptt, null);
            SubscribeMessage sm4 = new SubscribeMessage("0", "ch2.*", "ch2", "test-pnpres", "", "", "", 2, ott, ptt, null);
            List<SubscribeMessage> smLst = new List<SubscribeMessage>();
            if(testwc && isPresence){
                smLst.Add(sm4);
            }else if(testwc){    
                smLst.Add(sm3);
            }else if(testChannelGroup){
                smLst.Add(sm);
            } else if (testChannel){
                smLst.Add(sm2);
            }

            SubscribeEnvelope subscribeEnvelope = new SubscribeEnvelope ();

            subscribeEnvelope.Messages = smLst;
            subscribeEnvelope.TimetokenMeta = new TimetokenMetadata(14691897960994791,"");

            Helpers.ProcessResponseCallbacksV2(ref subscribeEnvelope, requestState, "", pubnub.JsonPluggableLibrary);
        }

        public  void UserCallbackProcessReponse (string result)
        {
            TestAssertions(result, false, false, false);

        }

        public  void UserCallbackProcessReponse (object result)
        {
            TestAssertions(result.ToString(), false, false, false);
        }

        public  void ConnectCallbackProcessReponse (string result)
        {
            TestAssertions(result, true, false, false);
        }

        public  void ConnectCallbackProcessReponse (object result)
        {
            TestAssertions(result.ToString(), true, false, false);
        }

        public  void UserCallbackProcessReponseWC (string result)
        {
            TestAssertions(result, false, true, false);

        }

        public  void UserCallbackProcessReponseWC (object result)
        {
            TestAssertions(result.ToString(), false, true, false);
        }

        public  void ConnectCallbackProcessReponseWC (string result)
        {
            TestAssertions(result, true, true, false);
        }

        public  void ConnectCallbackProcessReponseWC (object result)
        {
            TestAssertions(result.ToString(), true, true, false);
        }

        public  void UserCallbackProcessReponseCG (string result)
        {
            TestAssertions(result, false, false, true);

        }

        public  void UserCallbackProcessReponseCG (object result)
        {
            TestAssertions(result.ToString(), false, false, true);
        }

        public  void ConnectCallbackProcessReponseCG (string result)
        {
            TestAssertions(result, true, false, true);
        }

        public  void ConnectCallbackProcessReponseCG (object result)
        {
            TestAssertions(result.ToString(), true, false, true);
        }

        void TestAssertions(string result, bool connect, bool wc, bool cg){
            if(wc){
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch2.*"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-pnpres"));

                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("ch2.*"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-wc"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            } else if (cg){
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch8-pnpres"));
                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("cg2"));
                        Assert.True(result.Contains("ch2"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("cg2"));
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test-cg"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            } else{
                if(connect){
                    UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                        Assert.True(result.Contains("Presence Connected"));
                        Assert.True(result.Contains("ch8-pnpres"));
                    } else {
                        Assert.True(result.Contains("Connected"));
                        Assert.True(result.Contains("ch2"));
                    }
                } else {
                    UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
                    if(result.Contains(Utility.PresenceChannelSuffix)){
                    } else {
                        Assert.True(result.Contains("ch2"));
                        Assert.True(result.Contains("test"));
                        Assert.True(result.Contains("14691897960994791"));

                    }
                }
            }
        }

        [Test]
        public void TestCreatePubnubClientErrorWebEx(){
            TestCreatePubnubClientErrorCommon<string>(false, false, false, false, true,
                UserCallbackProcessReponseWC, ConnectCallbackProcessReponseWC, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreatePubnubClientErrorMessageCh(){
            TestCreatePubnubClientErrorCommon<string>(false, false, false, true, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreatePubnubClientErrorMessageCE(){
            TestCreatePubnubClientErrorCommon<string>(false, false, true, false, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreatePubnubClientErrorExCh(){
            TestCreatePubnubClientErrorCommon<string>(false, true, false, false, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestCreatePubnubClientErrorExCE(){
            TestCreatePubnubClientErrorCommon<string>(true, false, false, false, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        public void TestCreatePubnubClientErrorCommon<T>(bool testExceptionCE,
            bool testExceptionChannels, bool testMessageCE,
            bool testMessageChannels, bool testWebException, Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            List<ChannelEntity> channelEntities = Common.CreateListOfChannelEntities(false, true, 
                true, true, userCallback, connectCallback, wildcardPresenceCallback, disconnectCallback);

            string uuid = "CustomUUID";

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                ResponseType.SubscribeV2, false, 0, false, 0, typeof(T), uuid,
                userCallback, Common.ErrorCallback
            );

            string ch = Helpers.GetNamesFromChannelEntities(channelEntities, false);
            string cg = Helpers.GetNamesFromChannelEntities(channelEntities, true);

            if(testWebException){
                string message = "testWebException";
                WebException webex = new WebException(message, WebExceptionStatus.ConnectFailure);

                PubnubClientError pubnubClientError = Helpers.CreatePubnubClientError(webex, requestState, ch
                    , PubnubErrorSeverity.Info); 
                ParsePubnubClientError(pubnubClientError, ch, cg, message, 
                    PubnubErrorSeverity.Info, (int)PubnubErrorCode.ConnectFailure);
            } else if (testExceptionCE){
                string message = "Test Exception";
                Exception ex = new Exception (message);

                PubnubClientError pubnubClientError = Helpers.CreatePubnubClientError<T> (ex, requestState, 
                    channelEntities, PubnubErrorCode.None,
                    PubnubErrorSeverity.Info);  
                ParsePubnubClientError(pubnubClientError, ch, cg, message, 
                    PubnubErrorSeverity.Info, (int)PubnubErrorCode.None);
            } else if (testExceptionChannels){
                string message = "testExceptionChannels";
                PubnubErrorCode pnErrorCode = PubnubErrorCode.UnsubscribedAfterMaxRetries;

                PubnubClientError pubnubClientError = Helpers.CreatePubnubClientError<T> (message, null, 
                    pnErrorCode,
                    PubnubErrorSeverity.Info, ch, cg );  
                ParsePubnubClientError(pubnubClientError, ch, cg, message, 
                    PubnubErrorSeverity.Info, (int)pnErrorCode);
            } else if (testMessageCE){
                string message = "testMessageCE";
                PubnubErrorCode pnErrorCode = PubnubErrorCode.UnsubscribedAfterMaxRetries;

                PubnubClientError pubnubClientError = Helpers.CreatePubnubClientError<T> (message, null, 
                    channelEntities, pnErrorCode,
                    PubnubErrorSeverity.Warn);  
                ParsePubnubClientError(pubnubClientError, ch, cg, message, 
                    PubnubErrorSeverity.Warn, (int)pnErrorCode);
            } else if (testMessageChannels){
                Exception ex = new Exception ("Test Exception");
                string message = "testMessageChannels";
                PubnubErrorCode pnErrorCode = PubnubErrorCode.UnsubscribedAfterMaxRetries;

                PubnubClientError pubnubClientError = Helpers.CreatePubnubClientError<T> (message, null, 
                    pnErrorCode,
                    PubnubErrorSeverity.Critical, ch, cg );  
                ParsePubnubClientError(pubnubClientError, ch, cg, message, 
                    PubnubErrorSeverity.Critical, (int)pnErrorCode);
                
            }
        }

        void ParsePubnubClientError(PubnubClientError pubnubClientError, 
            string channel, string channelGroup, string message, 
            PubnubErrorSeverity severity, int statusCode){
            if(channel!=""){
                Assert.IsTrue(pubnubClientError.Channel.Equals(channel), "CH");
            }
            if(channelGroup!=""){
                Assert.IsTrue(pubnubClientError.ChannelGroup.Equals(channelGroup), "CG");
            }
            Assert.IsTrue(pubnubClientError.Message.Equals(message), "message:"+pubnubClientError.Message);
            Assert.IsTrue(pubnubClientError.Severity.Equals(severity), "severity:"+pubnubClientError.Severity);
            Assert.IsTrue(pubnubClientError.StatusCode.Equals(statusCode), "statusCode:"+ pubnubClientError.StatusCode + statusCode);
        }

        [Test]
        public void TestJsonEncodePublishMsg(){
            TestJsonEncodePublishMsgCommon("testmessage", false, "\"testmessage\"");
        }

        [Test]
        public void TestJsonEncodePublishMsgCipher(){
            TestJsonEncodePublishMsgCommon("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"");
        }

        public void TestJsonEncodePublishMsgCommon(object message, bool cipher, string expected
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );

            string str = Helpers.JsonEncodePublishMsg(message, cipher?"enigma":"", pubnub.JsonPluggableLibrary);
            Assert.True(str.Equals(expected), str);
        }


        [Test]
        public void TestWrapResultBasedOnResponseTypeSubV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", false, "{\"t\":{\"t\":\"14694563235781852\",\"r\":4},\"m\":[{\"a\":\"1\",\"f\":514,\"p\":{\"t\":\"14694563234675841\",\"r\":3},\"k\":\"demo\",\"c\":\"hello_world\",\"d\":\"Hello World\",\"b\":\"hello_world\"}]}",
                false, ResponseType.SubscribeV2,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        /*[Test]
        public void TestWrapResultBasedOnResponseTypeCipherSubV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"",
                false,ResponseType.SubscribeV2,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        /*[Test]
        public void TestWrapResultBasedOnResponseTypePresV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", false, "\"testmessage\"",
                false, ResponseType.PresenceV2,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeCipherPresV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"",
                false,ResponseType.PresenceV2,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }*/

        [Test]
        public void TestWrapResultBasedOnResponseTypePub(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", false, "[1, \"Sent\", 14606134331557853]",
                false, ResponseType.Publish,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeCipherPub(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "[1, \"Sent\", 14606134331557853]",
                false,ResponseType.Publish,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeDH(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", false, "[[\"m1\",\"m2\",\"m3\"],14606134331557853,14606134485013970]",
                false, ResponseType.DetailedHistory,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeCipherDH(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "[[\"EGwV+Ti43wh2TprPIq7o0KMuW5j6B3yWy352ucWIOmU=\\n\",\"EGwV+Ti43wh2TprPIq7o0KMuW5j6B3yWy352ucWIOmU=\\n\",\"EGwV+Ti43wh2TprPIq7o0KMuW5j6B3yWy352ucWIOmU=\\n\"],14606134331557853,14606134485013970]",
                false,ResponseType.DetailedHistory,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeTime(){
            TestWrapResultBasedOnResponseTypeCommon<string>(14606134331557853, false, "[14606134331557853]",
                false, ResponseType.Time,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeLeave(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "{\"action\":\"leave\"}",
                false,ResponseType.Leave,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeHN(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "{\"status\": 200, \"message\": \"OK\", \"payload\": {\"hello_world\": {\"jf\": \"k\"}}, \"service\": \"Presence\"}",
                false,ResponseType.HereNow,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        /*[Test]
        public void TestWrapResultBasedOnResponseTypeCipherSubV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"",
                false,ResponseType.ChannelGroupAdd,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeCipherSubV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"",
                false,ResponseType.GlobalHereNow,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }

        [Test]
        public void TestWrapResultBasedOnResponseTypeCipherSubV2(){
            TestWrapResultBasedOnResponseTypeCommon<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"",
                false,ResponseType.ChannelGroupGrantAccess,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback);
        }*/

        public void TestWrapResultBasedOnResponseTypeCommon<T>(object expectedMessage, bool cipher, string json,
            bool isChannelGroup, ResponseType respType,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );
            List<ChannelEntity> channelEntities = Common.CreateListOfChannelEntities(isChannelGroup, true, 
                false, true, userCallback, connectCallback, wildcardPresenceCallback, disconnectCallback);

            string uuid = "CustomUUID";

            RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channelEntities, 
                respType, false, 0, false, 0, typeof(T), uuid,
                userCallback, Common.ErrorCallback
            );

            string ch = Helpers.GetNamesFromChannelEntities(channelEntities, false);
            string cg = Helpers.GetNamesFromChannelEntities(channelEntities, true);

            List<object> lstObj = new List<object>();
            Helpers.WrapResultBasedOnResponseType(requestState, json, pubnub.JsonPluggableLibrary,
                PubnubErrorFilter.Level.Info,
                cipher?"enigma":"", ref lstObj);

            UnityEngine.Debug.Log(string.Format("COUNT: {0} {1}",
                lstObj.Count, respType)
            );

            switch (respType) {
            case ResponseType.DetailedHistory:
                Assert.True(lstObj.Count.Equals(4));
                //Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                Assert.True(lstObj[1].Equals(14606134331557853), lstObj[1].ToString());
                Assert.True(lstObj[2].Equals(14606134485013970), lstObj[2].ToString());
                Assert.True(lstObj[3].Equals("ch1,ch2,ch7"), lstObj[3].ToString());
                break;
            case ResponseType.Time:
                Assert.True(lstObj.Count.Equals(1));
                Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                break;
            case ResponseType.Leave:
                Assert.True(lstObj.Count.Equals(2));
                //Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                Assert.True(lstObj[1].Equals("ch1,ch2,ch7"), lstObj[1].ToString());
                break;
            case ResponseType.SubscribeV2:
            case ResponseType.PresenceV2:
                Assert.True(lstObj.Count.Equals(2));
                //Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                //Assert.True(lstObj[1].Equals("ch1,ch2,ch7"), lstObj[1].ToString());
                break;
            case ResponseType.Publish:
            case ResponseType.PushRegister:
            case ResponseType.PushRemove:
            case ResponseType.PushGet:
            case ResponseType.PushUnregister:
                Assert.True(lstObj.Count.Equals(4));
                Assert.True(lstObj[0].Equals(1), lstObj[0].ToString());
                Assert.True(lstObj[1].Equals("Sent"), lstObj[1].ToString());
                Assert.True(lstObj[2].Equals(14606134331557853), lstObj[2].ToString());
                Assert.True(lstObj[3].Equals("ch1,ch2,ch7"), lstObj[3].ToString());
                break;
            case ResponseType.GrantAccess:
            case ResponseType.AuditAccess:
            case ResponseType.RevokeAccess:
            case ResponseType.GetUserState:
            case ResponseType.SetUserState:
            case ResponseType.WhereNow:
            case ResponseType.HereNow:
                Assert.True(lstObj.Count.Equals(2));
                //Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                Assert.True(lstObj[1].Equals("ch1,ch2,ch7"), lstObj[1].ToString());
                break;
            case ResponseType.GlobalHereNow:
                Assert.True(lstObj.Count.Equals(2));
                Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                Assert.True(lstObj[1].Equals("ch1,ch2,ch7"), lstObj[1].ToString());
                break;
            case ResponseType.ChannelGroupAdd:
            case ResponseType.ChannelGroupRemove:
            case ResponseType.ChannelGroupGet:
                Assert.True(lstObj.Count.Equals(1));
                Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());

                break;
            case ResponseType.ChannelGroupGrantAccess:
            case ResponseType.ChannelGroupAuditAccess:
            case ResponseType.ChannelGroupRevokeAccess:
                Assert.True(lstObj.Count.Equals(1));
                Assert.True(lstObj[0].Equals(expectedMessage), lstObj[0].ToString());
                break;

            default:
                break;
            }


/*            UnityEngine.Debug.Log(string.Format("{0} {1} {2} {3}",
                lstObj[0].ToString(),
                lstObj[1].ToString(),
                lstObj[2].ToString(),
                (isChannelGroup)?lstObj[3].ToString():"")
            );*/

        }

        [Test]
        public void TestAddMessageToListV2(){
            TestAddMessageToListV2Common<string>("testmessage", false, "\"testmessage\"", false, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2Cipher(){
            TestAddMessageToListV2Common<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"", false, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2CG(){
            TestAddMessageToListV2Common<string>("testmessage", false, "\"testmessage\"", true, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2CipherCG(){
            TestAddMessageToListV2Common<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"", true, false,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2Pres(){
            TestAddMessageToListV2Common<string>("testmessage", false, "\"testmessage\"", false, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2CipherPres(){
            TestAddMessageToListV2Common<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"", false, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2CGPres(){
            TestAddMessageToListV2Common<string>("testmessage", false, "\"testmessage\"", true, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        [Test]
        public void TestAddMessageToListV2CipherCGPres(){
            TestAddMessageToListV2Common<string>("testmessage", true, "\"cpStRYxH32LTowAezXLlxw==\"", true, true,
                UserCallbackProcessReponseCG, ConnectCallbackProcessReponseCG, 
                Common.WildcardPresenceCallback, Common.DisconnectCallback
            );
        }

        public void TestAddMessageToListV2Common<T>(object message, bool cipher, string expected, 
            bool isChannelGroup, bool isPresence,
            Action<T> userCallback, Action<T> connectCallback,
            Action<T> wildcardPresenceCallback, Action<T> disconnectCallback
        ){
            Pubnub pubnub = new Pubnub (
                Common.PublishKey,
                Common.SubscribeKey,
                "",
                "",
                true
            );
            string ch = (isChannelGroup)?"cg2":"ch2";
            string pres =  (isPresence)?ch+"-pnpres":ch;

            ChannelEntity ce1 = Helpers.CreateChannelEntity<T>(pres, 
                false, isChannelGroup, null, 
                userCallback, connectCallback,
                ErrorCallbackUserState, disconnectCallback, 
                wildcardPresenceCallback);

            List<object> lstObj;
            TimetokenMetadata ott = new TimetokenMetadata(14691896187882984, "");
            TimetokenMetadata ptt = new TimetokenMetadata(14691896542063327, "");
            SubscribeMessage sm = new SubscribeMessage("0", "cg2", "ch2", message, "", "", "", 2, ott, ptt, null);

            Helpers.AddMessageToListV2(cipher?"enigma":"", pubnub.JsonPluggableLibrary,
                sm, ce1, out lstObj);

            UnityEngine.Debug.Log(string.Format("{0} {1} {2} {3}",
                lstObj[0].ToString(),
                lstObj[1].ToString(),
                lstObj[2].ToString(),
                (isChannelGroup)?lstObj[3].ToString():"")
            );

            Assert.True(lstObj[0].Equals(message), lstObj[0].ToString());
            Assert.True(lstObj[1].Equals("14691896542063327"), lstObj[1].ToString());
            if(isChannelGroup){
                Assert.True(lstObj[2].Equals("cg2"), lstObj[2].ToString());
                Assert.True(lstObj[3].Equals("ch2"), lstObj[3].ToString());
            } else {
                Assert.True(lstObj[2].Equals("ch2"), lstObj[2].ToString());
            }
        }

        #endif
    }
}

