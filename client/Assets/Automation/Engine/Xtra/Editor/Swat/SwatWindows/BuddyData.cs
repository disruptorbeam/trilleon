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
using System.Collections.Generic;

namespace TrilleonAutomation {

	public class BuddyData : SwatWindow {

		string _newBuddyName = string.Empty;
		bool _chooseBuddyFromHistory, _ignoreBuddyTests, _isBuddySet, _isPrimaryBuddy;
		List<string> _buddyHistory = new List<string>();
		DateTime _lastBuddyRetrieval = new DateTime();
		int _buddySelection;
		int _redrawRateSeconds = 10;

		public override void Set() {}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			_newBuddyName = AutomationReport.GetMostRecentBuddy(); //Display last-used Buddy by default.

			//Tell test runner to not/ignore BuddySystem tests in the current test run.
			if(Application.isPlaying) {

				if(_ignoreBuddyTests != AutomationMaster.IgnoreAllBuddyTests) {

					if(!AutomationMaster.LockIgnoreBuddyTestsFlag) {

						AutomationMaster.IgnoreAllBuddyTests = _ignoreBuddyTests;

					}

				}

			}


			GUIStyle buddyWindowLabel = new GUIStyle(GUI.skin.label);
			buddyWindowLabel.fontStyle = FontStyle.Bold;
			buddyWindowLabel.padding = Nexus.BaseRectOffset;
			buddyWindowLabel.normal.textColor = Swat.WindowDefaultTextColor;

			GUIStyle sl = new GUIStyle(GUI.skin.label);
			sl.padding = new RectOffset(15, 0, 0, 0);

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("- Set Buddy Info -", buddyWindowLabel);
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			sl.normal.textColor = Swat.WindowDefaultTextColor;

			if(_isBuddySet && !string.IsNullOrEmpty(BuddyHandler.BuddyName)) {

				EditorGUILayout.LabelField(string.Format("Current Buddy is {0}", BuddyHandler.BuddyName), sl);
				EditorGUILayout.Space();

			}

			sl.fixedWidth = 125;

			if(!_buddyHistory.Any() && DateTime.Now.Subtract(_lastBuddyRetrieval).TotalSeconds > _redrawRateSeconds) {

				_buddyHistory = AutomationReport.GetBuddyHistory();
				_lastBuddyRetrieval = DateTime.Now;

			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Ignore Buddy Tests? ", sl);
			_ignoreBuddyTests = EditorGUILayout.Toggle(_ignoreBuddyTests);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if(_buddyHistory.Any()){

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Use Known Buddy? ", sl);
				_chooseBuddyFromHistory = EditorGUILayout.Toggle(_chooseBuddyFromHistory);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Is Primary? ", sl);
			_isPrimaryBuddy = EditorGUILayout.Toggle(_isPrimaryBuddy);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if(!_chooseBuddyFromHistory || !_buddyHistory.Any()) {

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Buddy Name: ", sl);
				_newBuddyName = EditorGUILayout.TextField(_newBuddyName);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

			}

			GUIStyle buddies = new GUIStyle(EditorStyles.popup);
			buddies.margin = new RectOffset(15, 0, 0, 0);

			if(_chooseBuddyFromHistory){

				if(_buddyHistory.Any()){

					buddies.fixedHeight = 15;
					_buddySelection = EditorGUILayout.Popup(_buddySelection, _buddyHistory.ToArray(), buddies, new GUILayoutOption[] { GUILayout.Height(50), GUILayout.MaxWidth(250)});

				}

			}

			buddies = new GUIStyle(GUI.skin.button);
			buddies.margin = new RectOffset(15, 0, 0, 0);
			buddies.fixedHeight = 25;

			if(GUILayout.Button("Set Buddy", buddies, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(25) })) {

				_isBuddySet = true;
				AutomationMaster.Arbiter.SendCommunication(string.Format("{{\"manual_set_buddy_{0}\":\"{1}\"}}", _isPrimaryBuddy ? "primary" : "secondary", _chooseBuddyFromHistory && _buddyHistory.Any() ? _buddyHistory[_buddySelection] : _newBuddyName));

			}

			EditorGUILayout.Space();

		}

	}

}
