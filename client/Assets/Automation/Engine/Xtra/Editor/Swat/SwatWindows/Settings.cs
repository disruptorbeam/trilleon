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

﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;

namespace TrilleonAutomation {

	public class Settings : SwatWindow {

		bool changesMade, customKeys, saved, showError, showWarningForEditsToCriticalKeys;

		string newEntryKey = string.Empty;
		string newEntryValue = string.Empty;
		List<KeyValuePair<string, string>> settings = new List<KeyValuePair<string, string>>();
		List<KeyValuePair<string, string>> settingsSaved = new List<KeyValuePair<string, string>>();

		DateTime saveTime = new DateTime();
		GUIStyle button, buttonChoices, error, horizontal, inputField, inputToggle, label, savedStyle, sectionHeader, warning;
		Color green;

		const string xIcon = "×";

		public string testField = string.Empty;
		public List<string[]> editableFields = new List<string[]>();
		public List<string[]> hiddenFields = new List<string[]>();
		public List<bool> criticalFields = new List<bool>();

		public override void Set() { }

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { 

			GetConfig();

		}

		public override void Render() {

			green = (Color)new Color32(210, 255, 215, 255);

			horizontal = new GUIStyle();
			horizontal.fixedHeight = 20;
			horizontal.fixedWidth = 375;

			button = new GUIStyle(GUI.skin.button);
			button.fontSize = 16;
			button.fontStyle = FontStyle.Bold;

			buttonChoices= new GUIStyle(GUI.skin.button);
			buttonChoices.fixedHeight = 22;
			buttonChoices.fixedWidth = 100;

			error = new GUIStyle(GUI.skin.label);
			error.normal.textColor = Color.red;

			inputField = new GUIStyle(GUI.skin.textField);
			inputField.fixedWidth = 275;

			label = new GUIStyle(GUI.skin.label);
			label.fontStyle = FontStyle.Bold;
			label.normal.textColor = Swat.WindowDefaultTextColor;

			savedStyle = new GUIStyle(GUI.skin.label);
			savedStyle.normal.textColor = EditorGUIUtility.isProSkin ? green : (Color)new Color32(0, 140, 20, 255);

			sectionHeader = new GUIStyle(GUI.skin.label);
			sectionHeader.fontSize = 22;
			sectionHeader.normal.textColor = Swat.WindowDefaultTextColor;
			sectionHeader.fixedHeight = 50;

			inputToggle = new GUIStyle();
			Texture2D defaultBackground = inputToggle.normal.background;

			warning = new GUIStyle(GUI.skin.label);
			warning.wordWrap = true;
			warning.margin = new RectOffset(20, 20, 0, 0);
			warning.normal.textColor = new Color32(255, 125, 0, 255);


			if(showWarningForEditsToCriticalKeys) {

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Warning: Editing critical settings values may result in undesired behavior!", warning);

			}

			EditorGUI.indentLevel++;

			if(saved) {

				if(DateTime.UtcNow.Subtract(saveTime).TotalSeconds > 10) {

					//Only show message for 10 seconds.
					saved = false;
					Nexus.Self.Repaint();

				} else {

					EditorGUILayout.LabelField("Changes saved!", savedStyle);

				}

			} else {

				EditorGUILayout.Space();

			}

			EditorGUILayout.Space();

			int critsAny = 0;

			if(settings == null || settings.Count == 0) {

				GetConfig();

			}

			for(int i = 0; i < settings.Count; i++) {

				if(string.IsNullOrEmpty(settings[i].Key)) {

					List<string[]> match = editableFields.FindAll(x => x[1] == settings[i].Value);
					if(!match.Any()) {

						editableFields.Add(new string[] { string.Empty, settings[i].Value });
						hiddenFields.Add(new string[] { string.Empty, settingsSaved[i].Value });

					}

					if(i != 0) {

						GUILayout.Space(15);

					}

					customKeys = !settings[i].Value.Contains("REQUIRED");

					EditorGUILayout.LabelField(customKeys ? "Custom Settings" : "Trilleon Settings", sectionHeader);
					GUILayout.Space(20);

				} else {

					button.normal.textColor = Color.red;

					if(customKeys) {

						inputField.fixedWidth = 225;
						EditorGUILayout.BeginHorizontal(horizontal);
						GUILayout.Space(20);
						EditorGUILayout.BeginVertical();
						GUILayout.Space(12);
						if(GUILayout.Button(xIcon, button, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) })) {

							changesMade = true;
							settings.RemoveAt(i);
							editableFields.RemoveAt(i);
							criticalFields.RemoveAt(i);
							continue;

						}
						EditorGUILayout.EndVertical();
						GUILayout.Space(-25);

					} else {

						inputField.fixedWidth = 275;

					}

					//Use string array for mutability of these key value pairs.
					string keyCasing = settings[i].Key.ToLower().Replace('_', ' ');
					keyCasing = new CultureInfo("en-US", false).TextInfo.ToTitleCase(keyCasing);

					List<string[]> match = editableFields.FindAll(x => x[0] == keyCasing);
					List<string[]> lastMatchVal = hiddenFields.FindAll(x => x[0] == keyCasing);

					if(!match.Any()) {

						string[] keyValPair = new string[2];
						keyValPair[0] = keyCasing;
						keyValPair[1] = settings[i].Value;
						editableFields.Add(keyValPair);
						match.Add(editableFields[editableFields.Count - 1]);

						//Is this a new pending setting that has not yet been saved? If so, skip this.
						if(settingsSaved.Count > i) {

							hiddenFields.Add(new string[] { keyCasing, settingsSaved[i].Value });
							lastMatchVal.Add(hiddenFields[hiddenFields.Count - 1]);

						}

					}

					string[] thisEntry = match.First();
					string[] lastMatchValEntry = lastMatchVal.Any() ? lastMatchVal.First() : new string[] { string.Empty, string.Empty };
					bool isBool = false;
					bool boolValue = false;
					switch(thisEntry[1].ToLower()) {
					case "false":
						isBool = true;
						break;
					case "true":
						boolValue = true;
						isBool = true;
						break;
					} 

					//Add label and instantiate field as equal to the entry in our editableFields list.
					if(customKeys) {

						EditorGUILayout.BeginVertical();

					}
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(18);
					if(GUILayout.Button(string.Format("{0}: ", keyCasing), label)) {

						if(isBool) {

							thisEntry[1] = (!bool.Parse(thisEntry[1])).ToString();
							int index = -1;
							editableFields.Find(x => {
								index++;
								return x[0] == thisEntry[0];
							});
							editableFields.RemoveAt(index);
							editableFields = editableFields.AddAt(index, thisEntry);
							boolValue = !boolValue;

						}

					}
					EditorGUILayout.EndHorizontal();

					if(lastMatchValEntry[1] != thisEntry[1] || !settingsSaved.FindAll(ss => ss.Key == settings[i].Key).Any()) {

						if(criticalFields[i]) {

							inputToggle.normal.background = Swat.MakeTextureFromColor((Color)new Color32(200, 0, 0, 50));
							critsAny++;

						} else {

							inputToggle.normal.background = Swat.MakeTextureFromColor((Color)new Color32(0, 200, 0, 50));
							changesMade = true;

						}

					} else {

						inputToggle.normal.background = defaultBackground;

					}

					if(isBool) {

						EditorGUILayout.BeginVertical(inputToggle);
						thisEntry[1] = EditorGUILayout.Toggle(boolValue, GUILayout.MinWidth(127), GUILayout.MaxWidth(252)).ToString();
						EditorGUILayout.EndVertical();

					} else {

						EditorGUILayout.BeginVertical(inputToggle);
						thisEntry[1] = EditorGUILayout.TextField(thisEntry[1], inputField, GUILayout.MinWidth(125), GUILayout.MaxWidth(250));
						EditorGUILayout.EndVertical();

					}

					if(customKeys) {

						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(20);

					}

					GUILayout.Space(20);

				}

			}

