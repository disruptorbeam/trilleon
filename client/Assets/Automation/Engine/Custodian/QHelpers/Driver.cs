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

﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrilleonAutomation {

	public class Driver : MonoBehaviour {

		public const float BASE_TIME_PER_KEY_TO_SEND = 1f; //Default amount of time added to total SendKeys time for each key requested.
		public const float TIMEOUT_DEFAULT = 20f; //Default timeout for Do-type events.
		public const float TRY_TIMEOUT_DEFAULT = 10f; //Default timeout for Try-type events.
		public const float INTERLOOP_WAIT_TIME = 1f; //Dictates the time between loop executions for wait functions.
		public const float UPDATE_DRAG_INTERVAL = 0.1f; //How often the IDragHandler receives position updates when dragging an object.
		public const float WAIT_AFTER_CLICK = 1f; //Sets the amount of time after event, such as Click, before returning control to the test.
		public const string DEFAULT_ERROR_MESSAGE = "Requested object could not be found"; //The default message to be displayed after an error if the call has not provided one.

		/// <summary>
		/// TODO: Ignore assertions if the requestion actions fails to execute properly.
		/// </summary>
		public Driver Try { 
			get {
				isTry = true;
				return this;
			} 
		}
		public bool isTry { get; protected set; }
		public bool isDeepDive { get; protected set; } //Prevents Try from being set to false if a driver call is made by a top-level Try call. The "Try" state does not trickle down otherwise, and would allow assertion failures within what is intended as a Try command.
		public static Sprite BlueRing { get; set; }
		public static Sprite MouseHand { get; set; }
		public static bool NoResourceDeactivateTracer { get; set; }
		public static GameObject AutomationSelectFeedback { get; set; }

		protected virtual void Start() {

			//Find Sprite asset from Resources; used by SelectTracer to show visual indication of automation-made selections.
			BlueRing = Resources.Load<Sprite>("BlueRing");
			MouseHand = Resources.Load<Sprite>("MouseHand");
			NoResourceDeactivateTracer = BlueRing == default(Sprite) ? true : false;

		}
	
		protected virtual void SelectTracer(GameObject selectedObject) {

			if(NoResourceDeactivateTracer || selectedObject ==  null || !Q.driver.IsActiveVisibleAndInteractable(selectedObject)) {
				
				return;

			}

			if(AutomationSelectFeedback != null) {
				
				DestroyImmediate(AutomationSelectFeedback);

			}

			#if UNITY_EDITOR
			if(AutomationRecorder.SelectionUpdatesHierarchy) {

				Selection.activeGameObject = selectedObject;

			}
			#endif

			AutomationSelectFeedback = new GameObject("AutomationSelectFeedback");
			AutomationSelectFeedback.transform.position = selectedObject.transform.position;
			AutomationSelectFeedback.AddComponent<AutomationSelectFeedback>();

		}

		protected virtual void PreCommandCheck(bool canTry = false) {

			AutomationMaster.LastUseTimer = DateTime.UtcNow;

		}

		protected virtual void PostCommandCheck(bool canTry = false) {

			if(canTry && !isDeepDive) {
				
				isTry = false;

			}

		}

		public IEnumerator TakeTestScreenshot(string name) {

			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());
			yield return StartCoroutine(AutomationMaster.StaticSelfComponent.TakeScreenshot(false, string.Format("{0}_{1}", AutomationMaster.CurrentTestContext.TestName, name.Replace(" ", "_"))));

		}

		#region Send Keys

		/// <summary>
		/// Sets the text field of an InputField object. Sets text character by character for a typing effect that would also trigger update listeners.
		/// </summary>
		public IEnumerator SendKeys(InputField field, string keysToSend, bool isAppend = false){

			PreCommandCheck(true);

			if(field != null) {

				SelectTracer(field.gameObject);

				if(keysToSend != null && field.isActiveAndEnabled) {

					AutoHud.UpdateMessage(string.Format("{0} SendKeys \"{1}\"", field.gameObject.name, keysToSend));
					Click(field.gameObject); //Send focus to input.

					if(!isAppend) {

						field.text = string.Empty;

					}

					char[] keys = keysToSend.ToCharArray();
					float waitTimeBetweenKeyPresses = BASE_TIME_PER_KEY_TO_SEND * keys.Length; //The wait between key presses reduces dramatically as the text to input increases.
					waitTimeBetweenKeyPresses = waitTimeBetweenKeyPresses > 0.1f ? 0.1f : waitTimeBetweenKeyPresses;
					for (int x = 0; x < keys.Length; x++) {

						field.text = string.Format("{0}{1}", field.text, keys[x]);
						yield return StartCoroutine(WaitRealTime(waitTimeBetweenKeyPresses));

					}

					if (isTry) {

						yield return StartCoroutine(Q.assert.Try.Pass(string.Format("Send Keys \"{0}\" to {1}.", keysToSend, field.gameObject.name)));

					} else {

						yield return StartCoroutine(Q.assert.Pass(string.Format("Send Keys \"{0}\" to {1}.", keysToSend, field.gameObject.name)));

					}

					Click(field.gameObject.transform.parent.gameObject); //Remove focus from input.
				}

			} else {

				if(isTry) {
					
					yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Cannot enter text in field {0}, as it is not available", field != null ? field.name : "[provided field is null or inactive]")));

				} else {

					yield return StartCoroutine(Q.assert.Fail(string.Format("Cannot enter text in field {0}, as it is not available", field != null ? field.name : "[provided field is null or inactive]")));

				}

			}

			PostCommandCheck(true);

		}

		#endregion

		#region Scroll

		//TODO: Handle BOTH horizontal and vertical scroller possibility.
		/// <summary>
		/// Scroll a ScrollRect.
		/// </summary>
		/// <param name="scrollRectObject">ScrollRect or GameObject containing ScrollRect.</param>
		/// <param name="durationOfScroll">How long does the scrolling action take to complete?</param>
		/// <param name="scrollDownwardOrToTheRight">Is this scrolling direction down/right or up/left?</param>
		/// <param name="distanceToTravelBetweenZeroAndOne">A ScrollRect scrolls a distance of 0 to 1. If we do not want to scroll to the end/beginning, how far do we scroll?</param>
		public IEnumerator Scroll(GameObject scrollRectObject, float durationOfScroll, bool scrollDownwardOrToTheRight = true, float distanceToTravelBetweenZeroAndOne = 1f) {


			if(scrollRectObject == null && !isTry) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail("Provided ScrollView GameObject is null."));

				} else {

					yield return StartCoroutine(Q.assert.Fail("Provided ScrollView GameObject is null."));

				}
				yield break;

			}

			AutoHud.UpdateMessage(string.Format("{0} Scroll \"{1}\"", scrollRectObject.name, scrollDownwardOrToTheRight ? "Right" : "Left"));
			ScrollRect rect = scrollRectObject.GetComponent<ScrollRect>();
			if(rect == null && !isTry) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Provided GameObject \"{0}\" lacks a ScrollRect that would allow it to behave as a scroll view.", scrollRectObject.name)));

				} else {

					yield return StartCoroutine(Q.assert.Fail(string.Format("Provided GameObject \"{0}\" lacks a ScrollRect that would allow it to behave as a scroll view.", scrollRectObject.name)));

				}
				yield break;

			}

			yield return StartCoroutine(Scroll(rect, durationOfScroll, scrollDownwardOrToTheRight, distanceToTravelBetweenZeroAndOne));

		}
		public IEnumerator Scroll(ScrollRect scrollRect, float durationOfScroll, bool scrollDownwardOrToTheRight = true, float distanceToTravelBetweenZeroAndOne = 1f) {

			if(scrollRect == null) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail("Provided ScrollRect is null."));

				} else {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail("Provided ScrollRect is null."));

				}
				yield break;

			}

			float distanceToTraverse;
			float distanceOverride = distanceToTravelBetweenZeroAndOne > 1f ? 1f : distanceToTravelBetweenZeroAndOne;

			bool noScrollBars = scrollRect.horizontalScrollbar == null && scrollRect.verticalScrollbar == null;
			if(noScrollBars) {

				distanceToTraverse = scrollRect.vertical ? scrollRect.content.sizeDelta.y : scrollRect.content.sizeDelta.x;
				distanceToTraverse = distanceToTraverse * distanceOverride; //Percentage of total distance.

			} else {
				
				distanceToTraverse = scrollRect.vertical ? (scrollDownwardOrToTheRight ? distanceOverride - scrollRect.verticalScrollbar.value : scrollRect.verticalScrollbar.value) : (scrollDownwardOrToTheRight ? distanceOverride - scrollRect.horizontalScrollbar.value : scrollRect.horizontalScrollbar.value);

			}

			float movementPerSecondBaseline = distanceToTraverse / durationOfScroll; //Divide the total distance to traverse by the scroll duration to get an idealized movement-per-second.
			float durationTimer = 0f;
			while(durationTimer <= durationOfScroll) {

				//Each frame delta time multiplied by the idealized movement-per-second to get a smoother scroll.
				float distanceMoveNow = Time.deltaTime * movementPerSecondBaseline;
				durationTimer += Time.deltaTime;
				Canvas.ForceUpdateCanvases();

				if(!noScrollBars) {
					
					if(!scrollRect.vertical) {
						
						scrollRect.horizontalScrollbar.value += (scrollDownwardOrToTheRight ? 1 : -1) * distanceMoveNow;

					} else {
						
						scrollRect.verticalScrollbar.value += (scrollDownwardOrToTheRight ? 1 : -1) * distanceMoveNow;

					}

				} else {

					Vector3 position = scrollRect.content.localPosition;

					if(!scrollRect.vertical) {

						position.x += (scrollDownwardOrToTheRight ? 1 : -1) * distanceMoveNow;
						scrollRect.content.localPosition = position;

					} else {

						position.y += (scrollDownwardOrToTheRight ? 1 : -1) * distanceMoveNow;
						scrollRect.content.localPosition = position;

					}

				}

				Canvas.ForceUpdateCanvases();
				if(!noScrollBars) {

					if((!scrollRect.vertical && (scrollRect.horizontalScrollbar.value <= 0 || scrollRect.horizontalScrollbar.value >= 1)) || (scrollRect.vertical && (scrollRect.verticalScrollbar.value <= 0 || scrollRect.verticalScrollbar.value >= 1))) {

						break;

					}

				} else {

					if(scrollRect.content.localPosition.y > distanceToTraverse || scrollRect.content.localPosition.x > distanceToTraverse) {

						break;

					}

				}

				yield return new WaitForEndOfFrame();

			}

			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		#endregion

		#region Is Condition True (Does Not Assert)

		/// <summary>
		/// Tests several different points of data that determine if a GameObject is actively visible and interactable to a user.
		/// 1) Does this object extend Selectable or does it have a CanvasGroup? If so, is the `interactable` field set to true?
		/// 2) Is the object active in its hierarchy?
		/// 3) Is the object active itself?
		/// 4) Performs game-specific checks as defined in GameMaster. For example, do you have a Tutorial mask that blocks raycasts down to the object, despite it being otherwise active? (you will need to explicitly add the logic to check that)
		/// </summary>
		public bool IsActiveVisibleAndInteractable(GameObject g, bool checkComponents = true) {


			PreCommandCheck();
			if(g == null) {

				PostCommandCheck();

				return false;

			} else {


				AutoHud.UpdateMessage(string.Format("{0} IsActiveVisibleAndInteractable", g.name));

				bool buttonInteractable = g.GetComponent<Button>() != null && checkComponents ? g.GetComponent<Button>().interactable : true;
				bool toggleInteractable = g.GetComponent<Toggle>() != null && checkComponents ? g.GetComponent<Toggle>().interactable : true;
				bool inputInteractable = g.GetComponent<InputField>() != null && checkComponents ? g.GetComponent<InputField>().interactable : true;
				bool isActive = g.activeInHierarchy;
				bool isVisible = g.activeSelf;
				CanvasGroup cg = g.GetComponent<CanvasGroup>();
				bool isInteractable = cg != null ? cg.interactable : true;
				bool isGameSpecificVisible = GameMaster.GameSpecificVisibilityChecks(g);

				PostCommandCheck();

				return buttonInteractable && toggleInteractable && inputInteractable && isActive && isVisible && isInteractable && isGameSpecificVisible;

			} 

		}
		public bool IsActiveVisibleAndInteractable(List<Component> cs, bool checkComponents = true) {

			for(int x = 0; x < cs.Count; x++) {

				if(!IsActiveVisibleAndInteractable(cs[x].gameObject, checkComponents)) {

					return false;

				}

			}

			return true;

		}
		public bool IsActiveVisibleAndInteractable(Component c, bool checkComponents = true) {

			if(c == null) {

				return false;

			} else {

				return IsActiveVisibleAndInteractable(c.gameObject, checkComponents);

			}

		}
		public bool IsActiveVisibleAndInteractable(List<GameObject> gs, bool checkComponents = true) {

			for(int x = 0; x < gs.Count; x++) {

				if(IsActiveVisibleAndInteractable(gs[x], checkComponents)) {

					return true;

				}

			}

			return false;

		}

        #endregion

        #region Find Objects

        public GameObject FindParent(GameObject obj, By by, string val, bool isContains = true) {

            if(obj == null) {

                if(isTry) {

                    Q.assert.StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Null GameObject provided to finder (FindParent) along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

                } else {

                    Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Null GameObject provided to finder (FindParent) along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

                }
                return null;

            }
            GameObject match = null;
            GameObject currentObjectToInspect = obj.transform.parent != null ? obj.transform.parent.gameObject : null;
            while(match == null && currentObjectToInspect != null) {

                bool isMatch = false;
                switch(by) {
                    case By.Name:
                        if(isContains) {

                            isMatch = currentObjectToInspect.name.Contains(val);

                        } else {

                            isMatch = currentObjectToInspect.name == val;

                        }
                        break;
                    case By.TagName:
                        if(isContains) {

                            isMatch = currentObjectToInspect.tag.Contains(val);

                        } else {

                            isMatch = currentObjectToInspect.tag == val;

                        }
                        break;
                    case By.ContainsComponent:
                        isMatch = currentObjectToInspect.GetComponent(val) != null;
                        break;
                    default:
                        throw new NotImplementedException("That search condition has not yet been implemented in FindParent().");

                }
      
                if(isMatch) {

                    match = currentObjectToInspect;
                    break;

                } else {

                    currentObjectToInspect = currentObjectToInspect.transform.parent != null ? currentObjectToInspect.transform.parent.gameObject : null;

                }

            }
            return match;

        }

		/// <summary>
		/// Find single object by attribute type and provided value.
		/// </summary>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
		public GameObject FindIn(GameObject obj, By by, string val, bool isContains = true) {

			PreCommandCheck();

			if(obj == null) {

				if(isTry) {

					Q.assert.StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Null GameObject provided to finder along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				} else {

					Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Null GameObject provided to finder along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				}
				return null;

			}

			List<GameObject> result = new List<GameObject>();
			List<GameObject> all = obj.GetChildren();
			switch(by) {
				case By.TextContent:
					result = GetByTextContent(val, isContains, all);
					return result.Any() ? result.First() : null;
				case By.TagName:
					return GameObject.FindWithTag(val);
				case By.GameObjectType:
					throw new NotImplementedException();
				case By.ImageFileName:
					throw new NotImplementedException();      
				case By.Hierarchy:
				case By.Name:
					List<GameObject> matches = new List<GameObject>();
					if(isContains) {
						
						matches = all.FindAll(x => x.name.Contains(val));

					} else {
						
						matches = all.FindAll(x => x.name == val);

					}
					if(matches.Count > 1) {
						
						matches = matches.GetActiveAndVisibleObjectsInList();

					}
					return matches.First();
				default:
					return null;
			}

		}
		public GameObject FindIn(Component comp, By by, string val, bool isContains = true) {
			
			return FindIn(comp == null ? null : comp.gameObject, by, val, isContains);

		}
		public GameObject FindIn(List<GameObject> objs, By by, string val, bool isContains = true) {

			PreCommandCheck();

			if(!objs.Any()) {

				if(isTry) {

					Q.assert.StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				} else {

					Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				}
				return null;

			}

			List<GameObject> result = new List<GameObject>();
			List<GameObject> all = objs.GetChildren();
			switch(by) {
				case By.TextContent:
					result = GetByTextContent(val, isContains, all);
					break;
				case By.TagName:
					if(isContains) {
						
						result = all.FindAll(x => x.tag.Contains(val));

					} else {
						
						result = all.FindAll(x => x.tag == val);

					}
					break;
				case By.GameObjectType:
					throw new NotImplementedException();
				case By.ImageFileName:
					throw new NotImplementedException();      
				case By.Hierarchy:
				case By.Name:
					List<GameObject> matches = new List<GameObject>();
					if(isContains) {
						
						matches = all.FindAll(x => x.name.Contains(val));

					} else {
						
						matches = all.FindAll(x => x.name == val);

					}
					if(matches.Count > 1) {
						
						matches = matches.GetActiveAndVisibleObjectsInList();

					}
					result = matches;
					break;
			default:
				return null;
			}

			PostCommandCheck();
			if(result.Any())
				return result.First();
			return null;

		}
		public GameObject FindIn(List<Component> comps, By by, string val, bool isContains = true) {
			
			return FindIn(comps.ToGameObjectList(), by, val, isContains);

		}

		/// <summary>
		/// Methods for finding GameObjects by "Contains Component".
		/// </summary>
		public T Find<T>() {

			GameObject result = SceneMaster.GetObjectPool().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).First();
			return result == null ? default(T) : result.GetComponent<T>();

		}
		public T FindIn<T>(List<GameObject> findInThese) {

			GameObject result = findInThese.GetChildren().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).First();
			return result == null ? default(T) : result.GetComponent<T>();

		}
		public T FindIn<T>(GameObject findInThis) {

			GameObject result = findInThis.GetChildren().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).First();
			return result == null ? default(T) : result.GetComponent<T>();

		}
		public List<T> FindAll<T>() {

			return SceneMaster.GetObjectPool().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).ToComponenentList<T>();

		}
		public List<T> FindAllIn<T>(GameObject findInThis) {

			return findInThis.GetChildren().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).ToComponenentList<T>();

		}
		public List<T> FindAllIn<T>(List<GameObject> findInThese) {

			return findInThese.GetChildren().FindAll(c => c.GetComponent<T>() != null && IsActiveVisibleAndInteractable(c)).ToComponenentList<T>();

		}

		/// <summary>
		/// Find single object by attribute type and provided value.
		/// </summary>
		public GameObject Find(By by, string val, bool isContains = true) {

			PreCommandCheck();

			List<GameObject> allObjects = SceneMaster.GetObjectPool();
			List<GameObject> results = new List<GameObject> ();
			switch(by) {
				case By.TextContent:
					results = GetByTextContent(val, isContains);
					break;
				case By.TagName:
					return GameObject.FindWithTag(val);
				case By.GameObjectType:
					throw new NotImplementedException();
				case By.ImageFileName:
					throw new NotImplementedException();      
				case By.Hierarchy:
				case By.Name:
					results = allObjects.FindAll(x => { 
						if(isContains) {
							
							return x.name.ContainsOrEquals(val);

						} else {
							
							return x.name == val;

						}
					});
					if(!results.Any()) {
						GameObject one = GameObject.Find(val);
						if(one != null) {
							
							return one;

						}
					}
					break;
				default:
					return null;
			}

			if(results.Any()) {

				if(results.Count > 1) {

					return results.GetActiveAndVisibleObjectsInList().First();

				} else {
					
					return results.First();

				}

			}

			PostCommandCheck();
			return null;

		}
		/// <summary>
		/// Find single object by attribute type and provided value.
		/// </summary>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
		public List<GameObject> FindAll(By by, string val, bool isContains = true) {

			PreCommandCheck();

			List<GameObject> allObjects = SceneMaster.GetObjectPool();
			List<GameObject> results = new List<GameObject> ();

			for(int x = 0; x < allObjects.Count; x++) {
				
				switch(by) {
					case By.TextContent:
						return GetByTextContent(val, isContains, allObjects);
					case By.TagName:
						return GameObject.FindGameObjectsWithTag(val).ToList();
					case By.GameObjectType:
						throw new NotImplementedException();
					case By.ImageFileName:
						throw new NotImplementedException();      
					case By.Hierarchy:
					case By.Name:
						if(isContains ? allObjects[x].name.Contains(val) : allObjects[x].name == val) {
							
							results.Add(allObjects[x]);

						}
						break;
					default:
						return null;
					}

			}

			PostCommandCheck();
			return results;

		}

		/// <summary>
		/// Find all objects by attribute type and provided value. To find all objects under a specific object, use Q.help.Children instead.
		/// </summary>
		/// <returns>All matching objects.</returns>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
		/// <param name="isContains">If set to <c>true</c> is contains.</param>
		public List<GameObject> FindAllIn(GameObject obj, By by, string val, bool isContains = true) {

			PreCommandCheck();

			if(obj == null) {

				if(isTry) {

					Q.assert.StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Null GameObject provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				} else {

					Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Null GameObject provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				}
				return new List<GameObject>();

			}

			List<GameObject> results = new List<GameObject>();
			List<GameObject> all = obj.GetChildren();
			switch(by) {
				case By.TextContent:
					results = GetByTextContent(val, isContains, all);
					break;
				case By.TagName:
					if(isContains) {
					
						results = all.FindAll(x => x.tag.Contains(val));

					} else {
					
						results = all.FindAll(x => x.tag == val);

					}
					break;
				case By.Hierarchy:
				case By.Name:
					if(isContains) {
					
						results = all.FindAll(x => x.name.Contains(val));

					} else {
					
						results = all.FindAll(x => x.name == val);

					}
					break;
				default:
					return null;
			}

			PostCommandCheck();
			if(results.Any())
				return results;
			return new List<GameObject>();

		}

		public List<GameObject> FindAllIn(Component obj, By by, string val, bool isContains = true) {

			return FindAllIn(obj == null ? null : obj.gameObject, by, val, isContains);

		}

		public List<GameObject> FindAllIn(List<GameObject> objs, By by, string val, bool isContains = true) {

			PreCommandCheck();

			if(!objs.Any()) {

				if(isTry) {

					Q.assert.StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				} else {

					Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val)));

				}
				return new List<GameObject>();

			}
			List<GameObject> results = new List<GameObject>();
			List<GameObject> all = new List<GameObject>(); 
			for(int i = 0; i < objs.Count; i++) {
				
				all.AddRange(objs[i].GetChildren());
				all.Add(objs[i]);

			}
			switch(by) {
				case By.TextContent:
					results = GetByTextContent(val, isContains, all);
					break;
				case By.TagName:
					if(isContains) {
						
						results = all.FindAll(x => x.tag.Contains(val));

					} else {
						
						results = all.FindAll(x => x.tag == val);

					}
					break;
				case By.Hierarchy:
				case By.Name:
					if(isContains) {
						
						results = all.FindAll(x => x.name.Contains(val));

					} else {
						
						results = all.FindAll(x => x.name == val);

					}
					break;
				default:
					return null;
			}

			PostCommandCheck();
			if(results.Any())
				return results;
			return new List<GameObject>();

		}
		public List<GameObject> FindAllIn(List<Component> comps, By by, string val, bool isContains = true) {
			
			return FindAllIn(comps.ToGameObjectList(), by, val, isContains);

		}

		#endregion 

		#region Interact With Object

		/// <summary>
		/// Select the supplied object using pointer handlers
		/// </summary>
		/// <param name="g">GameObject to click.</param>
		/// <param name="optionalOnFailMessage">Click fail message.</param>
		/// <param name="timeout">Timeout period where the driver will wait for the clickable object to be interactable.</param>
		/// <param name="clickInactive">Ignore that the clickable object is currently inactive.</param>
		public virtual IEnumerator Click(GameObject g, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout = TIMEOUT_DEFAULT, bool clickInactive = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));

			SelectTracer(g);
			GameObject errorToDismiss = Q.assert.AssertNoErrorPopups();
			if(g == null) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				} else {

					yield return StartCoroutine(Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				}
				yield break;


			} else {

				GameObject o = TryReturnFirstClickableObject(g);
				if(errorToDismiss != null) {

					o = errorToDismiss;

				}

				string assertion = string.Empty;
				if(optionalOnFailMessage != DEFAULT_ERROR_MESSAGE) {

					assertion = optionalOnFailMessage;

				} else {

					assertion = string.Format("Click {0}", g.name);

				}

				if(g.GetComponents<Component>().ToList().FindAll(x => x.GetType().Name.ToLower().ContainsOrEquals("collider")).Any()) {

					HandleColliderTrigger(g);

				} else {

					if(!clickInactive) {
						
						yield return StartCoroutine(WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(g), optionalOnFailMessage, timeout));

					}
					ExecuteEvents.Execute<IPointerClickHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
					ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
					ExecuteEvents.Execute<IPointerUpHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
					yield return StartCoroutine(WaitRealTime(WAIT_AFTER_CLICK));

				}
				yield return StartCoroutine(Q.assert.Pass(assertion));

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck(true);

		}
		public IEnumerator Click(GameObject g, By by, string findByValue, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT, bool clickInactive = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));

			SelectTracer(g);
			GameObject errorToDismiss = Q.assert.AssertNoErrorPopups();
			if(g == null) {

				if(!isTry) {
					
					if(isTry) {

						yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

					} else {

						yield return StartCoroutine(Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

					}
					yield break;

				}

			} else {

				GameObject o = TryReturnFirstClickableObject(g);
				if(errorToDismiss != null) {

					o = errorToDismiss;

				}

				string assertion = string.Empty;
				if(optionalOnFailMessage != DEFAULT_ERROR_MESSAGE) {

					assertion = optionalOnFailMessage;

				} else {

					assertion = string.Format("Click {0}", g.name);

				}

				if(g.GetComponents<Component>().ToList().FindAll(x => x.GetType().Name.ToLower().ContainsOrEquals("collider")).Any()) {

					HandleColliderTrigger(g);

				} else {
					
					if(!clickInactive) {

						yield return StartCoroutine(WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(g), optionalOnFailMessage, timeout));

					}
					ExecuteEvents.Execute<IPointerClickHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
					ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
					ExecuteEvents.Execute<IPointerUpHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
					yield return StartCoroutine(WaitRealTime(WAIT_AFTER_CLICK));

				}
				yield return StartCoroutine(Q.assert.Pass(assertion));

			}

			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck(true);

		}
		public IEnumerator Click(Component c, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout = TIMEOUT_DEFAULT, bool clickInactive = false) {

			yield return StartCoroutine(Click(c == null ? null : c.gameObject, optionalOnFailMessage, timeout));

		}
		public IEnumerator Click(Component c, By by, string findByValue, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT, bool clickInactive = false) {

			yield return StartCoroutine(Click(c == null ? null : c.gameObject, by, findByValue, optionalOnFailMessage, timeout));

		}
		public void HandleColliderTrigger(GameObject g) {

			Collider collider = g.GetComponent<Collider>();
			if(collider != null) {

				if(collider.isTrigger) {
					
					g.GetComponent<Collider>().SendMessage("OnTriggerEnter", g.GetComponent<Collider>());

				} else {
					
					g.GetComponent<Collider>().SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}
				return;

			} 

			Collider2D collider2d = g.GetComponent<Collider2D>();
			if(collider2d != null) {

				if(collider2d.isTrigger) {
					
					collider2d.SendMessage("OnTriggerEnter", g.GetComponent<Collider2D>());

				} else {
					
					collider2d.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}
				return;

			} 

		}

		/// <summary>
		/// Buttons may have a Long-hold script that expects the button to be held down for a certain period of time to launch the button event.
		/// </summary>
		public IEnumerator ClickAndHold(GameObject g, float timeToHoldSimulateTouch, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT, bool clickInactive = false) {

			PreCommandCheck();

			if (g == null && !isTry) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				} else {

					yield return StartCoroutine(Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				}

			}
			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			SelectTracer(g);
			Q.assert.AssertNoErrorPopups();

			string assertion = string.Empty;
			if(optionalOnFailMessage != DEFAULT_ERROR_MESSAGE) {

				assertion = optionalOnFailMessage;

			} else {

				assertion = string.Format("Click and Hold {0}", g.name);

			}

			GameObject o = TryReturnFirstClickableObject(g);
			yield return StartCoroutine(WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(g), optionalOnFailMessage, timeout));
			ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
			WaitRealTime(timeToHoldSimulateTouch);
			ExecuteEvents.Execute<IPointerUpHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			yield return StartCoroutine(Q.assert.Pass(assertion));
			PostCommandCheck();

		}

		public IEnumerator ClickAndHold(Component c, float timeToHoldSimulateTouch, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT) {

			yield return StartCoroutine(ClickAndHold(TryReturnFirstClickableObject(c.gameObject), timeToHoldSimulateTouch, optionalOnFailMessage, timeout));

		}

		private GameObject TryReturnFirstClickableObject(GameObject obj) {

			List<GameObject> objs = new List<GameObject> { obj };
			objs.AddRange(obj.GetChildren());
			for(int x = 0; x < objs.Count; x++) {
				
				if(objs[x] != null && IsFirstClickableObject(objs[x])) {
					
					return objs[x];

				}

			}
			//None returned, assume object has custom click script, or no click scripts were found.
			return obj;

		}

		/// <summary>
		/// Returns true if the game object is something we want to click on.
		/// </summary>
		/// <param name="obj">The game object to consider clicking.</param>
		/// <returns>true if the game object is something we want to click on.</returns>
		protected virtual bool IsFirstClickableObject(GameObject obj) {

			return obj.GetComponent<Button>() != null || obj.GetComponent<Toggle>() != null;

		}

		public IEnumerator ClickAndDrag(Component c, Vector2 releaseDragAt, float dragTime = 1f, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE) {

			yield return StartCoroutine(ClickAndDrag(TryReturnFirstClickableObject(c.gameObject), releaseDragAt, dragTime, optionalOnFailMessage));

		}

		public IEnumerator ClickAndDrag(GameObject g, Vector2 releaseDragAt, float dragTime = 1f, string optionalOnFailMessage = DEFAULT_ERROR_MESSAGE) {

			PreCommandCheck();

			if (g == null && !isTry) {

				if(isTry) {

					yield return StartCoroutine(Q.assert.Quiet.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				} else {

					yield return StartCoroutine(Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", optionalOnFailMessage)));

				}

			}

			//Set event data positions at initial click location.
			PointerEventData data = new PointerEventData(EventSystem.current);
			data.position = data.pressPosition = g.transform.position;

			float currentX = g.transform.position.x;
			float currentY = g.transform.position.y;
			float distanceX = releaseDragAt.x - g.transform.position.x;
			float distanceY = releaseDragAt.y - g.transform.position.y;

			//Distance traveled between IDragHandler updates should be the percentage of drag time that the interval represents, multiplied by the drag distance. This generates the smoothest drag possible given the supplied arguments.
			//Decrease the time represented by UPDATE_DRAG_INTERVAL to cause more frequent IDragHandler updates, and thus a smoother drag (at the possible cost of worse game performance during command execution).
			float distancePerUpdateX = distanceX * (UPDATE_DRAG_INTERVAL / dragTime);
			float distancePerUpdateY = distanceY * (UPDATE_DRAG_INTERVAL / dragTime);

			//Select Initial Position
			ExecuteEvents.Execute<IPointerDownHandler>(g, data, ExecuteEvents.pointerDownHandler);
			yield return StartCoroutine(Q.driver.WaitRealTime(0.1f));

			//Drag distance to final position over time for a smoother looking drag.
			while(distanceX > 0 && distanceY > 0) {

				currentX += currentX > 0 ? distancePerUpdateX : 0;
				currentY += currentY > 0 ? distancePerUpdateY : 0;
				distanceX -= distancePerUpdateX;
				distanceY -= distancePerUpdateY;
				data.position = data.pressPosition = new Vector2(currentX, currentY);

				ExecuteEvents.Execute<IDragHandler>(g, data, ExecuteEvents.dragHandler);

				yield return StartCoroutine(Q.driver.WaitRealTime(UPDATE_DRAG_INTERVAL));

			}

			string assertion = string.Empty;
			if(optionalOnFailMessage != DEFAULT_ERROR_MESSAGE) {

				assertion = optionalOnFailMessage;

			} else {

				assertion = string.Format("Click and Drag {0}", g.name);

			}

			data.position = data.pressPosition = releaseDragAt;
			ExecuteEvents.Execute<IPointerUpHandler>(g, data, ExecuteEvents.pointerUpHandler);

			yield return StartCoroutine(Q.assert.Pass(assertion));
			PostCommandCheck();

		}

		#endregion

		#region Wait For

		/// <summary>
		/// Waits for a single value to not be null or default. Waits for a list of values to return more than 0. Waits for object(s) to be active, visible, and interactable.
		/// </summary>
		/// <param name="propertyExpression">Lamba expression representing the check you wish to perform with each iteration of the loop. This is syntactically as simple as "() => SomeCondition && SomeOtherCondition", for example.</param>
		public IEnumerator WaitFor(Func<bool> condition, string optionalOnFailMessage = "", float timeout = TIMEOUT_DEFAULT, params int[] testCaseIds) {

			PreCommandCheck();

			isDeepDive = true;
			float timer = 0;
			while(!condition.Invoke() && timer <= timeout) {

				yield return StartCoroutine(WaitRealTime(1f));
				timer++;

			}
			if(timer > timeout) {

				if(!isTry) {

					yield return StartCoroutine(Q.assert.Fail(string.IsNullOrEmpty(optionalOnFailMessage) ? "Conditional wait (WaitFor) timed out." : optionalOnFailMessage));

				}

			}
			isDeepDive = false;
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			if(!string.IsNullOrEmpty(optionalOnFailMessage)) {

                yield return StartCoroutine(Q.assert.Pass(optionalOnFailMessage));

			}

			if(testCaseIds.Length > 0) {

				Q.assert.MarkTestRailsTestCase(timer > timeout, testCaseIds);

			}

			PostCommandCheck();
			yield return null;

		}

		#endregion

		/// <summary>
		/// Waits for real time rather than game time.
		/// </summary>
		/// <returns>The real time.</returns>
		/// <param name="waitTime">Wait time.</param>
		public IEnumerator WaitRealTime(float waitTime) {

			PreCommandCheck();

			float start = Time.realtimeSinceStartup;
			while(Time.realtimeSinceStartup < start + waitTime) {

				yield return null;

			}

			PostCommandCheck();
			yield return null;

		}

		private List<GameObject> GetByContainsComponent(string componentName, List<GameObject> findInObjs = null) { 

			List<GameObject> matches = new List<GameObject>(); 
			for(int i = 0; i < findInObjs.Count; i++) {
				
				List<GameObject> children = findInObjs[i].GetChildren();
				for(int x = 0; x < children.Count; x++) {
					
					if(children[x].GetComponent(componentName) != null) {
						
						matches.Add(children[x]);

					}

				}
				if(findInObjs[i].GetComponent(componentName) != null) {
					
					matches.Add(findInObjs[i]);

				}

			}
			return matches;

		}

		/// <summary>
		/// Find objects that contain the Text value provided.
		/// </summary>
		/// <returns>All matching objects.</returns>
		/// <param name="val">Value.</param>
		/// <param name="isContains">If set to <c>true</c> is contains.</param>
		private List<GameObject> GetByTextContent(string textVal, bool isContains, List<GameObject> findInObjs = null) {

			string textValLower = textVal.ToLower();
			List<GameObject> matches = new List<GameObject>();
			findInObjs = findInObjs == null ? SceneMaster.GetObjectPool() : findInObjs;

			for(int fio = 0; fio < findInObjs.Count; fio++) {

				Text txt = findInObjs[fio].gameObject.GetComponent<Text>();
				if(txt != null) {
					
					if(isContains) {
						
						if(txt.text.ToLower().Contains(textValLower)) {
							
							matches.Add(findInObjs[fio]);
							continue;

						}

					} else {
						
						if(txt.text.ToLower() == textValLower) {
							
							matches.Add(findInObjs[fio]);
							continue;

						}

					}

				} else {
					
					for(int a = 0; a < GameMaster.AdditionalTextAssets.Count; a++) {

						KeyValuePair<Type,string> asset = GameMaster.AdditionalTextAssets[a];
						if(findInObjs[fio].GetComponent(asset.Key) != null) {
							
							Type type = findInObjs[fio].GetComponent(GameMaster.AdditionalTextAssets[a].Key).GetType();
							if(type != null) {
								
								string textContent = type.GetProperty(asset.Value).GetValue(findInObjs[fio].GetComponent(GameMaster.AdditionalTextAssets[a].Key), null).ToString();
								if(isContains) {
									
									if(textContent.ToLower().Contains(textValLower)) {
										
										matches.Add(findInObjs[fio]);
										continue;

									}

								} else {
									
									if(textContent.ToLower() == textValLower) {
										
										matches.Add(findInObjs[fio]);
										continue;
									}

								}

							}

						}

					}

				}

			}
			return matches;

		}

		/// <summary>
		/// Get all active objects that exist in the scene.
		/// </summary>
		/// <returns>All active objects.</returns>
		private List<GameObject> GetAllActiveObjects() {

			List<GameObject> allActiveObjects = new List<GameObject>();
			GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
			for(int i = 0; i < allObjects.Length; i++) {

				if(allObjects[i].activeInHierarchy) {
					
					allActiveObjects.Add(allObjects[i]);

				}

			}
			return allActiveObjects;

		}

	}

}
