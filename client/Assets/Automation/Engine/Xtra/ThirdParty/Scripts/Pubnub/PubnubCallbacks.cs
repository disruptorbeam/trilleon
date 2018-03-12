using System;
using System.Collections.Generic;
using System.Net;

namespace PubNubMessaging.Core
{
    #region "Channel callback"
    internal class PubnubChannelCallback<T>
    {
        public Action<T> SuccessCallback;
        public Action<PubnubClientError> ErrorCallback;
        public Action<PNMessageResult> MessageCallback;
        public Action<T> ConnectCallback;
        public Action<T> DisconnectCallback;
        public Action<T> WildcardPresenceCallback;

        public PubnubChannelCallback ()
        {
            SuccessCallback = null;
            ConnectCallback = null;
            DisconnectCallback = null;
            ErrorCallback = null;
            WildcardPresenceCallback = null;
            MessageCallback = null;
        }
    }

    public enum CallbackType
    {
        Success,
        Message,
        Connect,
        Error,
        Disconnect,
        Wildcard
    }
    #endregion

    internal static class PubnubCallbacks
    {
        internal static void SendCallbacks<T>(IJsonPluggableLibrary jsonPluggableLibrary, ChannelEntity channelEntity, 
            List<object> itemMessage, CallbackType callbackType, bool checkType)
        {
            if ((itemMessage != null) && (itemMessage.Count > 0)) {
                SendCallbackChannelEntity<T> (jsonPluggableLibrary, channelEntity, itemMessage, callbackType, checkType);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks: itemMessage null", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);

            }
            #endif
        }

