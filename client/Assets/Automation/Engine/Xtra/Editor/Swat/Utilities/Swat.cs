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
using System.Collections.Generic;
using System;
using System.Reflection;

namespace TrilleonAutomation {

	/// <summary>
	/// Single Window Accessability Template (SWAT).
	/// Used to create an editor window logically grouping several sub windows.
	/// In the spirit of the Single Page Application (SPA) model.
	/// </summary>
	public class Swat : EditorWindow {

		public delegate void SwatDelegate();

		public const string MOVEUP = "▲";
		public const string MOVEDOWN = "▼";

		public TabDetails SelectedTab { get; private set; }
		public TabDetails LastSelectedTab { get; private set; }

		public class SwatControlError {

			public Vector2 ErrorPosition { get; set; }
			public string ErrorControlID { get; set; }
			public string ErrorMessage { get; set; }
			public float MessageHeight(float windowWidth) {

				float lines = 1;
				float messageWidth = DetermineRectWidthBasedOnLengthOfToolTip(ErrorMessage);

				if(windowWidth / 2 - messageWidth <= 0) {

					lines = (messageWidth / (windowWidth - 10));

					//Handle large words in tooltip that force wrapping and increase number of lines needed for display.
					float spacesInTooltip = ErrorMessage.Split(' ').Length;
					float expectedSpaces = ErrorMessage.Length / FOR_EVERY_NUMBER_CHARACTERS_EXPECT_ONE_SPACE;
					float addToLineTotal = (expectedSpaces / spacesInTooltip) - 1;

					if(addToLineTotal > 0) {

						lines += addToLineTotal;

					}
					lines = (float)Math.Ceiling(lines);

				}
				return lines * TOOLTIP_HEIGHT_MULTIPLIER;

			}

		}
		public List<SwatControlError> Errors { 
			get { 
				return _errors;
			}
			private set { 
				_errors = value;
			}
		}
		List<SwatControlError> _errors = new List<SwatControlError>();

		public static DockNextTo DockNextTo { 
			get { 
				switch(Customizer.Self.GetString("default_dock_nexus_next_to")) {
					case "Float":
						return DockNextTo.Float;
                    case "Hierarchy":
						return global::DockNextTo.Hierarchy;
                    case "GameView":
						return global::DockNextTo.GameView;
                    case "SceneView":
						return global::DockNextTo.SceneView;
                    case "Console":
						return global::DockNextTo.Console;
					case "Inspector":
					default:
						return global::DockNextTo.Inspector; //Default dock location.
				}
			}
		}

		/// <summary>
		/// Window tabs that will be displayed at the top of the Editor Window.
		/// A priority ID that is a negative value indicates that the window is hidden, and no tab buttons should appear. Only direct references can open these windows.
		/// </summary>
		/// <value> The Tab ID is the Key, and the value is a list of tab name values based on the rendered sizes, along with the colors and font sizes of each tab size and its content.</value>
		public List<TabDetails> SwatWindows { 
			get { 
				return _swatWindows;
			}
			private set {
				_swatWindows = value;
			}
		}
		private List<TabDetails> _swatWindows = new List<TabDetails>();

		/// <summary>
		/// Window tabs that will be displayed at the top of the Editor Window.
		/// </summary>
		/// <value> The Tab ID is the Key, and the value is a list of tab name values based on the rendered sizes, along with the colors and font sizes of each tab size and its content.</value>
		public List<SwatPopup> SwatPopups { 
			get { 
				return _swatPopups;
			}
			private set {
				_swatPopups = value;
			}
		}
		private List<SwatPopup> _swatPopups = new List<SwatPopup>();

		public const string TOGGLE_OFF = "◯";
		public const string TOGGLE_ON = "◉";

		public bool IsEditorRunningLastPass { get; set; }
		public bool RenderToolTip { get; set; }
		public bool Loading { get; private set; }
		public string LoadingMessage { get; private set; }

		int LoadingRemove { get; set; }
		bool popping { get; set; }

		public static Texture2D WindowBackgroundTexture { 
			get { 
				return EditorGUIUtility.whiteTexture;
			} 
		}
		public static Color WindowDefaultTextColor { 
			get { 
				return Color.black;
			} 
		}
		public const int WindowDefaultTextSize = 12;

