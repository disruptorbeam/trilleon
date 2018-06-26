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

namespace TrilleonAutomation {

	public class ConsoleMessage : SwatPopup {

		const int LAST_LAUNCHED_COOLDOWN = 2;

		static Type constraint = typeof(RunnerConsole); //This popup can ONLY be opened by the RunnerConsole Swat Window, and can only stay open as long as that is the selected tab.

		static Vector2 _scrollConsoleDetails = new Vector2();
		static int buttonHeight = 25;
		static int buttonWidth = 75;
		public bool DoPositionWindow = true;
		static float lastWindowWidth = 0;
		static DateTime lastOpened;

		public string Message = string.Empty;
		public string MessageType = string.Empty;
		public string MessageLevel = string.Empty;
		public string Timestamp = string.Empty; 

		GUIStyle buttons, buttonGroup, closeBox, details, detailsBox, detailsLabel, divider, header;

		public override bool Visible() {

			return IsVisible;

		}
		public bool IsVisible { get; set; }

		public override void PositionWindow() {

			position = new Rect(Nexus.Self.position.x + (Nexus.Self.position.width - Nexus.Self.minSize.x) / 2, 120, Nexus.Self.minSize.x, TestMonitorHelpers.DetermineRectHeightBasedOnLinesInNodeDetails(Message) + 140);
			minSize = maxSize = new Vector2(Nexus.Self.position.width, System.Convert.ToInt32(System.Math.Round(Nexus.Self.position.height / 1.25)));

		}

		public static void Pop(string message, string messageType, string messageLevel,string timeStamp) {

			EditorWindow.GetWindow<ConsoleMessage>().Close();
			ScriptableObject.CreateInstance<ConsoleMessage>().Set(message, messageType, messageLevel, timeStamp);

		}
		public void Set(string message, string messageType, string messageLevel,string timeStamp) {

			if(Nexus.Self.SelectedTab.Window.GetType() != constraint) {

				throw new UnityException("This popup is designed to only be called by the RunnerConsole Swat Window.");

			}

			ConsoleMessage pop = ScriptableObject.CreateInstance<ConsoleMessage>();

			//Allow updating of existing window, but not re-showing of popup.
			pop.Message = message;
			pop.MessageType = messageType;
			pop.MessageLevel = messageLevel;
			pop.Timestamp = timeStamp;
			pop.DoPositionWindow = true;

			//Due to how this popup is opened, it is easy to accidentally launch several times. If in cooldown, do nothing. If this is called when opening a test report, do nothing.
			if(Math.Abs(lastOpened.Subtract(DateTime.UtcNow).TotalSeconds) <= LAST_LAUNCHED_COOLDOWN || Math.Abs(Nexus.Self.Console.OpenedTestReport.Subtract(DateTime.UtcNow).TotalSeconds) <= LAST_LAUNCHED_COOLDOWN) {

				return;

			}

			pop.IsVisible = true;
			pop.ShowPopup();
			lastOpened = DateTime.UtcNow;

		}

		public override void OnGUI() {

			if(Nexus.Self == null || Nexus.Self.SelectedTab.Window.GetType() != constraint) {

				Close();
				return;

			}

			if(lastWindowWidth != Nexus.Self.position.width) {

				DoPositionWindow = true;

			}

			if(DoPositionWindow) {

				DoPositionWindow = false;
				lastWindowWidth = Nexus.Self.position.width;
				PositionWindow();

			}

			GUI.skin.textArea.wordWrap = true;

			buttons = new GUIStyle(GUI.skin.button);
			buttons.fixedHeight = buttonHeight;
			buttons.fixedWidth = buttonWidth;
			buttons.margin = new RectOffset(0, 0, 10, 0);
			buttons.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			buttonGroup = new GUIStyle();

			closeBox = new GUIStyle(GUI.skin.button);
			closeBox.fontSize = 16;
			closeBox.padding = new RectOffset(2, 0, 0, 0);
			closeBox.normal.textColor = Color.red; 

			detailsBox = new GUIStyle(GUI.skin.box);
			detailsBox.margin = new RectOffset(10, 25, 0, 0);
			detailsBox.fixedWidth = minSize.x - 20;
			detailsBox.fixedHeight = minSize.y - 100;

			detailsLabel = new GUIStyle(GUI.skin.label);
			detailsLabel.fontStyle = FontStyle.Bold;
			detailsLabel.fontSize = 12;

			details = new GUIStyle(GUI.skin.textArea);
			details.fixedHeight = detailsBox.fixedHeight - 150;
			details.fixedWidth = detailsBox.fixedWidth - 15;
			details.padding = detailsBox.padding = new RectOffset(5, 5, 5, 5);

			divider = new GUIStyle(GUI.skin.box);
			divider.normal.background = Swat.MakeTextureFromColor(Color.white);
			divider.margin = new RectOffset(25, 0, 10, 20);

			header = new GUIStyle(GUI.skin.label);
			header.fontSize = 14;
			header.fixedHeight = 25;
			header.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			header.fontStyle = FontStyle.Bold;
			header.alignment = TextAnchor.MiddleCenter;
			header.padding = new RectOffset(0, 0, -20, 0);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Nexus.Self.Button("X", "Show/Hide test status details section.", 
				new Nexus.SwatDelegate(delegate() {                
					IsVisible = false;
					Close();
				}), closeBox, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) });
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Console Message", header);
			GUILayout.Space(10);

			GUILayout.BeginVertical(detailsBox);
			EditorGUILayout.LabelField("Time:", detailsLabel, new GUILayoutOption[] { GUILayout.Width(120)});
			EditorGUILayout.SelectableLabel(Timestamp);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Type:", detailsLabel, new GUILayoutOption[] { GUILayout.Width(120)});
			EditorGUILayout.SelectableLabel(MessageType);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Verbosity:", detailsLabel, new GUILayoutOption[] { GUILayout.Width(120)});
			EditorGUILayout.SelectableLabel(MessageLevel);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Message:", detailsLabel, new GUILayoutOption[] { GUILayout.Width(120)});
			GUILayout.BeginHorizontal();
			_scrollConsoleDetails = GUILayout.BeginScrollView(_scrollConsoleDetails, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
			EditorGUILayout.SelectableLabel(Message, details);
			GUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			buttonGroup.margin = new RectOffset(System.Convert.ToInt32(System.Math.Round(position.width / 2)) - 25, 0, 0, 10);
			EditorGUILayout.BeginHorizontal(buttonGroup);
			if(GUILayout.Button("Close", buttons)) {
				IsVisible = false;
				Close();
			}
			EditorGUILayout.EndHorizontal();

		}

	}

}
