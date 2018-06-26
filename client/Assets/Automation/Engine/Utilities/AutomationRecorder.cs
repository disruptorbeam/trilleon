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
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	public enum ActableTypes { Clickable, Draggable, Input, KeyDown, Scroll, Screenshot, TextForAssert, Wait }

	#if UNITY_EDITOR

	/// <summary>
	/// Enables interactable objects to be assigned a listener that captures and records activity in the game.
	/// NOTE: Gameobjects stripped from a destroyed scene and existing solely int DontDestroyOnLoad cannot be assigned listeners, and will not be interactable using Record & Playback.
	/// </summary>
	public class AutomationRecorder : MonoBehaviour {

		const float UPDATE_COOLDOWN = 5f;
		const string FIND_IN_OBJECT_PARENT_NAME = "findInObjectParent";
		const int MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN = 2; //Used to drill down in generic FIND code generated on objects, to reduce the chance of finding multiple objects with the same name.

		public static GameObject StaticSelf {
			get { 
				return AutomationMaster.StaticSelf;
			}
		}

		public static AutomationRecorder StaticSelfComponent {
			get { 
				if(_staticSelfComponent == null) {
					if(StaticSelf != null) {
						if(StaticSelf.GetComponent<AutomationRecorder>() ==null) {
							_staticSelfComponent = StaticSelf.AddComponent<AutomationRecorder>();
						} else {
							_staticSelfComponent = StaticSelf.GetComponent<AutomationRecorder>();
						}
					}
				}
				return _staticSelfComponent;
			}
			private set { 
				_staticSelfComponent = value;
			}
		}
		private static AutomationRecorder _staticSelfComponent;

		public static List<AutomationListener> ActiveListeners {
			get { 
				return _activeListeners;
			}
			private set { 
				_activeListeners = value;
			}
		}
		private static List<AutomationListener> _activeListeners = new List<AutomationListener>();

		public static List<KeyValuePair<Type, ActableTypes>> GameSpecificActableTypes {
			get{ 
				return GameMaster.AdditionalAssetsAll;
			}
		}

		public static List<RecordedGameObjectData> RecordedActions {
			get {
				return _recordedActions;
			}
			private set {
				_recordedActions = value;
			}
		}
		static List<RecordedGameObjectData> _recordedActions = new List<RecordedGameObjectData>();

		public static bool SelectionUpdatesHierarchy { get; set; }
		public static bool NotRecordingActions { get; set; }
		public static bool PauseOnSelect { get; set; }
		public static bool ActivateTextComponentSelection { get; set; }

		static List<GameObject> all = new List<GameObject>();
		static bool forceRefresh = false;
		static float lastRunTime = 0f;
		public static int CurrentStepID { get; private set; }
		public static int CurrentAssertionID { get; private set; }

		public static void AddAction(RecordedGameObjectData data) {

			data.ID = CurrentStepID++;
			RecordedActions.Add(data);

		}

		//Create empty assertion and return new ID.
		public static int AddAssertionToActionId(int id) {

			RecordedActions[id].Assertions.Add(new RecordingAssertionData(CurrentAssertionID++));
			return CurrentAssertionID - 1;

		}

		//Add existing assertion to step.
		public static void AddAssertionToActionId(int id, RecordingAssertionData assertion) {

			assertion.ID = CurrentAssertionID++;
			RecordedActions.Find(x => x.ID == id).Assertions.Add(assertion);

		}

		public static void AddActionAtIndex(RecordedGameObjectData data , int index) {

			data.ID = CurrentStepID++;
			RecordedActions = RecordedActions.AddAt(index, data);

		}

		public static void RemoveActionAt(int index) {

			RecordedActions.RemoveAt(index);

		}

		public static void Clear() {

			RecordedActions = new List<RecordedGameObjectData>();

		}

		/// <summary>
		/// Add Automation listener to all objects.
		/// </summary>
		public void Initialize() {

			if(AutomationMaster.StaticSelf.GetComponent<AutomationRecorder>() == null) {

				StaticSelfComponent = AutomationMaster.StaticSelf.AddComponent<AutomationRecorder>();

			}
			StartCoroutine(Runner());

		}

		public static void Refresh() {

			forceRefresh = true;

		}

		public IEnumerator DelayedRefresh() {

			yield return StartCoroutine(Q.driver.WaitRealTime(0.25f));
			Refresh();
			yield break;

		}

		void Update() {

			if(Input.GetMouseButtonDown(0)) {

				StartCoroutine(DelayedRefresh());

			}

		}

		IEnumerator Runner() {

			while(true) {

				if(forceRefresh || Time.time - lastRunTime > UPDATE_COOLDOWN) {

					forceRefresh = false;
					all = all.GetUniqueObjectsBetween(SceneMaster.GetObjectPool().GetActiveAndVisibleObjectsInList());

					for(int x = 0; x < all.Count; x++) {

						if(all[x] == null) {

							continue;

						}

						AutomationListener al = all[x].GetComponent<AutomationListener>();
						if(al == null) {

							List<MonoBehaviour> components = all[x].GetComponents<MonoBehaviour>().ToList();

							for(int co = 0; co < components.Count; co++) {

								string scriptName = components[co].GetType().Name;

								#region Clickables
								if(scriptName == "Button" || scriptName == "Toggle" || components[co].GetType().IsAssignableFrom(typeof(Button)) || components[co].GetType().IsAssignableFrom(typeof(Toggle))) {

									AddListener(all[x], ActableTypes.Clickable);
									continue;

								}

								if(scriptName == "Collider" || components[co].GetType().IsAssignableFrom(typeof(Collider))) {

									if(all[x].GetComponent<Collider>().isTrigger) {

										AddListener(all[x], ActableTypes.Clickable);
										continue;

									}

								}
								#endregion

								#region Inputs
								if(scriptName == "InputField" || components[co].GetType().IsAssignableFrom(typeof(InputField))) {

									AddListener(all[x], ActableTypes.Input);
									continue;

								}
								#endregion

								#region Movables
								if(scriptName == "ScrollRect" || components[co].GetType().IsAssignableFrom(typeof(ScrollRect))) {

									AddListener(all[x], ActableTypes.Scroll);
									continue;

								}
								#endregion

								#region Fall Backs
								for(int t = 0; t < GameSpecificActableTypes.Count; t++) {

									if(scriptName == GameSpecificActableTypes[t].Key.Name) {

										AddListener(all[x], GameSpecificActableTypes[t].Value);
										continue;

									}

								}

								for(int a = 0; a < GameMaster.AdditionalAssetsAll.Count; a++) {

									if(scriptName == GameMaster.AdditionalAssetsAll[a].Key.Name  || components[co].GetType().IsAssignableFrom(GameMaster.AdditionalAssetsAll[a].Key)) {

										AddListener(all[x], GameMaster.AdditionalAssetsAll[a].Value);
										continue;

									}

								}

								//Fallback on Selectable as many interactive objects and extensions of them inheret from Selectable.
								if(scriptName == "Selectable" || components[co].GetType().IsAssignableFrom(typeof(Selectable))) {

									AddListener(all[x], ActableTypes.Clickable);
									continue;

								}
								#endregion

								/*
			                     for(int t = 0; t < GameMaster.AdditionalTextAssets.Count; t++) {

			                        if(all[x].GetComponent(GameMaster.AdditionalTextAssets[t]) != <span class='value'>null</span>) {

			                           al = all[x].AddComponent<AutomationListener>();
			                           al.type =  ActableTypes.TextForAssert;
			                           ActiveListeners.Add(al);
			                           continue;

			                        }

			                     }
			                    */

							}

						}

					}

					lastRunTime = Time.time;

				}

				yield return StartCoroutine(Q.driver.WaitRealTime(0.05f));

			}

		}

		public void AddListener(GameObject obj, ActableTypes type) {

			if(obj.GetComponent<AutomationListener>() == null) {

				AutomationListener listener = obj.AddComponent<AutomationListener> ();
				listener.type = type;
				ActiveListeners.Add(listener);

			}

		}

		public void Dismantle() {

			StopCoroutine("Runner");

			List<GameObject> all = SceneMaster.GetObjectPool();
			for(int x = 0; x < all.Count; x++) {

				AutomationListener al = all[x].GetComponent<AutomationListener>();
				if(al != null) {

					Destroy(al);

				}

			}

		}

		/// <summary>
		/// Remove Automation listener from all objects.
		/// </summary>
		public static void AutomationRelevantActionTaken(AutomationListener listener) {

			List<Component> components = listener.gameObject.GetComponents<Component>().ToList();

			//Ignore if no components.
			if(!components.Any()) {
				return;
			}

			RecordedGameObjectData data = new RecordedGameObjectData();;
			data.Name = listener.gameObject.name.Replace("(Clone)", string.Empty);
			KeyValuePair<string,List<string>> parents = GetTopLevelParentObject(listener.gameObject);
			data.ParentNames = parents.Value;
			data.TopLevelParentName = parents.Key;
			data.Tag = listener.gameObject.tag;
			data.Components.Add("GameObject");
			data.Components.AddRange(listener.gameObject.GetComponents<Component>().ToList().ToListOfNames());
			data.AsComponent = data.Components.FindIndexOf("GameObject");

			List<KeyValuePair<Type,ActableTypes>> matches = GameSpecificActableTypes.FindAll(x => data.Components.Contains(x.Key.Name));

			Button b = listener.gameObject.GetComponent<Button>();
			Toggle t = listener.gameObject.GetComponent<Toggle>();
			if(b != null || t != null || matches.FindAll(x => x.Value == ActableTypes.Clickable).Any()) {

				if(b != null) {

					data.AsComponent = data.Components.FindIndexOf("Button");

				} else if(t != null) {

					data.AsComponent = data.Components.FindIndexOf("Toggle");

				} else {

					data.AsComponent = data.Components.FindIndexOf(matches.Find(x => x.Value == ActableTypes.Clickable).Key.Name);

				}
				data.ID = CurrentStepID++;
				data.Action = ActableTypes.Clickable;
				RecordedActions.Add(data);
				return;

			}

			if(components.FindAll(z => z.GetType().Name.ToLower().ContainsOrEquals("collider")).Any()) {

				data.ID = CurrentStepID++;
				data.Action = ActableTypes.Clickable;
				RecordedActions.Add(data);
				return;

			}

			ScrollRect sr = listener.gameObject.GetComponent<ScrollRect>();
			if(sr != null || matches.FindAll(x => x.Value == ActableTypes.Scroll).Any()) {

				if(sr != null) {

					data.AsComponent = data.Components.FindAll(x => x == "ScrollRect").Any() ? data.Components.FindIndexOf("ScrollRect") : data.Components.FindIndexOf("ScrollRectEx");

				} else {

					data.AsComponent = data.Components.FindIndexOf(matches.Find(x => x.Value == ActableTypes.Scroll).Key.Name);

				} 

				data.ID = CurrentStepID++;
				data.Action = ActableTypes.Scroll;
				data.InitialScrollPosition = sr.verticalScrollbar == null ? (sr.horizontalScrollbar == null ? 0 : sr.horizontalScrollbar.value) : sr.verticalScrollbar.value;
				data.Duration = 1;
				RecordedActions.Add(data);
				return;

			}

			InputField i = listener.gameObject.GetComponent<InputField>();
			if(i != null || matches.FindAll(x => x.Value == ActableTypes.Input).Any()) {

				data.AsComponent = data.Components.FindIndexOf("InputField");
				data.ID = CurrentStepID++;
				data.Action = ActableTypes.Input;
				RecordedActions.Add(data);
				return;

			}

			for(int a = 0; a < GameMaster.AdditionalAssetsAll.Count; a++) {

				Component c = listener.gameObject.GetComponent(GameMaster.AdditionalAssetsAll[a].Key.Name);
				if(c != null || matches.FindAll(x => x.Value == GameSpecificActableTypes[a].Value).Any()) {

					data.AsComponent = data.Components.FindIndexOf(GameMaster.AdditionalAssetsAll[a].Key.Name);
					data.ID = CurrentStepID++;
					data.Action = ActableTypes.Input;
					RecordedActions.Add(data);
					return;

				}

			}

			for(int ty = 0; ty < GameSpecificActableTypes.Count; ty++) {

				Component c = listener.gameObject.GetComponent(GameSpecificActableTypes[ty].Key.Name);
				if(c != null || matches.FindAll(x => x.Value == GameSpecificActableTypes[ty].Value).Any()) {

					data.AsComponent = data.Components.FindIndexOf(GameSpecificActableTypes[ty].Key.Name);
					data.ID = CurrentStepID++;
					data.Action = ActableTypes.Input;
					RecordedActions.Add(data);
					return;

				}

			}

			/*TODO: Update around text clickable for floating assertions.
	        Text txt = listener.gameObject.GetComponent<Text>();
			TMPro.TextMeshProUGUI tmp = listener.gameObject.GetComponent<TMPro.TextMeshProUGUI>();
	        if(txt != null || tmp != null) {

	           data.AsComponent = txt != null ? data.Components.FindIndexOf("Text") : data.Components.FindIndexOf("TextMeshProUGUI");
	           data.ID = CurrentStepID++;
	           data.Action = ActableTypes.TextForAssert;
	           RecordingAssertionData assert = new RecordingAssertionData(CurrentAssertionID++);
	           assert.Type = AssertionType.IsTrue;
	           assert.AssertionIsTrue = AssertionIsTrue.TextContainsOrEquals;
	           RecordedActions.Add(data);
	           return;

	        }
			*/

		}

		/// <summary>
		/// Change the "chronological" order of a recorded action. Returns the moved object's new index in the entire action list.
		/// </summary>
		/// <param name="currentIndex"> Action's current index in the list. </param>
		/// <param name="moveUp"> Move the action up in the list? False moves the object down in the list. </param>
		public static int ReOrderAction(int currentIndex, bool moveUp) {

			int newIndex = moveUp ? currentIndex - 1 : currentIndex + 1;
			List<RecordedGameObjectData> ReorderedList = new List<RecordedGameObjectData>();
			for(int x = 0; x < AutomationRecorder.RecordedActions.Count; x++) {

				if(x == newIndex) {

					if(moveUp) {

						ReorderedList.Add(AutomationRecorder.RecordedActions[currentIndex]);
						ReorderedList.Add(AutomationRecorder.RecordedActions[x]);

					} else {

						ReorderedList.Add(AutomationRecorder.RecordedActions[x]);
						ReorderedList.Add(AutomationRecorder.RecordedActions[currentIndex]);

					}

				} else if(x == currentIndex) {

					continue; //Ignore, as this is re-added in the above condition.

				} else {

					ReorderedList.Add(AutomationRecorder.RecordedActions[x]);

				}

			}

			AutomationRecorder.RecordedActions = ReorderedList;
			return newIndex;

		}

		/// <summary>
		/// Grabs the object containing this one that should be used for searching and finding the correct object in test execution. 
		/// It is very likely that objects in the game share names, but much less likely that objects under the same parent level object share the same name.
		/// </summary>
		/// <returns>The top level parent object.</returns>
		/// <param name="obj">Object.</param>
		public static KeyValuePair<string,List<string>> GetTopLevelParentObject(GameObject obj) {

			List<string> allParentObjectNames = new List<string>();
			GameObject inspected = obj;
			bool match = false;
			while(!match) {

				List<Component> comps = inspected.GetComponents<Component>().ToList();
				//Check component script name or its base type name for match.
				if(comps.FindAll(x => GameMaster.ExpectedTopLevelMasterScripts.FindAll(y => y.Name == x.GetType().Name || (x.GetType().BaseType != null && y.Name == x.GetType().BaseType.Name)).Any()).Any()) {

					break;

				}

				if(inspected.transform.parent != null) {

					inspected = inspected.transform.parent.gameObject;
					allParentObjectNames.Add(inspected.name);

				} else {

					break; //No expected top level script, so simply return the top level object as the "find in" object.

				}

			}

			return new KeyValuePair<string, List<string>>(inspected.name, allParentObjectNames);

		}

		public static void GenerateFullCodeSnippet() {

			StringBuilder snippet = new StringBuilder();
			snippet.AppendLine("<style> .code_display { border: 1px solid black; border-radius: 6px; padding: 20px; margin: 20px; } .code_line { padding-bottom: 5px; } .object_declaration { color: #0f93b5; font-weight: bold; } .string_text { color: #cc7a00; } .title { padding: 20px; } .value { color: #008080; } .variable { color: #537da5; } </style>");
			snippet.AppendLine("<h2 class='title'>GENERATED CODE</h2>");

			snippet.AppendLine("<h4 class='title'>Using</h2>");
			snippet.AppendLine("<div id='using_statements' class='code_display'>");
			snippet.AppendLine("<div class='code_line'><span class='object_declaration'>using</span> System;</div>");
			snippet.AppendLine("<div class='code_line'><span class='object_declaration'>using</span> UnityEngine;</div>");
			if(AutomationRecorder.RecordedActions.FindAll(x => x.Action == ActableTypes.Input || x.Action == ActableTypes.Scroll).Any()) {

				snippet.AppendLine("<div class='code_line'><span class='object_declaration'>using</span> UnityEngine.UI;</div>");

			}
			snippet.AppendLine("</div>");

			snippet.AppendLine("<h4 class='title'>Test Code</h2>");
			snippet.AppendLine("<div id='code_display' class='code_display'>");

			snippet.AppendLine("<div class='code_line'><span class='object_declaration'>GameObject</span> <span class='variable'>parentObject</span> = <span class='value'>null</span>;</div>");
			bool setCurrentObjectValue = true;
			bool setMiddleLevelObjectValue = true;

			for(int x = 0; x < AutomationRecorder.RecordedActions.Count; x++) {

				snippet.AppendLine("<div class='code_line'>");
				bool anyAssertions = AutomationRecorder.RecordedActions[x].Assertions.Any();

				if(anyAssertions && setCurrentObjectValue) {

					setCurrentObjectValue = false;
					snippet.AppendLine("<div class='code_line'><span class='object_declaration'>GameObject</span> <span class='variable'>currentObject</span> = <span class='value'>null</span>;</div>");
					snippet.AppendLine("</div><div class='code_line'>");

				}

				if(setMiddleLevelObjectValue && AutomationRecorder.RecordedActions[x].ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN) {

					//Searches for a parent level object closer to the object attempting to be found, to decrease finding of wrong object with identical name.
					setMiddleLevelObjectValue = false;
					snippet.AppendLine("<div class='code_line'><span class='object_declaration'>GameObject</span> <span class='variable'>middleLevelObject</span> = <span class='value'>null</span>;</div>");
					snippet.AppendLine("</div><div class='code_line'>");

				}

				string assert = RenderAssertions(true, x);

				switch(AutomationRecorder.RecordedActions[x].Action) {
					case ActableTypes.Clickable:
						snippet.AppendLine(GenerateClickCodeStub(AutomationRecorder.RecordedActions[x], anyAssertions, assert));
						break;
					case ActableTypes.Input:
						snippet.AppendLine(GenerateInputCodeStub(AutomationRecorder.RecordedActions[x], anyAssertions, assert));
						break;
					case ActableTypes.Scroll:
						snippet.AppendLine(GenerateScrollCodeStub(AutomationRecorder.RecordedActions[x], anyAssertions, assert));
						break;
					case ActableTypes.Draggable:
						//TODO
						break;
					case ActableTypes.KeyDown:
						//TODO
						break;
					case ActableTypes.Screenshot:
						snippet.AppendLine(string.Format("<div class='code_line'><span class='value'><span class='value'>yield return StartCoroutine</span></span>(<span class='object_declaration'>Q</span>.driver.TakeTestScreenshot(\"{0}\"));</div>", AutomationRecorder.RecordedActions[x].Name));
						break;
					case ActableTypes.Wait:
						snippet.AppendLine(string.Format("<div class='code_line'><span class='value'><span class='value'>yield return StartCoroutine</span></span>(<span class='object_declaration'>Q</span>.driver.WaitRealTime({0}f));</div>", AutomationRecorder.RecordedActions[x].Duration.ToString()));
						break;
				}

				snippet.AppendLine("</div>");
				snippet.AppendLine(RenderAssertions(false, x));

			}

			snippet.AppendLine("</div>");
			FileBroker.CreateFileAtPath(string.Format("{0}RecordedTestCodeGen.html", FileBroker.RESOURCES_DIRECTORY), snippet.ToString());

		}

		public static string RenderAssertions(bool before, int x) {

			StringBuilder assertions = new StringBuilder();
			List<RecordingAssertionData> Assertions = AutomationRecorder.RecordedActions[x].Assertions.FindAll(y => y.AssertionBeforeStep == (before ? true : false));
			for(int a = 0; a < Assertions.Count; a++) {

				assertions.AppendLine("<div class='code_line'>");
				RecordingAssertionData assertion = Assertions[a];
				if(assertion.Type == AssertionType.FreeForm) {

					assertions.AppendLine(string.Format("<span class='object_declaration'>Q</span>.assert.{0}();", assertion.AssertionArgument));


				} else {

					string assertTypeCode = string.Empty;
					string assertArgumentCode = string.Empty;

					switch(assertion.Type) {
					case AssertionType.IsTrue:
						assertTypeCode = "IsTrue";
						assertArgumentCode = string.Format("currentObject.Text.Contains{0}(<span class='string_text'>\"{1}\"</span>)", assertion.AssertionIsTrue == AssertionIsTrue.TextEquals ? string.Empty : "OrEquals", assertion.AssertionArgument);
						break;
					case AssertionType.IsInteractable:
						assertTypeCode = "IsActiveVisibleAndInteractable";
						assertArgumentCode = "currentObject";
						break;                 
					case AssertionType.NotNull:
						assertTypeCode = "NotNull";
						assertArgumentCode = "currentObject";
						break;                   
					}

					assertions.AppendLine(string.Format("<div class='code_line'><span class='value'>yield return StartCoroutine</span>(<span class='object_declaration'>Q</span>.assert.{0}({1}, <span class='string_text'>\"{2}\"</span>{3}{4}));</div>", assertTypeCode, assertArgumentCode, assertion.AssertionMessage, assertion.FailureContext != FailureContext.Default ? string.Format(", FailureContext.{0}", assertion.FailureContext) : string.Empty, assertion.IsReverseCondition ? ", true" : string.Empty));

				}
				assertions.AppendLine("</div>");

			}

			if(!before && x == AutomationRecorder.RecordedActions.Count - 1 && !_lastAddedStep.Contains("yield return")) {

				//The final line of test code in an enumerator must be a return statement.
				assertions.AppendLine("<div class='code_line'><span class='value'>yield return null</span>;</div>");

			}

			return _lastAddedStep = assertions.ToString();

		}

		static string _lastParentObjectSet = string.Empty;
		static string _lastMidLevelObjectSet = string.Empty;
		static string _lastCurrentObjectSet = string.Empty;
		static string _lastAddedStep = string.Empty;

		public static string GenerateClickCodeStub(RecordedGameObjectData obj, bool anyAssertions, string assert) {

			StringBuilder stub = new StringBuilder();

			//TagName takes priority.
			bool hasTagName = obj.Tag.ToLower() != "untagged";

			//If no tag, search by name instead, under the context of the top level parent that contains a recognized top-level script.
			if(!hasTagName) {

				string parent = string.Format("<div class='code_line'><span class='variable'>parentObject</span> = <span class='object_declaration'>Q</span>.driver.Find(By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>", obj.TopLevelParentName);
				bool deferredAppend = false;
				if(_lastParentObjectSet != parent) {

					if(obj.ParentNames.Count <= MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN || string.IsNullOrEmpty(_lastParentObjectSet)) {

						stub.AppendLine(parent);

					}
					_lastParentObjectSet = parent;
					deferredAppend = true;

				} else {

					parent = string.Empty;

				}

				string midLevelParentName = string.Empty;
				if(obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN) {

					midLevelParentName = obj.ParentNames[(int)Math.Round((double)obj.ParentNames.Count / 2, 0)];
					if(midLevelParentName != obj.TopLevelParentName) {

						if(!string.IsNullOrEmpty(parent)) {

							stub.AppendLine(parent);

						}
						stub.AppendLine(string.Format("<div class='code_line'><span class='variable'>middleLevelObject</span> = <span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>parentObject</span>, By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>",midLevelParentName));


					} else if(deferredAppend) {

						stub.AppendLine(parent);

					}

				}

				string currentObjectFind = string.Format("<span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>{0}</span>, By.Name, <span class='string_text'>\"{1}\"</span>, false)",  obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN &&  midLevelParentName != obj.TopLevelParentName ? "middleLevelObject" : "parentObject", obj.Name);
				if(anyAssertions) {

					string current = string.Format("<div class='code_line'><span class='variable'>currentObject</span> = {0};</div>", currentObjectFind);
					if(_lastCurrentObjectSet != current) {

						_lastCurrentObjectSet = current;
						stub.AppendLine(_lastCurrentObjectSet);

					}

				}
				string assertionMessage = obj.IsTry ? string.Empty: string.Format(", <span class='string_text'>\"Click object with name {0}\"</span>", obj.Name);
				stub.AppendLine(string.Format("<div class='code_line'><span class='value'>yield return StartCoroutine</span>(<span class='object_declaration'>Q</span>.driver.{0}Click({1}{2}));</div>", obj.IsTry ? "Try" : string.Empty, anyAssertions ? "currentObject" : currentObjectFind, assertionMessage));

			} else {

				string currentObjectFind = string.Format("<span class='object_declaration'>Q</span>.driver.Find(By.TagName, <span class='string_text'>\"{0}\"</span>, false)", obj.IsTry ? "Try" : string.Empty, obj.Tag);
				if(anyAssertions) {

					string current = string.Format("<div class='code_line'><span class='variable'>currentObject</span> = {0};</div>", currentObjectFind);
					if(_lastCurrentObjectSet != current) {

						_lastCurrentObjectSet = current;
						stub.AppendLine(_lastCurrentObjectSet);

					}

				}
				string assertionMessage = obj.IsTry ? string.Empty: string.Format(", <span class='string_text'>\"Click object with tagname {0}\"</span>", obj.Name);
				if(!string.IsNullOrEmpty(assert)) {

					stub.AppendLine(assert);

				}
				stub.AppendLine(string.Format("<div class='code_line'><span class='value'>yield return StartCoroutine</span>(<span class='object_declaration'>Q</span>.driver.{0}Click({1}{2}));</div>", obj.IsTry ? "Try" : string.Empty, anyAssertions ? "currentObject" : currentObjectFind, assertionMessage));

			}

			return _lastAddedStep = stub.ToString();

		}

		public static string GenerateInputCodeStub(RecordedGameObjectData obj, bool anyAssertions, string assert) {

			StringBuilder stub = new StringBuilder();
			string parent = string.Format("<div class='code_line'><span class='variable'>parentObject</span> = <span class='object_declaration'>Q</span>.driver.Find(By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>", obj.TopLevelParentName);
			if(_lastParentObjectSet != parent) {

				if(obj.ParentNames.Count <= MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN) {

					stub.AppendLine(parent);

				}
				_lastParentObjectSet = parent;

			} else {

				parent = string.Empty;

			}

			string midLevelParentName = string.Empty;
			if(obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN) {

				if(!string.IsNullOrEmpty(parent)) {

					stub.AppendLine(parent);

				}
				midLevelParentName = obj.ParentNames[(int)Math.Round((double)obj.ParentNames.Count / 2, 0)];
				if(midLevelParentName != obj.TopLevelParentName && _lastMidLevelObjectSet != midLevelParentName) {

					_lastMidLevelObjectSet = midLevelParentName;
					stub.AppendLine(string.Format("<div class='code_line'><span class='variable'>middleLevelObject</span> = <span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>parentObject</span>, By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>", midLevelParentName));

				}

			}

			string currentObjectFind = string.Format("<span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>{0}</span>, By.Name, <span class='string_text'>\"{1}\"</span>)", obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN &&  midLevelParentName != obj.TopLevelParentName? "middleLevelObject" : "parentObject", obj.Name);
			if(anyAssertions) {

				string current = string.Format("<div class='code_line'><span class='variable'>currentObject</span> = {0};</div>", currentObjectFind);
				if(_lastCurrentObjectSet != current) {

					_lastCurrentObjectSet = current;
					stub.AppendLine(_lastCurrentObjectSet);

				}

			}
			if(!string.IsNullOrEmpty(assert)) {

				stub.AppendLine(assert);

			}

			string inputClassName = string.Empty;
			for(int d = 0; d < obj.Components.Count; d++) {

				if(obj.Components[d] == "InputField") {

					inputClassName = "InputField";
					break;

				} else if(GameSpecificActableTypes.FindAll(x => x.Key.Name == obj.Components[d] && x.Value == ActableTypes.Input).Any()) {

					inputClassName = GameSpecificActableTypes.Find(x => x.Key.Name == obj.Components[d] && x.Value == ActableTypes.Input).Key.Name;
					break;

				} else if(GameMaster.AdditionalAssetsAll.FindAll(x => x.Key.Name == obj.Components[d] && x.Value == ActableTypes.Input).Any()) {

					inputClassName = GameMaster.AdditionalAssetsAll.Find(x => x.Key.Name == obj.Components[d] && x.Value == ActableTypes.Input).Key.Name;
					break;

				}

			}

			stub.AppendLine(string.Format("<div class='code_line'><span class='value'>yield return StartCoroutine</span>(<span class='object_declaration'>Q</span>.driver.SendKeys({0}.GetComponent&lt;<span class='variable'>{1}</span>&gt;(), {2}));</div>", anyAssertions ? "currentObject" : currentObjectFind, inputClassName, obj.RandomStringLength > 0 ? string.Format("<span class='object_declaration'>Q</span>.help.RandomString({0}, {1})", obj.RandomStringLength.ToString(), obj.RandomStringAllowSpecialCharacters ? "false" : "true") : string.Format("<span class='string_text'>\"{0}\"</span>", obj.TextValue)));
			return _lastAddedStep = stub.ToString();

		}

		public static string GenerateScrollCodeStub(RecordedGameObjectData obj, bool anyAssertions, string assert) {

			StringBuilder stub = new StringBuilder();
			_lastParentObjectSet = string.Format("<div class='code_line'>parentObject = <span class='object_declaration'>Q</span>.driver.Find(By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>", obj.TopLevelParentName);
			string parent = string.Format("<div class='code_line'>parentObject = <span class='object_declaration'>Q</span>.driver.Find(By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>", obj.TopLevelParentName);
			bool deferredAppend = false;
			if(_lastParentObjectSet != parent) {

				if(obj.ParentNames.Count <= MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN || string.IsNullOrEmpty(_lastParentObjectSet)) {

					stub.AppendLine(parent);

				}
				_lastParentObjectSet = parent;
				deferredAppend = true;

			} else {

				parent = string.Empty;

			}

			string midLevelParentName = string.Empty;
			if(obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN) {

				midLevelParentName = obj.ParentNames[(int)Math.Round((double)obj.ParentNames.Count / 2, 0)];
				if(midLevelParentName != obj.TopLevelParentName) {

					if(!string.IsNullOrEmpty(parent)) {

						stub.AppendLine(parent);

					}
					stub.AppendLine(string.Format("<div class='code_line'><span class='variable'>middleLevelObject</span> = <span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>parentObject</span>, By.Name, <span class='string_text'>\"{0}\"</span>, false);</div>",midLevelParentName));

				} else if(deferredAppend) {

					stub.AppendLine(parent);

				}

			}

			string currentObjectFind = string.Format("<span class='object_declaration'>Q</span>.driver.FindIn(<span class='variable'>{0}</span>, By.Name, <span class='string_text'>\"{1}\"</span>)", obj.ParentNames.Count > MAX_NUM_PARENTNAMES_BEFORE_DRILLDOWN &&  midLevelParentName != obj.TopLevelParentName? "middleLevelObject" : "parentObject", obj.Name);
			if(anyAssertions) {

				string current = string.Format("<div class='code_line'><span class='variable'>currentObject</span> = {0};</div>", currentObjectFind);
				if(_lastCurrentObjectSet != current) {

					_lastCurrentObjectSet = current;
					stub.AppendLine(_lastCurrentObjectSet);

				}

			}
			if(!string.IsNullOrEmpty(assert)) {

				stub.AppendLine(assert);

			}
			stub.AppendLine(string.Format("<div class='code_line'><span class='value'>yield return StartCoroutine</span>(<span class='object_declaration'>Q</span>.driver.Scroll({0}, 1, true, {1}));</div>", anyAssertions ? "currentObject" : currentObjectFind, obj.ScrollDistance.ToString()));

			return _lastAddedStep = stub.ToString();

		}

	}
	#endif

}
