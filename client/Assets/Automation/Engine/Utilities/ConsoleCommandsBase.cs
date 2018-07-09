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

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MiniJSON;

namespace TrilleonAutomation {

    public abstract class ConsoleCommandsBase : MonoBehaviour {

        public const KeyCode TOGGLE_CONSOLE_VISIBLE = KeyCode.BackQuote; //aka "Tilde".

        /// <summary>
        /// Add all valid commands with accompanying logic to launch in region below. Command aliases cannot contains spaces.
        /// </summary>
        public static List<Command> RegisteredCommands = new List<Command> {

			#region Trilleon commands.
			new Command("Runs the provided Test(s), Category(ies) etc.", RunTests,
                new List<KeyValuePair<string,string>> {
                    new KeyValuePair<string,string>("Manifest", "Test(s) or Cateogory(ies) to run. Examples: \"rt all\" would run all tests" +
                        ", \"rt *SomeTest\" would launch a single test, \"rt SomeCategory\" would run a single category. To run multiple," +
                        " begin command in same way, and then provided comma-delimited list.")
                }, "RT", "RunTests"),
            new Command("Posts logs from the Auto Console. Abridged messages are important framework notifications. " +
                "Pubsub messages are only messages sent and received to/from a server. Verbose messages include both important and unimportant messages.", PostAutoConsoleLogs,
                new List<KeyValuePair<string,string>> {
                    new KeyValuePair<string,string>("LogCount", "The max number of most recent logs to post, up to AutoConsole.MAX_LOG_COUNT_HISTORY."),
                    new KeyValuePair<string,string>("MessageLevel", "Expects A for Abridged, V for Verbose, P for Pubsub, or left empty for Abridged."),
                }, "L", "Logs"),
            new Command("Returns automation details relevant to test run context and device identification.", ReturnDetails,
                new List<KeyValuePair<string,string>>(), "AD", "Details"),
            new Command("Change identity that this client responds to in any communications.", ChangeIdentity,
                new List<KeyValuePair<string,string>> {
                    new KeyValuePair<string,string>("NewIdentity", "The new idendity for this client.")
                }, "CI", "ChangeIdentity"),
            new Command("Returns simplified results of tests already run in the current test run (if test run is active).", CurrentTestRunResults,
                new List<KeyValuePair<string,string>>(), "TR", "Results"),
            new Command("Resets all tests as Untested in the provided TestRails test run.", ResetTestRailsTestStatuses,
                new List<KeyValuePair<string,string>> {
                    new KeyValuePair<string,string>("TestRunID", "The id of the test run that contains the tests to be reset.")
                }, "TRRS", "RailsReset"),
            new Command("Validates Trilleon framework (effectively unit tests).", LaunchTrilleonFrameworkValidation,
                new List<KeyValuePair<string,string>>(), "VT", "Validate"),
            new Command("Get whatever socket ports/channels the device is listening on.", GetSockets,
                new List<KeyValuePair<string,string>>(), "SC", "Sockets")
			#endregion

		};


        #region Console Command delegates

        static string GetSockets(List<string> args) {

            #if !UNITY_ANDROID
            StringBuilder sockets = new StringBuilder();
            for(int s = 0; s < SocketConnectionStrategy.Subscriptions.Count; s++) {

                sockets.Append(string.Format("{0}:{1}", SocketConnectionStrategy.Subscriptions[s].Key.Host, SocketConnectionStrategy.Subscriptions[s].Key.Port));
                if(s + 1 != SocketConnectionStrategy.Subscriptions.Count) {
                    
                    sockets.Append(" | ");

                }

            }
            return string.Format("Sockets connected - ({0} total) - {1}", SocketConnectionStrategy.Subscriptions.Count, sockets.ToString());
            #else
            return "Sockets Disabled On Android";
            #endif

        }

		static string ChangeIdentity(List<string> args) {

			AutomationMaster.Arbiter.GridIdentity = args.First();
			return string.Format("Grid Identity changed to \"{0}\"", AutomationMaster.Arbiter.GridIdentity);

		}

