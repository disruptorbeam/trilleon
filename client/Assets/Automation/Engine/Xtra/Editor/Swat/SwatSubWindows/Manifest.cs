using System;
using UnityEngine;
using UnityEditor;

namespace TrilleonAutomation {

	public class Manifest : SwatWindow {

		public override void Set() {

		}

		public override bool UpdateWhenNotInFocus() {

			return true;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle labels = new GUIStyle(EditorStyles.label);
			labels.padding = new RectOffset(25, 15, 2, 2);
			GUIStyle cs = new GUIStyle(EditorStyles.label);
			cs.padding = new RectOffset(5, 15, 2, 2);
			cs.wordWrap = true;
			cs.fontStyle = FontStyle.Italic;
			EditorGUILayout.LabelField("This feature is entirely incomplete at the moment. Will have as much data about current test run progress as possible. Coming Soon!", cs);
			GUILayout.Space(25);
			EditorGUILayout.LabelField(string.Format("{0} out of {1} completed!", AutomationMaster.TestRunContext.CompletedTests.Count, AutomationMaster.Methods.Count), labels);
			GUILayout.Space(5);
			EditorGUILayout.LabelField(string.Format("Currently Running: {0}", AutomationMaster.CurrentTestContext.TestName), labels);
			GUIStyle tests = new GUIStyle(EditorStyles.foldout);
			tests.margin = new RectOffset(15, 15, 2, 2);
			GUILayout.Space(25);
			for(int x = 0; x < AutomationMaster.Methods.Count; x++) {

				EditorGUILayout.Foldout(false, AutomationMaster.Methods[x].Value.Name, tests);

			}

		}

	}

}