/* 
+   This file is part of Trilleon.  Trilleon is a client automation framework.
+  
+   Copyright (C) 2017 Disruptor Beam
+  
+   Trilleon is free software: you can redistribute it and/or modify
+   it under the terms of the GNU Lesser General Public License as published by
+   the Free Software Foundation, either version 3 of the License, or
+   (at your option) any later version.
+
+   This program is distributed in the hope that it will be useful,
+   but WITHOUT ANY WARRANTY; without even the implied warranty of
+   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
+   GNU Lesser General Public License for more details.
+
+   You should have received a copy of the GNU Lesser General Public License
+   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrilleonAutomation {
		
	/// <summary>
	/// Incomplete. Conceptual for connecting to game server APIs.
	/// </summary>
	public class GameServerBroker : MonoBehaviour {

		public enum RequestType { POST, GET }

		public List<KeyValuePair<string,string>> Endpoints = new List<KeyValuePair<string,string>> {
			new KeyValuePair<string,string>("example_url", "myurl.com/api/some_method")
		};

		public IEnumerator SendRequest(string endpoint, RequestType requestType, List<KeyValuePair<string,string>> args, List<KeyValuePair<string,string>> headers) {

			WWWForm form = new WWWForm();
			for(int x = 0; x < args.Count; x++) {
				
				form.AddField(args[x].Key, args[x].Value);

			}

			Dictionary<string, string> _headers = form.headers;
			for(int x = 0; x < headers.Count; x++) {
				
				_headers[headers[x].Key] = headers[x].Value;

			}

			string url = Endpoints.Find(x => x.Key == endpoint).Value;
			WWW www = new WWW(url, form.data, _headers);
			yield return www;

			string message = string.Empty;
			if (www.error == null) {

				message = string.Format("API Call Success: {0}", url);

			}
			else {

				message = string.Format("API Call Failure: {0} Message: {1}", url, www.error);
				yield return StartCoroutine(Q.assert.Fail(message));

			}
			AutoConsole.PostMessage(message);
			yield return null;

		}
			
	}

}
