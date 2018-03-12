#if((!USE_JSONFX_UNITY_IOS) && (!USE_MiniJSON))
#define USE_JSONFX_UNITY_IOS
#endif

using MiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PubNubMessaging.Core
{
    #region "Json Pluggable Library"
    public interface IJsonPluggableLibrary
    {
        bool IsArrayCompatible (string jsonString);

        bool IsDictionaryCompatible (string jsonString);

        string SerializeToJsonString (object objectToSerialize);

        List<object> DeserializeToListOfObject (string jsonString);

        object DeserializeToObject (string jsonString);

        Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString);
    }

    public static class JSONSerializer{
        private static IJsonPluggableLibrary jsonPluggableLibrary = null;
        public static IJsonPluggableLibrary JsonPluggableLibrary{
            get {
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog("JSON LIB: USE_MiniJSON", LoggingMethod.LevelInfo);
                #endif
                jsonPluggableLibrary = new MiniJSONObjectSerializer();
                #if (ENABLE_PUBNUB_LOGGING)
                LoggingMethod.WriteToLog ("JSON LIB: USE_JSONFX_UNITY_IOS", LoggingMethod.LevelInfo);
                #endif
                return jsonPluggableLibrary;
            }
        }

    }

    
    public class MiniJSONObjectSerializer : IJsonPluggableLibrary
    {
        public bool IsArrayCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("[");
        }

        public bool IsDictionaryCompatible (string jsonString)
        {
            return jsonString.Trim().StartsWith("{");
        }

        public string SerializeToJsonString (object objectToSerialize)
        {
            string json = Json.Serialize (objectToSerialize); 
            return PubnubCryptoBase.ConvertHexToUnicodeChars (json);
        }

        public List<object> DeserializeToListOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as List<object>;
        }

        public object DeserializeToObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as object;
        }

        public Dictionary<string, object> DeserializeToDictionaryOfObject (string jsonString)
        {
            return Json.Deserialize (jsonString) as Dictionary<string, object>;
        }
    }

	#endregion
}