		public static Texture2D TabButtonBackgroundSelectedTexture { 
			get { 
				return WindowBackgroundTexture;
			} 
		}
		public static Texture2D TabButtonBackgroundTexture { 
			get { 
				if(_tabButtonBackgroundTexture == default(Texture2D)) {
					_tabButtonBackgroundTexture = MakeTextureFromColor((Color)new Color32(200, 200, 200, 255));
				}
				return _tabButtonBackgroundTexture;
			} 
		}
		static Texture2D _tabButtonBackgroundTexture;
		public static Color TabButtonTextColor { 
			get { 
				return Color.black;
			} 
		}

		public static Texture2D ToggleButtonBackgroundSelectedTexture { 
			get { 
				if(_toggleButtonBackgroundSelectedTexture == default(Texture2D)) {
					_toggleButtonBackgroundSelectedTexture = MakeTextureFromColor((Color)new Color32(200, 200, 200, 255));
				}
				return _toggleButtonBackgroundSelectedTexture;
			} 
		}
		static Texture2D _toggleButtonBackgroundSelectedTexture;
		public static Texture2D ToggleButtonBackgroundTexture { 
			get { 
				if(_toggleButtonBackgroundTexture == default(Texture2D)) {
					_toggleButtonBackgroundTexture = MakeTextureFromColor((Color)new Color32(100, 100, 100, 255));
				}
				return _toggleButtonBackgroundTexture;
			} 
		}
		static Texture2D _toggleButtonBackgroundTexture;
		public static Color ToggleButtonTextColor { 
			get { 
				return Color.white;
			} 
		}
		public static Color ToggleButtonSelectedTextColor { 
			get { 
				return Color.black;
			} 
		}

		public static Texture2D ActionButtonTexture { 
			get { 
				if(_actionButtonTexture == default(Texture2D)) {
					_actionButtonTexture = MakeTextureFromColor((Color)new Color32(100, 100, 100, 255));
				}
				return _actionButtonTexture;
			} 
		}
		static Texture2D _actionButtonTexture;
		public static Color ActionButtonTextColor { 
			get { 
				return Color.white;
			} 
		}

		public static Texture2D BoxAreaBackgroundTexture { 
			get { 
				if(_boxAreaBackgroundTexture == default(Texture2D)) {
					_boxAreaBackgroundTexture = MakeTextureFromColor((Color)new Color32(235, 235, 235, 255));
				}
				return _boxAreaBackgroundTexture;
			} 
		}
		static Texture2D _boxAreaBackgroundTexture;

		public static Texture2D DefaultButtonTexture { 
			get { 
				return new GUIStyle(GUI.skin.button).normal.background;
			} 
		}

		public static Color TextGreen {
			get {
				if(_textGreen == default(Color)) {
					_textGreen = (Color)new Color32(0, 140, 20, 255);
				}
				return _textGreen;
			}
		}
		static Color _textGreen;

		DateTime _hoverBegan = new DateTime();
		int _floatOffsetY = 50;
		const float PIXELS_WIDTH_PER_CHARACTER = 7.5f;
		const float TOOLTIP_HEIGHT_MULTIPLIER = 22;
		const float PIXELS_WIDTH_PER_CHARACTER_TOOLTIP_ONLY = 7.15f;
		const float FOR_EVERY_NUMBER_CHARACTERS_EXPECT_ONE_SPACE = 10;
		int _tabsOffsetY = 20;
		int _closeButtonWidth = 25;
		float _boxWidth = 0;
		float _tabWidthTotal;
		string _lastHover = string.Empty;
		string _toolTipMessage = string.Empty;
		Rect _currentChildPosition;
		Vector2 _currentTooltipPosition;
		Vector2 _scroll = new Vector2();
		GUIStyle _tabButton;
		TabSize _currentTabSize;

