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

	public class RunClassesAlert : SwatPopup {

		static Type constraint = typeof(TestManifest); //This popup can ONLY be opened by the TestManifest Swat Window, and can only stay open as long as that is the selected tab.

		public List<string> Cats = new List<string>();
		public List<bool> CatsSelected = new List<bool>();

		public bool positionWindow = true;
		float lastWindowWidth = 0;
		int foldoutHeaderHeight = 20;
		int buttonHeight = 25;
		int buttonWidth = 75;
		int headerHeight = 20;
		int breakSpace = 40;

		Vector2 scrollY;
		GUIStyle border, buttons, buttonGroup, divider, header, scrollBar, toggle, toggleGroup, toggleLabel;

		public static void Pop(List<string> cats) {

			EditorWindow.GetWindow<RunClassesAlert>().Close();
			ScriptableObject.CreateInstance<RunClassesAlert>().Set(cats);

		}
		public void Set(List<string> cats) {

			if(Nexus.Self.SelectedTab.Window.GetType() != constraint) {

				throw new UnityException("This popup is designed to only be called by the Test Manifest Swat Window.");

			}
				
			RunClassesAlert pop = ScriptableObject.CreateInstance<RunClassesAlert>();
			pop.Cats = cats;
			pop.IsVisible = true;
			pop.positionWindow = true;
			pop.Cats = pop.Cats.FindAll(x => x != "Debug" && !x.Contains("Debug")); //Remove Debug classes from list. They should only be run alone.
			pop.CatsSelected = new List<bool>(new bool[pop.Cats.Count]);
			pop.ShowPopup();

		}

		public override bool Visible() {

			return IsVisible;

		}
		public bool IsVisible { get; set; }

		public override void PositionWindow() {

			int foldoutHeight = (Cats.Count * 12) + (foldoutHeaderHeight * 2) + headerHeight + buttonHeight + (breakSpace * 2) + 250;
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

			divider = new GUIStyle(GUI.skin.box);
			divider.normal.background = Swat.MakeTextureFromColor(Color.white);
			divider.margin = new RectOffset(25, 0, 10, 20);

			toggleGroup = new GUIStyle();
			toggleGroup.margin = new RectOffset(12, 0, 0, 0);

			toggle = new GUIStyle(GUI.skin.label);
			toggle.padding = new RectOffset(35, 0, 0, 0);
			toggle.normal.textColor = Color.white;
			toggle.fixedWidth = 25;
			toggle.fixedHeight = 25;

			toggleLabel = new GUIStyle(GUI.skin.label);
			toggleLabel.normal.textColor = Color.white;

			header = new GUIStyle(GUI.skin.label);
			header.fontSize = 15;
			header.fixedHeight = 20;
			header.normal.textColor = Color.white;
			header.fontStyle = FontStyle.Bold;
			header.alignment = TextAnchor.MiddleCenter;

			buttons = new GUIStyle(GUI.skin.button);
			buttons.fixedHeight = buttonHeight;
			buttons.fixedWidth = buttonWidth;
			buttons.normal.textColor = Color.white;
			buttons.normal.background = Swat.MakeTextureFromColor((Color)new Color32(80, 80, 80, 255));

			buttonGroup = new GUIStyle();

			EditorGUILayout.BeginVertical();

			GUILayout.Space(15);
			EditorGUILayout.LabelField("Select Categories", header);
			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });

			scrollY = EditorGUILayout.BeginScrollView(scrollY, scrollBar);

			for(int x = 0; x < Cats.Count; x++) {

				EditorGUILayout.BeginHorizontal(toggleGroup);
				toggle.padding = CatsSelected[x] ? new RectOffset(0, 0, -6, 0) : new RectOffset(2, 0, -2, 0);
				toggle.fontSize = CatsSelected[x] ? 25 : 18;
				if(GUILayout.Button(CatsSelected[x] ? Swat.TOGGLE_ON : Swat.TOGGLE_OFF, toggle)) {
					
					CatsSelected[x] = !CatsSelected[x];

				}

				GUILayout.Space(-15);
				if(GUILayout.Button(string.Format("  {0}", Cats[x]), toggleLabel)) {
					
					CatsSelected[x] = !CatsSelected[x]; //Toggle accompanying checkbox.

				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(-10);

			}

			EditorGUILayout.EndScrollView();

			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });

			GUILayout.Space(15);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(50);
			toggle.padding = AutomationMaster.DisregardDependencies ? new RectOffset(0, 0, -6, 0) : new RectOffset(2, 0, -2, 0);
			toggle.fontSize = AutomationMaster.DisregardDependencies ? 25 : 18;
			if(GUILayout.Button(AutomationMaster.DisregardDependencies ? Swat.TOGGLE_ON : Swat.TOGGLE_OFF, toggle)) {
				
				AutomationMaster.DisregardDependencies = !AutomationMaster.DisregardDependencies;

			}
			GUILayout.Space(-10);
			if(GUILayout.Button("Also Run Dependencies? ", toggleLabel)) {
				
				AutomationMaster.DisregardDependencies = !AutomationMaster.DisregardDependencies; //Toggle accompanying checkbox.
			
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(15);

			float margin = (position.width - (buttonWidth * 2)) / 2;
			buttonGroup.margin = new RectOffset((int)margin,(int)margin, 0, 10);

			EditorGUILayout.BeginHorizontal(buttonGroup);

			if(GUILayout.Button("Cancel", buttons)) {
				
				Nexus.Overseer.Master_Editor_Override = new KeyValuePair<string, string>();
				IsVisible = false;
				Close();

			}
			if(GUILayout.Button("Launch", buttons)) {
				
				Nexus.Overseer.ignoreDependentTestsForRun = true;
				string command = string.Empty;
				string testsOnly = string.Empty;
				for(int x = 0; x < Cats.Count; x++) {

					if(CatsSelected[x]) {

						if(Cats[x].StartsWith("*")) {

							string cat = Cats[x].Replace("*", string.Empty);
							List<KeyValuePair<string,List<KeyValuePair<bool,string>>>> match = Nexus.Self.Favorites.FavoritesList.FindAll(f => f.Key == cat);
							if(match.Count == 0) {

								SimpleAlert.Pop(string.Format("Cannot find data for Favorite, \"{0}\", selected in multi-category launch.", cat), null);
								return;

							} else {

								List<KeyValuePair<bool,string>> contents = match.First().Value;
								for(int c = 0; c < contents.Count; c++) {

									if(contents[c].Key) {

										//If the next item in this list is not a category, then the current category is merely a header for the test list that follows, and should not be included.
										if(c + 1 < contents.Count && !contents[c + 1].Key) {

											continue;

										}
										
										command += string.Format("{0},", contents[c].Value);

									} else {

										testsOnly += string.Format("{0},", contents[c].Value);

									}

								}

							}

						} else {

							string category = string.Empty;
							if(Cats[x].Contains("(")) {

								category = Cats[x].Replace("<", string.Empty).Replace(">", string.Empty).Split('(')[1].Trim(')');

							} else {

								category = Cats[x].Replace("<", string.Empty).Replace(">", string.Empty);

							}
							command += string.Format("{0},", category);

						}

					}

				}
				command = command.Trim(',').Replace("*", "@"); //@ represents favorite when * represents single test.
				if(testsOnly.Length > 0) {

					command = string.Format("&&{0}%{1}", command, testsOnly);

				}

				Close();
				Nexus.Self.Tests.LaunchTests(command, "class");
				IsVisible = false;
				AutomationMaster.DisregardDependencies = true; //Ignore dependencies.

			}

			EditorGUILayout.EndHorizontal();

		}

	}

}
