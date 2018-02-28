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

                     if(AutomationRecorder.RecordedActions.Last().Key != ActableTypes.KeyDown || (AutomationRecorder.RecordedActions.Last().Value.Value != null && AutomationRecorder.RecordedActions.Last().Value.Value.KeyDown != k)) {

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

			if(GetComponent<InputField>() != null) {

				if(PointerDownHandled && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Input && AutomationRecorder.RecordedActions.Last().Name == name) {

					return;

				}

				PointerDownHandled = true;
				GetComponent<InputField>().onValueChanged.AddListener(delegate {
					UpdateExistingInputStep();
				});

			}

			if(GetComponent<ScrollRect>() != null) {

				if(PointerDownHandled && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Scroll && AutomationRecorder.RecordedActions.Last().Name == name) {

					return;

				}

				PointerDownHandled = true;
				GetComponent<ScrollRect>().onValueChanged.AddListener(delegate {
					UpdateExistingScroller();
				});

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
				data.ScrollDistance = (float)Math.Round(Math.Abs(scrollVal - data.InitialScrollPosition), 1) * 100; //Percent 0-100.
				data.ScrollDirection = scrollVal - data.InitialScrollPosition < 0 ? ScrollDirection.RightOrDownToLeftOrUp : ScrollDirection.LeftOrUpToRightOrDown; 
				AutomationRecorder.RemoveActionAt(AutomationRecorder.RecordedActions.Count - 1);
				AutomationRecorder.AddAction(data);

			} else {

				//The last action should be for this input field. If it isn't, something unexpected has happened, so clear this listener.
				GetComponent<ScrollRect>().onValueChanged.RemoveListener(delegate { UpdateExistingScroller(); });

			}

		}

		void UpdateExistingInputStep() {

			if(AutomationRecorder.RecordedActions != null && AutomationRecorder.RecordedActions.Last().Action == ActableTypes.Input) {

				RecordedGameObjectData data = AutomationRecorder.RecordedActions.Last();
				data.TextValue = GetComponent<InputField>().text;
				AutomationRecorder.RemoveActionAt(AutomationRecorder.RecordedActions.Count - 1);
				AutomationRecorder.AddAction(data);

			} else {

				//The last action should be for this input field. If it isn't, something unexpected has happened, so clear this listener.
				GetComponent<InputField>().onValueChanged.RemoveListener(delegate { UpdateExistingInputStep(); });

			}

		}

		void Selected() {

			if(!Q.driver.IsActiveVisibleAndInteractable(this.gameObject)) {

				return;

			}

			if(type == ActableTypes.TextForAssert && !AutomationRecorder.ActivateTextComponentSelection) {

				//return;

			}

			if(!AutomationRecorder.NotRecordingActions) {

				AutomationRecorder.AutomationRelevantActionTaken(this);
				StartCoroutine(AutomationRecorder.StaticSelfComponent.DelayedRefresh());

			}

			if(AutomationRecorder.SelectionUpdatesHeirarchy) {

				Selection.activeGameObject = gameObject;
				if(AutomationRecorder.PauseOnSelect) {

					EditorApplication.ExecuteMenuItem("Edit/Pause");

				}

			}

		}

	}

}
#endif