        public static T ShowWindow<T>(Type type, string name, DockNextTo dockNextTo = DockNextTo.Default, string tooltip = "Swat Window") where T : EditorWindow {

			/* 
	            Close any existing windows, as window instance may exist in Editor, but not be accessible.
	            If we do not close that invalid instance, the GetWindow method below will return the
	            broken instance. The broken window will be inaccessible and will freeze remaining GUI windows. 
	        */

			GetWindow(type, false, name).Close(); 

			Assembly editorAssembly = typeof(Editor).Assembly;
			Type dockWindow = null;
			DockNextTo dockThis;

			if(dockNextTo != DockNextTo.Default) {

				dockThis = dockNextTo;

			} else {

				dockThis = DockNextTo; //Will retrieve the default Dock position from on Trilleon Settings.

			}

			switch(dockThis) {
				case DockNextTo.Float:
					dockWindow = null;
					break;
				case global::DockNextTo.SceneView:
					dockWindow = typeof(SceneView);
					break;
				case global::DockNextTo.GameView:
					dockWindow = editorAssembly.GetType("UnityEditor.GameView");
					break;
				case global::DockNextTo.Hierarchy:
					dockWindow = editorAssembly.GetType("UnityEditor.SceneHierarchyWindow");
					break;
				case global::DockNextTo.Console:
					dockWindow = editorAssembly.GetType("UnityEditor.ConsoleWindow");
					break;
				case global::DockNextTo.Inspector:
				default:
					dockWindow = editorAssembly.GetType("UnityEditor.InspectorWindow");
					break;
			}

			EditorWindow win = EditorWindow.GetWindow<T>(name, true, dockWindow);
			win.minSize = new Vector2(350, 350);
            win.titleContent = new GUIContent(name, tooltip);
			return (T)win;

		}

		public bool IsWindowSelected(SwatWindow tab) {

			return SelectedTab != null && SelectedTab.Window == tab;

		}

		public void SelectTab(SwatWindow tabToSelect) {

			SelectedTab = SwatWindows.Find(x => x.Window == tabToSelect);

		}

		public void AddWindowTab(TabDetails details) {

			SwatWindows.Add(details);

		}

		public void AddPopup(SwatPopup popup) {

			SwatPopups.Add(popup);

		}

		public void SwitchTabOrder(string tabOne, string tabTwo) {

			TabDetails tabOneSwap = SwatWindows.Find(x => x.Window.GetType().Name == tabOne);
			SwatWindows.Remove(tabOneSwap);
			TabDetails tabTwoSwap = SwatWindows.Find(x => x.Window.GetType().Name == tabTwo);
			SwatWindows.Remove(tabTwoSwap);
			int idOne = tabOneSwap.PriorityID;
			int idTwo = tabTwoSwap.PriorityID;
			tabOneSwap.PriorityID = idTwo;
			tabTwoSwap.PriorityID = idOne;
			SwatWindows.Add(tabOneSwap);
			SwatWindows.Add(tabTwoSwap);

		}

		bool _initialized = false;
		public void Initialize(PlayModeStateChange p) {

			_initialized = true;
			if(SelectedTab == null) {

				SelectedTab = SwatWindows.First();

			}

			for(int s = 0; s < SwatWindows.Count; s++) {

				SwatWindows[s].Window.Set();

			}

			//Close all open popup instances.
			List<Type> popTypes = new List<Type>();
			List<Assembly> assembliesAll = AppDomain.CurrentDomain.GetAssemblies().ToList();
			for(int x = 0; x < assembliesAll.Count; x++) {

				popTypes.AddRange(assembliesAll[x].GetTypes().ToList().FindAll(m => m.IsClass && m.GetInterface("SwatPopup") != null));

			}
			for(int pt = 0; pt < popTypes.Count; pt++) {

				EditorWindow.GetWindow(popTypes[pt], false).Close();

			}
			Errors = new List<SwatControlError>();
			Repaint();

		}

		/// <summary>
		/// Called every pass whether window is in focus or not.
		/// Some windows should be repainted regardless of focus.
		/// </summary>
		void Update() {

			if(SelectedTab == null) {

				Initialize(PlayModeStateChange.EnteredEditMode);

			}

			if(SelectedTab == null) {

				return;

			}

			if(SelectedTab.Window.UpdateWhenNotInFocus()) {

				Repaint();

			}

		}

		/*TODO: Fix it not being called.
		void OnGUI() {
			
			bool active = false;
			Event e = Event.current;
			if(e != null && e.type == EventType.keyDown) {
				
				if(Event.current.keyCode == KeyCode.LeftShift) {
					
					active = true;

				}

			}
			if(active) {
				
				for(int s = 0; s < SwatWindows.Count; s++) {

					bool activated = false;
					for(int k = 0; k < SwatWindows[s].Shortcut.Count; k++) {

						if(Event.current.keyCode == SwatWindows[s].Shortcut[k]) {
							
							activated = true;

						} else {

							activated = false;
							break;

						}

					}

					if(activated) {
						
						SelectedTab = SwatWindows[s];

					}

				}

			}

		}*/

