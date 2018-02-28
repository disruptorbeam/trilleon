/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TrilleonAutomation {

	public class AdditionalTools : SwatWindow {

		Dock dock;
		GUIStyle description, editorName, open;
		byte currentAlphaStep = 60;
		int recentlyReorderedIndex = -1;

		public List<KeyValuePair<string,int>> TabPreferences { 
			get { 
				if(tabPreferences.Count == 0) {

					return GetTabPreferences();

				}
				return tabPreferences;
			} 
		}
		List<KeyValuePair<string,int>> tabPreferences = new List<KeyValuePair<string,int>>();

		public override void Set() { }

		public override void OnTabSelected() { }

		public List<KeyValuePair<string,int>> GetTabPreferences() {

			dock = Swat.DockNextTo;
			List<string> rawPreferences = FileBroker.GetNonUnityTextResource(FileResource.NexusTabs).Split(AutomationMaster.DELIMITER).ToList().RemoveNullAndEmpty();
			tabPreferences = new List<KeyValuePair<string,int>>();
			int highestRank = 0;

			for(int r = 0; r < rawPreferences.Count; r++) {

				string[] pref = rawPreferences[r].Split('=');
				int rank = int.Parse(pref.Last());
				tabPreferences.Add(new KeyValuePair<string,int>(pref.First(), rank));
				highestRank = rank > highestRank ? rank : highestRank;

			}

			for(int w = 0; w < Nexus.Self.SwatWindows.Count; w++) {

				string windowName = Nexus.Self.SwatWindows[w].Window.GetType().Name;
				//If no data exists for this window, add it to the end of existing ranked windows.
				if(!tabPreferences.FindAll(x => x.Key == windowName).Any()) {

					tabPreferences.Add(new KeyValuePair<string,int>(windowName, ++highestRank));

				}

			}

			return tabPreferences;

		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void Render() {

			if(TabPreferences.Count == 0) {

				GetTabPreferences();

			}

			description = new GUIStyle(GUI.skin.label);
			description.fontSize = 12;
			description.wordWrap = true;
			description.margin = new RectOffset(10, 10, 0, 0);
			description.normal.textColor = Swat.WindowDefaultTextColor;

			editorName = new GUIStyle(GUI.skin.label);
			editorName.fontSize = 16;
			editorName.fixedHeight = 20;
			editorName.fontStyle = FontStyle.Bold;
			editorName.padding = new RectOffset(8, 0, 0, 0);
			editorName.normal.textColor = Swat.WindowDefaultTextColor;

			open = new GUIStyle(GUI.skin.button);
			open.fontSize = 14;
			open.fixedHeight = 32;
			open.fixedWidth = 100;
			open.margin = new RectOffset(10, 10, 0, 0);
			open.normal.textColor = Swat.WindowDefaultTextColor;
			open.normal.background = open.active.background = open.focused.background = Swat.ToggleButtonBackgroundSelectedTexture;

			GUILayout.Space(25);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Dock Next To:", description, new GUILayoutOption[] { GUILayout.Width(100) });
			dock = (Dock)Nexus.Self.DropDown(dock, 5, 25, 140);
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(18);

			EditorGUILayout.LabelField("Assistant", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField("This window watches inspected elements in the hierarchy window and displays automation-relevant details on this object. With the presented information," +
				"you may choose actions to perform on the object and generate code stubs for interacting with that object.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				Nexus.Self.SelectTab(Nexus.Self.Generator);
				Nexus.Self.Generator.SelectedSubTab = GeneratorSubTab.Assistant;

			}
			GUILayout.Space(18);

			EditorGUILayout.LabelField("Buddy Details", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField("This window allows customization of Buddy System data for editor-based test runs.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				Nexus.Self.SelectTab(Nexus.Self.BuddyData);

			}
			GUILayout.Space(18);

			EditorGUILayout.LabelField("Dependency Architecture", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField(@"This window displays the Dependency Class and Dependency Test attribute usage within the all existing tests.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				Nexus.Self.SelectTab(Nexus.Self.Architect);

			}
			GUILayout.Space(18);

			EditorGUILayout.LabelField("Dependency Web", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField(@"This window displays Dependency Web usage throughout existing tests, helping to visualize the relationships between tests in a graphical web format." +
				"Because of the way this window is rendered, docking options are limited to Floating and DockNextToGameWindow only when opening.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				//Web must be viewed in a large screen move. Dock next to Game, or allow float.
				Swat.ShowWindow<DependencyVisualizer>(typeof(DependencyVisualizer), "Web", dock == Dock.Float ? dock : Dock.NextToGame);

			}
			GUILayout.Space(18);

			EditorGUILayout.LabelField("Recorder", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField(@"This window allows you to begin recording automation-relevant actions that you take in the Unity Editor. When you stop recording, all actions are converted into code that can" +
				"be pasted directly into a test method, and played back as a new test.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				Nexus.Self.SelectTab(Nexus.Self.Generator);
				Nexus.Self.Generator.SelectedSubTab = GeneratorSubTab.Recorder;

			}
			GUILayout.Space(18);

			EditorGUILayout.LabelField("Settings", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField(@"All Trilleon settings keys are displayed here so that their values can easily be updated, and new custom keys can be added on the fly.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				Nexus.Self.SelectTab(Nexus.Self.Settings);

			}
			GUILayout.Space(20);
	
		}

	}

}
