using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PubNubMessaging.Core
{
    internal class MultiplexExceptionEventArgs<T> : EventArgs
    {
        internal List<ChannelEntity> channelEntities;
        internal bool reconnectMaxTried;
        internal bool resumeOnReconnect;
        internal ResponseType responseType;
    }

    public class ExceptionHandlers
    {
        private static EventHandler<EventArgs> multiplexException;
        public static event EventHandler<EventArgs> MultiplexException {
            add {
                if (multiplexException == null || !multiplexException.GetInvocationList ().Contains (value)) {
                    multiplexException += value;
                }
            }
            remove {
                multiplexException -= value;
            }
        }

        internal static void CreateErrorCodeAndCallErrorCallback<T> (int statusCode, CustomEventArgs<T> cea, RequestState<T> requestState,
            PubnubErrorFilter.Level errorLevel){
            PubnubClientError error = new PubnubClientError (statusCode, PubnubErrorSeverity.Critical, cea.Message, PubnubMessageSource.Server, 
                requestState.Request, requestState.Response, cea.Message, requestState.ChannelEntities);
            PubnubCallbacks.CallErrorCallback<T> (requestState.ErrorCallback, requestState.ChannelEntities,
                error, errorLevel);
            
        }

        internal static void ResponseCallbackErrorOrTimeoutHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState,
            PubnubErrorFilter.Level errorLevel){

            WebException webEx = new WebException (cea.Message);

            if ((cea.Message.Contains ("NameResolutionFailure")
                || cea.Message.Contains ("ConnectFailure")
                || cea.Message.Contains ("ServerProtocolViolation")
                || cea.Message.Contains ("ProtocolError")
            )) {
                webEx = new WebException ("Network connnect error", WebExceptionStatus.ConnectFailure);

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (cea.Message, requestState, 
                    PubnubErrorSeverity.Warn, PubnubErrorCode.NoInternetRetryConnect, errorLevel);

            } else if (cea.IsTimeout || Utility.CheckRequestTimeoutMessageInError(cea)) {
            } else if ((cea.Message.Contains ("403")) 
                || (cea.Message.Contains ("java.io.FileNotFoundException")) 
                || ((PubnubUnity.Version.Contains("UnityWeb")) && (cea.Message.Contains ("Failed downloading")))) {
                CreateErrorCodeAndCallErrorCallback<T> (403, cea, requestState, errorLevel);

            } else if (cea.Message.Contains ("500")) {
                CreateErrorCodeAndCallErrorCallback<T> (500, cea, requestState, errorLevel);
                
            } else if (cea.Message.Contains ("502")) {
                CreateErrorCodeAndCallErrorCallback<T> (502, cea, requestState, errorLevel);                
            } else if (cea.Message.Contains ("503")) {
                CreateErrorCodeAndCallErrorCallback<T> (503, cea, requestState, errorLevel);                
            } else if (cea.Message.Contains ("504")) {
                CreateErrorCodeAndCallErrorCallback<T> (504, cea, requestState, errorLevel);
            } else if (cea.Message.Contains ("414")) {
                CreateErrorCodeAndCallErrorCallback<T> (414, cea, requestState, errorLevel);                
            } else if (cea.Message.Contains ("451")) {
                CreateErrorCodeAndCallErrorCallback<T> (451, cea, requestState, errorLevel);                
            } else if (cea.Message.Contains ("481")) {
                CreateErrorCodeAndCallErrorCallback<T> (481, cea, requestState, errorLevel);                
            } else {
                CreateErrorCodeAndCallErrorCallback<T> (400, cea, requestState, errorLevel);    
            }
            ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, errorLevel);
        }

        internal static void ResponseCallbackWebExceptionHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, 
            WebException webEx,
            PubnubErrorFilter.Level errorLevel){
            if ((requestState!=null) && (requestState.ChannelEntities != null || requestState.RespType != ResponseType.Time)) {

                if(requestState.RespType.Equals(ResponseType.SubscribeV2)
                    || requestState.RespType.Equals(ResponseType.PresenceV2)
                ) {

                    if (webEx.Message.IndexOf ("The request was aborted: The request was canceled") == -1
                        || webEx.Message.IndexOf ("Machine suspend mode enabled. No request will be processed.") == -1) {

                        PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (webEx, requestState, 
                            PubnubErrorSeverity.Warn, errorLevel);
                    }
                } else {
                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (webEx, requestState,
                        PubnubErrorSeverity.Warn, errorLevel);
                }
            }
            ProcessResponseCallbackWebExceptionHandler<T> (webEx, requestState, errorLevel);
        }

        internal static void ResponseCallbackExceptionHandler<T> (CustomEventArgs<T> cea, RequestState<T> requestState, 
            Exception ex, PubnubErrorFilter.Level errorLevel){

            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, Process Response Exception: = {1}", DateTime.Now.ToString (), ex.ToString ()), LoggingMethod.LevelError);
            #endif
            if (requestState.ChannelEntities != null) {

                if(requestState.RespType.Equals(ResponseType.SubscribeV2)
                    || requestState.RespType.Equals(ResponseType.PresenceV2)
                ) {

                    PubnubCallbacks.FireErrorCallbacksForAllChannels (ex, requestState, 
                        PubnubErrorSeverity.Warn, PubnubErrorCode.None, errorLevel);
                } else {

                    PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (ex, requestState, 
                        PubnubErrorSeverity.Critical, PubnubErrorCode.None, errorLevel);
                }
            }
            ProcessResponseCallbackExceptionHandler<T> (ex, requestState, errorLevel);
        }

        internal static void ProcessResponseCallbackExceptionHandler<T> (Exception ex, RequestState<T> asynchRequestState, 
            PubnubErrorFilter.Level errorLevel)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponseCallbackExceptionHandler Exception= {1} for URL: {2}", 
                DateTime.Now.ToString (), ex.ToString (), 
                (asynchRequestState.Request!=null)?asynchRequestState.Request.RequestUri.ToString (): "asynchRequestState.Request null"),
                LoggingMethod.LevelInfo);
            #endif
            UrlRequestCommonExceptionHandler<T> (ex.Message, asynchRequestState, asynchRequestState.Timeout, 
                false, errorLevel);
        }

        internal static void ProcessResponseCallbackWebExceptionHandler<T> (WebException webEx, RequestState<T> asynchRequestState, 
             PubnubErrorFilter.Level errorLevel)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            if (webEx.ToString ().Contains ("Aborted")) {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponseCallbackWebExceptionHandler WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelInfo);
            } else {
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, ProcessResponseCallbackWebExceptionHandler WebException: {1}", DateTime.Now.ToString (), webEx.ToString ()), LoggingMethod.LevelError);
            }
            #endif

            UrlRequestCommonExceptionHandler<T> (webEx.Message, asynchRequestState, asynchRequestState.Timeout,
                false, errorLevel);
        }

        static void FireMultiplexException<T>(bool resumeOnReconnect, RequestState<T> requestState)
        {
            #if (ENABLE_PUBNUB_LOGGING)
            LoggingMethod.WriteToLog(string.Format("DateTime {0}, UrlRequestCommonExceptionHandler for Subscribe/Presence", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
            #endif
            MultiplexExceptionEventArgs<T> mea = new MultiplexExceptionEventArgs<T>();
            mea.channelEntities = requestState.ChannelEntities;
            mea.resumeOnReconnect = resumeOnReconnect;
            mea.reconnectMaxTried = false;
            mea.responseType = requestState.RespType;

            multiplexException.Raise(typeof(ExceptionHandlers), mea);
        }

        internal static void UrlRequestCommonExceptionHandler<T> (string message, RequestState<T> requestState,
            bool requestTimeout, bool resumeOnReconnect, PubnubErrorFilter.Level errorLevel)
        {
            switch (requestState.RespType)
            {
                case ResponseType.PresenceV2:
                case ResponseType.SubscribeV2:
                    FireMultiplexException<T>(resumeOnReconnect, requestState);
                    break;
                case ResponseType.GlobalHereNow:
                case ResponseType.Time:
                    CommonExceptionHandler<T>(requestState, message, requestTimeout, errorLevel);
                    break;
                case ResponseType.Leave:
                case ResponseType.PresenceHeartbeat:
                    break;
                case ResponseType.PushGet:
                case ResponseType.PushRegister:
                case ResponseType.PushRemove:
                case ResponseType.PushUnregister:
                    PushNotificationExceptionHandler<T>(requestState, requestTimeout, errorLevel);
                    break;
                case ResponseType.ChannelGroupAdd:
                case ResponseType.ChannelGroupRemove:
                case ResponseType.ChannelGroupGet:
                case ResponseType.ChannelGroupGrantAccess:
                case ResponseType.ChannelGroupAuditAccess:
                case ResponseType.ChannelGroupRevokeAccess:
                    ChannelGroupExceptionHandler<T>(requestState, requestTimeout, errorLevel);
                    break;
                default:
                    CommonExceptionHandler<T> (requestState, message, requestTimeout, errorLevel);
                    break;
                    
            }
        }

        internal static void PushNotificationExceptionHandler<T>(RequestState<T> requestState, bool requestTimeout, 
            PubnubErrorFilter.Level errorLevel )
        {
            if (requestTimeout)
            {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, PushExceptionHandler response={1}", DateTime.Now.ToString(), message), LoggingMethod.LevelInfo);
                #endif
                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (message, requestState, 
                    PubnubErrorSeverity.Critical, PubnubErrorCode.PushNotificationTimeout, errorLevel);
            }
        }

        internal static void ChannelGroupExceptionHandler<T>(RequestState<T> requestState, bool requestTimeout, 
            PubnubErrorFilter.Level errorLevel)
        {
            if (requestTimeout)
            {
                string message = (requestTimeout) ? "Operation Timeout" : "Network connnect error";

                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, ChannelGroupExceptionHandler response={1}, {2}", 
                    DateTime.Now.ToString(), message, Helpers.GetNamesFromChannelEntities(requestState.ChannelEntities)), LoggingMethod.LevelInfo);
                #endif
                PubnubCallbacks.FireErrorCallbacksForAllChannels<T>(message, requestState, 
                    PubnubErrorSeverity.Critical, PubnubErrorCode.ChannelGroupTimeout, errorLevel);
            }
        }

        internal static void CommonExceptionHandler<T> (RequestState<T> requestState, string message, bool requestTimeout, 
            PubnubErrorFilter.Level errorLevel
        )
        {
            if (requestTimeout) {
                message = "Operation Timeout";
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} CommonExceptionHandler response={2}", DateTime.Now.ToString (), 
                    requestState.RespType.ToString (), message), LoggingMethod.LevelInfo);
                #endif

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (message, requestState, 
                    PubnubErrorSeverity.Critical, Helpers.GetTimeOutErrorCode (requestState.RespType), errorLevel);
            } else {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1} CommonExceptionHandler response={2}", DateTime.Now.ToString (), requestState.RespType.ToString (), message), LoggingMethod.LevelInfo);
                #endif

                PubnubCallbacks.FireErrorCallbacksForAllChannels<T> (message, requestState, 
                    PubnubErrorSeverity.Critical, PubnubErrorCode.None, errorLevel);
            }
        }
    }
}