		/// <summary>
		/// Render all tabs and selected SWAT window.
		/// </summary>
		public void Render() {

			RenderToolTip = false;
			if(!_initialized) {

				Initialize(PlayModeStateChange.EnteredEditMode);
				#if UNITY_2017_2_OR_NEWER
				EditorApplication.playModeStateChanged += Initialize;
				#else 
				EditorApplication.playmodeStateChanged += Initialize;
				#endif

			}

			GUI.DrawTexture(new Rect(0, 0, position.width, position.height), WindowBackgroundTexture);

			if(popping) {

				GUI.DrawTexture(new Rect(0, 0, position.width, position.height), Swat.MakeTextureFromColor((Color)new Color32(100, 100, 100, 150)));

			}

			if(!SwatWindows.Any()) {

				return;

			}

			_tabButton = new GUIStyle(GUI.skin.button);
			_closeButtonWidth = 25;

			//Render tabs based on current size of window.
			_tabWidthTotal = 90 * SwatWindows.Count + _closeButtonWidth;

			if(position.width >= _tabWidthTotal){

				_currentTabSize = TabSize.Large;

			} else if(position.width > (_tabWidthTotal/1.25)){

				_currentTabSize = TabSize.Medium;

			} else {

				_currentTabSize = TabSize.Small;

			}

			_tabWidthTotal = position.width; //Tabs will span the entire width of the Swat window.

			EditorGUILayout.BeginHorizontal();

			//Only render the tab group if this is a primary window. Sub windows (such as Favorites) will not be included in the toolbar.
			if(SelectedTab.PriorityID >= 0) {

				for(int tb = 1; tb < SwatWindows.Count; tb++) {

					TabDetails tabDetails = SwatWindows.Find(s => s.PriorityID == tb);
					if(tabDetails == null) {

						continue; //Hidden window, so no tabs.

					}
					RenderTab(tabDetails);

				}

				List<TabDetails> priorityZeros = SwatWindows.FindAll(s => s.PriorityID == 0);
				for(int p = 0; p < priorityZeros.Count; p++) {

					RenderTab(priorityZeros[p]);

				}

			}

			_tabButton = new GUIStyle(GUI.skin.button);
			_tabButton.normal.textColor = SelectedTab.PriorityID >= 0 ? Color.red : TextGreen;
			_tabButton.margin = new RectOffset(-2, -2, 0, 0);
			_tabButton.fontSize = 14;
			_tabButton.alignment = TextAnchor.MiddleCenter;
			_tabButton.normal.background = TabButtonBackgroundTexture;
			_closeButtonWidth = SelectedTab.PriorityID >= 0 ? _closeButtonWidth : 50;

			Nexus.Self.Button(SelectedTab.PriorityID >= 0 ? "X" : "<<<", "Show/Hide currently-selected game object in heirarchy window.", 
				new Nexus.SwatDelegate(delegate() {   

					if(SelectedTab.PriorityID >= 0) {

						this.CloseSwat();

					} else {

						SelectedTab = LastSelectedTab;

					}

				}), _tabButton, new GUILayoutOption[] { GUILayout.Height(25), GUILayout.Width(_closeButtonWidth) });

			EditorGUILayout.EndHorizontal();

			if(SelectedTab != LastSelectedTab) {

				_scroll = new Vector2(0,0);
				SelectedTab.Window.OnTabSelected();

			}

			if(!SelectedTab.OverrideScroll) {

				_scroll =  EditorGUILayout.BeginScrollView(_scroll);

			}

			try {

				if(SelectedTab.Window == null) {

					return;

				}
				if(LastSelectedTab != SelectedTab && SelectedTab.PriorityID >= 0) {

					LastSelectedTab = SwatWindows.Find(x => x.Window == SelectedTab.Window);

				}
				SelectedTab.Window.Render();
				RenderAnyError();
				RenderLoadingOverlay();

			} catch(Exception e) {

				if(IsValidError(e.Message)) {

					throw e;

				} 

			}

			if(!SelectedTab.OverrideScroll) {

				EditorGUILayout.EndScrollView();

			}

			RenderToolTipIfHovering(position);

		}

