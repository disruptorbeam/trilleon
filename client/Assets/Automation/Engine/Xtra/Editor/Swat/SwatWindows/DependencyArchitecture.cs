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
using System.Reflection;

namespace TrilleonAutomation {

	public class DependencyArchitecture : SwatWindow {

		List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>> DependencyOrderingMaster;
		List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>> DependencyOrderingMasterless;
		List<bool> FoldoutListMasterBools;
		List<int> FoldoutListMasterIds;
		List<bool> FoldoutListMasterLessBools;
		List<string> FoldoutListMasterLessIds;
		bool _buildingList, _errorInDependencyUsage;
		List<MonoScript> masterlessSriptFiles = new List<MonoScript>();

		public override void Set() {

			_buildingList = true;
			FoldoutListMasterIds = new List<int>();
			FoldoutListMasterBools = new List<bool>();
			FoldoutListMasterLessBools = new List<bool>();
			FoldoutListMasterLessIds = new List<string>();
			DependencyOrderingMaster = new List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>>();
			DependencyOrderingMasterless = new List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>>();
			GetDependencyClassTestStructure();
			_buildingList = false;

		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle description = new GUIStyle(GUI.skin.label);
			description.fontSize = 12;
			description.wordWrap = true;
			description.margin = new RectOffset(10, 10, 0, 0);
			description.normal.textColor = Swat.WindowDefaultTextColor;

			GUIStyle editorName = new GUIStyle(GUI.skin.label);
			editorName.fontSize = 16;
			editorName.fixedHeight = 20;
			editorName.fontStyle = FontStyle.Bold;
			editorName.padding = new RectOffset(8, 0, 0, 0);
			editorName.normal.textColor = Swat.WindowDefaultTextColor;

			GUIStyle open = new GUIStyle(GUI.skin.button);
			open.fontSize = 14;
			open.fixedHeight = 32;
			open.fixedWidth = 100;
			open.margin = new RectOffset(10, 10, 0, 0);
			open.normal.textColor = Swat.WindowDefaultTextColor;
			open.normal.background = open.active.background = open.focused.background = Swat.ToggleButtonBackgroundSelectedTexture;

			GUILayout.Space(18);
			EditorGUILayout.LabelField("Dependency Web", editorName);
			GUILayout.Space(4);
			EditorGUILayout.LabelField(@"This window displays Dependency Web usage throughout existing tests, helping to visualize the relationships between tests in a graphical web format." +
				"Because of the way this window is rendered, docking options are limited to Floating and DockNextToGameWindow only when opening.", description);
			GUILayout.Space(4);
			if(GUILayout.Button("Open", open)) {

				//Web must be viewed in a large screen move. Dock next to Game, or allow float.
				Swat.ShowWindow<DependencyVisualizer>(typeof(DependencyVisualizer), "Web", Swat.DockNextTo == DockNextTo.Float ? DockNextTo.Float : DockNextTo.GameView);

			}
			GUILayout.Space(18);

			GUIStyle s = new GUIStyle(GUI.skin.label);
			s.padding = new RectOffset(18, 0, 0, 0);
			s.normal.textColor = Swat.WindowDefaultTextColor;

			GUIStyle fo = new GUIStyle(EditorStyles.foldout);
			fo.padding = new RectOffset(10, 0, 0, 2);
			fo.normal.textColor = Swat.WindowDefaultTextColor;
			fo.normal.background = Swat.WindowBackgroundTexture;

			GUIStyle header = new GUIStyle(GUI.skin.label);
			header.fontSize = 16;
			header.fixedHeight = 18;
			header.normal.textColor = Swat.WindowDefaultTextColor;
			header.fontStyle = FontStyle.Bold;
			header.padding = new RectOffset(5, 0, 0, 0);

			GUIStyle padding = new GUIStyle();
			padding.margin = new RectOffset(10, 0, 0, 0);

			EditorGUILayout.Space();

			if(!_buildingList) {

				EditorGUILayout.LabelField("Master Dependencies", header);
				EditorGUILayout.Space();

				for(int x = 0; x < DependencyOrderingMaster.Count; x++) {

					int index = 0;
					//If this is a master DependencyClass list.
					List<int> match = FoldoutListMasterIds.FindAll(y => y == DependencyOrderingMaster[x].Key.Key);
					if(!match.Any()) {

						index = FoldoutListMasterIds.Count;
						FoldoutListMasterIds.Add(index);
						FoldoutListMasterBools.Add(false);

					} else {

						index = FoldoutListMasterIds.FindIndex(a => a == DependencyOrderingMaster[x].Key.Key);

					}

					string FoldoutTitle = string.Format( "Dependency Class {0}", index);
					EditorGUILayout.BeginHorizontal(padding);
					FoldoutListMasterBools[index] = Nexus.Self.Foldout(FoldoutListMasterBools[index], FoldoutTitle, true, fo);
					EditorGUILayout.EndHorizontal();

					if(FoldoutListMasterBools[index]) {

						for(int m = 0; m < DependencyOrderingMaster[x].Value.Count; m++) {

							//Add this type as a component if not already a MonoBehavior instance.
							if(TestMonitorHelpers.Helper.GetComponent(DependencyOrderingMaster[x].Value[m].Key) == null){

								TestMonitorHelpers.Helper.AddComponent(DependencyOrderingMaster[x].Value[m].Key);

							}

							MonoBehaviour thisComponent = TestMonitorHelpers.Helper.GetComponent(DependencyOrderingMaster[x].Value[m].Key) as MonoBehaviour;
							int scriptIndex = 0;
							MonoScript thisScript = MonoScript.FromMonoBehaviour(thisComponent);

							if(!masterlessSriptFiles.Contains(thisScript)) {

								masterlessSriptFiles.Add(thisScript);
								scriptIndex = masterlessSriptFiles.Count - 1;

							} else {

								scriptIndex = masterlessSriptFiles.FindIndex(a => a == thisScript);

							}

							EditorGUILayout.LabelField(string.Format("{0}) {1}", m + 1, DependencyOrderingMaster[x].Value[m].Value.Name), s);
							EditorGUILayout.BeginHorizontal(padding);
							GUILayout.Space(20);
							masterlessSriptFiles[scriptIndex] = thisScript;
							masterlessSriptFiles[scriptIndex] = EditorGUILayout.ObjectField(masterlessSriptFiles[scriptIndex], DependencyOrderingMaster[x].Value[m].Key, true, new GUILayoutOption[] { GUILayout.MaxWidth(175) }) as MonoScript;
							EditorGUILayout.EndHorizontal();

							//If the MonoScript is empty, the current test class has a different name than the file containing it. Warn that this should not be the case.
							if(string.IsNullOrEmpty(thisScript.text)) {

								s.normal.textColor = Color.red;
								EditorGUILayout.LabelField("The above test class name does not match the file name. Please ensure that they match.", s);
								s.normal.textColor = Swat.WindowDefaultTextColor;

							}

						}

					}

					EditorGUILayout.Space();

				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Masterless Dependencies", header);
				EditorGUILayout.Space();

				for(int z = 0; z < DependencyOrderingMasterless.Count; z++) {

					int index = 0;
					//If this is a master DependencyClass list.
					List<string> match = FoldoutListMasterLessIds.FindAll(y => y == DependencyOrderingMasterless[z].Key.Value);
					if(!match.Any()) {

						index = FoldoutListMasterLessIds.Count; 
						FoldoutListMasterLessIds.Add(DependencyOrderingMasterless[z].Key.Value);
						FoldoutListMasterLessBools.Add(false);

					} else {

						index = FoldoutListMasterLessIds.FindIndex(a => a == DependencyOrderingMasterless[z].Key.Value);

					}

					string FoldoutTitle = DependencyOrderingMasterless[z].Key.Value;

					//Add this class type as a component if not already a MonoBehavior instance.
					if(TestMonitorHelpers.Helper.GetComponent(DependencyOrderingMasterless[z].Value[0].Key) == null) {

						TestMonitorHelpers.Helper.AddComponent(DependencyOrderingMasterless[z].Value[0].Key);

					}

					MonoBehaviour thisComponent = TestMonitorHelpers.Helper.GetComponent(DependencyOrderingMasterless[z].Value[0].Key) as MonoBehaviour;
					int scriptIndex = 0;
					MonoScript thisScript = MonoScript.FromMonoBehaviour(thisComponent);

					if(!masterlessSriptFiles.Contains(thisScript)) {

						masterlessSriptFiles.Add(thisScript);
						scriptIndex = masterlessSriptFiles.Count - 1;

					} else {

						scriptIndex = masterlessSriptFiles.FindIndex(a => a == thisScript);

					}

					EditorGUILayout.BeginVertical(padding);
					masterlessSriptFiles[scriptIndex] = thisScript;
					masterlessSriptFiles[scriptIndex] = EditorGUILayout.ObjectField(masterlessSriptFiles[scriptIndex], DependencyOrderingMasterless[z].Value[0].Key, true, new GUILayoutOption[] { GUILayout.MaxWidth(175) }) as MonoScript;
					FoldoutListMasterLessBools[index] = Nexus.Self.Foldout(FoldoutListMasterLessBools[index], FoldoutTitle, true, fo);
					EditorGUILayout.EndVertical();

					//If the MonoScript is empty, the current test class has a different name than the file containing it. Warn that this should not be the case.
					if(thisScript != null && string.IsNullOrEmpty(thisScript.text)) {

						s.normal.textColor = Color.red;
						EditorGUILayout.LabelField("The above test class name does not match the file name. Please ensure that they match.", s);
						s.normal.textColor = Swat.WindowDefaultTextColor;

					}

					if(FoldoutListMasterLessBools[index]) {

						for(int m = 0; m < DependencyOrderingMasterless[z].Value.Count; m++) {

							EditorGUILayout.LabelField(string.Format("{0}) {1}", m + 1, DependencyOrderingMasterless[z].Value[m].Value.Name), s);

						}

					}

					EditorGUILayout.Space();

				}

			}

			if(_errorInDependencyUsage) {

				GUIStyle error = new GUIStyle(GUI.skin.label);
				error.padding = new RectOffset(10, 0, 0, 0);
				error.wordWrap = true;
				error.normal.textColor = Color.red;
				EditorGUILayout.LabelField("An error has been detect in the framework's usage of dependency attributes. Please run validation tools to identify where the error is located.", error);

			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}


		void GetDependencyClassTestStructure() {

			//Get all automation test methods.
			List<KeyValuePair<Type, MethodInfo>> allTestMethods = new List<KeyValuePair<Type, MethodInfo>>();
			List<Type> AutomationClasses = AutomationMaster.GetAutomationClasses();
			List<Type> masterDependencyClasses = AutomationClasses.FindAll(x => x.GetCustomAttributes(typeof(DependencyClass), false).ToList().Any());
			List<Type> masterlessDependencyClasses = AutomationClasses.FindAll(x => {
				return !x.GetCustomAttributes(typeof(DependencyClass), false).ToList().Any() && x.GetMethods().ToList().FindAll(y => {
					DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(y, typeof(DependencyTest));
					return dt != null;
				}).Any();
			});

			for(int i = 0; i < AutomationClasses.Count; i++) {

				List<MethodInfo> methods = AutomationClasses[i].GetMethods().ToList().FindAll(y => y.GetCustomAttributes(typeof(Automation), false).ToList().Any());
				for(int x = 0; x < methods.Count; x++) {
					allTestMethods.Add(new KeyValuePair<Type, MethodInfo>(AutomationClasses[i], methods[x]));
				}   

			}

			//Add groupings of master dependencies (DependencyTests under DependencyClasses)
			int classId = 0;
			for(int a = 0; a < masterDependencyClasses.Count; a++) {

				List<KeyValuePair<Type,MethodInfo>> allMasterDependencyMethods = new List<KeyValuePair<Type,MethodInfo>>();
				for(int all = 0; all < masterDependencyClasses.Count; all++) {
					
					List<MethodInfo> allMethods = masterDependencyClasses[all].GetMethods().ToList().FindAll(y => y.GetCustomAttributes(typeof(DependencyTest), false).ToList().Any());
					for(int am = 0; am < allMethods.Count; am++) {
						
						allMasterDependencyMethods.Add(new KeyValuePair<Type,MethodInfo>(masterDependencyClasses[all], allMethods[am]));

					}

				}

				List<MethodInfo> thisDependencyClassIdMethods = allMasterDependencyMethods.FindAll(x => {
					//Return all methods under the current DependencyClass ID.
					DependencyClass dc = (DependencyClass)Attribute.GetCustomAttribute(x.Key, typeof(DependencyClass));
					return dc != null && dc.order == classId;
				}).ExtractListOfValuesFromKeyValList();

				thisDependencyClassIdMethods = thisDependencyClassIdMethods.FindAll(x => {
					//Return all methods that have a DependencyTest attribute.
					DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(x, typeof(DependencyTest));
					return dt != null;
				});

				//If any methods found.
				if(thisDependencyClassIdMethods.Any()) {

					KeyValuePair<int,string> DepClassThis = new KeyValuePair<int,string>(classId, string.Empty); //This Dep Class id.
					List<KeyValuePair<Type,MethodInfo>> DepTestThese = new List<KeyValuePair<Type,MethodInfo>>();

					for(int ms = 0; ms < thisDependencyClassIdMethods.Count; ms++) {

						List<MethodInfo> nextMethod = thisDependencyClassIdMethods.FindAll(x => {
							DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(x, typeof(DependencyTest));
							return dt.order == ms + 1;
						});

						if(nextMethod.Any()) {
							
							//Add method to next order in list.
							DepTestThese.Add(new KeyValuePair<Type,MethodInfo>(allMasterDependencyMethods.FindAll(x => x.Value.Name == nextMethod.First().Name).First().Key, nextMethod.First()));
						
						} else {

							int duplicateId = 0;
							List<KeyValuePair<string,int>> ordersFound = new List<KeyValuePair<string,int>>();
							//Check if there is a duplicate ID, rather than a missing ID.
							for(int d = 0; d < thisDependencyClassIdMethods.Count; d++) {

								DependencyTest dt = (DependencyTest)Attribute.GetCustomAttribute(thisDependencyClassIdMethods[d], typeof(DependencyTest));
								if(ordersFound.FindAll(x => x.Value == dt.order).Any()) {

									duplicateId = dt.order;
									ordersFound.Add(new KeyValuePair<string,int>(thisDependencyClassIdMethods[d].Name, dt.order));
									break;

								} else {

									ordersFound.Add(new KeyValuePair<string,int>(thisDependencyClassIdMethods[d].Name, dt.order));

								}

							}

							List<Type> allTestClassesThatShareThisMasterId = allMasterDependencyMethods.FindAll(x => { 
								DependencyClass dc = (DependencyClass)Attribute.GetCustomAttribute(x.Key, typeof(DependencyClass));
								return dc.order == DepClassThis.Key;
							}).ExtractListOfKeysFromKeyValList().Distinct();
							StringBuilder classNameList = new StringBuilder();
							for(int nl = 0; nl < allTestClassesThatShareThisMasterId.Count; nl++) {

								classNameList.Append(allTestClassesThatShareThisMasterId[nl].Name);
								if(nl + 1 != allTestClassesThatShareThisMasterId.Count) {

									classNameList.Append(", ");

								}

							}

							if(duplicateId > 0) {

								StringBuilder testNameList = new StringBuilder();
								for(int nl = 0; nl < ordersFound.Count; nl++) {

									testNameList.Append(ordersFound[nl].Key);
									if(nl + 1 != ordersFound.Count) {
										
										testNameList.Append(", ");

									}

								}
								_errorInDependencyUsage = true;
								throw new UnityException(string.Format("There multiple tests with the DependencyTest ID of {0} under the DependencyClass ( Name(s): {1} - DependencyClass ID: {2} ). Tests with duplicate ID ( {3} )", duplicateId, classNameList.ToString(), DepClassThis.Key, testNameList.ToString()));

							} else {

								_errorInDependencyUsage = true;
								throw new UnityException(string.Format("There should be a DependencyTest of ID {0} under the DependencyClass ( Name(s): {1} - ID: {2} )", ms + 1, classNameList.ToString(), DepClassThis.Key));
						
							}

						}

					}

					//Add this pairing to the building list of all Dependencies.
					DependencyOrderingMaster.Add(new KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>(DepClassThis, DepTestThese));

				} else {
					
					break;

				}

				classId++;
			}

			//Add groupings of master dependencies (DependencyTests under DependencyClasses)
			for(int b = 0; b < masterlessDependencyClasses.Count; b++) {

				List<KeyValuePair<Type,MethodInfo>> DepTestThese = new List<KeyValuePair<Type,MethodInfo>>();
				KeyValuePair<int,string> DepClassThis = new KeyValuePair<int,string>(0, masterlessDependencyClasses[b].Name); //This Dep Class id.

				//Add each method based on the id of the Dependency Test.
				List<MethodInfo> allMasterlessDependencyMethods = masterlessDependencyClasses[b].GetMethods().ToList().FindAll(y => y.GetCustomAttributes(typeof(DependencyTest), false).ToList().Any());
				for(int ms = 0; ms < allMasterlessDependencyMethods.Count; ms++) {
					MethodInfo nextMethod = allMasterlessDependencyMethods.FindAll(x => {
						DependencyTest dc = (DependencyTest)Attribute.GetCustomAttribute(x, typeof(DependencyTest));
						return dc.order == ms + 1;
					}).First();
					//Add method to next order in list.
					DepTestThese.Add(new KeyValuePair<Type,MethodInfo>(masterlessDependencyClasses[b], nextMethod));
				}

				//Add this pairing to the building list of all Dependencies.
				DependencyOrderingMasterless.Add(new KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>(DepClassThis, DepTestThese));

			}

			//Filter debug classes, unless debug classes represent ALL existing dependency archituecture tests. In that case, show debug tests to demonstrate how this editor window works.
			List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>> filteredDebugMaster = DependencyOrderingMaster.FindAll(x => !x.Value.FindAll(t => t.Key.GetCustomAttributes(typeof(DebugClass), false).Length > 0).Any());
			List<KeyValuePair<KeyValuePair<int,string>,List<KeyValuePair<Type,MethodInfo>>>> filteredDebugMasterfilteredDebugMaster = DependencyOrderingMasterless.FindAll(x => !x.Value.FindAll(t => t.Key.GetCustomAttributes(typeof(DebugClass), false).Length > 0).Any());
			if(filteredDebugMaster.Any() || filteredDebugMasterfilteredDebugMaster.Any()) {

				DependencyOrderingMaster = filteredDebugMaster;
				DependencyOrderingMasterless = filteredDebugMasterfilteredDebugMaster;

			}

		}

	}

}
