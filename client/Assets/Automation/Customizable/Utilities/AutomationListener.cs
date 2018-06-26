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
+*/

#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {

	[DisallowMultipleComponent]
	public class AutomationListener : MonoBehaviour, IPointerDownHandler {

		bool PointerDownHandled { get; set; }
		DateTime KeyDownStart { get; set; }
		public ActableTypes type { get; set; }

		void Update() {

			/* TODO: HANDLE KEYDOWN SCENARIOS
         //Ignore KeyDown functionality for objects that expect input.
         if(!InputPointerDownHandled && Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.Mouse1)) {
            
            if(Input.anyKeyDown) {
            
               foreach(KeyCode k in System.Enum.GetValues(typeof(KeyCode))) {
               
                  if(Input.GetKey(k)) {

                     RecordedGameObjectData data = new RecordedGameObjectData();
                     KeyDownStart = DateTime.UtcNow;                  

                     if(AutomationRecorder.RecordedActions.Last().Key != ActableTypes.KeyDown ||(AutomationRecorder.RecordedActions.Last().Value.Value != null && AutomationRecorder.RecordedActions.Last().Value.Value.KeyDown != k)) {

                        data.KeyDown = k;
                        data.Duration = data.Duration + Math.Abs(KeyDownStart.Subtract(DateTime.UtcNow).TotalSeconds); 
                        AutomationRecorder.RecordedActions.Add(new KeyValuePair<ActableTypes,KeyValuePair<string,RecordedGameObjectData>>(ActableTypes.KeyDown, new KeyValuePair<string,RecordedGameObjectData>(Enum.GetName(typeof(KeyCode), k), data)));

                     }

                     string name = AutomationRecorder.RecordedActions.Last().Value.Key;
                     data = AutomationRecorder.RecordedActions.Last().Value.Value;
                     data.Duration = data.Duration + Math.Abs(KeyDownStart.Subtract(DateTime.UtcNow).TotalSeconds); 
                     AutomationRecorder.RecordedActions.RemoveAt(AutomationRecorder.RecordedActions.Count - 1);
                     AutomationRecorder.RecordedActions.Add(new KeyValuePair<ActableTypes,KeyValuePair<string,RecordedGameObjectData>>(ActableTypes.KeyDown, new KeyValuePair<string,RecordedGameObjectData>(name, data)));

                  }

               }

            }

         }
         */

		}

		//TODO: Fix multi triggering
		bool selected = false;
		public void OnTriggerEnter(Collider other) {

			if(!selected) {

				selected = true;
				Selected();

			}

		}

		public void OnPointerDown(PointerEventData eventData) {

			List<MonoBehaviour> components = this.GetComponents<MonoBehaviour>().ToList();

			for(int x = 0; x < components.Count; x++) {

				Type componentType = typeof(InputField);
				ActableTypes found = ActableTypes.Wait;
				for(int a = 0; a < GameMaster.AdditionalAssetsAll.Count; a++) {

					if(components[x].GetType().IsAssignableFrom(GameMaster.AdditionalAssetsAll[a].Key)) {

						found = GameMaster.AdditionalAssetsAll[a].Value;
						componentType = GameMaster.AdditionalAssetsAll[a].Key;
						break;

					}

				}

				if(components[x].GetType().IsAssignableFrom(typeof(InputField)) || found == ActableTypes.Input) {

					if(PointerDownHandled && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Input && AutomationRecorder.RecordedActions.Last().Name == name) {

						return;

					}

					PointerDownHandled = true;
					if(componentType == typeof(InputField)) {
						
						GetComponent<InputField>().onValueChanged.AddListener(delegate {
							UpdateExistingInputStep();
						});

					} else {

						//TODO: Add your custom types here.

					}
					break;
						
				}

				if(components[x].GetType().IsAssignableFrom(typeof(ScrollRect))) {

					if(PointerDownHandled && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Scroll && AutomationRecorder.RecordedActions.Last().Name == name) {

						return;

					}

					PointerDownHandled = true;
					GetComponent<ScrollRect>().onValueChanged.AddListener(delegate {
						UpdateExistingScroller();
					});

					break;

				}

			}

			Selected();

		}

		void OnDisable() {

			//Remove Automation listener method for any input.
			if(GetComponent<InputField>() != null) {

				GetComponent<InputField>().onValueChanged.RemoveListener(delegate { UpdateExistingInputStep(); });

			} 
				
			//Remove Automation listener method for any ScrollRect.
			if(GetComponent<ScrollRect>() != null) {

				GetComponent<ScrollRect>().onValueChanged.RemoveListener(delegate { UpdateExistingScroller(); });

			} 

			//TODO: Add your custom types here.

		}

		void UpdateExistingScroller() {

			if(AutomationRecorder.RecordedActions != null && AutomationRecorder.RecordedActions.Any() && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Scroll) {

				ScrollRect rect = GetComponent<ScrollRect>();
				RecordedGameObjectData data = AutomationRecorder.RecordedActions.Last();
				float scrollVal = 0f;
				if(rect.verticalScrollbar == null && rect.horizontalScrollbar == null) {

					scrollVal = rect.vertical ? rect.content.position.x : rect.content.position.y;

				}
				scrollVal = rect.vertical ? rect.horizontalScrollbar.value : rect.verticalScrollbar.value;
				if(scrollVal == data.InitialScrollPosition) {

					scrollVal = scrollVal == 0 ? 1 : 0;

				}
				data.ScrollDistance =(float)Math.Round(Math.Abs(scrollVal - data.InitialScrollPosition), 1) * 100; //Percent 0-100.
				data.ScrollDirection = scrollVal - data.InitialScrollPosition < 0 ? ScrollDirection.RightOrDownToLeftOrUp : ScrollDirection.LeftOrUpToRightOrDown; 
				AutomationRecorder.RemoveActionAt(AutomationRecorder.RecordedActions.Count - 1);
				AutomationRecorder.AddAction(data);

			} else {

				//The last action should be for this input field. If it isn't, something unexpected has happened, so clear this listener.
				GetComponent<ScrollRect>().onValueChanged.RemoveListener(delegate { UpdateExistingScroller(); });

			}

		}

		void UpdateExistingInputStep() {

			List<MonoBehaviour> components = this.GetComponents<MonoBehaviour>().ToList();
			Type componentType = typeof(InputField);
			ActableTypes found = ActableTypes.Wait;

			for(int x = 0; x < components.Count; x++) {

				for(int a = 0; a < GameMaster.AdditionalAssetsAll.Count; a++) {

					if(components[x].GetType().IsAssignableFrom(GameMaster.AdditionalAssetsAll[a].Key)) {

						found = GameMaster.AdditionalAssetsAll[a].Value;
						componentType = GameMaster.AdditionalAssetsAll[a].Key;
						break;

					}

				}

			}

			if(AutomationRecorder.RecordedActions != null && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Input) {

				RecordedGameObjectData data = AutomationRecorder.RecordedActions.Last();
				if(componentType == typeof(InputField)) {

					data.TextValue = GetComponent<InputField>().text;

				} else {

					//TODO: Add your custom types here.
	
				}

				AutomationRecorder.RemoveActionAt(AutomationRecorder.RecordedActions.Count - 1);
				AutomationRecorder.AddAction(data);

			} else {

				//The last action should be for this input field. If it isn't, something unexpected has happened, so clear this listener.
				if(componentType == typeof(InputField)) {

					GetComponent<InputField>().onValueChanged.RemoveListener(delegate { UpdateExistingInputStep(); });

				} else {

					//TODO: Add your custom types here.

				}

			}

		}

		void Selected() {

			if(!Q.driver.IsActiveVisibleAndInteractable(this.gameObject)) {

				return;

			}

			if(type == ActableTypes.TextForAssert && !AutomationRecorder.ActivateTextComponentSelection) {

				//TODO: Not Implemented return; 

			}

			if(!AutomationRecorder.NotRecordingActions) {

				AutomationRecorder.AutomationRelevantActionTaken(this);
				StartCoroutine(AutomationRecorder.StaticSelfComponent.DelayedRefresh());

			}

			if(AutomationRecorder.SelectionUpdatesHierarchy) {

				Selection.activeGameObject = gameObject;
				if(AutomationRecorder.PauseOnSelect) {

					EditorApplication.ExecuteMenuItem("Edit/Pause");

				}

			}

		}

	}

}
#endif