		void RenderTab(TabDetails tabDetails) {

			SizedTab tabToRender = tabDetails.Get(_currentTabSize);

			if(SelectedTab.Window == tabDetails.Window) {

				_tabButton = new GUIStyle(GUI.skin.label);
				_tabButton.normal.background = TabButtonBackgroundSelectedTexture;

			} else {

				_tabButton = new GUIStyle(GUI.skin.button);
				_tabButton.normal.background = TabButtonBackgroundTexture;

			}

			_tabButton.alignment = TextAnchor.MiddleCenter;
			_tabButton.margin = new RectOffset(-2, -2, 0, 0);
			_tabButton.fontSize = tabToRender.FontSize;
			_tabButton.normal.textColor = tabToRender.TextColor;

			Nexus.Self.Button(tabToRender.TabText, tabDetails.Get(TabSize.Large).TabText,
				new Nexus.SwatDelegate(delegate() {                
					SelectedTab = tabDetails;
				}), _tabButton, new GUILayoutOption[] {
					GUILayout.Height(25),
					GUILayout.Width(_tabWidthTotal / SwatWindows.Count - (_closeButtonWidth / SwatWindows.Count)),
					GUILayout.MaxWidth(125)
				});
			
		}

		/// <summary>
		/// Use this in static instances of SwatWindow classes where GetWindow is used to reinstantiate a null instance.
		/// </summary>
		static KeyValuePair<Type,DateTime> ClosedWindow = new KeyValuePair<Type,DateTime>();
		public static bool Closing(Type swatWindow) {

			return swatWindow == ClosedWindow.Key && Math.Abs(ClosedWindow.Value.Subtract(DateTime.UtcNow).TotalSeconds) < 2.5f;

		}

		void CloseSwat() {

			#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged -= Initialize;
			#else 
			EditorApplication.playmodeStateChanged -= Initialize;
			#endif
			ClosedWindow = new KeyValuePair<Type,DateTime>(this.GetType(), DateTime.UtcNow);
			this.Close();

		}

		#region Tooltip

		/// <summary>
		/// Set this control's tooltip.
		/// </summary>
		public void SetToolTip(string tooltip, int floatOffsetY = 0) {

			this._floatOffsetY = floatOffsetY;

			if(tooltip != _lastHover) {

				_currentTooltipPosition = Event.current.mousePosition;
				_currentChildPosition = new Rect();
				_lastHover = tooltip;
				_hoverBegan = DateTime.Now;

			} else {

				if(DateTime.Now.Subtract(_hoverBegan).TotalSeconds >= 1.25) {
					RenderToolTip = true;
					_toolTipMessage = tooltip;
				}

			}

		}

		/// <summary>
		/// If hovering over the last-rendered control, show its tooltip (if one exists).
		/// </summary>
		/// <param name="position">Position.</param>
		public void RenderToolTipIfHovering(Rect position) {

			_currentChildPosition = position;
			if(RenderToolTip) {

				GUI.Label(TooltipRect(_currentTooltipPosition, _toolTipMessage), string.Format("  {0}", _toolTipMessage), TooltipStyle);

			}

		}

		/// <summary>
		/// Detect if the cursor is hovering over the last-rendered control.
		/// </summary>
		/// <returns><c>true</c>, if over was moused, <c>false</c> otherwise.</returns>
		public bool MouseOver() {

			return Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);

		}

		public GUIStyle TooltipStyle {
			get { 
				GUIStyle tooltipStyle = new GUIStyle(GUI.skin.label);
				tooltipStyle.normal.textColor = Color.white;
				tooltipStyle.normal.background = MakeTextureFromColor(new Color32(140, 140, 140, 255));
				tooltipStyle.margin = new RectOffset(2, 2, 2, 2);
				tooltipStyle.alignment = TextAnchor.MiddleCenter;
				tooltipStyle.wordWrap = true;
				return tooltipStyle;
			}
		}

		Rect TooltipRect(Vector2 mousePosition, string tooltip) {

			float lines = 1;
			_boxWidth = 0;
			float indent = 0;
			float messageWidth = DetermineRectWidthBasedOnLengthOfToolTip(tooltip);

			float distanceToEdgeLeft = mousePosition.x;
			float distanceToEdgeRight = _currentChildPosition.width - mousePosition.x;

			if(position.width / 2 - messageWidth <= 0) {

				lines = (messageWidth / (position.width - 10));

				//Handle large words in tooltip that force wrapping and increase number of lines needed for display.
				float spacesInTooltip = tooltip.Split(' ').Length;
				float expectedSpaces = tooltip.Length / FOR_EVERY_NUMBER_CHARACTERS_EXPECT_ONE_SPACE;
				float addToLineTotal = (expectedSpaces / spacesInTooltip) - 1;

				if(addToLineTotal > 0) {

					lines += addToLineTotal;

				}
				lines = (float)Math.Ceiling(lines);

			}

			_boxWidth = messageWidth / lines;
			if(_boxWidth / 2 > distanceToEdgeLeft) {

				indent = (_boxWidth / 2 - distanceToEdgeLeft);

			} else if(_boxWidth / 2 > distanceToEdgeRight) {

				indent = -(_boxWidth / 2 - distanceToEdgeRight);

			} 

			float normalX = mousePosition.x - (_boxWidth / 2); 

			return new Rect(new Vector2(normalX + indent, mousePosition.y + _floatOffsetY), new Vector2(_boxWidth + 10, lines * TOOLTIP_HEIGHT_MULTIPLIER));

		}

