using System;
using System.Text;
using System.Collections.Generic;

namespace PubNubMessaging.Core
{
	public class BuildRequests
	{

		#region "Build Request State"

		internal static RequestState<T> BuildRequestState<T>(List<ChannelEntity> channelEntities, ResponseType responseType, 
			bool reconnect, long id, bool timeout, long timetoken, Type typeParam, string uuid,
			Action<T> userCallback, Action<PubnubClientError> errorCallback
		){
			RequestState<T> requestState = new RequestState<T> ();
			requestState.ChannelEntities = channelEntities;
			requestState.RespType = responseType;
			requestState.Reconnect = reconnect;
			requestState.SuccessCallback = userCallback;
			requestState.ErrorCallback = errorCallback;
			requestState.ID = id;
			requestState.Timeout = timeout;
			requestState.Timetoken = timetoken;
			requestState.TypeParameterType = typeParam;
			requestState.UUID = uuid;
			return requestState;
		}

		internal static RequestState<T> BuildRequestState<T>(List<ChannelEntity> channelEntities, ResponseType responseType, 
			bool reconnect, long id, bool timeout, long timetoken, Type typeParam
		){
			return BuildRequestState<T> (channelEntities, responseType, reconnect, id, timeout, timetoken,
				typeParam, String.Empty, null, null);
		}

		internal static RequestState<T> BuildRequestState<T>(Action<T> userCallback, Action<PubnubClientError> errorCallback, ResponseType responseType, 
			bool reconnect, long id, bool timeout, long timetoken, Type typeParam, string uuid
		){
			return BuildRequestState<T> (null, responseType, reconnect, id, timeout, timetoken,
				typeParam, uuid, userCallback, errorCallback);
		}

		#endregion
		#region "Build Requests"
		internal static Uri BuildRegisterDevicePushRequest(string channel, PushTypeService pushType, 
			string pushToken,  string uuid, 
			bool ssl, string origin, string authenticationKey,string subscribeKey)
		{
			StringBuilder parameterBuilder = new StringBuilder();

			parameterBuilder.AppendFormat("?add={0}", Utility.EncodeUricomponent(channel, ResponseType.PushRegister, true, false));
			parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLower());

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("push");
			url.Add("sub-key");
			url.Add(subscribeKey);
			url.Add("devices");
			url.Add(pushToken);

