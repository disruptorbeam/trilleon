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
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using UnityEngine.UI;
using System;
using UnityEditor;

namespace TrilleonAutomation {

	public class TestMonitorHelpers {
		
		public static string NAME = "TestMonitorHelpers";

		private static Overseer overseer {
			get { 
				if(inspectOverseer == null){
					inspectOverseer = GameObject.Find(NAME).GetComponent<Overseer>();
				}
				return inspectOverseer;
			}
		}
		private static Overseer inspectOverseer;

		private static Driver driver {
			get { 
				if(inspectDriver == null){
					inspectDriver = GameObject.Find(NAME).GetComponent<Driver>();
				}
				return inspectDriver;
			}
		}
		private static Driver inspectDriver;

		public static GameObject Helper;
		public static List<Type> allTestObjects = new List<Type>();

		public static string SetParentComponentObject(GameObject child) {

			Component thisParentComponent = null;
			Component foundMatch = null;
			GameObject currentTreeItem = null;
			GameObject currentChild = child;

			while(true) {

				if(currentChild.transform.parent == null) {
					if(currentChild = child) {
						//A selected GameObject is at the top level and has no parents.
						overseer.parentObject = null;
					} else {
						//A selected GameObject lacks a top level Component for TestObjects, and inherets its TestObject name from the object name.
						overseer.parentObject = currentChild.transform.parent.gameObject;
						return string.Format("{0}TestObject", thisParentComponent.gameObject.name);
					}
					break;
				}
				currentTreeItem = currentChild.transform.parent.gameObject;

				//Get list of expected top level parent Components that we should see on a top level object.
				for(int i = 0; i < GameMaster.ExpectedTopLevelMasterScripts.Count; i++) {
					thisParentComponent = currentTreeItem.GetComponent(GameMaster.ExpectedTopLevelMasterScripts[i]);
					if(thisParentComponent != null){
						foundMatch = thisParentComponent;
					}
				}

				currentChild = currentTreeItem;

			}

			if(foundMatch != null) {
				overseer.parentObject = foundMatch.gameObject;
				overseer.parentBaseType = foundMatch.GetType().BaseType.Name;
				return string.Format("{0}TestObject", foundMatch.GetType().Name);
			}

			return string.Empty;

		}   

		public static string GetNewTestObjectPath(string fileName) {
			return string.Format("{0}/Automation/{1}/Core/{2}/{3}.cs", Application.dataPath, AutomationMaster.ConfigReader.GetString("GAME_FOLDER_NAME"), AutomationMaster.ConfigReader.GetString("GAME_TEST_OBJECTS_FOLDER_NAME"), fileName);
		}

		public static void FullReset() {

			GameObject.DestroyImmediate(GameObject.Find(NAME));
			CreateTestObjectHelper();

		}

		public static void CreateTestObjectHelper() {

			Helper = GameObject.Find(NAME);
			if(Helper == null) {
				
				Helper = new GameObject();
				Helper.name = NAME;
				Helper.hideFlags = HideFlags.HideAndDontSave; //Don't save this object, and don't show it in the scene.

			}

			if(Helper.GetComponent<Driver>() == null) {
				
				inspectDriver = Helper.AddComponent<Driver>();

			}
			if(Helper.GetComponent<Overseer>() == null) {
				
				inspectOverseer = Helper.AddComponent<Overseer>();

			}
			if(Helper.GetComponent<Arbiter>() == null) {
				
				Helper.AddComponent<Arbiter>();

			}
				
			List<Type> typesAll = new List<Type>();
			List<Assembly> assembliesAll = AppDomain.CurrentDomain.GetAssemblies().ToList();
			for(int x = 0; x < assembliesAll.Count; x++) {

				typesAll.AddRange(assembliesAll[x].GetTypes());

			}

			allTestObjects = typesAll.FindAll(y => y.Name.EndsWith("TestObject"));

		}

		//Get the types of components on this object. Return only MonoScript objects.
		public static List<KeyValuePair<Component,Type>> FindMonoScriptReferences(GameObject go) {

			List<KeyValuePair<Component,Type>> results = new List<KeyValuePair<Component,Type>>();
			List<MonoBehaviour> components = go.GetComponents<MonoBehaviour>().ToList();

			for(int i = 0; i < components.Count; i++) {

				//Skip if this is a base Unity object under UnityEngine namespace.
				try {

					if(components[i].GetType().ReflectedType.ReflectedType.Name.ToLower().Contains("unityengine")) {
						continue;
					}

				} catch{ }

				Type type = components[i].GetType();
				if(type != null) {

					MonoScript thisScript = MonoScript.FromMonoBehaviour(components[i]);

					if(!string.IsNullOrEmpty(thisScript.text)) {
						
						results.Add(new KeyValuePair<Component, Type>(components[i], type));

					}

				}

			}

			return results;

		}

		public static float DetermineRectHeightBasedOnLinesInNodeDetails(string details) {

			int lines = details.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
			return (lines * 14) + 25;

		}

	}

}
#endif
