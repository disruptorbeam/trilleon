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

﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace TrilleonAutomation {

	public class AutomationReport : MonoBehaviour {

		public static int SkipFailCount { get; set; }
		public static double TestSuiteRunTime { get; set; }

		static bool fatalInternalError = false;
		static int orderIndex = 0;
		static StringBuilder xmlBody;
		static StringBuilder htmlBody;
		static StringBuilder jsonBody;
		//static StringBuilder imageCaptureFailures;
		static StringBuilder fileText;

		public static bool IgnoreTestRailsReporting = false;
		public static string reportPath = string.Empty;
		public const string DEPENDENCY_WEB_CIRCULAR_FAILURE_TITLE = "SECONDARY CIRCULAR DEPENDENCY FAILURE DETECTED"; //If this text is changed, also change it in AuotmationReportDatatable.txt!
		public const string FATAL_INTERNAL_FAILURE_TITLE = "FATAL INTERNAL ERROR DETECTED; TEST RUN ABORTED!"; //If this text is changed, also change it in AuotmationReportDatatable.txt!
		private const string PASSED_NAME = "Passed";
		private const string FAILED_NAME = "Failed";
		private const string IGNORED_NAME = "Ignored";
		private const string SKIPPED_NAME = "Skipped";

		private static Dictionary<string,int> TEST_RAILS_STATUSES = new Dictionary<string,int>();

		/// <summary>
		/// Begin an XML report for device farm to determine test results.
		/// </summary>
		public static void Initialize() { 

			TEST_RAILS_STATUSES = new Dictionary<string,int>{
				{ PASSED_NAME, 1 },
				{ FAILED_NAME, 5 },
				{ IGNORED_NAME, 7 },
				{ SKIPPED_NAME, 8 }
			};
			xmlBody = new StringBuilder();
			htmlBody = new StringBuilder();
			jsonBody = new StringBuilder();
			//imageCaptureFailures = new StringBuilder();
			orderIndex = SkipFailCount = 0;
			TestSuiteRunTime = 0d;

		}

		private static List<string> _allScreenshots = new List<string>();

		#if UNITY_EDITOR

		public static List<KeyValuePair<string,string[]>> testsMeta {
			get { 
				return _testsMeta; 
			}
		}
		private static List<KeyValuePair<string,string[]>> _testsMeta = new List<KeyValuePair<string,string[]>>();

		public static void SaveBuddyHistory(string buddyName){

			string saveName = buddyName.Replace(Arbiter.DEVICE_IDENTIFIER_PREFIX, string.Empty);

			string txt = FileBroker.GetNonUnityTextResource(FileResource.BuddyHistory);
			List<string> resultsRaw = txt.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
			StringBuilder fileText = new StringBuilder();

			if(!resultsRaw.Contains(saveName)) {
				
				resultsRaw.Add(saveName);

			}
			resultsRaw = resultsRaw.RemoveNullAndEmpty().Distinct();
			for(int f = 0; f < resultsRaw.Count; f++) {
				
				fileText.AppendLine(saveName);

			}

			//Save updated results.
			FileBroker.SaveNonUnityTextResource(FileResource.BuddyHistory, fileText);

		}

		public static string GetMostRecentBuddy() {

			List<string> buddies = GetBuddyHistory();
			if(buddies.Any()) {
				
				buddies.Last();

			}
			return string.Empty;

		}

		public static List<string> GetBuddyHistory() {

			string txt = FileBroker.GetNonUnityTextResource(FileResource.BuddyHistory);
			return txt.Split(new string[] { "\n" }, StringSplitOptions.None).ToList().FindAll(x => x.Length > 0);

		}

		public static void SaveMostRecentsResults(string status) {

			_testsMeta = new List<KeyValuePair<string,string[]>>();
			fileText = new StringBuilder();

			string plainText = string.Format("status:{0}|name:{1}|class:{2}|test_categories:{3}", 
				status, AutomationMaster.CurrentTestContext.TestName, AutomationMaster.CurrentTestContext.ClassName, string.Join(", ", AutomationMaster.CurrentTestContext.Categories.ToArray()));

			GetMostRecentsResults(status, plainText);

			//Save updated results.
			FileBroker.SaveNonUnityTextResource(FileResource.LatestTestResults, fileText);

		}

		public static void GetMostRecentsResults(string status = "", string plainText = "") {

			_testsMeta = new List<KeyValuePair<string,string[]>>();

			//TODO: Remove tests from file that no longer are automation tests.
			bool skipCreate = string.IsNullOrEmpty(status);

			List<string> resultsRaw = new List<string>();

			string txt = FileBroker.GetNonUnityTextResource(FileResource.LatestTestResults);
			resultsRaw = txt.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

			if(resultsRaw.Count == 0 && !skipCreate) {

				fileText.AppendLine(plainText);

			} else {

				List<Type> currentClasses = AutomationMaster.GetAutomationClasses();
				List<MethodInfo> currentMethods = new List<MethodInfo>();
				currentClasses.ForEach(x => currentMethods.AddRange(x.GetMethods()));

				//Filter out tests that no longer exist, have been renamed, or are no longer marked as automation methods.
				currentMethods = currentMethods.FindAll(x => { 
					Automation[] aut =(Automation[])Attribute.GetCustomAttributes(x, typeof(Automation));
					return aut != null && aut.Length > 0;
				});

				for(int i = 0; i < resultsRaw.Count; i++) {
					
					if(!string.IsNullOrEmpty(resultsRaw[i])) {
						
						string[] row = resultsRaw[i].Split(AutomationMaster.DELIMITER);
						string testName = string.Empty;
						for(int r = 0; r < row.Length; r++) {
							
							string[] keyVal = row[r].Split(':');
							if(keyVal[0] == "name") {
								
								testName = keyVal[1];

							}

						}
						if(!string.IsNullOrEmpty(testName)) {
							
							//Ignore references to methods that are no longer valid automation methods for whatever reason.
							if(currentMethods.FindAll(x => x.Name == testName).Any()) {
								
								_testsMeta.Add(new KeyValuePair<string,string[]>(testName, row));

							}

						}

					}

				}

				if(!skipCreate) {
					
					List<KeyValuePair<string,string[]>> matches = testsMeta.FindAll(x => x.Key == AutomationMaster.CurrentTestContext.TestName);

					if(!matches.Any()) {
						
						_testsMeta.Add(new KeyValuePair<string,string[]>(AutomationMaster.CurrentTestContext.TestName, plainText.Split(AutomationMaster.DELIMITER)));

					} else {
						
						_testsMeta.Remove(matches.First());
						string[] newValues = new string[] {
							string.Format("status:{0}", status),
							string.Format("name:{0}", AutomationMaster.CurrentTestContext.TestName),
							string.Format("class:{0}", AutomationMaster.CurrentTestContext.ClassName),
							string.Format("test_categories:{0}", string.Join(",", AutomationMaster.CurrentTestContext.Categories.ToArray())),
						};
						KeyValuePair<string,string[]> newInsert = new KeyValuePair<string,string[]>(AutomationMaster.CurrentTestContext.TestName, newValues);
						_testsMeta.Add(newInsert);

					}

					for(int f = 0; f < testsMeta.Count; f++) {
						
						fileText.AppendLine(string.Join(AutomationMaster.DELIMITER.ToString(), _testsMeta[f].Value));

					}

				}

			}

		}

		#endif

		/// <summary>
		/// Create x
		/// </summary>
		public static void AddToReport(bool isSuccess, double runTime, bool skipped = false) {

			if(AutomationMaster.ValidationRun) {
				
				return;

			}

			string testName = AutomationMaster.CurrentTestContext.TestName;
			ConsoleMessageType type;
			string status = string.Empty;
			List<string> assertions = AutomationMaster.CurrentTestContext.Assertions;

			if(skipped && isSuccess) {
				
				testName = AutomationMaster.TestRunContext.Ignored.Tests[AutomationMaster.TestRunContext.Ignored.Tests.Count - 1];
				xmlBody.Append(string.Format("<testcase classname=\"{0}\" name=\"{1}\" time=\"{2}\" status=\"ignored\"></testcase>", AutomationMaster.CurrentTestContext.ClassName, testName, 0));
				status = IGNORED_NAME;
				type = ConsoleMessageType.AssertionIgnore; 

			} else if(skipped && !isSuccess){
				
				xmlBody.Append(string.Format("<testcase classname=\"{0}\" name=\"{1}\" time=\"{2}\" status=\"skipped\">", AutomationMaster.CurrentTestContext.ClassName, testName, 0));
				xmlBody.Append(string.Format("<skipped message=\"{0}\" type=\"Test Failure\"></skipped></testcase>", assertions.Any() ? assertions.Last() : "Error occurred before any test assertions were made."));
				status = SKIPPED_NAME;
				type = ConsoleMessageType.AssertionSkip; 
				SkipFailCount++;

			} else if(AutomationMaster.CurrentTestContext.IsSuccess) {
				
				xmlBody.Append(string.Format("<testcase classname=\"{0}\" name=\"{1}\" time=\"{2}\"></testcase>", AutomationMaster.CurrentTestContext.ClassName, testName, runTime));
				status = PASSED_NAME;
				type = ConsoleMessageType.AssertionPass; 

			} else {
				
				xmlBody.Append(string.Format("<testcase classname=\"{0}\" name=\"{1}\" time=\"{2}\">", AutomationMaster.CurrentTestContext.ClassName, testName, runTime));
				xmlBody.Append(string.Format("<failure message=\"{0}\" type=\"Test Failure\"></failure></testcase>", assertions.Any() ? assertions.Last() : "Error occurred before any test assertions were made."));
				status = FAILED_NAME;
				type = ConsoleMessageType.AssertionFail; 

			}

			TestSuiteRunTime += runTime;
			AutoConsole.PostMessage(string.Format("{0}     {1}", status, AutomationMaster.CurrentTestContext.TestName), MessageLevel.Abridged, type, AutomationMaster.CurrentTestContext.TestName);

			#if UNITY_EDITOR
			SaveMostRecentsResults(status);
			#endif

			StringBuilder assertionsJson = new StringBuilder();
			for(int a = 0; a < AutomationMaster.CurrentTestContext.Assertions.Count; a++) {

				//Note; Replace "[{" to prevent the reading of any encoded json as actual JSON, which may cause a parsing failure in the html report.
				assertionsJson.Append(string.Format("{{\"assertion\":\"{0}\"}}{1}", AutomationMaster.CurrentTestContext.Assertions[a].Replace("[{", "||"),(AutomationMaster.CurrentTestContext.Assertions.Count - 1) == a ? string.Empty : ","));

			}

			orderIndex++;
			string error_details = AutomationMaster.CurrentTestContext.Notices.Any() ? string.Join(", ", AutomationMaster.CurrentTestContext.Notices.ToArray()) : string.Empty;
			error_details += AutomationMaster.CurrentTestContext.ErrorDetails.Replace("[{", "||");
			error_details = error_details ?? "No Error Details Were Reported For This Failure!";
			string json = string.Format("{{\"order_ran\":\"{0}\", \"status\":\"{1}\", \"name\":\"{2}\", \"class\":\"{3}\", \"test_categories\":\"{4}\", \"result_details\":\"{5}\", \"assertions\":[{6}]}},", 
				orderIndex, status, AutomationMaster.CurrentTestContext.TestName, AutomationMaster.CurrentTestContext.ClassName, string.Join(", ", AutomationMaster.CurrentTestContext.Categories.ToArray()), 
				error_details, assertionsJson.ToString());

			if(json.Length > ConnectionStrategy.MaxMessageLength) {

				SendJsonInPieces("SINGLE_TEST_RESULTS_JSON", json, orderIndex);
				jsonBody.Append(json);

			} else {

				jsonBody.Append(json);
				AutoConsole.PostMessage(string.Format("SINGLE_TEST_RESULTS_JSON|{0}|", json), MessageLevel.Abridged);

			}

		}

		public static void SendJsonInPieces(string jsonAttributePrefix, string json, int orderRan = -1) {

			//Break this message into pieces.
			List<string> pieces = new List<string>();
			int charPos = 0;

            if(ConnectionStrategy.MaxMessageLength <= 0) {

                throw new UnityException("ConnectionStrategy.MaxMessageLength must be a positive number. This would cause an infinite loop and hard crash. Please fix the source of this value and rerun automation.");

            }

            while(charPos < json.Length) {

				if(charPos + ConnectionStrategy.MaxMessageLength <= json.Length) {

					pieces.Add(json.Substring(charPos, ConnectionStrategy.MaxMessageLength));
					charPos += ConnectionStrategy.MaxMessageLength;

				} else {

					pieces.Add(json.Substring(charPos, json.Length - charPos - 1));
					break; 

				} 

			}

			for(int x = 0; x < pieces.Count; x++) {

				if(orderRan >= 0) {
					
					AutoConsole.PostMessage(string.Format("{5}_MULTI_PART|{1}{0}{2}{0}{3}{0}{4}|", AutomationMaster.PARTIAL_DELIMITER, x, pieces.Count, orderIndex, pieces[x], jsonAttributePrefix), MessageLevel.Abridged);

				} else {

					AutoConsole.PostMessage(string.Format("{0}_MULTI_PART|{1}{2}{3}|", jsonAttributePrefix, x, AutomationMaster.PARTIAL_DELIMITER, pieces[x]), MessageLevel.Abridged);

				}

			}

		}

		#region Test Rails Reporting

		public static void MarkTestRailsTestCase(string status, int testRunId, int testCaseId) {

			if(!AutomationMaster.CurrentTestContext.TestCaseIds.Contains(testCaseId)) {
				
				SendResultForAllCasesToTestRails(status, testRunId, new List<int> { testCaseId });

			}

		}

		public static void MarkTestRailsTestCase(string status, params int[] testCaseIds) {

			if(IgnoreTestRailsReporting) {
				
				return;

			}

			TestRails tr = (TestRails)AutomationMaster.CurrentTestContext.Method.GetCustomAttributes(typeof(TestRails), false).ToList().First();
			if(tr == null) {

				tr = (TestRails)AutomationMaster.CurrentTestContext.Method.ReflectedType.GetCustomAttributes(typeof(TestRails), false).ToList().First();

			}
			if(tr == null) {
				
				string failure = string.Format("The current test, {0}, has test rails id's passed into assertions, but does not have an attribute that provides the required Test Run ID(ex: [TestRails(5555)]).", AutomationMaster.CurrentTestContext.TestName);
				AutomationMaster.Arbiter.SendCommunication(failure);
				Q.assert.StartCoroutine(Q.assert.Fail(failure));
				return;

			}

			List<int> remainingIds = new List<int>();
			List<int> passedIds = testCaseIds.ToList();
			for(int x = 0; x < passedIds.Count; x++) {
				
				if(!AutomationMaster.CurrentTestContext.TestCaseIds.Contains(passedIds[x])) {
					
					remainingIds.Add(passedIds[x]);

				}

			}

			if(remainingIds.Any()) {

				AutomationMaster.CurrentTestContext.TestCaseIds.AddRange(remainingIds);
				SendResultForAllCasesToTestRails(status, tr.RunId, remainingIds);

			}

		}

		//Report test results to TestRails.
		private static void SendResultForAllCasesToTestRails(string status, int testRunId, List<int> testCaseIds) {

			StringBuilder json = new StringBuilder();
			json.Append("{\"results\": [");
			int idStatus = TEST_RAILS_STATUSES[status];

			for(int x = 0; x < testCaseIds.Count; x++) {

				string comment = string.Empty;
				string deviceDetails = AutomationMaster.GameMaster.GetDeviceDetails();
				if(status == FAILED_NAME) {
					
					comment = string.Format("FROM TEST METHOD - {0} - FAILED! Details: {1} [{2}]", AutomationMaster.CurrentTestContext.TestName, AutomationMaster.CurrentTestContext.ErrorDetails, deviceDetails);

				} else {
					
					comment = string.Format("FROM TEST METHOD - {0} - {1} [{2}]", AutomationMaster.CurrentTestContext.TestName, status.ToUpper(), deviceDetails);

				}
				json.AppendLine("{");
				json.Append(string.Format("\"test_id\":{0},", testCaseIds[x]));
				json.Append(string.Format("\"status_id\":{0},", idStatus));
				json.Append(string.Format("\"comment\":\"{0}\"", comment));
				json.Append("}");
				if(x + 1 < testCaseIds.Count) {
					json.Append(",");
				}

			}
			json.Append("]");
			json.Append("}");

			TestRailsAPIClient client = new TestRailsAPIClient(GameMaster.BASE_TEST_RAILS_URL);
			AutoConsole.PostMessage(client.SendPost(string.Format("add_results/{0}", testRunId), json.ToString()));

			for(int t = 0; t < testCaseIds.Count; t++) {

				string jsonResult = client.GetTestName(testCaseIds[t]);
				//Only report as failure if assertion has not already failed, leading to these test cases being marked as fails.
				if(!Q.assert.IsFailing) {

					AutomationMaster.CurrentTestContext.AddAssertion(string.Format("<a href='{0}{1}'>{2}</a>", TestRailsAPIClient.GetUrl(), testCaseIds[t], jsonResult));
				
				} else {

					AutomationMaster.CurrentTestContext.AddTestCaseAssertionOnFailure(string.Format("**TRY_FAIL**<a href='{0}{1}'>{2}</a>", TestRailsAPIClient.GetUrl(), testCaseIds[t], jsonResult));

				}

			}

		}

		#endregion

		public void ReportUnhandledException(string message, string stacktrace) {

			fatalInternalError = true;
			AutomationMaster.CurrentTestContext.AddAssertion(string.Format("{0}: {1}", FATAL_INTERNAL_FAILURE_TITLE, message));
			AutomationMaster.CurrentTestContext.ErrorDetails = stacktrace;
			AutomationMaster.CurrentTestContext.IsSuccess = false;
			AutomationMaster.TestRunContext.Failed.Add(AutomationMaster.CurrentTestContext.TestName, new string[] { message, stacktrace, string.Empty });
			AddToReport(false, 0, false);
			StartCoroutine(SaveReport());

		}

		/// <summary>
		/// Write to XML report for device farm to determine test results.
		/// </summary>
		public IEnumerator SaveReport() {

			AutoConsole.ReportOnce = true;
			StringBuilder allXml = new StringBuilder();
			allXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			allXml.Append("<testsuites>");
			allXml.Append(string.Format("<testsuite failures=\"{0}\" tests=\"{1}\" errors=\"{0}\" name=\"Automation-Tests\" skipped=\"{2}\" time=\"{3}\">", AutomationMaster.TestRunContext.Failed.Tests.Count,
				(AutomationMaster.TestRunContext.Failed.Tests.Count + AutomationMaster.TestRunContext.Passed.Tests.Count + AutomationMaster.TestRunContext.Ignored.Tests.Count), AutomationMaster.TestRunContext.Ignored.Tests.Count, TestSuiteRunTime));
			allXml.Append(xmlBody.ToString());
			allXml.Append("</testsuite>");
			allXml.Append("</testsuites>");
			SetCompleteReport();

			#if UNITY_WEBGL
			WebGLBroker.ReportXmlResults(allXml.ToString());
			WebGLBroker.ReportJsonResults(jsonBody.ToString());
			#endif

			//Send XML results.
			int finalXmlLength = allXml.ToString().Length;
			int handledCharactersIndex = 0;
			int subStringLength = ConnectionStrategy.MaxMessageLength;
			int iterationIndex = 0;
			bool complete = false;

			AutomationMaster.Arbiter.SendCommunication("xml_start", string.Format("index|{0}|", Math.Ceiling((double)finalXmlLength / subStringLength)));
			yield return StartCoroutine(Q.driver.WaitRealTime(2.5f));

			//Send XML piece by piece through Pubnub channel, respecting the maximum message size limit of 32 KB.
			while(!complete && handledCharactersIndex < finalXmlLength) {

				string messagePrefix = string.Format("xml_fragment_{0}", iterationIndex);

				//If the remaining characters is greater than 0 but less than the substring length, make the new substring length equal to that character count, so that we do not overflow the substring method.
				if(finalXmlLength - handledCharactersIndex < subStringLength) {
					subStringLength = finalXmlLength - handledCharactersIndex;
					complete = true;
				}

				AutomationMaster.Arbiter.SendCommunication(messagePrefix, string.Format("||{0}||", allXml.ToString().Substring(handledCharactersIndex, subStringLength)));
				handledCharactersIndex += subStringLength;
				iterationIndex++;
				yield return StartCoroutine(Q.driver.WaitRealTime(5f));

			}

			yield return StartCoroutine(Q.driver.WaitRealTime(5f));
			AutomationMaster.Arbiter.SendCommunication("xml_complete", "0");
			yield return StartCoroutine(Q.driver.WaitRealTime(5f));

			#if UNITY_EDITOR
			GameObject go = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME);
			if(go != null) {
				
				go.GetComponent<AutomationMaster>().HandleReportSending(htmlBody.ToString());

			}
			#endif

			yield return null;

		}

		private static string GetExceptionJson() {

			StringBuilder exceptionJson = new StringBuilder();
			exceptionJson.Append("[");
			for(int e = 0; e < AutomationMaster.TestRunContext.Exceptions.Reported.Count; e++) {
				
				BatchContext.GameException ex = AutomationMaster.TestRunContext.Exceptions.Reported[e];
				exceptionJson.Append(string.Format("{{ \"CurrentRunningTest\":\"{0}\", \"TimeStamp\":\"{1}\", \"TestExecutionTime\":\"{2}\", \"Error\":\"{3}\", \"ErrorDetails\":\"{4}\" }}",
					ex.CurrentRunningTest, ex.TimeStamp, ex.TestExecutionTime, ex.Error.Replace("[{", "||"), ex.ErrorDetails));
				if(e + 1 < AutomationMaster.TestRunContext.Exceptions.Reported.Count) {
					
					exceptionJson.Append(",");

				}

			}
			exceptionJson.Append("]");

			return exceptionJson.ToString();

		}

		private static string GetReportScripts(){

			StringBuilder scripts = new StringBuilder();
			scripts.Append("<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'>");
			scripts.Append("<script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js'></script>");
			scripts.Append("<script type='text/javascript' src='https://cdn.datatables.net/1.10.12/js/jquery.dataTables.min.js'></script>");
			scripts.Append("<script type='text/javascript' src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js'></script>");
			scripts.Append("<script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script>");
			scripts.Append(string.Format("<script>{0}</script>", FileBroker.GetTextResource(FileResource.ReportJavascript)));

			return scripts.ToString();

		}

		private static string GetReportCss(){

			StringBuilder css = new StringBuilder();
			css.Append("<style>");

			//Temporararily removing the usage of inline png's.
			/*for(int f = 0; f < AutomationMaster.TestRunContext.Failed.tests.Count; f++) {
         	string testName = AutomationMaster.TestRunContext.Failed.tests[f].Key;
         	bool saveInline = !AutomationMaster.ConfigReader.GetBool("EDITOR_IS_DOT_NET_SUBSET");
         	string inlinePngString = saveInline ? GetScreenShotAsInlinePng(testName) : string.Empty;
         	if(!string.IsNullOrEmpty(inlinePngString)) {
            	css.AppendLine(string.Format(".fail_test_image_{0} {{ background-repeat: no-repeat; width: 1240px; height: 600px; background-image:url('data:image/png;base64,{1}'); }}", testName, inlinePngString));
            } else {
            	if(saveInline) {
               	imageCaptureFailures.Append(string.Format("<div id='fail_test_no_image_{0}' style='display: none;'></div>", testName));
               }
            }
         	}*/

			css.Append(FileBroker.GetTextResource(FileResource.ReportCss));
			css.Append("</style>");

			return css.ToString();

		}    

		public static void BuildTemporaryReportForSingleConsoleFailure(string testMethodName) {

			#if UNITY_EDITOR
			SetCompleteReport(testMethodName);
			FileBroker.CreateFileAtPath(string.Format("{0}SingleTestAsReportTemp.html", FileBroker.RESOURCES_DIRECTORY), htmlBody.ToString());
			#endif

		}

		private static void SetCompleteReport(string filterSingleTest = "") {

			//Build report locally.
			if(Application.isEditor) {

				StringBuilder html = new StringBuilder();
				html.Append(GetReportScripts());
				html.Append(GetReportCss());
				html.Append("<body>");
				html.Append("<div class='container'>");
				html.Append("<h2>Trilleon: Automation Test Run Report</h2>");
				html.Append("<input id='IsEditorCreated' type='hidden' value='True'/>");
				html.Append(string.Format("<input id='ExceptionJsonHidden' type='hidden' value='{0}'/>", GetExceptionJson()));

				if(string.IsNullOrEmpty(filterSingleTest)) {

					html.Append(AutomationMaster.GameMaster.GetGameSpecificEmailReportTagLine());
					html.Append("<div class='switch_to_from_chart' type='button' onClick='ToggleChartView();'><div id='chart_image' class='switch_image_div chart_image_div chart_image'></div></div>");

					//Memory Usage Chart Panel.
					html.Append("<div id='memory_graphs'>"); 
					html.Append("<div class='tab_toggle button tab_toggle_left_end status_success_show' type='button' onclick='ToggleChartDisplay($(this));'><div class='tab_toggle_name'>Garbage Collection</div></div>");        
					html.Append("<div class='tab_toggle button tab_toggle_inner status_success_hide' type='button' onclick='ToggleChartDisplay($(this));'><div class='tab_toggle_name'>Asset Run Time</div></div>");        
					html.Append("<div class='tab_toggle button tab_toggle_right_end status_success_hide' type='button' onclick='ToggleChartDisplay($(this));'><div class='tab_toggle_name'>FPS</div></div>"); 
					html.Append("<div id='chart_gc_div'>");
					html.Append(string.Format("<span class='chart_result'><strong>Min GC Memory:</strong> {0} mb</span>", PerformanceTracker.min_gc_memory));
					html.Append(string.Format("<span class='chart_result'><strong>Avg GC Memory:</strong> {0} mb</span>", PerformanceTracker.GetAverageGarbageCollectorMemoryUsageDuringTestRun()));
					html.Append(string.Format("<span class='chart_result'><strong>Max GC Memory:</strong> {0} mb</span>", PerformanceTracker.max_gc_memory));
					html.Append("<div id='chart_gc' class='memory_gc_chart'></div>");
					html.Append("</div>");
					html.Append("<div id='chart_rt_div'>");
					html.Append(string.Format("<span class='chart_result'><strong>Min RT Memory:</strong> {0} mb</span>", PerformanceTracker.min_hs_memory));
					html.Append(string.Format("<span class='chart_result'><strong>Avg RT Memory:</strong> {0} mb</span>", PerformanceTracker.GetAverageHeapSizeMemoryUsageDuringTestRun()));
					html.Append(string.Format("<span class='chart_result'><strong>Max RT Memory:</strong> {0} mb</span>", PerformanceTracker.max_hs_memory));
					html.Append("<div id='chart_rt' class='memory_rt_chart'></div>");
					html.Append("</div>");
					html.Append("<div id='chart_fps_div'>");
					html.Append(string.Format("<span class='chart_result'><strong>Min FPS:</strong> {0} </span>", PerformanceTracker.min_fps));
					html.Append(string.Format("<span class='chart_result'><strong>Avg FPS:</strong> {0} </span>", PerformanceTracker.GetAverageFPSDuringTestRun()));
					html.Append(string.Format("<span class='chart_result'><strong>Max FPS:</strong> {0} </span>", PerformanceTracker.max_fps));
					html.Append("<div id='chart_fps' class='fps_chart'></div>");
					html.Append("</div></div>");

					//All screenshots for this report.
					StringBuilder screenshotLinks = new StringBuilder();
					for(int x = 0; x < _allScreenshots.Count; x++) {
						screenshotLinks.Append(string.Format("{0}.png|", _allScreenshots[x]));
					}
					html.Append(string.Format("<input id='screenshot_file_names' type='hidden' value='{0}'/>", screenshotLinks.ToString()));
					html.Append(string.Format("<input id='memory_usage_rt_hidden' type='hidden' value='{0}'/>", PerformanceTracker.GetHeapSizeCounterJsonReportWithReset()));
					html.Append(string.Format("<input id='memory_usage_gc_hidden' type='hidden' value='{0}'/>", PerformanceTracker.GetGarbageCollectorJsonReportWithReset()));
					html.Append(string.Format("<input id='performance_fps_hidden' type='hidden' value='{0}'/>", PerformanceTracker.GetFpsJsonReportWithReset()));
					html.Append("<input id='test_run_type' type='hidden' value='full'/>");

				} else {

					html.Append(string.Format("<input id='test_run_type' type='hidden' value='single_test'/><input id='filter_single_test' type='hidden' value='{0}'/>", filterSingleTest));

				}

				//Test Results Panel.
				html.Append(string.Format("<input id='results_hidden' type='hidden' value='[{0}]'/>", jsonBody.ToString().Trim(',')));
				html.Append("<div id='test_results_table_panel'>");
				html.Append(string.Format("<div class='critical_error_detected status_critical_error_show'>{0}</div>", fatalInternalError ? "Fatal Exception Encountered; Test Run Aborted!" : "Secondary Circular Dependency Failure Encountered!"));
				if(string.IsNullOrEmpty(filterSingleTest)) {
					
					html.Append("<br/>");
					html.Append("<div style='display:inline-block; margin-right:10px;'><strong>Info Panels:</strong></div>");
					html.Append("<div id='gallery_button' class='button screenshots_button' type='button' onClick='ShowPanel(\"Gallery\");'>Screenshots</div>");
					html.Append("<div id='warnings_button' class='button_toggle button warnings_button' type='button' onClick='ShowPanel(\"Warnings\");'>Warnings</div>");
					if(AutomationMaster.TestRunContext.Exceptions.Reported.Count > 0) {

						html.Append("<div id='exceptions_button' class='button_toggle button warnings_button' type='button' onClick='ShowPanel(\"Exceptions\");'>Exceptions</div>");

					}
					html.Append("<br/>");

				}
				html.Append("<div style='display:inline-block; margin-right:10px;'><strong>Show/Hide:</strong> </div>");
				html.Append("<div class='button_toggle button status_success_show' type='button' onClick='ToggleVisibility(\"Success\");'>Success</div>");
				html.Append("<div class='button_toggle button status_failure_show' type='button' onClick='ToggleVisibility(\"Failure\");'>Failed</div>");
				html.Append("<div class='button_toggle button status_skipped_show' type='button' onClick='ToggleVisibility(\"Skipped\");'>Skipped</div>");
				html.Append("<div class='button_toggle button status_ignored_show' type='button' onClick='ToggleVisibility(\"Ignored\");'>Ignored</div>");
				html.Append("<table id='test_results' class='table'>");
				html.Append("<thead><tr class='table_header'><td>Test Name</td><td>Test Class</td><td>Test Categories</td><td>Status</td><td>Details</td><td>Order Ran</td></tr></thead>");
				html.Append("<tbody></tbody>");
				html.Append(@"<div id='show_results' class='details_Panel'>
                        <div class='wrapper'>
                           <div class='close' onClick='HideDetails();'>X</div>
                           <div class='result_details_title'>Test Details</div>
                           <div id='result_details' class='result_details'>DETAILS</div>
                           <div class='details_title'>Error Message:</div>
                           <div id='result_message'class='error_message_details'>PLACEHOLDER</div>
                           <div class='details_title'>Error Details:</div>
                           <div id='result_message_details'class='error_message_details'>PLACEHOLDER</div>
                           <div id='assertion_title' class='details_title'>Assertions:</div>
                           <div id='assertions_list'class='error_message_details'>PLACEHOLDER</div>
                           <div id='reason_title' class='details_title'>Reason:</div>
                           <div id='reason_message_details'class='error_message_details'>PLACEHOLDER</div>
                        </div>
                        </div>
                        </div>
                        </div>");

				//Toolbar and hidden data fields.
				html.Append("<div id='corner_tab' class='corner_tab'><div id='corner_tab_arrow' onClick='ToggleFooterPanel($(this));' class='corner_tab_arrow corner_tab_open'>&#10095;</div></div>");
				html.Append("<div class='notice-popup display_panel'><div class='close' onclick='$(this).parent().hide(400); $(\".background_transparency\").hide(400);'>X</div><div class='message'></div></div>");
				html.Append("<div id='bottom_panel' class='bottom_panel'>");
				html.Append("<div class='additional_tools_div'><a class='additional_tools' href='www.trilleonautomation.wiki' target='_blank'>About This Report</a></div>");
				html.Append("<div class='link_separator'>&#9679;</div>");
				html.Append("<div class='additional_tools_div'><a class='additional_tools' href='www.trilleonautomation.wiki' target='_blank'>About Trilleon</a></div>");
				html.Append("</div>");
				html.Append("<div id='screenshot_panel' class='display_panel'><div class='close' onclick='$(this).parent().hide(400); CloseTransparencyLayer();'>X</div><div class='screenshot_image'></div></div>");
				html.Append("<div id='communications_panel' class='display_panel'><div class='close' onclick='$(this).parent().hide(400); CloseTransparencyLayer();'>X</div><h2 style='margin-top: 10px;'>Communications</h2>");
				html.Append("<div style='width:10px;height:10px;background-color: #007AA2;display: inline-block;'></div> Server | <div style='width:10px;height:10px;background-color: #009056;display: inline-block;'></div> Client");
				html.Append("<div class='message_exchange_list'></div></div>");
				html.Append("<div id='warnings_panel' class='display_panel'><div class='close' onclick='$(this).parent().hide(400); CloseTransparencyLayer();'>X</div><h2 style='margin-top: 10px;'>Warnings</h2><div class='warnings_list'></div></div>");
				html.Append("<div id='gallery_panel' class='display_panel'><div class='close' onclick='$(this).parent().hide(400); CloseTransparencyLayer();'>X</div><h2 style='margin: 10px; margin-bottom: 40px;'>Screenshot Gallery</h2></div>");
				html.Append("<div id='exceptions_panel' class='display_panel'><div class='close' onclick='$(this).parent().hide(400); CloseTransparencyLayer();'>X</div><h2 style='margin: 10px; margin-bottom: 40px;'>Non-Trilleon Exceptions</h2></div>");
				if(AutomationMaster.TestRunContext.Exceptions.Reported.Count > 0) {
					
					StringBuilder exceptionsData = new StringBuilder();
					exceptionsData.Append("[");
					for(int ed = 0; ed < AutomationMaster.TestRunContext.Exceptions.Reported.Count; ed++) {

						exceptionsData.Append(string.Format("{{\"screenshot_name\":\"{0}\",\"error\":\"{1}\",\"error_details\":\"{2}\"}}", AutomationMaster.TestRunContext.Exceptions.Reported[ed].ScreenshotName, AutomationMaster.TestRunContext.Exceptions.Reported[ed].Error.Length > 200 ? AutomationMaster.TestRunContext.Exceptions.Reported[ed].Error.Substring(0, 200) : AutomationMaster.TestRunContext.Exceptions.Reported[ed].Error, string.Format("{0}: {1}", AutomationMaster.TestRunContext.Exceptions.Reported[ed].Error, AutomationMaster.TestRunContext.Exceptions.Reported[ed].ErrorDetails)));
						if(ed + 1 != AutomationMaster.TestRunContext.Exceptions.Reported.Count) {

							exceptionsData.Append(",");

						}

					}
					exceptionsData.Append("]");
					html.Append(string.Format("<input id='exceptions_hidden' type='hidden' value='{0}'/>", exceptionsData.ToString()));

				}
				html.Append("<div class='background_transparency'></div>");
				html.Append("</body>");

				htmlBody = new StringBuilder();
				htmlBody.Append(html.ToString());
				SkipFailCount = 0; //Reset value;

			}

		}

		#region ScreenShot

		public IEnumerator TakeScreenshot(string screenshotName) {

			#if UNITY_EDITOR

			_allScreenshots.Add(screenshotName);

			string directory = string.Format("{0}{1}/screenshots/", FileBroker.REPORTS_DIRECTORY, AutomationMaster.TestRunLocalDirectory);
			if(!Directory.Exists(directory)) {
				
				Directory.CreateDirectory(Path.GetDirectoryName(directory));

			}

			directory = string.Format("{0}{1}.png",directory, screenshotName);
			string singleTestReportScreenshotsDirectory = string.Format("{0}screenshots/{1}.png", FileBroker.RESOURCES_DIRECTORY, screenshotName);

			int counter = 1;
			while(Directory.Exists(singleTestReportScreenshotsDirectory) || Directory.Exists(directory)) {
				
				singleTestReportScreenshotsDirectory = string.Format("{0}screenshots/{1}.png", FileBroker.RESOURCES_DIRECTORY, string.Format("{0}_{1}", screenshotName, counter.ToString()));
				directory = string.Format("{0}{1}.png",directory, string.Format("{0}_{1}", screenshotName, counter.ToString()));
				counter ++;

			}

			ScreenCapture.CaptureScreenshot(singleTestReportScreenshotsDirectory);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			FileBroker.CopyFile(singleTestReportScreenshotsDirectory, directory);

			#endif

			yield return null;

		}

		private static string GetScreenShotAsInlinePng(string fileName) {

			_allScreenshots.Add(fileName);
			string file = string.Empty;
			#if UNITY_EDITOR
			file = string.Format("{0}/{1}.png", FileBroker.SCREENSHOTS_DIRECTORY, fileName);
			#else
			file = string.Format("{0}/{1}.png", Application.persistentDataPath, fileName);
			#endif

			//Application.CaptureScreenshot occassionally fails. This means the file may not exist.
			if(FileBroker.Exists(file)) {
				
				byte[] imageData = File.ReadAllBytes(file);
				return Convert.ToBase64String(imageData);

			} else {
				
				return string.Empty;

			}

		}

		#endregion

		private static List<KeyValuePair<string,string>> EncodingKeys = new List<KeyValuePair<string,string>>();
		public static string EncodeCharactersForJson(string val) {

			if(string.IsNullOrEmpty(val)) {
				
				return string.Empty;

			}

			string result = System.Text.RegularExpressions.Regex.Replace(val, @"[^\u0000-\u007F]+", string.Empty); //Remove non ASCII characters from strings.
			result = new string(result.ToCharArray().ToList().FindAll(c => !char.IsControl(c)).ToArray()); //Remove control characters from strings.
			if(EncodingKeys.Count == 0) {

				EncodingKeys.Add(new KeyValuePair<string, string>("<","&#60;"));
				EncodingKeys.Add(new KeyValuePair<string, string>(">","&#62;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("-","&#45;"));
				EncodingKeys.Add(new KeyValuePair<string, string>(".","&#46;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("~","&#126;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("[","&#91;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("]","&#93;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("{","&#123;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("}","&#125;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("\\","&#47;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("\"","&#39;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("'","&#39;"));
				EncodingKeys.Add(new KeyValuePair<string, string>(":","&#58;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("(","&#40;"));
				EncodingKeys.Add(new KeyValuePair<string, string>(")","&#41;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("*","&#42;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("/","&#47;"));
				EncodingKeys.Add(new KeyValuePair<string, string>("\n"," "));
				EncodingKeys.Add(new KeyValuePair<string, string>("\t"," "));
				EncodingKeys.Add(new KeyValuePair<string, string>("\r"," "));
				EncodingKeys.Add(new KeyValuePair<string, string>("\b"," "));

			}

			for(int x = 0; x < EncodingKeys.Count; x++) {

				result = result.Replace(EncodingKeys[x].Key.ToString(), EncodingKeys[x].Value);

			}

			return result;

		}

	}

}
