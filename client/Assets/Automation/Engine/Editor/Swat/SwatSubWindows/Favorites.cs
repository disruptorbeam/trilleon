using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace TrilleonAutomation {

	public class Favorites : SwatWindow {

		int EditId { get; set; }
		int SelectedCatIndex { get; set; }
		int SelectedTestIndex { get; set; }
		bool MakeNew { get; set; }
		List<KeyValuePair<string,List<KeyValuePair<bool,string>>>> _favorites = new List<KeyValuePair<string,List<KeyValuePair<bool,string>>>>();
		string[] _requestedMethods;
		string[] _categories;

		public override void Set() {

			//string rawFavoritesData = FileBroker.GetNonUnityTextResource(FileResource.Favorites);
			//List<string> favoritesEachRaw = rawFavoritesData.Split("").ToString();

			_favorites.Add(new KeyValuePair<string,List<KeyValuePair<bool,string>>>("SAVE 1",new List<KeyValuePair<bool,string>> {
				new KeyValuePair<bool,string>(true, "NPE"),
				new KeyValuePair<bool,string>(false, "Arena_CommanderDivision_Battle_CanComplete"),
				new KeyValuePair<bool,string>(true, "Smoketest"),
				new KeyValuePair<bool,string>(false, "Arena_AdditionalTickets_CanPurchase"),
				new KeyValuePair<bool,string>(false, "Arena_CommanderDivision_ShipAndCrew_CanSet")
			}));

			if(!AutomationMaster.AllMethodsInFramework.Any()) {

				AutomationMaster.SetAllMethodsInFramework();

			}
			_requestedMethods = AutomationMaster.AllMethodsInFramework.ExtractListOfKeysFromKeyValList(true, 1).ToArray();
			_categories = Nexus.Self.Tests.CategoryTests.ExtractListOfKeysFromKeyValList().ToArray();

		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle addNew = new GUIStyle(GUI.skin.button);
			addNew.margin = new RectOffset(10, 0, 0, 0);
			addNew.fixedWidth = 75;
			addNew.fixedHeight = 35;
			addNew.normal.textColor = Swat.ToggleButtonSelectedTextColor;
			addNew.normal.background = Swat.ToggleButtonBackgroundSelectedTexture;

			GUIStyle edit = new GUIStyle(GUI.skin.button);
			edit.fixedWidth = 50;
			edit.fixedHeight = 25;
			edit.normal.textColor = Swat.ActionButtonTextColor;
			edit.normal.background = Swat.ActionButtonTexture;

			GUIStyle favoriteHeader = new GUIStyle(GUI.skin.label);
			favoriteHeader.padding = new RectOffset(10, 0, 0, 0);
			favoriteHeader.fontStyle = FontStyle.Bold;
			favoriteHeader.alignment = TextAnchor.LowerLeft;
			favoriteHeader.fontSize = 14;

			GUIStyle name = new GUIStyle(GUI.skin.label);
			name.padding = new RectOffset(25, 0, 0, 0);

			GUIStyle typeHeader = new GUIStyle(GUI.skin.label);
			typeHeader.padding = new RectOffset(20, 0, 0, 0);
			typeHeader.fontStyle = FontStyle.Italic;
			favoriteHeader.fontSize = 12;

			GUILayout.Space(25);
			Nexus.Self.Button("Add New", "Add new Favorite test run.", 
				new Nexus.SwatDelegate(delegate() {                
					MakeNew = true;
				}), addNew);
			GUILayout.Space(15);

			for(int f = 0; f < _favorites.Count; f++) {

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(_favorites[f].Key, favoriteHeader);
				Nexus.Self.Button("Edit", "Edit this Favorite.", 
					new Nexus.SwatDelegate(delegate() {                
						//TODO:
					}), edit, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(10);

				List<KeyValuePair<bool,string>> categories = _favorites[f].Value.FindAll(x => x.Key);
				EditorGUILayout.LabelField("Categories", typeHeader);
				for(int c = 0; c < categories.Count; c ++) {

					EditorGUILayout.LabelField(categories[c].Value, name);
					GUILayout.Space(2);

				}

				List<KeyValuePair<bool,string>> tests = _favorites[f].Value.FindAll(x => !x.Key);
				EditorGUILayout.LabelField("Tests", typeHeader);
				for(int t = 0; t < tests.Count; t ++) {

					EditorGUILayout.LabelField(tests[t].Value, name);
					GUILayout.Space(2);

				}
				GUILayout.Space(20);

			}

			if(MakeNew) {

				if(EditId == SelectedCatIndex || EditId == SelectedTestIndex) {

					SelectedCatIndex = Nexus.Self.DropDown(SelectedCatIndex, _categories);
					SelectedTestIndex = Nexus.Self.DropDown(SelectedTestIndex, _requestedMethods);

				}

				GUILayout.Space(25);
				Nexus.Self.Button("Save", "Save Favorite.", 
					new Nexus.SwatDelegate(delegate() {                
						MakeNew = false;
					}), addNew);

			}

		}

	}

}