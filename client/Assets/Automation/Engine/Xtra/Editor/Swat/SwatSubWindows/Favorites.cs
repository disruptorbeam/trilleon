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

﻿using System;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace TrilleonAutomation {

	public class Favorites : SwatWindow {

		public List<KeyValuePair<string,List<KeyValuePair<bool,string>>>> FavoritesList = new List<KeyValuePair<string,List<KeyValuePair<bool,string>>>>();

		string internalDelimiter = "$$";
		string classIndicator = "*";
		string newName = string.Empty;
		string keyToDelete { get; set; }
		int EditId { get; set; }
		int SelectedCatIndex { get; set; }
		int SelectedTestIndex { get; set; }
		bool makeNew { get; set; }
		bool saveCurrent { get; set; }
		List<KeyValuePair<string,string>> buildingAdded = new List<KeyValuePair<string,string>>();
		string[] _requestedMethods;
		string[] _categories;

		public override void Set() {

			if(!AutomationMaster.AllMethodsInFramework.Any()) {

				AutomationMaster.SetAllMethodsInFramework();

			}

			string rawFavoritesData = FileBroker.GetNonUnityTextResource(FileResource.Favorites);
			List<string> favoritesEachRaw = rawFavoritesData.Split(AutomationMaster.DELIMITER).ToList();
			FavoritesList = new List<KeyValuePair<string,List<KeyValuePair<bool,string>>>>();
			for(int f = 0; f < favoritesEachRaw.Count; f++) {

				//Invalid data
				if(!favoritesEachRaw[f].Trim().Contains(internalDelimiter)) {
					
					continue;

				}

				List<string> pieces = favoritesEachRaw[f].Split(new string[] { internalDelimiter }, StringSplitOptions.None).ToList();
				string name = pieces.First();
				List<KeyValuePair<bool,string>> tests = new List<KeyValuePair<bool,string>>();
				for(int t = 1; t < pieces.Count; t++) {

					tests.Add(new KeyValuePair<bool,string>(pieces[t].EndsWith(classIndicator) ? true : false, pieces[t].EndsWith(classIndicator) ? pieces[t].Replace(classIndicator, string.Empty) : pieces[t]));

				}
				FavoritesList.Add(new KeyValuePair<string,List<KeyValuePair<bool,string>>>(name, tests));

			}
				
		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { 
		
			List<string> methods = AutomationMaster.AllMethodsInFramework.ExtractListOfKeysFromKeyValList(true, 1);
			methods.Sort();
			_requestedMethods = methods.ToArray();
			_categories = Nexus.Self.Tests.CatKeys.FindAll(x => !x.StartsWith("*")).ToArray();

		}

		public override void Render() {

			GUIStyle addNew = new GUIStyle(GUI.skin.button);
			addNew.margin = new RectOffset(20, 0, 0, 0);
			addNew.fixedWidth = 75;
			addNew.fixedHeight = 25;
			addNew.normal.textColor = Swat.ActionButtonTextColor;
			addNew.normal.background = Swat.ActionButtonTexture;

			GUIStyle delete = new GUIStyle(GUI.skin.button);
			delete.margin = new RectOffset(-10, 0, -5, 0);
			delete.fixedWidth = 50;
			delete.fixedHeight = 25;
			delete.normal.textColor = Swat.ActionButtonTextColor;
			delete.normal.background = Swat.ActionButtonTexture;

			GUIStyle launch = new GUIStyle(GUI.skin.button);
			launch.margin = new RectOffset(-10, 0, -5, 0);
			launch.fixedWidth = 50;
			launch.fixedHeight = 25;
			launch.normal.textColor = Swat.TabButtonTextColor;
			launch.normal.background = Swat.TabButtonBackgroundTexture;

			GUIStyle save = new GUIStyle(GUI.skin.button);
			save.margin = new RectOffset(20, 0, 0, 0);
			save.fixedWidth = 100;
			save.fixedHeight = 25;
			save.normal.textColor = Color.white;
			save.normal.background = Swat.MakeTexture(1, 1, Color.black);

			GUIStyle favoriteHeader = new GUIStyle(GUI.skin.label);
			favoriteHeader.padding = new RectOffset(10, 0, 0, 0);
			favoriteHeader.fontStyle = FontStyle.Bold;
			favoriteHeader.alignment = TextAnchor.LowerLeft;
			favoriteHeader.fontSize = 15;
			favoriteHeader.fixedHeight = 15f;

			GUIStyle nameField = new GUIStyle(GUI.skin.textField);
			nameField.margin = new RectOffset(20, 0, 5, 5);

			GUIStyle typeHeader = new GUIStyle(GUI.skin.label);
			typeHeader.padding = new RectOffset(20, 0, 0, 0);
			typeHeader.fontSize = 12;

			GUIStyle catFoldOut = new GUIStyle(EditorStyles.foldout);
			catFoldOut.margin = new RectOffset(20, 0, 0, 0);
			catFoldOut.fontStyle = FontStyle.Italic;
			catFoldOut.fontSize = 11;

			GUIStyle catTest = new GUIStyle(GUI.skin.label);
			catTest.padding = new RectOffset(30, 0, 0, 0);
			catTest.fontSize = 12;

			GUILayout.Space(25);
			GUIStyle cs = new GUIStyle(EditorStyles.label);
			cs.padding = new RectOffset(25, 15, 2, 2);
			cs.wordWrap = true;
			cs.fontStyle = FontStyle.Italic;
			Nexus.Self.Button(makeNew ? "Cancel" : "Add New", "Add new Favorite test run.", 
				new Nexus.SwatDelegate(delegate() {                
					makeNew = !makeNew;
					buildingAdded = new List<KeyValuePair<string,string>>();
				}), addNew);
			GUILayout.Space(35);

			if(makeNew) {

				SelectedCatIndex = Nexus.Self.DropDown(SelectedCatIndex, _categories, 20);
				GUILayout.Space(5);
				Nexus.Self.Button("Add", "Add To List.", 
					new Nexus.SwatDelegate(delegate() {                
						if(!buildingAdded.FindAll(x => x.Key == _categories[SelectedCatIndex]).Any()) {
							buildingAdded.Add(new KeyValuePair<string,string>(string.Format("{0}{1}", _categories[SelectedCatIndex], classIndicator), string.Empty));
						}
					}), addNew);
				GUILayout.Space(10);
				SelectedTestIndex = Nexus.Self.DropDown(SelectedTestIndex, _requestedMethods, 20);
				GUILayout.Space(5);
				Nexus.Self.Button("Add", "Add To List.", 
					new Nexus.SwatDelegate(delegate() {    
						if(!buildingAdded.FindAll(x => x.Value == _requestedMethods[SelectedTestIndex]).Any()) {
							buildingAdded.Add(new KeyValuePair<string,string>(GetTestsClassName(_requestedMethods[SelectedTestIndex]), _requestedMethods[SelectedTestIndex]));
						}
					}), addNew);

				GUILayout.Space(10);
				for(int b = 0; b < buildingAdded.Count; b++) {

					EditorGUILayout.LabelField(string.Format("{0}{1}", buildingAdded[b].Value.Length > 0 ? string.Format("({0}) ", buildingAdded[b].Key) : buildingAdded[b].Key, buildingAdded[b].Value.Length > 0 ? buildingAdded[b].Value : string.Empty), typeHeader);
					GUILayout.Space(2);

				}
				GUILayout.Space(10);

				EditorGUILayout.LabelField("Name:", typeHeader);
				newName = EditorGUILayout.TextField(newName, nameField, new GUILayoutOption[] { GUILayout.Width(150) });
				Nexus.Self.Button("Save", "Save Favorite.", 
					new Nexus.SwatDelegate(delegate() {
						if(newName.Trim().Length == 0) {

							SimpleAlert.Pop("A name is required to save this Favorites list.", null);

						} else if(buildingAdded.Count == 0) {

							SimpleAlert.Pop("At least one category or test needs to be added to this Favorites list to save.", null);

						} else {
							
							saveCurrent = true;
							makeNew = false;
							SaveNew();

						}
					}), save);
				GUILayout.Space(35);

			}

			int foldoutIndex = 0;
			for(int f = 0; f < FavoritesList.Count; f++) {

				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(320) });
				EditorGUILayout.LabelField(FavoritesList[f].Key, favoriteHeader);
				Nexus.Self.Button("Launch", "Edit this Favorite.", 
					new Nexus.SwatDelegate(delegate() {    
						//This is a Favorite list, and not a true Category. Gather requested tests/classes.
						List<KeyValuePair<bool,string>> favoriteList = FavoritesList[f].Value;
						string commandClasses = string.Empty;
						string commandTests = string.Empty;
						for(int x = 0; x < favoriteList.Count; x++) {

							//Is the next item in the list a test? If so, this category is only meant to define the tests that follow, so ignore it.
							if(favoriteList[x].Key && x + 1 < favoriteList.Count ? !favoriteList[x + 1].Key : false) {

								continue;

							}

							if(favoriteList[x].Key) {

								//All Tests In This Class
								string category = string.Empty;
								if(favoriteList[x].Value.Contains("(")) {
									category = favoriteList[x].Value.Replace("*", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split('(')[1].Trim(')');
								} else {
									category = favoriteList[x].Value.Replace("*", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty);
								}
								commandClasses += string.Format("{0},", category);

							} else {

								//Just This Test
								commandTests += string.Format("{0},", favoriteList[x].Value);

							}

						}
						string command = string.Format("&&{0}%{1}", commandClasses.Trim(','), commandTests.Trim(',')); ;
						Nexus.Self.Tests.LaunchTests(command, "mix");
					}), launch, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
				Nexus.Self.Button("Delete", "Edit this Favorite.", 
					new Nexus.SwatDelegate(delegate() {    
						keyToDelete = FavoritesList[f].Key;
						SimpleAlert.Pop("Are you sure you want to delete this Favorites list?", new EditorDelegate(DeleteFavorite));
					}), delete, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(10);

				for(int z = 0; z < FavoritesList[f].Value.Count; z++) {
					
					if(FavoritesList[f].Value[z].Key && (z + 1 == FavoritesList[f].Value.Count || FavoritesList[f].Value[z + 1].Key)) {

						EditorGUILayout.LabelField(FavoritesList[f].Value[z].Value, typeHeader);
						GUILayout.Space(10);

					} else if(FavoritesList[f].Value[z].Key) {

						Nexus.Self.Foldout(true, new GUIContent(FavoritesList[f].Value[z].Value), true, catFoldOut);
						GUILayout.Space(4);

						for(int o = z + 1; o < FavoritesList[f].Value.Count; o++) {

							if(FavoritesList[f].Value[o].Key) {

								break;

							}
							EditorGUILayout.LabelField(FavoritesList[f].Value[o].Value, catTest);

						}
						GUILayout.Space(10);

					}

				}

				foldoutIndex++;
				GUILayout.Space(20);

			}

		}

		void SaveNew() {

			buildingAdded = buildingAdded.OrderByKeys();
			StringBuilder sb = new StringBuilder();

			if(FavoritesList.Count > 0) {
				
				sb.Append(AutomationMaster.DELIMITER);

			} 

			sb.Append(newName);
			sb.Append(internalDelimiter);
			List<string> handledClasses = new List<string>();
			for(int x = 0; x < buildingAdded.Count; x++) {

				string className = buildingAdded[x].Key;
				if(handledClasses.Contains(className)) {

					continue;

				}
				List<KeyValuePair<string,string>> sameClass = buildingAdded.FindAll(k => k.Key == className);
				sameClass = sameClass.OrderByValues();
				string nextToAdd = string.Format("{0}{1}{2}{3}", className, classIndicator, buildingAdded[x].Value.Length > 0 ? internalDelimiter : string.Empty, buildingAdded[x].Value.Length > 0 ? (sameClass.Count > 1 ? string.Join(internalDelimiter, sameClass.ExtractListOfValuesFromKeyValList().ToArray()) : buildingAdded[x].Value) : string.Empty);
				sb.Append(nextToAdd);
				if(x + 1 != buildingAdded.Count) {
					
					sb.Append(internalDelimiter);

				}
				handledClasses.Add(className);

			}
			FileBroker.SaveNonUnityTextResource(FileResource.Favorites, sb.ToString(), false);
			newName = string.Empty;
			Set();
			Nexus.Self.Tests.Update_Data = true;

		}

		//Take raw file data and simply splice out substring that represents this Favorite's data.
		void DeleteFavorite() {

			string rawFavoritesData = FileBroker.GetNonUnityTextResource(FileResource.Favorites);
			List<string> split = rawFavoritesData.Split(new string[] { string.Format("{0}{1}", AutomationMaster.DELIMITER, keyToDelete) }, StringSplitOptions.None).ToList();

			StringBuilder newData = new StringBuilder();
			if(split.Count > 1) {

				//The item to be deleted is the first Favorites list in the file, so there will not be a complete splice.
				newData.Append(split.First());

			}
			split = split.Last().Split(AutomationMaster.DELIMITER).ToList();
			for(int s = 1; s < split.Count; s++) {

				newData.Append(string.Format("{0}{1}", AutomationMaster.DELIMITER, split[s]));

			}
			FileBroker.SaveNonUnityTextResource(FileResource.Favorites, newData.ToString(), true);
			Set();
			Nexus.Self.Tests.Update_Data = true;

		}

		string GetTestsClassName(string test) {

			return AutomationMaster.AllMethodsInFramework.Find(x => x.Key.Split(AutomationMaster.DELIMITER).Last() == test).Key.Split(AutomationMaster.DELIMITER).First();

		}

	}

}
