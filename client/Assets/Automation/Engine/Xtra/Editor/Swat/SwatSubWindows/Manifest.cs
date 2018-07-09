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

using UnityEngine;
using UnityEditor;

namespace TrilleonAutomation {

    public class Manifest : SwatWindow {

        Vector2 scroll = new Vector2();

        public override void Set() { }

        public override bool UpdateWhenNotInFocus() {

            return true;

        }

        public override void OnTabSelected() { }

        public override void Render() {

            scroll = new Vector2(0, scroll.y);
            GUIStyle labels = new GUIStyle(EditorStyles.label);
            labels.padding = new RectOffset(25, 15, 2, 2);
            GUIStyle cs = new GUIStyle(EditorStyles.label);
            cs.padding = new RectOffset(5, 15, 2, 2);
            cs.wordWrap = true;
            cs.fontStyle = FontStyle.Italic;
            EditorGUILayout.LabelField("This feature is incomplete at the moment. It will have as much data about current test run progress as possible. Coming Soon!", cs);
            GUILayout.Space(25);
            EditorGUILayout.LabelField(string.Format("{0} out of {1} completed!", AutomationMaster.TestRunContext.CompletedTests.Count, AutomationMaster.Methods.Count), labels);
            GUILayout.Space(5);
            EditorGUILayout.LabelField(string.Format("Currently Running: {0}", AutomationMaster.CurrentTestContext.TestName), labels);
            GUIStyle tests = new GUIStyle(EditorStyles.foldout);
            tests.margin = new RectOffset(15, 15, 2, 2);
            GUILayout.Space(25);
            scroll = GUILayout.BeginScrollView(scroll);
            for(int x = 0; x < AutomationMaster.Methods.Count; x++) {

                EditorGUILayout.Foldout(false, AutomationMaster.Methods[x].Value.Name, tests);

            }
            GUILayout.EndScrollView();

        }

    }

}
