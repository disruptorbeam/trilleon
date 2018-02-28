#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace TrilleonAutomation {

	public static class FileBroker {

		public static string BASE_RESOURCE_PATH = string.Empty;
		public static string BASE_NON_UNITY_PATH = string.Empty;
		public static string FILE_PATH_SPLIT = string.Empty;

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
			knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ReportJavascript, ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_JAVASCRIPT_USE")));
			knownTrilleonResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ReportCss, ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_CSS_USE")));

			BASE_RESOURCE_PATH = string.Format("{0}/Automation/Xtra/Resources/", Application.dataPath);


			#if UNITY_EDITOR

			AssetDatabase.Refresh();

			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.LatestTestResults, "nexus_test_manifest_latest_test_results.txt"));
			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.LaunchInstructions, "nexus_test_manifest_launch_instructions.txt"));
			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.ManifestGUISettings, "nexus_test_manifest_categories_foldout_bools.txt"));
			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.BuddyHistory, "nexus_buddy_data_history.txt"));
			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.NexusTabs, "nexus_tab_preferences.txt"));
			knownEditorResourceFiles.Add(new KeyValuePair<FileResource,string>(FileResource.Favorites, "nexus_manifest_favorites.txt"));

			BASE_NON_UNITY_PATH = string.Empty;
			if(Application.platform == RuntimePlatform.WindowsEditor) {

				BASE_NON_UNITY_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			} else {

				string[] pathPieces = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Split(new string[] { FILE_PATH_SPLIT }, StringSplitOptions.None);
				if(pathPieces.Length <= 2) {

					string error = string.Format("Unrecognized file path encountered. Trilleon cannot interact with local files, and will not be fully functional until the issue is corrected. Path [{0}]", basePath);
					AutoConsole.PostMessage(error);
					Debug.Log(error);
					return;

				}
				BASE_NON_UNITY_PATH = string.Format("/{0}/{1}", pathPieces[1], pathPieces[2]);

			}

			string directory = string.Format("{0}{1}{2}/", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY"), GameMaster.GAME_NAME);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(Path.GetDirectoryName(directory));
			}

			directory = string.Format("{0}{1}{2}/", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_CONSOLE_LOG_DIRECTORY"), GameMaster.GAME_NAME);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(Path.GetDirectoryName(directory));
			}

			directory = string.Format("{0}{1}{2}/", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_TEST_REPORTS_DIRECTORY"), GameMaster.GAME_NAME);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(Path.GetDirectoryName(directory));
			}

			directory = string.Format("{0}{1}/screenshots/", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY"));
			if(!Directory.Exists(directory)) {
				//This is the single report screenshot storage directory.
				Directory.CreateDirectory(Path.GetDirectoryName(directory));
			}

			string file = string.Format("{0}TrilleonConfig.txt", BASE_RESOURCE_PATH);
			FileInfo fileInfo = new FileInfo(file);
			fileInfo.IsReadOnly = false;
			file = string.Format("{0}{1}.txt",  BASE_RESOURCE_PATH, ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_JAVASCRIPT_USE"));
			fileInfo = new FileInfo(file);
			fileInfo.IsReadOnly = false;
			file = string.Format("{0}{1}.txt", BASE_RESOURCE_PATH, ConfigReader.GetString("AUTOMATION_RESULTS_REPORT_CSS_USE"));
			fileInfo = new FileInfo(file);
			fileInfo.IsReadOnly = false;

			//Create any missing required files.
			for(int k = 0; k < knownEditorResourceFiles.Count; k++) {
				string fileName = string.Format("{0}{1}{2}/{3}", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY"), GameMaster.GAME_NAME, knownEditorResourceFiles[k].Value);
				if(!File.Exists(fileName)) {
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

			if(!File.Exists(copyThis)) {
				AutoConsole.PostMessage(string.Format("Could not copy \"{0}\" to \"{1}\". File to copy does not currently exist.", copyThis, destination), MessageLevel.Abridged);
			}
			File.Copy(copyThis, destination);
			AutoConsole.PostMessage(string.Format("Copied \"{0}\" to \"{1}\"", copyThis, destination), MessageLevel.Verbose);

		}

		public static void SaveUnboundFile(string fileNameWithPath, string contents, bool overwrite = true) {

			if(!isSet) {
				Set();
			}

			#if UNITY_EDITOR

			string directory = BASE_NON_UNITY_PATH + fileNameWithPath;
			if (!Directory.Exists(Path.GetDirectoryName(directory))) {

				Directory.CreateDirectory(Path.GetDirectoryName(directory));
				Set();

			}

			if(overwrite || !File.Exists(fileNameWithPath)) {

				Set();
				File.WriteAllText(fileNameWithPath, contents);

			} else {

				File.AppendAllText(fileNameWithPath, contents);

			}

			#endif

		}

		public static void SaveConsoleFile(string fileNameWithPath, string contents, bool overwrite = true) {

			if(!isSet) {
				Set();
			}

			#if UNITY_EDITOR

			string fileNameComplete = string.Format("{0}{1}{2}/{3}", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_CONSOLE_LOG_DIRECTORY"), GameMaster.GAME_NAME, fileNameWithPath);
			SaveUnboundFile(fileNameComplete, contents, overwrite);

			#endif

		}

		public static string[] ReadUnboundFile(string directoryWithFileName) {

			if(!isSet) {
				Set();
			}

			if(File.Exists(directoryWithFileName)) {
				return File.ReadAllLines(directoryWithFileName);
			}
			return new string[] {};

		}

		public static void DeleteFile(string fileName){

			if(!isSet) {
				Set();
			}

			#if UNITY_EDITOR

			File.Delete(fileName);

			#endif

		}

		#region Config Reader

		public static void SaveConfig(string key, string value){

			if(!isSet) {
				Set();
			}

			#if UNITY_EDITOR

			string[] currentKeyVals = FileBroker.GetTextResource(FileResource.TrilleonConfig).Split('\n');
			StringBuilder textInfo = new StringBuilder();

			for(int kv = 0; kv < currentKeyVals.Length; kv++) {
				string[] keyVal = currentKeyVals[kv].Split('=');
				if(keyVal[0] == key) {
					textInfo.Append(string.Format("{0}={1}", keyVal[0], value));
				} else {
					textInfo.Append(string.Format("{0}={1}", keyVal[0], keyVal[1]));
				}
			}

			//Overwrite and save settings to file.
			FileBroker.SaveTextResource(FileResource.TrilleonConfig, textInfo.ToString());

			AssetDatabase.Refresh();
			ConfigReader.ForceRefreshConfigData();

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
				Debug.Log("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
			}

			string directory = string.Format("{0}{1}{2}/{3}", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY"),  GameMaster.GAME_NAME, knownEditorResourceFiles.Find(x => x.Key == file).Value);
			if (!Directory.Exists(Path.GetDirectoryName(directory))) {

				Set();
				Directory.CreateDirectory(Path.GetDirectoryName(directory));

			}

			if (!File.Exists(directory)) {

				Set();
				Directory.CreateDirectory(Path.GetDirectoryName(directory));

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
				Debug.Log("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
			}

			string directory = string.Format("{0}{1}{2}/{3}", BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY"), GameMaster.GAME_NAME, knownEditorResourceFiles.Find(x => x.Key == file).Value);

			string fileText = string.Empty;
			if (!File.Exists(directory)) {

				Set();
				File.Create(directory);

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
				Debug.Log("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
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
				Debug.Log("The supplied file could not be found. Make sure you are referencing a file stored outside of the Unity project."); //No usage of string.Format with file name data. Using Enum.GetName here causes exceptions on compile "Recursive Serialization is not supported. You can't dereference a PPtr while loading.".
			}

			string filePath = string.Format("{0}{1}{2}.txt", Application.dataPath, ConfigReader.GetString("AUTOMATION_RESOURCES_FOLDER_PATH"), knownTrilleonResourceFiles.Find(x => x.Key == file).Value);
			File.WriteAllText(filePath, value);

			AssetDatabase.Refresh();

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

			return File.Exists(path);

		}

	}

}