		#endregion

		#region Custom GUI Extensions

		/// <summary>
		/// Creates extended text field that allows for placeholder text functionality.
		/// </summary>
		/// <returns>The field.</returns>
		/// <param name="uniqueFieldName">Unique field name that differentiates this field from others rendered in the same pass.</param>
		/// <param name="textField">Text field that this field updates.</param>
		/// <param name="placeholderText">Placeholder text to display while field is empty.</param>
		/// <param name="style">Optional Style.</param>
		/// <param name="options">Optional GUILayoutOptions.</param>
		public string TextField(string uniqueFieldName, string textField, string placeholderText, GUIStyle style = null, params GUILayoutOption[] options) {

			GUI.SetNextControlName(uniqueFieldName);
			GUIStyle textFieldStyle = style ?? new GUIStyle(GUI.skin.textField);
			string thisTextField = string.Empty;
			if(GUI.GetNameOfFocusedControl() == uniqueFieldName && string.IsNullOrEmpty(textField)) {

				//Render no text in this pass.
				return EditorGUILayout.TextField(textField, textFieldStyle, options);

			} else if(string.IsNullOrEmpty(textField)) {

				//Render placeholder text.
				textFieldStyle.normal.textColor = textFieldStyle.active.textColor = textFieldStyle.focused.textColor = (Color)new Color32(75, 75, 75, 200);
				thisTextField = EditorGUILayout.TextField(placeholderText, textFieldStyle, options);

			} else {

				//Render user-entered text.
				thisTextField = EditorGUILayout.TextField(textField, textFieldStyle, options);

			}

			if(thisTextField == placeholderText) {

				return string.Empty;

			} else {

				return thisTextField;

			}

		}

		public int DropDown(int setValue, string[] options, int leftOffset = 10, int height = 30, int width = 175) {

			int val = setValue;

			GUIStyle dropDown = new GUIStyle(GUI.skin.button);
			dropDown.margin = new RectOffset(leftOffset, 0, -10, 0);
			dropDown.fixedHeight = height;
			dropDown.fixedWidth = width;
			dropDown.normal.background = dropDown.active.background = dropDown.focused.background = ActionButtonTexture;
			dropDown.normal.textColor = dropDown.active.textColor = dropDown.focused.textColor =  ActionButtonTextColor;

			GUIStyle upDownArrowButtons = new GUIStyle(GUI.skin.button);
			upDownArrowButtons.fixedHeight = height / 2;
			upDownArrowButtons.fixedWidth = 20;
			upDownArrowButtons.normal.background = upDownArrowButtons.active.background = upDownArrowButtons.focused.background = ActionButtonTexture;
			upDownArrowButtons.normal.textColor = upDownArrowButtons.active.textColor = upDownArrowButtons.focused.textColor = ActionButtonTextColor;
			upDownArrowButtons.fontSize = (int)upDownArrowButtons.fixedHeight - 4;

			EditorGUILayout.BeginHorizontal();

			val = EditorGUILayout.Popup(setValue, options, dropDown, new GUILayoutOption[] { GUILayout.MaxWidth(width + upDownArrowButtons.fixedWidth), GUILayout.Height(height) });

			GUILayout.Space(-25f);

			EditorGUILayout.BeginVertical(); 

			GUILayout.Space(-10f);
			if (GUILayout.Button(MOVEUP, upDownArrowButtons)) {}               
			GUILayout.Space(-3f);

			upDownArrowButtons.fontSize = (int)upDownArrowButtons.fixedHeight - 6;
			if (GUILayout.Button(MOVEDOWN, upDownArrowButtons)) {}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			return val;

		}

