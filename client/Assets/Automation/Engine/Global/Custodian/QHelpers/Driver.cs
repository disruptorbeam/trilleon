using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrilleonAutomation {

	public class Driver : MonoBehaviour {

		public const float TIME_PER_KEY_TO_SEND = 0.1f; //Default amount of time added to total SendKeys time for each key requested.
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

		void Start() {

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
			if(AutomationRecorder.SelectionUpdatesHeirarchy) {

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
		public IEnumerator SendKeys(InputField field, string keysToSend){

			PreCommandCheck(true);

			SelectTracer(field.gameObject);
				
			if(field != null && keysToSend != null && field.isActiveAndEnabled) {

				AutoHud.UpdateMessage(string.Format("{0} SendKeys \"{1}\"", field.gameObject.name, keysToSend));
				Click(field.gameObject); //Send focus to input.

				char[] keys = keysToSend.ToCharArray();
				for(int x = 0; x < keys.Length; x++) {

					field.text = string.Format("{0}{1}", field.text, keys[x]);
					yield return StartCoroutine(WaitRealTime(TIME_PER_KEY_TO_SEND));

				}
				Click(field.gameObject.transform.parent.gameObject); //Remove focus from input.

				if(isTry) {
					
					Q.assert.Try.Pass(string.Format("Send Keys \"{0}\" to {1}.", keysToSend, field.gameObject.name));

				} else {

					Q.assert.Pass(string.Format("Send Keys \"{0}\" to {1}.", keysToSend, field.gameObject.name));

				}

			} else {

				if(isTry) {
					
					Q.assert.Try.Fail(string.Format("Cannot enter text in field {0}, as it is not available", field != null ? field.name : "[provided field is null or inactive]"));

				} else {

					Q.assert.Fail(string.Format("Cannot enter text in field {0}, as it is not available", field != null ? field.name : "[provided field is null or inactive]"));

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

					Q.assert.Try.Fail("Provided ScrollView GameObject is null.");

				} else {

					Q.assert.Fail("Provided ScrollView GameObject is null.");

				}
				yield break;

			}

			AutoHud.UpdateMessage(string.Format("{0} Scroll \"{1}\"", scrollRectObject.name, scrollDownwardOrToTheRight ? "Right" : "Left"));
			ScrollRect rect = scrollRectObject.GetComponent<ScrollRect>();
			if(rect == null && !isTry) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Provided GameObject \"{0}\" lacks a ScrollRect that would allow it to behave as a scroll view.", scrollRectObject.name));

				} else {

					Q.assert.Fail(string.Format("Provided GameObject \"{0}\" lacks a ScrollRect that would allow it to behave as a scroll view.", scrollRectObject.name));

				}
				yield break;

			}

			yield return StartCoroutine(Scroll(rect, durationOfScroll, scrollDownwardOrToTheRight, distanceToTravelBetweenZeroAndOne));

		}
		public IEnumerator Scroll(ScrollRect scrollRect, float durationOfScroll, bool scrollDownwardOrToTheRight = true, float distanceToTravelBetweenZeroAndOne = 1f) {

			if(scrollRect == null && !isTry) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Provided ScrollRect for \"{0}\" is null.", scrollRect.gameObject.name));

				} else {

					Q.assert.Try.Fail(string.Format("Provided ScrollRect for \"{0}\" is null.", scrollRect.gameObject.name));

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
		public bool IsActiveVisibleAndInteractable(GameObject g, bool checkComponents = false) {


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
		public bool IsActiveVisibleAndInteractable(List<Component> cs, bool checkComponents = false) {

			for(int x = 0; x < cs.Count; x++) {

				if(!IsActiveVisibleAndInteractable(cs[x].gameObject, checkComponents)) {

					return false;

				}

			}

			return true;

		}
		public bool IsActiveVisibleAndInteractable(Component c, bool checkComponents = false) {

			if(c == null) {

				return false;

			} else {

				return IsActiveVisibleAndInteractable(c.gameObject, checkComponents);

			}

		}
		public bool IsActiveVisibleAndInteractable(List<GameObject> gs, bool checkComponents = false) {

			for(int x = 0; x < gs.Count; x++) {

				if(!IsActiveVisibleAndInteractable(gs[x], checkComponents)) {

					return false;

				}

			}

			return true;

		}

		#endregion

		#region Find Objects

		/// <summary>
		/// Find single object by attribute type and provided value.
		/// </summary>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
		public GameObject FindIn(GameObject obj, By by, string val, bool isContains = true) {

			PreCommandCheck();

			if(obj == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Null GameObject provided to finder along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

				} else {

					Q.assert.Fail(string.Format("Null GameObject provided to finder along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

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

					Q.assert.Try.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

				} else {

					Q.assert.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

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
		/// <returns>The in.</returns>
		/// <param name="comps">Comps.</param>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
		/// <param name="isContains">If set to <c>true</c> is contains.</param>
		public GameObject Find<T>() {

			return SceneMaster.GetObjectPool().FindAll(c => c.GetComponent<T>() != null).First();

		}
		public GameObject FindIn<T>(List<GameObject> findInThese) {

			return findInThese.GetChildren().FindAll(c => c.GetComponent<T>() != null).First();

		}
		public GameObject FindIn<T>(GameObject findInThis) {

			return findInThis.GetChildren().FindAll(c => c.GetComponent<T>() != null).First();

		}
		public List<GameObject> FindAll<T>() {

			return SceneMaster.GetObjectPool().FindAll(c => c.GetComponent<T>() != null);

		}
		public List<GameObject> FindAllIn<T>(List<GameObject> findInThese) {

			return findInThese.GetChildren().FindAll(c => c.GetComponent<T>() != null);

		}
		public List<GameObject> FindAllIn<T>(GameObject findInThis) {

			return findInThis.GetChildren().FindAll(c => c.GetComponent<T>() != null);

		}

		/// <summary>
		/// Find single object by attribute type and provided value.
		/// </summary>
		/// <param name="by">By.</param>
		/// <param name="val">Value.</param>
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

				return results.First();

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

					Q.assert.Try.Fail(string.Format("Null GameObject provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

				} else {

					Q.assert.Fail(string.Format("Null GameObject provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

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

					Q.assert.Try.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

				} else {

					Q.assert.Fail(string.Format("Empty list provided to finder method along with By \"{0}\" and Search term was \"{1}\".", Enum.GetName(typeof(By), by), val));

				}
				return new List<GameObject>();

			}
			List<GameObject> results = new List<GameObject>();
			List<GameObject> all = new List<GameObject>(); 
			for(int i = 0; i < objs.Count; i++) {
				all.AddRange(objs[i].GetChildren());
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
		/// Select the supplied object using PointerClickHandler
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="clickFailMessage">Click fail message.</param>
		/// <param name="timeout">Timeout.</param>
      public IEnumerator Click(GameObject g, string clickFailMessage = DEFAULT_ERROR_MESSAGE, float timeout = TIMEOUT_DEFAULT  ) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));

			SelectTracer(g);
			GameObject errorToDismiss = Q.assert.AssertNoErrorPopups();
			if(g == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

				}
				yield break;


			} else {

				GameObject o = TryReturnFirstClickableObject(g);
				if(errorToDismiss != null) {

					o = errorToDismiss;

				}

				if(g.GetComponents<Component>().ToList().FindAll(x => x.GetType().Name.ToLower().ContainsOrEquals("collider")).Any()) {

					HandleColliderTrigger(g);

				} else {

					yield return StartCoroutine(WaitForCondition(o, Condition.IsActiveVisibleAndInteractable, string.Empty, clickFailMessage, timeout));
					ExecuteEvents.Execute<IPointerClickHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
					ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
					yield return StartCoroutine(WaitRealTime(WAIT_AFTER_CLICK));

				}

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck(true);

		}
		public IEnumerator Click(GameObject g, By by, string findByValue, string clickFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			SelectTracer(g);
			GameObject errorToDismiss = Q.assert.AssertNoErrorPopups();
			if(g == null) {

				if(!isTry) {
					
					if(isTry) {

						Q.assert.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

					} else {

						Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

					}
					yield break;

				}

			} else {

				GameObject o = TryReturnFirstClickableObject(g);
				if(errorToDismiss != null) {

					o = errorToDismiss;

				}


				if(g.GetComponents<Component>().ToList().FindAll(x => x.GetType().Name.ToLower().ContainsOrEquals("collider")).Any()) {

					HandleColliderTrigger(g);

				} else {

					yield return StartCoroutine(WaitForCondition(o, Condition.Exists, by, findByValue, clickFailMessage, timeout));
					ExecuteEvents.Execute<IPointerClickHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
					ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
					yield return StartCoroutine(WaitRealTime(WAIT_AFTER_CLICK));

				}

			}

			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck(true);

		}
      public IEnumerator Click(GameObject g, float timeout) {
         yield return StartCoroutine(Click(g, DEFAULT_ERROR_MESSAGE, timeout));
      }
              
      public IEnumerator Click(Component c ) {

         yield return StartCoroutine(Click(c == null ? null : c.gameObject, DEFAULT_ERROR_MESSAGE, TIMEOUT_DEFAULT));

      }
      public IEnumerator Click(Component c, string clickFailMessage ) {

         yield return StartCoroutine(Click(c == null ? null : c.gameObject, clickFailMessage, TIMEOUT_DEFAULT));

      }
      public IEnumerator Click(Component c, float timeout ) {

         yield return StartCoroutine(Click(c == null ? null : c.gameObject, DEFAULT_ERROR_MESSAGE, timeout));

      }
		public IEnumerator Click(Component c, By by, string findByValue, string clickFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT) {

			yield return StartCoroutine(Click(c == null ? null : c.gameObject, by, findByValue, clickFailMessage, timeout));

		}
		public void HandleColliderTrigger(GameObject g) {

			Collider collider = g.GetComponent<Collider>();
			if(collider != null) {

				if(collider.isTrigger) {
					
					g.GetComponent<Collider>().SendMessage("OnTriggerEnter", g.GetComponent<Collider>());

				} else {
					
					g.GetComponent<Collider>().SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

			BoxCollider box = g.GetComponent<BoxCollider>();
			if(box != null) {

				if(box.isTrigger) {
					
					g.GetComponent<BoxCollider>().SendMessage("OnTriggerEnter", g.GetComponent<BoxCollider>());

				} else {

					g.GetComponent<BoxCollider>().SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			}

			SphereCollider sphere = g.GetComponent<SphereCollider>();
			if(sphere != null) {

				if(sphere.isTrigger) {
					
					sphere.SendMessage("OnTriggerEnter", g.GetComponent<SphereCollider>());

				} else {
					
					sphere.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

			CapsuleCollider capsule = g.GetComponent<CapsuleCollider>();
			if(capsule != null) {

				if(capsule.isTrigger) {
					
					capsule.SendMessage("OnTriggerEnter", g.GetComponent<CapsuleCollider>());

				} else {
					
					capsule.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

			BoxCollider2D box2d = g.GetComponent<BoxCollider2D>();
			if(box2d != null) {

				if(box2d.isTrigger) {
					
					box2d.SendMessage("OnTriggerEnter", g.GetComponent<BoxCollider2D>());

				} else {
					
					box2d.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

			CircleCollider2D circle2d = g.GetComponent<CircleCollider2D>();
			if(circle2d != null) {

				if(circle2d.isTrigger) {
					
					circle2d.SendMessage("OnTriggerEnter", g.GetComponent<CircleCollider2D>());

				} else {
					
					circle2d.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

			Collider2D collider2d = g.GetComponent<Collider2D>();
			if(collider2d != null) {

				if(collider2d.isTrigger) {
					
					collider2d.SendMessage("OnTriggerEnter", g.GetComponent<Collider2D>());

				} else {
					
					collider2d.SendMessage("OnMouseDown", g.GetComponent<Collider>());

				}

			} 

		}

		/// <summary>
		/// Buttons may have a Long-hold script that expects the button to be held down for a certain period of time to launch the button event.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="timeToHoldSimulateTouch">Time to hold simulate touch.</param>
		public IEnumerator ClickAndHold(GameObject g, float timeToHoldSimulateTouch, string clickFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT) {

			PreCommandCheck();

			if (g == null && !isTry) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

				}

			}
			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			SelectTracer(g);
			Q.assert.AssertNoErrorPopups();
			GameObject o = TryReturnFirstClickableObject(g);
			yield return StartCoroutine(WaitForCondition(o, Condition.IsActiveVisibleAndInteractable, string.Empty, clickFailMessage, timeout));
			ExecuteEvents.Execute<IPointerDownHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
			WaitRealTime(timeToHoldSimulateTouch);
			ExecuteEvents.Execute<IPointerUpHandler>(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck();

		}

		public IEnumerator ClickAndHold(Component c, float timeToHoldSimulateTouch, string clickFailMessage = DEFAULT_ERROR_MESSAGE, float timeout =  TIMEOUT_DEFAULT) {

			yield return StartCoroutine(ClickAndHold(TryReturnFirstClickableObject(c.gameObject), timeToHoldSimulateTouch, clickFailMessage, timeout));

		}

		private GameObject TryReturnFirstClickableObject(GameObject obj) {

			List<GameObject> objs = new List<GameObject> { obj };
			objs.AddRange(obj.GetChildren());
			for(int x = 0; x < objs.Count; x++) {
				if(objs[x] != null && (objs[x].GetComponent<Button>() != null || objs[x].GetComponent<Toggle>() != null)) {
					return objs[x];
				}
			}
			//None returned, assume object has custom click script, or no click scripts were found.
			return obj;

		}

		public IEnumerator ClickAndDrag(Component c, Vector2 releaseDragAt, float dragTime = 1f, string clickFailMessage = DEFAULT_ERROR_MESSAGE) {

			yield return StartCoroutine(ClickAndDrag(TryReturnFirstClickableObject(c.gameObject), releaseDragAt, dragTime, clickFailMessage));

		}

		public IEnumerator ClickAndDrag(GameObject g, Vector2 releaseDragAt, float dragTime = 1f, string clickFailMessage = DEFAULT_ERROR_MESSAGE) {

			PreCommandCheck();

			if (g == null && !isTry) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null. Error Message: {0}", clickFailMessage));

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

			data.position = data.pressPosition = releaseDragAt;
			ExecuteEvents.Execute<IPointerUpHandler>(g, data, ExecuteEvents.pointerUpHandler);

			PostCommandCheck();

		}

		#endregion

		#region Wait For

		/// <summary>
		/// Waits for a single value to not be null or default. Waits for a list of values to return more than 0. Waits for object(s) to be active, visible, and interactable.
		/// </summary>
		/// <param name="propertyExpression">Lamba expression representing the check you wish to perform with each iteration of the loop. This is syntactically as simple as "() => SomeCondition && SomeOtherCondition", for example.</param>
		public IEnumerator WaitFor(Func<bool> condition, string optionalOnFailMessage = "", float timeout = TIMEOUT_DEFAULT) {

			PreCommandCheck();

			isDeepDive = true;
			float timer = 0;
			while(!condition.Invoke() && timer <= timeout) {

				yield return StartCoroutine(WaitRealTime(1f));
				timer++;

			}
			if(timer > timeout) {

				if(!isTry) {

					Q.assert.Fail(string.IsNullOrEmpty(optionalOnFailMessage) ? "Conditional wait (WaitFor) timed out." : optionalOnFailMessage);

				}

			}
			isDeepDive = false;
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

			PostCommandCheck();

		}

		#endregion

		#region Wait For Condition

		/// <summary>
		/// Wait for condition to be true before continuing test.
		/// </summary>
		/// <returns>The for condition.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="condition">Condition.</param>
		/// <param name="val">Value.</param>
		/// <param name="optionalOnFailMessage">Optional on fail message.</param>
		/// <param name="timeout">Timeout.</param>

		public IEnumerator WaitForCondition(GameObject go, Condition condition, string containsValue, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(go == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return _WaitForCondition(go, condition, containsValue, timeout, true, optionalOnFailMessage, By.Default, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator TryWaitForCondition(GameObject go, Condition condition, string containsValue, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(go == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return _WaitForCondition(go, condition, containsValue, timeout, false, optionalOnFailMessage, By.Default, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator WaitForCondition(GameObject go, Condition condition, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(go == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return _WaitForCondition(go, condition, string.Empty, timeout, true, optionalOnFailMessage, By.Default, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator WaitForCondition(Type testObjectClass, string parameterName, Condition condition, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			yield return _WaitForCondition(testObjectClass, parameterName, condition, timeout, true, optionalOnFailMessage, reverseCondition);

		}

		public IEnumerator WaitForCondition(GameObject go, Condition condition, By by, string containsVal, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(go == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return _WaitForCondition(go, condition, containsVal, timeout, true, optionalOnFailMessage, by, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator WaitForCondition(Condition condition, By by, string containsVal, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			yield return _WaitForCondition(condition, containsVal, timeout, true, optionalOnFailMessage, by, reverseCondition);

		}

		public IEnumerator WaitForCondition(By by, Condition condition, string containsVal, float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			yield return _WaitForCondition(new List<GameObject>(), condition, containsVal, timeout, true, string.Empty, by, reverseCondition);

		}
	
		public IEnumerator WaitForCondition(Component c, Condition condition, string containsValue, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(c == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return _WaitForCondition(c.gameObject, condition, containsValue, timeout, true, optionalOnFailMessage, By.Default, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator WaitForCondition(Component c, Condition condition, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(c == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return WaitForCondition(c.gameObject, condition, optionalOnFailMessage, timeout, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		public IEnumerator WaitForCondition(Component c, Condition condition, By by, string containsVal, string optionalOnFailMessage = "", float timeout =  TIMEOUT_DEFAULT, bool reverseCondition = false) {

			Q.assert.AssertNoErrorPopups();
			if(c == null) {

				if(isTry) {

					Q.assert.Try.Fail(string.Format("Supplied object was null."));

				} else {

					Q.assert.Fail(string.Format("Supplied object was null."));

				}

			} else {

				yield return WaitForCondition(c.gameObject, condition, containsVal, optionalOnFailMessage, timeout, reverseCondition);

			}
			yield return StartCoroutine(Q.game.WaitForNoLoadingIndicators());

		}

		/// <summary>
		/// Waits for condition.
		/// </summary>
		/// <returns>The for condition.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="cdtn">Cdtn.</param>
		/// <param name="val">Value.</param>
		/// <param name="to">To.</param>
		/// <param name="failIfTimeoutAndStillNotFound">If set to <c>true</c> fail if timeout and still not found.</param>
		/// <param name="optionalOnFailMessage">Optional on fail message.</param>
		private IEnumerator _WaitForCondition(GameObject obj, Condition cdtn, string val, float to, bool failIfTimeoutAndStillNotFound, string optionalOnFailMessage, By optionalBy = By.Default, bool reverseCondition = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			float timeSpent = 0f;
			bool condition = false;

			//Break if obj passed in is null.
			if(obj == null) {
				if(failIfTimeoutAndStillNotFound) {
					Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("Object passed in for wait is null") : optionalOnFailMessage);
				} else {
					yield return null;
				}
			}
			while(timeSpent <= to) {
				switch(cdtn) {
				case Condition.TextContains:
					Text txt = obj.gameObject.GetComponent<Text>();
					if(txt != null) {
						condition = txt.text.ToLower().Contains(val.ToLower());
						if(reverseCondition) {
							condition = !condition;
						}
					} 
					if(!condition) {
						for(int a = 0; a < GameMaster.AdditionalTextAssets.Count; a++) {
							Type type = obj.GetComponent(GameMaster.AdditionalTextAssets[a]).GetType();
							if(type != null) {
								string textContent = type.GetProperty("text").GetValue(obj.GetComponent(GameMaster.AdditionalTextAssets[a]),null).ToString().ToLower();
								condition = textContent.Contains(val.ToLower());
								if(condition) {
									break;
								}
							}
						}
					}
					break;
				case Condition.IsActiveVisibleAndInteractable:
					condition = IsActiveVisibleAndInteractable(obj);
					if(reverseCondition) {
						condition = !condition;
					}
					break;
				case Condition.Exists:
					condition = Q.driver.FindIn(obj, optionalBy, val) != null;
					if(reverseCondition) {
						condition = !condition;
					}
					break;
				default:
					new UnityException("Condition check not implemented!");
					break;
				}
				if(condition) {

					break;

				}
				yield return StartCoroutine(Q.driver.WaitRealTime(INTERLOOP_WAIT_TIME)); //Wait for an interloopWaitTime time period and begin loop again.
				timeSpent += INTERLOOP_WAIT_TIME;

			}

			//If we have broken out of the loop due to a timeout, throw an error IF we are not using a "TryWait" call.
			if(failIfTimeoutAndStillNotFound && !isTry) {

				Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			PostCommandCheck(true);

		}

		private IEnumerator _WaitForCondition(Condition cdtn, string val, float to, bool failIfTimeoutAndStillNotFound, string optionalOnFailMessage, By optionalBy = By.Default, bool reverseCondition = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			float timeSpent = 0f;
			bool condition = false;

			while(timeSpent <= to) {

				GameObject obj = Find(optionalBy, val);
				switch(cdtn) {
				case Condition.IsActiveVisibleAndInteractable:
					condition = IsActiveVisibleAndInteractable(obj);
					if(reverseCondition) {
						condition = !condition;
					}
					break;
				case Condition.Exists:
					condition = obj != null;
					break;
				case Condition.TextContains:
					if(obj == null) {
						continue;
					}
					condition = obj.GetComponent<Text>() != null && obj.GetComponent<Text>().text.ToLower().Contains(val.ToLower());
					if(!condition) {
						for(int a = 0; a < GameMaster.AdditionalTextAssets.Count; a++) {
							Type type = GameMaster.AdditionalTextAssets[a];
							if(type != null) {
								string textContent = type.GetProperty("text").GetValue(obj.GetComponent(GameMaster.AdditionalTextAssets[a]), null).ToString().ToLower();
								condition = textContent.Contains(val.ToLower());
								if(condition) {
									break;
								}
							}
						}
					}
					break;
				default:
					new UnityException("Condition check not implemented!");
					break;
				}

				if(condition) {

					yield return StartCoroutine(Q.driver.WaitRealTime(WAIT_AFTER_CLICK));
					break;

				}

				yield return StartCoroutine(Q.driver.WaitRealTime(INTERLOOP_WAIT_TIME)); //Wait for an interloopWaitTime time period and begin loop again.
				timeSpent += INTERLOOP_WAIT_TIME;

			}

			//If we have broken out of the loop due to a timeout, throw an error IF we are not using a "TryWait" call.
			if(failIfTimeoutAndStillNotFound && !isTry) {
				
				Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			PostCommandCheck(true);

		}

		private IEnumerator _WaitForCondition(Type testObjectClass, string parameterName, Condition cdtn, float to, bool failIfTimeoutAndStillNotFound, string optionalOnFailMessage = "", bool reverseCondition = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			float timeSpent = 0f;
			bool condition = false;

			List<PropertyInfo> properties = testObjectClass.GetProperties().ToList();
			PropertyInfo property = properties.FindAll(propInfo => propInfo.Name == parameterName).First();

			List<FieldInfo> fields = new List<FieldInfo>();
			FieldInfo field = null;

			if(property == null){
				fields = testObjectClass.GetFields().ToList();
				field = fields.FindAll(fieldInfo => fieldInfo.Name == parameterName).First();
			}

			if(failIfTimeoutAndStillNotFound) {
				Q.assert.IsTrue(field != null || property != null, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}.{1}", testObjectClass, parameterName) : optionalOnFailMessage);
			}

			if(field != null || property != null) {
				while(timeSpent <= to) {

					IEnumerable result = null;

					switch(cdtn) {
					case Condition.Any:
						if(property == null) {
							result = field.GetValue(null) as IEnumerable;
						} else {
							result = property.GetValue(null, null) as IEnumerable;
						}
						ICollection collectionBase = result as ICollection;
						if(collectionBase != null) {
							condition = collectionBase.Count > 0;
						}
						break;
					}

					if(condition) {

						yield return StartCoroutine(Q.driver.WaitRealTime(WAIT_AFTER_CLICK));
						break;

					}
					yield return StartCoroutine(Q.driver.WaitRealTime(INTERLOOP_WAIT_TIME)); //Wait for an interloopWaitTime time period and begin loop again.
					timeSpent += INTERLOOP_WAIT_TIME;

				}

				//If we have broken out of the loop due to a timeout, throw an error IF we are not using a "TryWait" call.
				if(failIfTimeoutAndStillNotFound && !isTry) {

					if(isTry) {

						Q.assert.Try.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}.{1}", testObjectClass, parameterName) : optionalOnFailMessage);

					} else {

						Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}.{1}", testObjectClass, parameterName) : optionalOnFailMessage);
				
					}
					yield break;

				}

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			PostCommandCheck(true);

		}

		private IEnumerator _WaitForCondition(List<GameObject> objs, Condition cdtn, string val, float to, bool failIfTimeoutAndStillNotFound, string optionalOnFailMessage, By optionalBy = By.Default, bool reverseCondition = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			float timeSpent = 0f;
			bool condition = false;

			//Break if obj passed in is null.
			if(objs == null || objs.Count == 0) {
				if(failIfTimeoutAndStillNotFound) {

					if(isTry) {

						Q.assert.Try.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("Object passed in for wait is null") : optionalOnFailMessage);

					} else {

						Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("Object passed in for wait is null") : optionalOnFailMessage);

					}
					yield break;

				} else {
					
					yield return null;

				}
			}
			//Continuously search for condition to be true as long as timeout has not been reached, and condition has not been met.
			while(timeSpent <= to) {
				switch(cdtn) {
				case Condition.TextContains:
					condition =objs.GetChildren().FindAll(x => x.GetComponent<Text>() != null).FindAll(x => x.GetComponent<Text>().text.ToLower().Contains(val.ToLower())).Any();
					if(!condition) {
						for(int a = 0; a < GameMaster.AdditionalTextAssets.Count; a++) {
							List<Type> types = objs.Transmute(GameMaster.AdditionalTextAssets[a]);

							if(types.Any()) {
								for(int t = 0; t < types.Count; t++) {
									for(int gos = 0; gos < objs.Count; gos++) {
										string textContent = types[t].GetProperty("text").GetValue(objs[gos].GetComponent(GameMaster.AdditionalTextAssets[a]), null).ToString().ToLower();
										condition = textContent.Contains(val.ToLower());
										if(condition) {
											break;
										}
									}
								}
							}
						}
					}
					break;
				case Condition.Exists:
					condition = objs.Any() ? Q.driver.Find(optionalBy, val) != null : Q.driver.FindIn(objs, optionalBy, val) != null;
					if(reverseCondition) {
						condition = !condition;
					}
					break;
				default:
					new UnityException("Condition check not implemented!");
					break;
				}

				if(condition) {

					yield return StartCoroutine(Q.driver.WaitRealTime(WAIT_AFTER_CLICK));
					yield break;

				}

				yield return StartCoroutine(Q.driver.WaitRealTime(INTERLOOP_WAIT_TIME)); //Wait for an interloopWaitTime time period and begin loop again.
				timeSpent += INTERLOOP_WAIT_TIME;
			}

			//If we have broken out of the loop due to a timeout, throw an error IF we are not using a "TryWait" call.
			if(failIfTimeoutAndStillNotFound && !isTry) {

				if(isTry) {

					Q.assert.Try.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

				} else {

					Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

				}

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			PostCommandCheck(true);

		}

		private IEnumerator _WaitForCondition(Button obj, Condition cdtn, string val, float to, bool failIfTimeoutAndStillNotFound, string optionalOnFailMessage, By optionalBy = By.Default, bool reverseCondition = false) {

			PreCommandCheck(true);

			yield return StartCoroutine(WaitRealTime(Assert.ScreenshotRequestWaitTime));
			float timeSpent = 0f;
			bool condition = false;

			//Break if obj passed in is null.
			if(obj == null) {
				if(failIfTimeoutAndStillNotFound) {

					if(isTry) {

						Q.assert.Try.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("Object passed in for wait is null") : optionalOnFailMessage);

					} else {

						Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("Object passed in for wait is null") : optionalOnFailMessage);

					}

				} else {
					yield return null;
				}
			}
			//Continuously search for condition to be true as long as timeout has not been reached, and condition has not been met.
			while(timeSpent <= to) {
				switch(cdtn) {
				case Condition.TextContains:
					Text txt = obj.gameObject.GetComponent<Text>();
					if(txt != null) {
						condition = obj.gameObject.GetComponent<Text>().text.ToLower().Contains(val.ToLower());
						if(reverseCondition) {
							condition = !condition;
						}
					}
					break;
				case Condition.IsActiveVisibleAndInteractable:
					condition = IsActiveVisibleAndInteractable(obj);
					if(reverseCondition) {
						condition = !condition;
					}
					break;
				default:
					new UnityException("Condition check not implemented!");
					break;
				}
				if(condition) {

					yield return StartCoroutine(Q.driver.WaitRealTime(WAIT_AFTER_CLICK));
					break;

				}
				yield return StartCoroutine(Q.driver.WaitRealTime(INTERLOOP_WAIT_TIME)); //Wait for an interloopWaitTime time period and begin loop again.
				timeSpent += INTERLOOP_WAIT_TIME;
			}

			//If we have broken out of the loop due to a timeout, throw an error IF we are not using a "TryWait" call.
			if(failIfTimeoutAndStillNotFound && !isTry) {

				if(isTry) {

					Q.assert.Try.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

				} else {

					Q.assert.IsTrue(condition, string.IsNullOrEmpty(optionalOnFailMessage) ? string.Format("No object with value found: {0}", val) : optionalOnFailMessage);

				}
				yield break;

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

			PostCommandCheck(true);

		}
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

		}

		#endregion

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
			findInObjs = findInObjs == null ? new List<GameObject>() : findInObjs;

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
						if(findInObjs[fio].GetComponent(GameMaster.AdditionalTextAssets[a]) != null) {
							Type type = findInObjs[fio].GetComponent(GameMaster.AdditionalTextAssets[a]).GetType();
							if(type != null) {
								string textContent = type.GetProperty("text").GetValue(findInObjs[fio].GetComponent(GameMaster.AdditionalTextAssets[a]), null).ToString();
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