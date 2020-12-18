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
using UnityEditor;

namespace TrilleonAutomation {

	public delegate void EditorDelegate();

	public class SimpleAlert : SwatPopup {

		SimpleAlert pop;
		bool _positionWindow;
		int buttonHeight = 25;
		int buttonWidth = 75;
		float lastWindowWidth = 0;

		string Message = string.Empty;
		EditorDelegate OnConfirm;

		GUIStyle border, buttons, divider, header, messageLabel, scrollBar;

		public override void PositionWindow() {

			position = new Rect(Nexus.Self.position.x + (Nexus.Self.position.width - Nexus.Self.minSize.x) / 2, Nexus.Self.position.height / 3, Nexus.Self.minSize.x, TestMonitorHelpers.DetermineRectHeightBasedOnLinesInNodeDetails(Message) + 140);

		}

		public static void Pop(string message, EditorDelegate onAccept) {

			EditorWindow.GetWindow<SimpleAlert>().Close();
			ScriptableObject.CreateInstance<SimpleAlert>().Set(message, onAccept);

		}
		public void Set(string message, EditorDelegate onAccept) {

			SimpleAlert pop = ScriptableObject.CreateInstance<SimpleAlert>();
			pop.Message = message;
			pop.OnConfirm = onAccept;
			pop.IsVisible = true;
			pop.ShowPopup();

		}

		public override bool Visible() {

			return IsVisible;

		}
		public bool IsVisible { get; set; }

		public override void OnGUI() {

			if(Nexus.Self == null) {

				Close();
				return;

			}

			if(lastWindowWidth != Nexus.Self.position.width) {

				_positionWindow = true;

			}

			if(_positionWindow) {

				_positionWindow = false;
				lastWindowWidth = Nexus.Self.position.width;
				PositionWindow();

			}

			GUI.DrawTexture(new Rect(0, 0, position.width, position.height), Swat.MakeTextureFromColor(Color.gray));

			divider = new GUIStyle(GUI.skin.box);
			divider.normal.background = Swat.MakeTextureFromColor(Color.white);
			divider.margin = new RectOffset(25, 0, 10, 20);

			header = new GUIStyle(GUI.skin.label);
			header.fontSize = 15;
			header.normal.textColor = Color.white;
			header.fontStyle = FontStyle.Bold;
			header.alignment = TextAnchor.MiddleCenter;
			header.fixedHeight = 40;
			header.padding = new RectOffset(0, 0, 5, 0);

			buttons = new GUIStyle(GUI.skin.button);
			buttons.fixedHeight = buttonHeight;
			buttons.fixedWidth = buttonWidth;
			buttons.normal.background = Swat.MakeTextureFromColor((Color)new Color32(80, 80, 80, 255));
			buttons.normal.textColor = Color.white;

			messageLabel = new GUIStyle(GUI.skin.label);
			messageLabel.fontSize = 12;
			messageLabel.padding = new RectOffset(32, 0, 0, 0);
			messageLabel.normal.textColor = Color.white;
			messageLabel.wordWrap = true;

			EditorGUILayout.LabelField("Please Confirm", header);
			GUILayout.Space(20);

			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });
			EditorGUILayout.LabelField(Message, messageLabel,  new GUILayoutOption[] { GUILayout.Width(position.width - 50) });
			GUILayout.Space(10);
			GUILayout.Box(string.Empty, divider, new GUILayoutOption[] { GUILayout.Height(1), GUILayout.Width(position.width - 50) });
			buttons.margin = new RectOffset((int)System.Math.Round((position.width / 2) - buttons.fixedWidth), 0, 0, 0); //Position just left of center.

			//Only show Confirm button if no action but closing of the alert is expected.
			if(OnConfirm != null) {

				EditorGUILayout.BeginHorizontal();
				Nexus.Self.Button("Cancel", "Deny alert criteria.", 
					new Nexus.SwatDelegate(delegate() {                
						//Close and do nothing.
						IsVisible = false;
						Close();
					}), buttons);

				buttons = new GUIStyle(GUI.skin.button);
				buttons.fixedHeight = buttonHeight;
				buttons.fixedWidth = buttonWidth;
				buttons.margin = new RectOffset(5, 0, 0, 0);
				buttons.normal.background = Swat.MakeTextureFromColor((Color)new Color32(80, 80, 80, 255));
				buttons.normal.textColor = Color.white;

			} else {

				buttons.margin = new RectOffset((int)System.Math.Round((position.width / 2) - (buttons.fixedWidth / 2)), 0, 0, 0); //Position exact center.

			}

			Nexus.Self.Button("Confirm", "Accept alert criteria.", 
				new Nexus.SwatDelegate(delegate() {                
					//Invoke provided delegate and close this confirmation popup.
					if(OnConfirm != null) {
						OnConfirm(); 
					}
					IsVisible = false;
					Close();
				}), buttons);

			if(OnConfirm != null) {

				EditorGUILayout.EndHorizontal();

			}

			GUILayout.Space(10);

		}

	}

}
