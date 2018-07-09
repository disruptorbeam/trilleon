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
using UnityEngine.UI;
using System.Collections;

namespace TrilleonAutomation {

	//TODO: NOT IMPLEMENTED. Possibly to be scrapped, or functionality folded into CommandConsole.
	public class AutoHud : MonoBehaviour {

		public const string GAME_OBJECT_NAME = "AutoHud";
		const int DEFAULT_MESSAGE_DURATION = 100;

		public bool IsSet { get; set; }
		public AutoHudAnchor PositionOnScreen {
			get { 
				return _positionOnScreen;
			}
			set { 

				_positionOnScreen = value;

				switch(value) {
				case AutoHudAnchor.MiddleLeft:

					break;
				case AutoHudAnchor.MiddleRight:

					break;
				case AutoHudAnchor.TopLeft:

					break;
				case AutoHudAnchor.TopMiddle:

					break;
				case AutoHudAnchor.TopRight:

					break;
				case AutoHudAnchor.BottomLeft:

					break;
				case AutoHudAnchor.BottomMiddle:
					positionHud.anchorMax = new Vector2(0.5f, 0);
					positionHud.anchorMin = new Vector2(0.5f, 0);
					positionHud.pivot = new Vector2(0.5f, 0.5f);
					break;
				case AutoHudAnchor.BottomRight:

					break;
				}

			}
		}
		public AutoHudAnchor _positionOnScreen;

		Text Text {
			get { 
				if(_text == null) {
					GameObject textObj = TextObject;
					if(textObj != null) {
						_text = textObj.GetComponent<Text>();
						return _text;
					}
					return null;
				}
				return _text;
			}
			set { 
				_text = value;
			}
		}
		Text _text = null;

		CanvasGroup TextCanvasGroup {
			get { 
				if(_textCanvasGroup == null) {
					_textCanvasGroup = this.GetComponent<CanvasGroup>();
					return _textCanvasGroup;
				}
				return _textCanvasGroup;
			}
			set { 
				_textCanvasGroup = value;
			}
		}
		CanvasGroup _textCanvasGroup = null;

        CanvasScaler CanvasScaler {
            get {
                if(_canvasScaler == null) {
                    _canvasScaler = this.GetComponent<CanvasScaler>();
                    return _canvasScaler;
                }
                return _canvasScaler;
            }
            set {
                _canvasScaler = value;
            }
        }
        CanvasScaler _canvasScaler = null;

		public GameObject AutoHudBackground {
			get {
				if(_autoHudBackground == null) {
					_autoHudBackground = Q.driver.FindIn(this.GetChildren(), By.Name, GAME_OBJECT_NAME);
					return _autoHudBackground;
				}
				return _autoHudBackground;
			}
			private set{
				_autoHudBackground = value;
			}
		}
		public GameObject _autoHudBackground;

		public GameObject TextObject {
			get {
				if(_textObject == null) {
					_textObject = Q.driver.FindIn(gameObject, By.Name, "Text");
					return _textObject;
				}
				return _textObject;
			}
			private set{
				_textObject = value;
			}
		}
		public GameObject _textObject;

		RectTransform positionHud = new RectTransform();

		void Start() {
            
            if(!Application.isEditor) {

                return;

            }
			//Create AutoHud object.
			GetComponent<RectTransform>().position = new Vector3(0, 40, 0);

			//Put RenderMode in Screen Space.
			Canvas autoHudCanvas = gameObject.AddComponent<Canvas>();
			autoHudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

			//Do not block raycasts.
			CanvasGroup autoHudCanvasGroup = gameObject.AddComponent<CanvasGroup>();
			autoHudCanvasGroup.blocksRaycasts = false;

			//Position the HUD in the requested spot on screen.
			RectTransform autoHudRect = this.GetComponent<RectTransform>();
			autoHudRect.position = new Vector3(0, 0, 0);

			AutoHudBackground = new GameObject("Bg", typeof(RectTransform));
			//Set this object as a direct child of the AutoHud.
			AutoHudBackground.transform.SetParent(transform);

			positionHud = AutoHudBackground.GetComponent<RectTransform>();
            positionHud.sizeDelta = new Vector2(Screen.width * 0.75f, 75);
			positionHud.position = new Vector3(0, 40, 0);

			//Add region for text to appear.
			TextObject = new GameObject("Text", typeof(RectTransform));
			TextObject.AddComponent<Text>();
			//Set this object as a direct child of the Background object.
			TextObject.transform.SetParent(AutoHudBackground.transform);
			TextObject.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);

			//Middle Bottom TODO: GET PREFERENCE
			PositionOnScreen = AutoHudAnchor.BottomMiddle;

			//Give background to HUD text area.
			Image bg = AutoHudBackground.AddComponent<Image>();
			bg.color = new Color32(120, 120, 120, 110);
			bg.raycastTarget = false;

			//Set Initial Text.
			Text.raycastTarget = false;
			Text.color = Color.white;
			Text.fontSize = 25;
			Text.alignment = TextAnchor.MiddleCenter;
			Text.text = "Initialized";
            Text.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((Screen.width * 0.75f) - 20f, 75);
			Text.transform.localPosition = new Vector3(0, 0, 0);
			Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			Text.font = ArialFont;
			TextCanvasGroup = Text.gameObject.AddComponent<CanvasGroup>();
			TextCanvasGroup.alpha = 1f;

            CanvasScaler = gameObject.AddComponent<CanvasScaler>();
            CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CanvasScaler.referenceResolution = new Vector2(768, 1024);
			IsSet = true;

		}

		public static void UpdateMessage(string message, int duration = DEFAULT_MESSAGE_DURATION) {

			#if UNITY_EDITOR
            if(AutomationMaster.AutoHud != null) {

                AutomationMaster.AutoHud.SetMessage(message, duration);

			}
			#endif

		}

		void SetMessage(string message, int duration = DEFAULT_MESSAGE_DURATION) {

			StopCoroutine("FadeOut"); //Stop existing fade out.
			if(Text != null) {
                
				Text.text = message;
				TextCanvasGroup.alpha = 1f;

			}

		}

	}

	public enum AutoHudAnchor {
		MiddleLeft,
		MiddleRight,
		TopLeft,
		TopMiddle,
		TopRight,
		BottomLeft,
		BottomMiddle,
		BottomRight
	};

}