			if(critsAny > 0) {

				showWarningForEditsToCriticalKeys = true;

			} else {

				showWarningForEditsToCriticalKeys = false;

			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(horizontal);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(30);
			button.normal.textColor = Swat.TextGreen;
			inputField.fixedWidth = 225;

			if(GUILayout.Button("+", button, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) })) {

				if(!string.IsNullOrEmpty(newEntryKey)) {

					if(!newEntryKey.Contains(" ")) {

						settings.Add(new KeyValuePair<string, string>(newEntryKey, newEntryValue));
						criticalFields.Add(false);
						newEntryKey = string.Empty;
						newEntryValue = string.Empty;
						showError = false;
						Nexus.Self.Repaint();

					} else {

						showError = true;

					}

				} else {

					showError = true;

				}

			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(-30);
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Key: ", label);
			newEntryKey = EditorGUILayout.TextField(newEntryKey, inputField, new GUILayoutOption[] { GUILayout.MaxWidth(250)});
			EditorGUILayout.LabelField("Val: ", label);
			newEntryValue = EditorGUILayout.TextField(newEntryValue, inputField, new GUILayoutOption[] { GUILayout.MaxWidth(250)});
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(25);
			if(showError) {

				EditorGUILayout.LabelField("A key cannot contain spaces. Use underscores in place of spaces.", error);

			}
			GUILayout.Space(40);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(65);
			if(changesMade) {

				buttonChoices.normal.background = Swat.MakeTextureFromColor((Color)new Color32(0, 255, 0, 50));

			}
			if(GUILayout.Button("Save Changes", buttonChoices, GUILayout.Width(100))) {

				SaveSettings();

			}
			GUILayout.Space(20);
			if(GUILayout.Button("Revert Changes", buttonChoices, GUILayout.Width(100))) {

				changesMade = false;
				editableFields = new List<string[]>();
				hiddenFields = new List<string[]>();
				criticalFields = new List<bool>();
				saved = false;
				Set();
				GetConfig();

			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(30);

			EditorGUI.indentLevel--;

		}

		void GetConfig() {

			settings = new List<KeyValuePair<string, string>>();
			settingsSaved = new List<KeyValuePair<string, string>>();
			string[] instructions = FileBroker.GetTextResource(FileResource.TrilleonConfig).Split('\n');
			bool customKey = false;

			for(int i = 0; i < instructions.Length; i++) {

				if(string.IsNullOrEmpty(instructions[i])) {

					continue;

				}

				if(!instructions[i].StartsWith("**")) {

					string[] keyValPair = instructions[i].Split('=');
					if(keyValPair[0].StartsWith("!")) {

						criticalFields.Add(true);
						keyValPair[0] = keyValPair[0].Substring(1, keyValPair[0].Length - 1);

					} else {

						criticalFields.Add(false);

					}
					KeyValuePair<string, string> pair = new KeyValuePair<string, string>(keyValPair[0], keyValPair[1]);
					settings.Add(pair);
					settingsSaved.Add(pair);

				} else {

					KeyValuePair<string, string> pair = new KeyValuePair<string, string>(string.Empty, !customKey ? "**** TRILLEON CONFIGS REQUIRED ****" : "**** CUSTOM CONFIGS ****");
					settings.Add(pair);
					settingsSaved.Add(pair);
					criticalFields.Add(false);
					customKey = !customKey;

				}

			}

		}

		void SaveSettings() {

			changesMade = false;

			//Build file text.
			StringBuilder textInfo = new StringBuilder();
			for(int i = 0; i < editableFields.Count; i++) {

				if(string.IsNullOrEmpty(editableFields[i][0])) {

					textInfo.AppendLine(editableFields[i][1]);

				} else {

					string key = editableFields[i][0].ToUpper().Replace(' ', '_');
					HandleSpecificKeyChangeRequirements(key, editableFields[i][1]);
					textInfo.AppendLine(string.Format("{0}{1}={2}", criticalFields[i] ? "!" : string.Empty, key, editableFields[i][1]));

				}

			}

			//Overwrite and save settings to file.
			FileBroker.SaveTextResource(FileResource.TrilleonConfig, textInfo.ToString());

			editableFields = new List<string[]>();
			hiddenFields = new List<string[]>();
			criticalFields = new List<bool>();
			saveTime = DateTime.UtcNow;
			saved = true;
			Set();
			GetConfig();

		}

		void HandleSpecificKeyChangeRequirements(string key, string value) {

			switch(key) {
				case "NEVER_AUTO_LOCK_RELOAD_ASSEMBLIES":
					if(value == "True") {

						EditorApplication.UnlockReloadAssemblies();

					} else {
					
						EditorApplication.LockReloadAssemblies();

					}
					break;
				default:
					break;
			}

		}

	}

}
