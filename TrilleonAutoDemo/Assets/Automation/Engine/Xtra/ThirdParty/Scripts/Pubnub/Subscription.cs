using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubMessaging.Core
{
    public struct ChannelIdentity
    {
        public string ChannelOrChannelGroupName {get; set;}
        public bool IsChannelGroup {get; set;}
        public bool IsPresenceChannel {get; set;}

        public ChannelIdentity(string channelOrChannelGroupName, bool isChannelGroup, bool isPresenceChannel){
            ChannelOrChannelGroupName = channelOrChannelGroupName;
            IsChannelGroup = isChannelGroup;
            IsPresenceChannel = isPresenceChannel;
        }
    }

    public class ChannelParameters
    {
        public bool IsAwaitingConnectCallback {get; set;}

        public bool IsSubscribed {get; set;}
        public object Callbacks {get; set;}
        public Dictionary<string, object> UserState {get; set;}
        public Type TypeParameterType {get; set;}

        public ChannelParameters(){
            IsAwaitingConnectCallback = false;
            IsSubscribed = false;
            UserState = null;
            Callbacks = null;
            TypeParameterType = null;
        }
    }

    public class ChannelEntity
    {
        public ChannelIdentity ChannelID;
        public ChannelParameters ChannelParams;
        public ChannelEntity(ChannelIdentity channelID, ChannelParameters channelParams){
            this.ChannelID = channelID;
            this.ChannelParams = channelParams;
        }
    }

    public sealed class Subscription
    {
        private static volatile Subscription instance;
        private static object syncRoot = new Object();

        public static Subscription Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null) 
                            instance = new Subscription();
                        
                        instance.ChannelsAndChannelGroupsAwaitingConnectCallback = new List<ChannelEntity> ();
                        instance.AllPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
                        instance.AllNonPresenceChannelsOrChannelGroups = new List<ChannelEntity> ();
                        instance.AllChannels = new List<ChannelEntity> ();
                        instance.AllChannelGroups = new List<ChannelEntity> ();
                        instance.AllSubscribedChannelsAndChannelGroups  = new List<ChannelEntity> ();
                    }
                }

                return instance;
            }
        }

        public bool HasChannelGroups {get; private set;}
        public bool HasPresenceChannels {get; private set;}
        public bool HasChannelsOrChannelGroups {get; private set;}
        public bool HasChannels {get; private set;}

        public int CurrentSubscribedChannelsCount {
            get;
            private set;
        }

        public int CurrentSubscribedChannelGroupsCount {
            get;
            private set;
        }

        public List<ChannelEntity> ChannelsAndChannelGroupsAwaitingConnectCallback {
            get;
            private set;
        }

        public List<ChannelEntity> AllChannels {
            get;
            private set;
        }

        public List<ChannelEntity> AllChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllSubscribedChannelsAndChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllPresenceChannelsOrChannelGroups {
            get;
            private set;
        }

        public List<ChannelEntity> AllNonPresenceChannelsOrChannelGroups {
            get;
            private set;
        }

        public string CompiledUserState {
            get;
            private set;
        }

        public bool ConnectCallbackSent {
            get;
            private set;
            //update SubscribedChannelsAndChannelGroupsAwaitingConnectCallback
            //and set isAwaitingConnectCallback false
        }

        public SafeDictionary<ChannelIdentity, ChannelParameters> ChannelEntitiesDictionary 
        {
            get {return channelEntitiesDictionary;}
        }

        private SafeDictionary<ChannelIdentity, ChannelParameters> channelEntitiesDictionary = new SafeDictionary<ChannelIdentity, ChannelParameters>();

        public void Add(ChannelEntity channelEntity, bool reset){
            
            if (!channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {
                channelEntity.ChannelParams.IsSubscribed = true;
                channelEntitiesDictionary.Add (channelEntity.ChannelID, channelEntity.ChannelParams);
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Add: channelEntities key add {1} {2} {3}", DateTime.Now.ToString (), 
                    channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup, 
                    channelEntity.ChannelParams.IsSubscribed
                ), LoggingMethod.LevelInfo);
                #endif
            } else {
                channelEntitiesDictionary [channelEntity.ChannelID].Callbacks = channelEntity.ChannelParams.Callbacks;
                channelEntitiesDictionary [channelEntity.ChannelID].IsAwaitingConnectCallback = channelEntity.ChannelParams.IsAwaitingConnectCallback;
                channelEntitiesDictionary [channelEntity.ChannelID].IsSubscribed = true;
                channelEntitiesDictionary [channelEntity.ChannelID].TypeParameterType = channelEntity.ChannelParams.TypeParameterType;
                Dictionary<string, object> userState = channelEntitiesDictionary [channelEntity.ChannelID].UserState;
                if (userState == null) {
                    channelEntitiesDictionary [channelEntity.ChannelID].UserState = channelEntity.ChannelParams.UserState;
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Add: channelEntities key update {1} {2} {3}", DateTime.Now.ToString (), 
                    channelEntity.ChannelID.ChannelOrChannelGroupName, channelEntity.ChannelID.IsChannelGroup,
                    channelEntity.ChannelParams.IsSubscribed
                ), LoggingMethod.LevelInfo);
                #endif
            }
            if (reset) {
                ResetChannelsAndChannelGroupsAndBuildState ();
            }
        }

        public void Add(List<ChannelEntity> channelEntities){
            foreach (ChannelEntity ce in channelEntities) {
                Add (ce, false);
            }
            ResetChannelsAndChannelGroupsAndBuildState ();
        }

        public bool Delete(ChannelEntity channelEntity)
        {
            ChannelParameters cp;
            bool bDeleted = channelEntitiesDictionary.Remove(channelEntity.ChannelID, out cp);
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Delete: channelEntities key found {1} {2}", DateTime.Now.ToString (), 
                channelEntity.ChannelID.ChannelOrChannelGroupName, bDeleted.ToString()), LoggingMethod.LevelInfo);
            #endif

            ResetChannelsAndChannelGroupsAndBuildState ();
            return bDeleted;
        }

        public void ResetChannelsAndChannelGroupsAndBuildState(){
            AllPresenceChannelsOrChannelGroups.Clear ();
            AllNonPresenceChannelsOrChannelGroups.Clear ();
            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            AllChannels.Clear ();
            AllChannelGroups.Clear ();
            AllSubscribedChannelsAndChannelGroups.Clear ();
            HasChannelGroups = false;
            HasChannels = false;
            HasPresenceChannels = false;
            HasChannelsOrChannelGroups = false;
            CurrentSubscribedChannelsCount = 0;
            CurrentSubscribedChannelGroupsCount = 0;

            foreach (var ci in channelEntitiesDictionary) {
                if (ci.Value.IsSubscribed) {
                    if (ci.Key.IsChannelGroup) {
                        CurrentSubscribedChannelGroupsCount++;
                        AllChannelGroups.Add(new ChannelEntity (ci.Key, ci.Value));
                    } else {
                        CurrentSubscribedChannelsCount++;
                        AllChannels.Add(new ChannelEntity (ci.Key, ci.Value));
                    }
                    AllSubscribedChannelsAndChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));

                    if (ci.Key.IsPresenceChannel) {
                        AllPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    } else {
                        AllNonPresenceChannelsOrChannelGroups.Add (new ChannelEntity (ci.Key, ci.Value));
                    }

                    if (ci.Value.IsAwaitingConnectCallback) {
                        ChannelsAndChannelGroupsAwaitingConnectCallback.Add (new ChannelEntity (ci.Key, ci.Value));
                    }
                }
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ResetChannelsAndChannelGroupsAndBuildState: channelEntities subscription key/val {1} {2}", DateTime.Now.ToString (), 
                    ci.Key.ChannelOrChannelGroupName, ci.Value.IsSubscribed), LoggingMethod.LevelInfo);
                #endif
            }
            if(CurrentSubscribedChannelGroupsCount > 0){
                HasChannelGroups = true;
            }
            if(CurrentSubscribedChannelsCount > 0){
                HasChannels = true;
            }
            if (AllPresenceChannelsOrChannelGroups.Count > 0) {
                HasPresenceChannels = true;
            }
            if(HasChannels || HasChannelGroups){
                HasChannelsOrChannelGroups = true;
            }
            CompiledUserState = Helpers.BuildJsonUserState (AllSubscribedChannelsAndChannelGroups);
        }

        public Dictionary<string, object> EditUserState(Dictionary<string, object> newUserState, 
            Dictionary<string, object> oldUserState, bool edit)
        {
            if(newUserState != null){
                string[] userStateKeys = newUserState.Keys.ToArray<string> ();
                for (int keyIndex = 0; keyIndex < userStateKeys.Length; keyIndex++) {
                    string userStateKey = userStateKeys [keyIndex];
                    object userStateObj = newUserState [userStateKey];

                    if(oldUserState.ContainsKey(userStateKey)){
                        if(userStateObj != null){
                            oldUserState[userStateKey] = userStateObj;
                        } else {
                            oldUserState.Remove(userStateKey);
                        }
                    } else {
                        if(userStateObj != null){
                            oldUserState.Add(userStateKey, userStateObj);
                        }
                    }

                }
            }
            #if (ENABLE_PUBNUB_LOGGING)
            foreach(KeyValuePair<string, object> kvp in oldUserState){
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, EditUserState: userstate kvp: {1}, {2}, edit: {3}\n",
                    DateTime.Now.ToString(), 
                    kvp.Key, kvp.Value, edit), LoggingMethod.LevelInfo);
            }
            #endif
            return oldUserState;
        }

        public bool UpdateOrAddUserStateOfEntity(ref ChannelEntity channelEntity, Dictionary<string, object> userState, bool edit){
            bool stateChanged = false;
            if (channelEntitiesDictionary.ContainsKey (channelEntity.ChannelID)) {
                
                string newState = PubnubUnity.JsonPluggableLibrary.SerializeToJsonString (userState);
                if (channelEntitiesDictionary [channelEntity.ChannelID].UserState != null) {
                    string oldState = Helpers.BuildJsonUserState (channelEntitiesDictionary [channelEntity.ChannelID].UserState);
                    if (!oldState.Equals (newState)) {
                        channelEntitiesDictionary [channelEntity.ChannelID].UserState = EditUserState(userState, 
                            channelEntitiesDictionary [channelEntity.ChannelID].UserState, edit);

                        stateChanged = true;
                    }
                } else {
                    channelEntitiesDictionary [channelEntity.ChannelID].UserState = userState;
                    stateChanged = true;
                }

            } else {
                channelEntity.ChannelParams.UserState = userState;
                channelEntity.ChannelParams.IsSubscribed = false;
                channelEntitiesDictionary.Add (channelEntity.ChannelID, channelEntity.ChannelParams);
                stateChanged = true;
            }
            if(stateChanged){
                channelEntity.ChannelParams.UserState = channelEntitiesDictionary[channelEntity.ChannelID].UserState;
            }
            ResetChannelsAndChannelGroupsAndBuildState ();

            return stateChanged;
        }

        public void UpdateIsAwaitingConnectCallbacksOfEntity(List<ChannelEntity> channelEntity, bool isAwaitingConnectCallback){
            foreach (ChannelEntity ce in channelEntity) {
                if (channelEntitiesDictionary.ContainsKey (ce.ChannelID)) {
                    channelEntitiesDictionary [ce.ChannelID].IsAwaitingConnectCallback = isAwaitingConnectCallback;
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, UpdateIsAwaitingConnectCallbacksOfEntity key/val1 {1} {2}", DateTime.Now.ToString (), 
                        ce.ChannelID.ChannelOrChannelGroupName, ce.ChannelID.IsChannelGroup.ToString()), LoggingMethod.LevelInfo);
                    #endif
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else 
                {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, UpdateIsAwaitingConnectCallbacksOfEntity not found key/val1 {1} {2}", DateTime.Now.ToString (), 
                        ce.ChannelID.ChannelOrChannelGroupName, ce.ChannelID.IsChannelGroup.ToString()), LoggingMethod.LevelInfo);
                    Helpers.LogChannelEntitiesDictionary();
                }
                #endif
                
            }

            bool connectCallbackSent = true;

            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            foreach (var ci in channelEntitiesDictionary) {
                if (ci.Value.IsAwaitingConnectCallback) {
                    connectCallbackSent = false;
                    ChannelsAndChannelGroupsAwaitingConnectCallback.Add (new ChannelEntity(ci.Key, ci.Value));
                }
            }

            ConnectCallbackSent = connectCallbackSent;
        }

        public void CleanUp(){
            ConnectCallbackSent = false;
            ChannelsAndChannelGroupsAwaitingConnectCallback.Clear ();
            channelEntitiesDictionary.Clear ();
            AllPresenceChannelsOrChannelGroups.Clear ();
            AllNonPresenceChannelsOrChannelGroups.Clear ();
            AllChannels.Clear ();
            AllChannelGroups.Clear ();
            AllSubscribedChannelsAndChannelGroups.Clear ();
            CompiledUserState = String.Empty;
            CurrentSubscribedChannelsCount = 0;
            CurrentSubscribedChannelGroupsCount = 0;
            HasChannelGroups = false;
            HasChannels = false;
            HasChannelsOrChannelGroups = false;
            HasPresenceChannels = false;
        }
    }
}

