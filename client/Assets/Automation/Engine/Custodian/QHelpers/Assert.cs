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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrilleonAutomation {

	/// <summary>
	/// Assert that a condition is true and end the test if a failure occurs.
	/// Why are these IEnumerators? Because StopAllCoroutines waits until the end of the current frame to stop. A parent coroutine, such as a TestObject step will continue to execute for 1 frame after an assertion failure. That causes problems and potential exceptions.
	/// </summary>
	public class Assert : MonoBehaviour {

		#region Soft Assert

		/// <summary>
		/// Soft Assert fails the test on an assertion, but does NOT stop the test's execution. 
		/// Useful in preventing errors in later tests due to current test not executing functionality.
		/// Concurrent failures in the same test are ignored when reporting the cause of the test's failure,
		/// but if the failure is for a non-Soft assertion, the test's execution will still be halted.
		/// </summary>
		public Assert Soft { 
			get {
				isSoft = true;
				return this;
			} 
		}
		private bool isSoft;

		#endregion

		#region Soft Assert

		/// <summary>
		/// Quiet prevents an assertion from being listed in the assertion list if it is a success.
		/// Failed assertions still appear in the list. As an example; Say you have a loop that takes 
		/// a group of related game objects, performs an action on each one, and checks each one after 
		/// this action to see if they are still visible to the player. You may not want to see essentially 
		/// the same assertion repeated ad nauseum in youy report. Use this to make that assertion only 
		/// visible if there is a failure.
		/// </summary>
		public Assert Quiet { 
			get {
				quiet = true;
				return this;
			} 
		}
		private bool quiet;

		#endregion

		#region Try

		/// <summary>
		/// Try does NOT fail a test, while Soft assert does (even though both allow a test to continue executing). 
		/// It reports the step taken as a special color-coded assertion step. It does not affect the success of a test. 
		/// It essentially turns an assertion into a special log that merely records that an assertion was checked.
		/// (Used primarily by "Try-based" driver commands)
		/// </summary>
		public Assert Try { 
			get {
				isTry = true;
				return this;
			} 
		}
		private bool isTry;

		#endregion

        /// <summary>
        /// Kills execution of entire test run. Starting with next test execution, the provided message (value) is posted as a "test result" in reports, explaining that all tests are skipped for the provided reason.
        /// </summary>
        public void CriticalTestRunFailure(string reason) {
            
            _critical_Test_Run_Failure = new KeyValuePair<bool, string>(true, reason);

        }
        public static KeyValuePair<bool, string> Critical_Test_Run_Failure {
            get {
                return _critical_Test_Run_Failure;
            }
            set {
                _critical_Test_Run_Failure = value;
            }
        }
        static KeyValuePair<bool, string> _critical_Test_Run_Failure = new KeyValuePair<bool, string>();

		/// <summary>
		/// Setting this to true will tell the Assert class to not kill test execution if any assertion of any variety fails from this point until the current test completes.
		/// Essentially acts identically to TestFlag.TryContinueAfterFail, but only becomes active when you want it to, at some critical point in your test's execution.
		/// </summary>
		public bool MideExecution_MarkTestToTryContinueAfterFail { get; set; }

		private static DateTime screenshotRequestTime;
		public static float ScreenshotRequestWaitTime { 
			get { 
				float waitTime = 2.5f - (float)Math.Abs(screenshotRequestTime.Subtract(DateTime.UtcNow).TotalSeconds);
				return waitTime > 0 ? waitTime : 0f;
			}
		}

		public static int ConcurrentFailures { get; set; }
		public static bool UnitTestStepFailure { get; set; }
		public bool IsFailing { get; set; }

		#region Assert Future Test Failure

		public IEnumerator MarkTestOrClassForFailure(bool isClassType, Type name, string message) {

			string realName = isClassType ? name.Name : name.Name.Split('<', '>')[1];
			AutomationMaster.AutoFails.Add(new KeyValuePair<string[], string>(new string[] { isClassType ? "class" : "test" , realName}, message) );
			if(!isClassType && AutomationMaster.TestRunContext.CompletedTests.Contains(realName)){

				yield return StartCoroutine(Q.assert.Fail(string.Format("The test, \"{0}\", was marked for failure using an assertion - but the test was already run!", realName)));

			}
			yield return null;

		}

		public IEnumerator MarkTestOrClassForSkip(bool isClassType, Type name, string message) {

			string realName = isClassType ? name.Name : name.Name.Split('<', '>')[1];
			AutomationMaster.AutoSkips.Add(new KeyValuePair<string[], string>(new string[] { isClassType ? "class" : "test" , realName}, message) );
			if(!isClassType && AutomationMaster.TestRunContext.CompletedTests.Contains(realName)){

				yield return StartCoroutine(Q.assert.Fail(string.Format("The test, \"{0}\", was marked to be skipped using an assertion - but the test was already run!", realName)));

			}
			yield return null;

		}

		#endregion

		#region Assert Truth

		public IEnumerator IsTrue(bool b, string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(b, false, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator Pass(string message, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(true, false, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator Fail(string message, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(false, false, message, FailureContext.Default, testRailsIds));

		}

		//Under certain circumstances, it may be necessary to detect a game state that you do not want to report as a full failure, instead opting to automatically skip the current test immediately.
		public IEnumerator Skip(string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(false, false, message, FailureContext.Skipped, testRailsIds));

		}

		public IEnumerator Warn(string message, FailureContext failureContext = FailureContext.Default) {

			AutoConsole.PostMessage(string.Format("{0} {1}", AutomationMaster.WARNING_FLAG, message), MessageLevel.Abridged);
			yield return StartCoroutine(Unifier(true, false, message, failureContext));

		}

		#endregion

		#region Assert Null

		public IEnumerator NotNull(GameObject go, string message, bool expectNull = false, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(go != null, expectNull, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator NotNull(Component c, string message, bool expectNull = false, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(c != null, expectNull, message, FailureContext.Default, testRailsIds));

		}

		#endregion

		#region Assert Activeness

		public IEnumerator IsActiveVisibleAndInteractable(List<Component> cs, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(cs, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(List<Component> cs, string message, bool expectInactive, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(cs), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(List<Component> cs, string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(cs), false, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(List<GameObject> gs, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(gs, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(List<GameObject> gs, string message, bool expectInactive, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(gs), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(List<GameObject> gs, string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(gs), false, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(Component c, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(c, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(Component c, string message, bool expectInactive, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(c), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(Component c, string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(c), false, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(GameObject g, string message, bool expectInactive, bool checkComponents , params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(g, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(GameObject g, string message, bool expectInactive, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(g), expectInactive, message, FailureContext.Default, testRailsIds));

		}

		public IEnumerator IsActiveVisibleAndInteractable(GameObject g, string message, params int[] testRailsIds) {

			yield return StartCoroutine(Unifier(Q.driver.IsActiveVisibleAndInteractable(g), false, message, FailureContext.Default, testRailsIds));

		}

		#endregion

		#region Assert Dirty

		public IEnumerator IsDirty(Component originalState, object currentState, string message, bool expectDirty = false, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			throw new UnityException("Not implemented!");

		}

		#endregion

		#region Update Test Context

		public void MarkTestRailsTestCase(bool isSuccess, params int[] testCaseIds) {

			if(!IsFailing) {

				AutomationReport.MarkTestRailsTestCase(isSuccess ? "Passed" : "Failed", testCaseIds);

			}

		}

		/// <summary>
		/// Adds this test to an override list that tells any tests dependeny on this one that they do not have to skip, even if this test fails later in its execution.
		/// </summary>
		public void MarkTestAsRunDependenciesRegardlessOfFailure() {

			AutomationMaster.RunDependenciesOverride.Add(AutomationMaster.CurrentTestContext.TestName);

		}

		#endregion

		#region helpers

		static string lineNumber = string.Empty;
		static string lineCall = string.Empty;
		static string className = string.Empty;
		protected IEnumerator Unifier(bool b, bool inverse, string message, FailureContext newFailureContext, params int[] testRailsId) {

			//If test was already marked as a failure, and test has flag indicating that it should continue despite failure, ignore.
			if(!AutomationMaster.CurrentTestContext.IsSuccess || ((AutomationMaster.TryContinueOnFailure || MideExecution_MarkTestToTryContinueAfterFail )&& IsFailing)) {

				UnitTestStepFailure = isSoft = isTry = quiet = false; //Reset the UnitTestStepFailure, Soft, Try, and Quiet flags.
				yield break;

			}

			//Automatically label this assertion as quiet if the previous assertion is identical to this one.
			quiet = quiet ? true : AutomationMaster.CurrentTestContext.Assertions.Last() == message;
				
			_failureContext = newFailureContext;
			if((!b && inverse) || (b && !inverse)) {

				if(isTry && !quiet) {

					AutomationMaster.CurrentTestContext.AddAssertion(string.Format("**TRY_SUCCESS**{0}", message));
					UnitTestStepFailure = isSoft = isTry = quiet = false; //Reset the UnitTestStepFailure, Soft, Try, and Quiet flags.
					yield break;

				} 

				ConcurrentFailures = 0;
				AutomationMaster.CurrentTestContext.IsSuccess = true;
				if(!string.IsNullOrEmpty(message) && !isTry && !quiet) {
					
					AutomationMaster.CurrentTestContext.AddAssertion(message);

				}

			} else {

				//TODO: UnitTestStepFailure - Determine if an assertion has failed within the context of a TestObject "steps" method. If so, set this to true. Used to disabled certain TestRunner reactive logic, such as screenshots.

				if(isTry) {

					if(!quiet) {
						
						AutomationMaster.CurrentTestContext.AddAssertion(string.Format("**TRY_FAIL**{0}", message));

					}
					UnitTestStepFailure = isSoft = isTry = quiet = false; //Reset the UnitTestStepFailure, Soft, Try, and Quiet flags.
					yield break;

				}

				IsFailing = true;

				string recentLogs = AutomationReport.EncodeCharactersForJson(AutoConsole.ReturnLatestLogs(5));

				if(newFailureContext == FailureContext.Skipped) {

					AutomationMaster.TestRunContext.Skipped.Add(AutomationMaster.CurrentTestContext.TestName);

				} else {

                    SetReflectedTestData();
					AutomationMaster.TestRunContext.Failed.Add(AutomationMaster.CurrentTestContext.TestName, new string[] {
						message,
						recentLogs,
						lineNumber
					});

				}

				AutomationMaster.CurrentTestContext.IsSuccess = false;
				AutomationMaster.CurrentTestContext.AddAssertion(message);

				if(failureContext != FailureContext.Skipped) {

                    AutomationMaster.CurrentTestContext.ErrorDetails += string.Format("Error Message [{0}] : Test Line [{1}] : Debug Logs [{2}] \n\n", message, string.Format("Line [{0}] Call [{1}]", lineNumber, lineCall), recentLogs);
                    AutomationMaster.CurrentTestContext.ErrorDetails += string.Format(" FULL STACK: [{0}]", Environment.StackTrace.Replace(" at", string.Format(" {0} at", AutomationMaster.NEW_LINE_INDICATOR)));
					//Take screenshot if a failure is not a "Skip" failure (In which case a test does not run at all, and there is no value in taking a screenshot as the current screen has no relevance to the reason it failed).
					yield return StartCoroutine(AutomationMaster.StaticSelfComponent.TakeScreenshot());
					screenshotRequestTime = DateTime.UtcNow;

				}

				//Handle errors occurring outside of the context of the current test's execution. Only certain contexts require additional handling over what is offered by default.
				switch(AutomationMaster.ExecutionContext) {
					case AutomationMaster.CurrentExecutionContext.SetUpClass:
						AutomationMaster.AutoSkips.Add(new KeyValuePair<string[], string>(new string[] { "class" , AutomationMaster.CurrentTestContext.ClassName}, string.Format("FAILURE OCCURRED IN SETUPCLASS:", message)) );
						break;
					case AutomationMaster.CurrentExecutionContext.SetUp:
						AutomationMaster.AutoSkips.Add(new KeyValuePair<string[], string>(new string[] { "test" , AutomationMaster.CurrentTestContext.TestName}, string.Format("FAILURE OCCURRED IN SETUP:", message)) );
						break;
					case AutomationMaster.CurrentExecutionContext.TearDownClass:
					    yield return StartCoroutine(Q.assert.Warn(string.Format("A failure occurred in the TearDownClass logic for  the test \"{0}.{1}\". This fails the last-run test, and may cause other undesirable behavior for downstream test execution.", AutomationMaster.CurrentTestContext.ClassName, AutomationMaster.CurrentTestContext.TestName)));
						//Will automatically handle the failure of this test.
						break;
					//case AutomationMaster.CurrentExecutionContext.TearDown:
						//Will automatically handle the failure of this test.
					//case AutomationMaster.CurrentExecutionContext.Test:
						//Will automatically handle the failure of this test.
					
				}

				if((AutomationMaster.TryContinueOnFailure || MideExecution_MarkTestToTryContinueAfterFail) && ConcurrentFailures > 5) {

					AutomationMaster.OverrideContinueOnFailureAfterTooManyConcurrentFailures = true;

				}

				#if UNITY_EDITOR
				AutomationMaster.PauseEditorOnFailure();
				#endif

				//Any FailureContext beyond TestMethod will not have an instantiated test method.
				if(!AutomationMaster.TryContinueOnFailure || failureContext == FailureContext.Skipped) {
					
					if((!isSoft && AutomationMaster.OverrideContinueOnFailureAfterTooManyConcurrentFailures) || (!MideExecution_MarkTestToTryContinueAfterFail && (_failureContext == FailureContext.TestMethod || _failureContext == FailureContext.Default) || failureContext == FailureContext.Skipped)) {
						
						try {
							
							AutomationMaster.CurrentTestMethod.Stop(); //Kill current test, only if the currently queued test has been initialized.
							
						} catch { }
						yield return new WaitForEndOfFrame(); //Allow all Coroutines to be stopped before returning control. In reality, the coroutine calling this will be stopped, so control will never be returned anyway.
						
					}
					
				}

				if(!isSoft && (AutomationMaster.TryContinueOnFailure || MideExecution_MarkTestToTryContinueAfterFail)) {

					ConcurrentFailures++;

				}

			}

			if(testRailsId.Length > 0) {

				AutomationReport.MarkTestRailsTestCase(AutomationMaster.CurrentTestContext.IsSuccess ? "Passed" : "Failed", testRailsId);

			}

			AutoConsole.PostMessage(string.Format("Assert [{0}] |{1}| {2}", AutomationMaster.CurrentTestContext.TestName, AutomationMaster.CurrentTestContext.IsSuccess ? "Success" : "Failure", message), MessageLevel.Verbose, ConsoleMessageType.TestRunnerUpdate);
			UnitTestStepFailure = isSoft = isTry = quiet = false; //Reset the UnitTestStepFailure, Soft, Try, and Quiet flags.
			yield return null;

		}

		public static FailureContext failureContext {
			get { return _failureContext; }
		}
		private static FailureContext _failureContext;

		/// <summary>
		/// Get test name and line number where error occurred.
		/// </summary>
		private static void SetReflectedTestData() {
            
			StackTrace st = new StackTrace(true);
			List<StackFrame> stf = st.GetFrames().ToList();

			if(_failureContext == FailureContext.Default) {

				List<string> reflectedTypes = new List<string>();
				for(int s = 0; s < stf.Count; s++) {

					if(stf[s].GetMethod().ReflectedType != null) {

						reflectedTypes.Add(stf[s].GetMethod().ReflectedType.Name.ToLower());

					}
				}

				if(reflectedTypes.FindAll(x => x.Contains("<setupclass>")).Any()) {

					_failureContext = FailureContext.SetUpClass;

				} else if(reflectedTypes.FindAll(x => x.Contains("<setup>")).Any()) {

					_failureContext = FailureContext.SetUp;

				} else if(reflectedTypes.FindAll(x => x.Contains("<teardownclass>")).Any()) {

					_failureContext = FailureContext.TearDownClass;

				} else if(reflectedTypes.FindAll(x => x.Contains("<teardown>")).Any()) {

					_failureContext = FailureContext.TearDown;

				} else {

					_failureContext = FailureContext.TestMethod;

				}
			}

			for(int i = 0; i < stf.Count; i++) {

				string filename = stf[i].GetFileName();

				if(!string.IsNullOrEmpty(filename)) {

					string[] fragments = filename.Split('/');
					className = fragments[fragments.Length - 1].Replace(".cs", string.Empty);
					Type thisType = Type.GetType(string.Format("TrilleonAutomation.{0}", className));
					if(thisType != null && thisType.GetCustomAttributes(typeof(AutomationClass), false).Length > 0) {

						lineCall = stf[i].ToString();
						lineNumber = stf[i].GetFileLineNumber().ToString();
						return;

					}  

				}

			}

		}

		#endregion

	}

}
