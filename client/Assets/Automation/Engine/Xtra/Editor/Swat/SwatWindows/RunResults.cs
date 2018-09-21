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
using System.IO;

namespace TrilleonAutomation {

	public class RunResults : SwatWindow {

		DateTime _fileDeletedTime = new DateTime();
		public bool ReDrawAutomationReports = true;
        bool _showFileDeletedMessage;
		List<FileInfo> automationReports = new List<FileInfo>();
		List<KeyValuePair<string,List<KeyValuePair<string,string>>>> metaData = new List<KeyValuePair<string,List<KeyValuePair<string,string>>>>();

		public override void Set() { }

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() {

			ReDrawAutomationReports = true;

		}

		public override void Render() {

			if(_showFileDeletedMessage) {

				if(DateTime.Now.Subtract(_fileDeletedTime).TotalSeconds > 2) {

					_showFileDeletedMessage = false;

				}

			}

			GUIStyle back = new GUIStyle(GUI.skin.button);
			back.fontSize = 20;
			back.margin = new RectOffset(25, 0, 10, 20);

			if(ReDrawAutomationReports) {

				ReDrawAutomationReports = false;

				if(!Directory.Exists(Path.GetDirectoryName(FileBroker.REPORTS_DIRECTORY))) {

					Directory.CreateDirectory(Path.GetDirectoryName(FileBroker.REPORTS_DIRECTORY));

				}

				//Grab any report files.
				List<string> all = Directory.GetFiles(FileBroker.REPORTS_DIRECTORY, "report.html", SearchOption.AllDirectories).ToList();
				automationReports = new List<FileInfo>();

				for (int x = 0; x < all.Count; x++) {

					automationReports.Add(new FileInfo(all[x]));

				}

				//Grab meta files for reports.
				all = Directory.GetFiles(FileBroker.REPORTS_DIRECTORY, "report.meta", SearchOption.AllDirectories).ToList();
				metaData = new List<KeyValuePair<string,List<KeyValuePair<string,string>>>>();
				for(int m = 0; m < all.Count; m++) {

					FileInfo info = new FileInfo(all[m]);
					string[] metaLines = FileBroker.ReadUnboundFile(info.FullName);
					List<KeyValuePair<string,string>> values = new List<KeyValuePair<string,string>>();

					for(int ml = 0; ml < metaLines.Length; ml++) {

						string[] keyVals = metaLines[ml].Split(':');
						values.Add(new KeyValuePair<string,string>(keyVals[0], keyVals[1]));

					}

					metaData.Add(new KeyValuePair<string,List<KeyValuePair<string,string>>>(all[m].Split(new string[] { string.Format("{0}{1}", GameMaster.GAME_NAME, FileBroker.FILE_PATH_SPLIT) }, StringSplitOptions.None)[1].Replace(string.Format("{0}report.meta", FileBroker.FILE_PATH_SPLIT), ".html"), values));
				}

				automationReports.Reverse();

			}

			if(!automationReports.Any()) {

				GUIStyle noResults = new GUIStyle(GUI.skin.label);
				noResults.padding = new RectOffset(10, 0, 0, 0);
				noResults.fontSize = 14;
				noResults.normal.textColor = Color.blue;
				GUILayout.Space(20);
				EditorGUILayout.LabelField("No test reports currently exist.", noResults);
				return;

			}

			Color32 boxGreen = new Color32(20, 175, 0, 255);
			GUIStyle fileLabel = new GUIStyle(GUI.skin.label);
			fileLabel.normal.textColor = boxGreen;
			fileLabel.padding = new RectOffset(32, 0, 0, 1);

			GUIStyle fileNameButton = new GUIStyle(GUI.skin.button);
			fileNameButton.margin = new RectOffset(35, 0, 0, 0);
			fileNameButton.normal.background = fileNameButton.active.background = fileNameButton.focused.background = Swat.TabButtonBackgroundTexture;
			fileNameButton.normal.textColor = Swat.WindowDefaultTextColor;

			GUIStyle deleteFileButton = new GUIStyle(GUI.skin.button);
			deleteFileButton.normal.textColor = Color.red;
			deleteFileButton.fontSize = 14;
			deleteFileButton.margin = new RectOffset(1, 0, 0, 5);
			deleteFileButton.normal.background = deleteFileButton.active.background = deleteFileButton.focused.background = Swat.TabButtonBackgroundTexture;
			deleteFileButton.normal.textColor = Color.red;

			GUIStyle divider = new GUIStyle(GUI.skin.box);
			divider.normal.background = Swat.MakeTextureFromColor(boxGreen);
			divider.margin = new RectOffset(35, 10, 10, 10);

			GUIStyle deleteAllButton = new GUIStyle(GUI.skin.button);
			deleteAllButton.fontSize = 12;
			deleteAllButton.normal.textColor = Color.red;
			deleteAllButton.fontStyle = FontStyle.Bold;
			deleteAllButton.fixedHeight = 38;
			deleteAllButton.margin = new RectOffset(0, 25, 5, 0);
			deleteAllButton.normal.background = deleteAllButton.active.background = deleteAllButton.focused.background = Swat.TabButtonBackgroundTexture;
			deleteAllButton.normal.textColor = Color.red;

			GUILayout.Space(20);

			GUIStyle padding = new GUIStyle();
			padding.margin = new RectOffset(25, 0, 0, 0);
			GUILayout.BeginHorizontal(padding);
			EditorGUILayout.Space();
			if(_showFileDeletedMessage) {

				GUIStyle deleteFileAlert = new GUIStyle(GUI.skin.label);
				deleteFileAlert.normal.textColor = Color.red;
				deleteFileAlert.fontSize = 14;
				deleteFileAlert.fixedHeight = 28;
				deleteFileAlert.padding = new RectOffset(30, 0, 14, 0);
				EditorGUILayout.LabelField("Deleted!", deleteFileAlert);

			}
			EditorGUILayout.Space();
			if(GUILayout.Button("Delete All", deleteAllButton, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(75) })) {

				SimpleAlert.Pop("Are you sure you want to delete all stored test run reports?", new EditorDelegate(DeleteAll));

			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20);