        internal static void SendCallbackChannelEntity<T>(IJsonPluggableLibrary jsonPluggableLibrary, ChannelEntity channelEntity, 
            List<object> itemMessage, CallbackType callbackType, bool checkType)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbackChannelEntity: currentChannel: {1},  typeof(T): {2}, TypeParameterType: {3}", DateTime.Now.ToString (), 
                channelEntity.ChannelID.ChannelOrChannelGroupName,
                typeof(T).ToString (), channelEntity.ChannelParams.TypeParameterType
            ), LoggingMethod.LevelInfo);
            #endif

            if (checkType) {
                if ((channelEntity.ChannelParams.TypeParameterType.Equals (typeof(string)))
                    || (channelEntity.ChannelParams.TypeParameterType.Equals (typeof(object)))) {
                    SendCallback<T> (jsonPluggableLibrary, channelEntity, itemMessage, callbackType);
                }
            } else {
                SendCallback<T> (jsonPluggableLibrary, channelEntity, itemMessage, callbackType);
            }
        }

        internal static void SendCallbacks<T>(IJsonPluggableLibrary jsonPluggableLibrary, RequestState<T> asynchRequestState, 
            List<object> itemMessage, CallbackType callbackType, bool checkType)
        {
            if (asynchRequestState.ChannelEntities != null) {
                SendCallbacks<T> (jsonPluggableLibrary, asynchRequestState.ChannelEntities, itemMessage, callbackType, checkType);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks1: Callback type={1}", DateTime.Now.ToString (), callbackType.ToString()), LoggingMethod.LevelInfo);
                #endif

                if (callbackType.Equals(CallbackType.Success)) {
                    GoToCallback<T> (itemMessage, asynchRequestState.SuccessCallback, jsonPluggableLibrary);
                }
            }
        }

        internal static void SendCallbackBasedOnType <T>(IJsonPluggableLibrary jsonPluggableLibrary, PubnubChannelCallback<T> channelCallbacks, 
            List<object> itemMessage, CallbackType callbackType)
        {
            if (callbackType.Equals (CallbackType.Connect)) {
                GoToCallback<T> (itemMessage, channelCallbacks.ConnectCallback, jsonPluggableLibrary);
            } else if (callbackType.Equals (CallbackType.Disconnect)) {
                GoToCallback<T> (itemMessage, channelCallbacks.DisconnectCallback, jsonPluggableLibrary);
            } else if (callbackType.Equals (CallbackType.Success)) {
                GoToCallback<T> (itemMessage, channelCallbacks.SuccessCallback, jsonPluggableLibrary);
            } else if (callbackType.Equals (CallbackType.Wildcard)) {
                GoToCallback<T> (itemMessage, channelCallbacks.WildcardPresenceCallback, jsonPluggableLibrary);
            }
        }

        internal static void SendCallbacks<T>(IJsonPluggableLibrary jsonPluggableLibrary, List<ChannelEntity> channelEntities, 
            List<object> itemMessage, CallbackType callbackType, bool checkType)
        {

            if (channelEntities != null) {
                if ((itemMessage != null) && (itemMessage.Count > 0)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks2: itemMessage.Count={1} {2}", DateTime.Now.ToString (), 
                        itemMessage.Count.ToString(), channelEntities.Count.ToString()
                    ), LoggingMethod.LevelInfo);
                    #endif

                    foreach (ChannelEntity channelEntity in channelEntities) {
                        SendCallbackChannelEntity<T> (jsonPluggableLibrary, channelEntity, itemMessage, callbackType, checkType);
                    }
                } 
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks2: itemMessage null or count <0", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);

                }
                #endif
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks2: channelEntities null", 
                    DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
                
            }
            #endif
        }

        internal static void SendCallback<T>(IJsonPluggableLibrary jsonPluggableLibrary, ChannelEntity channelEntity, 
            List<object> itemMessage, CallbackType callbackType){
            PubnubChannelCallback<T> channelCallbacks = channelEntity.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            if (channelCallbacks != null) {
                SendCallbackBasedOnType<T> (jsonPluggableLibrary, channelCallbacks, itemMessage, callbackType);
            } 
            #if (ENABLE_PUBNUB_LOGGING)
            else {
                
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, SendCallbacks3: channelCallbacks null", DateTime.Now.ToString ()), LoggingMethod.LevelInfo);
            }
            #endif
        }

        internal static void FireErrorCallbacksForAllChannels<T> (WebException webEx, RequestState<T> requestState, 
            PubnubErrorSeverity severity, PubnubErrorFilter.Level errorLevel){

            foreach (ChannelEntity channelEntity in requestState.ChannelEntities) {
                PubnubClientError error = Helpers.CreatePubnubClientError<T> (webEx, requestState, channelEntity.ChannelID.ChannelOrChannelGroupName, 
                    severity);  
                FireErrorCallback<T> (channelEntity,
                     errorLevel, error);
                
            }
        }

        internal static void FireErrorCallbacksForAllChannels<T> (Exception ex, RequestState<T> requestState, 
            PubnubErrorSeverity severity, PubnubErrorCode errorType, 
            PubnubErrorFilter.Level errorLevel){

            FireErrorCallbacksForAllChannelsCommon<T> (ex, "", requestState, requestState.ChannelEntities, severity,
                errorType, requestState.RespType, errorLevel
            );
        }

        internal static void FireErrorCallbacksForAllChannelsCommon<T>(Exception ex, string message, RequestState<T> requestState,
            List<ChannelEntity> channelEntities,
            PubnubErrorSeverity severity, PubnubErrorCode errorType, 
            ResponseType responseType, PubnubErrorFilter.Level errorLevel)
        {
            if ((channelEntities != null) && (channelEntities.Count > 0)) {
                foreach (ChannelEntity channelEntity in channelEntities) {
                    string channel = "";
                    string channelGroup = "";
                    if (channelEntity.ChannelID.IsChannelGroup) {
                        channelGroup = channelEntity.ChannelID.ChannelOrChannelGroupName;
                    } else {
                        channel = channelEntity.ChannelID.ChannelOrChannelGroupName;
                    }

                    PubnubClientError error = null;
                    if (ex != null) {
                        error = Helpers.CreatePubnubClientError<T> (ex, requestState, errorType,
                            severity, channel, channelGroup);  
                    } else {
                        error = Helpers.CreatePubnubClientError<T> (message, requestState, errorType,
                            severity, channel, channelGroup);  
                    }

                    FireErrorCallback<T> (channelEntity,
                        errorLevel, error);
                }
            }

            else {
                if ((requestState != null) && (requestState.ErrorCallback != null)) {
                    PubnubClientError error = null;
                    if (ex != null) {
                        error = Helpers.CreatePubnubClientError<T> (ex, requestState, errorType,
                            severity, "", "");  
                    } else {
                        error = Helpers.CreatePubnubClientError<T> (message, requestState, errorType,
                            severity, "", "");  
                    }

                    GoToCallback (error, requestState.ErrorCallback, errorLevel);
                }
                #if (ENABLE_PUBNUB_LOGGING)
                else {
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, FireErrorCallbacksForAllChannelsCommon: {1}",
                        DateTime.Now.ToString (),
                        (requestState!=null)?"ErrorCallback null":"requestState null"
                    ), LoggingMethod.LevelInfo);

                }
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, FireErrorCallbacksForAllChannelsCommon: channelEntities count: {1}",
                    DateTime.Now.ToString (),
                    (channelEntities!=null)?channelEntities.Count.ToString():"channelEntities null"
                ), LoggingMethod.LevelInfo);
                #endif
            }


        }

        internal static void FireErrorCallbacksForAllChannels<T> (string message, RequestState<T> requestState,
            PubnubErrorSeverity severity, PubnubErrorCode errorType, 
            PubnubErrorFilter.Level errorLevel){

            FireErrorCallbacksForAllChannelsCommon<T> (null, message, requestState, requestState.ChannelEntities, severity,
                errorType, requestState.RespType, errorLevel
            );

        }

        internal static void FireErrorCallback<T> (ChannelEntity channelEntity, PubnubErrorFilter.Level errorLevel, 
            PubnubClientError error){

            PubnubChannelCallback<T> channelCallback = channelEntity.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            if (channelCallback != null) {
                GoToCallback (error, channelCallback.ErrorCallback, errorLevel);
            }

        }

        internal static PubnubChannelCallback<T> GetPubnubChannelCallback<T>(Action<T> userCallback, Action<T> connectCallback, 
            Action<PubnubClientError> errorCallback, Action<T> disconnectCallback, Action<T> wildcardPresenceCallback
        ){
            PubnubChannelCallback<T> pubnubChannelCallbacks = new PubnubChannelCallback<T> ();
            pubnubChannelCallbacks.SuccessCallback = userCallback;
            pubnubChannelCallbacks.ConnectCallback = connectCallback;
            pubnubChannelCallbacks.ErrorCallback = errorCallback;
            pubnubChannelCallbacks.DisconnectCallback = disconnectCallback;
            pubnubChannelCallbacks.WildcardPresenceCallback = wildcardPresenceCallback;
            return pubnubChannelCallbacks;
        }

        #region "Error Callbacks"

        internal static void CallErrorCallback<T>(Exception ex, 
            List<ChannelEntity> channelEntities, PubnubErrorCode errorCode, PubnubErrorSeverity severity,
            PubnubErrorFilter.Level errorLevel){

            PubnubClientError clientError = Helpers.CreatePubnubClientError<T> (ex, null, channelEntities, errorCode, severity);
            CallCallbackForEachChannelEntity<T> (channelEntities, clientError, errorLevel);
        }

        internal static void CallErrorCallback<T>(string message, 
            Action<PubnubClientError> errorCallback, PubnubErrorCode errorCode, PubnubErrorSeverity severity,
            PubnubErrorFilter.Level errorLevel){

            //request state can be null

            PubnubClientError clientError = Helpers.CreatePubnubClientError<T> (message, null, null,
                                                errorCode, severity);

            GoToCallback (clientError, errorCallback, errorLevel);
        }

        internal static void CallErrorCallback<T>(ChannelEntity channelEntity, string message,
            PubnubErrorCode errorCode, PubnubErrorSeverity severity,
            PubnubErrorFilter.Level errorLevel){

            List<ChannelEntity> channelEntities = new List<ChannelEntity> ();
            channelEntities.Add (channelEntity);
            PubnubClientError clientError = Helpers.CreatePubnubClientError<T> (message, null, channelEntities,
                errorCode, severity);

            PubnubChannelCallback<T> channelCallback = channelEntity.ChannelParams.Callbacks as PubnubChannelCallback<T>;
            if (channelCallback != null) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CallErrorCallback calling GoToCallback: clientError={1} ", DateTime.Now.ToString (), clientError), LoggingMethod.LevelInfo);
                #endif

                GoToCallback (clientError, channelCallback.ErrorCallback, errorLevel);
            }

        }

        internal static void CallCallbackForEachChannelEntity<T> (List<ChannelEntity> channelEntities, PubnubClientError clientError, 
            PubnubErrorFilter.Level errorLevel){
            foreach (ChannelEntity ce in channelEntities) {
                PubnubChannelCallback<T> channelCallback = ce.ChannelParams.Callbacks as PubnubChannelCallback<T>;
                if (channelCallback != null) {
                    GoToCallback (clientError, channelCallback.ErrorCallback, errorLevel);
                }
            }
        }

        internal static void CallErrorCallback<T>(Action<PubnubClientError> errorCallback, List<ChannelEntity> channelEntities,
            PubnubClientError clientError, PubnubErrorFilter.Level errorLevel)
        {
            if (channelEntities != null) {
                CallCallbackForEachChannelEntity<T> (channelEntities, clientError, errorLevel);
            } else if (errorCallback != null) {
                GoToCallback (clientError, errorCallback, errorLevel);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, CallErrorCallback errorCallback null, clientError = {1} ", 
                    DateTime.Now.ToString (), clientError.ToString ()), LoggingMethod.LevelWarning);        
                #endif
            }
        }

        private static void JsonResponseToCallback<T> (List<object> result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            string callbackJson = "";

            if (typeof(T) == typeof(string)) {
                callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);

                Action<string> castCallback = callback as Action<string>;
                castCallback (callbackJson);
            }
        }

        private static void JsonResponseToCallback<T> (object result, Action<T> callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            string callbackJson = "";

            if (typeof(T) == typeof(string)) {
                try {
                    callbackJson = jsonPluggableLibrary.SerializeToJsonString (result);
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, after _jsonPluggableLibrary.SerializeToJsonString {1}", DateTime.Now.ToString (), callbackJson), LoggingMethod.LevelInfo);
                    #endif
                    Action<string> castCallback = callback as Action<string>;
                    castCallback (callbackJson);
                } catch (Exception ex) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, JsonResponseToCallback = {1} ", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);        
                    #endif
                }
            }
        }

        internal static void GoToCallback<T> (object result, Action<T> Callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (Callback != null) {
                if (typeof(T) == typeof(string)) {
                    JsonResponseToCallback (result, Callback, jsonPluggableLibrary);
                } else {
                    Callback ((T)(object)result);
                }
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0} Callback null={1}", DateTime.Now.ToString (), result.ToString()), LoggingMethod.LevelInfo);
                #endif
            }
        }

        internal static void GoToCallback (object result, Action<string> Callback, IJsonPluggableLibrary jsonPluggableLibrary)
        {
            if (Callback != null) {
                JsonResponseToCallback (result, Callback, jsonPluggableLibrary);
            }
        }

        internal static void GoToCallback (object result, Action<object> Callback)
        {
            if (Callback != null) {
                Callback (result);
            }
        }

        internal static void GoToCallback (PubnubClientError error, Action<PubnubClientError> Callback, PubnubErrorFilter.Level errorLevel)
        {
            if (Callback != null && error != null) {
                if ((int)error.Severity <= (int)errorLevel) { //Checks whether the error serverity falls in the range of error filter level
                    //Do not send 107 = PubnubObjectDisposedException
                    //Do not send 105 = WebRequestCancelled
                    //Do not send 130 = PubnubClientMachineSleep
                    if (error.StatusCode != 107
                        && error.StatusCode != 105
                        && error.StatusCode != 130
                        && error.StatusCode != 4040) { //Error Code that should not go out
                        Callback (error);
                    }
                }
            }
        }


        #endregion
    }
}

