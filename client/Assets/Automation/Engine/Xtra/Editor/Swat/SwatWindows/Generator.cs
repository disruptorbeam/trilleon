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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Reflection;

namespace TrilleonAutomation {

	public enum GeneratorSubTab {
		Assistant = 0,
		Recorder = 1
	}

	public class Generator : SwatWindow {

		const string ASSERT_ARROW_UP = "➦";
		const string ASSERT_ARROW_DOWN = "➥";

		Vector2 _scroll = new Vector2();
		public GeneratorSubTab SelectedSubTab { get; set; }

		public int SubTabCount = 2;

		public override void Set() {

			//Will we populate elements based on highlighted element in hierarchy window, or by dragging the desired object into our editor window.
			inspectBasedOnSelectedHierarchy = AutomationMaster.ConfigReader.GetBool("EDITOR_WINDOW_INSPECT_BY_HIERARCHY_HIGHLIGHTED");
		
		}

		public override bool UpdateWhenNotInFocus() {

			return true;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle TabOptions = new GUIStyle(GUI.skin.button);
			TabOptions.fixedWidth = (Nexus.Self.position.width / SubTabCount > 250 ? 250 : Nexus.Self.position.width / SubTabCount) + 10;
			TabOptions.fixedHeight = 35;
			TabOptions.fontSize = 16;
			TabOptions.alignment = TextAnchor.MiddleCenter;
			TabOptions.normal.background = Swat.TabButtonBackgroundTexture;
			TabOptions.normal.textColor = Color.white;

			EditorGUILayout.BeginHorizontal(); // ID 01
			GUILayout.Space(-5);

			if(SelectedSubTab == GeneratorSubTab.Assistant) {

				TabOptions.normal.background = Swat.WindowBackgroundTexture;
				TabOptions.normal.textColor = Swat.WindowDefaultTextColor;

			}

			if(GUILayout.Button("Assistant", TabOptions)) {

				SelectedSubTab = GeneratorSubTab.Assistant;

			}

			TabOptions.normal.background = Swat.TabButtonBackgroundTexture;
			TabOptions.normal.textColor = Color.white;
			GUILayout.Space(-5);

			if(SelectedSubTab == GeneratorSubTab.Recorder) {

				TabOptions.normal.background = Swat.WindowBackgroundTexture;
				TabOptions.normal.textColor = Swat.WindowDefaultTextColor;

			}

			if(GUILayout.Button("Recorder", TabOptions)) {

				EditorGUI.indentLevel++;
				AutomationRecorder.SelectionUpdatesHierarchy = true; //Default on.
				SelectedSubTab = GeneratorSubTab.Recorder;
				EditorGUI.indentLevel--;

			}
			EditorGUILayout.EndHorizontal(); // ID 01

			GUILayout.Space(10);

			GUIStyle horizontal = new GUIStyle(GUI.skin.box);
			horizontal.fixedHeight = Nexus.Self.position.height - 90;
			horizontal.fixedWidth = Nexus.Self.position.width - 20;
			horizontal.margin = new RectOffset(10, 0, 0, 10);
			horizontal.normal.background = Swat.WindowBackgroundTexture;

			EditorGUILayout.BeginVertical(horizontal); // ID 02
			_scroll = EditorGUILayout.BeginScrollView(_scroll);
			if(SelectedSubTab == GeneratorSubTab.Assistant) {

				RenderAssistant();

			} else if(SelectedSubTab == GeneratorSubTab.Recorder) {

				RenderRecorder();

			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical(); // ID 02

		}

		#region Recording Tool

		bool advancedSearch = false;
		bool setUpReferences = true;
		bool isCreatingTestObject = false;
		bool inspectBasedOnSelectedHierarchy = true;
		GameObject lastInspectedObject = null;

		Driver driver {
			get {
				if(_objectDriver == null) {
					_objectDriver = GameObject.Find(TestMonitorHelpers.NAME).GetComponent<Driver>();
				}
				return _objectDriver;
			}
		}
		Driver _objectDriver;

		//Used by horizontals where there are two displayed elements. Get current window width; divide in half, subtract 10 to give padding between the two elements.
		int HalfRowLengthElement {
			get {
				return (int)Nexus.Self.position.width / 2 - 10;
			}
		}

		Type thisType;
		string newTestObjectFilePath = string.Empty;
		string thisTestObject = string.Empty;
		List<GameObject> objectsWithThisName = new List<GameObject>();
		List<GameObject> _references = new List<GameObject>();
		List<MonoScript> _scripts = new List<MonoScript>();
		List<KeyValuePair<string,string>> _refParentScriptParameterName = new List<KeyValuePair<string,string>>();
		List<string> _displayedReferenceNames = new List<string>();
		List<Component> components = new List<Component>();
		GameObject inspectedObject = null;

		#region Enum Holders

		AutoObjectType lastType = AutoObjectType.None;
		By lastFindBy = By.Default;
		Condition lastCondition = Condition.Exists;
		AutoStepType lastStep = AutoStepType.None;
		AutoObjectAssert lastAssertObject = AutoObjectAssert.None;
		int selectedReferenceChoice = 0;

		#endregion 

		bool activateHierarchySelector = false;
		void RenderAssistant() {

			if(!EditorApplication.isPlaying) {

				activateHierarchySelector = false;

			}
			GUILayout.Space(10);
			Nexus.Self.ToggleButton(activateHierarchySelector, "Hierarchy Autoselect", "Automatically select GameObjects in the hierarchy editor window when selecting interactable objects in the game view window.", 
				new Nexus.SwatDelegate(delegate() {  
					if(EditorApplication.isPlaying) {

						activateHierarchySelector = AutomationRecorder.NotRecordingActions = AutomationRecorder.SelectionUpdatesHierarchy = !activateHierarchySelector;
						AutomationRecorder.StaticSelfComponent.Initialize();

					} else {

						SimpleAlert.Pop("The editor must be running to activate this feature.", null);

					}
				}));

			GUILayout.Space(20);

			if(Selection.activeGameObject != null) {

				GUIStyle button = new GUIStyle(GUI.skin.button);
				button.padding = new RectOffset(0, 0, 5, 5);
				button.fontSize = 12;
				button.fontStyle = FontStyle.Bold;

				GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				button.normal.textColor = Selection.activeGameObject.activeSelf ? Swat.ToggleButtonSelectedTextColor : Swat.ToggleButtonTextColor;
				button.normal.background = Selection.activeGameObject.activeSelf ? Swat.ToggleButtonBackgroundSelectedTexture : Swat.ToggleButtonBackgroundTexture;

				Nexus.Self.Button(Selection.activeGameObject.activeSelf ? "Hide" : "Show", "Show/Hide currently-selected game object in hierarchy window.", 
					new Nexus.SwatDelegate(delegate() {
						Selection.activeGameObject.SetActive(!Selection.activeGameObject.activeSelf);
					}), button, new GUILayoutOption[] { GUILayout.Width(120), GUILayout.Height(30) });

				button.normal.textColor = advancedSearch ? Swat.ToggleButtonSelectedTextColor : Swat.ToggleButtonTextColor;
				button.normal.background = advancedSearch ? Swat.ToggleButtonBackgroundSelectedTexture : Swat.ToggleButtonBackgroundTexture;

				Nexus.Self.Button(advancedSearch ? "Default Search" : "Full Search", "Search for references to selected objects in just the current parent-child hierarchy (default), or in all scene game objects (full).", 
					new Nexus.SwatDelegate(delegate() {                
						advancedSearch = !advancedSearch;
						setUpReferences = true;
					}), button, new GUILayoutOption[] { GUILayout.Width(120), GUILayout.Height(30) });

				GUILayout.EndHorizontal();

			}

			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.margin = new RectOffset(20,0,0,0);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if(inspectBasedOnSelectedHierarchy) {
				inspectedObject = Selection.activeGameObject;
				if(lastInspectedObject != inspectedObject) {
					NewObjectSelected();
					setUpReferences = true;
					return;
				}
				lastInspectedObject = inspectedObject;
			} else {
				inspectedObject = EditorGUILayout.ObjectField("Inspect Object ", inspectedObject, typeof(GameObject), true, new GUILayoutOption[] { GUILayout.Width(HalfRowLengthElement), GUILayout.MaxWidth(400) }) as GameObject;
			}

			if(inspectedObject != Nexus.Overseer.storedObjectLast) {
				ResetValues();
				breadCrumbTrails = new List<KeyValuePair<int,KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>>>();
				GetReferences();
				Nexus.Overseer.storedObjectLast = inspectedObject;
			}
			EditorGUILayout.BeginHorizontal(); // ID 03
			GUILayout.Space(10);

			GUIStyle launchCatLabel = new GUIStyle(GUI.skin.label);
			launchCatLabel.padding = new RectOffset(10, 0, 0, 0);
			launchCatLabel.normal.textColor = Swat.WindowDefaultTextColor;
			EditorGUILayout.LabelField("Master Step Type: ", launchCatLabel);
			Nexus.Overseer.stepMaster = (AutoStepMaster)Nexus.Self.DropDown(Nexus.Overseer.stepMaster, 0, 25, 200);
			EditorGUILayout.EndHorizontal(); // ID 03

			//SimpleWait selection.
			if(Nexus.Overseer.stepMaster == AutoStepMaster.SimpleWait) {

				Nexus.Overseer.waitTime = EditorGUILayout.FloatField("Wait Time (Seconds): ", Nexus.Overseer.waitTime);
				EditorGUILayout.Space();
				if(GUILayout.Button("Generate Code", buttonStyle, new GUILayoutOption[] { GUILayout.Width(100) })) {
					Nexus.Overseer.generate = true;
				}
				EditorGUILayout.Space();
				if(Nexus.Overseer.generate) {
					EditorGUILayout.LabelField("Test Step Code: ");
					Nexus.Overseer.testCodeGenerated = EditorGUILayout.TextArea(string.Format("{0}Q.driver.WaitRealTime({1}));", AutoCreateScript.START_COROUTINE, Nexus.Overseer.waitTime));
				}

			}

			//ObjectAction or AdvancedWait selection without GameObject to inspect.
			if(inspectedObject == null && (Nexus.Overseer.stepMaster == AutoStepMaster.ObjectStep || Nexus.Overseer.stepMaster == AutoStepMaster.WaitFor)) {

				EditorGUILayout.Space();
				GUIStyle s = new GUIStyle(GUI.skin.label);
				s.normal.textColor = Color.red;
				s.wordWrap = true;
				GUILayout.Label("Select a GameObject in the hierarchy window to view automation details.", s);

			}

			//ObjectAction selection with GameObject to inspect.
			if(inspectedObject && (Nexus.Overseer.stepMaster == AutoStepMaster.ObjectStep || Nexus.Overseer.stepMaster == AutoStepMaster.WaitFor)) {

				EditorGUILayout.Space();

				if(Nexus.Overseer.parentObject != null && inspectedObject != null) {

					Nexus.Overseer.showBasic = Nexus.Self.Foldout(Nexus.Overseer.showBasic, "Show Basic GameObject Details", true);
					EditorGUI.indentLevel++;

					if(Nexus.Overseer.showBasic) {

						EditorGUILayout.Space();

						#region Basic GameObject details

						EditorGUIUtility.labelWidth = 225;
						GUI.enabled = false;

						//If a generated object in the hierarchy window is selected in Play Mode, and then Play Mode is stopped, the inspected object will be null. Skip this if it is null.
						Nexus.Overseer.parentObject = EditorGUILayout.ObjectField("Top Parent Object ", Nexus.Overseer.parentObject, typeof(GameObject), true, new GUILayoutOption[] { GUILayout.Width(HalfRowLengthElement), GUILayout.MaxWidth(400) }) as GameObject;
						GUI.enabled = true;
						Nexus.Overseer.objectName = EditorGUILayout.TextField("Object Name: ", inspectedObject.name);
						Nexus.Overseer.countObjectsWithThisName = EditorGUILayout.TextField("# Active Objects w/ This Name: ", objectsWithThisName.Count.ToString());
						if(driver != null) {
							Nexus.Overseer.objectActiveness = EditorGUILayout.TextField("Is Active/Visible/Interactible: ", (driver.IsActiveVisibleAndInteractable(inspectedObject) ? "True" : "False"));
						}
						Nexus.Overseer.objectTag = EditorGUILayout.TextField("Object Tag: ", inspectedObject.tag);
						EditorGUIUtility.labelWidth = 0;

					}

					#endregion

					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal(); // ID 04
				GUILayout.Space(10);
				Nexus.Overseer.showAdvanced = Nexus.Self.Foldout(Nexus.Overseer.showAdvanced, "Show Advanced GameObject Details", true);
				EditorGUILayout.EndHorizontal(); // ID 04
				if(Nexus.Overseer.showAdvanced) {

					EditorGUILayout.Space();

					#region Text Details

					Nexus.Overseer.textContainsValue = EditorGUILayout.TextField("Text Value:", Nexus.Overseer.textVal);

					#endregion

					#region Component List

					if(!string.IsNullOrEmpty(thisTestObject)) {
						newTestObjectFilePath = TestMonitorHelpers.GetNewTestObjectPath(thisTestObject);
						if(!FileBroker.Exists(newTestObjectFilePath)) {
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("This Component does not have a Test Object associated with it. Create one now?");
							GUILayout.Space(5);
							if(GUILayout.Button("Create", buttonStyle, new GUILayoutOption[] { GUILayout.Width(100) })) {
								string fileContents = AutoCreateScript.GetFileContentsForNewTestObject(thisTestObject);
								string metaFile = AutoCreateScript.GetMetaFileNewTestObject();
								FileBroker.CreateFileAtPath(newTestObjectFilePath, fileContents);
								FileBroker.CreateFileAtPath(string.Format("{0}.meta", newTestObjectFilePath), metaFile);
								isCreatingTestObject = true;
							}
							EditorGUILayout.Space();
						}
					}

					if(thisType != null && TestMonitorHelpers.Helper.GetComponent(thisType) == null) {
						TestMonitorHelpers.Helper.AddComponent(thisType);               
					}

					if(isCreatingTestObject && thisType == null) {
						Type newType = Type.GetType(string.Format("TrilleonAutomation.{0}", thisTestObject));
						if(newType != null) {
							thisType = newType;
							TestMonitorHelpers.allTestObjects.Add(newType);
							TestMonitorHelpers.Helper.AddComponent(newType);
						}
					}

					//If we do not have a master Test Object from the inspected object, do not attempt this logic.
					if(!string.IsNullOrEmpty(thisTestObject)) {
						if(TestMonitorHelpers.allTestObjects.FindAll(x => x.Name.ToLower() == thisTestObject.ToLower()).Any()) {
							if(TestMonitorHelpers.Helper.GetComponent(thisType) != null) {
								MonoBehaviour thisComponent = TestMonitorHelpers.Helper.GetComponent(thisType) as MonoBehaviour;
								Nexus.Overseer.testObjectFile = MonoScript.FromMonoBehaviour(thisComponent);
								Nexus.Overseer.testObjectFile = EditorGUILayout.ObjectField("Test Object: ", Nexus.Overseer.testObjectFile, thisType, true, new GUILayoutOption[] { GUILayout.Width(HalfRowLengthElement), GUILayout.MaxWidth(400) }) as MonoScript;
								isCreatingTestObject = false;
								Nexus.Overseer.testObject = thisTestObject;
							} else {
								Nexus.Overseer.testObject = EditorGUILayout.TextField("Test Object: ", thisTestObject);
							}
						} else {
							if(isCreatingTestObject) {
								GUIStyle testObjectButton = new GUIStyle(GUI.skin.button);
								testObjectButton.fixedWidth = Nexus.DetermineRectWidthBasedOnLengthOfString(thisTestObject);
								testObjectButton.margin = new RectOffset(20, 0, 8, 8);
								if(GUILayout.Button(thisTestObject, testObjectButton)) {
									System.Diagnostics.Process.Start(TestMonitorHelpers.GetNewTestObjectPath(thisTestObject));
								}
								GUIStyle s = new GUIStyle(EditorStyles.boldLabel);
								s.normal.textColor = Swat.TextGreen;
								EditorGUILayout.LabelField("A monoscript reference will not appear until next compilation.", s);
								EditorGUILayout.Space();
							} else {
								Nexus.Overseer.testObject = EditorGUILayout.TextField("Test Object: ", thisTestObject);
							}
						}
					}

					#endregion

					if(inspectedObject.GetComponent<Button>() != null) {
						Nexus.Overseer.objectType = AutoObjectType.Button;
					} else if(inspectedObject.GetComponent<InputField>() != null) {
						Nexus.Overseer.objectType = AutoObjectType.InputField;
					} else if(!string.IsNullOrEmpty(Nexus.Overseer.textVal)) {
						Nexus.Overseer.objectType = AutoObjectType.Text;
					} else {
						Nexus.Overseer.objectType = AutoObjectType.GameObject;
					}

					Nexus.Overseer.objectType = (AutoObjectType)EditorGUILayout.EnumPopup("Object Type: ", Nexus.Overseer.objectType, new GUILayoutOption[] { GUILayout.Width(275) });
					if(Nexus.Overseer.objectType != lastType) {
						ResetValues();
						lastType = Nexus.Overseer.objectType;
					}

					#region Component List

					EditorGUILayout.Space();
					Nexus.Overseer.showComponents = Nexus.Self.Foldout(Nexus.Overseer.showComponents, "Show Components List", true);
					if(Nexus.Overseer.showComponents) {
						EditorGUI.indentLevel++;
						Nexus.Overseer.componentsList = new string[components.Count];
						for(int i = 0; i < components.Count; i++) {
							Nexus.Overseer.componentsList[i] = EditorGUILayout.TextField(string.Format("Component {0}:", i.ToString()), components[i].GetType().Name);
						}
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.Space();

					#endregion
					/*
               #region Master Script Fields & Properties List

               EditorGUILayout.Space();
               Nexus.Overseer.showFieldsAndProperties = Nexus.Self.Foldout(Nexus.Overseer.showFieldsAndProperties, "Master Script Fields & Properties", true);
               if(Nexus.Overseer.showFieldsAndProperties) {
                  EditorGUI.indentLevel++;
                  Nexus.Overseer.fieldsAndPropertiesList = new string[fieldsAndProperties.Count];
                  for(int i = 0; i < fieldsAndProperties.Count; i++) {
                     Nexus.Overseer.fieldsAndPropertiesList[i] = EditorGUILayout.TextField(string.Format("{0}", i.ToString()), fieldsAndProperties[i].Key);
                  }
                  EditorGUI.indentLevel--;
               }
               EditorGUILayout.Space();

               #endregion
               */
					#region Reference List

					GUIStyle labelRef = new GUIStyle(GUI.skin.label);
					labelRef.padding = new RectOffset(10, 0, 0, 0);
					labelRef.fontStyle = FontStyle.Bold;
					if(!_displayedReferenceNames.Any()){
						if(setUpReferences) {
							RenderReferences();
						}
						labelRef.normal.textColor = Color.red;
						EditorGUILayout.LabelField("- No references found -", labelRef);
					} else {
						EditorGUILayout.LabelField("- References -");
						RenderReferences();
						selectedReferenceChoice = EditorGUILayout.Popup("Use Which Ref?", selectedReferenceChoice, _displayedReferenceNames.ToArray());
					}
					EditorGUILayout.Space();

					#endregion

					//AdvancedWait selection.
					if(/* Temporarily disable else's code generation logic */ true || inspectedObject && Nexus.Overseer.stepMaster == AutoStepMaster.WaitFor) {

						labelRef.normal.textColor = Color.grey;
						labelRef.fontStyle = FontStyle.Italic;
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("Code Stub Generation Logic Is Temporarily Disabled.", labelRef);
						EditorGUILayout.LabelField("Refactoring And Updating In Progress.", labelRef);

					} else {

						//TODO: Update for WAIT FOR logic. EditorGUILayout.TextField();
						Nexus.Overseer.step = (AutoStepType)EditorGUILayout.EnumPopup("Step Type: ", Nexus.Overseer.step, new GUILayoutOption[] { GUILayout.Width(275) });
						if(Nexus.Overseer.step != lastStep) {
							ResetValues();
							lastStep = Nexus.Overseer.step;
						}
						switch(Nexus.Overseer.step) {
						case AutoStepType.Action:
							Nexus.Overseer.action = (AutoAction)EditorGUILayout.EnumPopup("Act On: ", Nexus.Overseer.action, new GUILayoutOption[] { GUILayout.Width(275) });
							break;
						case AutoStepType.ObjectAssert:
							Nexus.Overseer.assertObject = (AutoObjectAssert)EditorGUILayout.EnumPopup("Object Assert: ", Nexus.Overseer.assertObject, new GUILayoutOption[] { GUILayout.Width(275) });
							if(Nexus.Overseer.assertObject != lastAssertObject) {
								ResetValues();
								lastAssertObject = Nexus.Overseer.assertObject;
							}
							if(Nexus.Overseer.assertObject != AutoObjectAssert.None) {
								Nexus.Overseer.reverseCondition = EditorGUILayout.Toggle("Reverse Condition? ", Nexus.Overseer.reverseCondition);
							}
							break;
						case AutoStepType.BoolAssert:
							//action = (AutoAction)EditorGUILayout.EnumPopup("Act On: ", action);
							break;
						}

					}

					//We do not need to find the object programmatically if we have a reference to it.
					if(_references.Count == 0) {

						if(Nexus.Overseer.action != AutoAction.None) {
							Nexus.Overseer.findBy = (By)EditorGUILayout.EnumPopup("Find By: ", Nexus.Overseer.findBy, new GUILayoutOption[] { GUILayout.Width(275) }); 
							if(Nexus.Overseer.findBy != lastFindBy) {
								ResetValues();
								lastFindBy = Nexus.Overseer.findBy;
							}
						}

						if(Nexus.Overseer.findBy == By.TextContent) {
							Nexus.Overseer.textContainsToUse = EditorGUILayout.TextField("Search Val: ", Nexus.Overseer.textContainsValue.Length > 25 ? Nexus.Overseer.textContainsValue.Substring(0, 25) : Nexus.Overseer.textContainsValue);
						}

					}

					//Show controls for generated code.
					if(Nexus.Overseer.action != AutoAction.None || Nexus.Overseer.assertObject != AutoObjectAssert.None) {

						GUILayout.Space(5);
						if(GUILayout.Button("Generate Code", buttonStyle, new GUILayoutOption[] { GUILayout.Width(100) })) {
							if(_refParentScriptParameterName.Count > 1) {
								Nexus.Overseer.referencedScript = _refParentScriptParameterName[selectedReferenceChoice];
							} else if(_refParentScriptParameterName.Count == 1) {
								Nexus.Overseer.referencedScript = _refParentScriptParameterName[0];
							}
							Nexus.Overseer.testObjectCodeGenerated = AutoCreateScript.GenerateTestObjectCode();
							Nexus.Overseer.testCodeGenerated = AutoCreateScript.GenerateLineOfTestCode(Nexus.Overseer.textType);
							Nexus.Overseer.generate = true;
						}
						EditorGUILayout.Space();
						if(Nexus.Overseer.generate) {
							EditorGUILayout.LabelField("Test Object Code: ");
							Nexus.Overseer.testObjectCodeGenerated = EditorGUILayout.TextArea(Nexus.Overseer.testObjectCodeGenerated);
							EditorGUILayout.LabelField("Test Step Code: ");
							Nexus.Overseer.testCodeGenerated = EditorGUILayout.TextArea(Nexus.Overseer.testCodeGenerated);
						}

					}

				}

			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}

		List<KeyValuePair<FieldInfo,string>> thisSerializableList;
		List<KeyValuePair<int,KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>>> breadCrumbTrails = new List<KeyValuePair<int,KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>>>();

		void GetReferences() {

			/*
          * Add programmatic reference variable names as we travel from sub class fields to top level sub class references.
          * Each breadCrumbTrails item contains:
          * 1) a key which is simply the id of the breadcrumb trail
          * 2) a value which is a keyvaluepair:
          *       A) a key that is the MonoScript where a reference was found
          *       B) a value that is a list of keyvaluepairs:
          *             1) a key that is a string holding the field name of the reference
          *             2) a value that is a keyvaluepair: 
          *                   A) a key which is the GameObject that held the MonoScript where a reference was found
          *                   B) a key which is the Type of the field reference match
         */

			_references = new List<GameObject>();
			_scripts = new List<MonoScript>();
			_displayedReferenceNames = new List<string>();

			//Reset currently-selected script reference.
			Nexus.Overseer.referencedScript = new KeyValuePair<string, string>();

			//If we are doing an advances search for references (default is parent-based search only).
			List<GameObject> AllGameObjects = new List<GameObject>();
			if(advancedSearch) {
				AllGameObjects = SceneMaster.GetObjectPool();
			}

			//Get All Parents Of This Object
			if(inspectedObject != null) {

				Transform findReferencesInThisObjectTransform = inspectedObject.transform.parent;
				List<GameObject> objectsToLookForReferencesOf = new List<GameObject>();
				objectsToLookForReferencesOf.Add(inspectedObject);

				while(findReferencesInThisObjectTransform != null || (advancedSearch && AllGameObjects.Any())) {

					GameObject searchRefObj = findReferencesInThisObjectTransform.gameObject;
					List<KeyValuePair<Component,Type>> thisComponent = TestMonitorHelpers.FindMonoScriptReferences(searchRefObj);

					//For each attached script component on the object we are currently searching for references on.
					for(int tc = 0; tc < thisComponent.Count; tc++) {

						if(thisComponent[tc].Key != null) {

							List<FieldInfo> fieldInfos = thisComponent[tc].Value.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty).ToList();
							List<PropertyInfo> propertyInfos = thisComponent[tc].Value.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty).ToList();

							//Search all field infos for reference to the inspected object.
							for(int fi = 0; fi < fieldInfos.Count; fi++) {

								try {

									//Convert component into a System.Object so that we can use it to get values from FieldInfo class.
									System.Object obj = (System.Object)thisComponent[tc].Key;

									if(obj == null) {
										continue;
									}

									object value = fieldInfos[fi].GetValue(obj);

									if(value == null) {
										continue;
									}

									Type fieldReferenceType = value.GetType();
									string typeName = fieldReferenceType.Name;
									string fieldName = fieldInfos[fi].Name;
									string serializableSubClassFieldName = string.Empty;
									System.Object fieldValueSystemObject = fieldInfos[fi].GetValue(obj);
									UnityEngine.Object unityObjectField = fieldValueSystemObject as UnityEngine.Object;

									//Check if this instantiated object is a serializable subclass with a reference we are looking for.
									if(unityObjectField == null) {

										FieldInfo serializableSubClassFieldMatch = fieldValueSystemObject.GetType().GetFields().ToList().Find(x => { 
											System.Object sobj = x.GetValue(fieldValueSystemObject);
											if(sobj == null) {
												return false;
											}
											UnityEngine.Object uobj = sobj as UnityEngine.Object;
											if(uobj == null) {
												return false;
											}
											MonoBehaviour mono = uobj as MonoBehaviour;
											if(mono == null) {
												return false;
											}
											GameObject gobj = mono.gameObject;
											if(gobj == null) {
												return false;
											}
											return objectsToLookForReferencesOf.Contains(gobj);
										});

										if(serializableSubClassFieldMatch != null) {

											//Set values for reference under this serializable sub-class.
											fieldValueSystemObject = serializableSubClassFieldMatch.GetValue(fieldValueSystemObject);
											serializableSubClassFieldName = fieldName;
											fieldName = serializableSubClassFieldMatch.Name; 
											unityObjectField = fieldValueSystemObject as UnityEngine.Object;
											fieldReferenceType = value.GetType();

										} else {

											continue;

										}

									}

									GameObject go = null;
									switch(typeName.ToLower()) {
									case "button":
										Button b = (Button)unityObjectField;
										go = b.gameObject;
										break;
									case "text":
										Text t = (Text)unityObjectField;
										go = t.gameObject;
										break;
									case "gameobject":
										go = (GameObject)unityObjectField;
										break;
										//case "textmeshprougui":
										//TODO: Your Code Here! For Code Generation.
										//TextMeshProUGUI pro = (TextMeshProUGUI)unityObjectField;
										//go = pro.gameObject;
										//break;
									default:
										if(unityObjectField.GetType().BaseType != null) {
											MonoBehaviour mono = null;
											try {
												mono =(MonoBehaviour)unityObjectField;
											} catch{}
											if(mono == null) {
												Component c = (Component)unityObjectField;
												go = c.gameObject;
											} else {
												go = mono.gameObject;
											}
										}
										break;
									}

									List<GameObject> matches = objectsToLookForReferencesOf.FindAll(x => x == go);
									if(matches.Any()) {

										List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>> objectAndFieldReference = new List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>();
										MonoBehaviour behaviour = thisComponent[tc].Key as MonoBehaviour;
										MonoScript script = MonoScript.FromMonoBehaviour(behaviour);

										AttributeInfo thisAttributeInfo = new AttributeInfo(fieldInfos[fi]);
										string fieldNameToUse = string.IsNullOrEmpty(serializableSubClassFieldName) ? fieldName : string.Format("{0}.{1}", serializableSubClassFieldName, fieldName);

										//A private field in the breadcrumb trail will invalidate the breadcrumb unless A) We can find a property for it -OR- B) We simply alert the user that a property must be made to access the breadcrumb.
										if(fieldInfos[fi].IsPrivate) {

											//Note! Expects standards in naming conventions. Backing fields must use the same string name as the property. Only underscores "_" are expected/handled here for private backers.
											string propertyNameStandardExpected = fieldInfos[fi].Name.Replace("_", string.Empty);
											PropertyInfo match = propertyInfos.Find(x => x.Name.ToLower() == propertyNameStandardExpected.ToLower());
											if(match != null) {

												thisAttributeInfo = new AttributeInfo(match);
												fieldNameToUse = match.Name;

											}

										}

										if(!breadCrumbTrails.FindAll(x => matches.Contains(x.Value.Value.Last().Value.Key)).Any()) {

											//Create new trail if this is a reference to the top level object.
											int newId = breadCrumbTrails.Count;
											objectAndFieldReference.Add(new KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>(fieldNameToUse, new KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>(searchRefObj, new KeyValuePair<AttributeInfo,Type>(thisAttributeInfo, fieldReferenceType))));
											KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>> firstPairingEntry = new KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>(script, objectAndFieldReference);
											breadCrumbTrails.Add(new KeyValuePair<int,KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>>(newId, firstPairingEntry));
											objectsToLookForReferencesOf.Add(searchRefObj);

										} else {

											int breadCrumbToBeUpdated = breadCrumbTrails.FindIndex(x => matches.Contains(x.Value.Value.Last().Value.Key));
											objectAndFieldReference.AddRange(breadCrumbTrails[breadCrumbToBeUpdated].Value.Value); //Add older crumbs.
											objectAndFieldReference.Add(new KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>(fieldNameToUse, new KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>(searchRefObj, new KeyValuePair<AttributeInfo,Type>(thisAttributeInfo, fieldReferenceType)))); //Add newest crumb.

											//Only delete the original reference if the last object in the chain is not identical to the current parent object beeing checked for references.
											//If this condition is not met, then the inspected object is referenced in two or mroe scripts on the same parent object, and should both have references recorded separately.
											if(breadCrumbTrails[breadCrumbToBeUpdated].Value.Value.Last().Value.Key != searchRefObj) {
												breadCrumbTrails.RemoveAt(breadCrumbToBeUpdated);
											}

											KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>> pairingEntries = new KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>(script, objectAndFieldReference);
											int newId = breadCrumbTrails.Count;
											breadCrumbTrails.Add(new KeyValuePair<int,KeyValuePair<MonoScript,List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>>>>(newId, pairingEntries));

											objectsToLookForReferencesOf.Add(searchRefObj);

										}

									}
									//TODO: Serializable sub class.
									//Check if field is serializable, and check its sub-fields
									//List<FieldInfo> serializableFields = fieldReferenceType.GetFields().ToList();

								} catch(Exception e) {
									AutoConsole.PostMessage(e.Message);
								} 

							} 

						}

					}

					//Move to next parent object.
					if(advancedSearch) {

						AllGameObjects.Remove(searchRefObj);
						findReferencesInThisObjectTransform = AllGameObjects.Any() ? AllGameObjects.First().transform : null;
					} else {

						findReferencesInThisObjectTransform = searchRefObj.transform.parent;

					}

				}

			}

			if(!_references.Any()) {
				EditorGUILayout.LabelField("No references found.");
			}

		}

		struct AttributeInfo {

			public PropertyInfo Property { get; private set; }
			public FieldInfo Field { get; private set; }
			public bool IsProperty { get; private set; }
			public AttributeInfo(PropertyInfo obj) : this() {

				IsProperty = true;
				Property = obj;

			}
			public AttributeInfo(FieldInfo obj) : this() {

				Field = obj;

			}

		}

		List<KeyValuePair<string,List<List<string>>>> fullDetails = new List<KeyValuePair<string,List<List<string>>>>();
		List<List<string>> addativeDetails = new List<List<string>>();

		bool[] drillDowns;
		void RenderReferences() {

			bool expandErredBreadcrumbs = false; //Only set to true on intial "get" of breadcrumb trails.

			if(setUpReferences && breadCrumbTrails.Any()) {

				expandErredBreadcrumbs = true;
				setUpReferences = false;
				int drillDownCount = 0;

				for(int y = 0; y < breadCrumbTrails.Count; y++) {

					addativeDetails = new List<List<string>>();
					List<KeyValuePair<string,KeyValuePair<GameObject,KeyValuePair<AttributeInfo,Type>>>> pairings = breadCrumbTrails[y].Value.Value;
					pairings.Reverse(); 
					_references.Add(pairings.Last().Value.Key);
					_scripts.Add(breadCrumbTrails[y].Value.Key);

					StringBuilder displayName = new StringBuilder();
					StringBuilder programmaticName = new StringBuilder();
					for(int z = 0; z < pairings.Count; z++) {

						programmaticName.Append(pairings[z].Key);
						displayName.Append(pairings[z].Key);
						if(z != pairings.Count - 1) {
							displayName.Append(".");
							programmaticName.Append(".");
						}

						AttributeInfo field = pairings[z].Value.Value.Key;
						string accessability =!field.IsProperty && field.Field.IsPrivate || field.IsProperty && field.Property.GetAccessors(true).First().IsPrivate ? "Private (Needs Public Getter!)" : "Public";
						string oriented = !field.IsProperty && field.Field.IsStatic || field.IsProperty && field.Property.GetAccessors(true).First().IsStatic ? "Static" : "Instance";
						addativeDetails.Add(new List<string> { pairings[z].Value.Value.Value.Name, accessability, oriented });
						drillDownCount++;

					}

					fullDetails.Add(new KeyValuePair<string,List<List<string>>>(displayName.ToString(), addativeDetails));
					_displayedReferenceNames.Add(displayName.ToString());

					KeyValuePair<string,string> insert = new KeyValuePair<string,string>(pairings.Last().Value.Value.Value.Name, programmaticName.ToString());
					_refParentScriptParameterName.Add(insert);

				}

				drillDowns = new bool[drillDownCount];

			}

			GUIStyle breadcrumbMeta = new GUIStyle(GUI.skin.label);
			GUIStyle breadcrumbPiece = new GUIStyle(GUI.skin.label);
			breadcrumbPiece.fontStyle = FontStyle.Bold;
			GUIStyle breadcrumbTrail = new GUIStyle();
			GUIStyle breadcrumb = new GUIStyle();
			breadcrumbPiece.margin = breadcrumbMeta.margin = breadcrumbTrail.margin = new RectOffset(5, 0, 0, 0);

			Texture2D currentBg = breadcrumb.normal.background;

			for(int x = 0; x < breadCrumbTrails.Count; x++) {

				KeyValuePair<string,List<List<string>>> drillDownsForSingleTrail = fullDetails.Find(r => r.Key == _displayedReferenceNames[x]);
				List<string> crumbNamesIndividual = drillDownsForSingleTrail.Key.Split('.').ToList();

				EditorGUILayout.BeginVertical(breadcrumbTrail);
				Type refType = _references[x].GetType();
				Type scriptType = _scripts[x].GetType();

				_references[x] = EditorGUILayout.ObjectField("     Ref Obj: ", _references[x], refType, true, new GUILayoutOption[] {
					GUILayout.MaxWidth(400)
				}) as GameObject;
				_scripts[x] = EditorGUILayout.ObjectField("     Script: ", _scripts[x], scriptType, true, new GUILayoutOption[] {
					GUILayout.MaxWidth(400)
				}) as MonoScript;
				EditorGUILayout.TextField("     Name: ", _displayedReferenceNames[x]);
				GUILayout.Space(5);

				for(int dd = 0; dd < drillDownsForSingleTrail.Value.Count; dd++) {

					EditorGUI.indentLevel++;
					bool privateCrumb = drillDownsForSingleTrail.Value[dd][1].Contains("Private (Needs Public Getter!)");

					if(privateCrumb) {

						//Alert user that this breadcrumb trail requires implementation of public accessors to be valid.
						breadcrumb.normal.background = Swat.MakeTextureFromColor(new Color32(255, 0, 0, 50));
						if(expandErredBreadcrumbs) {

							drillDowns[dd] = true;

						}

					}

					EditorGUILayout.BeginVertical(breadcrumb);
					drillDowns[dd] = Nexus.Self.Foldout(drillDowns[dd], new GUIContent(string.Format("Drilldown [{0}]", crumbNamesIndividual[dd])), true);
					if(drillDowns[dd]) {

						EditorGUILayout.LabelField(string.Format("      <Type> {0}", drillDownsForSingleTrail.Value[dd][0]), breadcrumbMeta);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(string.Format("{0}   <Accessability> {1} ", privateCrumb ? "!!!" : "   ", drillDownsForSingleTrail.Value[dd][1]), breadcrumbMeta);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.LabelField(string.Format("      <Inst/Static> {0}", drillDownsForSingleTrail.Value[dd][2]), breadcrumbMeta);

					}
					breadcrumb.normal.background = currentBg;
					if(dd != drillDownsForSingleTrail.Value.Count - 1) {

						GUILayout.Space(2);
						EditorGUILayout.LabelField("                ↓  (Nested Variable Reference)  ↓", breadcrumbMeta);
						GUILayout.Space(2);

					}
					EditorGUILayout.EndVertical();
					EditorGUI.indentLevel--;

				}

				EditorGUILayout.EndVertical();
				expandErredBreadcrumbs = false;
				GUILayout.Space(10);

			}

		}

		void NewObjectSelected() {

			if(inspectedObject == null) {
				return;
			}

			isCreatingTestObject = false;
			_references = new List<GameObject>();
			_scripts = new List<MonoScript>();
			Nexus.Overseer.referencedScript = new KeyValuePair<string, string>();
			_refParentScriptParameterName = new List<KeyValuePair<string,string>>();
			_displayedReferenceNames = new List<string>();
			objectsWithThisName = Nexus.Overseer.parentObject.GetChildren().FindAll(x => x.name == Nexus.Overseer.objectName);

			//Get text component value if any.
			bool containsText = inspectedObject.GetComponent<Text>() != null;
			Nexus.Overseer.textVal = containsText ? inspectedObject.GetComponent<Text>().text : string.Empty;
			Nexus.Overseer.textType = containsText ? "Text" : string.Empty;
			if(!containsText) {
				for(int a = 0; a < GameMaster.AdditionalTextAssets.Count; a++) {
					if(inspectedObject.GetComponent(GameMaster.AdditionalTextAssets[a].Key) != null) {
						Type type = inspectedObject.GetComponent(GameMaster.AdditionalTextAssets[a].Key).GetType();
						if(type != null) {
							Nexus.Overseer.textVal = type.GetProperty(GameMaster.AdditionalTextAssets[a].Value).GetValue(inspectedObject.GetComponent(GameMaster.AdditionalTextAssets[a].Key), null).ToString();
							if(!string.IsNullOrEmpty(Nexus.Overseer.textVal)) {
								Nexus.Overseer.textType = type.Name;
								break;
							}
						}
					}
				}
			}

			//Get components of this object.
			thisTestObject = TestMonitorHelpers.SetParentComponentObject(inspectedObject);

			List<Type> thisTypeList = new List<Type>();
			List<Assembly> assembliesAll = AppDomain.CurrentDomain.GetAssemblies().ToList();
			for(int x = 0; x < assembliesAll.Count; x++) {
				thisTypeList.AddRange(assembliesAll[x].GetTypes().ToList().FindAll(z => z.Name.ToLower() == thisTestObject.ToLower()));
			}
			thisType = thisTypeList.First();
			components = inspectedObject.GetComponents<Component>().ToList();

		}

		void ResetValues() {

			Nexus.Overseer.testObjectCodeGenerated = string.Empty;
			Nexus.Overseer.testCodeGenerated = string.Empty;
			Nexus.Overseer.textContainsToUse = string.Empty;
			Nexus.Overseer.generate = false;
			Nexus.Overseer.reverseCondition = false;

		}

		#endregion

		#region Recording Tool

		const string ASSERTION_MESSAGE_PLACEHOLDER = "Assertion Message";
		public bool isRecording = false;
		public bool isPaused = false;
		bool _awaitingGen = false;
		bool _showingGen = false;
		bool _refreshed = false;
		Vector2 scroll = new Vector2();
		List<int> ShowAdditionalToolsIds = new List<int>();
		enum SelectActions { Click, ClickAndHold, TryClick, ClickAndDrag, WaitForCondition }
		List<SelectActions> selected;
		int recentlyReorderedIndex = -1;
		byte currentAlphaRefresh = 255;
		byte currentAlphaStep = 60;
		int editingId = -1;
		float maxWidth = 0;

		KeyValuePair<int,int> editingAssertionId = new KeyValuePair<int,int>(-1, -1);
		GUIStyle assertArrow, addWait, addWaitInput, assertTypeToggle, assertTypeEdit, buttonStyle, componentTypeToggle, editInput, s, step,
		stepAssert, stepDelete, stepDuplicate, stepEdit, stepScreenshot, stepWait, stepOptionsExpand, stepWrapper, textArea, upDownArrowButtons;

		void RenderRecorder() {

			assertArrow = new GUIStyle(GUI.skin.label);
			assertArrow.fontStyle = FontStyle.Bold;
			assertArrow.fixedHeight = 30;
			assertArrow.fontSize = 20;
			assertArrow.alignment = TextAnchor.MiddleCenter;

			addWait = new GUIStyle(GUI.skin.button);
			addWait.padding = new RectOffset(0, 0, 0, 4);
			addWait.margin = new RectOffset(0, 0, 2, 0);
			addWait.fixedHeight = addWait.fixedWidth = 24;
			addWait.fontSize = (int)addWait.fixedHeight;
			addWait.fontStyle = FontStyle.Bold;
			addWait.normal.background = Swat.ToggleButtonBackgroundSelectedTexture;
			addWait.normal.textColor = Swat.TextGreen;

			addWaitInput = new GUIStyle(GUI.skin.textField);
			addWaitInput.fixedHeight = 25;
			addWaitInput.alignment = TextAnchor.MiddleCenter;

			assertTypeToggle = new GUIStyle(GUI.skin.button);
			assertTypeToggle.normal.background = Swat.MakeTextureFromColor((Color)new Color32(225, 225, 225, 255));
			assertTypeToggle.margin = new RectOffset(0, 0, -5, 0);
			assertTypeToggle.fixedHeight = 29;
			assertTypeToggle.fixedWidth = 120;
			assertTypeToggle.fontSize = 14;

			assertTypeEdit = new GUIStyle(GUI.skin.button);
			assertTypeEdit.normal.textColor = Swat.ActionButtonTextColor;
			assertTypeEdit.normal.background = Swat.ActionButtonTexture;
			assertTypeEdit.margin = assertTypeToggle.margin;
			assertTypeEdit.fixedHeight = assertTypeToggle.fixedHeight;
			assertTypeEdit.fontSize = assertTypeToggle.fontSize;
			assertTypeEdit.focused.textColor = assertTypeEdit.normal.textColor;
			assertTypeEdit.focused.background = assertTypeEdit.normal.background;

			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.margin = new RectOffset(5, 0, 0, 0);
			buttonStyle.normal.background = Swat.ActionButtonTexture;
			buttonStyle.normal.textColor = Swat.ActionButtonTextColor;

			componentTypeToggle = new GUIStyle(GUI.skin.button);
			componentTypeToggle.normal.background = assertTypeToggle.normal.background;
			componentTypeToggle.margin = assertTypeToggle.margin;
			componentTypeToggle.fixedHeight = assertTypeToggle.fixedHeight;
			componentTypeToggle.fixedWidth = assertTypeToggle.fixedWidth;
			componentTypeToggle.fontSize = assertTypeToggle.fontSize;
			componentTypeToggle.alignment = TextAnchor.MiddleCenter;
			componentTypeToggle.focused.textColor = assertTypeToggle.normal.textColor;
			componentTypeToggle.focused.background = assertTypeToggle.normal.background;

			editInput = new GUIStyle(GUI.skin.textField);
			editInput.fixedHeight = 30;
			editInput.margin = new RectOffset(0, 0, -5, 0);
			editInput.alignment = TextAnchor.MiddleCenter;

			s = new GUIStyle(GUI.skin.label);
			s.wordWrap = true;
			s.padding = new RectOffset(5, 0, 0, 0);

			step = new GUIStyle(GUI.skin.label);
			step.fixedHeight = 24;
			step.fontSize = 14;
			step.padding = new RectOffset(0, 0, 4, 0);
			step.alignment = TextAnchor.MiddleLeft;

			stepDelete = new GUIStyle(GUI.skin.button);
			stepDelete.margin = new RectOffset(10, 0, 0, 0);
			stepDelete.normal.textColor = Color.red;
			stepDelete.fixedHeight = 30;
			stepDelete.fixedWidth = 30;
			stepDelete.fontSize = 24;
			stepDelete.normal.background = Swat.TabButtonBackgroundTexture;  

			stepAssert = new GUIStyle(GUI.skin.button);
			stepAssert.fixedHeight = stepDelete.fixedHeight;
			stepAssert.fixedWidth = 25;
			stepAssert.fontSize = 30;
			stepAssert.normal.background = stepDelete.normal.background;

			stepDuplicate = new GUIStyle(GUI.skin.button);
			stepDuplicate.padding = new RectOffset(0, 0, -5, 0);
			stepDuplicate.fixedHeight = stepDelete.fixedHeight;
			stepDuplicate.fixedWidth = stepAssert.fixedWidth;
			stepDuplicate.fontSize = 28;
			stepDuplicate.normal.background = stepDelete.normal.background;

			stepScreenshot = new GUIStyle(GUI.skin.button);
			stepScreenshot.fixedHeight = stepDelete.fixedHeight;
			stepScreenshot.fixedWidth = stepAssert.fixedWidth;
			stepScreenshot.fontSize = 20;
			stepScreenshot.normal.background = stepDelete.normal.background;

			stepWait = new GUIStyle(GUI.skin.button);
			stepWait.padding = new RectOffset(0, 0, -8, 0);
			stepWait.fixedHeight = stepDelete.fixedHeight;
			stepWait.fixedWidth = stepAssert.fixedWidth;
			stepWait.fontSize = 36;
			stepWait.normal.background = stepDelete.normal.background;

			stepEdit = new GUIStyle(GUI.skin.button);
			stepEdit.fixedHeight = stepDelete.fixedHeight;
			stepEdit.fixedWidth = stepAssert.fixedWidth;
			stepEdit.fontSize = 18;
			stepEdit.normal.background = stepDelete.normal.background;

			stepOptionsExpand = new GUIStyle(GUI.skin.button);
			stepOptionsExpand.fixedHeight = stepDelete.fixedHeight;
			stepOptionsExpand.fixedWidth = 15;
			stepOptionsExpand.normal.background = stepDelete.normal.background;
			stepOptionsExpand.padding = new RectOffset(-1, 0, 0, 0);

			textArea = new GUIStyle(GUI.skin.button);
			textArea.margin = new RectOffset(5, 10, 10, 10);
			textArea.padding = new RectOffset(10, 10, 10, 10);
			textArea.alignment = TextAnchor.MiddleLeft;
			textArea.wordWrap = true;

			upDownArrowButtons = new GUIStyle(GUI.skin.button);
			upDownArrowButtons.fixedHeight = stepDelete.fixedHeight / 2;
			upDownArrowButtons.fixedWidth = stepDelete.fixedWidth - 5;
			upDownArrowButtons.normal.background = stepDelete.normal.background;
			upDownArrowButtons.normal.textColor = Color.blue;

			scroll = EditorGUILayout.BeginScrollView(scroll);

			if(isRecording && !EditorApplication.isPlaying && !EditorApplication.isPaused) {

				isRecording = false;
				AutomationRecorder.StaticSelfComponent.Dismantle();

			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(isRecording ? "Recording" : "Not Recording", s);
			GUILayout.Space(5);

			if(isRecording) {

				GUIStyle stop = new GUIStyle(GUI.skin.button);;
				stop.margin = new RectOffset(5, 0, 0, 0);
				stop.normal.background = Swat.MakeTextureFromColor((Color)new Color32(125, 0, 0, 255));
				stop.normal.textColor = Color.white;
				stop.fontStyle = FontStyle.Bold;
				stop.fontSize = 15;

				Nexus.Self.Button("Stop", "Stop recording and generate a script based on actions taken.",
					new Nexus.SwatDelegate(delegate() {                
						_awaitingGen = true;
						StopRecording();           
					}), stop, new GUILayoutOption[] { GUILayout.Height(40), GUILayout.Width(60) });

				Nexus.Self.ToggleButton(isPaused, "Record", "Pause/Resume recording of actions.",
					new Nexus.SwatDelegate(delegate() {                
						PauseRecording();
						AutomationRecorder.NotRecordingActions = !AutomationRecorder.NotRecordingActions;
					}), 125, 90, null, new string[] { "Active", "Paused" }, true);

				Nexus.Self.ToggleButton(AutomationRecorder.SelectionUpdatesHierarchy, "Hierarchy Select", "Toggle automatic selection of interacted game objects in hierarchy window.", 
					new Nexus.SwatDelegate(delegate() {                
						AutomationRecorder.SelectionUpdatesHierarchy = !AutomationRecorder.SelectionUpdatesHierarchy;
					}), 160);

				Nexus.Self.ToggleButton(AutomationRecorder.PauseOnSelect, "Pause On Select", "Some objects are destroyed after executing their click events. If this is anticipated, and examining the object in the hierarchy window is desired, activate this option to pause the game immediately after highlighting GameObject.", 
					new Nexus.SwatDelegate(delegate() {                
						AutomationRecorder.PauseOnSelect = !AutomationRecorder.PauseOnSelect;
					}), 160);

				Nexus.Self.ToggleButton(AutomationRecorder.ActivateTextComponentSelection, "Activate Text Assert", "Allow selection of Text-type components to create assertions based on rendered text.", 
					new Nexus.SwatDelegate(delegate() {                
						AutomationRecorder.ActivateTextComponentSelection = !AutomationRecorder.ActivateTextComponentSelection;
					}), 160);

				EditorGUILayout.BeginHorizontal();

				Nexus.Self.Button("Refresh", "Forces a check for objects that should have listeners, but currently do not. This happens automaatically after a step is recorded, and also at set intervals. However, there may be a scenario where you interact with an object, and it takes over 1 second for the next object you intend to interact with to be created. This may cause the delay-created object to lack a listener until the next interval check.", 
					new Nexus.SwatDelegate(delegate() {                
						AutomationRecorder.Refresh();
						_refreshed = true;
					}), buttonStyle, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(110) });

				GUIStyle checkRefreshed = new GUIStyle(GUI.skin.label);
				checkRefreshed.fontSize = 25;
				checkRefreshed.margin = new RectOffset(0, 0, -4, 0);
				checkRefreshed.fixedWidth = checkRefreshed.fixedHeight = 35;
				if(_refreshed) {

					if((int)currentAlphaRefresh - 4 < 0) {

						currentAlphaRefresh = 60;
						_refreshed = false;

					} else {

						currentAlphaRefresh -= (byte)4;
						checkRefreshed.normal.textColor = (Color)new Color32(0, 100, 0, currentAlphaRefresh);
						Nexus.Self.Repaint();

					}
					EditorGUILayout.LabelField("✔", checkRefreshed);

				}
				EditorGUILayout.EndHorizontal();

			} else {

				if(_awaitingGen) {

					_awaitingGen = false;
					_showingGen = true;
					AutomationRecorder.GenerateFullCodeSnippet();

				} else {

					Nexus.Self.Button("Record", "Begin recording interactions with automation relevant game objects.",
						new Nexus.SwatDelegate(delegate() {                
							_showingGen = false;
							AutomationRecorder.NotRecordingActions = false;
							if(Application.isPlaying) {
								StartRecording();
								Nexus.Overseer.failedToBeginRecording = false;
							} else {
								Nexus.Overseer.failedToBeginRecording = true;
							}      
						}), buttonStyle, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(110) });

				}

				if(AutomationRecorder.RecordedActions.Count > 0) {

					EditorGUILayout.Space();

					Nexus.Self.Button("Clear", "Clear all recorded data.",
						new Nexus.SwatDelegate(delegate() {                
							_showingGen = false;
							_awaitingGen = false;
							AutomationRecorder.Clear();    
						}), buttonStyle, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(110) });

				}

			}

			if(!_showingGen && Application.isPlaying) {

				if(isRecording && !isPaused) {

					Nexus.Overseer.record = true;

				}

				GUILayout.Space(20);

				for(int x = 0; x < AutomationRecorder.RecordedActions.Count; x++) {

					RenderAssertionSteps(x, true);

					stepWrapper = new GUIStyle();
					stepWrapper.fixedHeight = stepDelete.fixedHeight;
					if(recentlyReorderedIndex == x) {

						if((int)currentAlphaStep - 1 < 0) {

							currentAlphaStep = 60;
							recentlyReorderedIndex = -1;

						} else {

							currentAlphaStep -= (byte)1;
							stepWrapper.normal.background = Swat.MakeTextureFromColor((Color)new Color32(0, 200, 0, currentAlphaStep));
							Nexus.Self.Repaint();

						}

					}

					string actionName = string.Empty;
					switch(AutomationRecorder.RecordedActions[x].Action) {
						case ActableTypes.Clickable:
							actionName = "SELECT";
							break;
						case ActableTypes.Input:
							actionName = "TYPE INTO";
							break;
						case ActableTypes.Scroll:
							actionName = "SCROLL";
							break;
						case ActableTypes.Draggable:
							actionName = "DRAG";
							break;
						case ActableTypes.KeyDown:
							actionName = "PRESS";
							break;
						case ActableTypes.Screenshot:
							actionName = "SCREENSHOT";
							break;
						case ActableTypes.Wait:
							actionName = "WAIT FOR";
							break;
					}

					if(AutomationRecorder.RecordedActions[x].Action != ActableTypes.Wait && AutomationRecorder.RecordedActions[x].IsTry) {

						actionName = string.Format("TRY {0}", actionName);

					}

					GUILayout.Space(5);

					EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.MaxWidth(200) }); // ID 06
					if(GUILayout.Button("X", stepDelete)) {

						AutomationRecorder.RecordedActions.RemoveAt(x);
						if(AutomationRecorder.RecordedActions.Count == 0) {

							EditorGUILayout.EndHorizontal(); // ID 06.1
							break;

						} else {

							x--;

						}

					}
					GUILayout.Space(-3);
					EditorGUILayout.BeginVertical(); // ID 07
					GUILayout.Space(-0.4f);
					upDownArrowButtons.fontSize = (stepDelete.fontSize / 2) + 3;
					if(x == 0) {

						upDownArrowButtons.normal.textColor = Color.grey;

					} else {

						upDownArrowButtons.normal.textColor = Color.blue;

					}
					if(GUILayout.Button(Swat.MOVEUP, upDownArrowButtons)) {

						if(x != 0) {

							recentlyReorderedIndex = AutomationRecorder.ReOrderAction(x, true);

						}

					}               
					GUILayout.Space(-3f);
					upDownArrowButtons.fontSize = stepDelete.fontSize / 2;
					if(x == AutomationRecorder.RecordedActions.Count - 1) {

						upDownArrowButtons.normal.textColor = Color.grey;

					} else {

						upDownArrowButtons.normal.textColor = Color.blue;

					}
					if(GUILayout.Button(Swat.MOVEDOWN, upDownArrowButtons)) {

						if(x != AutomationRecorder.RecordedActions.Count - 1) {

							recentlyReorderedIndex = AutomationRecorder.ReOrderAction(x, false);

						}

					} else {

						upDownArrowButtons.normal.textColor = Color.blue;

					}
					EditorGUILayout.EndVertical(); // ID 07

					GUILayout.Space(-3);

					EditorGUILayout.BeginVertical(); // ID 09
					GUILayout.Space(-0.3f);

					if(ShowAdditionalToolsIds.Contains(AutomationRecorder.RecordedActions[x].ID)) {

						stepOptionsExpand.fontSize = 24;
						EditorGUILayout.BeginHorizontal(); // ID 10
						Nexus.Self.Button("✠", "Add an assertion tied to this step.", 
							new Nexus.SwatDelegate(delegate() {
								//Create empty assertion step and open it immediately for edit.
								editingAssertionId = new KeyValuePair<int,int>(AutomationRecorder.RecordedActions[x].ID, AutomationRecorder.AddAssertionToActionId(x));
							}), stepAssert);
						GUILayout.Space(-3);
						Nexus.Self.Button("⌛", "Add an explicit wait after this step.", 
							new Nexus.SwatDelegate(delegate() {
								RecordedGameObjectData data = new RecordedGameObjectData();
								data.Duration = 1;
								data.Action = ActableTypes.Wait;
								AutomationRecorder.AddActionAtIndex(data, x + 1);
								editingId = AutomationRecorder.CurrentStepID - 1;
								ShowAdditionalToolsIds.Add(editingId);
							}), stepWait);
						GUILayout.Space(-3);
						Nexus.Self.Button("❒", "Add a screenshot after this step", 
							new Nexus.SwatDelegate(delegate() {
								RecordedGameObjectData data = new RecordedGameObjectData();
								data.Action = ActableTypes.Screenshot;
								data.Name = string.Empty;
								AutomationRecorder.AddActionAtIndex(data, x + 1);
								editingId = AutomationRecorder.CurrentStepID - 1;
								ShowAdditionalToolsIds.Add(editingId);
							}), stepScreenshot);
						GUILayout.Space(-3);
						Nexus.Self.Button("+", "Duplicate this action. New action is pasted as the next action.", 
							new Nexus.SwatDelegate(delegate() {
								recentlyReorderedIndex = x + 1;
								RecordedGameObjectData newAction = new RecordedGameObjectData();
								newAction.Action = AutomationRecorder.RecordedActions[x].Action;
								newAction.Components = AutomationRecorder.RecordedActions[x].Components;
								newAction.Duration = AutomationRecorder.RecordedActions[x].Duration;
								newAction.IsTry = AutomationRecorder.RecordedActions[x].IsTry;
								newAction.KeyDown = AutomationRecorder.RecordedActions[x].KeyDown;
								newAction.Name = AutomationRecorder.RecordedActions[x].Name;
								newAction.TopLevelParentName = AutomationRecorder.RecordedActions[x].TopLevelParentName;
								newAction.Tag = AutomationRecorder.RecordedActions[x].Tag;
								newAction.TextValue = AutomationRecorder.RecordedActions[x].TextValue;
								newAction.AsComponent = AutomationRecorder.RecordedActions[x].AsComponent;
								newAction.InitialScrollPosition = AutomationRecorder.RecordedActions[x].InitialScrollPosition;
								newAction.IsFloatingAssertion = AutomationRecorder.RecordedActions[x].IsFloatingAssertion;
								newAction.RandomStringAllowSpecialCharacters = AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters;
								newAction.RandomStringLength = AutomationRecorder.RecordedActions[x].RandomStringLength;
								newAction.ScrollDirection = AutomationRecorder.RecordedActions[x].ScrollDirection;
								newAction.ScrollDistance = AutomationRecorder.RecordedActions[x].ScrollDistance;
								AutomationRecorder.AddActionAtIndex(newAction, x + 1);
								editingId = AutomationRecorder.CurrentStepID - 1;

								for(int a = 0; a < AutomationRecorder.RecordedActions[x].Assertions.Count; a++) {

									RecordingAssertionData assertion = new RecordingAssertionData(0);
									assertion.AssertionBeforeStep = AutomationRecorder.RecordedActions[x].Assertions[a].AssertionBeforeStep;
									assertion.Type = AutomationRecorder.RecordedActions[x].Assertions[a].Type;
									assertion.AssertionArgument = AutomationRecorder.RecordedActions[x].Assertions[a].AssertionArgument;
									assertion.AssertionIsTrue = AutomationRecorder.RecordedActions[x].Assertions[a].AssertionIsTrue;
									assertion.AssertionMessage = AutomationRecorder.RecordedActions[x].Assertions[a].AssertionMessage;
									assertion.IsReverseCondition = AutomationRecorder.RecordedActions[x].Assertions[a].IsReverseCondition;
									assertion.FailureContext = AutomationRecorder.RecordedActions[x].Assertions[a].FailureContext;
									AutomationRecorder.AddAssertionToActionId(editingId, assertion);

								}

								ShowAdditionalToolsIds.Add(recentlyReorderedIndex);
							}), stepDuplicate);
						GUILayout.Space(-3);
						Nexus.Self.Button("=", "Edit this action's data.", 
							new Nexus.SwatDelegate(delegate() {
								if(editingId == AutomationRecorder.RecordedActions[x].ID) {
									editingId = -1;
								} else {
									editingId = AutomationRecorder.RecordedActions[x].ID;
								}
							}), stepEdit);
						GUILayout.Space(-3);
						Nexus.Self.Button("◀", "Hide additional options.", 
							new Nexus.SwatDelegate(delegate() {
								if(editingId == AutomationRecorder.RecordedActions[x].ID) editingId = -1;
								ShowAdditionalToolsIds.Remove(AutomationRecorder.RecordedActions[x].ID);
							}), stepOptionsExpand);
						EditorGUILayout.EndHorizontal(); // ID 10

					} else {

						stepOptionsExpand.fontSize = 12;
						Nexus.Self.Button("▶", "Show additional options.", 
							new Nexus.SwatDelegate(delegate() {
								ShowAdditionalToolsIds.Add(AutomationRecorder.RecordedActions[x].ID);
							}), stepOptionsExpand);

					}
					EditorGUILayout.EndVertical(); // ID 09

					if(AutomationRecorder.RecordedActions[x].ID != editingId) {

						//If this step is not currently being edited
						string stepText = string.Format(" {0}", AutomationRecorder.RecordedActions[x].Name);
						if(AutomationRecorder.RecordedActions[x].Action == ActableTypes.Input) {

							if(AutomationRecorder.RecordedActions[x].RandomStringLength > 0) {

								stepText += string.Format(" - Random{0}String ({1} characters long)", AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters ? " " : " Alphanumeric ",AutomationRecorder.RecordedActions[x].RandomStringLength);

							} else {

								stepText += string.Format(" - Text \"{0}\"", AutomationRecorder.RecordedActions[x].TextValue);

							}

						} else if(AutomationRecorder.RecordedActions[x].Action == ActableTypes.Wait) {

							stepText += string.Format(" {0} Seconds(s)", AutomationRecorder.RecordedActions[x].Duration.ToString());

						} else if(AutomationRecorder.RecordedActions[x].Action == ActableTypes.KeyDown) {

							stepText += string.Format(" For {0} Seconds(s)", AutomationRecorder.RecordedActions[x].Duration.ToString());

						} else if(AutomationRecorder.RecordedActions[x].Action == ActableTypes.Scroll) {

							stepText += string.Format(" DISTANCE ({0} %) DIRECTION ({1}) DURATION ({2} s) ", AutomationRecorder.RecordedActions[x].ScrollDistance.ToString(), 
								AutomationRecorder.RecordedActions[x].ScrollDirection == ScrollDirection.LeftOrUpToRightOrDown ? "From Left/Top To Right/Bottom" : "From Right/Bottom To Left/Top", AutomationRecorder.RecordedActions[x].Duration.ToString());

						}

						float thisWidth = Swat.DetermineRectWidthBasedOnLengthOfString(stepText + actionName) + 25f;
						maxWidth = maxWidth < thisWidth ? thisWidth : maxWidth;

						EditorGUILayout.BeginHorizontal(stepWrapper); // ID 08.A
						Nexus.Self.Button(actionName, string.Empty, 
							new Nexus.SwatDelegate(delegate() {

								if(AutomationRecorder.RecordedActions[x].Action != ActableTypes.Wait && AutomationRecorder.RecordedActions[x].Action != ActableTypes.Screenshot && AutomationRecorder.RecordedActions[x].Action != ActableTypes.Scroll) {

									AutomationRecorder.RecordedActions[x].IsTry = !AutomationRecorder.RecordedActions[x].IsTry;

								}

							}), assertTypeToggle);
						EditorGUILayout.LabelField(stepText, step, new GUILayoutOption[] { GUILayout.Width(maxWidth) });
						EditorGUILayout.EndHorizontal(); // ID 08.A

						//If Screenshot step, and no name is given after hiding the edit field, show error.
						if(AutomationRecorder.RecordedActions[x].Action == ActableTypes.Wait) {

							if(Event.current.type == EventType.Repaint && !string.IsNullOrEmpty(AutomationRecorder.RecordedActions[x].Name) && !Q.help.IsAlphaNumeric(AutomationRecorder.RecordedActions[x].Name, '_', '-')) {

								Nexus.Self.ErrorOccurred(string.Format("Take_Screenshot_StepId_{0}", AutomationRecorder.RecordedActions[x].ID), "Screenshot names may only contain alpha numeric characters, or underscores \"_\" and short dashes \"-\".", GUILayoutUtility.GetLastRect().position);

							} else {

								Nexus.Self.RemoveError(string.Format("Take_Screenshot_StepId_{0}", AutomationRecorder.RecordedActions[x].ID));

							}

						}

					} else {

						//Display this recorded action in editable mode.
						EditorGUILayout.BeginHorizontal(stepWrapper); // ID 08.B
						if(AutomationRecorder.RecordedActions[x].Components.Any()) {

							AutomationRecorder.RecordedActions[x].AsComponent = EditorGUILayout.Popup(AutomationRecorder.RecordedActions[x].AsComponent, AutomationRecorder.RecordedActions[x].Components.ToArray(), componentTypeToggle, new GUILayoutOption[] { GUILayout.Width(componentTypeToggle.fixedWidth), GUILayout.Height(componentTypeToggle.fixedHeight) });
							GUILayout.Space(1);

						}
						AutomationRecorder.RecordedActions[x].Action = (ActableTypes)EditorGUILayout.EnumPopup(AutomationRecorder.RecordedActions[x].Action, assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(assertTypeToggle.fixedWidth), GUILayout.Height(assertTypeToggle.fixedHeight) });

						switch(AutomationRecorder.RecordedActions[x].Action) {
						case ActableTypes.Wait:
							AutomationRecorder.RecordedActions[x].Duration = EditorGUILayout.DoubleField(AutomationRecorder.RecordedActions[x].Duration, editInput, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(assertTypeToggle.fixedHeight) });
							break;
						case ActableTypes.Screenshot:
							AutomationRecorder.RecordedActions[x].Name = EditorGUILayout.TextField(AutomationRecorder.RecordedActions[x].Name, editInput, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(assertTypeToggle.fixedHeight) });
							if(Event.current.type == EventType.Repaint && !string.IsNullOrEmpty(AutomationRecorder.RecordedActions[x].Name) && !Q.help.IsAlphaNumeric(AutomationRecorder.RecordedActions[x].Name, '_', '-')) {

								Nexus.Self.ErrorOccurred(string.Format("Screenshot_Step_{0}", x.ToString()), "Screenshot names may only contain alpha numeric characters, or underscores \"_\" and short dashes \"-\".", GUILayoutUtility.GetLastRect().position);

							} else {

								Nexus.Self.RemoveError(string.Format("Screenshot_Step_{0}", x.ToString()));

							}
							break;
						case ActableTypes.Input:
							AutomationRecorder.RecordedActions[x].Name = EditorGUILayout.TextField(AutomationRecorder.RecordedActions[x].Name, editInput, new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(assertTypeToggle.fixedHeight) });
							if(AutomationRecorder.RecordedActions[x].RandomStringLength > 0) {
								EditorGUILayout.LabelField("Random String Length ➜ ", new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(assertTypeToggle.fixedHeight) });
								AutomationRecorder.RecordedActions[x].RandomStringLength = EditorGUILayout.IntField(AutomationRecorder.RecordedActions[x].RandomStringLength, editInput, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(assertTypeToggle.fixedHeight) });
								Nexus.Self.Button(AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters ? "@$" : "A1", AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters ? "Only allow alphanumeric characters to be included in the random string." : "Allow special characters to be included in the random string.", 
									new Nexus.SwatDelegate(delegate() {
										AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters = !AutomationRecorder.RecordedActions[x].RandomStringAllowSpecialCharacters;
									}), editInput, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(assertTypeToggle.fixedHeight) });
								Nexus.Self.Button("X", "Return to explicitly typed string.", 
									new Nexus.SwatDelegate(delegate() {
										AutomationRecorder.RecordedActions[x].RandomStringLength = 0;
									}), assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(assertTypeToggle.fixedHeight) });
							} else {
								AutomationRecorder.RecordedActions[x].TextValue = EditorGUILayout.TextField(AutomationRecorder.RecordedActions[x].TextValue, editInput, new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(assertTypeToggle.fixedHeight) });
								Nexus.Self.Button("R#", "Use randomized alphanumeric string instead?", 
									new Nexus.SwatDelegate(delegate() {
										AutomationRecorder.RecordedActions[x].RandomStringLength = 8;
									}), assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(assertTypeToggle.fixedHeight) });
							}
							break;
						case ActableTypes.Scroll:
							AutomationRecorder.RecordedActions[x].Name = EditorGUILayout.TextField(AutomationRecorder.RecordedActions[x].Name, editInput, new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(assertTypeToggle.fixedHeight) });
							AutomationRecorder.RecordedActions[x].ScrollDistance = EditorGUILayout.FloatField(AutomationRecorder.RecordedActions[x].ScrollDistance, editInput, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(assertTypeToggle.fixedHeight) });
							AutomationRecorder.RecordedActions[x].ScrollDirection = (ScrollDirection)EditorGUILayout.EnumPopup(AutomationRecorder.RecordedActions[x].ScrollDirection, assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(225), GUILayout.Height(assertTypeToggle.fixedHeight) });
							AutomationRecorder.RecordedActions[x].Duration = EditorGUILayout.DoubleField(AutomationRecorder.RecordedActions[x].Duration, editInput, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(assertTypeToggle.fixedHeight) });
							break;
						default:
							AutomationRecorder.RecordedActions[x].Name = EditorGUILayout.TextField(AutomationRecorder.RecordedActions[x].Name, editInput, new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(assertTypeToggle.fixedHeight) });
							break;
						}

						EditorGUILayout.EndHorizontal(); // ID 08.B

					}

					EditorGUILayout.EndHorizontal(); // ID 06.1

					RenderAssertionSteps(x, false);

				}

			} else {

				if(_showingGen) {

					GUILayout.Space(15f);
					GUIStyle viewCode = new GUIStyle(GUI.skin.button);
					viewCode.normal.background = Swat.MakeTextureFromColor(Swat.TextGreen);
					viewCode.normal.textColor = Color.white;
					viewCode.fontSize = 14;
					viewCode.fontStyle = FontStyle.Bold;
					if(GUILayout.Button("View Code", viewCode, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(110) })) {

						System.Diagnostics.Process.Start(string.Format("{0}RecordedTestCodeGen.html", FileBroker.RESOURCES_DIRECTORY));

					}

				} else {

					EditorGUILayout.Space();

					if(Nexus.Overseer.failedToBeginRecording) {

						s.normal.textColor = Color.red;
						EditorGUILayout.LabelField("Cannot record without first pressing play button in the Editor.", s);

					} else {

						s.normal.textColor = Swat.TextGreen;
						EditorGUILayout.LabelField("Press play to enable recording.", s);

					}

					s.normal.textColor = Swat.WindowDefaultTextColor;

				}


			}

			EditorGUILayout.EndScrollView();


			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}

		void RenderAssertionSteps(int x, bool beforeStep) {

			List<RecordingAssertionData> Assertions = AutomationRecorder.RecordedActions[x].Assertions.FindAll(y => y.AssertionBeforeStep == (beforeStep ? true : false));

			//Render any assertion steps.
			if(Assertions.Any()) {

				for(int a = 0; a < Assertions.Count; a++) {

					GUILayout.Space(5);
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.MaxWidth(200) }); // ID 13
					GUILayout.Space(20);
					EditorGUILayout.LabelField(Assertions[a].AssertionBeforeStep ? ASSERT_ARROW_UP : ASSERT_ARROW_DOWN, assertArrow, new GUILayoutOption[] { GUILayout.MaxWidth(25) });
					bool breakOut = false;

					Nexus.Self.Button("X", "Delete this assertion.", 
						new Nexus.SwatDelegate(delegate() {
							AutomationRecorder.RecordedActions[x].Assertions.RemoveAt(a);
							if(AutomationRecorder.RecordedActions[x].Assertions.Count == 0) {
								EditorGUILayout.EndHorizontal(); // ID 06.1
								breakOut = true;
							} else {
								x--;
							}
						}), stepDelete);
					if(breakOut) {

						break;

					}
					GUILayout.Space(-3);
					EditorGUILayout.BeginVertical(); // ID 11
					GUILayout.Space(-0.3f);
					Nexus.Self.Button("=", "Edit this assertion's data.", 
						new Nexus.SwatDelegate(delegate() {
							if(editingAssertionId.Key == AutomationRecorder.RecordedActions[x].ID && editingAssertionId.Value == Assertions[a].ID) {
								editingAssertionId = new KeyValuePair<int,int>(-1, -1);
							} else {
								editingAssertionId = new KeyValuePair<int,int>(AutomationRecorder.RecordedActions[x].ID, Assertions[a].ID);
							}
						}), stepEdit);
					EditorGUILayout.EndVertical(); // ID 11
					GUILayout.Space(-2f);
					EditorGUILayout.BeginVertical(); // ID 16.B
					GUILayout.Space(-0.3f);
					Nexus.Self.Button(Assertions[a].AssertionBeforeStep ? "⇣" : "⇡", "Edit this assertion's data.", 
						new Nexus.SwatDelegate(delegate() {
							Assertions[a].AssertionBeforeStep = !Assertions[a].AssertionBeforeStep;
						}), stepEdit);
					EditorGUILayout.EndVertical(); // ID 16.B

					EditorGUILayout.BeginHorizontal(stepWrapper); // ID 12
					if(editingAssertionId.Key == AutomationRecorder.RecordedActions[x].ID && editingAssertionId.Value == Assertions[a].ID) {

						//User is currently editing this assertion step. Render editable fields.
						Assertions[a].Type = (AssertionType)EditorGUILayout.EnumPopup(Assertions[a].Type, assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(assertTypeToggle.fixedWidth), GUILayout.Height(assertTypeToggle.fixedHeight) });
						GUILayout.Space(1f);
						//If this is an `IsTrue` assertion, additional customization is available.
						if(Assertions[a].Type == AssertionType.IsTrue) {

							Assertions[a].AssertionIsTrue = (AssertionIsTrue)EditorGUILayout.EnumPopup(Assertions[a].AssertionIsTrue, assertTypeEdit, new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(assertTypeToggle.fixedHeight) });
							GUILayout.Space(1f);
							Assertions[a].AssertionArgument = EditorGUILayout.TextField(Assertions[a].AssertionArgument, editInput, new GUILayoutOption[] { GUILayout.Width(maxWidth) });

						}
						Assertions[a].AssertionMessage = Nexus.Self.TextField("Edit_Assertion", Assertions[a].AssertionMessage, ASSERTION_MESSAGE_PLACEHOLDER, editInput, new GUILayoutOption[] { GUILayout.Width(maxWidth), GUILayout.Height(assertTypeToggle.fixedHeight) });

					} else {

						EditorGUILayout.BeginHorizontal(); // ID 14
						GUILayout.Space(-1f);
						//Render static display of assertion step.
						EditorGUILayout.LabelField(Enum.GetName(typeof(AssertionType), Assertions[a].Type), assertTypeToggle, new GUILayoutOption[] { GUILayout.Width(assertTypeToggle.fixedWidth), GUILayout.Height(assertTypeToggle.fixedHeight) });
						EditorGUILayout.EndHorizontal(); // ID 14
						//If this is an `IsTrue` assertion, additional customization is available.
						if(Assertions[a].Type == AssertionType.IsTrue) {

							EditorGUILayout.LabelField(Enum.GetName(typeof(AssertionIsTrue), Assertions[a].AssertionIsTrue), assertTypeToggle);
							EditorGUILayout.LabelField(Assertions[a].AssertionArgument, step);

						}
						EditorGUILayout.LabelField(Assertions[a].AssertionMessage, step);


					}
					//TODO: CHECK FOR DUPLICATES AND ALERT THE USER.
					EditorGUILayout.EndHorizontal(); // ID 12
					EditorGUILayout.EndHorizontal(); // ID 13

				}

			}

		}

		void StartRecording() {

			TestMonitorHelpers.FullReset();
			AutomationRecorder.StaticSelfComponent.Initialize();
			isRecording = true;

		}

		void PauseRecording() {

			isPaused = !isPaused; //Pause/Unpause;

		}

		void StopRecording() {

			AutomationRecorder.StaticSelfComponent.Dismantle();
			isRecording = isPaused = false;
			Nexus.Overseer.isAwaitingCodeGen = true;

		}

		#endregion

	}

}
