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
		bool isEdit { get; set; }
		bool saveCurrent { get; set; }
		List<KeyValuePair<string,string>> buildingAdded = new List<KeyValuePair<string,string>>();
		string[] _requestedMethods;
		string[] _categories;

		public override void Set() {

			if(!AutomationMaster.AllMethodsInFramework.Any()) {

				AutomationMaster.SetAllMethodsInFramework();

			}

			string rawFavoritesData = FileBroker.GetNonUnityTextResource(AutomationMaster.UnitTestMode ? FileResource.FavoritesUnit : FileResource.Favorites);
			List<string> favoritesEachRaw = rawFavoritesData.Split(AutomationMaster.DELIMITER).ToList();
			FavoritesList = new List<KeyValuePair<string,List<KeyValuePair<bool,string>>>>();
			for(int f = 0; f < favoritesEachRaw.Count; f++) {

				//Invalid data
				if(!favoritesEachRaw[f].Trim().Contains(internalDelimiter)) {
					
					continue;

				}

				List<string> pieces = favoritesEachRaw[f].Split(new string[] { internalDelimiter }, StringSplitOptions.None).ToList();
				pieces = pieces.RemoveNullAndEmpty(); //Remove possible empties due to line breaks at end of file.
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
			FavoritesList = new List<KeyValuePair<string,List<KeyValuePair<bool,string>>>>();
			Set();

		}

		public override void Render() {

			GUIStyle add = new GUIStyle(GUI.skin.button);
			add.margin = new RectOffset(20, 0, -10, 0);
			add.fixedWidth = 50;
			add.fixedHeight = 30;
			add.normal.textColor = Swat.TabButtonTextColor;
			add.normal.background = Swat.TabButtonBackgroundTexture;

			GUIStyle addNew = new GUIStyle(GUI.skin.button);
			addNew.margin = new RectOffset(20, 0, 0, 0);
			addNew.fixedWidth = 75;
			addNew.fixedHeight = 25;
			addNew.fontSize = 14;
			addNew.normal.textColor = Swat.ActionButtonTextColor;

			if(makeNew) {

				addNew.normal.background = Swat.MakeTextureFromColor(Color.black);

			} else {
				
				addNew.normal.background = Swat.ActionButtonTexture;

			}

			GUIStyle deleteItem = new GUIStyle(GUI.skin.button);
			deleteItem.margin = new RectOffset(20, 0, -2, 0);
			deleteItem.fontSize = 16;
			deleteItem.normal.textColor = Color.red;
			deleteItem.normal.background = Swat.TabButtonBackgroundTexture;

			GUIStyle launchGroup = new GUIStyle(GUI.skin.button);
			launchGroup.margin = new RectOffset(-10, 2, -5, 0);
			launchGroup.fixedWidth = 25;
			launchGroup.fixedHeight = 25;
			launchGroup.normal.textColor = Swat.TabButtonTextColor;
			launchGroup.normal.background = Swat.TabButtonBackgroundTexture;

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
					isEdit = false;
					buildingAdded = new List<KeyValuePair<string,string>>();

				}), addNew);
			GUILayout.Space(30);

			if(makeNew) {

				EditorGUILayout.BeginHorizontal();
				Nexus.Self.Button("Add", "Add To List.", 
					new Nexus.SwatDelegate(delegate() {       
						
						if(!buildingAdded.FindAll(x => x.Key == _categories[SelectedCatIndex]).Any()) {
							
							buildingAdded.Add(new KeyValuePair<string,string>(string.Format("{0}{1}", _categories[SelectedCatIndex], classIndicator), string.Empty));

						}

					}), add);
				GUILayout.Space(-20);
				SelectedCatIndex = Nexus.Self.DropDown(SelectedCatIndex, _categories, 20, width: 280);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
				EditorGUILayout.BeginHorizontal();
				Nexus.Self.Button("Add", "Add To List.", 
					new Nexus.SwatDelegate(delegate() {   
						
						if(!buildingAdded.FindAll(x => x.Value == _requestedMethods[SelectedTestIndex]).Any()) {
							
							buildingAdded.Add(new KeyValuePair<string,string>(GetTestsClassName(_requestedMethods[SelectedTestIndex]), _requestedMethods[SelectedTestIndex]));

						}

					}), add);
				GUILayout.Space(-20);
				SelectedTestIndex = Nexus.Self.DropDown(SelectedTestIndex, _requestedMethods, 20, width: 280);
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(15);
				for(int b = 0; b < buildingAdded.Count; b++) {

					//If the next item is a test under this class, then don't render the classname as a header. We only want to render class names as a header in the saved, non-editable display.
					if(b + 1 < buildingAdded.Count ? buildingAdded[b].Value.Length == 0 && buildingAdded[b + 1].Value.Length > 0 && buildingAdded[b].Key.Replace("*", string.Empty) == buildingAdded[b + 1].Key : false) {

						continue;

					}
					EditorGUILayout.BeginHorizontal();
					bool deleted = false;
					Nexus.Self.Button("X", "Remove this item.", 
						new Nexus.SwatDelegate(delegate() {   

							//Check if the previous item is a class name and matches this test's class name. If so, then check if the next item does not share the same category name.
							//If both are true, then remove the previous item, as it is just a category header that no longer has any tests under it. Leave it if there are other tests after this one that require that category header.
							if((b - 1 >= 0 ? buildingAdded[b - 1].Value.Length == 0 && buildingAdded[b].Key == buildingAdded[b - 1].Key.Replace("*", string.Empty) : false) && (b + 1 < buildingAdded.Count ? buildingAdded[b + 1].Key != buildingAdded[b].Key : true)) {
								
								buildingAdded.RemoveAt(b - 1);
								b--;

							}
							buildingAdded.RemoveAt(b);
							b--;
							deleted = true;

						}), deleteItem, new GUILayoutOption[] { GUILayout.Width(20) });
					if(deleted) {
						
						EditorGUILayout.EndHorizontal();
						continue;

					}
					GUILayout.Space(-10);
					EditorGUILayout.LabelField(string.Format("{0}{1}", buildingAdded[b].Value.Length > 0 ? string.Format("({0}) ", buildingAdded[b].Key) : buildingAdded[b].Key, buildingAdded[b].Value.Length > 0 ? buildingAdded[b].Value : string.Empty), typeHeader);
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(2);

				}
				GUILayout.Space(15);

				EditorGUILayout.LabelField("Name:", typeHeader);
				newName = EditorGUILayout.TextField(newName, nameField, new GUILayoutOption[] { GUILayout.Width(150) });
				Nexus.Self.Button("Save", "Save Favorite.", 
					new Nexus.SwatDelegate(delegate() {
						
						if(newName.Trim().Length == 0) {

							SimpleAlert.Pop("A name is required to save this Favorites list.", null);

						} else if(buildingAdded.Count == 0) {

							SimpleAlert.Pop("At least one category or test needs to be added to this Favorites list to save.", null);

						} else if(FavoritesList.FindAll(c => c.Key == newName.Trim()).Any() && (isEdit ? FavoritesList[EditId].Key != newName.Trim() : true)) {

							SimpleAlert.Pop("There is already a Favorite with this name. Please choose a unique name.", null);

						} else {
							
							saveCurrent = true;
							makeNew = false;
							SaveNew();

						}

					}), addNew);
				GUILayout.Space(20);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.Space(20);

			}

			int foldoutIndex = 0;
			for(int f = 0; f < FavoritesList.Count; f++) {

				if(isEdit && EditId == f) {

					continue;

				}

				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(300) });
				launchGroup.fontSize = 16;
				EditorGUILayout.LabelField(FavoritesList[f].Key, favoriteHeader);
				Nexus.Self.Button("▶", "Launch this Favorite.", 
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

					}), launchGroup);
				launchGroup.fontSize = 22;
				Nexus.Self.Button("✎", "Edit this Favorite.", 
					new Nexus.SwatDelegate(delegate() { 
						
						makeNew = true;
						isEdit = true;
						newName = FavoritesList[f].Key;
						EditId = f;
						buildingAdded = new List<KeyValuePair<string,string>>();
						for(int l = 0; l < FavoritesList[f].Value.Count; l++) {

							KeyValuePair<string,string> newItem = new KeyValuePair<string,string>();
							if(FavoritesList[f].Value[l].Key) {
								
								newItem = new KeyValuePair<string,string>(string.Format("{0}{1}", FavoritesList[f].Value[l].Value, classIndicator), string.Empty);

							} else {
								
								newItem = new KeyValuePair<string,string>(GetTestsClassName(FavoritesList[f].Value[l].Value), FavoritesList[f].Value[l].Value);

							}

							//Avoid possible duplicates
							if(l + 1 == FavoritesList[f].Value.Count || (!buildingAdded.FindAll(b => b.Key == newItem.Key).Any() && !buildingAdded.FindAll(b => b.Value == newItem.Value).Any())) {

								buildingAdded.Add(newItem);

							}

						}

					}), launchGroup);
				launchGroup.fontSize = 16;
				Nexus.Self.Button("X", "Delete this Favorite.", 
					new Nexus.SwatDelegate(delegate() {    
						
						keyToDelete = FavoritesList[f].Key;
						SimpleAlert.Pop("Are you sure you want to delete this Favorites list?", new EditorDelegate(DeleteFavorite));

					}), launchGroup);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);

				for(int z = 0; z < FavoritesList[f].Value.Count; z++) {

					if((z + 1 == FavoritesList[f].Value.Count && FavoritesList[f].Value[z].Key) || (z + 1 < FavoritesList[f].Value.Count && FavoritesList[f].Value[z + 1].Key && FavoritesList[f].Value[z].Key)) {

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

			StringBuilder sb = new StringBuilder();
			if(FavoritesList.Count > 0) {
				
				sb.Append(AutomationMaster.DELIMITER);

			}

			if(isEdit) {

				keyToDelete = FavoritesList[EditId].Key;
				DeleteFavorite();
				isEdit = false;

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
			FileBroker.SaveNonUnityTextResource(AutomationMaster.UnitTestMode ? FileResource.FavoritesUnit : FileResource.Favorites, sb.ToString(), false);
			newName = string.Empty;
			Set();
			Nexus.Self.Tests.Update_Data = true;

		}

		//Take raw file data and simply splice out substring that represents this Favorite's data.
		void DeleteFavorite() {

			string rawFavoritesData = FileBroker.GetNonUnityTextResource(AutomationMaster.UnitTestMode ? FileResource.FavoritesUnit : FileResource.Favorites);
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
			FileBroker.SaveNonUnityTextResource(AutomationMaster.UnitTestMode ? FileResource.FavoritesUnit : FileResource.Favorites, newData.ToString(), true);
			Set();
			Nexus.Self.Tests.Update_Data = true;

		}

		string GetTestsClassName(string test) {

			return AutomationMaster.AllMethodsInFramework.Find(x => x.Key.Split(AutomationMaster.DELIMITER).Last() == test).Key.Split(AutomationMaster.DELIMITER).First();

		}

	}

}
