using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace PubNubMessaging.Core
{
    #region "Push Notification Types"
    public enum PushTypeService
    {
        None,
        MPNS, //MicrosoftPushNotificationService
        WNS, //WindowsNotificationService,
        GCM,
        APNS
    }

    #endregion
    internal static class Utility
    {

        internal const string PresenceChannelSuffix = "-pnpres";
        internal const int iOSRequestTimeout = 59;

        #if(UNITY_IOS)
        internal static int CheckTimeoutValue(int value){
            if (value > iOSRequestTimeout) {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog (string.Format("Forcing timeout value to {0} as iOS force closes the www request after {0} secs", iOSRequestTimeout), LoggingMethod.LevelInfo);
                #endif
                
                return iOSRequestTimeout;
            } else {
                return value;
            }
        }
        #endif    

        internal static long CheckKeyAndParseLong(IDictionary dict, string what, string key){
            long sequenceNumber = 0; 
            if (dict.Contains (key)) {
                long seqNumber;
                if (!Int64.TryParse (dict [key].ToString(), out seqNumber)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1}, {2} conversion failed: {3}.", 
                        DateTime.Now.ToString (), what, key, dict [key].ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                sequenceNumber = seqNumber;
            }
            return sequenceNumber;
        }

        internal static int CheckKeyAndParseInt(IDictionary dict, string what, string key){
            int sequenceNumber = 0; 
            if (dict.Contains (key)) {
                int seqNumber;
                if (!int.TryParse (dict [key].ToString(), out seqNumber)) {
                    #if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog (string.Format ("DateTime {0}, {1}, {2} conversion failed: {3}.", 
                        DateTime.Now.ToString (), what, key, dict [key].ToString ()), LoggingMethod.LevelInfo);
                    #endif
                }
                sequenceNumber = seqNumber;
            }
            return sequenceNumber;
        }

        internal static long ValidateTimetoken(string timetoken, bool raiseError){
            if(!string.IsNullOrEmpty(timetoken)){
                long r;
                if (long.TryParse (timetoken, out r)) {
                    return r;
                } else if (raiseError) {
                    throw new ArgumentException ("Invalid timetoken");
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }

        internal static string CheckChannelGroup(string channelGroup, bool convertToPresence){
            string[] multiChannelGroups = channelGroup.Split(',');
            if (multiChannelGroups.Length > 0) {
                for (int index = 0; index < multiChannelGroups.Length; index++) {
                    if (!string.IsNullOrEmpty (multiChannelGroups [index]) && multiChannelGroups [index].Trim ().Length > 0) {
                        if (convertToPresence) {
                            multiChannelGroups [index] = string.Format ("{0}{1}", multiChannelGroups [index], Utility.PresenceChannelSuffix);
                        } 
                    } else {
                        throw new MissingMemberException (string.Format("Invalid channel group '{0}'", multiChannelGroups [index]));
                    }
                }
            } else {
                throw new ArgumentException(string.Format("Channel Group is null"));
            }
            return string.Join(",", multiChannelGroups);
        }

        internal static List<string> CheckAndAddNameSpace(string nameSpace){
            List<string> url = new List<string>();
            if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Trim().Length > 0)
            {
                url.Add("namespace");
                url.Add(nameSpace);
                return url;
            }
            return null;
        }

        internal static void CheckPushType(PushTypeService pushType)
        {
            if (pushType == PushTypeService.None)
            {
                throw new ArgumentException("Missing PushTypeService");
            }
        }

        internal static void CheckChannelOrChannelGroup(string channel, string channelGroup){
            if ((string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(channel.Trim())) 
                && (string.IsNullOrEmpty(channelGroup) || string.IsNullOrEmpty(channelGroup.Trim())))
            {
                throw new ArgumentException("Both Channel and ChannelGroup are empty.");
            }
        }

        internal static void CheckChannels(string[] channels)
        {
            if (channels == null || channels.Length == 0)
            {
                throw new ArgumentException("Missing channel(s)");
            }
        }

        internal static void CheckChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(channel.Trim()))
            {
                throw new ArgumentException("Missing Channel");
            }
        }

        internal static void CheckMessage(object message)
        {
            if (message == null)
            {
                throw new ArgumentException("Message is null");
            }
        }

        internal static void CheckString(string message, string what)
        {
            if (message == null)
            {
                throw new ArgumentException(string.Format("{0} is null", what));
            }
        }

        internal static void CheckPublishKey(string publishKey)
        {
            if (string.IsNullOrEmpty(publishKey) || string.IsNullOrEmpty(publishKey.Trim()) || publishKey.Length <= 0)
            {
                throw new MissingMemberException("Invalid publish key");
            }
        }

        internal static void CheckCallback<T>(Action<T> callback, CallbackType callbackType)
        {
            if (callback == null)
            {
                throw new ArgumentException(string.Format("Missing {0} Callback", callbackType.ToString()));
            }
        }

        internal static void CheckJSONPluggableLibrary()
        {
            if (PubnubUnity.JsonPluggableLibrary == null)
            {
                throw new NullReferenceException("Missing Json Pluggable Library for Pubnub Instance");
            }
        }

        internal static void CheckUserState(string jsonUserState)
        {
            if (string.IsNullOrEmpty(jsonUserState) || string.IsNullOrEmpty(jsonUserState.Trim()))
            {
                throw new ArgumentException("Missing User State");
            }
        }

        internal static void CheckSecretKey(string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(secretKey.Trim()) || secretKey.Length <= 0)
            {
                throw new MissingMemberException("Invalid secret key");
            }
        }

        internal static Guid GenerateGuid ()
        {
            return Guid.NewGuid ();
        }

        internal static bool CheckRequestTimeoutMessageInError<T>(CustomEventArgs<T> cea){
            if (cea.IsError && cea.Message.ToString().Contains ("The request timed out.")) {
                return true;
            } else {
                return false;
            }
        }

        internal static bool IsPresenceChannel (string channel)
        {
            if (channel.LastIndexOf (PresenceChannelSuffix) > 0) {
                return true;
            } else {
                return false;
            }
        }

        internal static bool IsUnsafe (char ch, bool ignoreComma)
        {
            if (ignoreComma) {
                return " ~`!@#$%^&*()+=[]\\{}|;':\"/<>?".IndexOf (ch) >= 0;
            } else {
                return " ~`!@#$%^&*()+=[]\\{}|;':\",/<>?".IndexOf (ch) >= 0;
            }
        }

        private static char ToHex (int ch)
        {
            return (char)(ch < 10 ? '0' + ch : 'A' + ch - 10);
        }

        public static string EncodeUricomponent (string s, ResponseType type, bool ignoreComma, bool ignorePercent2fEncode)
        {
            string encodedUri = "";
            StringBuilder o = new StringBuilder ();
            foreach (char ch in s) {
                if (IsUnsafe (ch, ignoreComma)) {
                    o.Append ('%');
                    o.Append (ToHex (ch / 16));
                    o.Append (ToHex (ch % 16));
                } else {
                    if (ch == ',' && ignoreComma) {
                        o.Append (ch.ToString ());
                    } else if (Char.IsSurrogate (ch)) {
                        o.Append (ch);
                    } else {
                        string escapeChar = System.Uri.EscapeDataString (ch.ToString ());
                        o.Append (escapeChar);
                    }
                }
            }
            encodedUri = o.ToString ();
            if (type == ResponseType.HereNow || type == ResponseType.DetailedHistory || type == ResponseType.Leave || type == ResponseType.PresenceHeartbeat
                || type == ResponseType.PushRegister || type == ResponseType.PushRemove || type == ResponseType.PushGet || type == ResponseType.PushUnregister
            ) {
                if (!ignorePercent2fEncode) {
                    encodedUri = encodedUri.Replace ("%2F", "%252F");
                }
            }

            return encodedUri;
        }

        public static string Md5 (string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider ();
            byte[] data = Encoding.Unicode.GetBytes (text);
            byte[] hash = md5.ComputeHash (data);
            string hexaHash = "";
            foreach (byte b in hash)
                hexaHash += String.Format ("{0:x2}", b);
            return hexaHash;
        }

        public static long TranslateDateTimeToSeconds (DateTime dotNetUTCDateTime)
        {
            TimeSpan timeSpan = dotNetUTCDateTime - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timeStamp = Convert.ToInt64 (timeSpan.TotalSeconds);
            return timeStamp;
        }

        /// <summary>
        /// Convert the UTC/GMT DateTime to Unix Nano Seconds format
        /// </summary>
        /// <param name="dotNetUTCDateTime"></param>
        /// <returns></returns>
        public static long TranslateDateTimeToPubnubUnixNanoSeconds (DateTime dotNetUTCDateTime)
        {
            TimeSpan timeSpan = dotNetUTCDateTime - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timeStamp = Convert.ToInt64 (timeSpan.TotalSeconds) * 10000000;
            return timeStamp;
        }

        /// <summary>
        /// Convert the Unix Nano Seconds format time to UTC/GMT DateTime
        /// </summary>
        /// <param name="unixNanoSecondTime"></param>
        /// <returns></returns>
        public static DateTime TranslatePubnubUnixNanoSecondsToDateTime (long unixNanoSecondTime)
        {
            double timeStamp = unixNanoSecondTime / 10000000;
            DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds (timeStamp);
            return dateTime;
        }

        internal static List<string> CheckKeyAndConvertObjToStringArr(object obj){
            if (obj == null) {
                return null;
            }
            List<string> lstArr = ((IEnumerable)obj).Cast<string> ().ToList ();
            #if (ENABLE_PUBNUB_LOGGING)
            foreach (string lst in lstArr){
                UnityEngine.Debug.Log ("clientlist:" + lst);
            }
            #endif
            return lstArr;
        }

    }
}

