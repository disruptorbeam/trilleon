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

﻿//TODO: Remove pragma; Update `new EditorWindow` instances to get rid of "must instantiate with ScriptableObject" errors.
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
					_self.Initialize(PlayModeStateChange.EnteredEditMode);
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
					GameObject tmh = GameObject.Find(TestMonitorHelpers.NAME);
					if(tmh == null) {
						TestMonitorHelpers.CreateTestObjectHelper();
						tmh = GameObject.Find(TestMonitorHelpers.NAME);
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
		public CommandConsoleView CommandConsoleView;
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

		//Set Nexus as the primary EditorWindow under Trilleon. Can be opened using shortcut "option + shift + a".
		[MenuItem("Trilleon/Nexus #&a", false, 1)]
		static void ShowWindow() {

            Self = ShowWindow<Nexus>(typeof(Nexus), "Nexus", DockNextTo.Default, "Trilleon Automation tools and support window.");

		}

		void OnEnable() {

			//If AutomationMaster has not been initialized, we may encounter errors using the Nexus window. Attaching the AutoConsole listener will allow for more helpful explanations.
			if(!AutomationMaster.Initialized) {

				Application.logMessageReceived -= AutoConsole.GetLog; //Detach if already attached.
				Application.logMessageReceived += AutoConsole.GetLog; //Attach handler to recieve incoming logs.

			}
			Application.logMessageReceived += ExceptionCallback;

			//Windows
			Architect = new DependencyArchitecture();
			BuddyData = new BuddyData();
			Console = new RunnerConsole();
			CommandConsoleView = new CommandConsoleView();
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

			//Custom defined Tab order set from Additional Tools?
            List<string> tabOrderData = Customizer.Self.GetString("nexus_custom_tab_order").Split(AutomationMaster.DELIMITER).ToList().RemoveNullAndEmpty();
			Dictionary<string,int> tabData = new Dictionary<string,int>();
			for(int t = 0; t < tabOrderData.Count; t++) {

                tabData.Add(tabOrderData[t], t + 1);

			}

			TabDetails tab;

			//Test Manager Tab.
			int priorityId = 0;
			tabData.TryGetValue(Tests.GetType().Name, out priorityId);
			tab = new TabDetails(Tests, priorityId > 0 ? priorityId : 1);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha1 };
			tab.Tabs.Add(new SizedTab("⇊", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Tests", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Tests View", TabSize.Large));
			AddWindowTab(tab);

			//Automation Console Window Tab.
			tabData.TryGetValue(Console.GetType().Name, out priorityId);
			tab = new TabDetails(Console, priorityId > 0 ? priorityId : 2);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha2 };
			tab.Tabs.Add(new SizedTab("✦", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Console", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Auto Console", TabSize.Large));
			tab.OverrideScroll = true;
			AddWindowTab(tab);

			//Command Console View Tab.
			tabData.TryGetValue(CommandConsoleView.GetType().Name, out priorityId);
			tab = new TabDetails(CommandConsoleView, priorityId > 0 ? priorityId : 3);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha3 };
			tab.Tabs.Add(new SizedTab("☊", TabSize.Small, 22));
			tab.Tabs.Add(new SizedTab("Commands", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Commands View", TabSize.Large));
			tab.OverrideScroll = true;
			AddWindowTab(tab);

			//Test Results Window Tab.
			tabData.TryGetValue(Results.GetType().Name, out priorityId);
			tab = new TabDetails(Results, priorityId > 0 ? priorityId : 4);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha4 };
			tab.Tabs.Add(new SizedTab("☑", TabSize.Small, TextGreen, 18));
			tab.Tabs.Add(new SizedTab("Results", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Tests Results", TabSize.Large));
			AddWindowTab(tab);

			//Generator Window Tab.
			tabData.TryGetValue(Generator.GetType().Name, out priorityId);
			tab = new TabDetails(Generator, priorityId > 0 ? priorityId : 5);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha5 };
			tab.Tabs.Add(new SizedTab("●", TabSize.Small, Color.red, 26));
			tab.Tabs.Add(new SizedTab("Generator", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Generator", TabSize.Large));
			tab.OverrideScroll = true;
			AddWindowTab(tab);

			//Architect Hidden Tab
			tabData.TryGetValue(Architect.GetType().Name, out priorityId);
			tab = new TabDetails(Architect, priorityId > 0 ? priorityId : 6);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha6 };
			tab.Tabs.Add(new SizedTab("❖", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Dep Arc", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Dependency", TabSize.Large));
			AddWindowTab(tab);

			//Buddy Data Hidden Tab
			tabData.TryGetValue(BuddyData.GetType().Name, out priorityId);
			tab = new TabDetails(BuddyData, priorityId > 0 ? priorityId : 7);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha7 };
			tab.Tabs.Add(new SizedTab("❦", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Buddy", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Buddy Data", TabSize.Large));
			AddWindowTab(tab);

			//Settings Hidden Tab
			tabData.TryGetValue(Settings.GetType().Name, out priorityId);
			tab = new TabDetails(Settings, priorityId > 0 ? priorityId : 8);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha8 };
			tab.Tabs.Add(new SizedTab("✎", TabSize.Small, 18));
			tab.Tabs.Add(new SizedTab("Settings", TabSize.Medium));
			tab.Tabs.Add(new SizedTab("Settings", TabSize.Large));
			AddWindowTab(tab);

			//Other Tools Tab.
			tab = new TabDetails(Tools, 0);
			tab.Shortcut = new List<KeyCode> { KeyCode.Alpha9 };
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

			SelectTab(SwatWindows.Find(x => x.PriorityID ==  1).Window); //Sets this as the default landing on load.

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

			//Hide loading panel if Automation is successfully executing, or an exception was encountered afte pressing "Play" in Nexus window.
			if((AutomationMaster.Busy && Application.isPlaying) || (!Application.isPlaying && AutoConsole.Logs.FindAll(x => x.type == LogType.Exception && x.stackTrace.Contains("TrilleonAutomation.") && !x.stackTrace.Contains("/Editor/")).Any())) {

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

		/// <summary>
		/// If an exception occurs, make sure that loading overlay is removed.
		/// </summary>
		static void ExceptionCallback(string message, string stackTrace, LogType type) {
				
			if(type == LogType.Exception || type == LogType.Error) {
				
				Nexus.Self.HideLoading();

			}

		}

	}

}