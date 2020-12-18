/*
 * TestRail API binding for .NET (API v2, available since TestRail 3.0)
 *
 * Learn more:
 *
 * http://docs.gurock.com/testrail-api2/start
 * http://docs.gurock.com/testrail-api2/accessing
 *
 * Copyright Gurock Software GmbH.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TrilleonAutomation {

	public class TestRailsAPIClient {        
		public static string TEST_RUN_URL_PATH = "index.php?/tests/view/";
		private string m_user {
			get { 
				return AutomationMaster.ConfigReader.GetString("TEST_RAILS_USERNAME");
			}
		}
		private string m_password {
			get { 
				return AutomationMaster.ConfigReader.GetString("TEST_RAILS_PASSWORD");
			}
		}
		private string m_url;

		public TestRailsAPIClient(string base_url) {
			if (!base_url.EndsWith("/")) {
				base_url += "/";
			}

			this.m_url = base_url + "index.php?/api/v2/";
		}

		public string User {
			get { return this.m_user; }
		}

		public static string GetUrl() {

			return string.Format("{0}{1}{2}", GameMaster.BASE_TEST_RAILS_URL, GameMaster.BASE_TEST_RAILS_URL.EndsWith("/") ? string.Empty : "/", TestRailsAPIClient.TEST_RUN_URL_PATH);

		}

		public string SendPost(string uri, string json) {
			
			if(UnityEngine.Application.isEditor && !AutomationMaster.ConfigReader.GetBool("TEST_RAILS_ACTIVATE_WHILE_IN_EDITOR_MODE")) {
				
				return "Test Rails Deactivated! This can be changed in Trilleon settings.";

			} else {
				
				return SendRequest("POST", uri, json);

			}

		}

		public string SendGet(string uri) {

			if(!UnityEngine.Application.isEditor && !AutomationMaster.ConfigReader.GetBool("TEST_RAILS_ACTIVATE_WHILE_IN_EDITOR_MODE")) {
				
				return "Test Rails Deactivated! This can be changed in Trilleon settings.";

			} else {

				return SendRequest("GET", uri, "[{}]");

			}

		}

		public string GetTestName(int idTest) {

			if(UnityEngine.Application.isEditor && !AutomationMaster.ConfigReader.GetBool("TEST_RAILS_ACTIVATE_WHILE_IN_EDITOR_MODE")) {
				
				return "Test Rails Deactivated! This can be changed in Trilleon settings.";

			} else {

				string error = "**Error retrieving this test's name through TestRails api**";
				string result = SendRequest("GET", string.Format("get_test/{0}", idTest), string.Empty);
				string[] split = new string[] { "\"title\":\"" };
				string[] splitResult = result.Split(split, StringSplitOptions.None);
				if(splitResult.Length < 2) {
					return string.Format("{0} Returned status: ", error);
				}
				result = splitResult[1];
				split = new string[] { "\",\"" }; 
				splitResult = result.Split(split, StringSplitOptions.None);
				if(splitResult.Length < 2) {
					return string.Format("{0} Returned status: ", error);
				}

				return splitResult[0].Replace("\\", string.Empty);

			}

		}

		private string SendRequest(string method, string uri, string json) {

			try {

				string url = this.m_url + uri;

				// Create the request object and set the required HTTP method
				// (GET/POST) and headers (content type and basic auth).
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.ContentType = "application/json";
				request.Method = method;

				string auth = Convert.ToBase64String(
					Encoding.ASCII.GetBytes(
						String.Format(
							"{0}:{1}",
							this.m_user,
							this.m_password
						)
					)
				);

				request.Headers.Add("Authorization", "Basic " + auth);

				if (method == "POST") {
					// Add the POST arguments, if any. We just serialize the passed
					// data object (i.e. a dictionary) and then add it to the request
					// body.
					using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
						streamWriter.Write(json);
						streamWriter.Flush();
						streamWriter.Close();
					}

					/*if (!string.IsNullOrEmpty(json))
               {
                  byte[] block = Encoding.UTF8.GetBytes( 
                     json
                  );
                  request.GetRequestStream().Write(block, 0, block.Length);
               }*/
				}

				// Execute the actual web request (GET or POST) and record any
				// occurred errors.
				Exception ex = null;
				HttpWebResponse response = null;
				try {
					response = (HttpWebResponse)request.GetResponse();
				} catch (WebException e) {
					if(e.Response != null) {
						response = (HttpWebResponse)e.Response;
						ex = e;
					}
				}

				// Read the response body, if any, and deserialize it from JSON.
				string text = "";
				if (response != null) {
					var reader = new StreamReader(
						response.GetResponseStream(),
						Encoding.UTF8
					);

					using (reader){
						text = reader.ReadToEnd();
					}
				}

				// Check for any occurred errors and add additional details to
				// the exception message, if any (e.g. the error message returned
				// by TestRail).
				if (ex != null) {
					string error = text;
					if (error != null) {
						error = '"' + error + '"';
					}
					else {
						error = "No additional error message received";
					}
					AutoConsole.PostMessage(String.Format("TestRail API returned HTTP {0} ({1}) for IdTest {2} with JSON [{3}]",(int)response.StatusCode, error, uri, json), MessageLevel.Abridged);
				}

				return text;

			} catch(Exception e) {

				return e.Message;

			}

		}
	}
}