		public Enum DropDown(Enum setEnumValue, int leftOffset = 10,  int height = 30, int width = 175) {

			Enum val = setEnumValue;

			GUIStyle dropDown = new GUIStyle(GUI.skin.button);
			dropDown.margin = new RectOffset(leftOffset, 0, -10, 0);
			dropDown.fixedHeight = height;
			dropDown.fixedWidth = width;
			dropDown.normal.background = dropDown.active.background = dropDown.focused.background = ActionButtonTexture;
			dropDown.normal.textColor = dropDown.active.textColor = dropDown.focused.textColor =  ActionButtonTextColor;

			GUIStyle upDownArrowButtons = new GUIStyle(GUI.skin.button);
			upDownArrowButtons.fixedHeight = height / 2;
			upDownArrowButtons.fixedWidth = 20;
			upDownArrowButtons.normal.background = upDownArrowButtons.active.background = upDownArrowButtons.focused.background = ActionButtonTexture;
			upDownArrowButtons.normal.textColor = upDownArrowButtons.active.textColor = upDownArrowButtons.focused.textColor = ActionButtonTextColor;
			upDownArrowButtons.fontSize = (int)upDownArrowButtons.fixedHeight - 4;

			EditorGUILayout.BeginHorizontal();

			val = EditorGUILayout.EnumPopup(setEnumValue, dropDown, new GUILayoutOption[] { GUILayout.MaxWidth(width + upDownArrowButtons.fixedWidth), GUILayout.Height(height) });
			GUILayout.Space(-25f);

			EditorGUILayout.BeginVertical();

			GUILayout.Space(-10f);
			if(GUILayout.Button(MOVEUP, upDownArrowButtons)) {}               
			GUILayout.Space(-2f);
			upDownArrowButtons.fontSize = (int)upDownArrowButtons.fixedHeight - 4;
			if(GUILayout.Button(MOVEDOWN, upDownArrowButtons)) {}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			return val;

		}

