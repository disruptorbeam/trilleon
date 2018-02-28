using System;
using UnityEngine;
using UnityEditor;

namespace TrilleonAutomation {

	public class Manifest : SwatWindow {

		public override void Set() {


		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle tests = new GUIStyle(EditorStyles.foldout);
			tests.margin = new RectOffset(15, 15, 2, 2);
			GUILayout.Space(25);
			for(int x = 0; x < AutomationMaster.Methods.Count; x++) {

				EditorGUILayout.Foldout(false, AutomationMaster.Methods[x].Value.Name, tests);

			}

		}

	}

}