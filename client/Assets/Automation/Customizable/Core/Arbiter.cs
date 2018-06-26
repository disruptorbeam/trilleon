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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TrilleonAutomation {

    /* 
     * CUSTOMIZE:
     * IMPORTANT! While the Arbiter class is very customizable, you should not change logic used in the HandleMessage() method unless you are A) Adding new commands, or B) Know exactly what you are doing.
     * The existing commands are critical for proper functionality between a CI server and the client application. Some commands are even required for non-server (ex: local editor) automation test runs.
    */
    public class Arbiter : MonoBehaviour {

        public const string DEVICE_IDENTIFIER_PREFIX = "Trilleon-Automation-";
        public static bool LocalRunLaunch { get; set; }

        string lastMessageReceived = string.Empty;

        public string GridIdentity { get; set; }
        public string BuddyIdentity { get; set; }
        public string TestRunId { get; set; }
        public string DeviceUdid { get; set; }
        public DateTime LastMessage { get; set; }

        void Start() {

            TestRunId = string.Empty;
            DeviceUdid = string.Empty; //CANNOT USE: SystemInfo.deviceUniqueIdentifier - Causes requirement for new device permission. Find alternative.
            LastMessage = DateTime.Now;
            GridIdentity = GetDeviceIdentity();

        }

        public string ReturnDetails() {

            return string.Format("Grid Identity: {0} \nBuddy Identity: {1} \nDeviceUDID: {2} \nTestRunID: {3} \nProject: {4}\nAutomation_Running: {5}",
                GridIdentity, BuddyIdentity, DeviceUdid, TestRunId, GameMaster.GAME_NAME, AutomationMaster.Busy ? "True" : "False");

        }

        public void SendCommunication(string paramKey, string paramValue) {

            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>(paramKey, paramValue));
            SendCommunicationActual(parameters);

        }

        public void SendCommunication(List<KeyValuePair<string, string>> parameters) {

            SendCommunicationActual(parameters);

        }

        public void SendCommunication(string message) {

            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Message", message));
            SendCommunicationActual(parameters);

        }

        void SendCommunicationActual(List<KeyValuePair<string, string>> parameters) {

            //Generally no need to communicate with a server if we are in the Editor. Else, just comment out.
            if(!Application.isEditor || AutomationMaster.ConfigReader.GetBool("SEND_COMMUNICATIONS_IN_EDITOR")) {

                StringBuilder json = new StringBuilder();
                json.Append("[");

                //--Required Identifiers--
                json.Append(string.Format("{{\"grid_identity\":\"{0}\"}},", GridIdentity));
                json.Append(string.Format("{{\"grid_identity_buddy\":\"{0}\"}},", BuddyHandler.BuddyName));
                json.Append("{{\"grid_source\":\"client\"}},");
                json.Append(string.Format("{{\"test_run_id\":\"{0}\"}},", TestRunId));
                json.Append(string.Format("{{\"device_udid\":\"{0}\"}},", DeviceUdid));
                json.Append(string.Format("{{\"test_run_time\":\"{0}\"}},", AutomationMaster.Busy ? AutomationReport.TestSuiteRunTime.ToString() : "0 "));
                json.Append(string.Format("{{\"game\":\"{0}\"}},", GameMaster.GAME_NAME));
                //--End Required Params--

                for(int x = 0; x < parameters.Count; x++) {

                    json.Append(string.Format("{{\"{0}\":\"{1}\"}}", parameters[x].Key.ToLower(), parameters[x].Value.Replace("\"", "@APOS@").Replace("'", "@QUOT@")));
                    if(x + 1 < parameters.Count) {

                        json.Append(",");

                    }

                }
                json.Append("]");
                ConnectionStrategy.SendCommunication(json.ToString());
                AutoConsole.PostMessage(json.ToString(), ConsoleMessageType.Pubsub);

            }

        }

        /// <summary>
        /// Handles incoming pubsub messages. Expects JSON format.
        /// </summary>
        public IEnumerator HandleMessage(string result) {

            //Ignore duplicate or empty messages. Ignore messages not meant for this client.
            if(lastMessageReceived == result || string.IsNullOrEmpty(result.Trim())) {

                yield break;

            }

            lastMessageReceived = result;

            List<KeyValuePair<string, string>> parameters = DeserializeJsonString(result);

            if(!LocalRunLaunch) {

                //If no context or identity is provided, then this is not a valid command. If the DeviceUdid is not valid, then ignore the command.
                if(!parameters.FindAll(x => x.Key.ToLower() == "grid_source").Any() || !parameters.FindAll(x => x.Key.ToLower() == "grid_identity").Any()) {

                    yield break;

                }

                string source = parameters.Find(x => x.Key == "grid_source").Value;
                string identity = parameters.Find(x => x.Key == "grid_identity").Value;
                string buddy = parameters.Find(x => x.Key == "grid_identity_buddy").Value;

                //If message simply contains no reference to this GridIdentity OR the identity is self, then it is chatter and can be ignored.
                bool isChatter = !result.Contains(GridIdentity);
                bool isInvalid = string.IsNullOrEmpty(TestRunId) ? !parameters.FindAll(x => x.Key == "set_test_run_id").Any() : parameters.FindAll(x => x.Key == "test_run_id").Any() && TestRunId != parameters.Find(x => x.Key == "test_run_id").Value;
                bool isEcho = identity == GridIdentity && source == "client";

                //If message is from a client source where the identity is not that of this client, but contains this client's identity, then this it is a BuddySystem message.
                bool isBuddyMessage = source != "server" && parameters.FindAll(x => x.Key.StartsWith("buddy_")).Any() && buddy == GridIdentity && identity == BuddyHandler.BuddyName;

                //If this message is meant for a different client, or is an echo from the current client, simply ignore the message.
                if(!isBuddyMessage && (isChatter || isEcho || isInvalid)) {

                    yield break;

                }

            } else if(!LocalRunLaunch && parameters.FindAll(x => x.Key.ToLower() == "grid_identity").Any() ? parameters.Find(x => x.Key == "grid_identity").Value != GridIdentity : false) {

                yield break;

            }

            LastMessage = DateTime.Now;

            if(parameters.Count > 0) {

                //Process each command.
                for(int c = 0; c < parameters.Count; c++) {

                    string command = parameters[c].Key.ToLower();
                    string message = parameters[c].Value.TrimEnd(',');
                    bool isRecognizedCommand = true;

                    switch(command.ToLower()) {
                        case "change_connection_strategy":
                            AutomationMaster.ConnectionStrategy.ChangeConnectionStrategy(message);
                            break;
                        case "change_communications_identity":
                            AutomationMaster.ConnectionStrategy.UpdateChannelIdentity(message);
                            break;
                        case "no_interval_screenshots":
                            AutomationMaster.NoIntervalScreenshots = true;
                            break;
                        case "ignore_memory_tracking":
                            AutomationMaster.IgnoreMemoryTracking = true;
                            break;
                        case "health_check":
                            SendCommunication(string.Format("heartbeat_{0}", (++AutomationMaster.HeartBeatIndex).ToString(), "0"));
                            break;
                        case "buddy_ignore_all":
                            AutomationMaster.IgnoreAllBuddyTests = true;
                            AutomationMaster.LockIgnoreBuddyTestsFlag = true; //Prevents Test editor window from implicitly updating the Ignore flag.
                            break;
                        case "buddy_ready_for_tests":
                            //TODO: Refactor and re-add GRIDLOCK logic. Without it, BuddySystem will not informatively report that both buddies are reporting as the same role.
                            //if((BuddyHandler.IsPrimary && message == "primary") || (!BuddyHandler.IsPrimary && message == "secondary")) {

                            //Gridlock. One client must be the primary, and one must be the secondary.
                            //SendCommunication("buddy_gridlock_detected", "0");
                            //BuddyHandler.RoleGridLock = true;

                            //} else {

                            SendCommunication("buddy_ready_for_tests_acknowledged", BuddyHandler.IsPrimary ? "primary" : "secondary");
                            BuddyHandler.IsBuddyReadyForBuddyTests = true;

                            //}
                            break;
                        case "buddy_ready_for_tests_acknowledged":
                            BuddyHandler.HasBuddyAcknowledgedOurReadiness = true;
                            break;
                        case "buddy_switching_roles":
                            BuddyHandler.BuddyHasSuccessfullySwitchRoles = true;
                            break;
                        case "buddy_requesting_required_details":
                            //Send/Resend details required by Primary Buddy.
                            BuddyHandler.SendBasicBuddyDetails();
                            break;
                        case "buddy_starting_reaction":
                            BuddyHandler.SecondaryReactionsStarted = true;
                            break;
                        case "buddy_tearing_down":
                            BuddyHandler.BuddyTearingDown = true;
                            break;
                        case "buddy_data_update":
                            BuddyHandler.SetCurrentBuddyRequiredDetails(message);
                            break;
                        case "buddy_primary_test_complete":
                            BuddyHandler.CurrentPrimaryTest = message;
                            BuddyHandler.ReadyForReactionTests = true;
                            BuddyHandler.SendBuddyCommunication("buddy_xyz", string.Format("Buddy Primary Test Completion ({0}) Acknowledged ({1}) %%%%", BuddyHandler.CurrentPrimaryTest, BuddyHandler.ReadyForReactionTests));
                            break;
                        case "buddy_primary_pretest_commands":
                            AutomationMaster.BuddyHandler.PreTestCommandReceived(message);
                            break;
                        case "buddy_secondary_pretest_commands_complete":
                            BuddyHandler.BuddyProcessingCommands = false;
                            break;
                        case "buddy_secondary_pretest_commands_failure":
                            BuddyHandler.BuddyCommandExecutionFailure = true;
                            BuddyHandler.BuddyProcessingCommands = false;
                            BuddyHandler.BuddyCommandFailure = message;
                            break;
                        case "buddy_secondary_tests_complete":
                            BuddyHandler.WaitingForBuddyToCompleteReactionTests = false;
                            break;
                        case "buddy_primary_test_failed":
                            BuddyHandler.PrimaryFailed = true;
                            break;
                        case "buddy_primary_complete_action_tests":
                            BuddyHandler.IsPrimaryFinishedWithActionTests = true;
                            break;
                        case "loop_tests":
                            //This command should be sent before or at the same time as the run command. Sending it after may result in failing to have the desired effect.
                            List<KeyValuePair<string, int>> loopTests = new List<KeyValuePair<string, int>>();
                            List<string> RawRequests = message.Split(AutomationMaster.DELIMITER).ToList();
                            for(int x = 0; x < RawRequests.Count; x++) {

                                string testName = RawRequests[x].Split('@').First();
                                string count = RawRequests[x].Split('@').Last();
                                if(RawRequests[x].Split('@').ToList().Count != 2 || count.ToInt() == 0) {

                                    AutoConsole.PostMessage("Provided loop_tests command is invalid. The value must be a string and then integer, separated by an @ symbol.");
                                    continue;

                                }
                                loopTests.Add(new KeyValuePair<string, int>(testName, count.ToInt()));

                            }
                            AutomationMaster.LoopTests = loopTests;
                            break;
                        case "request_response":
                            switch(message) {
                                case "screenshot":
                                    AutomationMaster.AwaitingScreenshot = false;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "request_buddy":
                            //AutomationMaster.BuddyRequest(message, "newbuddy");
                            break;
                        case "set_test_run_id":
                            TestRunId = message;
                            break;
                        case "manual_set_buddy_primary":
                            BuddyHandler.BuddyName = message;
                            BuddyHandler.IsPrimary = true;
                            BuddyHandler.SendBasicBuddyDetails();
                            BuddyIdentity = message;
                            break;
                        case "manual_set_buddy_secondary":
                            BuddyHandler.BuddyName = message;
                            BuddyHandler.IsPrimary = false;
                            BuddyHandler.SendBasicBuddyDetails();
                            BuddyIdentity = message;
                            break;
                        case "no_test_rails_reporting":
                            AutomationReport.IgnoreTestRailsReporting = true;
                            break;
                        case "server_heartbeat":
                            AutomationMaster.ServerHeartbeatReceived();
                            break;
                        case "console_command":
                            List<string> commands = message.Trim().Split('|').ToList();
                            for(int co = 0; co < commands.Count; co++) {

                                string com = string.Format("{0} {1}", commands[co].Split('$').First(), commands[co].Split('$').Last());
                                Q.SendConsoleCommand(com);
                                AutoConsole.PostMessage(string.Format("Ran Command: {0}", com), MessageLevel.Abridged);

                            }
                            break;
                        case "server_broker_response":
                            Q.request.CommandResponseReceived(message);
                            break;
                        case "automation_command":
                            if(AutomationMaster.Busy) {

                                SendCommunication("Notification", "Busy completing previous test run.");
                                break;

                            }
                            SendCommunication("Notification", "Beginning pre-run checks.");

                            if(parameters.Find(x => x.Key == "grid_source").Value == "server") {

                                AutomationMaster.IsServerListening = true;

                            }

                            //Incoming server-based runs will require the carrot before "rt all", for example, to run unit tests instead of automation.
                            if(message.StartsWith("^")) {

                                AutomationMaster.UnitTestMode = true;

                            }

                            yield return StartCoroutine(Q.driver.WaitRealTime(1));
                            AutomationMaster.Busy = true;
                            AutomationMaster.LockIgnoreBuddyTestsFlag = true;

                            //Split string and discard only command prefix. Also allows for spaces in test Category names.
                            message = message.TrimStart().TrimEnd().Replace(", ", ",").Split(new char[] { ' ' }, 2)[1].Trim().ToLower();
                            if(message == "all") {

                                AutomationMaster.LaunchType = LaunchType.All;

                            } else if(message.StartsWith("*") && message.Contains(",")) {

                                message = message.Replace("*", string.Empty);
                                AutomationMaster.LaunchType = LaunchType.MultipleMethodNames;

                            } else if(message.StartsWith("*")) {

                                message = message.Replace("*", string.Empty);
                                AutomationMaster.LaunchType = LaunchType.MethodName;

                            } else if(message.StartsWith("&&")) {

                                message = message.Replace("&&", string.Empty);
                                AutomationMaster.LaunchType = LaunchType.Mix;

                            } else if(message.Contains(",")) {

                                AutomationMaster.LaunchType = LaunchType.MultipleCategoryNames;

                            } else {

                                AutomationMaster.LaunchType = LaunchType.CategoryName;

                            }

                            //Wait until loading of game is complete to attempt a launch of the automation suite
                            yield return StartCoroutine(Q.game.WaitForGameLoadingComplete());
                            StartCoroutine(AutomationMaster.StaticSelfComponent.BeginTestLaunch(message));

                            break;
                        case "buddy_secondary_test_complete":
                        case "buddy_requesting_value_ready":
                        case "buddy_setting_ready_to":
                            //Commands that do not require any action, but should be considered valid for logging purposes (isRecognizedCommand).
                            break;
                        default:
                            isRecognizedCommand = false;
                            break;
                    }

                    Arbiter.LocalRunLaunch = false;
                    if(isRecognizedCommand && !string.IsNullOrEmpty(message)) {

                        AutoConsole.PostMessage(string.Format("SENDER [{0}] - COMMAND [{1}] - MESSAGE [{2}]", parameters.Find(x => x.Key == "grid_identity").Value, command, message), ConsoleMessageType.Pubsub);

                    }

                }

            }

        }

        /*
         * CUSTOMIZE: Generates a reasonably predictable name for devices that will be unique as long as multiple devices of the same exact model and software version are utilized at the same time by server automation runs.
         * IMPORTANT! If you need a unique identifier under the above circumstances, you will need to customize this logic. Recommend you do NOT use SystemInfo.deviceUniqueIdentifier, as this will force your Android apps
         * to request permission for the app to access phone calls and personal data, which is a scary permission check that should be avoided if not absolutely necessary.
        */
        private string GetDeviceIdentity() {

            string[] stringSeparators = new string[] { " OS X ", " OS " };
            string os = SystemInfo.operatingSystem;
            if(string.IsNullOrEmpty(os)) {

                return "NO_DEVICE_NAME";

            }
            if(os.ToLower().Contains("windows ")) {

                string raw = os.Split('(')[1];
                os = raw.Split(')')[0];

            } else if(os.ToLower().Replace(" ", string.Empty).Contains("androidos")) {

                string model = SystemInfo.deviceModel.Replace(" ", string.Empty);
                os = os.Replace(" ", string.Empty);
                os = os.ToLower().Replace("androidos", string.Empty);
                os = os.ToLower().Split('/')[0];
                return string.Format("{0}{1}{2}", DEVICE_IDENTIFIER_PREFIX, Q.help.ReturnStringAsAlphaNumericWithExceptions(model), Q.help.ReturnStringAsAlphaNumericWithExceptions(os)).Replace(",", ".");

            } else {

                string[] split = os.Split(stringSeparators, StringSplitOptions.None);
                if(split.Length > 1) {

                    os = split[1];
                }

            }

            return string.Format("{0}{1}{2}", DEVICE_IDENTIFIER_PREFIX, Q.help.ReturnStringAsAlphaNumericWithExceptions(SystemInfo.deviceName), Q.help.ReturnStringAsAlphaNumericWithExceptions(os));

        }

        /// <summary>
        /// Basic deserializer of JSON messages. Designed to avoid extra third party dependencies that weren't ideal to include in the framework.
        /// Also allows for certain cusomizations specific to Trilleon.
        /// </summary>
        public static List<KeyValuePair<string, string>> DeserializeJsonString(string jsonString) {

            if(!jsonString.Contains("}")) {

                return new List<KeyValuePair<string, string>>(); //Not Json

            }
            int index = jsonString.LastIndexOf("}");
            string trimmedResult = index > 0 ? jsonString.Substring(0, index) : jsonString; //Remove trailing details tacked onto the JSON by Pubsub.
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            List<string> JsonAttributes = trimmedResult.Split(new string[] { "},{", "}, {", "} ,{", "} , {" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for(int x = 0; x < JsonAttributes.Count; x++) {

                string[] currentRawString = JsonAttributes[x].Split(':');
                string key = currentRawString.Length > 0 ? currentRawString[0].Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("+", " ") : string.Empty;
                string value = currentRawString.Length > 1 ? currentRawString[1].Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("+", " ") : string.Empty;
                results.Add(new KeyValuePair<string, string>(key.Trim(), value.Trim()));

            }

            return results;

        }

    }

}