		static string LaunchTrilleonFrameworkValidation(List<string> args) {

			Arbiter.LocalRunLaunch = true;
			ConnectionStrategy.ReceiveMessage("[{\"automation_command\": \"rt Trilleon/Validation\"}]");
			return "Validation Started; Request logs to view ongoing results.";

		}

		static string ReturnDetails(List<string> args) {

			return AutomationMaster.Arbiter.ReturnDetails();

		}

		static string RunTests(List<string> args) {

			Arbiter.LocalRunLaunch = true;
			ConnectionStrategy.ReceiveMessage(string.Format("{{ \"automation_command\": \"rt {0}\" }}", string.Join(string.Empty, args.ToArray())));
			return "Test run command received. Please view the Nexus AutoConsole for further logging updates.";

		}

		static string CurrentTestRunResults(List<string> args) {

			if(!AutomationMaster.Busy) {

				return "No test run is currently in progress.";

			} else {

				StringBuilder results = new StringBuilder();
				results.AppendLine("Current Test Run Results:");
				for(int c = 0; c < AutomationMaster.TestRunContext.CompletedTests.Count; c++) {

					string test = AutomationMaster.TestRunContext.CompletedTests[c];
					string status = string.Empty;
					if(AutomationMaster.TestRunContext.Failed.Tests.FindAll(x => x.Key == test).Any()) {

						status = "FAIL";

					} else if(AutomationMaster.TestRunContext.Passed.Tests.FindAll(x => x == test).Any()) {

						status = "PASS";

					} else if(AutomationMaster.TestRunContext.Skipped.Tests.FindAll(x => x == test).Any()) {

						status = "SKIP";

					} else if(AutomationMaster.TestRunContext.Ignored.Tests.FindAll(x => x == test).Any()) {

						status = "IGNORE";

					}
					results.AppendLine(string.Format("{0}  {1}", status, test));

				}
				return results.ToString();

			}

		}

		static string PostAutoConsoleLogs(List<string> args) {

			int log_count = Convert.ToInt32(args.First());
			char message_level = args.Count == 2 ? args.Last()[0] : '\0';
			List<string> LogsRequested = new List<string>();
			List<AutoConsoleMessage> Logs = AutoConsole.messageListDisplayed;
			Logs.Reverse();

			int message_order = 1;
			for(int x = 0; x < Logs.Count && message_order <= log_count; x++) {

				MessageLevel level = MessageLevel.Abridged;
				if(message_level == 'V' || message_level == 'v') {

					level = MessageLevel.Verbose;

				}

				if(message_level == '\0' || (Logs[x].level == level && (Logs[x].messageType == ConsoleMessageType.Pubsub ? message_level == 'P' || message_level == 'p' : true))) {

					LogsRequested.Add(string.Format("{0}: {1}", message_order++, Logs[x].message));

				}

			}

			return string.Join("\n", LogsRequested.ToArray());

		}