			return BuildRestApiRequest<Uri>(url, ResponseType.PushRegister, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildRemoveChannelPushRequest(string channel, PushTypeService pushType, 
			string pushToken,  string uuid, 
			bool ssl, string origin, string authenticationKey,string subscribeKey)
		{
			StringBuilder parameterBuilder = new StringBuilder();

			parameterBuilder.AppendFormat("?remove={0}", Utility.EncodeUricomponent(channel, ResponseType.PushRemove, true, false));
			parameterBuilder.AppendFormat("&type={0}", pushType.ToString().ToLower());

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("push");
			url.Add("sub-key");
			url.Add(subscribeKey);
			url.Add("devices");
			url.Add(pushToken);

			return BuildRestApiRequest<Uri>(url, ResponseType.PushRemove, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildGetChannelsPushRequest(PushTypeService pushType, string pushToken, string uuid, 
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			StringBuilder parameterBuilder = new StringBuilder();

			parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLower());

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("push");
			url.Add("sub-key");
			url.Add(subscribeKey);
			url.Add("devices");
			url.Add(pushToken);

			return BuildRestApiRequest<Uri>(url, ResponseType.PushGet, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildUnregisterDevicePushRequest(PushTypeService pushType, string pushToken, string uuid, 
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			StringBuilder parameterBuilder = new StringBuilder();

			parameterBuilder.AppendFormat("?type={0}", pushType.ToString().ToLower());

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("push");
			url.Add("sub-key");
			url.Add(subscribeKey);
			url.Add("devices");
			url.Add (pushToken);
			url.Add("remove");

			return BuildRestApiRequest<Uri>(url, ResponseType.PushUnregister, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildPublishRequest (string channel, string message, bool storeInHistory, string uuid, 
			bool ssl, string origin, string authenticationKey, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey, 
			string metadata, uint messageCounter, int ttl
		)
		{
			StringBuilder parameterBuilder = new StringBuilder ();
			parameterBuilder.AppendFormat ("&seqn={0}", messageCounter.ToString ());
			parameterBuilder.Append ((storeInHistory) ? "" : "&store=0");
			if (ttl >= 0) {
				parameterBuilder.AppendFormat ("&ttl={0}", ttl.ToString());
			}

			if (!string.IsNullOrEmpty (metadata) || metadata.Equals("\"\"")) {
				parameterBuilder.AppendFormat ("&meta={0}", Utility.EncodeUricomponent (metadata, ResponseType.Publish, false, false));
			}

			// Generate String to Sign
			string signature = "0";
			if (secretKey.Length > 0) {
				StringBuilder stringToSign = new StringBuilder ();
				stringToSign
					.Append (publishKey)
					.Append ('/')
					.Append (subscribeKey)
					.Append ('/')
					.Append (secretKey)
					.Append ('/')
					.Append (channel)
					.Append ('/')
					.Append (message); // 1

				// Sign Message
				signature = Utility.Md5 (stringToSign.ToString ());
			}

			// Build URL
			List<string> url = new List<string> ();
			url.Add ("publish");
			url.Add (publishKey);
			url.Add (subscribeKey);
			url.Add (signature);
			url.Add (channel);
			url.Add ("0");
			url.Add (message);

			return BuildRestApiRequest<Uri> (url, ResponseType.Publish, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString ());
		}

		internal static Uri BuildDetailedHistoryRequest (string channel, long start, long end, 
			int count, bool reverse, bool includeToken, string uuid, bool ssl, string origin, 
			string authenticationKey, string subscribeKey)
		{
			StringBuilder parameterBuilder = new StringBuilder ();
			if (count <= -1)
				count = 100;

			parameterBuilder.AppendFormat ("?count={0}", count);
			if (includeToken) {
				parameterBuilder.AppendFormat ("&include_token={0}", includeToken.ToString ().ToLower ());
			}
			if (reverse) {
				parameterBuilder.AppendFormat ("&reverse={0}", reverse.ToString ().ToLower ());
			}
			if (start != -1) {
				parameterBuilder.AppendFormat ("&start={0}", start.ToString ().ToLower ());
			}
			if (end != -1) {
				parameterBuilder.AppendFormat ("&end={0}", end.ToString ().ToLower ());
			}
			if (!string.IsNullOrEmpty (authenticationKey)) {
				parameterBuilder.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.DetailedHistory, false, false));
			}

			parameterBuilder.AppendFormat ("&uuid={0}", Utility.EncodeUricomponent (uuid, ResponseType.DetailedHistory, false, false));
			parameterBuilder.AppendFormat ("&pnsdk={0}", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.DetailedHistory, false, true));

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("history");
			url.Add ("sub-key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (channel);

			return BuildRestApiRequest<Uri> (url, ResponseType.DetailedHistory, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildHereNowRequest (string channel, string channelGroups, bool showUUIDList, bool includeUserState, string uuid, 
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			int disableUUID = (showUUIDList) ? 0 : 1;
			int userState = (includeUserState) ? 1 : 0;
			StringBuilder parameterBuilder = new StringBuilder ();
			parameterBuilder.AppendFormat ("?disable_uuids={0}&state={1}", disableUUID, userState);
			if (!string.IsNullOrEmpty(channelGroups))
			{
				parameterBuilder.AppendFormat("&channel-group={0}",  Utility.EncodeUricomponent(channelGroups, ResponseType.HereNow, true, false));
			}

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.IsNullOrEmpty(channel) ? "," : channel);

			return BuildRestApiRequest<Uri> (url, ResponseType.HereNow, uuid, ssl, origin, 0, authenticationKey, parameterBuilder.ToString());
		}

		internal static Uri BuildGlobalHereNowRequest (bool showUUIDList, bool includeUserState, string uuid, 
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			int disableUUID = (showUUIDList) ? 0 : 1;
			int userState = (includeUserState) ? 1 : 0;
			string parameters = string.Format ("?disable_uuids={0}&state={1}", disableUUID, userState);

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);

			return BuildRestApiRequest<Uri> (url, ResponseType.GlobalHereNow, uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildWhereNowRequest (string uuid, string sessionUUID,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("uuid");
			url.Add (uuid);

			return BuildRestApiRequest<Uri> (url, ResponseType.WhereNow, sessionUUID, ssl, origin, 0, authenticationKey, "");
		}

		internal static Uri BuildTimeRequest (string uuid, bool ssl, string origin)
		{
			List<string> url = new List<string> ();

			url.Add ("time");
			url.Add ("0");

			return BuildRestApiRequest<Uri> (url, ResponseType.Time, uuid, ssl, origin, 0, "", "");
		}

		internal static Uri BuildGrantAccessRequest (string channel, bool read, bool write, int ttl, string uuid, 
			bool ssl, string origin, string authenticationKey, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";
			long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
			string queryString = "";

			StringBuilder queryStringBuilder = new StringBuilder ();
			if (!string.IsNullOrEmpty (authenticationKey)) {
				queryStringBuilder.AppendFormat ("auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.GrantAccess, false, false));
			}

			if (!string.IsNullOrEmpty (channel)) {
				queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (channel, ResponseType.GrantAccess, false, false));
			}

			queryStringBuilder.AppendFormat ("{0}", (queryStringBuilder.Length > 0) ? "&" : "");
			queryStringBuilder.AppendFormat ("pnsdk={0}", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.GrantAccess, false, true));
			queryStringBuilder.AppendFormat ("&r={0}", Convert.ToInt32 (read));
			queryStringBuilder.AppendFormat ("&timestamp={0}", timeStamp.ToString ());
			if (ttl > -1) {
				queryStringBuilder.AppendFormat ("&ttl={0}", ttl.ToString ());
			}
			queryStringBuilder.AppendFormat ("&uuid={0}", Utility.EncodeUricomponent (uuid, ResponseType.GrantAccess, false, false));
			queryStringBuilder.AppendFormat ("&w={0}", Convert.ToInt32 (write));

			if (secretKey.Length > 0) {
				StringBuilder string_to_sign = new StringBuilder ();
				string_to_sign.Append (subscribeKey)
					.Append ("\n")
					.Append (publishKey)
					.Append ("\n")
					.Append ("grant")
					.Append ("\n")
					.Append (queryStringBuilder.ToString ());

				PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKey);
				signature = pubnubCrypto.PubnubAccessManagerSign (secretKey, string_to_sign.ToString ());
				queryString = string.Format ("signature={0}&{1}", signature, queryStringBuilder.ToString ());
			}

			string parameters = "";
			parameters += "?" + queryString;

			List<string> url = new List<string> ();
			url.Add ("v1");
			url.Add ("auth");
			url.Add ("grant");
			url.Add ("sub-key");
			url.Add (subscribeKey);

			return BuildRestApiRequest<Uri> (url, ResponseType.GrantAccess, uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildAuditAccessRequest (string channel, string uuid, 
			bool ssl, string origin, string authenticationKey, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";

			long timeStamp = Utility.TranslateDateTimeToSeconds (DateTime.UtcNow);
			string queryString = "";
			StringBuilder queryStringBuilder = new StringBuilder ();
			if (!string.IsNullOrEmpty (authenticationKey)) {
				queryStringBuilder.AppendFormat ("auth={0}", Utility.EncodeUricomponent (authenticationKey, ResponseType.AuditAccess, false, false));
			}
			if (!string.IsNullOrEmpty (channel)) {
				queryStringBuilder.AppendFormat ("{0}channel={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (channel, ResponseType.AuditAccess, false, false));
			}
			queryStringBuilder.AppendFormat ("{0}pnsdk={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (PubnubUnity.Version, ResponseType.AuditAccess, false, true));
			queryStringBuilder.AppendFormat ("{0}timestamp={1}", (queryStringBuilder.Length > 0) ? "&" : "", timeStamp.ToString ());
			queryStringBuilder.AppendFormat ("{0}uuid={1}", (queryStringBuilder.Length > 0) ? "&" : "", Utility.EncodeUricomponent (uuid, ResponseType.AuditAccess, false, false));

			if (secretKey.Length > 0) {
				StringBuilder string_to_sign = new StringBuilder ();
				string_to_sign.Append (subscribeKey)
					.Append ("\n")
					.Append (publishKey)
					.Append ("\n")
					.Append ("audit")
					.Append ("\n")
					.Append (queryStringBuilder.ToString ());

				PubnubCrypto pubnubCrypto = new PubnubCrypto (cipherKey);
				signature = pubnubCrypto.PubnubAccessManagerSign (secretKey, string_to_sign.ToString ());
				queryString = string.Format ("signature={0}&{1}", signature, queryStringBuilder.ToString ());
			}

			string parameters = "";
			parameters += "?" + queryString;

			List<string> url = new List<string> ();
			url.Add ("v1");
			url.Add ("auth");
			url.Add ("audit");
			url.Add ("sub-key");
			url.Add (subscribeKey);

			return BuildRestApiRequest<Uri> (url, ResponseType.AuditAccess, uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildSetUserStateRequest (string channel, string channelGroup, string jsonUserState, string uuid,  string sessionUUID,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			StringBuilder paramBuilder = new StringBuilder ();
			paramBuilder.AppendFormat ("?state={0}", Utility.EncodeUricomponent (jsonUserState, ResponseType.SetUserState, false, false));
			if (!string.IsNullOrEmpty(channelGroup) && channelGroup.Trim().Length > 0)
			{
				paramBuilder.AppendFormat("&channel-group={0}", Utility.EncodeUricomponent(channelGroup, ResponseType.SetUserState, true, false));
			}

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.IsNullOrEmpty(channel) ? "," : channel);
			url.Add ("uuid");
			url.Add (uuid);
			url.Add ("data");

			return BuildRestApiRequest<Uri> (url, ResponseType.SetUserState, sessionUUID, ssl, origin, 0, authenticationKey, paramBuilder.ToString ());
		}

		internal static Uri BuildGetUserStateRequest (string channel, string channelGroup, string uuid, string sessionUUID,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			string parameters = "";
			if (!string.IsNullOrEmpty(channelGroup) && channelGroup.Trim().Length > 0)
			{
				parameters = string.Format("&channel-group={0}", Utility.EncodeUricomponent(channelGroup, ResponseType.GetUserState, true, false));
			}

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.IsNullOrEmpty(channel) ? "," : channel);
			url.Add ("uuid");
			url.Add (uuid);

			return BuildRestApiRequest<Uri> (url, ResponseType.GetUserState, sessionUUID, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildPresenceHeartbeatRequest (string channels, string channelGroups, string channelsJsonState, string uuid,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			StringBuilder presenceParamBuilder = new StringBuilder ();
			if (channelsJsonState != "{}" && channelsJsonState != "") {
				presenceParamBuilder.AppendFormat("&state={0}", Utility.EncodeUricomponent (channelsJsonState, ResponseType.PresenceHeartbeat, false, false));
			}
			if (channelGroups != null && channelGroups.Length > 0)
			{
				presenceParamBuilder.AppendFormat("&channel-group={0}", Utility.EncodeUricomponent(channelGroups, ResponseType.PresenceHeartbeat, true, false));
			}

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
			url.Add ("heartbeat");

			return BuildRestApiRequest<Uri> (url, ResponseType.PresenceHeartbeat, uuid, ssl, origin, 0, authenticationKey, presenceParamBuilder.ToString());
		}

		internal static Uri BuildMultiChannelLeaveRequest (string channels, string channelGroups, string uuid, 
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			StringBuilder unsubscribeParamBuilder = new StringBuilder ();
			if(!string.IsNullOrEmpty(Subscription.Instance.CompiledUserState)){
				unsubscribeParamBuilder.AppendFormat("&state={0}", Utility.EncodeUricomponent(Subscription.Instance.CompiledUserState, 
					ResponseType.Leave, false, false));
			}
			if (channelGroups != null && channelGroups.Length > 0)
			{
				unsubscribeParamBuilder.AppendFormat("&channel-group={0}",  Utility.EncodeUricomponent(channelGroups, ResponseType.Leave, true, false));
			}

			List<string> url = new List<string> ();

			url.Add ("v2");
			url.Add ("presence");
			url.Add ("sub_key");
			url.Add (subscribeKey);
			url.Add ("channel");
			url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
			url.Add ("leave");

			return BuildRestApiRequest<Uri> (url, ResponseType.Leave, uuid, ssl, origin, 0, authenticationKey, unsubscribeParamBuilder.ToString());
		}

		internal static Uri BuildMultiChannelSubscribeRequestV2 (string channels, string channelGroups, string timetoken, 
			string channelsJsonState, string uuid, string region, string filterExpr,
			bool ssl, string origin, string authenticationKey, string subscribeKey, int presenceHeartbeat)
		{
			StringBuilder subscribeParamBuilder = new StringBuilder ();
			subscribeParamBuilder.AppendFormat ("&tt={0}", timetoken);

			if (!string.IsNullOrEmpty (filterExpr)) {
				subscribeParamBuilder.AppendFormat ("&filter-expr=({0})",  Utility.EncodeUricomponent(filterExpr, ResponseType.SubscribeV2, false, false));
			}

			if (!string.IsNullOrEmpty (region)) {
				subscribeParamBuilder.AppendFormat ("&tr={0}", Utility.EncodeUricomponent(region, ResponseType.SubscribeV2, false, false));
			}

			if (channelsJsonState != "{}" && channelsJsonState != "") {
				subscribeParamBuilder.AppendFormat ("&state={0}", Utility.EncodeUricomponent (channelsJsonState, ResponseType.SubscribeV2, false, false));
			}

			if (!string.IsNullOrEmpty(channelGroups))
			{
				subscribeParamBuilder.AppendFormat ("&channel-group={0}", Utility.EncodeUricomponent (channelGroups, ResponseType.SubscribeV2, true, false));
			}                   

			List<string> url = new List<string> ();
			url.Add ("v2");
			url.Add ("subscribe");
			url.Add (subscribeKey);
			url.Add (string.IsNullOrEmpty(channels) ? "," : channels);
			url.Add ("0");

			return BuildRestApiRequest<Uri> (url, ResponseType.SubscribeV2, uuid, ssl, origin, presenceHeartbeat, authenticationKey, subscribeParamBuilder.ToString ());
		}

		internal static Uri BuildAddChannelsToChannelGroupRequest(string[] channels, string nameSpace, string groupName, string uuid,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			string parameters = string.Format("?add={0}", Utility.EncodeUricomponent(string.Join(",", channels), ResponseType.ChannelGroupAdd, true, false));

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("channel-registration");
			url.Add("sub-key");
			url.Add(subscribeKey);
			List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
			if (ns != null) {
				url.AddRange (ns);    
			}

			url.Add("channel-group");
			url.Add(groupName);

			return BuildRestApiRequest<Uri>(url, ResponseType.ChannelGroupAdd,
				uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildRemoveChannelsFromChannelGroupRequest(string[] channels, string nameSpace, string groupName, string uuid,
			bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			bool channelsAvailable = false;
			string parameters = "";
			if (channels != null && channels.Length > 0) {
				parameters = string.Format ("?remove={0}", Utility.EncodeUricomponent(string.Join(",", channels), ResponseType.ChannelGroupRemove, true, false));
				channelsAvailable = true;
			}

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("channel-registration");
			url.Add("sub-key");
			url.Add(subscribeKey);
			List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
			if (ns != null) {
				url.AddRange (ns);    
			}
			url.Add("channel-group");
			url.Add(groupName);

			ResponseType respType = ResponseType.ChannelGroupRemove;
			if (!channelsAvailable) {
				url.Add ("remove");
				respType = ResponseType.ChannelGroupRemoveAll;
			}

			return BuildRestApiRequest<Uri> (url, respType,
				uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildGetChannelsForChannelGroupRequest(string nameSpace, string groupName, bool limitToChannelGroupScopeOnly,
			string uuid, bool ssl, string origin, string authenticationKey, string subscribeKey)
		{
			bool groupNameAvailable = false;
			bool nameSpaceAvailable = false;

			// Build URL
			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("channel-registration");
			url.Add("sub-key");
			url.Add(subscribeKey);
			List<string> ns = Utility.CheckAndAddNameSpace (nameSpace);
			if (ns != null) {
				nameSpaceAvailable = true;
				url.AddRange (ns);    
			}

			if (limitToChannelGroupScopeOnly)
			{
				url.Add("channel-group");
			}
			else
			{
				if (!string.IsNullOrEmpty(groupName) && groupName.Trim().Length > 0)
				{
					groupNameAvailable = true;
					url.Add("channel-group");
					url.Add(groupName);
				}

				if (!nameSpaceAvailable && !groupNameAvailable)
				{
					url.Add("namespace");
				}
				else if (nameSpaceAvailable && !groupNameAvailable)
				{
					url.Add("channel-group");
				}
			}

			return BuildRestApiRequest<Uri>(url, ResponseType.ChannelGroupGet,
				uuid, ssl, origin, 0, authenticationKey, "");
		}

		internal static Uri BuildChannelGroupAuditAccessRequest(string channelGroup, string uuid, 
			bool ssl, string origin, string authenticationKey, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";
			long timeStamp = Utility.TranslateDateTimeToSeconds(DateTime.UtcNow);

			string queryString = "";
			StringBuilder queryStringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(authenticationKey))
			{
				queryStringBuilder.AppendFormat("auth={0}", Utility.EncodeUricomponent(authenticationKey, ResponseType.ChannelGroupAuditAccess, false, false));
			}
			if (!string.IsNullOrEmpty(channelGroup))
			{
				queryStringBuilder.AppendFormat("{0}channel-group={1}", (queryStringBuilder.Length > 0) ? "&" : "", 
					Utility.EncodeUricomponent(channelGroup, ResponseType.ChannelGroupAuditAccess, false, false));
			}
			queryStringBuilder.AppendFormat("{0}pnsdk={1}", (queryStringBuilder.Length > 0) ? "&" : "", 
				Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupAuditAccess, false, true));
			queryStringBuilder.AppendFormat("{0}timestamp={1}", (queryStringBuilder.Length > 0) ? "&" : "", timeStamp.ToString());
			queryStringBuilder.AppendFormat("{0}uuid={1}", (queryStringBuilder.Length > 0) ? "&" : "", 
				Utility.EncodeUricomponent(uuid, ResponseType.ChannelGroupAuditAccess, false, false));

			if (secretKey.Length > 0)
			{
				StringBuilder string_to_sign = new StringBuilder();
				string_to_sign.Append(subscribeKey)
					.Append("\n")
					.Append(publishKey)
					.Append("\n")
					.Append("audit")
					.Append("\n")
					.Append(queryStringBuilder.ToString());

				PubnubCrypto pubnubCrypto = new PubnubCrypto(cipherKey);
				signature = pubnubCrypto.PubnubAccessManagerSign(secretKey, string_to_sign.ToString());
				queryString = string.Format("signature={0}&{1}", signature, queryStringBuilder.ToString());
			}

			string parameters = "";
			parameters += "?" + queryString;

			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("auth");
			url.Add("audit");
			url.Add("sub-key");
			url.Add(subscribeKey);

			return BuildRestApiRequest<Uri>(url, ResponseType.ChannelGroupAuditAccess,
				uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		internal static Uri BuildChannelGroupGrantAccessRequest(string channelGroup, bool read, bool write, bool manage, int ttl, string uuid, 
			bool ssl, string origin, string authenticationKey, 
			string publishKey, string subscribeKey, string cipherKey, string secretKey)
		{
			string signature = "0";
			long timeStamp = Utility.TranslateDateTimeToSeconds(DateTime.UtcNow);
			string queryString = "";
			StringBuilder queryStringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(authenticationKey))
			{
				queryStringBuilder.AppendFormat("auth={0}", Utility.EncodeUricomponent(authenticationKey, ResponseType.ChannelGroupGrantAccess, false, false));
			}

			if (!string.IsNullOrEmpty(channelGroup))
			{
				queryStringBuilder.AppendFormat("{0}channel-group={1}", (queryStringBuilder.Length > 0) ? "&" : "", 
					Utility.EncodeUricomponent(channelGroup, ResponseType.ChannelGroupGrantAccess, false, false));
			}

			queryStringBuilder.AppendFormat("{0}", (queryStringBuilder.Length > 0) ? "&" : "");
			queryStringBuilder.AppendFormat("m={0}", Convert.ToInt32(manage));
			queryStringBuilder.AppendFormat("&pnsdk={0}", Utility.EncodeUricomponent(PubnubUnity.Version, ResponseType.ChannelGroupGrantAccess, false, true));
			queryStringBuilder.AppendFormat("&r={0}", Convert.ToInt32(read));
			queryStringBuilder.AppendFormat("&timestamp={0}", timeStamp.ToString()  );
			if (ttl > -1)
			{
				queryStringBuilder.AppendFormat("&ttl={0}", ttl.ToString());
			}
			queryStringBuilder.AppendFormat("&uuid={0}", Utility.EncodeUricomponent(uuid, ResponseType.ChannelGroupGrantAccess, false, false));
			//queryStringBuilder.AppendFormat("&w={0}", Convert.ToInt32(write)); Not supported at this time.

			if (secretKey.Length > 0)
			{
				StringBuilder string_to_sign = new StringBuilder();
				string_to_sign.Append(subscribeKey)
					.Append("\n")
					.Append(publishKey)
					.Append("\n")
					.Append("grant")
					.Append("\n")
					.Append(queryStringBuilder.ToString());

				PubnubCrypto pubnubCrypto = new PubnubCrypto(cipherKey);
				signature = pubnubCrypto.PubnubAccessManagerSign(secretKey, string_to_sign.ToString());
				queryString = string.Format("signature={0}&{1}", signature, queryStringBuilder.ToString());
			}

			string parameters = "";
			parameters += "?" + queryString;

			List<string> url = new List<string>();
			url.Add("v1");
			url.Add("auth");
			url.Add("grant");
			url.Add("sub-key");
			url.Add(subscribeKey);

			return BuildRestApiRequest<Uri>(url, ResponseType.ChannelGroupGrantAccess,
				uuid, ssl, origin, 0, authenticationKey, parameters);
		}

		static StringBuilder AddSSLAndEncodeURL<T>(List<string> urlComponents, ResponseType type, bool ssl, string origin, StringBuilder url)
		{
			// Add http or https based on SSL flag
			if (ssl)
			{
				url.Append("https://");
			}
			else
			{
				url.Append("http://");
			}
			// Add Origin To The Request
			url.Append(origin);
			// Generate URL with UTF-8 Encoding
			for (int componentIndex = 0; componentIndex < urlComponents.Count; componentIndex++)
			{
				url.Append("/");
				if (type == ResponseType.Publish && componentIndex == urlComponents.Count - 1)
				{
					url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, false, false));
				}
				else
				{
					url.Append(Utility.EncodeUricomponent(urlComponents[componentIndex].ToString(), type, true, false));
				}
			}
			return url;
		}

		private static StringBuilder AppendAuthKeyToURL(StringBuilder url, string authenticationKey, ResponseType type){
			if (!string.IsNullOrEmpty (authenticationKey)) {
				url.AppendFormat ("&auth={0}", Utility.EncodeUricomponent (authenticationKey, type, false, false));
			}
			return url;
		}

		private static StringBuilder AppendUUIDToURL(StringBuilder url, string uuid, bool firstInQS){
			if (firstInQS)
			{
				url.AppendFormat("?uuid={0}", uuid);
			}
			else
			{
				url.AppendFormat("&uuid={0}", uuid);
			}
			return url;
		}

		private static StringBuilder AppendPresenceHeartbeatToURL(StringBuilder url, int pubnubPresenceHeartbeatInSeconds){
			if (pubnubPresenceHeartbeatInSeconds != 0) {
				url.AppendFormat ("&heartbeat={0}", pubnubPresenceHeartbeatInSeconds);
			}
			return url;
		}

		private static StringBuilder AppendPNSDKVersionToURL(StringBuilder url, string pnsdkVersion, ResponseType type){
			url.AppendFormat ("&pnsdk={0}", Utility.EncodeUricomponent (pnsdkVersion, type, false, true));
			return url;
		}

		//sessionid
		//ssl
		//origin
		//pubnubPresenceHeartbeatInSeconds
		//authenticationKey
		//pnsdkVersion    
		//parameters
		private static Uri BuildRestApiRequest<T> (List<string> urlComponents, ResponseType type, string uuid, bool ssl, string origin, 
			int pubnubPresenceHeartbeatInSeconds, string authenticationKey, string parameters)
		{
			StringBuilder url = new StringBuilder ();
			string pnsdkVersion = PubnubUnity.Version;
			uuid = Utility.EncodeUricomponent (uuid, type, false, false);

			url = AddSSLAndEncodeURL<T>(urlComponents, type, ssl, origin, url);

			switch (type) {
			case ResponseType.Leave:
			case ResponseType.SubscribeV2:
			case ResponseType.PresenceV2:

				url = AppendUUIDToURL(url, uuid, true);
				url.Append(parameters);
				url = AppendAuthKeyToURL(url, authenticationKey, type);

				url = AppendPresenceHeartbeatToURL(url, pubnubPresenceHeartbeatInSeconds);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.PresenceHeartbeat:

				url = AppendUUIDToURL(url, uuid, true);
				url.Append (parameters);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.SetUserState:

				url.Append (parameters);
				url = AppendUUIDToURL(url, uuid, false);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.GetUserState:

				url = AppendUUIDToURL(url, uuid, true);
				url.Append (parameters);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;
			case ResponseType.HereNow:

				url.Append (parameters);
				url = AppendUUIDToURL(url, uuid, false);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.GlobalHereNow:

				url.Append (parameters);
				url = AppendUUIDToURL(url, uuid, false);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.WhereNow:

				url = AppendUUIDToURL(url, uuid, true);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;

			case ResponseType.Publish:

				url = AppendUUIDToURL(url, uuid, true);
				url.Append (parameters);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);

				break;
			case ResponseType.PushGet:
			case ResponseType.PushRegister:
			case ResponseType.PushRemove:
			case ResponseType.PushUnregister:
			case ResponseType.ChannelGroupAdd:
			case ResponseType.ChannelGroupRemove:
				url.Append (parameters);
				url = AppendUUIDToURL (url, uuid, false);
				url = AppendAuthKeyToURL (url, authenticationKey, type);
				url = AppendPNSDKVersionToURL (url, pnsdkVersion, type);

				break;
			case ResponseType.ChannelGroupGet:
			case ResponseType.ChannelGroupRemoveAll:
				url.Append (parameters);
				url = AppendUUIDToURL(url, uuid, true);
				url = AppendAuthKeyToURL(url, authenticationKey, type);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;
			case ResponseType.DetailedHistory:
			case ResponseType.GrantAccess:
			case ResponseType.AuditAccess:
			case ResponseType.RevokeAccess:
			case ResponseType.ChannelGroupGrantAccess:
			case ResponseType.ChannelGroupAuditAccess:
			case ResponseType.ChannelGroupRevokeAccess:

				url.Append (parameters);
				break;
			default:
				url = AppendUUIDToURL(url, uuid, true);
				url = AppendPNSDKVersionToURL(url, pnsdkVersion, type);
				break;
			}

			Uri requestUri = new Uri (url.ToString ());

			return requestUri;

		}
		#endregion
	}
}
