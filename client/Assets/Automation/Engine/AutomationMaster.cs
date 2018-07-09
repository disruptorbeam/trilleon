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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrilleonAutomation {

	public delegate void OnNewTestLaunch();

	[DisallowMultipleComponent]
	public class AutomationMaster : MonoBehaviour {

		public const string AUTOMATION_CUSTODIAN_NAME = "AutomationCustodian";
		public const string WARNING_FLAG = "--WARNING--";
		public const char DELIMITER = '|';
		public const string PARTIAL_DELIMITER = "$$$";
		public const string NEW_LINE_INDICATOR = "^^^";
		public const int SERVER_HEARTBEAT_TIMEOUT = 60;

		#region Self Data

		public static GameObject StaticSelf {
			get { 
				if(_staticSelf == null) {
					_staticSelf = GameObject.Find(AUTOMATION_CUSTODIAN_NAME);
				}
				return _staticSelf;
			}
		}
		private static GameObject _staticSelf;

		public static AutomationMaster StaticSelfComponent {
			get { 
				if(_staticSelfComponent == null) {
					if(StaticSelf != null) {
						_staticSelfComponent = StaticSelf.GetComponent<AutomationMaster>();
					}
				}
				return _staticSelfComponent;
			}
		}
		private static AutomationMaster _staticSelfComponent;

		public static string TrilleonNamespace {
			get { 
				if(string.IsNullOrEmpty(_trilleonNamespace)) {
					_trilleonNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
				}
				return _trilleonNamespace;
			}
		}
		private static string _trilleonNamespace;

		public static Arbiter Arbiter {
			get { 
				if(_arbiter == null) {
					_arbiter = StaticSelf.GetComponent<Arbiter>();
				}
				return _arbiter; 
			}
			set { _arbiter = value; }
		}
		private static Arbiter _arbiter;

		public static ConnectionStrategy ConnectionStrategy {
			get { 
				if(_connectionStrategy == null) {
					_connectionStrategy = StaticSelf.GetComponent<ConnectionStrategy>();
				}
				return _connectionStrategy; 
			}
			set { _connectionStrategy = value; }
		}
		private static ConnectionStrategy _connectionStrategy;

		public static GameMaster GameMaster {
			get { 
				if(_gameMaster == null) {
					_gameMaster = StaticSelf.GetComponent<GameMaster>();
				}
				return _gameMaster; 
			}
			set { _gameMaster = value; }
		}
		private static GameMaster _gameMaster;

		public static PerformanceTracker MemoryTracker {
			get { 
				if(_memoryTracker == null) {
					_memoryTracker = StaticSelf.GetComponent<PerformanceTracker>();
				}
				return _memoryTracker; 
			}
			set { _memoryTracker = value; }
		}
		private static PerformanceTracker _memoryTracker;

		public static BuddyHandler BuddyHandler {
			get { 
				if(_buddyHandler == null) {
					_buddyHandler = StaticSelf.GetComponent<BuddyHandler>();
				}
				return _buddyHandler; 
			}
			set { _buddyHandler = value; }
		}
		private static BuddyHandler _buddyHandler;

		public static AutomationReport AutomationReport {
			get { 
				if(_automationReport == null) {
					_automationReport = StaticSelf.GetComponent<AutomationReport>();
				}
				return _automationReport; 
			}
			set { _automationReport = value; }
		}
		private static AutomationReport _automationReport;

		public static List<OnNewTestLaunch> OnNewTestLaunch {
			get { 
				return onNewTestLaunch;
			}
			set { 
				onNewTestLaunch = value;
			}
		}
		static List<OnNewTestLaunch> onNewTestLaunch = new List<OnNewTestLaunch>();

        public static AutoHud AutoHud {
            get {
                if(_autoHud == null) {
                    _autoHud = StaticSelf.GetComponent<AutoHud>();
                }
                return _autoHud;
            }
            set { _autoHud = value; }
        }
        private static AutoHud _autoHud;


		#endregion

		public static DateTime LastUseTimer { get; set; } //Used to detect stall in current test execution.
		#if !UNITY_EDITOR
		private static int _maxLastUseLimit = 60; //Used to detect stall in current test execution.
		#endif

        public static int SessionRunCount { get; set; }

		public static int HeartBeatIndex { get; set; }

		public static int TestRunMethodCount { get; private set; }

		public static int CurrentMethodsHandledCount { get; private set; }

		public static string ErrorPopupMessage { get; set; }

		public static string TestRunLocalDirectory { get; set; }

		public static bool NoIntervalScreenshots { get; set; }

		public static bool IgnoreMemoryTracking { get; set; }

		public static bool DisregardDependencies { get; set; }

		public static bool IgnoreAllBuddyTests { get; set; }

		public static bool LockIgnoreBuddyTestsFlag { get; set; }

		public static bool IsDeadLaunch { get; set; }

		public static bool AutomationStarted { get; set; }

		public static bool AllRemainingMarkedAsDeferred { get; set; }

		public static bool AwaitingScreenshot { get; set; }

		public static bool IsServerListening { get; set; }

		public static bool ErrorPopupDetected { get; set; }

		public static bool TryContinueAfterError { get; set; }

		public static bool TryContinueOnFailure { get; set; }

		public static bool PauseOnFailure { get; set; }

		public static bool EditorBasedLaunch { get; set; }

		public static bool Initialized { get; set; }

		public static bool UnitTestMode { get; set; }

		public static bool UnexpectedErrorOccurredDuringCurrentSession { get; set; }

		public static bool ErrorOccurredBeforeTestExecutionCouldBegin { get; set; }

		public static bool OverrideContinueOnFailureAfterTooManyConcurrentFailures { get; set; }

		public static bool? Validated_Phase1 { get; private set; }

		public static bool? Validated_Phase2 { get; private set; }

		public static bool ValidationRun { get; set; }

		static bool AlreadyValidated { get; set; }

		public static LaunchType LaunchType { get; set; }

		public static StoppableCoroutine CurrentTestMethod { get; private set; }

		public static IEnumerator CurrentTestEnumerator { get; private set; }

		public static MonoBehaviour CurrentMonoBehaviourInstance { get; private set; }

		public static BatchContext TestRunContext { get; private set; }

		public static TestContext CurrentTestContext { get; set; }

		public static TestContext LastTestContext { get; private set; }

		public static DateTime GameLaunchCompleted { get; set; }

		public static DateTime AutomationMasterScriptActivated { get; private set; }

		public static DateTime LastServerHeartbeat { get; private set; }

		public static double LaunchTime { get; set; }

		public enum CurrentExecutionContext { None, SetUp, SetUpClass, Test, TearDown, TearDownClass };
		public static CurrentExecutionContext ExecutionContext { get; set; }

		public static bool Busy { 
			get { 
				if(Application.isEditor && !Application.isPlaying) {
					return false;
				} else {
					return busy;
				}
			} 
			set { busy = value; }
		}
		private static bool busy;

        public static ConfigReader ConfigReader { 
            get {
                if(_configReader == null) {
                    _configReader = new ConfigReader();
                }
                return _configReader;
            } 
        }
        private static ConfigReader _configReader;

		public static List<KeyValuePair<string[], string>> AutoFails {
			get { return _autoFails; }
			private set { _autoFails = value; }
		}
		private static List<KeyValuePair<string[], string>> _autoFails = new List<KeyValuePair<string[], string>>();

		public static List<KeyValuePair<string[], string>> AutoSkips {
			get { return _autoSkips; }
			private set { _autoSkips = value; }
		}
		private static List<KeyValuePair<string[], string>> _autoSkips = new List<KeyValuePair<string[], string>>();

		public static List<string> RunDependenciesOverride {
			get { return _runDependenciesOverride; }
			private set { _runDependenciesOverride = value; }
		}
		private static List<string> _runDependenciesOverride = new List<string>();

		public static List<KeyValuePair<string, MethodInfo>> Methods {
			get { return _methods; }
			private set { _methods = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _methods = new List<KeyValuePair<string, MethodInfo>>();

		public static List<Type> AllAutomationClasses {
			get { return _allAutomationClasses; }
			private set { _allAutomationClasses = value; }
		}
		private static List<Type> _allAutomationClasses = new List<Type>();

		public static List<KeyValuePair<string, MethodInfo>> AllMethodsInFramework {
			get { return _allMethodsInFramework; }
			private set { _allMethodsInFramework = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _allMethodsInFramework = new List<KeyValuePair<string, MethodInfo>>();

		public static List<KeyValuePair<string, MethodInfo>> Deferred {
			get { return _deferred; }
			private set { _deferred = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _deferred = new List<KeyValuePair<string, MethodInfo>>();

		public static List<KeyValuePair<string, MethodInfo>> FloatingDependencies {
			get { return _floatingDependencies; }
			private set { _floatingDependencies = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _floatingDependencies = new List<KeyValuePair<string, MethodInfo>>();
		private static List<MonoBehaviour> _floatingDependenciesMonos = new List<MonoBehaviour>();

		public static List<KeyValuePair<string, MethodInfo>> MasterlessDependencies {
			get { return _masterlessDependencies; }
			private set { _masterlessDependencies = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _masterlessDependencies = new List<KeyValuePair<string, MethodInfo>>();

		public static List<KeyValuePair<string, MethodInfo>> MasterDependencies {
			get { return _masterDependencies; }
			private set { _masterDependencies = value; }
		}
		private static List<KeyValuePair<string, MethodInfo>> _masterDependencies = new List<KeyValuePair<string, MethodInfo>>();

		public static List<KeyValuePair<string, int>> LoopTests {
			get { return _loopTests; }
			set { _loopTests = value; }
		}
		private static List<KeyValuePair<string, int>> _loopTests = new List<KeyValuePair<string, int>>();

		public static List<string> EntireUnitySessionCompletedTests {
			get { return _entireUnitySessionCompletedTests; }
			private set { _entireUnitySessionCompletedTests = value; }
		}
		private static List<string> _entireUnitySessionCompletedTests = new List<string>();

		public static List<ConditionalDeferrment> ConditionalDeferrments {
			get { return _conditionalDeferrments; }
			private set { _conditionalDeferrments = value; }
		}
		private static List<ConditionalDeferrment> _conditionalDeferrments = new List<ConditionalDeferrment>();

		FailureContext _failureContext { get; set; }

		List<TestFlag> _flags = new List<TestFlag>();
		List<Type> _classSetUpRan = new List<Type>();
		List<Type> _classTearDownRan = new List<Type>();
		List<MethodInfo> _validationOrderRan = new List<MethodInfo>();
		List<KeyValuePair<string,bool>> _classSupportMethodsHandledTestRun = new List<KeyValuePair<string,bool>>(); //Class name/Setup=true,TearDown=false

		string _lastType = string.Empty;
		string _launchCommand = string.Empty;

		double _runTime = 0;
		int _lastDeferred = 0;
		int buddyCount = 0;
		bool _dependencyLogJam = false;

		public static void Initialize() {

            if(!GameMaster.Enabled || StaticSelf != null) {
				
				return;

			}

			AutomationMasterScriptActivated = DateTime.UtcNow;
			Busy = false;

			//Enable automation framework
			GameObject automationCustodian = new GameObject();
			automationCustodian.name = AUTOMATION_CUSTODIAN_NAME;
			automationCustodian.AddComponent<AutomationMaster>();

			if(Application.isPlaying) {

				UnityEngine.Object.DontDestroyOnLoad(automationCustodian);

			}

			if(Application.isEditor) {

				AutoConsole.GetConsoleMessagesAfterAppPlay();

			}

			TestRunContext = new BatchContext();
			CurrentTestContext = new TestContext();

			Application.logMessageReceived -= AutoConsole.GetLog; //Detach if already attached.
			Application.logMessageReceived += AutoConsole.GetLog; //Attach handler to recieve incoming logs.
			#if UNITY_EDITOR
				#if UNITY_2017_2_OR_NEWER
					EditorApplication.playModeStateChanged += AssemblyUnlock;
				#else 
					EditorApplication.playmodeStateChanged += AssemblyUnlock;
				#endif
			#endif
			#if UNITY_WEBGL
			WebGLBroker.AutomationReady();
			#endif
			Initialized = true;

		}

		#if UNITY_EDITOR

		//Cleanup delegates and anything GC does not handle on destruction.
		~AutomationMaster() {

			#if UNITY_2017_2_OR_NEWER
				EditorApplication.playModeStateChanged -= AssemblyUnlock;
			#else 
				EditorApplication.playmodeStateChanged -= AssemblyUnlock;
			#endif

		}

		#if UNITY_2017_2_OR_NEWER
			static void AssemblyUnlock(PlayModeStateChange p) {

				//Lock reload assemblies while automation is active and application is running.
				if(!EditorApplication.isPlaying && !EditorApplication.isPaused) {
                
					EditorApplication.UnlockReloadAssemblies();

				}

			}
		#else 
			static void AssemblyUnlock() {

				//Lock reload assemblies while automation is active and application is running.
				if(!EditorApplication.isPlaying && !EditorApplication.isPaused) {

					EditorApplication.UnlockReloadAssemblies();

				}

			}
		#endif

		public static void PauseEditorOnFailure() {

			if(PauseOnFailure) {

				EditorApplication.ExecuteMenuItem("Edit/Pause");

			}

		}

		#endif

		void Start() {

			//Attach dependencies to Automation Custodian game object.
			gameObject.AddComponent<ConsoleCommands>(); //Command Console support.
			gameObject.AddComponent<ConnectionStrategy>(); //Communication hub.
			gameObject.AddComponent<GameAssert>(); //Assert conditions.
			gameObject.AddComponent<GameDriver>(); //Main test driver.
			gameObject.AddComponent<GameMaster>(); //Game master logic.
			gameObject.AddComponent<GameHelperFunctions>(); //Test helper logic.
			gameObject.AddComponent<BuildServerBroker>(); //Brokers communication with any listening CI server.
			gameObject.AddComponent<GameServerBroker>(); //Brokers communication with any related game server.
			gameObject.AddComponent<Commands>(); //Game-specific SetUp commands.
			gameObject.AddComponent<BuddyHandler>(); //BuddyHandler.Buddy handling logic.
			gameObject.AddComponent<PerformanceTracker>(); //Track memory usage.
			gameObject.AddComponent<AutomationReport>(); //Report on test run.
			gameObject.AddComponent<Arbiter>(); //Validate and react to incoming commands.
			#if UNITY_WEBGL || UNITY_EDITOR
			gameObject.AddComponent<WebGLBroker>(); //WebGL interactions
			#endif
			#if UNITY_EDITOR
			StartCoroutine(CheckForEditorBasedLaunch());
			#endif

		}

		#if UNITY_EDITOR
		Overseer overseer;
		//If launched via the Editor Window Test Manifest gui, then queue the requested test(s) on launch.
		public IEnumerator CheckForEditorBasedLaunch() {

			yield return StartCoroutine(Q.driver.WaitRealTime(10));
			GameObject go = GameObject.Find(TestMonitorHelpers.NAME);
			if(go != null) {

				overseer = go.GetComponent<Overseer>();
				if(overseer != null) {

					PauseOnFailure = overseer.pauseOnFailure;
					string launchCommandsRaw = FileBroker.GetNonUnityTextResource(FileResource.LaunchInstructions);

					if(!string.IsNullOrEmpty(launchCommandsRaw)) {

						string[] launchCommands = launchCommandsRaw.Split(DELIMITER);
						UnitTestMode = launchCommands.ToList().Last() == "U" ? true : false;
						DisregardDependencies = launchCommands[1] == "test" ? true: false;
						string command = string.Format("[{{\"automation_command\":\"rt {0}{1}\"}}]", launchCommands[1] == "test" ? "*" : string.Empty, launchCommands[0]);
						overseer.Master_Editor_Override = new KeyValuePair<string,string>(launchCommands[0], launchCommands[1]);
						if(launchCommands.Length > 3) {

							Arbiter.LocalRunLaunch = true;
							ConnectionStrategy.ReceiveMessage(string.Format("[{{\"loop_tests\":\"{0}\"}}]", launchCommands[2]));

						}
						Arbiter.LocalRunLaunch = true;
						ConnectionStrategy.ReceiveMessage(command);
						EditorBasedLaunch = true;
						FileBroker.SaveNonUnityTextResource(FileResource.LaunchInstructions, string.Empty);

					}

				}

			}

			yield return null;

		}
		#endif

		public static void ErrorDetected(string error, bool tryContinue = true) {

			ErrorPopupDetected = true;
			TryContinueAfterError = tryContinue;
			ErrorPopupMessage = error;
			AutoConsole.PostMessage(string.Format("*ERROR POPUP* ({0})", error), MessageLevel.Abridged);

		}

		/// <summary>
		/// Reset test runner for any future test runs in the same session.
		/// </summary>
		public void ResetTestRunner() {

			LoopTests = new List<KeyValuePair<string, int>>();
			AutoSkips = new List<KeyValuePair<string[], string>>();
			Methods = Deferred = FloatingDependencies = MasterlessDependencies = MasterDependencies = new List<KeyValuePair<string, MethodInfo>>();
			LaunchTime = _runTime = _lastDeferred = buddyCount = CurrentMethodsHandledCount = TestRunMethodCount = HeartBeatIndex = 0;
			_lastType = _launchCommand = TestRunLocalDirectory = ErrorPopupMessage = string.Empty;
			Busy = NoIntervalScreenshots = IgnoreMemoryTracking = DisregardDependencies = IgnoreAllBuddyTests = LockIgnoreBuddyTestsFlag = IsDeadLaunch = AutomationStarted = IsServerListening = 
				ErrorPopupDetected = TryContinueAfterError = PauseOnFailure = UnexpectedErrorOccurredDuringCurrentSession = ErrorOccurredBeforeTestExecutionCouldBegin = OverrideContinueOnFailureAfterTooManyConcurrentFailures = 
					ValidationRun = _dependencyLogJam = false;
			TestRunContext = new BatchContext();
			CurrentTestContext = new TestContext();

		}

		/// <summary>
		/// Prepare test runner by performing phase 1 framework validation, insuring proper usage of attributes and test formatting.
		/// </summary>
		public IEnumerator BeginTestLaunch(string message) {

			GameLaunchCompleted = DateTime.UtcNow;
			Validated_Phase1 = Validated_Phase2 = null;

			#if UNITY_EDITOR
				#if UNITY_2017_2_OR_NEWER
					EditorApplication.playModeStateChanged += AssemblyUnlock;
				#else 
					EditorApplication.playmodeStateChanged += AssemblyUnlock;
				#endif
			if(!ConfigReader.GetBool("NEVER_AUTO_LOCK_RELOAD_ASSEMBLIES")) {

				EditorApplication.LockReloadAssemblies();

			}
			#endif

            //Handle creation of Automation HUD.
            if(ConfigReader.GetBool("AUTO_HUD_ENABLED")) {

                GameObject autoHudGo = new GameObject(AutoHud.GAME_OBJECT_NAME, typeof(RectTransform));
                AutoHud = autoHudGo.AddComponent<AutoHud>();
                autoHudGo.transform.SetParent(gameObject.transform);
                AutoHud.UpdateMessage("Automation Hud Enabled");

            }

			ValidationRun = message.ToLower().Contains("trilleon/validation");
			if(ValidationRun) {

				AutoConsole.PostMessage("Phase 1: Enforcement of valid attribute usage, dependency ordering, and adherence to standards.", MessageLevel.Abridged, ConsoleMessageType.AssertionIgnore);
				AutoConsole.PostMessage("Phase 2: Running of \"Trilleon/Validation\" tests to verify Test Runner (AutomationMaster) logic.", MessageLevel.Abridged, ConsoleMessageType.AssertionIgnore);

			}

			if(!AutomationStarted) {

				AutomationStarted = true;
				ValidateFramework_Phase1(); //Required for ALL test runs to begin.

				if(Validated_Phase1 == null || !(bool)Validated_Phase1) {

					string error = "FATAL ERROR In Trilleon Framework Validation (Phase 1)! Correct all validation errors and try again.";
					AutoConsole.PostMessage(error, MessageLevel.Abridged);
					Debug.LogError(error);
					AutomationStarted = false;
					Busy = false;

				} else {

					if(ValidationRun) {

						AutoConsole.PostMessage("Beginning validation [Test Runner] (Phase 2)", MessageLevel.Abridged);

						yield return StartCoroutine(LaunchTests(message));
						yield return StartCoroutine(ValidateFramework_Phase2());

						if(Validated_Phase2 == null || !(bool)Validated_Phase2) {

							string error = "FATAL ERROR In Trilleon Framework Validation (Phase 2)! Correct all validation errors and try again.";
							AutoConsole.PostMessage(error, MessageLevel.Abridged);
							Debug.LogError(error);
							AutomationStarted = false;
							Busy = false;

						}

					} else {

						yield return StartCoroutine(LaunchTests(message));

					}

				}

			}

			yield return null;

		}

		/// <summary>
		/// Set test information and begin launch procedure.
		/// </summary>
		public IEnumerator LaunchTests(string message) {

			SessionRunCount++;
			DateTime dateTime = DateTime.UtcNow;
			string time = dateTime.ToUniversalTime().ToString();
			TestRunLocalDirectory = time.Replace(":", string.Empty).Replace(" ", string.Empty).Replace("/", string.Empty);
			RunDependenciesOverride = new List<string>();
			buddyCount = 0;
			AutoHud.UpdateMessage("Automation Run Start");
			yield return StartCoroutine(LaunchInOrder(message));

		}

		/// <summary>
		/// Launch tests run, test by test.
		/// </summary>
		private IEnumerator LaunchInOrder(string command) {

			_launchCommand = command;

			//If this test run does not include all tests, then find and include all tests the selected group depends on.
			if(LaunchType != LaunchType.All) {

				if(!DisregardDependencies) {

					Methods = OrderTests(GatherAllTestsThatNeedToBeRunToSatisfyAllDependenciesForPartialTestRun(GetAllMethodsToRun(command)));

				} else {

					Methods = OrderTests(GetAllMethodsToRun(command));

				}

			} else {

				Methods = OrderTests(GetAllMethodsToRun(command));

			}

			LastUseTimer = DateTime.UtcNow;
			HeartBeatIndex = CurrentMethodsHandledCount = 0;

			//Reset batch context parameters.
			TestRunContext = new BatchContext();
			CurrentTestContext = new TestContext();

			TestRunMethodCount = Methods.Count + (IgnoreAllBuddyTests ? 0 : buddyCount);
			AutomationReport.Initialize();
			yield return StartCoroutine(Q.driver.WaitRealTime(1f)); //Wait for LoopTests to be set, if it was set with the same command as the launch command (which it should be).

			if(!ValidationRun) {

				int additional_tests = 0;
				if(LoopTests.Any()) {

					for(int l = 0; l < LoopTests.Count; l++) {

						//Add the number of times a test is being run, and subtract by one, as that one is already counted in the TestRunMethodCount.
						additional_tests += LoopTests[l].Value - 1;

					}

				}
				StartCoroutine(TestRunnerMonitor()); //Begin monitoring progress of current test run.
				Arbiter.SendCommunication("starting_automation", "0");
				string message = string.Format("Starting Automation: {0} Tests", TestRunMethodCount + additional_tests);
				ReportOnTest(message, false, MessageLevel.Abridged);

			}

			yield return StartCoroutine(GameMaster.PreTestRunLaunch()); //Launch any game-specific pretest code.
			yield return StartCoroutine(CheckForUnexpectedErrorAlerts());
            //TODO: StartCoroutine(FindBuddyHandler.Buddy()); //Dynamically find Buddy from active/available devices.

			//Run each test one after another.
			for(int m = 0; m < Methods.Count; m++) {

                //Check for declared critical failure. Mark test results and shut down test run.
                if(Assert.Critical_Test_Run_Failure.Key) {

                    CurrentTestContext = new TestContext();
                    CurrentTestContext.IsInitialized = true;
                    CurrentTestContext.IsSuccess = false;
                    CurrentTestContext.TestName = "CRITICAL FAILURE";
                    CurrentTestContext.ErrorDetails = Assert.Critical_Test_Run_Failure.Value;
                    AutomationReport.AddToReport(false, 0, false); //Save results to test run's XML file.
                    break;

                }

				//If we are still running tests within the same class as the previous tests, skip check for masterless dependencies.
				if(Methods[m].Key.Split(DELIMITER)[0] != CurrentTestContext.ClassName) {
					yield return StartCoroutine(CheckMasterlessDependencies(Methods[m], m));
				}

				KeyValuePair<string, MethodInfo> method = Methods[m];

				//Is this a Performance-like "loop" test, or just a normal run-once test execution flow?
				int RunTimes = LoopTests.FindAll(l => l.Key == method.Value.Name).Any() ? LoopTests.Find(l => l.Key == method.Value.Name).Value : 1;
				for(int l = 0; l < RunTimes; l++) {
					
					CurrentTestContext = new TestContext();
					yield return StartCoroutine(LaunchSingleTest(method, m)); //Launch test and its associated Methods.

				}

				//If we are on the final test in the loop, check for deferred tests, and run them.
				if(m >= Methods.Count - 1){

					int deferrAttrCount = Deferred.FindAll(d => (Deferr)Attribute.GetCustomAttribute(d.Value, typeof(Deferr)) != null || (d.Value.DeclaringType != null ? (Deferr)Attribute.GetCustomAttribute(d.Value.DeclaringType, typeof(Deferr)) != null : false)).Count;
					deferrAttrCount += ConditionalDeferrments.Count;
					if(deferrAttrCount == Deferred.Count) {

						AllRemainingMarkedAsDeferred = true;

					} 
					if(Deferred.Count > 0) {

						//If the deferred list has remained the same in two consecutive runs...
						if(_lastDeferred == Deferred.Count && deferrAttrCount != Deferred.Count) {

							//If we have already reversed the order of the list, and the deferred list is still identical, then the deferrence has failed.
							if(_dependencyLogJam) {

								//Report on error.
								yield return StartCoroutine(DetermineSecondaryCircularDependencyCause());
								//Exit loop and end batch test run.
								break;

							}

							//If this is the first time the deferred list remains constant in consecutive runs, reverse the order of the tests to see if dependencies can be satisfied.
							_dependencyLogJam = true;
							Deferred.Reverse();

						} else {

							_dependencyLogJam = false;

						}
						m = -1;
						Methods = new List<KeyValuePair<string,MethodInfo>>();

						//Re-assigning method and reset iterator.
						for(int o = 0; o < Deferred.Count; o++) {

							KeyValuePair<string,MethodInfo> val = Deferred[o];
							Methods.Add(new KeyValuePair<string,MethodInfo>(val.Key, val.Value));

						}

						_lastDeferred = Deferred.Count;
						Deferred = new List<KeyValuePair<string,MethodInfo>>();
						continue;

					}

				}

				LastTestContext = CurrentTestContext;

			}

			//If there are any BuddyHandler.Buddy system tests, then the test run is not complete.
            if(!Assert.Critical_Test_Run_Failure.Key && BuddyHandler.Buddies.Any()) {

				yield return StartCoroutine(BuddyHandler.RunBuddySystemTests());

			}

			//Finish tracking memory usage.
			yield return StartCoroutine(MemoryTracker.TrackMemory("Automation Finished"));

			PerformanceTracker.auto_track = false;
			if(!ValidationRun) {

				string message = "Completed Automation Run. Generating Report And Cleaning Up Test Runner.";
				ReportOnTest(message, false, MessageLevel.Abridged);
				yield return StartCoroutine(AutomationReport.SaveReport()); //Save results to xml file; wait for this process to complete before declaring tests run as complete.
				Arbiter.SendCommunication("completed_automation", "0");

			}

			#if UNITY_EDITOR
			EditorApplication.UnlockReloadAssemblies();
			if(overseer != null){

				overseer.Master_Editor_Override = new KeyValuePair<string,string>(); //Reset Editor Launcher

			}


			#endif

			//Send details to server.
			LaunchTime = GameLaunchCompleted.Subtract(AutomationMasterScriptActivated).TotalSeconds;
			if(LaunchTime <= 0) {

				AutoConsole.PostMessage(string.Format("{0} The performance metric for recording launch time is a negative value. The AutomationMasterScriptActivated value is set when the AutomationMaster Monobehaviour is attached. " +
					"GameLaunchCompleted is explicitly set for each game, as this value is subjective. Please make sure that DateTime value is set at some point in the game-specific code that is an ostensible \"game is now loaded\" point.", WARNING_FLAG), MessageLevel.Abridged);

			} else {

				Arbiter.SendCommunication("game_launch_seconds", string.Format("GAME_LAUNCH_SECONDS|{0}|", LaunchTime.ToString()));

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutoConsole.PostMessage(string.Format("DEVICE_DETAILS_HTML|{0}|", GameMaster.GetGameSpecificEmailReportTagLine()), MessageLevel.Abridged);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutoConsole.PostMessage(string.Format("GARBAGE_COLLECTION|{0},{1},{2}|", PerformanceTracker.min_gc_memory, PerformanceTracker.GetAverageGarbageCollectorMemoryUsageDuringTestRun(), PerformanceTracker.max_gc_memory), MessageLevel.Abridged);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutoConsole.PostMessage(string.Format("HEAP_SIZE|{0},{1},{2}|", PerformanceTracker.min_hs_memory, PerformanceTracker.GetAverageHeapSizeMemoryUsageDuringTestRun(), PerformanceTracker.max_hs_memory), MessageLevel.Abridged);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutoConsole.PostMessage(string.Format("FPS_VALUES|{0},{1},{2}|", PerformanceTracker.min_fps, PerformanceTracker.GetAverageFPSDuringTestRun(), PerformanceTracker.max_fps), MessageLevel.Abridged);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			AutomationReport.SendJsonInPieces("HEAP_JSON", PerformanceTracker.GetHeapSizeCounterJsonReportWithReset());
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutomationReport.SendJsonInPieces("GC_JSON", PerformanceTracker.GetGarbageCollectorJsonReportWithReset());
			yield return StartCoroutine(Q.driver.WaitRealTime(1));
			AutomationReport.SendJsonInPieces("FPS_JSON", PerformanceTracker.GetFpsJsonReportWithReset());
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			if(TestRunContext.Exceptions.Reported.Count > 0) {

				StringBuilder exceptionsData = new StringBuilder();
				exceptionsData.Append("[");
				for(int ed = 0; ed < TestRunContext.Exceptions.Reported.Count; ed++) {

					exceptionsData.Append(string.Format("{{\"screenshot_name\":\"{0}\",\"error\":\"{1}\",\"error_details\":\"{2}\",\"occurrences\":\"{3}\"}}", TestRunContext.Exceptions.Reported[ed].ScreenshotName, TestRunContext.Exceptions.Reported[ed].Error.Length > 75 ? TestRunContext.Exceptions.Reported[ed].Error.Substring(0, 75) : TestRunContext.Exceptions.Reported[ed].Error, string.Format("{0}: {1}", TestRunContext.Exceptions.Reported[ed].Error, TestRunContext.Exceptions.Reported[ed].ErrorDetails), TestRunContext.Exceptions.Reported[ed].Occurrences));
					if(ed + 1 != TestRunContext.Exceptions.Reported.Count) {
						
						exceptionsData.Append(",");

					}

				}
				exceptionsData.Append("]");
				AutomationReport.SendJsonInPieces("EXCEPTION_DATA", exceptionsData.ToString());

			}

			Application.logMessageReceived -= AutoConsole.GetLog; //Clean up delegate.
			ResetTestRunner();			
			yield return StartCoroutine(Q.driver.WaitRealTime(1f));

		}

		/// <summary>
		/// Launchs a provided test.
		/// </summary>
		/// <returns>Run the provided test.</returns>
		/// <param name="method">Test method.</param>
		/// <param name="m">Index of current method in all Methods.</param>
		public IEnumerator LaunchSingleTest(KeyValuePair<string, MethodInfo> method, int m, TestStatus buddyActionStatus = TestStatus.None, string autoFailErrorMessage = "") {

			_failureContext = FailureContext.Unbound;
			Q.assert.MideExecution_MarkTestToTryContinueAfterFail = false; //Reset.

			//Launch any delegates.
			if(OnNewTestLaunch.Any()) {
				
				for(int d = 0; d < OnNewTestLaunch.Count; d++) {
					
					OnNewTestLaunch[d].Invoke(); 

				}

			}

			bool initialized = TryContinueOnFailure = false;
			Q.assert.IsFailing = false;

			CurrentTestContext.TestInitialize(method.Value); //Initialize our test runner.
			//Get class containing this method.
			CurrentTestContext.ClassName = method.Key.Split(DELIMITER)[0];
			Type thisType = Type.GetType(string.Format("{0}.{1}", TrilleonNamespace, CurrentTestContext.ClassName));

			TestRunnerFlags flags = null;
			//Check for TestRunnerFlag attribute(s) on class containing this test.
			if(m + 1 < Methods.Count && Methods[m + 1].Key.Split(DELIMITER)[0] != CurrentTestContext.ClassName) {
				
				flags = (TestRunnerFlags)thisType.GetCustomAttributes(typeof(TestRunnerFlags), false).ToList().First();
				if(flags != null) {
					
					for(int f = 0; f < flags.Flags.Count; f++) {
						
						HandleFlags(flags.Flags[f]);

					}

				}

			} else {
				
				OverrideContinueOnFailureAfterTooManyConcurrentFailures = false; //Reset for next class to allow Flag activity on those tests.
				Assert.ConcurrentFailures = 0;

			}

			//Is this test or its class marked as Deferr? Then automatically add it to deferrment until all other tests have run.
			Deferr deferr = (Deferr)Attribute.GetCustomAttribute(method.Value, typeof(Deferr));
			if(deferr == null) {

				deferr = (Deferr)Attribute.GetCustomAttribute(thisType, typeof(Deferr));

			}
			if(deferr != null && !AllRemainingMarkedAsDeferred) {
				
				Deferred.Add(new KeyValuePair<string,MethodInfo>(string.Format("{0}{1}{2}", CurrentTestContext.ClassName, DELIMITER, CurrentTestContext.TestName), method.Value));
				yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
				yield break;

			}

			//Check for TestRunnerFlag attribute(s) on this test.
			flags = (TestRunnerFlags)method.Value.GetCustomAttributes(typeof(TestRunnerFlags), false).ToList().First();
			if(flags != null) {
				
				for(int f = 0; f < flags.Flags.Count; f++) {
					
					HandleFlags(flags.Flags[f]);

				}

			}

			//Check for Ignore attribute on class containing this test.
			if(thisType.GetCustomAttributes(typeof(Ignore), false).ToList().Any()) {
				
				Ignore reason = (Ignore)thisType.GetCustomAttributes(typeof(Ignore), false).ToList().First();
				CurrentTestContext.ErrorDetails = string.Format("#IGNORED#{0}", reason.IgnoredBecauseReason);
				TestRunContext.Ignored.Add(method.Value.Name); //Add skipped Methods to list in batch context.
				AutomationReport.AddToReport(true, 0, true); //Save results to test run's XML file.
				yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
				yield break;

			}

			//Check the Automation categories for this method, and set the context categories list.
			if(UnitTestMode) {

				List<UnityTest> categories = Attribute.GetCustomAttributes(method.Value, typeof(UnityTest)).ToList().ConvertAll(x => (UnityTest)x);
				for(int c = 0; c < categories.Count; c++) {

					CurrentTestContext.Categories.Add(categories[c].CategoryName);

				}

			} else {

				List<Automation> categories = Attribute.GetCustomAttributes(method.Value, typeof(Automation)).ToList().ConvertAll(x => (Automation)x);
				for(int c = 0; c < categories.Count; c++) {

					CurrentTestContext.Categories.Add(categories[c].CategoryName);

				}

			}

			//Check for Ignore attribute on this test.
			if(method.Value.GetCustomAttributes(typeof(Ignore), false).ToList().Any()) {
				
				Ignore reason = (Ignore)method.Value.GetCustomAttributes(typeof(Ignore), false).ToList().First();
				CurrentTestContext.ErrorDetails = string.Format("#IGNORED#{0}", reason.IgnoredBecauseReason);
				TestRunContext.Ignored.Add(method.Value.Name); //Add skipped Methods to list in batch context.
				AutomationReport.AddToReport(true, 0, true); //Save results to test run's XML file.
				yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
				yield break;

			}

			//Check for dependencies and skip or defer
			DependencyWeb dependenciesWebList = (DependencyWeb)Attribute.GetCustomAttribute(method.Value, typeof(DependencyWeb));
			DependencyWeb dependencyWebOnClass = (DependencyWeb)Attribute.GetCustomAttribute(thisType, typeof(DependencyWeb));         
			DependencyTest dependenciesTest = (DependencyTest)Attribute.GetCustomAttribute(method.Value, typeof(DependencyTest));
			TestRunnerFlags DisregardFlagOnTest = CurrentTestContext.Method.GetCustomAttributes(typeof(TestRunnerFlags), false).ToList().ConvertAll(x => (TestRunnerFlags)x).First();
			TestRunnerFlags DisregardFlagOnClass = thisType.GetCustomAttributes(typeof(TestRunnerFlags), false).ToList().ConvertAll(x => (TestRunnerFlags)x).First();
			_flags = new List<TestFlag>();
			if(DisregardFlagOnTest != null) {
				
				_flags.AddRange(DisregardFlagOnTest.Flags);

			}
			if(DisregardFlagOnClass != null) {
				
				_flags.AddRange(DisregardFlagOnClass.Flags);

			}
				
			if(dependenciesWebList != null || dependencyWebOnClass != null) {

				//Handle Dependency Web relationships and skip or defer this test if necessary.
				List<string> dependencies = new List<string>();
				if(dependenciesWebList != null) {
					
					dependencies.AddRange(dependenciesWebList.Dependencies);

				}
				if(dependencyWebOnClass != null) {
					
					dependencies.AddRange(dependencyWebOnClass.Dependencies);

				}

				for(int t = 0; t < dependencies.Count; t++) {

					///Check if dependency has run, and whether it has passed, failed, or been ignored.
					if(TestRunContext.Passed.Tests.Contains(dependencies[t]) || !Methods.FindAll(z => z.Value.Name == dependencies[t]).Any()) {

						//If method has passed, or is not even a part of the current run, then skip dependency checking.
						continue; 

					} else if(RunDependenciesOverride.Contains(dependencies[t])) {

						//If a dependency failed, but explicitly was marked to be overridden, run this test. This should occur when a dependent test has its requirements satisfied by partial completion of its dependency.
						if(TestRunContext.Failed.Tests.KeyValListContainsKey(dependencies[t])) {

							string notice = string.Format("The test \"{0}\" failed and was a dependency of \"{1}\", but the former was marked \"Override and Run Dependencies\". Ostensibly, this means that whatever was required by the latter test was satisfied by a partial completion of the former. Because of this, \"{1}\" will now be launched.", dependencies[t], CurrentTestContext.TestName);
							AutoConsole.PostMessage(notice, MessageLevel.Abridged);
							CurrentTestContext.ErrorDetails += notice;
							CurrentTestContext.AddAssertion(notice);

						}

						continue; 

					} else if(TestRunContext.Failed.Tests.KeyValListContainsKey(dependencies[t])) {

						//If we explicitly do not want this test to be skipped, regardless of its dependency failing, then continue.
						if(_flags.Contains(TestFlag.DependencyNoSkip)) {

							continue;

						}

						string errorMessage = string.Format("Dependency Architecture Failure! Test {0} was a dependency for the current method {1} and failed. Skipping test.", dependencies[t], CurrentTestContext.TestName);
						CurrentTestContext.ErrorDetails = errorMessage;
						yield return StartCoroutine(Q.assert.Skip(errorMessage));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					} else if(TestRunContext.Ignored.Tests.Contains(dependencies[t])) {

						//If we explicitly do not want this test to be skipped, regardless of its dependency failing, then continue.
						if(_flags.Contains(TestFlag.DependencyNoSkip)) {

							continue;

						}

						string errorMessage = string.Format("Dependency Architecture Failure! Test {0} was a dependency for the current method {1} and is set to be Ignored. Skipping test.", dependencies[t], CurrentTestContext.TestName);
						CurrentTestContext.ErrorDetails = errorMessage;
						yield return StartCoroutine(Q.assert.Skip(errorMessage));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					} else {

						if(!DisregardDependencies && !Deferred.KeyValListContainsKey(string.Format("{0}{1}", DELIMITER, method.Value.Name), true)) {

							Deferred.Add(new KeyValuePair<string,MethodInfo>(string.Format("{0}{1}{2}", CurrentTestContext.ClassName, DELIMITER, CurrentTestContext.TestName), method.Value));
							yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
							yield break;

						}

					}

				}

			} else if(dependenciesTest != null && dependenciesTest.order > 1) {

				//Handle Dependecy Test relationships and skip this test if the previous test executed was skipped or failed.
				if(LastTestContext != null && !LastTestContext.IsSuccess) {

					//If we explicitly do not want this test to be skipped, regardless of its dependency failing, then continue.
					if(!_flags.Contains(TestFlag.DependencyNoSkip)) {

						string errorMessage = string.Format("Dependency Architecture Failure! Test {0} was a dependency for the current method {1} and is set to be Ignored. Skipping test.", LastTestContext.Method.Name, CurrentTestContext.TestName);
						CurrentTestContext.ErrorDetails = errorMessage;
						yield return StartCoroutine(Q.assert.Skip(errorMessage));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				}

			}

			//Handle "Or" based dependencies, where only one test from the list is required to be passed to run this test.
			List<string> OneOfDependencies = dependenciesWebList != null ? dependenciesWebList.OneOfDependencies : new List<string>();
			bool oneOfDepCheck = OneOfDependencies.Count > 0;
			if(oneOfDepCheck) {

				//Ignore this check if none of the possible dependencies are even included in the current test run.
				if(Methods.FindAll(o => OneOfDependencies.Contains(o.Value.Name)).Any()) {
					
					for(int p = 0; p < OneOfDependencies.Count; p++) {

						if(TestRunContext.Passed.Tests.Contains(OneOfDependencies[p])) {

							oneOfDepCheck = false;
							break;

						}

					}

					if(oneOfDepCheck) {

						Deferred.Add(new KeyValuePair<string,MethodInfo>(string.Format("{0}{1}{2}", CurrentTestContext.ClassName, DELIMITER, CurrentTestContext.TestName), method.Value));
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				}


			}

			//Handle and declare floating dependencies.
			FloatingDependency floatingDependencies = (FloatingDependency)Attribute.GetCustomAttribute(method.Value, typeof(FloatingDependency));
			FloatingDependencies = new List<KeyValuePair<string, MethodInfo>>();
			if(floatingDependencies != null) {

				for(int f = 0; f < floatingDependencies.Dependencies.Count; f++) {

					FloatingDependencies.Add(AllMethodsInFramework.Find(am => am.Value.Name == floatingDependencies.Dependencies[f]));

				}

				if(FloatingDependencies.Count != floatingDependencies.Dependencies.Count) {

					string errorMessage = string.Format("Dependency Architecture Failure! A failure occurred in gathering floating dependencies for test {0}. One or more declared tests could not be found.", CurrentTestContext.TestName);
					CurrentTestContext.ErrorDetails = errorMessage;
					yield return StartCoroutine(Q.assert.Fail(errorMessage, FailureContext.Skipped));
					AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
					ReportOnTest();
					yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
					yield break;

				}

			}

			//Autofail/AutoSkip test. This only occurs during BuddySystem execution.
			if(buddyActionStatus != TestStatus.None) {

				if(buddyActionStatus == TestStatus.Fail) {

					CurrentTestContext.ErrorDetails = string.Format("Required Buddy test failed.");
					yield return StartCoroutine(Q.assert.Fail(autoFailErrorMessage, FailureContext.Skipped));
					AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
					ReportOnTest();
					yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
					yield break;

				} else if(buddyActionStatus == TestStatus.Ignore) {

					CurrentTestContext.ErrorDetails = string.Format("#IGNORED#{0}", "Buddy tests were explicitly set to be ignored in this test run.");
					TestRunContext.Ignored.Add(method.Value.Name); //Add skipped Methods to list in batch context.
					AutomationReport.AddToReport(true, 0, true); //Save results to test run's XML file.
					ReportOnTest();
					yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
					yield break;

				}

			}

			//Begin launch actual.
			if(this.GetComponent(thisType) == null) {
				
				CurrentMonoBehaviourInstance = gameObject.AddComponent(thisType) as MonoBehaviour; //Add this script to Automation Custodian and save instance.

			} else {
				
				CurrentMonoBehaviourInstance = this.GetComponent(thisType) as MonoBehaviour; //Script is already on Automation Custodian. Only save instance.

			}
			initialized = true;

			CurrentMethodsHandledCount++;
			yield return StartCoroutine(CheckForUnexpectedErrorAlerts(thisType, initialized));

			//Launch applicable SetUp Methods.
			bool runClassInit = _lastType != CurrentTestContext.ClassName;
			if(runClassInit && !BuddyHandler.BuddySystemHandlingStarted) {
				
				//Launch global test class setup.
				if(!UnitTestMode && !_flags.Contains(TestFlag.DisregardSetUpClassGlobal) && CurrentTestContext.IsSuccess) {
					
					yield return StartCoroutine(Q.game.GlobalSetUpClass());
					_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.SetUpClassGlobal : FailureContext.Unbound;

				}

				if(CurrentTestContext.IsSuccess) {
					
					yield return StartCoroutine(LaunchSupportMethod(thisType, "setupclass"));
					_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.SetUpClass : FailureContext.Unbound;

				}

			}
			if(!_flags.Contains(TestFlag.DisregardSetUpTest) && CurrentTestContext.IsSuccess) {
				
				yield return StartCoroutine(LaunchSupportMethod(thisType, "setupclasstest"));
				_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.SetUp : FailureContext.Unbound;

			}
			if(!UnitTestMode && !_flags.Contains(TestFlag.DisregardSetUpGlobal) && CurrentTestContext.IsSuccess) {
				
				yield return StartCoroutine(Q.game.GlobalSetUpTest()); //Global SetUp dictated by game specific functionality.
				_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.SetUpGlobal : FailureContext.Unbound;

			}

			//Check for dynamically-added ConditionalDeferrments, which are registered in the SetUpClass (preferred) or SetUp methods.
			List<ConditionalDeferrment> matches = ConditionalDeferrments.FindAll(x => x.TestMethod == CurrentTestContext.TestName);
			if(matches.Any()) {

				if(!matches.First().Condition.Invoke() && !AllRemainingMarkedAsDeferred) {

					if(matches.First().NumberOfTimesAlreadyDeferred > matches.First().NumberOfTimesToAllowsTestToBeDeferred) {

						CurrentTestContext.ErrorDetails = string.Format("Test \"{0}\" has a ConditionalDeferrment registered by \"{1}\" in the SetUpClass method that has caused the test to be deferred more than the allowed number of times ({2}).", method.Value, CurrentTestContext.ClassName, matches.First().NumberOfTimesAlreadyDeferred);
						yield return StartCoroutine(Q.assert.Fail(autoFailErrorMessage, FailureContext.Skipped));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					} else {
						
						matches.First().NumberOfTimesAlreadyDeferred++;
						Deferred.Add(new KeyValuePair<string,MethodInfo>(string.Format("{0}{1}{2}", CurrentTestContext.ClassName, DELIMITER, CurrentTestContext.TestName), method.Value));
						yield return StartCoroutine (SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				}

				ConditionalDeferrments.Remove(matches.First()); //Remove from list since we are running this test. Required to keep Deferred test count accurate.

			}

			//If the test has inhereted a failure during SetUp or SetUpClass, fail the test now with the inhereted error.
			if(!CurrentTestContext.IsSuccess) {

				yield return StartCoroutine(Q.assert.Fail(string.Format(string.Format("An exception occurred in the {0} method for class {1}. Failing {2}.", Enum.GetName(typeof(FailureContext), _failureContext), CurrentTestContext.ClassName, CurrentTestContext.TestName))));
				AutomationReport.AddToReport(false, 0); //Save results to test run's XML file.
				ReportOnTest();
				yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
				yield break;

			}

			//Special "Dependency" case handling for explicit skips.
			for(int af = 0; af < AutoSkips.Count; af++) {

				string[] key = AutoSkips[af].Key;
				if(key[0] == "class") {

					if(CurrentTestContext.ClassName == key[1]) {

						yield return StartCoroutine(Q.assert.Skip(string.Format("AutoSkip: Test Class {0} was marked as an auto skip with this message: {1}", CurrentTestContext.ClassName, AutoSkips[af].Value)));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				} else {

					if(CurrentTestContext.TestName == key[1]) {

						yield return StartCoroutine(Q.assert.Skip(string.Format("AutoSkip: Test Method {0} was marked as an auto skip with this message: {1}", CurrentTestContext.TestName, AutoSkips[af].Value)));
						AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				}

			}

			//Special "Dependency" case handling for explicit fails.
			for(int af = 0; af < AutoFails.Count; af++) {

				string[] key = AutoFails[af].Key;
				if(key[0] == "class") {

					if(CurrentTestContext.ClassName == key[1]) {

						yield return StartCoroutine(Q.assert.Fail(string.Format("AutoFail: Test Class {0} was marked as an auto fail with this message: {1}", CurrentTestContext.ClassName, AutoFails[af].Value)));
						AutomationReport.AddToReport(false, 0); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				} else {

					if(CurrentTestContext.TestName == key[1]) {

						yield return StartCoroutine(Q.assert.Fail(string.Format("AutoFail: Test Method {0} was marked as an auto fail with this message: {1}", CurrentTestContext.TestName, AutoFails[af].Value)));
						AutomationReport.AddToReport(false, 0); //Save results to test run's XML file.
						ReportOnTest();
						yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));
						yield break;

					}

				}

			}

			string message = string.Format("Launching {0}", CurrentTestContext.TestName);
			AutoConsole.PostMessage(message, MessageLevel.Abridged, ConsoleMessageType.TestRunnerUpdate);
			yield return StartCoroutine(MemoryTracker.TrackMemory(message)); //Track memory before test start.

			if(ValidationRun) {
				
				_validationOrderRan.Add(CurrentTestContext.Method);

			}

			ExecutionContext = CurrentExecutionContext.Test; //Used to properly handle Assertion failures that occur outside of the execution of a test (Setup/TearDown etc).

			//Run test.
			CurrentTestEnumerator = (IEnumerator)method.Value.Invoke(CurrentMonoBehaviourInstance, new object[]{ }); //Get the test method as an IEnumerator so that it can be launched as a coroutine. 
			CurrentTestMethod = CurrentMonoBehaviourInstance.StartCoroutineEx(CurrentTestEnumerator);
			yield return CurrentTestMethod.WaitFor(); //Begin test and wait for completion.
			_runTime = (DateTime.UtcNow - CurrentTestContext.StartTime).TotalSeconds;

			message = string.Format("Completed {0}", CurrentTestContext.TestName);
			yield return StartCoroutine(MemoryTracker.TrackMemory(message)); //Track memory after test complete.

			//Attach any tags declared for the current test. These will be displayed in this test's data panel.
			Tag tagAttribute = (Tag)CurrentTestContext.Method.GetCustomAttributes(typeof(Tag), false).First();
			if(tagAttribute != null && tagAttribute.Notifications.Any()) {
				
				CurrentTestContext.Notices.AddRange(tagAttribute.Notifications);

			}

			bool failureDuringTestMainExecution = false;
			if(!CurrentTestContext.IsSuccess) {

				failureDuringTestMainExecution = true;

			}

			yield return StartCoroutine(SingleTestLaunchCleanup(method.Value.Name, thisType, initialized, m));

			//If no failures, report success. Failures are stored through Assert class.
			if(CurrentTestContext.IsSuccess) {

				if(TestRunContext.Failed.Tests.FindAll(x => x.Key.Split(DELIMITER)[0] == CurrentTestContext.TestName).Any()) {

					CurrentTestContext.IsSuccess = false;

				} else {

					TestRunContext.Passed.Add(CurrentTestContext.TestName);

				}

			} else {

				if(!failureDuringTestMainExecution && !CurrentTestContext.IsSuccess) {

					yield return StartCoroutine(TakeScreenshot());

				}

			}

			AutomationReport.AddToReport(CurrentTestContext.IsSuccess, _runTime, TestRunContext.Skipped.Tests.Contains(CurrentTestContext.TestName)); //Save results to test run's XML string builder.
			ReportOnTest(); //Report success or failure.

			yield return null;

		}

		//Clean up current test's run.
		private IEnumerator SingleTestLaunchCleanup(string methodName, Type thisType, bool initialized, int m) {

			bool isSuccessBeforeLaunchingTearDown = CurrentTestContext.IsSuccess;

			EntireUnitySessionCompletedTests.Add(methodName);

			bool runClassTearDown = true;
			if(runClassTearDown && Methods.Count > m + 1) {
				
				runClassTearDown = Methods.Count == (m+1) || Methods[m+1].Key.Split(DELIMITER)[0] != CurrentTestContext.ClassName; //If next method's class is different.

			}

			//Tear down class/test.
			if(runClassTearDown && initialized && !BuddyHandler.BuddySystemHandlingStarted) {
				
				if(!_flags.Contains(TestFlag.DisregardTearDownTest)) {
					
					yield return StartCoroutine(LaunchSupportMethod(thisType, "teardownclasstest"));
					_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.TearDown : FailureContext.Unbound;

				}

				yield return StartCoroutine(LaunchSupportMethod(thisType, "teardownclass"));

				if(!UnitTestMode && !_flags.Contains(TestFlag.DisregardTearDownGlobal) && CurrentTestContext.IsSuccess) {
                    
					yield return StartCoroutine(Q.game.GlobalTearDownTest()); //Global TearDown dictated by game specific functionality.
					_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.TearDownGlobal : FailureContext.Unbound;

				}

				if(!UnitTestMode && !_flags.Contains(TestFlag.DisregardTearDownClassGlobal) && CurrentTestContext.IsSuccess) {
					
					yield return StartCoroutine(Q.game.GlobalTearDownClass());
					_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.TearDownClass : FailureContext.Unbound;

				}

				Destroy(this.GetComponent(thisType)); //Remove this test script from the Automation Custodian game object.

			} else if(initialized) {
				
				yield return StartCoroutine(LaunchSupportMethod(thisType, "teardownclasstest"));
				_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.TearDown : FailureContext.Unbound;

			}

			if(!UnitTestMode && !_flags.Contains(TestFlag.DisregardTearDownGlobal) && CurrentTestContext.IsSuccess) {
				
				yield return StartCoroutine(Q.game.GlobalTearDownTest()); //Global TearDown dictated by game specific functionality.
				_failureContext = !CurrentTestContext.IsSuccess ? FailureContext.TearDownGlobal : FailureContext.Unbound;

			}

			//If the test has inhereted a failure during SetUp or SetUpClass, fail the test now with the inhereted error.
			if(isSuccessBeforeLaunchingTearDown && !CurrentTestContext.IsSuccess) {

				yield return StartCoroutine(Q.assert.Fail(string.Format(string.Format("An exception occurred in the {0} method for class {1}. Failing {2}.", Enum.GetName(typeof(FailureContext), _failureContext), CurrentTestContext.ClassName, CurrentTestContext.TestName))));

			}

			if(initialized) {
				
				_lastType = CurrentTestContext.ClassName;
				TestRunContext.CompletedTests.Add(methodName);

			}

			//Clean up monobehaviours added for floating dependencies.
			for (int f = 0; f < _floatingDependenciesMonos.Count; f++) {
				
				Destroy(_floatingDependenciesMonos[f]);

			}

			yield return null; 

		}

		#region Validate Automation Suite Stability And Enforce Post-Compilation Rules

		public static void ValidateFramework_Phase1() {

			List<string> errors = new List<string>();
			List<string> warnings = new List<string>();

			string errorPrefix = "POST COMPILATION VALIDATION FAILURE";
			string warningPrefix = "VALIDATION WARNING";
			List<string> invalidClassNames = new List<string>();
			List<KeyValuePair<string, string[]>> methodDependencyGrouping = new List<KeyValuePair<string, string[]>>();

			SetAllMethodsInFramework();
			List<string> methodKeys = AllMethodsInFramework.ExtractListOfKeysFromKeyValList(true, 1);

			//Get all dependencies
			for(int dl = 0; dl < AllMethodsInFramework.Count; dl++) {

				KeyValuePair<string,MethodInfo> method = AllMethodsInFramework[dl];
				string[] keyPair = method.Key.Split(DELIMITER);
				string AutomationClassName = keyPair[0];
				string AutomationTestName = keyPair[1];

				#region Validate DependencyWeb

				DependencyWeb dependenciesList = (DependencyWeb)Attribute.GetCustomAttribute(method.Value, typeof(DependencyWeb));
				DependencyWeb dependenciesOnClass = (DependencyWeb)Attribute.GetCustomAttribute(Type.GetType(string.Format("TrilleonAutomation.{0}", method.Key.Split(DELIMITER).First())), typeof(DependencyWeb));         

				if(dependenciesList != null) {

					List<string> dependencies = new List<string>();
					if(dependenciesList != null) {
						
						dependencies.AddRange(dependenciesList.Dependencies);
						dependencies.AddRange(dependenciesList.OneOfDependencies);

					}
					if(dependenciesOnClass != null) {
						
						dependencies.AddRange(dependenciesOnClass.Dependencies);

					}
					if(!dependencies.Any()) {

						errors.Add(string.Format("{0}: Test \"{1}\" has a DependencyWeb attribute, but no test names were provided.", errorPrefix, method.Value));

					}

					methodDependencyGrouping.Add(new KeyValuePair<string,string[]>(method.Value.Name, dependencies.ToArray()));
					//Check if dependency is a valid test method.
					for(int d = 0; d < dependencies.Count; d++) {

						if(!methodKeys.Contains(dependencies[d])) {

							errors.Add(string.Format("{0}: Test \"{1}\" declares \"{2}\" as a dependency, but this test does not exist or is not marked as an automation method.", errorPrefix, AutomationTestName, dependencies[d]));
							continue;

						}
						//If any method calls itself as a dependency, mark it as a failure immediately.
						if(method.Value.Name == dependencies[d]) {

							errors.Add(string.Format("{0}: Test \"{1}\" was marked as a dependency of itself. This is an invalid use of the Dependency Web.", errorPrefix, AutomationTestName));
							continue;

						}

						//Check all previously accessed dependencies, and verify that no circular dependencies exist between two tests.
						List<KeyValuePair<string, string[]>> referencedDependencies = methodDependencyGrouping.FindAll(x => x.Key == dependencies[d]);
						for(int r = 0; r < referencedDependencies.Count; r++) {

							KeyValuePair<string, string[]> thisRef = referencedDependencies[r];
							if(Array.IndexOf(thisRef.Value, AutomationTestName) >= 0) {

								errors.Add(string.Format("{0}: Tests \"{1}\" and \"{2}\" were marked as dependencies of one another. This is an invalid use of the Dependency Web.", errorPrefix, thisRef.Key, AutomationTestName));
								continue;

							}

						}

					}

					#region Validate Dependency Mixing

					//Get all methods that have a DependencyTest attribute under a class that has a DependencyClass attribute.
					List<KeyValuePair<string,MethodInfo>> mixedDependencies = AllMethodsInFramework.FindAll(x => { 
						return x.Value.DeclaringType.GetCustomAttributes(typeof(DependencyClass), false).Length > 0 && x.Value.GetCustomAttributes(typeof(DependencyTest), false).Length > 0;
					} );
					if(mixedDependencies.FindAll(x => x.Value.Name == method.Value.Name).Any()) {

						errors.Add(string.Format("{0}: Test \"{1}\" was assigned both a DependencyWeb and DependencyTest attribute while the class was assigned a DependencyClass attribute. This is an invalid use of the Dependency Web.", errorPrefix, AutomationTestName));
						continue;

					}

					#endregion

				}

				#region Validate Automation Test

				List<KeyValuePair<string,MethodInfo>> methodWithName = AllMethodsInFramework.FindAll(x => x.Value.Name == AutomationTestName);
				if(methodWithName.Count > 1) {

					errors.Add(string.Format("{0}: Test \"{1}\" is duplicated across the following classes: {2}", errorPrefix, AutomationTestName, string.Join(", ", methodWithName.ExtractListOfKeysFromKeyValList(true, 0).ToArray())));
					continue;

				}

				#endregion

				#region Validate Automation Class

				//Check once for each automation class name.
				if(!AutomationClassName.EndsWith("Tests")) {
					if(!invalidClassNames.Contains(AutomationClassName)) {

						invalidClassNames.Add(AutomationClassName);
						errors.Add(string.Format("{0}: Automation class \"{1}\" does not adhere to naming rules that dictate all automation classes end in the substring \"Tests\". This rule is in place for the sake of consistency and clarity.", errorPrefix, AutomationClassName));
						continue;

					}
				}

				#endregion

			}

			#region Validate Master Dependency Ordering

			//Verify correct ordering of DependencyClass classes.
			List<Type> masterDependencies = GetAutomationClasses(true).FindAll(x => { 
				return x.GetCustomAttributes(typeof(DependencyClass), false).Length > 0;
			} );

			//Verify that all DependencyClass classes maintain a direct order that is one index higher than the last.
			List<Type> handled = new List<Type>();
			int missingIndex = 0;
			for(int x = 0; x < masterDependencies.Count; x++) {

				List<Type> matchingOrder = masterDependencies.FindAll(d => { 
					DependencyClass dc = ((DependencyClass[])d.GetCustomAttributes(typeof(DependencyClass), false)).ToList().First();
					return dc.order == x;
				});
				if(x == 0) {

					for(int m = 0; m < matchingOrder.Count; m++) {

						//DebugClass order of 0 is reserved for Trilleon validation. Using it outside of a DebugClass will result in validation failure. Using it in a Debug class that is not part of validation will result in it being ignored.
						DebugClass d = ((DebugClass[])matchingOrder[m].GetCustomAttributes(typeof(DebugClass), false)).ToList().First();
						if(d == null) {

							errors.Add(string.Format("{0}: Dependency Classes with an order of 0 are reserved for Test Runner validation within Debug classes. Using it in Debug classes that are not part of validation will simply result in the attribute being ignored.", errorPrefix));
							break;

						}

					}

				}

				if(!matchingOrder.Any()) {

					//If we have handled all of the DependencyClass classes, then this will not be used. If there is a discrepency, then it is the first, or the only, missing DependcyClass order index.
					missingIndex = x;
					break;

				}
				handled.AddRange(matchingOrder);

			}
			if(handled.Count != masterDependencies.Count) {

				errors.Add(string.Format("{0}: A missing DependencyClass order was encountered. DependencyClass architecture requires a continous incremental order. For example: If one class has order 1, and another class has order 3, there must be a class with order 2. Missing order index is \"{1}\"", errorPrefix, missingIndex.ToString()));

			}

			//Now validate that methods under DependencyClasses have a valid DependencyTest order.
			missingIndex = 0;
			for(int x = 0; x < masterDependencies.Count; x++) {

				List<Type> matchingOrder =  masterDependencies.FindAll(d => { 
					DependencyClass dc = ((DependencyClass[])d.GetCustomAttributes(typeof(DependencyClass), false)).ToList().First();
					return dc.order == x;
				});
				List<MethodInfo> methods = new List<MethodInfo>();
				for(int m = 0; m < matchingOrder.Count; m++) {

					//Get all methods that have a DependencyTest attribute.
					methods.AddRange(matchingOrder[m].GetMethods().ToList().FindAll(d => {				
						//Return all methods that have a DependencyTest attribute.
						DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(d, typeof(DependencyTest));
						return dt != null;
					}));

				}

				for(int v = 0; v < methods.Count; v++) {

					List<MethodInfo> match = methods.FindAll(i => { 
						DependencyTest dt = ((DependencyTest[])i.GetCustomAttributes(typeof(DependencyTest), false)).ToList().First();
						return dt.order == v + 1;
					});

					if(match.Count < 1) {

						missingIndex = x;
						errors.Add(string.Format("{0}: The DependencyTest attribute ordering under DependencyClass ( Name(s): {1} - DependencyClass ID: {2} ) is invalid. A DependencyTest ID of {3} is expected and could not be found.", errorPrefix, masterDependencies[x].Name, x, missingIndex.ToString()));
						break;

					} else if(match.Count > 1) {

						StringBuilder matches = new StringBuilder();
						for(int m = 0; m < match.Count; m++) {

							matches.Append(match[m].Name);
							if(m + 1 != match.Count) {

								matches.Append(", ");

							}

						}
						StringBuilder classNames = new StringBuilder();
						for(int m = 0; m < matchingOrder.Count; m++) {

							classNames.Append(matchingOrder[m].Name);
							if(m + 1 != matchingOrder.Count) {

								classNames.Append(", ");

							}

						}
						errors.Add(string.Format("{0}: There are multiple tests with the DependencyTest ID of {1} under the DependencyClass ( Name(s): {2} - DependencyClass ID: {3} ). Tests with duplicate ID ( {4} )", errorPrefix, v + 1, classNames.ToString(), x, matches.ToString()));

					}
						
				}

			}

			#endregion

			#region Validate Master Dependency Ordering

			//Verify correct ordering of masterless classes.
			List<Type> masterlessDependencies = GetAutomationClasses(true).FindAll(x => { 
				return x.GetCustomAttributes(typeof(DependencyClass), false).Length == 0;
			} );

			for(int ml = 0; ml < masterlessDependencies.Count; ml++) {

				List<MethodInfo> depTests = masterlessDependencies[ml].GetMethods().ToList().FindAll(x => { 
					return x.GetCustomAttributes(typeof(DependencyTest), false).Length > 0;
				} );

				List <KeyValuePair<int,string>> ordersUsed = new List<KeyValuePair<int,string>>();
				for(int mdt = 0; mdt < depTests.Count; mdt++) {

					DependencyTest dt = (DependencyTest)depTests[mdt].GetCustomAttributes(typeof(DependencyTest), false).ToList().First();
					int id = dt.order;
					ordersUsed.Add(new KeyValuePair<int,string>(id, depTests[mdt].Name));

				}

				for(int ou = 1; ou <= ordersUsed.Count; ou++) {

					int matchCount = ordersUsed.FindAll(z => z.Key == ou).Count;

					if(matchCount == 0) {

						errors.Add(string.Format("{0}: Expecting a DependencyTest ID of {1} under test class \"{2}\". None was found.", errorPrefix, ou, masterlessDependencies[ml].Name));

					} else if(matchCount > 1) {

						errors.Add(string.Format("{0}: Duplicate DependencyTest ID of {1} under test class \"{2}\". The following tests share an order (\"{3}\"). A DependencyTest order must be unique within a single test class.", errorPrefix, ou, masterlessDependencies[ml].Name, string.Join("\", \"", ordersUsed.FindAll(z => z.Key == ou).ExtractListOfValuesFromKeyValList().ToArray())));
						ou += matchCount; //Since there were duplicates, the expected total order should be reduced.

					} 

				}

			}

			#endregion

			#endregion

			#region Validate Floating Dependency References

			List<KeyValuePair<string,MethodInfo>> floatingDependencyReferenceMethods = AllMethodsInFramework.FindAll(x => {
				return x.Value.GetCustomAttributes(typeof(FloatingDependency), false).Length > 0;
			});

			for(int f = 0; f < floatingDependencyReferenceMethods.Count; f++) {

				FloatingDependency floatDep = (FloatingDependency)floatingDependencyReferenceMethods[f].Value.GetCustomAttributes(typeof(FloatingDependency), false).First();
				if(floatDep != null) {

					List<string> testNames = floatDep.Dependencies;
					for(int tn = 0; tn < testNames.Count; tn++) {

						int count = AllMethodsInFramework.FindAll(x => {
							return x.Value.Name == testNames[tn];
						}).Count;

						if(count != 1) {

							errors.Add(string.Format("{0}: Test \"{1}\" declared Floating Dependency \"{2}\", but no test method by that name could be found.", errorPrefix, floatingDependencyReferenceMethods[f].Value.Name, testNames[tn]));

						}

					}

				} else {

					errors.Add(string.Format("{0}: Test \"{1}\" has a FloatingDependency attribute, but no test names were provided.", errorPrefix, floatingDependencyReferenceMethods[f].Value.Name));

				}

			}

			#endregion

			#region Validate BuddySystem

			List<KeyValuePair<string,MethodInfo>> markedBuddies = AllMethodsInFramework.FindAll(x => {
				return x.Value.GetCustomAttributes(typeof(BuddySystem), false).Length > 0;
			});

			if(markedBuddies.Any()) {

				for(int b = 0; b < markedBuddies.Count; b++) {

					BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(markedBuddies[b].Value, typeof(BuddySystem));

					if(bs.buddy != Buddy.Action) {

						if(string.IsNullOrEmpty(bs.ReactionOf)) {

							errors.Add(string.Format("{0}: Test \"{1}\" has an invalid BuddySystem attribute. \"Buddy.Reaction\" and \"Buddy.CounterReaction\" tags must provide the name of the associated Action test.", errorPrefix, markedBuddies[b].Value.Name));
							continue;

						}

						if(!methodKeys.Contains(bs.ReactionOf)) {

							errors.Add(string.Format("{0}: Test \"{1}\" has a BuddySystem attribute that references an Action test ({2}) which does not appear to exist.", errorPrefix, markedBuddies[b].Value.Name, bs.ReactionOf));
							continue;

						}

						//The referenced test method has been found, but does not have a BuddySystem atrribute.
						MethodInfo actionBuddy = markedBuddies.Find(x => x.Value.Name == bs.ReactionOf).Value;
						if(actionBuddy == null) {

							errors.Add(string.Format("{0}: Test \"{1}\" was referenced as a Buddy.Action of the test ({2}). This is an automation test, but does not have a BuddySystem attribute.", errorPrefix, bs.ReactionOf, markedBuddies[b].Value));
							continue;

						}

						//The referenced test method has been found, but has the wrong BuddySystem attribute (must be Buddy.Action).
						BuddySystem ab = (BuddySystem)Attribute.GetCustomAttribute(actionBuddy, typeof(BuddySystem));
						if(ab.buddy != Buddy.Action) {

							errors.Add(string.Format("{0}: Test \"{1}\" was referenced as a Buddy.Action of the test ({2}). This test has a BuddySystem attribute, but is not marked as a Buddy.Action test.", errorPrefix, bs.ReactionOf, markedBuddies[b].Value));
							continue;

						}

					} else {

						//The referenced test is a Buddy.Action, but was supplied with a ReactionOf value. This is superfluous, and may indicate an error.
						if(!string.IsNullOrEmpty(bs.ReactionOf)) {
							warnings.Add(string.Format("{0}: Test \"{1}\" is a Buddy.Action test, but was supplied with a ReactionOf parameter, which is only used by Reaction and CounterReaction tests. This may indicate a mistake in logic.", warningPrefix, markedBuddies[b].Value));
							continue;
						}

					}

				}

			}

			#endregion

			#region SetUp/TearDown

			List<MethodInfo> supportMethods = new List<MethodInfo>();
			for(int c = 0; c < AllAutomationClasses.Count; c++) {

				supportMethods.AddRange(AllAutomationClasses[c].GetMethods());

			}

			//Are tagged support methods IEnumerators?
			supportMethods = supportMethods.FindAll(s => s.GetCustomAttributes(typeof(Automation), false).Length > 0 || s.GetCustomAttributes(typeof(SetUp), false).Length > 0 || s.GetCustomAttributes(typeof(SetUpClass), false).Length > 0 || s.GetCustomAttributes(typeof(TearDown), false).Length > 0 ||  s.GetCustomAttributes(typeof(TearDownClass), false).Length > 0);
			for(int s = 0; s < supportMethods.Count; s++) {

				if(supportMethods[s].ReturnType != typeof(IEnumerator)) {

					errors.Add(string.Format("{0}: Class \"{1}\" Method \"{2}\" is not an IEnumerator. All test/support methods must be an IEnumerator.", errorPrefix, supportMethods[s].ReflectedType.Name, supportMethods[s].Name));
					continue;

				}

			}

			//Are tagged support methods unique within the context of this test class (There cannot be two SetUpClass methods, for example)?
			List<Type> classesWithSupportMethods = AllAutomationClasses.FindAll(ac => ac.GetMethods().ToList().FindAll(s => s.GetCustomAttributes(typeof(SetUp), false).Length > 0 || s.GetCustomAttributes(typeof(SetUpClass), false).Length > 0 || s.GetCustomAttributes(typeof(TearDown), false).Length > 0 ||  s.GetCustomAttributes(typeof(TearDownClass), false).Length > 0).Any());
			for(int s = 0; s < classesWithSupportMethods.Count; s++) {

				if(classesWithSupportMethods[s].GetMethods().ToList().FindAll(m => m.GetCustomAttributes(typeof(SetUp), false).Length > 0).Count > 1) {

					errors.Add(string.Format("{0}: Class \"{1}\" has more than one method marked as SetUp. This support method must be unique within the context of each test class.", errorPrefix, classesWithSupportMethods[s].Name));

				}

				if(classesWithSupportMethods[s].GetMethods().ToList().FindAll(m => m.GetCustomAttributes(typeof(SetUpClass), false).Length > 0).Count > 1) {

					errors.Add(string.Format("{0}: Class \"{1}\" has more than one method marked as SetUpClass. This support method must be unique within the context of each test class.", errorPrefix, classesWithSupportMethods[s].Name));

				}

				if(classesWithSupportMethods[s].GetMethods().ToList().FindAll(m => m.GetCustomAttributes(typeof(TearDown), false).Length > 0).Count > 1) {

					errors.Add(string.Format("{0}: Class \"{1}\" has more than one method marked as TearDown. This support method must be unique within the context of each test class.", errorPrefix, classesWithSupportMethods[s].Name));

				}

				if(classesWithSupportMethods[s].GetMethods().ToList().FindAll(m => m.GetCustomAttributes(typeof(TearDownClass), false).Length > 0).Count > 1) {

					errors.Add(string.Format("{0}: Class \"{1}\" has more than one method marked as TearDownClass. This support method must be unique within the context of each test class.", errorPrefix, classesWithSupportMethods[s].Name));

				}

			}

			#endregion

			#region Validate Automation Tests Global

			List<KeyValuePair<string,MethodInfo>> reservedCategoryNameOrInvalidCharacters = AllMethodsInFramework.FindAll(x => {

				List<object> autObjs = x.Value.GetCustomAttributes(typeof(Automation), false).ToList();
				for(int z = 0; z < autObjs.Count; z++){

					Automation aut = autObjs[z] as Automation;
					if(aut != null) {

						char[] arr = aut.CategoryName.ToCharArray();
						char[] filtered = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) 
							|| char.IsWhiteSpace(c) 
							|| c == '_'
							|| c == '/')));

						int countSlash = 0;
						while(countSlash < aut.CategoryName.Length && aut.CategoryName[countSlash] == '/') countSlash++;

						return !char.IsLetterOrDigit(arr.First()) || !char.IsLetterOrDigit(arr.First()) || aut.CategoryName.ToLower() == "all" || arr.Length != filtered.Length || countSlash > 1;

					}

				}
				return false;

			});

			if(reservedCategoryNameOrInvalidCharacters.Any()){

				errors.Add(string.Format("{0}: Test(s) \"{1}\" have an invalid category name. Category names must be alphanumeric with the execptions " +
					"being: whitespace characters, underscores, or a forward slash. The category name must begin and end with alphanumerics, and there can " +
					"only be a single forward slash, which seperates a category into a parent and sub-category. Additionally, the category name of \"All\" is " +
					"a reserved word in the Trilleon Framework used to indicate all findable tests.", errorPrefix, string.Join(", ", reservedCategoryNameOrInvalidCharacters.ExtractListOfKeysFromKeyValList(true, 1).ToArray())));

			}

			#endregion

			#region Circular Dependency Checks

			//Grab all DependencyWeb tests for each test, and build a list of tests that it is dependent on, along with a list of tests that are dependent on it. Make sure no test appears more than once in this web.
			List<KeyValuePair<string,MethodInfo>> depTestList = AllMethodsInFramework.FindAll(x => x.Value.GetCustomAttributes(typeof(DependencyWeb), false).Length > 0);
			List<KeyValuePair<string,DependencyWeb>> depWebTests = new List<KeyValuePair<string,DependencyWeb>>();
			for(int x = 0; x < depTestList.Count; x++) {

				depWebTests.Add(new KeyValuePair<string,DependencyWeb>(depTestList[x].Value.Name, ((DependencyWeb[])depTestList[x].Value.GetCustomAttributes(typeof(DependencyWeb), false)).ToList().First()));

			}

			List<string> CircularDepAlreadyDetected = new List<string>();
			for(int x = 0; x < depWebTests.Count; x++) {

				List<string> web = new List<string> { depWebTests[x].Key } ;
				web.AddRange(GatherDependencyHeirarchy(depWebTests, depWebTests[x].Key, true)); //Recursively find all tests that this origin test is directly or indirectly dependent on.
				web.AddRange(GatherDependencyHeirarchy(depWebTests, depWebTests[x].Key, false)); //Recursively find all tests that directly or indirectly rely on this origin test.

				for(int od = 0; od < web.Count; od++) {

					List<string> occurrence = web.FindAll(v => v == web[od]);
					if(occurrence.Count > 1) {

						if(CircularDepAlreadyDetected.Contains(web[od])) {

							continue;

						}
						CircularDepAlreadyDetected.AddRange(web.Distinct());
						errors.Add(string.Format("{0}: Circular dependency error detected. The following web of tests has an invalid reference that will cause the Test Runner to enter a recursive deferment loop that cannot be executed [{2}]. " +
							"No assumptions about the intended order can be made in this scenario. This can occur when the final test in a web of dependencies is marked as depending on the first test, creating a loop of dependencies that will infinitely defer execution of all tests in that web.", 
							errorPrefix, web[od], string.Join(", ", web.Distinct().ToArray())));
						break;

					}

				}

			}

			#endregion

            if(ConnectionStrategy.MaxMessageLength <= 0) {

                errors.Add(string.Format("{0}: ConnectionStrategy.MaxMessageLength bust be a positve, non-zero number. Make sure that ConnectionStrategy is finding the correct TrilleonConfig.txt value that assigns the MaxMessageLength variable.", errorPrefix));

            }

			errors = errors.Distinct(); //Remove duplicates (caused by class-based errors checked against each method in that class).
			for(int e = 0; e < errors.Count; e++) {

				AutoConsole.PostMessage(errors[e], MessageLevel.Abridged, ConsoleMessageType.AssertionFail);
				Debug.LogError(errors[e]);

			}

			for(int w = 0; w < warnings.Count; w++) {

				AutoConsole.PostMessage(warnings[w], MessageLevel.Abridged, ConsoleMessageType.AssertionSkip);
				Debug.LogWarning(warnings[w]);

			}

			Validated_Phase1 = !errors.Any();
			string message = string.Format("Framework validation [Compilation] (Phase 1) {0}!", Validated_Phase1 != null && (bool)Validated_Phase1 ? "successful" : "failed");
			AutoConsole.PostMessage(message, MessageLevel.Abridged, (bool)Validated_Phase1 ? ConsoleMessageType.AssertionPass : ConsoleMessageType.AssertionFail);
			Debug.Log(message);

		}
			
		public static List<string> GatherDependencyHeirarchy(List<KeyValuePair<string,DependencyWeb>> depWebTests, string currentTest, bool searchUp, List<string> buildingList = default(List<string>)) {

			List<string> resultsRecursive = new List<string>();
			DependencyWeb  thisDepTest = depWebTests.Find(x => x.Key == currentTest).Value;
			List<KeyValuePair<string,DependencyWeb>> directDeps = searchUp ? depWebTests.FindAll(d => thisDepTest.Dependencies.Contains(d.Key) || thisDepTest.OneOfDependencies.Contains(d.Key)) : depWebTests.FindAll(d => d.Value.Dependencies.Contains(currentTest) || d.Value.OneOfDependencies.Contains(currentTest));

			if(buildingList == default(List<string>)) {

				buildingList = new List<string> ();

			}
			if(directDeps.Count == 0) {

				return new List<string>();

			}
			for(int dd = 0; dd < directDeps.Count; dd++) {

				resultsRecursive.Add(directDeps[dd].Key);
				if(!buildingList.Contains(directDeps[dd].Key)) {

					buildingList.AddRange(resultsRecursive);
					resultsRecursive.AddRange(GatherDependencyHeirarchy(depWebTests, directDeps[dd].Key, searchUp, buildingList)); //Do the same for any children that can be found.

				}

			}
			return resultsRecursive;

		}

		public IEnumerator ValidateFramework_Phase2() {

			//Only validate once during a session/compilation.
			if(AlreadyValidated) {
				
				yield break;

			}
			AlreadyValidated = true;

			List<string> errors = new List<string>();
			if(AllMethodsInFramework.FindAll(x => x.Value.GetCustomAttributes(typeof(Validate), false).Length > 0).Count == 0) {

				errors.Add("Could not find any tests in framework marked with \"Validate\" attributes!");

			} else {

				#region Validate Actual Framework Execution

				_validationOrderRan = new List<MethodInfo>();
				for(int x = 0; x < _validationOrderRan.Count; x++) {

					Validate v = ((Validate[])_validationOrderRan[x].GetCustomAttributes(typeof(Validate), false)).ToList().First();
					if(v == null) {
						
						errors.Add(string.Format("The test method \"{0}\" should have a Validate attribute, but none could be found!", _validationOrderRan[x].Name));
						continue;

					}
					for(int e = 0; e < v.Expectations.Count; e++) {

						string testName = _validationOrderRan[x].Name;
						string error = string.Empty;
						switch(v.Expectations[e].Key) {
						case Expect.Success:
							if(!TestRunContext.Passed.Tests.Contains(testName)) {
								error = "Did Not Succeed";
							}
							break;
						case Expect.Failure:
							if(!TestRunContext.Failed.Tests.ExtractListOfKeysFromKeyValList().Contains(testName)) {
								error = "Did Not Fail";
							}
							break;
						case Expect.Ignored:
							if(!TestRunContext.Ignored.Tests.Contains(testName)) {
								error = "Was Not Ignored";
							}
							break;
						case Expect.Skipped:
							if(!TestRunContext.Passed.Tests.Contains(testName)) {
								error = "Was Not Skipped";
							}
							break;
						case Expect.OrderRan:
							int val = 0;
							bool isInt = Int32.TryParse(v.Expectations[e].Value, out val);
							if(!isInt) {
								error = string.Format("The provided value for Expect.OrderRan ({0}) could not be parsed as an integer.", v.Expectations[e].Value);
							} else if(x + 1 != val) {
								error = string.Format("Ran In Order ({0})", x + 1);
							}
							break;
						case Expect.RanAfter:
							MethodInfo findMethodAfter = _validationOrderRan.Find(n => n.Name == v.Expectations[e].Value);
							if(findMethodAfter == null) {
								error = string.Format("Could not find the Expect.RanAfter test method \"{0}\" in the test results.", v.Expectations[e].Value);
							} else if(_validationOrderRan.IndexOf(findMethodAfter) > x) {
								error = string.Format("{0} Ran Before Current Method When It Should Have Run After", findMethodAfter.Name);
							}
							break;
						case Expect.RanBefore:
							MethodInfo findMethodBefore = _validationOrderRan.Find(n => n.Name == v.Expectations[e].Value);
							if(findMethodBefore == null) {
								error = string.Format("Could not find the Expect.RanBefore test method \"{0}\" in the test results.", v.Expectations[e].Value);
							} else if(_validationOrderRan.IndexOf(findMethodBefore) < x) {
								error = string.Format("{0} Ran After Current Method When It Should Have Run Before", findMethodBefore.Name);
							}
							break;
						}

						if(!string.IsNullOrEmpty(error)) {
							errors.Add(string.Format("The test \"{0}\" failed validation. Result [{1}]", testName, error));
						}

					}

				}

				#endregion

			}

			#region Special Cases

			if(RunnerFlagTests.TearDownClassGlobalRunByTearDown) {

				errors.Add("The logic for attribute usage [TestRunnerFlag(TestFlag.DisregardTearDownClassGlobal)] failed validation. The GameMaster method TearDownClassGlobal logic ran despite this flag specifying that it should be ignored.");

			}
			if(!RunnerFlagTests.TearDownClassRun) {

				errors.Add("The test support method TearDownClass logic did not run, but should have been run afte all tests in the class executed.");

			}
			if(!RunnerFlagTests.TryCompleteAfterFailToken) {

				errors.Add("The logic for attribute usage [TestRunnerFlag(TestFlag.TryCompleteAfterFail)] failed validation. The test logic should be attempted after a failure occurs when this flag is applied.");

			}
			if(_validationOrderRan.FindAll(x => x.Name == "Flags_OnlyLaunchWhenExplicitlyCalled").Any()) {

				errors.Add("The test \"Flags_OnlyLaunchWhenExplicitlyCalled\" was launch despite being flagged as \"OnlyLaunchWhenExplicitlyCalled\".");

			}
				
			#endregion

			for(int e = 0; e < errors.Count; e++) {

				AutoConsole.PostMessage(errors[e], MessageLevel.Abridged, ConsoleMessageType.AssertionFail);
				Debug.LogError(errors[e]);

			}

			Validated_Phase2 = !errors.Any();
			string message = string.Format("Framework validation [Test Runner] (Phase 2) {0}!", Validated_Phase2 != null && (bool)Validated_Phase2 ? "successful" : "failed");
			AutoConsole.PostMessage(message, MessageLevel.Abridged, Validated_Phase2 != null && (bool)Validated_Phase2 ? ConsoleMessageType.AssertionPass : ConsoleMessageType.AssertionFail);
			Debug.Log(message);

		}

		#endregion

		IEnumerator CheckForUnexpectedErrorAlerts(Type thisType = null, bool initialized = false) {

			Q.assert.AssertNoErrorPopups();
			if(ErrorPopupDetected && !TryContinueAfterError && thisType != null) {

				//If a special error arose that we know should kill all execution, this will be run.
				string errorMessage = string.Format("#SKIPPED# Non-exception FATAL ERROR in test run has killed test execution. Error Popup Contents: {0}", ErrorPopupMessage);
				CurrentTestContext.ErrorDetails += errorMessage;
				AutomationReport.AddToReport(false, 0, true); //Save results to test run's XML file.
				TestRunContext.Skipped.Add(CurrentTestContext.Method.Name); //Add skipped Methods to list in batch context.
				yield return StartCoroutine(SingleTestLaunchCleanup(CurrentTestContext.Method.Name, thisType, initialized, CurrentMethodsHandledCount));
				ErrorPopupDetected = false;
				yield break;

			} else if(ErrorPopupDetected && thisType != null) {

				//If an error was presented to the user during the setup, execution, or tear down of the current test, this will be called.
				string errorMessage = string.Format("{0} Earlier error popup was encountered and dismissed. Automation test was still executed, but this may be related to any subsequent test failures. Previous Error Popup's Contents: {1}", WARNING_FLAG, ErrorPopupMessage);
				CurrentTestContext.ErrorDetails += errorMessage;
				yield return StartCoroutine(TakeScreenshot(false, string.Format("NonFatalErrorPopupBeforeOrDuring_{0}", CurrentTestContext.TestName)));
				//Try to dismiss error popup(s).
				yield return StartCoroutine(Q.game.TryDismissErrors());
				ErrorPopupDetected = false;

			} else if(UnexpectedErrorOccurredDuringCurrentSession && thisType != null) {

				//If an error occurred earlier in the test run, this will prepend details to all test's assertions and error details in the future.
				string prefix = ErrorOccurredBeforeTestExecutionCouldBegin ? "Game initialization error popup" : "Previous error popup";
				string errorMessage = string.Format("{0} {1} was encountered and dismissed. The current test was still executed, but this may have affected the its execution. Previous Error Popup's Contents: {2}", WARNING_FLAG, prefix, ErrorPopupMessage);
				CurrentTestContext.ErrorDetails += errorMessage;
				CurrentTestContext.AddAssertion(errorMessage);

			} else if(UnexpectedErrorOccurredDuringCurrentSession && thisType == null) {

				//If an error occurred before test execution could begin, such as during game load, this will ensure that the previous clause notes that in error logging.
				ErrorOccurredBeforeTestExecutionCouldBegin = true;

			}

			yield break;

		}

		public IEnumerator RunFloatingDependency(string dependencyName) {

			MethodInfo methodNonTest = FloatingDependencies.Find(x => x.Value.Name == dependencyName).Value;
			Type typeNonTest = methodNonTest.ReflectedType;
			MonoBehaviour monoNonTest = gameObject.AddComponent(typeNonTest) as MonoBehaviour;
			_floatingDependenciesMonos.Add(monoNonTest);
			yield return StartCoroutine((IEnumerator)methodNonTest.Invoke(monoNonTest, new object[]{ }));

		}

		/// <summary>
		/// Checks on state of test runner throughout course of test run.
		/// </summary>
		IEnumerator TestRunnerMonitor() {

			AutoConsole.PostMessage("TestRunnerMonitor Launching", MessageLevel.Abridged);
			Arbiter.LastMessage = DateTime.Now; //Set baseline.

			int intervalCheck = 0;
			while(Busy) {

            	//If it has been over 10 minutes since the last server message, stop this coroutine.
				double time = Math.Abs(DateTime.UtcNow.Subtract(Arbiter.LastMessage).TotalMinutes);
				if(time > 10 && time < 100) {

					AutoConsole.PostMessage("TestRunnerMonitor Shutting Down", MessageLevel.Abridged);
					StopCoroutine(TestRunnerMonitor());
					yield break;

				}

				yield return StartCoroutine(Q.driver.WaitRealTime(7.5f));

				if(!IgnoreMemoryTracking && intervalCheck == 0) {

					StartCoroutine(MemoryTracker.TrackMemory(string.Format("Interval Check {0}", DateTime.UtcNow.ToLongTimeString())));

				}
				intervalCheck = intervalCheck == 4 ? 0 : intervalCheck + 1;

				#if !UNITY_EDITOR
				CheckServerHeartbeat();

				double timediff = DateTime.Now.Subtract(_lastScreenshot).TotalSeconds;
				if(IsServerListening && Math.Abs(timediff) > PerformanceTracker.Screenshot_Interval) {

					_lastScreenshot = DateTime.Now;
					if(!NoIntervalScreenshots) {

						yield return StartCoroutine(TakeScreenshot(true));

					}

				}

				//If we exceed the last use time maximum, as set by Driver, then the current test has stalled and should be killed.
				double timeSinceLastDriverCall = DateTime.UtcNow.Subtract(LastUseTimer).TotalSeconds;
				if(timeSinceLastDriverCall > _maxLastUseLimit) {

					//Only kill test method if current method started before the last driver call.
					if(DateTime.UtcNow.Subtract(CurrentTestContext.StartTime).TotalSeconds > timeSinceLastDriverCall) {

						//If current method has failed to stop after a failure, ensure it is killed.
						try {

						    AutoConsole.PostMessage("Stall detected. Killing test from TestRunnerMonitor.", MessageLevel.Abridged);
						    CurrentTestMethod.Stop(); //Kill current test, only if the currently queued test has been initialized.

						} catch { }

					}

				}

				Arbiter.SendCommunication(string.Format("heartbeat_{0}", (++HeartBeatIndex).ToString()), "0");
				#endif

			}

			AutoConsole.PostMessage("TestRunnerMonitor Shutting Down", MessageLevel.Abridged);
			yield return null;

		}
		private static DateTime _lastScreenshot;

		/// <summary>
		/// Recognizes server heartbeat that extends test run timeout.
		/// </summary>
		void CheckServerHeartbeat() {

			//If a server instance launched this test run, but has ceased broadcasting heartbeats, the test run should full stop (with no reporting since nothing can receive the results).
			if(IsServerListening) {

				if(Math.Abs(LastServerHeartbeat.Subtract(DateTime.UtcNow).TotalSeconds) > SERVER_HEARTBEAT_TIMEOUT) {

					//TODO: AutoConsole.PostMessage("No Server Heartbeat Detected When Expected. Shutting Down Application.", MessageLevel.Abridged);
					//StopCoroutine("TestRunnerMonitor");
					//StopCoroutine("LaunchTests");
					//Application.Quit();

				}

			}

		}

		public static void ServerHeartbeatReceived() {

			LastServerHeartbeat = DateTime.UtcNow;

		}

		#region Test Flags

		private void HandleFlags(TestFlag flag) {

			switch(flag) {
			case TestFlag.TryCompleteAfterFail:
				TryContinueOnFailure = true;
				break;

			}

		}

		#endregion

		/// <summary>
		/// Recursively determine all dependencies needed for a successful test run when user chooses a partial test run to execute.
		/// </summary>
		/// <param name="requestedTests">Requested tests.</param>
		public List<KeyValuePair<string,MethodInfo>> GatherAllTestsThatNeedToBeRunToSatisfyAllDependenciesForPartialTestRun(List<KeyValuePair<string,MethodInfo>> requestedTests) {

			//Editor-based launch will have already compiled necessary tests, and accepted user run parameters to ignore or include dependencies.
			if(EditorBasedLaunch) {

				return requestedTests;

			}

			List<KeyValuePair<string,MethodInfo>> mutableCurrentTestList = new List<KeyValuePair<string,MethodInfo>>();
			mutableCurrentTestList.AddRange(requestedTests);
			List<KeyValuePair<string,MethodInfo>> finalTestList = new List<KeyValuePair<string,MethodInfo>>();
			finalTestList.AddRange(requestedTests);
			List<MethodInfo> checkedMethods = new List<MethodInfo>();
			List<Type> automationClasses = UnitTestMode ? GetUnityTestClasses() : GetAutomationClasses();

			KeyValuePair<string,MethodInfo> testToAdd = new KeyValuePair<string,MethodInfo>();

			while(mutableCurrentTestList.Count > 0) { 

				//List<string> keysMutableList = AutoList.ExtractListOfKeysFromKeyValList(mutableCurrentTestList, true, 1);
				List<string> keysFinalList = finalTestList.ExtractListOfKeysFromKeyValList(true, 1);

				//Remove this test from queue if it already appears in the checked list.
				if(checkedMethods.Contains(mutableCurrentTestList[0].Value)) {

					if(!keysFinalList.Contains(mutableCurrentTestList[0].Value.Name)) {

						finalTestList.Add(mutableCurrentTestList[0]);

					}

					mutableCurrentTestList.RemoveAt(0);
					continue;

				}

				//TODO: Also add Buddy.Reactions if adding a counter reaction, which assumes that a buddy will need to be involved (for sake of reciprocation).

				//Check Buddies
				BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(mutableCurrentTestList[0].Value, typeof(BuddySystem));
				if(bs != null) {
					//TODO: Buddy.Actions before this in same class may be a dependency too!
					//If this test is not a Buddy.Action test, add the referenced test(s) to list of tests being checked.
					if(bs.buddy != Buddy.Action) {

						testToAdd = AllMethodsInFramework.Find(x => x.Value.Name == bs.ReactionOf);
						finalTestList.Add(testToAdd);
						if (!mutableCurrentTestList.Contains(testToAdd) && !checkedMethods.Contains(testToAdd.Value)) {

							mutableCurrentTestList.Add(testToAdd); //Let's also check dependencies for this newly added test.

						}

					}

				}

				//Dependency Architecture checks.
				Type thisClass = Type.GetType(string.Format("{0}.{1}", TrilleonNamespace, mutableCurrentTestList[0].Key.Split(DELIMITER)[0]));
				DependencyClass dc = (DependencyClass)thisClass.GetCustomAttributes(typeof(DependencyClass), false).First();
				if(dc != null) {

					List<Type> classesWithThisId = automationClasses.FindAll(x => {
						DependencyClass attribute = (DependencyClass)x.GetCustomAttributes(typeof(DependencyClass), false).First();
						if(attribute != null) {
							if(attribute.order == dc.order) {
								return true;
							}
						}
						return false;
					});

					DependencyTest currentTestsDepTestOrder = (DependencyTest)mutableCurrentTestList[0].Value.GetCustomAttributes(typeof(DependencyTest), false).First();
					for(int c = 0; c < classesWithThisId.Count; c++) {

						List<KeyValuePair<string,MethodInfo>> methodsInThisClass = AllMethodsInFramework.FindAll(x => x.Key.Split(DELIMITER)[0] == classesWithThisId[c].Name);
						for(int m = 0; m < methodsInThisClass.Count; m++) {
							DependencyTest depTest = (DependencyTest)methodsInThisClass[m].Value.GetCustomAttributes(typeof(DependencyTest), false).First();
							if(depTest != null && currentTestsDepTestOrder != null ) {

								if(depTest.order < currentTestsDepTestOrder.order) {

									finalTestList.Add(methodsInThisClass[m]);
									if(!mutableCurrentTestList.Contains(methodsInThisClass[m]) && !checkedMethods.Contains(methodsInThisClass[m].Value)) {

										mutableCurrentTestList.Add(methodsInThisClass[m]); //Let's also check dependencies for this newly added test.

									}

								}

							}

						}

					}

				} else {

					//If this is not a DependencyClass, there still might be tests under this class with a DependencyTest order. They should be found and included in the updated dependency list.
					List<MethodInfo> ClassMethods = thisClass.GetMethods().ToList();
					for(int cm = 0; cm < ClassMethods.Count; cm++) {

						DependencyTest otherClassMethodDepTest = (DependencyTest)ClassMethods[cm].GetCustomAttributes(typeof(DependencyTest), false).First();
						if(otherClassMethodDepTest != null) {

							//This test has a DependencyTest attribute, even if the currently-checked test method does not. They are both in the same class, however, so this test must be included and take precedence.
							testToAdd = AllMethodsInFramework.Find(x => x.Value == ClassMethods[cm]);
							finalTestList.Add(testToAdd);
							if (!mutableCurrentTestList.Contains(testToAdd) && !checkedMethods.Contains(testToAdd.Value)) {

								mutableCurrentTestList.Add(testToAdd); //Let's also check dependencies for this newly added test.

							}

						}

					}

				}

				DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(mutableCurrentTestList[0].Value, typeof(DependencyTest));
				if(dt != null) {

					List<MethodInfo> testMethodsInThisClass = thisClass.GetMethods().ToList();
					for(int tm = 0; tm < testMethodsInThisClass.Count; tm++) {

						DependencyTest dtn = (DependencyTest)Attribute.GetCustomAttribute(testMethodsInThisClass[tm], typeof(DependencyTest));
						if(dtn != null) {

							if(dtn.order < dt.order) {

								testToAdd = AllMethodsInFramework.Find(x => x.Value == testMethodsInThisClass[tm]);
								finalTestList.Add(testToAdd);
								if (!mutableCurrentTestList.Contains(testToAdd) && !checkedMethods.Contains(testToAdd.Value)) {

									mutableCurrentTestList.Add(testToAdd); //Let's also check dependencies for this newly added test.

								}

							}

						}

					}

				}

				//Check DependencyWeb.
				DependencyWeb dw = (DependencyWeb)Attribute.GetCustomAttribute(mutableCurrentTestList[0].Value, typeof(DependencyWeb));
				DependencyWeb dwc = (DependencyWeb)Attribute.GetCustomAttribute(Type.GetType(string.Format("TrilleonAutomation.{0}", mutableCurrentTestList[0].Key.Split(DELIMITER).First())), typeof(DependencyWeb));
				if(dw != null || dwc != null) {

					List<string> dependencies = new List<string>();   
					if(dw != null) {
						
						dependencies.AddRange(dw.Dependencies);
						dependencies.AddRange(dw.OneOfDependencies);

					}
					if(dwc != null) {
						
						dependencies.AddRange(dwc.Dependencies);

					}

					if(dependencies.Any()) {

						for(int x = 0; x < dependencies.Count; x++) {

							testToAdd = AllMethodsInFramework.Find(y => y.Value.Name == dependencies[x]);
							finalTestList.Add(testToAdd);
							if (!mutableCurrentTestList.Contains(testToAdd) && !checkedMethods.Contains(testToAdd.Value)) {

								mutableCurrentTestList.Add(testToAdd); //Let's also check dependencies for this newly added test.

							}

						}

					}

				}

				//Add this method to list of checked methods.
				checkedMethods.Add(mutableCurrentTestList[0].Value);
				mutableCurrentTestList.RemoveAt(0);

			}

			return finalTestList.Distinct();

		}

		#region Existing Test Run Status

		private IEnumerator DetermineSecondaryCircularDependencyCause(){

			if(!DisregardDependencies) {

				AutoConsole.PostMessage("Found circular dependency.");
				List<string> failedMethods = Deferred.ExtractListOfKeysFromKeyValList<string,MethodInfo>();
				string failedMethodsString = string.Empty;
				List<string> linkedFailureCause = new List<string>();
				for (int i = 0; i < failedMethods.Count; i++) {
					
					string[] classMethod = failedMethods[i].Split(DELIMITER);
					if(!string.IsNullOrEmpty(failedMethodsString)) {
						
						failedMethodsString += ", ";

					}

					string next = string.Format("Class[{0}]-Method[{1}]", classMethod[0], classMethod[1]);
					failedMethodsString += next;
					if(i == 0 || i == failedMethods.Count - 1) {
						
						linkedFailureCause.Add(next);

					}

				}

				//Combine remaining deferred test names and report as a bulk Dependency Web failure.
				CurrentTestContext.TestName = string.Join (", ", failedMethods.ToArray ());
				CurrentTestContext.Categories = new List<string> ();
				CurrentTestContext.ClassName = string.Empty;
				string failMessage = string.Format("{0}. Could not complete the following Dependency Web test Methods regardless of order: {1}.", AutomationReport.DEPENDENCY_WEB_CIRCULAR_FAILURE_TITLE, failedMethodsString);
				yield return StartCoroutine(Q.assert.Fail(failMessage));
				CurrentTestContext.ErrorDetails = string.Format("The likely cause of this secondary circular dependency error is the dependency connection between {0} and {1} --- {2}", linkedFailureCause[0], linkedFailureCause[1], CIRCULAR_DEPENDENCY_WEB_ERROR);
				yield return StartCoroutine(Q.driver.WaitRealTime(0.25f));
				AutomationReport.AddToReport(false, _runTime); //Save results to test run's XML string builder.

			}

			yield return null;

		}

		private IEnumerator CheckMasterlessDependencies(KeyValuePair<string,MethodInfo> nextMethod, int currentIndex){

			List<string> MasterlessDependenciesHandledTestClasses = new List<string>();

			//Ignore this logic if no masterless dependencies remain to be handled.
			if(MasterlessDependencies.Any()) {
				//Ignore this logic for classes that have already been handled.
				if(!MasterlessDependenciesHandledTestClasses.Contains(nextMethod.Key.Split(DELIMITER)[0])) {

					Type thisTestClass = Type.GetType(string.Format("{0}.{1}", TrilleonNamespace, nextMethod.Key.Split(DELIMITER)[0]));
					List<MethodInfo> thisClassesTestsAll = thisTestClass.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList().FindAll(x => UnitTestMode ? x.GetCustomAttributes(typeof(UnityTest), false).Length > 0 : x.GetCustomAttributes(typeof(Automation), false).Length > 0);
					List<KeyValuePair<string[],MethodInfo>> MethodsSubListUnOrdered = new List<KeyValuePair<string[],MethodInfo>>();

					//Determine relationship between DependencyTest (and their order ID's) along with attributes and single test Methods.
					for(int c = 0; c < thisClassesTestsAll.Count; c++) {

						List<KeyValuePair<string,MethodInfo>> match = MasterlessDependencies.FindAll(x => x.Value == thisClassesTestsAll[c]);
						if(match.Any()) {

							KeyValuePair<string,MethodInfo> thisMatch = match.First();
							MasterlessDependencies.Remove(thisMatch); //Remove the match from the dependency list, as we will handle it here, and will not need it again.
							DependencyTest deptest = (DependencyTest)Attribute.GetCustomAttribute(thisMatch.Value, typeof(DependencyTest));

							MethodsSubListUnOrdered.Add(new KeyValuePair<string[], MethodInfo>(new string[] {
								thisMatch.Key,
								deptest.order.ToString()
							}, thisMatch.Value));

						}
					}

					//Filter completed tests from this logic.
					MethodsSubListUnOrdered = MethodsSubListUnOrdered.FindAll(x => {
						return !TestRunContext.CompletedTests.Contains(x.Value.Name);
					});

					//Get explicit dependency orders, and reorder tests based on this information.
					List<KeyValuePair<string,MethodInfo>> MethodsSubListOrdered = new List<KeyValuePair<string,MethodInfo>>();

					//Set new test method order.
					if(MethodsSubListUnOrdered.Any()) {

						for(int s = 1; s <= MethodsSubListUnOrdered.Count; s++) {

							//Determine next method to run in order.
							KeyValuePair<string[],MethodInfo> next = MethodsSubListUnOrdered.FindAll(x => x.Key[1] == s.ToString()).First();
							MethodsSubListOrdered.Add(new KeyValuePair<string, MethodInfo>(next.Key[0], next.Value));
							Methods = Methods.FindAll(x => x.Key != next.Key[0]);

						}

						List<KeyValuePair<string,MethodInfo>> allMethods = Methods;
						allMethods.InsertRange(currentIndex, MethodsSubListOrdered);
						Methods = allMethods;

					}

					//Add this class to list of handled classes, so that it is ignored in future masterless dependency logic.
					MasterlessDependenciesHandledTestClasses.Add(nextMethod.Key.Split(DELIMITER)[0]);

				}

			}

			yield return null;

		}

		#endregion

		public static void SetAllMethodsInFramework() {

			AllAutomationClasses = UnitTestMode ? GetUnityTestClasses() : GetAutomationClasses();
			List<MethodInfo> methodInfos = new List<MethodInfo>();
			AllMethodsInFramework = new List<KeyValuePair<string, MethodInfo>> ();

			//Build a list of tests with their type/name association as the dictionary key.
			for(int i = 0; i < AllAutomationClasses.Count; i++) {

				methodInfos = AllAutomationClasses[i].GetMethods().ToList().FindAll(y => UnitTestMode ? y.GetCustomAttributes(typeof(UnityTest), false).Length > 0 : y.GetCustomAttributes(typeof(Automation), false).Length > 0);

				for(int x = 0; x < methodInfos.Count; x++) {

					AllMethodsInFramework.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", AllAutomationClasses[i].Name, methodInfos[x].Name), methodInfos[x]));

				}   

			}

		}

		/// <summary>
		/// Gather all tests that are directly requested by the test run command.
		/// </summary>
		public List<KeyValuePair<string, MethodInfo>> GetAllMethodsToRun(string command) {

			if(!AllMethodsInFramework.Any()) {
				
				SetAllMethodsInFramework();

			}
			List<KeyValuePair<string, MethodInfo>> results = new List<KeyValuePair<string, MethodInfo>>();

			//Filter results based on requested criteria.
			switch(LaunchType) {
				case LaunchType.Mix:
					List<string> rawSplit = command.Split('%').ToList();
					List<string> tests = rawSplit.Last().Split(',').ToList().RemoveNullAndEmpty();
					List<string> categories = rawSplit.First().Split(',').ToList().RemoveNullAndEmpty();
					for(int d = 0; d < AllMethodsInFramework.Count; d++) {

						KeyValuePair<string, MethodInfo> method = AllMethodsInFramework[d];
						if(UnitTestMode) {

							UnityTest[] ut = (UnityTest[])Attribute.GetCustomAttributes(method.Value, typeof(UnityTest));
							for(int u = 0; u < ut.Length; u++) {

								string catName = ut[u].CategoryName.ToLower();
								bool match = false;
								if(catName.Contains("/")) {

									if(categories.Contains(catName) || categories.FindAll(x => catName.StartsWith(x)).Any() || categories.FindAll(x => catName.EndsWith(x)).Any()) {

										match = true;

									}

								} else {

									if(categories.Contains(catName)) {

										match = true;

									}

								}
								if(match) {

									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", method.Key.Split(DELIMITER)[0], method.Value.Name), method.Value));

								}

							}

						} else {
							
							Automation[] auto = (Automation[])Attribute.GetCustomAttributes(AllMethodsInFramework[d].Value, typeof(Automation));
							for(int at = 0; at < auto.Length; at++) {

								string catName = auto[at].CategoryName.ToLower();
								bool match = false;
								if(catName.Contains("/")) {

									if(categories.Contains(catName) || categories.FindAll(x => catName.StartsWith(x)).Any() || categories.FindAll(x => catName.EndsWith(x)).Any()) {

										match = true;

									}

								} else {

									if(categories.Contains(catName)) {

										match = true;

									}

								}
								if(match) {

									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", AllMethodsInFramework[d].Key.Split(DELIMITER)[0], AllMethodsInFramework[d].Value.Name), AllMethodsInFramework[d].Value));

								}

							}
							if(tests.Any()) {
								
								//Make sure these tests have not already been added to the results by a Class name that contains them.
								List<KeyValuePair<string, MethodInfo>> toAdd = AllMethodsInFramework.FindAll(x => tests.Contains(x.Value.Name.ToLower()));
								for(int t = 0; t < toAdd.Count; t++) {

									if(!results.FindAll(x => x.Value.Name == toAdd[t].Value.Name).Any()) {

										results.Add(toAdd[t]);

									}

								}

							}

						}

					}
					break;
				case LaunchType.MethodName:
					//Get single method from name.
					List<KeyValuePair<string, MethodInfo>> oneMethod = AllMethodsInFramework.FindAll(x => x.Key.Split(DELIMITER).Last().ToLower() == command.ToLower());
					if(oneMethod.Count > 1) {
						
						throw new Exception(string.Format("There is already a method in another class with the name \"{0}\". Do not duplicate test names!", command));
					
					} else if(oneMethod.Count == 0) {
						
						throw new Exception(string.Format("Cannot find a method with the name \"{0}\"!", command));

					} else {
						
						results = oneMethod;

					}
					break;
				case LaunchType.MultipleMethodNames:
					//Return multiple specific methods from list of names.
					List<string> names = command.ToLower().Replace(" ", string.Empty).Split(',').ToList().RemoveNulls(); 
					List<KeyValuePair<string, MethodInfo>> severalMethods = AllMethodsInFramework.FindAll(x => names.Contains(x.Key.Split(DELIMITER).Last().ToLower()));
					if(severalMethods.Count != names.Count) {
						
						throw new Exception(string.Format("{0} methods were provided in launch command, but only {1} methods were found in assembly! List of tests provided: {2}.", names.Count, severalMethods.Count, command));
					
					} else {
						
						results = severalMethods;

					}
					break;
				case LaunchType.MultipleCategoryNames:
					List<string> requestedClasses = command.Split(',').ToList();
					for (int d = 0; d < AllMethodsInFramework.Count; d++) {
						
						KeyValuePair<string, MethodInfo> method = AllMethodsInFramework[d];
						if(UnitTestMode) {

							UnityTest[] ut = (UnityTest[])Attribute.GetCustomAttributes(method.Value, typeof(UnityTest));
							for(int u = 0; u < ut.Length; u++) {

								string catName = ut[u].CategoryName.ToLower();
								bool match = false;
								if (catName.Contains("/")) {

									if(requestedClasses.Contains(catName) || requestedClasses.FindAll(x => catName.StartsWith(x)).Any() || requestedClasses.FindAll(x => catName.EndsWith(x)).Any()) {

										match = true;

									}

								} else {

									if(requestedClasses.Contains(catName)) {

										match = true;

									}

								}
								if(match) {

									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", method.Key.Split(DELIMITER)[0], method.Value.Name), method.Value));

								}

							}

						} else { 
							
							Automation[] aut = (Automation[])Attribute.GetCustomAttributes(method.Value, typeof(Automation));
							for(int a = 0; a < aut.Length; a++) {
							
								string catName = aut[a].CategoryName.ToLower();
								bool match = false;
								if (catName.Contains("/")) {
								
									if(requestedClasses.Contains(catName) || requestedClasses.FindAll(x => catName.StartsWith(x)).Any() || requestedClasses.FindAll(x => catName.EndsWith(x)).Any()) {

										match = true;

									}

								} else {
								
									if(requestedClasses.Contains(catName)) {
									
										match = true;
								
									}

								}
								if(match) {
								
									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", method.Key.Split(DELIMITER)[0], method.Value.Name), method.Value));
							
								}

							}

						}

					}
					break;
				case LaunchType.CategoryName:
					bool isTopLevelOnly = command.EndsWith("^");
					string commandTrimmed = command.TrimEnd(new char[] { '^' });
					//Check the Automation attribute value for each method, and add the matched tests to results.
					for(int d = 0; d < AllMethodsInFramework.Count; d++) {
						
						KeyValuePair<string, MethodInfo> method = AllMethodsInFramework[d];
						if(UnitTestMode) {
							
							UnityTest[] ut = (UnityTest[])Attribute.GetCustomAttributes(method.Value, typeof(UnityTest));
							for(int u = 0; u < ut.Length; u++) {
							
								string catName = ut[u].CategoryName.ToLower();
								bool match = false;
								if(catName.Contains("/")) {
								
									if(!isTopLevelOnly) {
									
										if(catName == commandTrimmed.ToLower() || catName.StartsWith(commandTrimmed.ToLower()) || catName.EndsWith(commandTrimmed.ToLower())) {

											match = true;

										}

									}

								} else {
								
									if(catName.Contains(commandTrimmed.ToLower())) {
									
										if(isTopLevelOnly) {
										
											if(catName == commandTrimmed.ToLower()) {
											
												match = true;

											}

										} else {
										
											match = true;

										}

									}

								}
								if(match) {
								
									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", method.Key.Split(DELIMITER)[0], method.Value.Name), method.Value));
							
								}

							}

						} else {

							Automation[] aut = (Automation[])Attribute.GetCustomAttributes(method.Value, typeof(Automation));
							for(int a = 0; a < aut.Length; a++) {

								string catName = aut[a].CategoryName.ToLower();
								bool match = false;
								if(catName.Contains("/")) {

									if(!isTopLevelOnly) {

										if(catName == commandTrimmed.ToLower() || catName.StartsWith(commandTrimmed.ToLower()) || catName.EndsWith(commandTrimmed.ToLower())) {

											match = true;

										}

									}

								} else {

									if(catName.Contains (commandTrimmed.ToLower())) {

										if(isTopLevelOnly) {

											if(catName == commandTrimmed.ToLower()) {

												match = true;

											}

										} else {

											match = true;

										}

									}

								}
								if(match) {

									results.Add(new KeyValuePair<string, MethodInfo>(string.Format("{0}|{1}", method.Key.Split(DELIMITER)[0], method.Value.Name), method.Value));

								}

							}

						}

					}
					break;
				case LaunchType.All:
					//Return all Automation tests.
					results = AllMethodsInFramework;
					break;
			}

			//If this is not a specified test/set of tests, remove any special tests flagged as "Only Launch When Explicitly Called".
			if(LaunchType != LaunchType.MethodName && LaunchType != LaunchType.MultipleMethodNames ) {

				List<KeyValuePair<string,MethodInfo>> sanitizedList = results.FindAll(x => {

					TestRunnerFlags flag = (TestRunnerFlags)x.Value.GetCustomAttributes(typeof(TestRunnerFlags), false).ToList().First();
					return flag == default(TestRunnerFlags) || !flag.Flags.Contains(TestFlag.OnlyLaunchWhenExplicitlyCalled);

				});

				List<string> removed = sanitizedList.GetUniqueObjectsBetween(results).ExtractListOfKeysFromKeyValList(true, 1);
				for(int s = 0; s < removed.Count; s++) {

					if(Application.isPlaying) {
						
						AutoConsole.PostMessage(string.Format("Removed the following tests flagged as \"OnlyLaunchWhenExplicitlyCalled\" from the calculated list of tests to run [{0}].", string.Join(", ", removed.ToArray())), MessageLevel.Abridged);
					
					}

				} 

				results = sanitizedList;

			}

			return results;

		}

		private List<KeyValuePair<string,MethodInfo>> OrderTests(List<KeyValuePair<string,MethodInfo>> methodsToRun) {

			List<KeyValuePair<string,MethodInfo>> results = methodsToRun;
			int intialMethodCount = methodsToRun.Count;

			if(results.Count <= 1 || DisregardDependencies) {
				
				return results;

			}

			//DependencyClass/Test handling.
			MasterDependencies = results.FindAll(x => { return x.Value.DeclaringType.GetCustomAttributes(typeof(DependencyClass), false).Length > 0; } );
			MasterlessDependencies = results.FindAll(x => { return x.Value.GetCustomAttributes(typeof(DependencyTest), false).Length > 0; } ).FindAll(x => { return !MasterDependencies.Contains(x); });
			List<KeyValuePair<int,int>> _dependencyClassCount = new List<KeyValuePair<int,int>>();
			if(MasterDependencies.Any()) {
				
				List<KeyValuePair<string, string>> dependencyOrder = new List<KeyValuePair<string, string>>();
				for(int m = 0; m < MasterDependencies.Count; m++) {
					
					//Enforce mutually exclusive nature of DependencyClass/Test and DependencyWeb.
					KeyValuePair<string,MethodInfo> thisMethod = MasterDependencies[m];
					//Add the order of each dependency test and their result's key to a list of keyvaluepairs.
					DependencyClass dc = (DependencyClass)Attribute.GetCustomAttribute(thisMethod.Value.DeclaringType, typeof(DependencyClass));
					DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(thisMethod.Value, typeof(DependencyTest));

					List<KeyValuePair<int,int>> match = _dependencyClassCount.FindAll(x => x.Key == dc.order);
					if(dt != null && match.Any()) {
						
						_dependencyClassCount.Remove(match.First());
						_dependencyClassCount.Add(new KeyValuePair<int, int>(dc.order, match.First().Value + 1));

					} else {
						
						_dependencyClassCount.Add(new KeyValuePair<int, int>(dc.order, 1));

					}

					dependencyOrder.Add(new KeyValuePair<string, string>(thisMethod.Key, string.Format("{0}|{1}", dc.order.ToString(), dt == null ? string.Format("*{0}", dc.order.ToString()) : dt.order.ToString()))); //Value of class order and then that class's test order. If non-dependency class, give asterisk in front of dep class ID.
				}

				//Order the list of keyvaluepairs exactly as they should be run.
				List<KeyValuePair<string, MethodInfo>> newResults = new List<KeyValuePair<string, MethodInfo>>();
				for(int cc = 0; cc < _dependencyClassCount.Count; cc++) {

					int dependencyClassIndex = cc + 1;

					//If this is a validation run, included the reserved DependencyClass ID of 0.
					if(ValidationRun) {
						
						dependencyClassIndex = cc;

					}

					List<KeyValuePair<string,string>> thisClassDependencyTests = dependencyOrder.FindAll(x => x.Value.Split(DELIMITER)[0] == (dependencyClassIndex).ToString() && !x.Value.Split(DELIMITER)[1].Contains("*")); //Get all DependencyTest's of this DependencyClass ID.
					for(int tc = 0; tc < thisClassDependencyTests.Count; tc++) {
						
						//Can only be a single DependencyTest in this DependencyClass with this order id. Fail if that is not the case.
						List<KeyValuePair<string,string>> thisDependency = dependencyOrder.FindAll(x => x.Value.Split(DELIMITER)[0] == (dependencyClassIndex).ToString() && x.Value.Split(DELIMITER)[1] == (tc+1).ToString());
						KeyValuePair<string,MethodInfo> thisMethod = results.FindAll(x => x.Key == thisDependency.First().Key).First(); //Unique key, so we expect only Single is possible anyway.
						if(thisMethod.Key == null) {
							
							Debug.Log("null");

						}
						newResults.Add(thisMethod); 
						results.Remove(thisMethod); //Temporarily remove this method from the results list.

					}

					//Add all tests in DependencyClass that lack a DependencyTest tag after the ranked tests; in no particular order.
					List<KeyValuePair<string,string>> thisClassNonDependencyTests = dependencyOrder.FindAll(x => {
						string order = x.Value.Split(DELIMITER)[1];
						bool match = false;
						if(order.Contains("*")){
							order = order.Replace("*", string.Empty);
							match = order == (cc+1).ToString();
						}
						return match;
					}); //Get all Non-DependencyTest tests of this DependencyClass.

					for(int ndt = 0; ndt < thisClassNonDependencyTests.Count; ndt++) {
						
						KeyValuePair<string,MethodInfo> thisMethod = results.FindAll(x => x.Key == thisClassNonDependencyTests[ndt].Key).First();
						if(thisMethod.Key == null) {
							
							Debug.Log("null");

						}
						newResults.Add(thisMethod);
						results.Remove(thisMethod); //Temporarily remove this method from the results list.

					}

				}

				//Add the remaining list of test Methods to the end of the new list that has ordered the master dependencies.
				newResults.AddRange(results);
				results = newResults;
			}
			results = results.FindAll(n => n.Value != null && n.Key != null);

			//While Buddy System tests are removed from the main method list after this point, the current method count should equal the initial count.
			if(intialMethodCount != results.Count) {
				
				throw new UnityException("An error occurred in the ordering logic that resulted in a different number of tests upon completion. Debug the logic to locate the source of the issue as there are multiple potential points of failure that cannot be known from examining the results.");
			
			}

			//BuddySystem handling.
			BuddyHandler.Buddies = new List<KeyValuePair<KeyValuePair<string,MethodInfo>,List<KeyValuePair<string,MethodInfo>>>>();
			BuddyHandler.BuddySystemMarked = results.FindAll(x => {
				return x.Value.GetCustomAttributes(typeof(BuddySystem), false).Length > 0;
			});
			List<KeyValuePair<string,MethodInfo>> buddySystemActors = BuddyHandler.BuddySystemMarked.FindAll(x => {
				BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(x.Value, typeof(BuddySystem));
				return bs.buddy == Buddy.Action;
			});

			for(int b = 0; b < buddySystemActors.Count; b++) {

				//Current actor method.
				KeyValuePair<string,MethodInfo> actorBuddy = buddySystemActors[b];

				//List of all reactors declaring this method as their actor.
				List<KeyValuePair<string,MethodInfo>> reactorBuddies = BuddyHandler.BuddySystemMarked.FindAll(x => {
					BuddySystem bs = (BuddySystem)Attribute.GetCustomAttribute(x.Value, typeof(BuddySystem));
					return bs.ReactionOf == actorBuddy.Value.Name;
				});

				BuddyHandler.BuddySystemMarked = BuddyHandler.BuddySystemMarked.GetUniqueObjectsBetween(reactorBuddies); //Remove handled _methods to make process faster next iteration of the loop.
				results = results.FindAll(x => x.Key != buddySystemActors[b].Key); //Remove buddy action method from main method list.
				results = results.GetUniqueObjectsBetween(reactorBuddies); //Remove buddy reaction methods from main method list.

				//Add this grouping of actor buddies and their reactors.
				BuddyHandler.Buddies.Add(new KeyValuePair<KeyValuePair<string,MethodInfo>,List<KeyValuePair<string,MethodInfo>>>(actorBuddy, reactorBuddies));
				buddyCount += reactorBuddies.Count + 1;

			}

			return results ;

		}

		/// <summary>
		/// Report test success or failure.
		/// </summary>
		public void ReportOnTest(string reportOnMessage = "", bool isFinalReport = false, MessageLevel messageLevel = MessageLevel.Verbose) {

			string message = !string.IsNullOrEmpty(reportOnMessage) ? reportOnMessage : 
				string.Format("[{0}] {1}{2}", CurrentTestContext.TestName, CurrentTestContext.IsSuccess ? "#Pass#" : 
					"#Fail#", CurrentTestContext.IsSuccess ? string.Empty : string.Format(" -- {0}", CurrentTestContext.Assertions.Last()));
			AutoConsole.PostMessage(message, messageLevel);

		}

		/// <summary>
		/// Launches SetUp/Class, TearDown/Class Methods.
		/// </summary>
		/// <returns>The support method.</returns>
		/// <param name="type">Type.</param>
		/// <param name="supportAttributeName">Support attribute's name.</param>
		private IEnumerator LaunchSupportMethod(Type type, string supportAttributeName) {
			
			string message = string.Empty;
			List<MethodInfo> result = new List<MethodInfo>();
			switch(supportAttributeName.ToLower()) {
				case "setupclass":
					if(_classSupportMethodsHandledTestRun.FindAll(s => s.Key == type.Name && s.Value).Any()) {

						break; //This can only be run once in a test run, even if a test is deferred, and thus the last test run was a different class.

					}
					ExecutionContext = CurrentExecutionContext.SetUpClass;
					message = string.Format("Set Up Class: {0}", type.Name);
					result = GetClassSetup(type);
						//Check whether or not this class SetUp has run before, which potentially occurs for deferred tests in the test run.
					if(result.Any() && _classSetUpRan.Contains(type)) {
						SetUpClass setUpClass = (SetUpClass)result.First().GetCustomAttributes(typeof(SetUpClass), false).First();
						if(setUpClass != null) {
							if(!setUpClass.RunAgainForDeferredTests) {
								yield break; // Do not run this setup again. This occurs when attribute is explicitly told to not run class SetUp for deferred tests.
							}
						}
					}
					_classSetUpRan.Add(type);
					_classSupportMethodsHandledTestRun.Add(new KeyValuePair<string,bool>(type.Name, true));
					break;
				case "setupclasstest":
					ExecutionContext = CurrentExecutionContext.SetUp;
					message = string.Format("Set Up Test: {0}", CurrentTestContext.TestName);
					result = GetClassTestSetup(type);
					break;
				case "teardownclass":
					if(_classSupportMethodsHandledTestRun.FindAll(s => s.Key == type.Name && !s.Value).Any()) {

						break; //This can only be run once in a test run, even if a test is deferred, and thus the last test run was a different class.

					}
					ExecutionContext = CurrentExecutionContext.TearDownClass;
					message = string.Format("Tear Down Class: {0}", type.Name);
					result = GetClassTearDown(type);

					//Check whether or not this class TearDown has run before, which potentially occurs for deferred tests in the test run.
					if(result.Any() && _classTearDownRan.Contains(type)) {
					
						TearDownClass tearDownClass = (TearDownClass)result.First().GetCustomAttributes(typeof(TearDownClass), false).First();
						if(tearDownClass != null) {
						
							if(!tearDownClass.RunAgainForDeferredTests) {
							
								yield break; // Do not run this teardown again. This occurs when attribute is explicitly told to not run class TearDown for deferred tests.

							}

						}

					}
					_classTearDownRan.Add(type);
					_classSupportMethodsHandledTestRun.Add(new KeyValuePair<string,bool>(type.Name, true));
					break;
				case "teardownclasstest":
					ExecutionContext = CurrentExecutionContext.TearDown;
					message = string.Format("Tear Down Test: {0}", CurrentTestContext.TestName);
					result = GetClassTestTearDown(type);
					break;
				default:
					throw new UnityException(string.Format("Invalid Support Attribute Name \"{0}\"", supportAttributeName));
			}

			if(result.Any()) {

				AutoConsole.PostMessage(message);
				IEnumerator routine = (IEnumerator)result.First().Invoke(CurrentMonoBehaviourInstance, new object[]{ }); //Get the test method as an IEnumerator so that it can be launched as a coroutine. 
                StoppableCoroutine supportMethod = CurrentMonoBehaviourInstance.StartCoroutineEx(routine);
				yield return supportMethod.WaitFor(); //Begin method and wait for completion.

			} else {

				yield return null;

			}

		}

		/// <summary>
		/// Get all classes that have the AutomationClass attribute assigned to them.
		/// </summary>
		/// <returns>The automation classes.</returns>
		public static List<Type> GetAutomationClasses(bool isEditor = false) {

			List<Type> typesAll = new List<Type>();
			List<Assembly> assembliesAll = AppDomain.CurrentDomain.GetAssemblies().ToList();
			for(int x = 0; x < assembliesAll.Count; x++) {
				
				typesAll.AddRange(assembliesAll[x].GetTypes());

			}

			//If the caller is not the test runner (Editor Window) or the test run is requesting all tests in the suite.
			if(LaunchType == LaunchType.All && !isEditor) {
				
				//Remove DebugClass types as they cannot be run in a "run all tests" context.
				return typesAll.FindAll(x => x.GetCustomAttributes(typeof(AutomationClass), false).Length > 0 && x.GetCustomAttributes(typeof(DebugClass), false).Length <= 0);

			} else {
				
				return typesAll.FindAll(x => x.GetCustomAttributes(typeof(AutomationClass), false).Length > 0);

			}

		}

		/// <summary>
		/// Get all classes that have the UnityTestClass attribute assigned to them.
		/// </summary>
		/// <returns>The automation classes.</returns>
		public static List<Type> GetUnityTestClasses() {

			List<Type> typesAll = new List<Type>();
			List<Assembly> assembliesAll = AppDomain.CurrentDomain.GetAssemblies().ToList();
			for(int x = 0; x < assembliesAll.Count; x++) {

				typesAll.AddRange(assembliesAll[x].GetTypes());

			}

			return typesAll.FindAll(x => x.GetCustomAttributes(typeof(UnityTestClass), false).Length > 0);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The automation classes.</returns>
		private static List<MethodInfo> GetClassTestSetup(Type type) {
			
			return type.GetMethods().ToList().FindAll(x => x.GetCustomAttributes(typeof(SetUp), false).Length > 0);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The automation classes.</returns>
		private static List<MethodInfo> GetClassSetup(Type type) {
			
			return type.GetMethods().ToList().FindAll(x => x.GetCustomAttributes(typeof(SetUpClass), false).Length > 0);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The automation classes.</returns>
		private List<MethodInfo> GetClassTestTearDown(Type type) {
			
			return type.GetMethods().ToList().FindAll(x => x.GetCustomAttributes(typeof(TearDown), false).Length > 0);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The automation classes.</returns>
		private static List<MethodInfo> GetClassTearDown(Type type) {
			
			return type.GetMethods().ToList().FindAll(x => x.GetCustomAttributes(typeof(TearDownClass), false).Length > 0);

		}

		#region Automation Server Commands

		public void TakeScreenshotAsync(bool isInterval = false, string overrideName = "") {

			StartCoroutine(TakeScreenshot(isInterval, overrideName));

		}

		public IEnumerator ShutDownApplicationAfterFatalException() {

			if(!Application.isEditor && IsServerListening) {

				AutoConsole.PostMessage("Application is not running in editor, and a server is listening; auto shut down activated. Shutting down application in 20 seconds.", MessageLevel.Abridged);
				yield return StartCoroutine(Q.driver.WaitRealTime(20f));
				Application.Quit();

			}
			yield return null;

		}

		/// <summary>
		/// Tell Appium server driver to take a screenshot of the device. Nothing will happen if there is no Appium server.
		/// </summary>
		/// <returns>The screen shot.</returns>
		public IEnumerator TakeScreenshot(bool isInterval = false, string overrideName = "") {

			if((UnitTestMode && !Assert.UnitTestStepFailure) || _dependencyLogJam) {

				yield break;

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(0.25f));

			string name = isInterval ? "Interval" : string.IsNullOrEmpty(overrideName) ? CurrentTestContext.TestName : overrideName;
			#if UNITY_EDITOR
			yield return StartCoroutine(AutomationReport.TakeScreenshot(name));
			#else
			int timeout = 20;
			int timer = 0;
			AwaitingScreenshot = true;
			AutoConsole.PostMessage(string.Format("request_screenshot||{0}||FPS||{1}||", name, Convert.ToDouble(Math.Round(1.0f / Time.deltaTime))), MessageLevel.Abridged);
			while(AwaitingScreenshot && timer < timeout) {

				yield return StartCoroutine(Q.driver.WaitRealTime(1));
				timer++;

			}
			if(timer >= timeout) {

				AutoConsole.PostMessage(string.Format("Failed to receive screenshot success alert for {0}; moving on.", CurrentTestContext.TestName), MessageLevel.Abridged);

			} else {

				AutoConsole.PostMessage(string.Format("Screenshot success acknowledged. Moving on.", CurrentTestContext.TestName), MessageLevel.Abridged);

			}
			AwaitingScreenshot = false;
			#endif

			yield return null;

		}

		#endregion

		#region Save Test Run Results

		public void HandleReportSending(string htmlBody) {

			//Stop here so that unhandled exceptions in main test run enumerator do not cause StopCoroutine from being run. The alternative being spam on pubsub server until application is killed on device.
			StopCoroutine("TestRunnerMonitor"); 

			if(!ValidationRun) {

				StartCoroutine(SendTestResultsReport(htmlBody));

			}

		}

		public IEnumerator SendTestResultsReport(string htmlBody) {

			AutoConsole.PostMessage(_launchCommand, MessageLevel.Verbose);
			#if UNITY_EDITOR
			//Save html file to root directory instead.
			if (!Directory.Exists(Path.GetDirectoryName(FileBroker.REPORTS_DIRECTORY))) {
				
				Directory.CreateDirectory(Path.GetDirectoryName(FileBroker.REPORTS_DIRECTORY));

			}

			string reportDirectory = string.Format("{0}{1}", FileBroker.REPORTS_DIRECTORY, TestRunLocalDirectory);
			FileBroker.SaveUnboundFile(string.Format("{0}/report.html", reportDirectory), htmlBody.ToString());

			//Save simple details to associated meta file for use in Test editor window.
			StringBuilder metaFile = new StringBuilder();
			metaFile.AppendLine(string.Format("RunCommand:{0}", _launchCommand));
			metaFile.AppendLine(string.Format("Passes:{0}", TestRunContext.Passed.Tests.Count));
			metaFile.AppendLine(string.Format("Fails:{0}", TestRunContext.Failed.Tests.Count));
			metaFile.AppendLine(string.Format("Skips:{0}", TestRunContext.Skipped.Tests.Count));
			metaFile.AppendLine(string.Format("Ignores:{0}", TestRunContext.Ignored.Tests.Count));
			FileBroker.SaveUnboundFile(string.Format("{0}/report.meta", reportDirectory), metaFile.ToString());
			#endif

			yield return null;

		}

		#endregion

		const string CIRCULAR_DEPENDENCY_WEB_ERROR = "Additional Details: Recursive dependencies are accounted" +
			" for in initial automation validation. A recursive dependency is a method that calls itself as a dependency. Additionally, circular dependencies" +
			" between two Methods are accounted for. This means that if Method A declares Method B as a dependency, and Method B declares Method A as a dependency," +
			" then this test-setup error will be caught and displayed explicitly in error logs. However, more complex circular dependencies involving 3 or more test" +
			" Methods are not caught in initial validation, and would ultimately cause all methods caught in this circular dependency to be included in this error. " +
			"An example of this might include 10 test methods; If each method in this list of 10 declares one after another as a dependency, and then the 10th test " +
			"declares the 1st test as a dependency, then there will be a critical deferrence failure. You can use the provided class|test combinations to trace and " +
			"repair this malformed dependency web.";

	}

}