		static string ResetTestRailsTestStatuses(List<string> args) {

			if(args.First().ToInt() > 0) {

				TestRailsAPIClient client = new TestRailsAPIClient(GameMaster.BASE_TEST_RAILS_URL);
                if(client == null) {

                    string message = "COULD NOT RESET RAILS! New API Client could not be instantiated.";
                    AutoConsole.PostMessage(message, MessageLevel.Abridged);
                    return message;

                }
				string json = client.SendGet(string.Format("get_tests/{0}", args.First()));
				System.Object jsonObj = Json.Deserialize(json);
				List<System.Object> list = (List<System.Object>)jsonObj;
				StringBuilder jsonUpdates = new StringBuilder();
				jsonUpdates.Append("{\"results\": [");

				for(int i = 0; i < list.Count; i++) {

					Dictionary<string,object> item = (Dictionary<string,object>)list[i];
					int id = item["id"].ToString().ToInt();
					int statusId = item["status_id"].ToString().ToInt();

					//If test is not marked as Blocked.
					if(statusId != 2) {

						jsonUpdates.AppendLine("{");
						jsonUpdates.Append(string.Format("\"test_id\":{0},", id));
						jsonUpdates.Append("\"status_id\":4,");
						jsonUpdates.Append("\"comment\":\"Resetting test for next run.\"");
						jsonUpdates.Append("}");
						if(i + 1 < list.Count) {
							jsonUpdates.Append(",");
						}

					}

				}

				jsonUpdates.Append("]");
				jsonUpdates.Append("}");
				string jsonFinal = jsonUpdates.ToString().Replace("},]", "}]");
				client.SendPost(string.Format("add_results/{0}", args.First()), jsonFinal);

				return "Test statuses set to Untested. Please check the test run in your browser to confirm.";

			} else {

				return "The provided TestRails TestRunID must be a valid integer.";

			}

		}

#endregion

		public static Text ConsoleLog { get; set; }
		private static GameObject console { get; set; }
		private static ConsoleCommandsBase consoleCommands { get; set; }
		private static InputField consoleInput { get; set; }
		private static ScrollRect consoleLogScroll { get; set; }
		private static List<string> rememberedCommands { get; set; }
		private static int currentCommandIndex { get; set; }
		private static bool focused { get; set; }
		private static bool transitionComplete { get; set; }
		private static DateTime lastScroll { get; set; }
		private static int swipe_ups_detected = 0;
		List<KeyValuePair<int,float>> currentTouches = new List<KeyValuePair<int,float>>();

		protected void ImplementBase() {

			if(AutomationMaster.ConfigReader.GetBool("AUTO_COMMAND_CONSOLE_ENABLED")) {

				Initialize();

				List<string> duplicatedAliases = new List<string>();
				//Report any duplicated command aliases. These need to be unique between each command for things to function properly.
				for(int c = 0; c < RegisteredCommands.Count; c++) {

					for(int a = 0; a < RegisteredCommands[c].Aliases.Count; a++) {

						string alias = RegisteredCommands[c].Aliases[a];
						int matches = RegisteredCommands.FindAll(x => x.Aliases.Contains(alias)).Count;

						if(matches > 1 && !duplicatedAliases.Contains(alias)) {

							duplicatedAliases.Add(alias);
							string message = string.Format("More than one command shares the alias \"{0}\". Please rename them so that each alias is unique.", alias);
							UpdateCommandConsoleOutput(message);

						}

					}

				}

			}

		}

		void Update() {

			if(console == null || consoleInput == null) {

				return;

			}

			//Detect three simoultaneous swipe-ups as the trigger to raise the console on devices.
			for(int i = 0; i < Input.touchCount; i++) {

				Touch touch = Input.GetTouch(i);
				if(touch.phase == TouchPhase.Began && !currentTouches.FindAll(x => x.Key == touch.fingerId).Any()) {

					currentTouches.Add(new KeyValuePair<int,float>(touch.fingerId, touch.position.y));

				} else if(touch.phase == TouchPhase.Ended && currentTouches.FindAll(x => x.Key == touch.fingerId).Any()) {

					if(Math.Abs(touch.position.y - currentTouches.Find(x => x.Key == touch.fingerId).Value) > 100) {

						swipe_ups_detected++;
						currentTouches.Remove(currentTouches.Find(x => x.Key == touch.fingerId));

					} 

				}

			}

			//Hide or show console. Make it easier to hide, than show - by reducing simoultaneous swipes required by one.
			if((console.activeSelf && swipe_ups_detected >= 2) || (swipe_ups_detected >= 3 || Input.GetKey(TOGGLE_CONSOLE_VISIBLE) || Input.GetKeyDown(TOGGLE_CONSOLE_VISIBLE)) && transitionComplete) {

				currentTouches = new List<KeyValuePair<int,float>>();
				StartCoroutine(ToggleVisibility());

			}
			swipe_ups_detected = 0; //Reset.

			//Submit command.
			if(focused && consoleInput.text.Length > 1 && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) ||  Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {

				SendCommand(consoleInput.text);

			} else {

				focused = consoleInput.isFocused;

			}

			//Reset the scroll to bottom if someone empties out the inpout field.
			if(consoleInput.text.Trim().Length == 0) {

				currentCommandIndex = rememberedCommands.Count;

			}

			//Cycle up or down previous commands submitted during this session.
			if(focused && Input.GetKey(KeyCode.UpArrow)) {

				consoleCommands.StartCoroutine(consoleCommands.ArrowCommandHistory(true));

			} else if(focused && Input.GetKey(KeyCode.DownArrow)) {

				consoleCommands.StartCoroutine(consoleCommands.ArrowCommandHistory(false));

			}

		}

