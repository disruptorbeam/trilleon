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

﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace TrilleonAutomation {

	public static class FileBroker {

		const string TOP_LEVEL_FOLDER_NAME = "Trilleon";
		public static string BASE_UNITY_PATH = string.Empty;
		public static string BASE_NON_UNITY_PATH = string.Empty;
		public static string FILE_PATH_SPLIT = string.Empty;

		public static string RESOURCES_DIRECTORY = string.Empty;
		public static string SCREENSHOTS_DIRECTORY = string.Empty;
		public static string CONSOLE_LOG_DIRECTORY = string.Empty;
		public static string REPORTS_DIRECTORY = string.Empty;

		private static List<KeyValuePair<FileResource,string>> knownEditorResourceFiles = new List<KeyValuePair<FileResource,string>>();
		private static List<KeyValuePair<FileResource,string>> knownTrilleonResourceFiles = new List<KeyValuePair<FileResource,string>>();

		private static bool isSet = false;

		static void Set() {

			isSet = true;
			string basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			if(Application.platform == RuntimePlatform.WindowsEditor) {
				
				if(basePath.Contains(@"\\")) {
					
					FILE_PATH_SPLIT = @"\\";

				} else {
					
					FILE_PATH_SPLIT = @"\";

				}

			} else {
				
				FILE_PATH_SPLIT = "/";

			}

			knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.TrilleonConfig, "TrilleonConfig"));
			knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ReportJavascript, AutomationMaster.ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_JAVASCRIPT_USE").Replace("/", FILE_PATH_SPLIT)));
			knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ReportCss, AutomationMaster.ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_CSS_USE").Replace("/", FILE_PATH_SPLIT)));

			#if UNITY_EDITOR

			AssetDatabase.Refresh();

            knownEditorResourceFiles.Add(new KeyValuePair<FileResource, string>(FileResource.CustomizationConfig, "nexus_customization.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource, string>(FileResource.LatestTestResults, "nexus_test_manifest_latest_test_results.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.LaunchInstructions, "nexus_test_manifest_launch_instructions.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ManifestGUISettings, "nexus_test_manifest_categories_foldout_bools.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.BuddyHistory, "nexus_buddy_data_history.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.Favorites, "nexus_manifest_favorites.config"));
            knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.FavoritesUnit, "nexus_manifest_favorites_unit.config"));

			BASE_NON_UNITY_PATH = string.Empty;
			if(Application.platform == RuntimePlatform.WindowsEditor) {

				BASE_NON_UNITY_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			} else {

				string[] pathPieces = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Split(new string[] { FILE_PATH_SPLIT }, StringSplitOptions.None);
				if(pathPieces.Length <= 2) {

					string error = string.Format("Unrecognized file path encountered. Trilleon cannot interact with local files, and will not be fully functional until the issue is corrected. Path [{0}]", basePath);
					AutoConsole.PostMessage(error);
					return;

				}
				BASE_NON_UNITY_PATH = string.Format("/{0}/{1}", pathPieces[1], pathPieces[2]);

			}

			RESOURCES_DIRECTORY = string.Format("{0}{4}{1}{4}{2}{3}", BASE_NON_UNITY_PATH, TOP_LEVEL_FOLDER_NAME, GameMaster.GAME_NAME, AutomationMaster.ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY").Replace("/", FILE_PATH_SPLIT), FILE_PATH_SPLIT);
			if(!Directory.Exists(RESOURCES_DIRECTORY)) {
				
				Directory.CreateDirectory(Path.GetDirectoryName(RESOURCES_DIRECTORY));

			}

			CONSOLE_LOG_DIRECTORY = string.Format("{0}{4}{1}{4}{2}{3}", BASE_NON_UNITY_PATH, TOP_LEVEL_FOLDER_NAME, GameMaster.GAME_NAME, AutomationMaster.ConfigReader.GetString("EDITOR_RESOURCE_CONSOLE_LOG_DIRECTORY").Replace("/", FILE_PATH_SPLIT), FILE_PATH_SPLIT);
			if(!Directory.Exists(CONSOLE_LOG_DIRECTORY)) {
				
				Directory.CreateDirectory(Path.GetDirectoryName(CONSOLE_LOG_DIRECTORY));

			}

			REPORTS_DIRECTORY = string.Format("{0}{4}{1}{4}{2}{3}", BASE_NON_UNITY_PATH, TOP_LEVEL_FOLDER_NAME, GameMaster.GAME_NAME, AutomationMaster.ConfigReader.GetString("EDITOR_RESOURCE_TEST_REPORTS_DIRECTORY").Replace("/", FILE_PATH_SPLIT), FILE_PATH_SPLIT);
			if(!Directory.Exists(REPORTS_DIRECTORY)) {
				
				Directory.CreateDirectory(Path.GetDirectoryName(REPORTS_DIRECTORY));

			}

			SCREENSHOTS_DIRECTORY = string.Format("{0}screenshots{1}", RESOURCES_DIRECTORY, FILE_PATH_SPLIT);
			if(!Directory.Exists(SCREENSHOTS_DIRECTORY)) {
				
				//This is the single report screenshot storage directory.
				Directory.CreateDirectory(Path.GetDirectoryName(SCREENSHOTS_DIRECTORY));

			}

			//Create any missing required files.
			for(int k = 0; k < knownEditorResourceFiles.Count; k++) {
				
				string fileName = string.Format("{0}{1}{2}", RESOURCES_DIRECTORY, FILE_PATH_SPLIT, knownEditorResourceFiles[k].Value);
				if(!Exists(fileName)) {
					
					File.WriteAllText(fileName, string.Empty);

				}

			}

			#endif

		}

		public static void ExtendKnownTrilleonResources(string resourceName) {

			if(!knownTrilleonResourceFiles.FindAll(x => x.Value == resourceName).Any()) {

				knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ExtendableResource, resourceName));

			}

		}

		public static void ExtendKnownEditorResources(string resourceName) {

			if(!knownEditorResourceFiles.FindAll(x => x.Value == resourceName).Any()) {

				knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ExtendableResource, resourceName));

			}

		}

		public static void CopyFile(string copyThis, string destination) {

			if(!Exists(copyThis)) {
				
				AutoConsole.PostMessage(string.Format("Could not copy \"{0}\" to \"{1}\". File to copy does not currently exist.", copyThis.Replace("/", FILE_PATH_SPLIT), destination.Replace("/", FILE_PATH_SPLIT)), MessageLevel.Abridged);
                return;

			}
			if(Exists(destination)) {

				File.Delete(destination);

			}
			File.Copy(copyThis, destination);
			AutoConsole.PostMessage(string.Format("Copied \"{0}\" to \"{1}\"", copyThis, destination), MessageLevel.Verbose);

		}

		public static void SaveUnboundFile(string fileNameWithPath, string contents, bool overwrite = true) {

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR

			string directory = fileNameWithPath.Replace("/", FILE_PATH_SPLIT).Contains(BASE_NON_UNITY_PATH) ? fileNameWithPath.Replace("/", FILE_PATH_SPLIT) : BASE_NON_UNITY_PATH + fileNameWithPath.Replace("/", FILE_PATH_SPLIT);
			string path = directory.Substring(0, directory.LastIndexOf(FILE_PATH_SPLIT));
			if(!Directory.Exists(path)) {

				Directory.CreateDirectory(path);
				Set();

			}

			if(overwrite || !Exists(fileNameWithPath.Replace("/", FILE_PATH_SPLIT))) {

				Set();
				File.WriteAllText(fileNameWithPath.Replace("/", FILE_PATH_SPLIT), contents);

			} else {
				
				File.AppendAllText(fileNameWithPath.Replace("/", FILE_PATH_SPLIT), contents);

			}

			#endif

		}

		public static void SaveConsoleFile(string fileNameWithPath, string contents, bool overwrite = true) {

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR

			string fileNameComplete = string.Format("{0}{1}{2}", CONSOLE_LOG_DIRECTORY, FILE_PATH_SPLIT, fileNameWithPath.Replace("/", FILE_PATH_SPLIT));
			SaveUnboundFile(fileNameComplete, contents, overwrite);

			#endif

		}

		public static string[] ReadUnboundFile(string directoryWithFileName) {

			if(!isSet) {
				
				Set();

			}

			if(Exists(directoryWithFileName.Replace("/", FILE_PATH_SPLIT))) {
				
				return File.ReadAllLines(directoryWithFileName.Replace("/", FILE_PATH_SPLIT));

			}
			return new string[] {};

		}

		#region Config Reader

		public static void SaveConfig(string key, string value){

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR
			string[] currentKeyVals = GetTextResource(FileResource.TrilleonConfig).Split('\n');
			StringBuilder textInfo = new StringBuilder();

			for(int kv = 0; kv < currentKeyVals.Length; kv++) {
				
				string[] keyVal = currentKeyVals[kv].Split('=');
				if(keyVal.Length == 1) {

					textInfo.AppendLine(keyVal[0]);

				} else if(keyVal[0] == key) {
					
					textInfo.AppendLine(string.Format("{0}={1}", keyVal[0], value));

				} else {
					
					textInfo.AppendLine(string.Format("{0}={1}", keyVal[0], keyVal[1]));

				}

			}

			//Overwrite and save settings to file.
			SaveTextResource(FileResource.TrilleonConfig, textInfo.ToString());

			AssetDatabase.Refresh();
			AutomationMaster.ConfigReader.Refresh();
			#endif

		}

		#endregion

		#region Non Unity Resources

		public static void SaveNonUnityTextResource(FileResource file, string value, bool overwrite = true){

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR

			if(!knownEditorResourceFiles.FindAll(x => x.Key == file).Any()) {
				
				AutoConsole.PostMessage("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
                return;

			}

			string directory = string.Format("{0}{1}{2}", RESOURCES_DIRECTORY, FILE_PATH_SPLIT, knownEditorResourceFiles.Find(x => x.Key == file).Value);
			if(!Directory.Exists(Path.GetDirectoryName(directory))) {
				
				Set();

			}

			if(!Exists(directory)) {

				Set();

			}

			if(overwrite) {

				File.WriteAllText(directory, value);

			} else {

				File.AppendAllText(directory, value);

			}

			#endif

		}

		public static void SaveNonUnityTextResource(FileResource file, StringBuilder value, bool overwrite = true) {

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR
			SaveNonUnityTextResource(file, value.ToString());
			#endif

		}

		/// <summary>
		/// Returns the text resource by the resource enumeration name, or by the provided string name if it is an extendable resource.
		/// </summary>
		public static string GetNonUnityTextResource(FileResource file, string extendableResourceName = "") {

			if(!isSet) {
				Set();
			}

			if(!knownEditorResourceFiles.FindAll(x => x.Key == file && (file == FileResource.ExtendableResource ? x.Value == extendableResourceName : true)).Any()) {
				
                AutoConsole.PostMessage("The supplied file is not an expected Trilleon resource. Make sure you are referencing a file declared in the FileBroker.cs Set() logic."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
                return string.Empty;

            }

			string directory = string.Format("{0}{1}{2}", RESOURCES_DIRECTORY, FILE_PATH_SPLIT, knownEditorResourceFiles.Find(x => x.Key == file).Value);
			string fileText = string.Empty;
			if(!Exists(directory)) {

				Set();

			} 
			fileText = File.ReadAllText(directory);

			return fileText;

		}

		#endregion

		#region Unity Resources

		public static string GetTextResource(FileResource file, string extendableResourceName = "") {

			if(!isSet) {
				
				Set();

			}

			if(!knownTrilleonResourceFiles.FindAll(x => x.Key == file).Any()) {
				
				AutoConsole.PostMessage("The supplied file is not an expected Trilleon resource. Make sure you are referencing a file declared in the FileBroker.cs Set() logic."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
                return string.Empty;

			}

			TextAsset txt = (TextAsset)Resources.Load(knownTrilleonResourceFiles.Find(x => x.Key == file && (file == FileResource.ExtendableResource ? x.Value == extendableResourceName : true)).Value, typeof(TextAsset));

			if(txt != null) {
				
				return txt.text;

			}

			return string.Empty;

		}


		public static void SaveTextResource(FileResource file, string value) {

			if(!isSet) {
				
				Set();

			}

			#if UNITY_EDITOR

			if(!knownTrilleonResourceFiles.FindAll(x => x.Key == file).Any()) {
				
				AutoConsole.PostMessage("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
                return;
                 
			}

			string filePath = string.Format("{0}{1}{2}.txt", Application.dataPath.Replace("/", FILE_PATH_SPLIT), AutomationMaster.ConfigReader.GetString("UNITY_RESOURCES_FILE_PATH").Replace("/", FILE_PATH_SPLIT), knownTrilleonResourceFiles.Find(x => x.Key == file).Value);
			File.WriteAllText(filePath, value);

			AssetDatabase.Refresh();
			if(file == FileResource.TrilleonConfig) {

				AutomationMaster.ConfigReader.Refresh();

			}

			#endif

		}


		#endregion

		#if UNITY_EDITOR

		#region Create File Any

		public static void CreateFileAtPath(string path, string contents) {

			if(!isSet) {
				
				Set();

			}

			File.WriteAllText(path, contents);

		}

		#endregion

		#endif

		public static bool Exists(string path) {

			return File.Exists(path.Replace("/", FILE_PATH_SPLIT));

		}

		public static void DeleteUnboundFile(string fileName){

			if(!isSet) {

				Set();

			}

			#if UNITY_EDITOR
			if(Exists(fileName.Replace("/", FILE_PATH_SPLIT))) {

				File.Delete(fileName.Replace("/", FILE_PATH_SPLIT));

			}
			#endif

		}

		public static void DeleteUnboundDirectory(string directory){

			if(!isSet) {

				Set();

			}

			#if UNITY_EDITOR
			if(Directory.Exists(directory.Replace("/", FILE_PATH_SPLIT))) {

				Directory.Delete(directory.Replace("/", FILE_PATH_SPLIT), true);

			}
			#endif

		}

		public static void ClearCache() {

			DeleteUnboundDirectory(CONSOLE_LOG_DIRECTORY);
			Directory.CreateDirectory(CONSOLE_LOG_DIRECTORY);

			string screenshotsDirectory = string.Format("{0}/screenshots", RESOURCES_DIRECTORY);
			DeleteUnboundDirectory(screenshotsDirectory);
			Directory.CreateDirectory(screenshotsDirectory);

			DeleteUnboundFile(string.Format("{0}/{1}", RESOURCES_DIRECTORY, "nexus_test_manifest_launch_instructions.txt"));
			DeleteUnboundFile(string.Format("{0}/{1}", RESOURCES_DIRECTORY, "nexus_test_manifest_categories_foldout_bools.txt"));
			DeleteUnboundFile(string.Format("{0}/{1}", RESOURCES_DIRECTORY, "nexus_buddy_data_history.txt"));
			DeleteUnboundFile(string.Format("{0}/{1}", RESOURCES_DIRECTORY, "nexus_tab_preferences.txt"));

		}

	}

}