		public bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style = null) {

			GUIStyle s = style == null ? new GUIStyle(EditorStyles.foldout) : style;
			Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, s);
			return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, s);

		}

		public bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style = null) {

			return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);

		}

		public void Button(string buttonText, string tooltipDescription, SwatDelegate invoker, GUIStyle style = null, params GUILayoutOption[] options) {

			if(style == null) {

				style = new GUIStyle(GUI.skin.button);
				style.normal.textColor = Swat.WindowDefaultTextColor;

			}

			if(DefaultButtonTexture == style.normal.background) {

				style.normal.background = TabButtonBackgroundTexture;

			}

			if(GUILayout.Button(buttonText, style, options)) {

				invoker.Invoke();

			}

			if(!string.IsNullOrEmpty(tooltipDescription)) {

				if(MouseOver()) {

					SetToolTip(tooltipDescription, _tabsOffsetY);

				}

			}

		}

		public void ToggleButton(bool isToggledOn, string buttonText, string tooltipDescription, SwatDelegate invoker, int buttonWidth = 175, int statusTextAreaWidth = 55, RectOffset offset = null, string[] onOffText = null, bool invertBooleanColorIndication = false) {

			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.padding = offset == null ? new RectOffset(10, 0, 0, 0) : offset;
			style.normal.background = Swat.ActionButtonTexture;
			style.normal.textColor = Swat.ActionButtonTextColor;
			style.fixedHeight = 35;
			style.fixedWidth = buttonWidth;
			style.fontSize = 14;

			if(DefaultButtonTexture == style.normal.background) {

				style.normal.background = TabButtonBackgroundTexture;

			}

			GUIStyle text = new GUIStyle(GUI.skin.label);
			text.fontStyle = FontStyle.Bold;
			text.normal.textColor = Color.white;

			GUIStyle activeness = new GUIStyle(GUI.skin.box);
			if(style.fixedHeight > 0) {

				text.fontSize = (int)(style.fixedHeight / 1.75f);
				text.fixedHeight = style.fixedHeight;

			}
			int paddingTopBottom = (int)((style.fixedHeight - text.fontSize) / 2 - 1);
			text.padding = new RectOffset(5, 5, paddingTopBottom, paddingTopBottom);

			if(style.fixedWidth > 0) {

				activeness.fixedWidth = style.fixedWidth + statusTextAreaWidth;

			}
			activeness.normal.background = MakeTextureFromColor((invertBooleanColorIndication ? !isToggledOn : isToggledOn) ? (Color)new Color32(0, 135, 0, 255) : (Color)new Color32(135, 0, 0, 255));

			EditorGUILayout.BeginHorizontal(activeness);
			GUILayout.Space(-0.25f);
			if(GUILayout.Button(buttonText, style)) {

				invoker.Invoke();

			}
			string[] textAlts = onOffText == null ? new string[] { "On", "Off" } : onOffText; 
			EditorGUILayout.LabelField((invertBooleanColorIndication ? !isToggledOn : isToggledOn) ? textAlts[0] : textAlts[1], text);
			EditorGUILayout.EndHorizontal();

			if(!string.IsNullOrEmpty(tooltipDescription)) {

				if(MouseOver()) {

					SetToolTip(tooltipDescription, _tabsOffsetY);

				}

			}

		}

		public void ErrorOccurred(string controlIdentifier, string errorMessage, Vector2 erredControlPosition) {

			if(Errors.FindAll(x => x.ErrorControlID == controlIdentifier).Any()) {

				return;

			}

			SwatControlError error = new SwatControlError();
			error.ErrorControlID = controlIdentifier;
			error.ErrorMessage = errorMessage;
			error.ErrorPosition = erredControlPosition;
			Errors.Add(error);

		}

		public void RemoveError(string controlIdentifier) {

			if(Errors.FindAll(x => x.ErrorControlID == controlIdentifier).Any()) {

				Errors.Remove(Errors.Find(x => x.ErrorControlID == controlIdentifier));

			}

		}

		public void RenderAnyError() {

			if(Errors.Any()) {

				GUIStyle error = new GUIStyle(GUI.skin.label);
				error.normal.textColor = Color.white;
				error.normal.background = MakeTextureFromColor(new Color32(175, 0, 0, 225));
				error.alignment = TextAnchor.MiddleCenter;
				error.margin = new RectOffset(2, 2, 2, 2);
				error.wordWrap = true;

				for(int e = 0; e < Errors.Count; e++) {

					GUI.Label(new Rect(new Vector2(15, Errors[e].ErrorPosition.y + 115), new Vector2(position.width - 40, Errors[e].MessageHeight(position.width))), string.Format("  {0}", Errors[e].ErrorMessage), error);

				}

			}

		}

		public void ShowLoading(string loadingMessage = "Loading...") {

			Loading = true;
			LoadingMessage = loadingMessage;

		}

		public void HideLoading() {

			Loading = false;
			LoadingRemove = 0;

		}

		void RenderLoadingOverlay() {

			if(Loading) {

				if(LoadingRemove >= 2) {

					HideLoading();

				}

				GUIStyle load = new GUIStyle(GUI.skin.box);
				load.normal.background = MakeTextureFromColor((Color)new Color32(75, 75, 75, 150));
				load.normal.textColor = Color.white;
				load.fontSize = 24;
				load.alignment = TextAnchor.MiddleCenter;
				if(GUI.Button(new Rect(0, 0, position.width, position.height), LoadingMessage, load)) {

					LoadingRemove++;

				}

			}

		}

		#endregion

		#region Helpers

		public static Texture2D MakeTextureFromColor(Color color) {

			Texture2D result = new Texture2D(1, 1);
			result.SetPixels(new Color[] { color });
			result.Apply();
			return result;

		}

		public static float DetermineRectWidthBasedOnLengthOfToolTip(string tooltip) {

			return tooltip.Length * PIXELS_WIDTH_PER_CHARACTER_TOOLTIP_ONLY;

		}

		public static float DetermineRectWidthBasedOnLengthOfString(string val) {

			return val.Length * PIXELS_WIDTH_PER_CHARACTER;

		}

		#endregion

		public bool IsValidError(string errorToLowerCase) {

			/*
            All three of these editors can occur when a control group "Begin..." and "End..." are conditionally drawn/not drawn between two passes. 
            They do not affect anything and can be ignored in the same way as "Coroutine Continue Failure" errors in game code.
         */

			return !errorToLowerCase.ToLower().Contains("mismatched layoutgroup") && !errorToLowerCase.ToLower().Contains("position in a group with only") && !errorToLowerCase.ToLower().Contains("you are pushing more guiclips than you are popping");

		}

	}

}


/// <summary>
/// Used to declare where a new window should be docked in the Unity Editor.
/// </summary>
public enum DockNextTo {
	Default,
	Inspector,
	GameView,
	SceneView,
	Hierarchy,
	Console,
	Float
}