		/// <summary>
		/// Send string command to console for execution. Can be accessed from the GUI console input, or by programmatically calling this and sending a raw command.
		/// </summary>
		void SendCommandInstance() {

			SendCommand(consoleInput.text);

		}
		public static void SendCommand(string raw) {

			focused = false;
			consoleInput.text = string.Empty;

			//Add this to a list of received commands so that someone can up/down arrow through session history.
			if(rememberedCommands.Last() != raw) {

				//Remove this command if it appears earlier in the list.
				if(rememberedCommands.Contains(raw)) {

					rememberedCommands.Remove(raw);

				}
				//Add this command to the list.
				rememberedCommands.Add(raw);

			}

			List<string> arguments = raw.Trim().Split(' ').ToList();
			string command = arguments.First().ToLower();
			arguments.RemoveAt(0);

			//TODO: Check if VARIABLES SUPPLIED. Add Required/not to variable data in command object.

			if(command == "help" && arguments.Any()) {

				command = arguments.First().ToLower();
				List<Command> matches = RegisteredCommands.FindAll(x => x.Aliases.Contains(command));
				if(matches.Count == 0 || matches.Count > 1) {

					//List Commands
					if(command == "cmds") {

						consoleCommands.StartCoroutine(consoleCommands.TextNotification("help", "Posting Command Manifest:"));
						StringBuilder returnVal = new StringBuilder();
						for(int v = 0; v < RegisteredCommands.Count; v++) {

							returnVal.Append("CMD: ");
							Command comm = RegisteredCommands[v];
							for(int al = 0; al < comm.Aliases.Count; al++) {

								returnVal.Append(comm.Aliases[al]);
								returnVal.Append(" ");

							}
							returnVal.Append("|| VARS: ");
							for(int al = 0; al < comm.Args.Count; al++) {

								returnVal.Append(comm.Args[al].Key);
								returnVal.Append(" ");

							}
							returnVal.AppendLine(string.Empty);

						}
						consoleCommands.StartCoroutine(consoleCommands.TextNotification("help", returnVal.ToString()));

					} else {

						string message = "Unrecognized console command.";
						UpdateCommandConsoleOutput(message);

					}

				} else {

					StringBuilder returnVal = new StringBuilder ();
					returnVal.AppendLine(string.Format("\nPURPOSE:\n{0}\nVARIABLES:", matches.First().Purpose));
					for(int v = 0; v < matches.First().Args.Count; v++) {

						string key = matches.First().Args[v].Key;
						string val = matches.First().Args[v].Value;
						returnVal.AppendLine(string.Format("{0}: {1}", key, val));

					}
					consoleCommands.StartCoroutine(consoleCommands.TextNotification("help", returnVal.ToString()));

				}

			} else if(command == "clear") {

				UpdateCommandConsoleOutput("Cleared!", true);

			} else {

				List<Command> matches = RegisteredCommands.FindAll(x => x.Aliases.Contains(command));
				if(matches.Count == 0) {

					string message = "Unrecognized console command.";
					UpdateCommandConsoleOutput(message);

				} else if(matches.Count > 1) {

					string message = string.Format("More than one command shares the alias \"{0}\". Please rename them so that each alias is unique.", command);
					UpdateCommandConsoleOutput(message);

				} else {

					consoleCommands.StartCoroutine(consoleCommands.TextNotification(raw, matches.First().Run.Invoke(arguments)));

				}

			}

			EventSystem.current.SetSelectedGameObject(consoleInput.gameObject, null); //Auto focus into input on show.
			consoleInput.OnPointerClick(new PointerEventData(EventSystem.current));

		}

