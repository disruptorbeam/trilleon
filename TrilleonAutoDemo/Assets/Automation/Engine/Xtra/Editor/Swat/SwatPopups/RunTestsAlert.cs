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
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {

	public class RunTestsAlert : SwatPopup {

		Type constraint = typeof(TestManifest); //This popup can ONLY be opened by the TestManifest Swat Window, and can only stay open as long as that is the selected tab.

		List<KeyValuePair<string, MethodInfo>>  requestedTests = new List<KeyValuePair<string, MethodInfo>>();
		List<string> requestedTestNames = new List<string>();
		List<KeyValuePair<string, MethodInfo>>  additionalTests = new List<KeyValuePair<string, MethodInfo>>();
		List<string> additionalTestNames = new List<string>();
		List<bool> additionalTestsToggleList = new List<bool>();
		string testName = string.Empty;
		string testType = string.Empty;
		bool isTestSubset = false;
		bool isSubsetRun = false;
		bool positionWindow = true;
		float lastWindowWidth = 0;

		int foldoutHeaderHeight = 20;
		int buttonHeight = 25;
		int buttonWidth = 75;
		int headerHeight = 20;
		int breakSpace = 40;

		Vector2 scrollY;
		GUIStyle additionalTestsStyle, buttons, divider, fo, header, requiredTestsStyle, scrollBar, testsToggle, toggleGroup;

		public static void Pop(List<KeyValuePair<string, MethodInfo>> requestedTests, List<KeyValuePair<string, MethodInfo>> additionalTests, string testName, string testType, bool isTestSubset = false) {

			EditorWindow.GetWindow<RunTestsAlert>().Close();
			ScriptableObject.CreateInstance<RunTestsAlert>().Set(requestedTests, additionalTests, testName, testType, isTestSubset);

		}
		public void Set(List<KeyValuePair<string, MethodInfo>> requestedTests, List<KeyValuePair<string, MethodInfo>> additionalTests, string testName, string testType, bool isTestSubset) {

			if(Nexus.Self.SelectedTab.Window.GetType() != constraint) {

				throw new UnityException("This popup is designed to only be called by the Test Manifest Swat Window.");

			}

			RunTestsAlert pop = ScriptableObject.CreateInstance<RunTestsAlert>();
			pop.IsVisible = true;
			pop.positionWindow = true;
			pop.requestedTestNames = new List<string>();
			pop.requestedTests = requestedTests;
			pop.additionalTestsToggleList = new List<bool>();
			pop.additionalTestNames = new List<string>();
			pop.additionalTests = additionalTests;
			pop.additionalTests = additionalTests != null ? additionalTests.OrderByKeys() : null;

			//Alphabetically order data.
			List<KeyValuePair<string, MethodInfo>> reorderedList = new List<KeyValuePair<string, MethodInfo>>();
			List<string> rawTests = requestedTests.ExtractListOfKeysFromKeyValList(true, 1);
			rawTests.Sort();
			for(int x = 0; x < rawTests.Count; x++) {

				reorderedList.Add(requestedTests.Find(k => k.Key.Contains(string.Format("{0}{1}", AutomationMaster.DELIMITER, rawTests[x]))));

			}
			pop.requestedTests = reorderedList;

			if(additionalTests == null) {

				pop.isSubsetRun = true;

			} else {

				reorderedList = new List<KeyValuePair<string, MethodInfo>>();
				rawTests = additionalTests.ExtractListOfKeysFromKeyValList(true, 1);
				rawTests.Sort();
				for(int x = 0; x < rawTests.Count; x++) {

					reorderedList.Add(additionalTests.Find(k => k.Key.Contains(string.Format("{0}{1}", AutomationMaster.DELIMITER, rawTests[x]))));

				}
				pop.additionalTests = reorderedList;

			}

			pop.testName = testName;
			pop.testType = testType;

			//If true, this is a popup from the Test view of the TestManifest window, where all tests in the framework are listed, and should be unselected by default.
			pop.isTestSubset = isTestSubset;

			pop.ShowPopup();

		}

		public override bool Visible() {

			return IsVisible;

		}
		public bool IsVisible { get; set; }

		public override void PositionWindow() {

			int foldoutHeight = (additionalTests == null ? 1 : additionalTests.Count * 12) + (foldoutHeaderHeight * 2) + headerHeight + buttonHeight + (breakSpace * 2) + 250;
			position = new Rect(Nexus.Self.position.x + (Nexus.Self.position.width - Nexus.Self.minSize.x) / 2, Nexus.Self.position.height / 3, Nexus.Self.minSize.x, foldoutHeight);

		}

		public override void OnGUI() {

			if(Nexus.Self == null || Nexus.Self.SelectedTab.Window.GetType() != constraint) {

				Close();
				return;

			}

			if(lastWindowWidth != Nexus.Self.position.width) {

				positionWindow = true;

			}

			if(positionWindow) {

				positionWindow = false;
				lastWindowWidth = Nexus.Self.position.width;
				PositionWindow();

			}

			GUI.DrawTexture(new Rect(0, 0, position.width, position.height), Swat.MakeTextureFromColor(Color.gray));

			scrollBar = new GUIStyle();
			scrollBar.margin = new RectOffset(25, 20, 0, 0);

			toggleGroup = new GUIStyle();
			toggleGroup.margin = new RectOffset(12, 0, 0, 0);

			divider = new GUIStyle(GUI.skin.box);
			divider.normal.background = Swat.MakeTextureFromColor(Swat.ActionButtonTextColor);
			divider.margin = new RectOffset(25, 0, 10, 20);

			requiredTestsStyle = new GUIStyle(GUI.skin.label);
			requiredTestsStyle.normal.textColor = Swat.ActionButtonTextColor;

			additionalTestsStyle = new GUIStyle(GUI.skin.label);
			additionalTestsStyle.normal.textColor = Swat.ActionButtonTextColor;

			testsToggle = new GUIStyle(GUI.skin.label);
			testsToggle.normal.textColor = Swat.ActionButtonTextColor;
			testsToggle.fixedWidth = 50;
			testsToggle.fixedHeight = 25;

			fo = new GUIStyle(EditorStyles.foldout);
			fo.margin = new RectOffset(-10, 0, 5, 5);
			fo.normal.textColor = fo.hover.textColor = fo.active.textColor = Swat.ActionButtonTextColor;

			header = new GUIStyle(GUI.skin.label);
			header.fontSize = 15;
			header.normal.textColor = Color.white;
			header.fontStyle = FontStyle.Bold;
			header.alignment = TextAnchor.MiddleCenter;

			buttons = new GUIStyle(GUI.skin.button);
			buttons.fixedHeight = buttonHeight;
			buttons.fixedWidth = buttonWidth;
			buttons.normal.textColor = Swat.ActionButtonTextColor;
			buttons.normal.background = Swat.MakeTextureFromColor((Color)new Color32(80, 80, 80, 255));

			EditorGUILayout.BeginVertical();
			GUILayout.Space(15);
			EditorGUILayout.LabelField("Please Confirm", header);
			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });

			scrollY = EditorGUILayout.BeginScrollView(scrollY, scrollBar);
			string testListFoldout = string.Empty;

			for(int x = 0; x < requestedTests.Count; x++) {

				if(!requestedTestNames.Contains(requestedTests[x].Value.Name)) {

					requestedTestNames.Add(requestedTests[x].Value.Name);

				}

			}
			if(!isSubsetRun) {

				string plurals = requestedTestNames.Count > 1 ? "these tests" : "this test";
				Nexus.Self.Foldout(true, string.Format(" You requested {0}", plurals), true, fo);
				for(int x = 0; x < requestedTestNames.Count; x++) {

					EditorGUILayout.LabelField(string.Format("    {0}", requestedTestNames[x]), requiredTestsStyle);
					GUILayout.Space(2);

				}
				plurals = additionalTests.Count > 1 ? "these tests" : "this test";
				testListFoldout = string.Format(" Which {0} dependent on {1}", additionalTests.Count > 1 ? "are" : "is", plurals);

			} else {

				testListFoldout = " Select subset of tests to run";
				additionalTests = requestedTests;

			}

			requiredTestsStyle.padding = new RectOffset(0, 0, 0, 0);
			Nexus.Self.Foldout(true, testListFoldout, true, fo);
			for(int x = 0; x < additionalTests.Count; x++) {

				EditorGUILayout.BeginHorizontal(toggleGroup);
				if(!additionalTestNames.Contains(additionalTests[x].Value.Name)) {

					additionalTestNames.Add(additionalTests[x].Value.Name);
					additionalTestsToggleList.Add(AutomationMaster.EntireUnitySessionCompletedTests.Contains(additionalTests[x].Value.Name) ? false : isTestSubset ? false : true);

				}

				testsToggle.padding = additionalTestsToggleList[x] ? new RectOffset(0, 0, -6, 0) : new RectOffset(2, 0, -2, 0);
				testsToggle.fontSize = additionalTestsToggleList[x] ? 25 : 18;
				if(GUILayout.Button(additionalTestsToggleList[x] ? Swat.TOGGLE_ON : Swat.TOGGLE_OFF, testsToggle, new GUILayoutOption[] { GUILayout.Width(40) })) {

					additionalTestsToggleList[x] = !additionalTestsToggleList[x];

				}

				GUILayout.Space(-30);
				if(GUILayout.Button(string.Format("  {0}", additionalTests[x].Value.Name), additionalTestsStyle)) {

					additionalTestsToggleList[x] = !additionalTestsToggleList[x]; //Toggle accompanying checkbox.

				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(-10);

			}

			EditorGUILayout.EndScrollView();

			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });

			AutomationMaster.DisregardDependencies = false; //Reset.

			float margin = (position.width - (buttonWidth * 3)) / 2;
			GUIStyle buttonGroup = new GUIStyle();
			buttonGroup.margin = new RectOffset((int)margin,(int)margin, 0, 10);

			EditorGUILayout.BeginHorizontal(buttonGroup);
			if(GUILayout.Button("Accept", buttons)) {

				Nexus.Overseer.ignoreDependentTestsForRun = false;
				List<string> listToRun = new List<string>();
				for(int x = 0; x < additionalTestNames.Count; x++) {

					if(additionalTestsToggleList[x]) {

						listToRun.Add(additionalTestNames[x]);

					}

				}
				string newTestList = "*";
				if(isSubsetRun) {

					newTestList += string.Join(",", listToRun.ToArray());

				} else {

					newTestList += string.Format("{0},{1}", string.Join(",", requestedTestNames.ToArray()), string.Join(",", listToRun.ToArray()));

				}
				Nexus.Overseer.Master_Editor_Override = new KeyValuePair<string, string>(newTestList, testType);

				bool ready = true;
				if(isSubsetRun) {

					//If this is a subset run, we will not have calculated dependencies beforehand. Do that now, and open a new alert. Doing so will discard this pop automatically, as all we need is the test data from it.
					List<KeyValuePair<string, MethodInfo>> selected = requestedTests.FindAll(x => listToRun.Contains(x.Value.Name));
					List<KeyValuePair<string, MethodInfo>> methodsToRunAfterMappingDependencies = Nexus.AutoMaster.GatherAllTestsThatNeedToBeRunToSatisfyAllDependenciesForPartialTestRun(selected);
					List<KeyValuePair<string, MethodInfo>> dependencies = methodsToRunAfterMappingDependencies.GetUniqueObjectsBetween(selected);

					if(dependencies.Count > 0) {
						
						RunTestsAlert.Pop(selected, dependencies, testName, testType, true);
						ready = false;

					} 

				} 

				if(ready) {

					Nexus.Self.Tests.LaunchTests(newTestList, testType);
					IsVisible = false;
					AutomationMaster.DisregardDependencies = true; //Ignore dependencies.
					Close();

				}

			}
			if(!isSubsetRun) {

				if(GUILayout.Button("Disregard", buttons)) {

					string newTestList = string.Empty; 

					if(requestedTestNames.Count > 1) {

						newTestList = string.Join(",", requestedTestNames.ToArray());
						Nexus.Overseer.Master_Editor_Override = new KeyValuePair<string, string>(newTestList, testType);
						AutomationMaster.DisregardDependencies = true; //Ignore dependencies.

					}
					AutomationMaster.DisregardDependencies = Nexus.Overseer.ignoreDependentTestsForRun = true;
					Nexus.Self.Tests.LaunchTests(string.IsNullOrEmpty(newTestList) ? testName : newTestList, string.IsNullOrEmpty(newTestList) ? testType : "test");
					IsVisible = false;
					Close();

				}

			}
			if(GUILayout.Button("Cancel", buttons)) {

				Nexus.Overseer.Master_Editor_Override = new KeyValuePair<string, string>();
				IsVisible = false;
				Close();

			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

		}

	}

}
