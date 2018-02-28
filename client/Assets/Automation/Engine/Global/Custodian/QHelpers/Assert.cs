using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrilleonAutomation {

	/// <summary>
	/// Assert that a condition is true and end the test if a failure occurs.
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
		public bool isSoft { get; protected set; }

		#endregion

		#region Soft Assert

		/// <summary>
		/// Soft Assert does NOT fail a test. It reports the step taken as a special color-coded assertion step 
		/// It does not affect the success of a test. Used primarily by "Try-based" driver commands.
		/// </summary>
		public Assert Try { 
			get {
				isTry = true;
				return this;
			} 
		}
		public bool isTry { get; protected set; }

		#endregion

		private static DateTime screenshotRequestTime;
		public static float ScreenshotRequestWaitTime { 
			get { 
				float waitTime = 2.5f - (float)Math.Abs(screenshotRequestTime.Subtract(DateTime.UtcNow).TotalSeconds);
				return waitTime > 0 ? waitTime : 0f;
			}
		}

		public static int ConcurrentFailures { get; set; }
		public bool IsFailing { get; set; }

		#region Assert Future Test Failure

		public void MarkTestForAutomaticFailure(FailType failType, Type name, string message) {

			string realName = failType == FailType.Class ? name.Name : name.Name.Split('<', '>')[1];
			if(failType == FailType.Test) {

				if(AutomationMaster.TestRunContext.completedTests.Contains(realName)){

					Q.assert.Fail(string.Format("The test, \"{0}\", was marked for failure using an assertion - but the test was already run!", realName));

				}

			}
			AutomationMaster.AutoSkips.Add(new KeyValuePair<string[], string>(new string[] { failType == FailType.Class ? "class" : "test" , realName}, message) );

		}

		#endregion

		#region Assert Truth

		public void IsTrue(bool b, string message, params int[] testRailsIds) {

			Unifier(b, false, message, FailureContext.Default, testRailsIds);

		}

		public void Pass(string message, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			Unifier(true, false, message, FailureContext.Default, testRailsIds);

		}

		public void Fail(string message, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			Unifier(false, false, message, FailureContext.Default, testRailsIds);

		}

		//Under rare circumstances, it may be necessary to detect a game state that you do not want to report as a full failure, instead opting to automatically skip the current test immediately.
		public void Skip(string message, params int[] testRailsIds) {

			Unifier(false, false, message, FailureContext.Skipped, testRailsIds);

		}

		public void Warn(string message, FailureContext failureContext = FailureContext.Default) {

			AutoConsole.PostMessage(string.Format("{0} {1}", AutomationMaster.WARNING_FLAG, message), MessageLevel.Abridged);
			Unifier(true, false, message, failureContext);

		}

		#endregion

		#region Assert Null

		public void NotNull(GameObject go, string message, bool expectNull = false, params int[] testRailsIds) {

			Unifier(go != null, expectNull, message, FailureContext.Default, testRailsIds);

		}

		public void NotNull(Component c, string message, bool expectNull = false, params int[] testRailsIds) {

			Unifier(c != null, expectNull, message, FailureContext.Default, testRailsIds);

		}

		#endregion

		#region Assert Activeness

		public void IsActiveVisibleAndInteractable(List<Component> cs, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(cs, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(List<Component> cs, string message, bool expectInactive, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(cs), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(List<Component> cs, string message, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(cs), false, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(List<GameObject> gs, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(gs, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(List<GameObject> gs, string message, bool expectInactive, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(gs), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(List<GameObject> gs, string message, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(gs), false, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(Component c, string message, bool expectInactive, bool checkComponents, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(c, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(Component c, string message, bool expectInactive, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(c), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(Component c, string message, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(c), false, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(GameObject g, string message, bool expectInactive, bool checkComponents , params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(g, checkComponents), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(GameObject g, string message, bool expectInactive, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(g), expectInactive, message, FailureContext.Default, testRailsIds);

		}

		public void IsActiveVisibleAndInteractable(GameObject g, string message, params int[] testRailsIds) {

			Unifier(Q.driver.IsActiveVisibleAndInteractable(g), false, message, FailureContext.Default, testRailsIds);

		}

		#endregion

		#region Assert Dirty

		public void IsDirty(Component originalState, object currentState, string message, bool expectDirty = false, FailureContext failureContext = FailureContext.Default, params int[] testRailsIds) {

			throw new UnityException("Not implemented!");

		}

		#endregion

		#region Update Test Context

		public void MarkTestRailsTestCase(bool isSuccess, params int[] testCaseIds) {

			if(!IsFailing) {

				AutomationReport.MarkTestRailsTestCase(isSuccess ? "Passed" : "Failed", testCaseIds);

			}

		}

		public void MarkTestRailsTestCase(bool isSuccess, int testRunId, int testCaseId) {

			if(!IsFailing) {

				AutomationReport.MarkTestRailsTestCase(isSuccess ? "Passed" : "Failed", testRunId, testCaseId);
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
		protected void Unifier(bool b, bool inverse, string message, FailureContext newFailureContext, params int[] testRailsId) {

			//If test was already marked as a failure, and test has flag indicating that it should continue despite failure, ignore.
			if(!AutomationMaster.CurrentTestContext.IsSuccess || (AutomationMaster.TryContinueOnFailure && IsFailing)) {

				isSoft = false; //Reset the soft flag.
				return;

			}

			//If already failing the current test, or no tests have begun yet, do nothing.
			if(IsFailing || !AutomationMaster.CurrentTestContext.IsInitialized) {

				if(AutomationMaster.TestRunContext.fails.tests.KeyValListContainsKey(AutomationMaster.CurrentTestContext.TestName)) {

					//If current method has failed to stop after a failure, ensure it is killed.
					try {

						AutomationMaster.CurrentTestMethod.Stop(); //Kill current test, only if the currently queued test has been initialized.

					} catch { }

				}
				isSoft = false; //Reset the soft flag.
				return;

			}

			_failureContext = newFailureContext;
			if((!b && inverse) || (b && !inverse)) {

				if(isTry) {

					AutomationMaster.CurrentTestContext.AddAssertion(string.Format("**TRY_SUCCESS**{0}", message));
					isTry = false;
					return;

				}
				ConcurrentFailures = 0;
				AutomationMaster.CurrentTestContext.IsSuccess = true;

				if(!string.IsNullOrEmpty(message)) {
					
					AutomationMaster.CurrentTestContext.AddAssertion(message);

				}

			} else {

				if(isTry && !message.Contains("Null GameObject provided to finder method")) {

					AutomationMaster.CurrentTestContext.AddAssertion(string.Format("**TRY_FAIL**{0}", message));
					isTry = false;
					return;

				}

				IsFailing = true;
				bool recordLogDetails = newFailureContext != FailureContext.Skipped;

				SetReflectedTestData();
				string recentLogs = AutomationReport.EncodeCharactersForJson(AutoConsole.ReturnLatestLogs(5));

				if(newFailureContext == FailureContext.Skipped) {

					AutomationMaster.TestRunContext.skipped.Add(AutomationMaster.CurrentTestContext.TestName);

				} else {

					AutomationMaster.TestRunContext.fails.Add(AutomationMaster.CurrentTestContext.TestName, new string[] {
						message,
						recentLogs.ToString(),
						lineNumber
					});

				}
				AutomationMaster.CurrentTestContext.IsSuccess = false;
				AutomationMaster.CurrentTestContext.AddAssertion(message);
				AutomationMaster.CurrentTestContext.ErrorDetails += string.Format("Error Message [{0}] : Test Line [{1}] : Debug Logs [{2}] ", message, string.Format("Line [{0}] Call [{1}]", lineNumber, lineCall), (recordLogDetails ? recentLogs : string.Format("#SKIPPED#{0}", message)));
				if(failureContext != FailureContext.Unbound) {

					//Take screenshot if a failure is not a "Skip" failure(In which case a test does not run at all, and there is no value in taking a screenshot as the current screen has no relevance to the reason it failed).
					AutomationMaster.StaticSelf.GetComponent<AutomationReport>().TakeScreenshot(AutomationMaster.CurrentTestContext.TestName);
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
						Q.assert.Warn("A failure occurred in the TearDownClass logic for  the test \"{0}.{1}\". This fails the last-run test, and may cause other undesirable behavior for downstream test execution.");
						//Will automatically handle the failure of this test.
						break;
					case AutomationMaster.CurrentExecutionContext.TearDown:
						//Will automatically handle the failure of this test.
					case AutomationMaster.CurrentExecutionContext.Test:
						//Will automatically handle the failure of this test.
					default:
						break;
					
				}

				if(AutomationMaster.TryContinueOnFailure && ConcurrentFailures > 5) {

					AutomationMaster.OverrideContinueOnFailureAfterTooManyConcurrentFailures = true;

				}

				//Any FailureContext beyond TestMethod will not have an instantiated test method.
				if(!isSoft && AutomationMaster.OverrideContinueOnFailureAfterTooManyConcurrentFailures ||(!AutomationMaster.TryContinueOnFailure && (_failureContext == FailureContext.TestMethod || _failureContext == FailureContext.Default))) {

					try {

						AutomationMaster.CurrentTestMethod.Stop(); //Kill current test, only if the currently queued test has been initialized.

					} catch { }

				}

				if(!isSoft && AutomationMaster.TryContinueOnFailure) {

					ConcurrentFailures++;

				}

				#if UNITY_EDITOR
				AutomationMaster.PauseEditorOnFailure();
				#endif

			}

			if(testRailsId.Length > 0) {

				AutomationReport.MarkTestRailsTestCase(AutomationMaster.CurrentTestContext.IsSuccess ? "Passed" : "Failed", testRailsId);

			}
			AutoConsole.PostMessage(string.Format("Assert [{0}] |{1}| {2}", AutomationMaster.CurrentTestContext.TestName, AutomationMaster.CurrentTestContext.IsSuccess ? "Success" : "Failure", message), MessageLevel.Verbose, ConsoleMessageType.TestRunnerUpdate);

			isSoft = isTry = false; //Reset the Soft and Try flags.

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