//TODO: Remove pragma; Update `new EditorWindow` instances to get rid of "must instantiate with ScriptableObject" errors.
/* This warning is not a problem programmatically because the ScriptableObject.CreateInstance<>() call is indeed being used when
 * the actual window is being shown. The "new EditorWindow" calls merely instantiate the class allowing the "Show()" method to be called.
*/
#pragma warning disable 

using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

	public class Nexus : Swat {

		public static Nexus Self { 
			get { 
				if(_self == null && !Swat.Closing(typeof(Nexus))) {
					if(Overseer.nexus == null || (Nexus)Overseer.nexus == null) {
						_self = GetWindow<Nexus>();
					} else {
						_self = (Nexus)Overseer.nexus;
					}
					_self.Initialize();
				}
				return _self;
			}
			private set { 
				Overseer.nexus = value;
				_self = value;
			}
		}
		public static Nexus _self;

		public static SimpleAlert Alert { 
			get { 
				if(_alert == null) {
					_alert = GetWindow<SimpleAlert>();
				}
				return _alert;
			}
			private set { 
				_alert = value;
			}
		}
		public static SimpleAlert _alert;

		public static Overseer Overseer {
			get {
				if(_objectOverseer == null) {
					GameObject tmh = GameObject.Find("TestMonitorHelper");
					if(tmh == null) {
						TestMonitorHelpers.CreateTestObjectHelper();
						tmh = GameObject.Find("TestMonitorHelper");
					}
					_objectOverseer = tmh.GetComponent<Overseer>();
				}
				return _objectOverseer;
			}
		}
		public static Overseer _objectOverseer;

		public static AutomationMaster AutoMaster {
			get {
				if(_autoMaster == null) {
					_autoMaster = TestMonitorHelpers.Helper.GetComponent<Overseer>().AutomationMaster;
				}
				return _autoMaster;
			}
		}
		static AutomationMaster _autoMaster;

		public static RectOffset BaseRectOffset {
			get {
				return new RectOffset(10, 0, 0, 0);
			}
		}

		public static bool IsRunAll { get; set; }
		public static bool IsEditorRunning { get; set; }

		public DependencyArchitecture Architect;
		public BuddyData BuddyData;
		public RunnerConsole Console;
		public Generator Generator;
		public RunResults Results;
		public Settings Settings;
		public TestManifest Tests;
		public AdditionalTools Tools;
		public Favorites Favorites;
		public Manifest Manifest;

		public static bool _launched, _hardStop, _isBuddySet, isPc, _resetOnStart, _showFileDeletedMessage = false;
		public static bool _isPrimaryBuddy, _setOnce, _update_data;
		int _selectedCategory;
		int _buddySelection;
		GUIStyle back, sl, r, fo, buddies, filter, closeBox, horizontalToolbarGroup, toolBarButtons, testStatusBox, b, h,launchCat;
		AnimBool m_ShowExtraFields;

		//Set Nexus as the primary EditoWindow under Trilleon. Can be opened using shortcut "cntrl+ shift + z".
		[MenuItem("Trilleon/Nexus #&a", false, 1)]
		static void ShowWindow() {

			Self = ShowWindow<Nexus>(typeof(Nexus), "Nexus");

		}

		void OnEnable() {

			//Windows
			Architect = new DependencyArchitecture();
			BuddyData = new BuddyData();
			Console = new RunnerConsole();
			Generator = new Generator();
			Results = new RunResults();
			Settings = new Settings();
			Tests = new TestManifest();
			Tools = new AdditionalTools();
			Favorites = new Favorites();
			Manifest = new Manifest();

			Application.runInBackground = true;
			EditorApplication.playmodeStateChanged += PlaymodeStateChangeUpdates;
			Set();

			m_ShowExtraFields = new AnimBool(true);
			m_ShowExtraFields.valueChanged.AddListener(Repaint);

			TabDetails tab;

			//Test Manager Tab.
			tab = new TabDetails(Tests, 1);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha1 };
			tab.Tabs.Add(new SizedTab("⇊", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Tests", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Tests View", TabSize.Large));
			AddWindowTab(tab);

			//Automation Console Window Tab.
			tab = new TabDetails(Console, 2);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha2 };
			tab.Tabs.Add(new SizedTab("✦", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Console", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Console", TabSize.Large));
			tab.OverrideScroll = true;
			AddWindowTab(tab);

			//Test Results Window Tab.
			tab = new TabDetails(Results, 3);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha3 };
			tab.Tabs.Add(new SizedTab("☑", TabSize.Small, TextGreen, 18));
			tab.Tabs.Add(new SizedTab("Results", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Tests Results", TabSize.Large));
			AddWindowTab(tab);

			//Generator Window Tab.
			tab = new TabDetails(Generator, 4);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha4 };
			tab.Tabs.Add(new SizedTab("●", TabSize.Small, Color.red, 26));
			tab.Tabs.Add(new SizedTab("Generator", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Generator", TabSize.Large));
			tab.OverrideScroll = true;
			AddWindowTab(tab);

			//Architect Hidden Tab
			tab = new TabDetails(Architect, 5);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha5 };
			tab.Tabs.Add(new SizedTab("❖", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Dep Arc", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Dependency", TabSize.Large));
			AddWindowTab(tab);

			//Buddy Data Hidden Tab
			tab = new TabDetails(BuddyData, 6);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha6 };
			tab.Tabs.Add(new SizedTab("❦", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Buddy", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Buddy Data", TabSize.Large));
			AddWindowTab(tab);

			//Settings Hidden Tab
			tab = new TabDetails(Settings, 7);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha7 };
			tab.Tabs.Add(new SizedTab("✎", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Settings", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Settings", TabSize.Large));
			AddWindowTab(tab);

			//Other Tools Tab.
			tab = new TabDetails(Tools, 0);
			tab.Shortcut = new List<KeyCode> { KeyCode.LeftAlt, KeyCode.Alpha8 };
			tab.Tabs.Add(new SizedTab("✙", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Tools", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Other Tools", TabSize.Large));
			AddWindowTab(tab);

			//Favorites Tab (Reusable Test Runs).
			tab = new TabDetails(Favorites, -1);
			AddWindowTab(tab);

			//Manifest Tab (Current Test Run).
			tab = new TabDetails(Manifest, -1);
			AddWindowTab(tab);

			AddPopup(ScriptableObject.CreateInstance<SimpleAlert>());
			AddPopup(ScriptableObject.CreateInstance<RunTestsAlert>());
			AddPopup(ScriptableObject.CreateInstance<RunClassesAlert>());
			AddPopup(ScriptableObject.CreateInstance<ConsoleMessage>());

			SelectTab(Tests); //Sets this as the default landing on load.

		}

		void Set() {

			_isPrimaryBuddy = _setOnce = Tests.Update_Data = true;
			isPc = Application.platform == RuntimePlatform.WindowsEditor;
			AutomationMaster.OnNewTestLaunch.AddIfNotDuplicate(new OnNewTestLaunch(NewTestLaunched));
			_launched = AutomationMaster.Busy;

			if(TestMonitorHelpers.Helper == null || Overseer == null) {

				TestMonitorHelpers.CreateTestObjectHelper();

			}

		}

		void OnGUI() {

			//Stop running Automation processes.
			if(AutomationMaster.Busy && Application.isPlaying) {

				Nexus.Self.HideLoading();

			}

			if(TestMonitorHelpers.Helper == null || Overseer == null) {

				TestMonitorHelpers.CreateTestObjectHelper();

			}

			Render();

		}

		public void NewTestLaunched() {

			Tests.Update_Data = true;

		}

		/// <summary>
		/// Used to make any updates necessary to the Nexus when playmode state has changed between play/pause/stop.
		/// </summary>
		static void PlaymodeStateChangeUpdates() {

			if(EditorApplication.isPlaying) {

				//If any loading screen was rendering before stop was pressed, it has nothing to trigger its removal.
				Nexus.Self.HideLoading();

			}

		}

	}

}