			for(int ar = 0; ar < automationReports.Count; ar++) {

				bool isNewest = ar == 0;
				string[] splitTime = automationReports[ar].CreationTime.ToShortTimeString().Split(':');
				int hour = int.Parse(splitTime[0]);
				bool am = true;
				if(hour >= 12) {
					am = false;
					if(hour != 12) {
						hour -= 12;
					}
				}

				if(!isNewest) {
					fileNameButton.normal.textColor = fileLabel.normal.textColor = Swat.WindowDefaultTextColor;
				} 

				string timeOfDay = string.Format("{0}:{1} {2}", hour.ToString(), splitTime[1], am ? "AM" : "PM");
				if(isNewest) {
					EditorGUILayout.LabelField("-----Newest-----", fileLabel);
				}
				EditorGUILayout.LabelField(string.Format("{0}    {1}", automationReports[ar].CreationTime.ToLongDateString(), timeOfDay), fileLabel);

				List<KeyValuePair<string,List<KeyValuePair<string,string>>>> matchMetaResults = metaData.FindAll(x => automationReports[ar].Directory.ToString().Contains(x.Key.Replace(".html", string.Empty)));

				if(matchMetaResults.Any()) {

					KeyValuePair<string,List<KeyValuePair<string,string>>> matchMeta = matchMetaResults.First();
					EditorGUILayout.LabelField(string.Format("{0}:{1}     {2}:{3}     {4}:{5}     {6}:{7}",
						TestManifest.PASSED, matchMeta.Value.Find(x => x.Key == "Passes").Value,
						TestManifest.FAILED, matchMeta.Value.Find(x => x.Key == "Fails").Value,
						TestManifest.SKIPPED, matchMeta.Value.Find(x => x.Key == "Skips").Value,
						TestManifest.IGNORED, matchMeta.Value.Find(x => x.Key == "Ignores").Value), fileLabel);

					if(Nexus.Self.MouseOver()) {

						Nexus.Self.SetToolTip(matchMeta.Value.Find(x => x.Key == "RunCommand").Value.Replace(",", "\n"));

					}

				}

				if(isNewest) {

					GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(180) });

				}

				GUILayout.BeginHorizontal();

				if(GUILayout.Button("Open Report", fileNameButton, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(150) })) {
						
				    System.Diagnostics.Process.Start(automationReports[ar].FullName);

				}
				if(GUILayout.Button("X", deleteFileButton, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(30) })) {

					//Delete report and associated meta file.
					Directory.Delete(automationReports[ar].Directory.ToString().Replace("/report.html", string.Empty), true);
					ReDrawAutomationReports = true;
					_fileDeletedTime = DateTime.Now;
					_showFileDeletedMessage = true;

				}
				GUILayout.EndHorizontal();

				if(isNewest) {

					GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(180) });

				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

			}


		}

		void DeleteAll() {

			//Delete reports and associated meta files.
			for(int x = 0; x < automationReports.Count; x++) {

				Directory.Delete(FileBroker.REPORTS_DIRECTORY, true);
				Directory.CreateDirectory(FileBroker.REPORTS_DIRECTORY);
				ReDrawAutomationReports = true;
				_fileDeletedTime = DateTime.Now;
				_showFileDeletedMessage = true;

			}

		}

	}

}
