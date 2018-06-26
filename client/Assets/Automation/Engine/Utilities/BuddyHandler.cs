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
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {
      
   public class BuddyHandler : MonoBehaviour {

      public static string BuddyName {
         get { return _buddyName;  }
         set { 
            #if UNITY_EDITOR
            AutomationReport.SaveBuddyHistory(value);
            #endif
            _buddyName = value; 
         }
      }
      private static string _buddyName = string.Empty;

      public List<KeyValuePair<KeyValuePair<string,MethodInfo>,List<KeyValuePair<string,MethodInfo>>>> Buddies {
         get { return _buddies; }
         set { _buddies = value; }
      }
      private List<KeyValuePair<KeyValuePair<string,MethodInfo>,List<KeyValuePair<string,MethodInfo>>>> _buddies = new List<KeyValuePair<KeyValuePair<string,MethodInfo>,List<KeyValuePair<string,MethodInfo>>>>();

      public AutomationMaster Master {
         get { return GetComponent<AutomationMaster>(); }
      }

      public static BuddyCommands BuddyCommands {
         get { 
            if(_buddyCommands == null) {
               _buddyCommands = AutomationMaster.StaticSelf.GetComponent<BuddyCommands>();
            }
            return _buddyCommands; }
      }
      private static BuddyCommands _buddyCommands;

      public static bool WaitingForBuddyToCompleteReactionTests {
         get { return _waitingForBuddyToCompleteReactionTests; }
         set { _waitingForBuddyToCompleteReactionTests = value; }
      }
      public static bool _waitingForBuddyToCompleteReactionTests = true;

      public static bool IsBuddyReadyForBuddyTests {
         get { return _isBuddyReadyForBuddyTests; }
         set { _isBuddyReadyForBuddyTests = value; }
      }
      static bool _isBuddyReadyForBuddyTests = false;

      public static bool HasBuddyAcknowledgedOurReadiness {
         get { return _hasBuddyAcknowledgedOurReadiness; }
         set { _hasBuddyAcknowledgedOurReadiness = value; }
      }
      static bool _hasBuddyAcknowledgedOurReadiness = false;

      public static bool ReadyForReactionTests {
         get { 
            return _readyForReactionTests; 
         }
         set { 
            _readyForReactionTests = value; 
         }
      }
      static bool _readyForReactionTests = false;

      public static bool IsPrimary {
         get { return _isPrimary; }
         set { _isPrimary = value; }
      }
      static bool _isPrimary = false;

      public static bool IsPrimaryFinishedWithActionTests {
         get { return _isPrimaryFinishedWithActionTests; }
         set { _isPrimaryFinishedWithActionTests = value; }
      }
      static bool _isPrimaryFinishedWithActionTests = false;

      public static bool PrimaryFailed {
         get { return _primaryFailed; }
         set { _primaryFailed = value; }
      }
      static bool _primaryFailed = false;

      public static bool BuddyProcessingCommands {
         get { return _buddyProcessingCommands; }
         set { _buddyProcessingCommands = value; }
      }
      static bool _buddyProcessingCommands = false;

      public static bool BuddyCommandExecutionFailure {
         get { return _buddyCommandExecutionFailure; }
         set { _buddyCommandExecutionFailure = value; }
      }
      static bool _buddyCommandExecutionFailure = false;

      public static bool BuddyHasSuccessfullySwitchRoles {
         get { return _buddyHasSuccessfullySwitchRoles; }
         set { _buddyHasSuccessfullySwitchRoles = value; }
      }
      static bool _buddyHasSuccessfullySwitchRoles = false;

      public static bool BuddyTearingDown { get; set; }
      public static bool RoleGridLock { get; set; }
      public static bool SecondaryReactionsStarted { get; set; }

      public static string BuddyCommandFailure {
         get { return _buddyCommandFailure; }
         set { _buddyCommandFailure = value; }
      }
      private static string _buddyCommandFailure = string.Empty;

      public static string CurrentPrimaryTest {
         get { return _currentPrimaryTest; }
         set { _currentPrimaryTest = value; }
      }
      static string _currentPrimaryTest = string.Empty;

      public bool BuddySystemHandlingStarted {
         get { return _buddySystemHandlingStarted; }
         set { _buddySystemHandlingStarted = value; }
      }
      static bool _buddySystemHandlingStarted = false;

      public static string GetValueFromBuddyTestDetails(string key) {
         
         List<KeyValuePair<string,string>> matches = BuddyTestRequiredDetails.FindAll(x => x.Key == key);
         if(matches.Any()) {
            
            return matches.First().Value;

         }
         return string.Empty;

      }

      public static void SendBuddyCommunication(string paramKey, string paramValue) {

         List<KeyValuePair<string,string>> parameters = new List<KeyValuePair<string,string>>();
         parameters.Add(new KeyValuePair<string,string>(paramKey, paramValue));
         parameters.Add(new KeyValuePair<string,string>("buddy_command_flag", "0")); //Declares this as a buddy command, which is critical for the server Jenkins scripts to recognize non-client messages as relevant to its client.
         AutomationMaster.Arbiter.SendCommunication(parameters);

      }

      public static void SendBasicBuddyDetails() {
         
         SendBuddyCommunication("buddy_data_update", GameMaster.BasicBuddyInformation());

      }

      public static void SetCurrentBuddyRequiredDetails(string details) {
         
         if(string.IsNullOrEmpty(details)) {
            
            return;

         }

         string[] detailsEach = details.Split(AutomationMaster.DELIMITER);
         for(int i = 0; i < detailsEach.Length; i++) {
            
            string[] keyVal = detailsEach[i].Split('=');
            if(BuddyTestRequiredDetails.FindAll(x => x.Key == keyVal[0]).Any()) {
               
               BuddyTestRequiredDetails.Remove(BuddyTestRequiredDetails.Find(x => x.Key == keyVal[0]));

            }
            BuddyTestRequiredDetails.Add(new KeyValuePair<string, string>(keyVal[0], keyVal[1]));

         }

      }
      public static List<KeyValuePair<string,string>> BuddyTestRequiredDetails = new List<KeyValuePair<string,string>>();
      public static List<KeyValuePair<string,string>>  SentBuddyTestRequiredDetails = new List<KeyValuePair<string,string>> ();

      public static string GetValueFromBuddyPrimaryTestForSecondaryTest(string expectedKey){
         List<KeyValuePair<string,string>> result = BuddyTestRequiredDetails.FindAll(x => x.Key == expectedKey);
         return result.Any() ? result.First().Value : string.Empty;;
      }

      public List<KeyValuePair<string,MethodInfo>> BuddySystemMarked {
         get { return _buddySystemMarked; }
         set { _buddySystemMarked = value; }
      }
      private List<KeyValuePair<string,MethodInfo>> _buddySystemMarked = new List<KeyValuePair<string,MethodInfo>>();

      public List<string> HandledBuddyTests {
         get { return _handledBuddyTests; }
         set { _handledBuddyTests = value; }
      }
      private List<string> _handledBuddyTests = new List<string>();

      private string _failureReason = string.Empty;
      private bool _isBuddySystemFailure = false;
      private int timeout = 600; 
      private int time = 0;


      void Start() {
         
         gameObject.AddComponent(Type.GetType(string.Format("{0}.BuddyCommands", AutomationMaster.TrilleonNamespace))); //Game-specific BuddySystem commands.

      }

      public static void SendInfoForBuddyTests(string key, string value) {

         if(SentBuddyTestRequiredDetails.FindAll(x => x.Key == key).Any()) {

            SentBuddyTestRequiredDetails.Remove(SentBuddyTestRequiredDetails.Find(x => x.Key == key));

         }
            
         SentBuddyTestRequiredDetails.Add(new KeyValuePair<string,string>(key, value));
         SendBuddyCommunication("buddy_data_update", string.Format("{0}={1}", key, value));
         Q.storage.Add(key, value);

      }

      public static void ResendAllInfoForBuddyTests() {

         StringBuilder message = new StringBuilder();
         for(int m = 0; m < SentBuddyTestRequiredDetails.Count; m++) {

            string pair = string.Format("{0}={1}", SentBuddyTestRequiredDetails[m].Key, SentBuddyTestRequiredDetails[m].Value);
            message.Append(pair);
            if(m + 1 != SentBuddyTestRequiredDetails.Count) {

               message.Append(AutomationMaster.DELIMITER);

            }

         }
         SendBuddyCommunication("buddy_data_update", message.ToString());

      }

      public IEnumerator RunBuddySystemTests() {

         //If a relationship has not yet been established, then cancel BuddySystem execution. This is not used if IgnoreAllBuddyTests flag set.
         BuddySystemHandlingStarted = true;
         _isBuddySystemFailure = string.IsNullOrEmpty(BuddyName);

         if(!AutomationMaster.IgnoreAllBuddyTests) {

            //If allowed, and Buddy is not yet set, then set Buddy to last-used Buddy.
            #if UNITY_EDITOR
            if(AutomationMaster.ConfigReader.GetBool("EDITOR_DEFAULT_BUDDY_TO_LAST")) {
               
               string mostRecentBuddy = AutomationReport.GetMostRecentBuddy();
               if(!string.IsNullOrEmpty(mostRecentBuddy)) {
                  
                  BuddyName = mostRecentBuddy;

               }

            }
            #endif

            if(_isBuddySystemFailure) {
               
               _failureReason = "Buddy was not set before BuddySystem test execution started.";

            }

            if(!_isBuddySystemFailure) {

               AutoConsole.PostMessage("Establishing Connection With Buddy", MessageLevel.Abridged);
               do {

                  if(RoleGridLock) {

                     _isBuddySystemFailure = true;
                     _failureReason = "This client and the associated Buddy client share the same role (primary/secondary). One must be a primary Buddy, and the other a secondary Buddy.";
                     break;

                  }

                  SendBuddyCommunication("buddy_ready_for_tests", IsPrimary ? "primary" : "secondary");
                  yield return StartCoroutine(Q.driver.WaitRealTime(5));
                  time += 5;

               } while((!_isBuddyReadyForBuddyTests || !HasBuddyAcknowledgedOurReadiness) && time <= timeout);
               AutoConsole.PostMessage(time > timeout ? "Buddy Connection Failure": "Buddy Connection Established", MessageLevel.Abridged);

               if(IsPrimary && !BuddyTestRequiredDetails.Any()) {
                  
                  //If this client is the Primary, and has not yet recieved its required information from the Secondary, request it.
                  for(int limit = 30; limit >= 0; limit--) {

                     if(!BuddyTestRequiredDetails.Any()) {
                        
                        break;

                     }

                     SendBuddyCommunication("buddy_requesting_required_details", "0");
                     yield return StartCoroutine(Q.driver.WaitRealTime(1));

                  }

               }

               if(time >= timeout || (IsPrimary && !BuddyTestRequiredDetails.Any())) {
                  
                  _isBuddySystemFailure = true;
                  _failureReason = "Timed out waiting for Buddy to be ready for multi-client testing.";

               }

               if(!_isBuddySystemFailure) {

                  SendBasicBuddyDetails();

                  if(_isPrimary) {

                     yield return StartCoroutine(PrimaryBuddyTestRun());
                     if(!_isBuddySystemFailure) {
                        
                        ResetBuddySystemValues();
                        yield return StartCoroutine(Q.driver.WaitRealTime(5));
                        SendBuddyCommunication("buddy_switching_roles", "0");

                        timeout = 300; 
                        time = 0;
                        while(!BuddyHasSuccessfullySwitchRoles && time <= timeout) {   

                           yield return StartCoroutine(Q.driver.WaitRealTime(5));
                           SendBuddyCommunication("buddy_primary_complete_action_tests", "0");
                           time += 5;

                        }
                        if(time > timeout) {
                           
                           _isBuddySystemFailure = true;
                           _failureReason = "Timed out waiting for Buddy to switch roles from Secondary to Primary.";

                        } else {

                           AutoConsole.PostMessage("Switching Roles With Buddy", MessageLevel.Abridged);
                           yield return StartCoroutine(SecondaryBuddyTestRun());

                        }

                     }

                  } else {

                     yield return StartCoroutine(SecondaryBuddyTestRun());
                     if(!_isBuddySystemFailure) {
                        
                        ResetBuddySystemValues();
                        yield return StartCoroutine(Q.driver.WaitRealTime(5));
                        SendBuddyCommunication("buddy_switching_roles", "0");

                        timeout = 300; 
                        time = 0;
                        while(!BuddyHasSuccessfullySwitchRoles && time <= timeout) {
                           
                           yield return StartCoroutine(Q.driver.WaitRealTime(5));
                           SendBuddyCommunication("buddy_secondary_tests_complete", "0");
                           time += 5;

                        }
                        if(time > timeout) {
                           
                           _isBuddySystemFailure = true;
                           _failureReason = "Timed out waiting for Buddy to switch roles from Primary to Secondary.";

                        } else {
                           
                           AutoConsole.PostMessage("Switching Roles With Buddy", MessageLevel.Abridged);
                           yield return StartCoroutine(PrimaryBuddyTestRun());

                        }

                     }

                  }

                  SendBuddyCommunication("buddy_tearing_down", "0");
                  yield return StartCoroutine(Q.driver.WaitRealTime(5));
                  SendBuddyCommunication("buddy_tearing_down", "0");

               }

            }

         }

         if(_isBuddySystemFailure || AutomationMaster.IgnoreAllBuddyTests) {

            //Fail all remaining tests.
            string errorMessage = string.Format("BuddySystem failure. Reason: {0} Skipping BuddySystem tests.", _failureReason);
            AutoConsole.PostMessage(errorMessage, MessageLevel.Abridged);
            for(int f = 0; f < _buddies.Count; f++) {
               
               if(!HandledBuddyTests.Contains(_buddies[f].Key.Key)) {
                  
                  AutomationMaster.CurrentTestContext = new TestContext();
                  if(!AutomationMaster.Methods.KeyValListContainsKey(_buddies[f].Key.Key)) {

                     AutomationMaster.Methods.Add(new KeyValuePair<string, MethodInfo>(_buddies[f].Key.Key, Buddies[f].Key.Value));

                  }

               }

               yield return StartCoroutine(Master.LaunchSingleTest(_buddies[f].Key, AutomationMaster.Methods.Count - 1, AutomationMaster.IgnoreAllBuddyTests ? TestStatus.Ignore : TestStatus.Fail, errorMessage));
               for(int fr = 0; fr < _buddies[f].Value.Count; fr++) {
                  
                  if(HandledBuddyTests.Contains(Buddies[f].Value[fr].Value.Name)) {
                     
                     continue;

                  }
                  AutomationMaster.CurrentTestContext = new TestContext();
                  if(!AutomationMaster.Methods.KeyValListContainsKey(_buddies[f].Value[fr].Key)) {
                     
                     AutomationMaster.Methods.Add(new KeyValuePair<string, MethodInfo>(_buddies[f].Value[fr].Key, Buddies[f].Value[fr].Value));

                  }
                  yield return StartCoroutine(Master.LaunchSingleTest(_buddies[f].Value[fr], AutomationMaster.Methods.Count - 1, AutomationMaster.IgnoreAllBuddyTests ? TestStatus.Ignore : TestStatus.Fail , errorMessage));

               }

            }

         }

         HandledBuddyTests = new List<string>();
         ResetBuddySystemValues();

         yield return null;

      }

      /// <summary>
      /// While this client is in the Primary Buddy role, run Action tests, and await completion of Secondary Buddy's Reaction tests.
      /// </summary>
      /// <returns>The buddy test run.</returns>
      public IEnumerator PrimaryBuddyTestRun() {

         for(int b = 0; b < _buddies.Count; b++) {

            //Skip this method if it has already been handled.
            if(HandledBuddyTests.Contains(Buddies[b].Key.Key)) {
               
               continue;

            }

            SecondaryReactionsStarted = false; //Reset for next reaction.

            timeout = 180; 
            time = 0;

            //Retrieve any BuddyCommands for this Action test, and send them to Buddy for execution.
            BuddyCommand command = _buddies[b].Key.Value.GetCustomAttributes(typeof(BuddyCommand), false).ToList().First() as BuddyCommand;
            if(command != null) {
               
               SendBuddyCommunication("buddy_primary_pretest_commands", string.Join( "|", command.Commands.ToArray()));
               BuddyProcessingCommands = true;
               while(BuddyProcessingCommands && time <= timeout) {

                  if(BuddyCommandExecutionFailure) {
                     
                     break;

                  }
                  //Every 50 seconds, report still waiting.
                  if(time % 50 == 0) {

                     AutoConsole.PostMessage(string.Format("Primary waiting for Secondary to complete pretest commands ({0}) seconds", time.ToString()), MessageLevel.Abridged);

                  }

                  yield return StartCoroutine(Q.driver.WaitRealTime(5));
                  time += 5;

               }

            }

            string errorMessage = string.Empty;
            HandledBuddyTests.Add(_buddies[b].Key.Key);
            if(!AutomationMaster.Methods.KeyValListContainsKey(Buddies[b].Key.Key)) {
               
               AutomationMaster.Methods.Add(new KeyValuePair<string, MethodInfo>(Buddies[b].Key.Key, Buddies[b].Key.Value));

            }

            if(!BuddyCommandExecutionFailure && time <= timeout) {
               
               //Launch Action test.
               yield return StartCoroutine(Master.LaunchSingleTest(Buddies[b].Key, AutomationMaster.Methods.Count - 1));

            } else {
               
               AutomationMaster.CurrentTestContext.TestInitialize(_buddies[b].Key.Value);
               if(BuddyCommandExecutionFailure) {
                  
                  errorMessage = string.Format("The Secondary Buddy of this client failed to successfully handle the command(s) {0} required by this Action test. Commands: {1}.", BuddyCommandFailure, string.Join("|", command.Commands.ToArray()));

               } else {
                  
                  errorMessage = "Timout occurred waiting for primary buddy to complete its pretest commands.";

               }
					yield return StartCoroutine(Q.assert.Fail(errorMessage, FailureContext.TestMethod));
               AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
               Master.ReportOnTest();

            }
            ResendAllInfoForBuddyTests();
            yield return StartCoroutine(Q.driver.WaitRealTime(2f));

            bool actionSuccessful = AutomationMaster.CurrentTestContext.IsSuccess;
            if(actionSuccessful) {

               SendBuddyCommunication("buddy_primary_test_complete", _buddies[b].Key.Value.Name);

            } else {

               SendBuddyCommunication("buddy_primary_test_failed", _buddies[b].Key.Value.Name);

            }

            //When the buddy has completed all reaction tests, we can then move on to the next.
            timeout = 300; 
            time = 0;
            _waitingForBuddyToCompleteReactionTests = true;
            int resendCount = 4;
            while(_waitingForBuddyToCompleteReactionTests && time <= timeout) {

               //Check if buddy has declared that it is running its secondary tests. If not, resend the command once.
               if(resendCount > 0 && time > 10 && !SecondaryReactionsStarted) {
                  
                  resendCount--;
                  if(actionSuccessful) {

                     SendBuddyCommunication("buddy_primary_test_complete", _buddies[b].Key.Value.Name);

                  } else {

                     SendBuddyCommunication("buddy_primary_test_failed", _buddies[b].Key.Value.Name);

                  }

               }

               //Every 50 seconds, report still waiting.
               if(time % 50 == 0) {

                  AutoConsole.PostMessage(string.Format("Primary waiting for Secondary ({0}) seconds", time.ToString()), MessageLevel.Abridged);

               }

               yield return StartCoroutine(Q.driver.WaitRealTime(5f));
               time += 5;

            }

            //If we time out, stop the run and break out of execution.
            _isBuddySystemFailure = time >= timeout;
            if(_isBuddySystemFailure) {

               _failureReason = errorMessage;
               AutoConsole.PostMessage(_failureReason, MessageLevel.Abridged);
               break;

            }
               
            //Run all CounterReaction tests for this Action.
            List<KeyValuePair<string,MethodInfo>> counterReactions = _buddies.FindAll(x => x.Key.Value.Name == _buddies[b].Key.Value.Name).First().Value.FindAll(x => {
               BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(x.Value, typeof(BuddySystem));
               return bs.buddy == Buddy.CounterReaction;
            });

            for(int c = 0; c < counterReactions.Count; c++) {
               
               HandledBuddyTests.Add(counterReactions[c].Key);
               if(!AutomationMaster.Methods.KeyValListContainsKey(counterReactions[c].Key)) {
                  
                  AutomationMaster.Methods.Add(new KeyValuePair<string,MethodInfo>(counterReactions[c].Key, counterReactions[c].Value));
               
               }
               yield return StartCoroutine(Master.LaunchSingleTest(counterReactions[c], AutomationMaster.Methods.Count - 1, actionSuccessful ? TestStatus.Pass : TestStatus.Fail));
            
            }
               
         }
            
         SendBuddyCommunication("buddy_primary_complete_action_tests", "0");

         yield return null;

      }

      public IEnumerator SecondaryBuddyTestRun() {

         do {

            int timeout = 300; 
            int time = 0;
            while(!ReadyForReactionTests && time <= timeout) {


               if(ReadyForReactionTests || IsPrimaryFinishedWithActionTests || PrimaryFailed || BuddyTearingDown) {

                  break;

               }

               //Every 50 seconds, report still waiting.
               if(time % 50 == 0) {
                  
                  AutoConsole.PostMessage(string.Format("Secondary waiting for Primary ({0}) seconds", time.ToString()), MessageLevel.Abridged);

               }

               yield return StartCoroutine(Q.driver.WaitRealTime(5f));
               time += 5;

            }

            //If there is no primary test, then we are complete the secondary run.
            if(string.IsNullOrEmpty(CurrentPrimaryTest) || BuddyTearingDown) {
               
               break;

            }
               
            SendBuddyCommunication("buddy_starting_reaction", "0");

            List<KeyValuePair<string,MethodInfo>> reactionTests = _buddies.FindAll(x => x.Key.Value.Name == CurrentPrimaryTest).First().Value.FindAll(x => {
               BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(x.Value, typeof(BuddySystem));
               return bs.buddy == Buddy.Reaction;
            });

            for(int r = 0; r < reactionTests.Count; r++) {
               
               if(_primaryFailed) {

                  //Add each test and "start" them with auto fail.
                  if(!AutomationMaster.Methods.KeyValListContainsKey(reactionTests[r].Key)) {
                     
                     AutomationMaster.Methods.Add(new KeyValuePair<string,MethodInfo>(reactionTests[r].Key, reactionTests[r].Value));

                  }
                  string errorMessage = string.Format("This BuddySystem Reaction test was skipped because its primary Action \"{0}\" failed.", CurrentPrimaryTest);
                  yield return StartCoroutine(Master.LaunchSingleTest(reactionTests[r], AutomationMaster.Methods.Count - 1, TestStatus.Fail,  errorMessage));

               } else {
                  
                  if(HandledBuddyTests.Contains(reactionTests[r].Key)) {
                     
                     continue;

                  }
                  HandledBuddyTests.Add(reactionTests[r].Key);
                  if(!AutomationMaster.Methods.KeyValListContainsKey(reactionTests[r].Key)) {
                     
                     AutomationMaster.Methods.Add(new KeyValuePair<string,MethodInfo>(reactionTests[r].Key, reactionTests[r].Value));

                  }
                  yield return StartCoroutine(Master.LaunchSingleTest(reactionTests[r], AutomationMaster.Methods.Count - 1));
                  SendBuddyCommunication("buddy_secondary_test_complete", reactionTests[r].Key);

               }

            }
               
            ReadyForReactionTests = false;
            CurrentPrimaryTest = string.Empty;

            //Alert buddy that reaction tests are complete.
            SendBuddyCommunication("buddy_secondary_tests_complete", "0");
            ResendAllInfoForBuddyTests();

         } while(!_isPrimaryFinishedWithActionTests);


         yield return null;

      }

      /// <summary>
      /// Send command received from Pubnub to IEnumerator handler.
      /// </summary>
      /// <param name="commandsRaw">Commands raw.</param>
      public void PreTestCommandReceived(string commandsRaw){

         StartCoroutine(RunPretestCommand(commandsRaw));

      }

      /// <summary>
      /// Handle a pretest command received through joint Buddy Pubnub channel.
      /// </summary>
      /// <returns>The pretest command.</returns>
      /// <param name="commandsRaw">Commands raw.</param>
      private IEnumerator RunPretestCommand(string commandsRaw){

         yield return StartCoroutine(BuddyCommands.HandleCommand(commandsRaw.Split(AutomationMaster.DELIMITER).ToList()));

         if(string.IsNullOrEmpty(BuddyCommandFailure)) {
            
            SendBuddyCommunication("buddy_secondary_pretest_commands_complete", "0");

         } else {
            
            SendBuddyCommunication("buddy_secondary_pretest_commands_failure", BuddyCommandFailure);

         }

         yield return null;

      }

      //Reset all BuddySystem values to default values. Used when switching between Primary and Secondary runs on the same client.
      private void ResetBuddySystemValues() {
         
         _isPrimary = !_isPrimary; //Set this client to the opposite role it played initially; Primary will now run its Reaction tests, and the Secondary will now run its Action tests.
         _waitingForBuddyToCompleteReactionTests = true;
         _isBuddyReadyForBuddyTests = _readyForReactionTests = _isPrimaryFinishedWithActionTests = _primaryFailed = BuddyCommandExecutionFailure = false;
         _currentPrimaryTest = BuddyCommandFailure = string.Empty;

      }


      //TODO: Incomplete - This logic will allow for dynamic Buddy finding. Tim Sibiski - 12/14/2016

      //static bool _isAwaitingBuddyResponse = false;
      //static bool response_received = false;
      public static List<string> DeclaredClients = new List<string>();
      //static string _latestBuddyResponse = string.Empty;
      //List<KeyValuePair<string,string>> KnownBuddies = new List<KeyValuePair<string,string>>();

      /*
         public IEnumerator FindBuddy() {

            AutomationMaster.arbiter.RequestPresence();
            AutomationMaster.arbiter.GetGlobalHistory();

            while(!DeclaredClients.Any()) {
               yield return StartCoroutine(Q.driver.WaitRealTime(5));
            }

            for(int i = 0; i < DeclaredClients.Count; i++) {
               if(!string.IsNullOrEmpty(buddy)) {
                  break;
               }
               response_received = false;
               _latestBuddyResponse = string.Empty;
               AutomationMaster.arbiter.SendBuddyRequest(DeclaredClients[0]);
               AutomationMaster.arbiter.GetGlobalHistory(); //Update list.

               int timeout = 15;
               int timeWaiting = 0;
               while(!response_received && timeWaiting <= timeout) {

                  yield return StartCoroutine(Q.driver.WaitRealTime(5));

               }

               if(timeWaiting <= timeout) {
                  break; //Move on to next potential buddy.
               }

               //bool relationshipFormed = false;
               string response = _latestBuddyResponse.Split('|')[1];
               if(response == "yes") {
                  //relationshipFormed = BuddyCheck(DeclaredClients[0]);
               } else if(response == "no") {

               }

               //UnsubscribeFromPotentialBuddy on failure

            }
            //See if this client is the client currently at the top of the queue. Wait if not first in line. Cancel this if buddy request is made by another client, and accepted by this one.

            //If/when this client is first in the queue, begin searching for buddy.
            //ParseListOfKnownBuddies
            //Remove known buddies from declared clients to get list of potential buddies.    List<string> PotentiallyAvailableClients = new List<string>();
            //Send request to first potential buddy, and await confirmation for relationship before assigning them as actual buddy.

            yield return null;

         }

         public bool BuddyCheck(string thisClient) {
            if(string.IsNullOrEmpty(buddy)) {
               buddy = thisClient;
               return true;
            } else {

               if(thisClient == buddy) {
                  return true;
               }



            }
            return false;
         }
         */

      /*
      public static void BuddyRequest(string clientName, string request) {

         if(request == "newbuddy" && string.IsNullOrEmpty(buddy)) {
            buddy = clientName;
            SendBuddyCommunication("requestbuddy_response|yes");
            response_received = true;
         } else if(request == "newbuddy") {
            SendBuddyCommunication("requestbuddy_response|no");
            response_received = true;
         } else if(request == "statusupdate") {
            SendBuddyCommunication(string.Format("requestbuddy_status_response|{0}", _isReadyForBuddyTests ?  "ready" : "notready" ));
         }

      }

      public static void BuddyRequestReceived(string request) {

         _latestBuddyResponse = request;
         response_received = true;

      }

      public IEnumerator GetBuddyStatus() {

         //_isAwaitingBuddyResponse = true;
         SendBuddyCommunication(string.Format("requestbuddy_status|status"));

         yield return null;

      }
      */


   }

}
