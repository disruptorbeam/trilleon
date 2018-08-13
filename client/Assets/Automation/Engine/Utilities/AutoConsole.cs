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

﻿#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TrilleonAutomation {

	public static class AutoConsole {

		const int MAX_LOG_COUNT_HISTORY = 25;
		const int MAX_LOG_RETURN_LENGTH = 2000;
		static string ConsoleLog = string.Empty;

		public static bool ReportOnce { get; set; }

		public static bool Paused {
			get { return _paused; }
			set { 
				_paused = value; 
				PauseValueChanged();
			}
		}
		static bool _paused;

		public static MessageLevel FilterLevel {
			get { return _filterLevel; }
			set { _filterLevel = value; }
		}
		static MessageLevel _filterLevel;

		public static List<Log> Logs {
			get { return _logs; }
			private set { _logs = value; }
		}
		static List<Log> _logs = new List<Log>();

		public static List<AutoConsoleMessage> messageListDisplayed = new List<AutoConsoleMessage>();
		public static List<AutoConsoleMessage> messageListQueued = new List<AutoConsoleMessage>();
		public static List<AutoConsoleMessage> messageListToSave = new List<AutoConsoleMessage>();

		public static void PostMessage(string message, ConsoleMessageType consoleMessageType) {

			PostMessage(new AutoConsoleMessage(message, MessageLevel.Verbose, consoleMessageType, string.Empty));

		}

		public static void PostMessage(string message, MessageLevel messageLevel = MessageLevel.Verbose) {

			PostMessage(new AutoConsoleMessage(message, messageLevel, ConsoleMessageType.TestRunnerUpdate, string.Empty));

		}

		public static void PostMessage(string message, MessageLevel messageLevel, ConsoleMessageType consoleMessageType, string testMethod = "") {

			PostMessage(new AutoConsoleMessage(message, messageLevel, consoleMessageType, testMethod));

		}

		public static void PostMessage(AutoConsoleMessage consoleMessage) {

			//Only queue value if console is paused.
			if(Paused) {

				//Do not post, but add the message to the queue until Verbose is chosen, or UnPaused.
				messageListQueued.Add(consoleMessage);

			} else {

				messageListDisplayed.Add(consoleMessage);

			}

			//Only send Abridged (Important) messages to the Arbiter communication channel.
			if(AutomationMaster.Initialized && consoleMessage.level == MessageLevel.Abridged) {
				
				AutomationMaster.Arbiter.SendCommunication(consoleMessage.message);

			}

			#if UNITY_EDITOR
			SaveLogEntry(consoleMessage.message);
			#endif

		}

		private static void PauseValueChanged() {

			if(!Paused) {

				messageListDisplayed.AddRange(messageListQueued);
				messageListQueued = new List<AutoConsoleMessage>();

			}

		}

		public static void SaveConsoleMessagesForAppPlay() {

			//If app is not playing, save console messages, which will be lost when starting the app.
			if(Application.isEditor && !Application.isPlaying) {

			    ConsoleLog = string.Format("{0}/console_before_start.txt", FileBroker.CONSOLE_LOG_DIRECTORY);

				//Overwrite the file.
				FileBroker.SaveUnboundFile(ConsoleLog, string.Empty);

				for(int x = 0; x < messageListDisplayed.Count; x++) {

					FileBroker.SaveUnboundFile(ConsoleLog, messageListDisplayed[x].message, false);

				}

			}

		}

		public static void GetConsoleMessagesAfterAppPlay() {

			//If app is not playing, save console messages, which will be lost when starting the app.
			if(Application.isEditor) {

				if(string.IsNullOrEmpty(ConsoleLog)) {

					ConsoleLog = string.Format("{0}/console_before_start.txt", FileBroker.CONSOLE_LOG_DIRECTORY);

				}

				//Iverwrite the file.
				List<string> Messages = FileBroker.ReadUnboundFile(ConsoleLog).ToList();

				for(int x = 0; x < Messages.Count; x++) {

					AutoConsoleMessage message = new AutoConsoleMessage(Messages[x], MessageLevel.Abridged, ConsoleMessageType.TestRunnerUpdate);
					messageListDisplayed.Add(message);

				}

			}

		}

		private static void SaveLogEntry(string consoleMessage) {

			#if UNITY_EDITOR
			string expectedFileName = string.Format("ConsoleLog{0}{1}{2}.txt", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
			FileBroker.SaveConsoleFile(expectedFileName, consoleMessage, false);
			#endif

		}

		public static Color32 MessageColor(ConsoleMessageType consoleMessageType) {

			switch(consoleMessageType) {
			case ConsoleMessageType.AssertionFail:
				return new Color32(255, 0, 0, 255);
			case ConsoleMessageType.AssertionIgnore:
				return new Color32(0, 210, 225, 255);
			case ConsoleMessageType.AssertionSkip:
				return new Color32(225, 180, 0, 255);
			case ConsoleMessageType.AssertionPass:
				return new Color32(0, 140, 20, 255);
			case ConsoleMessageType.FileSaved:
				return new Color32(50, 200, 255, 255);
			case ConsoleMessageType.TestRunnerUpdate:
			default:
				#if UNITY_EDITOR
				return Color.black;
				#else
				return Color.blue;
				#endif
			}

		}

		#region Unity Log Handling

		/// <summary>
		/// Return list of most recent logs. Stores a maximum of 25 most recent logs.
		/// </summary>
		/// <returns>The latest logs.</returns>
		/// <param name="numberOfMostRecentLogsToReturn">Number of most recent logs to return.</param>
		public static string ReturnLatestLogs(int numberOfMostRecentLogsToReturn) {
			
			List<Log> returnList = new List<Log>();
			if(Logs.Count <= numberOfMostRecentLogsToReturn) {
				
				returnList = Logs;

			} else {
				
				for(int i = 1; i <= numberOfMostRecentLogsToReturn; i++) {
					
					if(i < 0) {
						
						break;

					}

					Log thisLog = Logs[Logs.Count - i];
					if(!string.IsNullOrEmpty(thisLog.message)) {
						
						returnList.Add(thisLog);

					} else {
						
						Logs.RemoveAt(Logs.Count - i);
						i--;

					}

				}

			}
			StringBuilder logString = new StringBuilder();
			for(int i = 0; i < returnList.Count; i++) {
				
				logString.AppendLine(string.Format("(LOG ENTRY) MESSAGE: {0} ** STACKTRACE: {1} ** TYPE: {2}", returnList[i].message, returnList[i].stackTrace, returnList[i].type.ToString()));

			}
			if(logString.Length > MAX_LOG_RETURN_LENGTH) {
				
				return AutomationReport.EncodeCharactersForJson(logString.ToString().Substring(0, MAX_LOG_RETURN_LENGTH)).Replace(AutomationMaster.DELIMITER.ToString(), "%7C"); //Encode AutomationMaster.DELIMITER character or errors will occur in data parsing in server.

			} else {
				
				return AutomationReport.EncodeCharactersForJson(logString.ToString()).Replace(AutomationMaster.DELIMITER.ToString(), "%7C"); //Encode AutomationMaster.DELIMITER character or errors will occur in data parsing in server.

			}

		}

		//Record logs from Unity console.
		public static void GetLog(string message, string stackTrace, LogType type) {

			Log newLog = new Log() {
				message = message,
				stackTrace = stackTrace,
				type = type,
			};
			Logs.Add(newLog);
			//If we exceed the maximum storage space.
			if(Logs.Count >= MAX_LOG_COUNT_HISTORY) {
				
				Logs.RemoveAt(0); //Remove oldest log.

			}

			//If an unhandled exception has occurred in the Trilleon Framework during a test run, handle it!
			if(type == LogType.Exception) {

				#if UNITY_EDITOR
				EditorApplication.UnlockReloadAssemblies();
				#endif

				//If this error occurred directly in the Trilleon Framework (not editor), then stop all tests and report on the exception.
				if(AutomationMaster.Busy &&  AutomationMaster.Initialized && stackTrace.Contains("TrilleonAutomation.") && !stackTrace.Contains("/Editor/") && !ReportOnce) {

					ReportOnce = true;
					AutomationMaster.Arbiter.SendCommunication("An unhandled exception occurred in the Trilleon Framework, disrupting the test run execution");
					string stack = string.Format("The unhandled exception was: {0} {1}", message, stackTrace);
					if(stack.Length > ConnectionStrategy.MaxMessageLength) {
						
						stack = stack.Substring(0, ConnectionStrategy.MaxMessageLength - 50);

					}
					AutomationMaster.Arbiter.SendCommunication(stack);
					string assertionData = string.Join("**", AutomationMaster.CurrentTestContext.Assertions.ToArray());
					if(assertionData.Length > ConnectionStrategy.MaxMessageLength) {

						int startingIndex = assertionData.Length - ConnectionStrategy.MaxMessageLength - 50;
						assertionData = stack.Substring(startingIndex, stack.Length - startingIndex - 1);

					}
					AutomationMaster.Arbiter.SendCommunication(string.Format("ASSERTION DATA:[{0}]", assertionData));
					AutomationMaster.AutomationReport.ReportUnhandledException(message, stackTrace);

					#if UNITY_EDITOR
					//Reset test runner
					GameObject helper = GameObject.Find(TestMonitorHelpers.NAME);
					if(helper != null)
						AutomationMaster.Destroy(helper);
					#endif

					AutomationMaster.Destroy(AutomationMaster.StaticSelf);
					AutomationMaster.Initialize();
					AutomationMaster.StaticSelfComponent.ResetTestRunner();
					AutoConsole.PostMessage("Exception in framework killed TestRunner. Framework reset and ready for new commands.", MessageLevel.Abridged);

					AutomationMaster.StaticSelfComponent.StartCoroutine(AutomationMaster.StaticSelfComponent.TakeScreenshot(false, "FATAL_EXCEPTION"));
					AutomationMaster.StaticSelfComponent.StartCoroutine(AutomationMaster.StaticSelfComponent.ShutDownApplicationAfterFatalException());

				} else if(AutomationMaster.Busy && AutomationMaster.Initialized && !stackTrace.Contains("/Editor/")) {

					AutomationMaster.TestRunContext.Exceptions.Add(newLog);

				}

				#if UNITY_EDITOR
				if(!AutomationMaster.Initialized && stackTrace.Contains("TrilleonAutomation.") && message.ToLower().Contains("object reference")) {

					//Without AutomationMaster.Initialize(), a null reference error will occur trying to use most functionality in the Trilleon framework.
					string missingRefError = "Object reference error in Trilleon framework without initialization. This is most often caused when TrilleonAutomation.Initialize() is not called. This method is required in your Game's startup logic to activate Trilleon.";
					Debug.LogError(missingRefError);
					AutoConsole.PostMessage(missingRefError, MessageLevel.Abridged);

				}
				#endif

			}

		}

		public struct Log {
			public string message;
			public string stackTrace;
			public LogType type;
		}

		#endregion

	}

}