		/// <summary>
		/// Add the console to the AutomationCustodian object, and begin watching for commands.
		/// </summary>
		public void Initialize() {

			console = new GameObject("Console", typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
			console.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			console.GetComponent<Canvas>().pixelPerfect = true;

			float inputHeight = Screen.height / 15;

			GameObject input = Instantiate(Resources.Load("CommandInput")) as GameObject;
			consoleInput = input.GetComponent<InputField>();
			Q.driver.FindIn(input, By.Name, "Text").GetComponent<Text>().fontSize = (int)(inputHeight / 2.5f);
			input.transform.SetParent(console.transform);
			RectTransform inputTran = input.GetComponent<RectTransform>();
			inputTran.sizeDelta = new Vector2(Screen.width + 2, inputHeight);
			inputTran.anchorMax = new Vector2(0.5f, 0);
			inputTran.anchorMin = new Vector2(0.5f, 0);
			inputTran.pivot = new Vector2(0.5f, 0.5f);
			inputTran.localPosition = new Vector3(0, ((Screen.height - inputHeight) / 2) * -1, 0);

			//Add button to submit code if we are on devices.
#if(UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			GameObject submit = Instantiate(Resources.Load("SubmitCommand")) as GameObject;
			submit.transform.SetParent(console.transform);
			RectTransform submitTran = submit.GetComponent<RectTransform>();
			submitTran.sizeDelta = new Vector2(Screen.width / 10, inputHeight - 8);
			submitTran.localPosition = new Vector3((Screen.width / 2) - (inputHeight / 2), ((Screen.height - inputHeight) / 2) * -1 - 4, 0);
			Text submitText = Q.driver.FindIn (submit, By.Name, "Text").GetComponent<Text>();
			submitText.text = "Go";
			submitText.fontSize = (int)(inputHeight / 3);
			submit.GetComponent<Button>().onClick.AddListener(SendCommandInstance);
#endif

			GameObject log = Instantiate(Resources.Load("CommandLog")) as GameObject;
			log.transform.SetParent(console.transform);
			RectTransform logTran = log.GetComponent<RectTransform>();
			logTran.sizeDelta = new Vector2(Screen.width,Screen.height / 6f);
			logTran.anchorMax = new Vector2(0.5f, 0);
			logTran.anchorMin = new Vector2(0.5f, 0);
			logTran.pivot = new Vector2(0.5f, 0.5f);
			logTran.localPosition = new Vector3(0,((Screen.height - inputHeight - 40 - logTran.sizeDelta.y) / 2) * -1, 0);
			consoleLogScroll = Q.driver.FindIn(log, By.Name, "Scroll").GetComponent<ScrollRect>();
			ConsoleLog = Q.driver.FindIn(log, By.Name, "Log").GetComponent<Text>();
			ConsoleLog.fontSize = (int)(inputHeight / 4);
			ConsoleLog.text = string.Empty;
			console.transform.SetParent(this.gameObject.transform);
			console.GetComponent<CanvasGroup>().alpha = 0f; //The console should not render by default.
			consoleCommands = this;
			console.SetActive(false);

			List<GameObject> inputChildren = input.transform.GetChildren();
			for(int c = 0; c < inputChildren.Count; c++) {

				RectTransform rect = inputChildren[c].GetComponent<RectTransform>();//.rect.Set(inputTran.sizeDelta.x / 5f, 12f, 0, 0);
				rect.offsetMin = new Vector2((inputTran.sizeDelta.x / 12.5f), 0); //Left, Bottom
				rect.offsetMax = new Vector2(0, -1 * ((inputHeight / 2) - 8)); //Right, Top :: Values are inverse what you would expect logically.

			}

			transitionComplete = true;
			rememberedCommands = new List<string>();
			lastScroll = DateTime.UtcNow;
			UpdateCommandConsoleOutput(string.Format("TRILLEON COMMAND CONSOLE: Enter Command To Begin\n{0}", TOGGLE_CONSOLE_VISIBLE ==  KeyCode.BackQuote ? "Select tilde key ( ` ) to open in game view." : string.Empty));

		}

		/// <summary>
		/// Posts messages from executed commands to the scrollable console above the input.
		/// </summary>
		static void UpdateCommandConsoleOutput(string message, bool clearExisting = false) {

			string timestamp = System.DateTime.UtcNow.ToLongTimeString();
			if(clearExisting) {

				ConsoleLog.text = string.Format("\n{0}: {1}", timestamp, message);

			} else {

				ConsoleLog.text += string.Format("\n{0}: {1}", timestamp, message);

			}
			Canvas.ForceUpdateCanvases();
			consoleLogScroll.verticalNormalizedPosition = 0f;
			Canvas.ForceUpdateCanvases();

		}

		/// <summary>
		/// Sort up and down through history of commands issued during the current session.
		/// </summary>
		IEnumerator ArrowCommandHistory(bool isUp) {

			//Prevents scrolling over multiple history commands at once.
			if(Math.Abs(DateTime.UtcNow.Subtract(lastScroll).TotalMilliseconds) < 250) {

				yield break;

			} else {

				lastScroll = DateTime.UtcNow;

			}

			if(isUp) {

				currentCommandIndex--;

				if(currentCommandIndex >= 0 && currentCommandIndex <= rememberedCommands.Count - 1) {

					consoleInput.text = rememberedCommands[currentCommandIndex];

				} else {

					currentCommandIndex = currentCommandIndex < 0 ? 0 : rememberedCommands.Count;

				}


			} else {

				currentCommandIndex++;

				if(currentCommandIndex <= rememberedCommands.Count - 1 && currentCommandIndex >= 0) {

					consoleInput.text = rememberedCommands[currentCommandIndex];

				} else {

					currentCommandIndex = currentCommandIndex < 0 ? 0: rememberedCommands.Count;
					consoleInput.text = string.Empty;

				}

			}

		}

		IEnumerator ToggleVisibility() {

			consoleInput.text = string.Empty;
			transitionComplete = false;
			CanvasGroup canGroup = console.GetComponent<CanvasGroup>();
			float currentAlpha = canGroup.alpha;
			bool isHide = currentAlpha > 0f;
			for(float x = currentAlpha; isHide ? canGroup.alpha > 0f : canGroup.alpha < 1f ; x += isHide ? -0.125f : 0.125f) {

				canGroup.alpha = x;
				yield return new WaitForEndOfFrame();

			}
			if(!isHide) {

				console.SetActive(true);
				EventSystem.current.SetSelectedGameObject(consoleInput.gameObject, null); //Auto focus into input on show.
				consoleInput.OnPointerClick(new PointerEventData(EventSystem.current));

			} else {

				console.SetActive(false);

			}
			transitionComplete = true;
			yield return null;

		}

		IEnumerator TextNotification(string command, string return_message) {

			consoleInput.text = string.Empty;
			if(command != "help") {

				UpdateCommandConsoleOutput(string.Format("Command [ {0} ] Invoked", command));
				yield return StartCoroutine(Q.driver.WaitRealTime(1f));

			} else if(command.StartsWith("Unrecognized")) {

				UpdateCommandConsoleOutput(command);

			} 

			if(!string.IsNullOrEmpty(return_message)) {

				UpdateCommandConsoleOutput(return_message);

			}
			yield return null;

		}

	}

}
