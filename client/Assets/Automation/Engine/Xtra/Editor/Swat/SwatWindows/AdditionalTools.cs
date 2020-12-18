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

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TrilleonAutomation {

    public class AdditionalTools : SwatWindow {

        enum DefaultTestView { Automation, UnitTests };
        static DefaultTestView defaultTestView;
        static DefaultTestView lastPassDefaultTestView;
        static DockNextTo dock;
        static DockNextTo lastPassDock;

        GUIStyle description, dropDownLabel, editorName, open, stepWrapper;
        byte currentAlphaStep = 60;
        int recentlyReorderedIndex = -1;

        public static List<KeyValuePair<string, string>> TabPreferences {
            get {
                if(_tabPreferences.Count == 0) {

                    SetTabPreferences();

                }
                return _tabPreferences;
            }
            private set {
                _tabPreferences = value;
            }
        }
        static List<KeyValuePair<string, string>> _tabPreferences = new List<KeyValuePair<string, string>>();

        public override void Set() { }

        public override void OnTabSelected() { }

        public static void SetTabPreferences() {

            lastPassDock = dock = Swat.DockNextTo;
            _tabPreferences = new List<KeyValuePair<string, string>>();

            for(int w = 0; w < Nexus.Self.SwatWindows.Count; w++) {

                TabDetails match = Nexus.Self.SwatWindows.Find(x => x.PriorityID == w + 1);
                if(match == null) {

                    continue;

                }
                string windowName = match.Window.GetType().Name;
                //If no data exists for this window, add it to the end of existing ranked windows.
                if(!_tabPreferences.FindAll(x => x.Key == windowName).Any()) {

                    _tabPreferences.Add(new KeyValuePair<string, string>(windowName, match.Get(TabSize.Small).TabText));

                }

            }

        }

        public override bool UpdateWhenNotInFocus() {

            return false;

        }

        public override void Render() {

            description = new GUIStyle(GUI.skin.label);
            description.fontSize = 12;
            description.wordWrap = true;
            description.margin = new RectOffset(10, 10, 0, 0);
            description.normal.textColor = Swat.WindowDefaultTextColor;

            dropDownLabel = new GUIStyle(GUI.skin.label);
            dropDownLabel.fontSize = 12;
            dropDownLabel.padding = new RectOffset(10, 0, -5, 0);
            dropDownLabel.normal.textColor = Swat.WindowDefaultTextColor;

            editorName = new GUIStyle(GUI.skin.label);
            editorName.fontSize = 16;
            editorName.fixedHeight = 20;
            editorName.fontStyle = FontStyle.Bold;
            editorName.padding = new RectOffset(8, 0, 0, 0);
            editorName.normal.textColor = Swat.WindowDefaultTextColor;

            open = new GUIStyle(GUI.skin.button);
            open.fontSize = 14;
            open.fixedHeight = 32;
            open.fixedWidth = 100;
            open.margin = new RectOffset(10, 10, 0, 0);
            open.normal.textColor = Swat.WindowDefaultTextColor;
            open.normal.background = open.active.background = open.focused.background = Swat.ToggleButtonBackgroundSelectedTexture;

            GUILayout.Space(25);

            EditorGUILayout.LabelField("Assistant", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField("This window watches inspected elements in the hierarchy window and displays automation-relevant details on this object. With the presented information," +
                "you may choose actions to perform on the object and generate code stubs for interacting with that object.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.Generator);
                Nexus.Self.Generator.SelectedSubTab = GeneratorSubTab.Assistant;

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Buddy Details", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField("This window allows customization of Buddy System data for editor-based test runs.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.BuddyData);

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Command Console", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField("This window displays all of the console/cheat commands registered in the code. From here, the aliases, arguments, and details of each command are displayed." +
                "Commands can be launched with optional arguments from this window.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.CommandConsoleView);

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Dependency Architecture", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField(@"This window displays the Dependency Class and Dependency Test attribute usage within the all existing tests.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.Architect);

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Dependency Web", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField(@"This window displays Dependency Web usage throughout existing tests, helping to visualize the relationships between tests in a graphical web format." +
                "Because of the way this window is rendered, docking options are limited to Floating and DockNextToGameWindow only when opening.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                //Web must be viewed in a large screen move. Dock next to Game, or allow float.
                Swat.ShowWindow<DependencyVisualizer>(typeof(DependencyVisualizer), "Web", dock == DockNextTo.Float ? dock : DockNextTo.GameView);

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Recorder", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField(@"This window allows you to begin recording automation-relevant actions that you take in the Unity Editor. When you stop recording, all actions are converted into code that can" +
                "be pasted directly into a test method, and played back as a new test.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.Generator);
                Nexus.Self.Generator.SelectedSubTab = GeneratorSubTab.Recorder;

            }
            GUILayout.Space(18);

            EditorGUILayout.LabelField("Settings", editorName);
            GUILayout.Space(4);
            EditorGUILayout.LabelField(@"All Trilleon settings keys are displayed here so that their values can easily be updated, and new custom keys can be added on the fly.", description);
            GUILayout.Space(4);
            if(GUILayout.Button("Open", open)) {

                Nexus.Self.SelectTab(Nexus.Self.Settings);

            }

            GUILayout.Space(40);
            EditorGUILayout.LabelField("Customize Your Nexus", editorName);

            GUILayout.Space(18);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Default Test View", dropDownLabel, new GUILayoutOption[] { GUILayout.Width(125) });
            defaultTestView = (DefaultTestView)Nexus.Self.DropDown(defaultTestView, 0, 25, 140);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(25);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Dock Next To:", dropDownLabel, new GUILayoutOption[] { GUILayout.Width(125) });
            dock = (DockNextTo)Nexus.Self.DropDown(dock, 0, 25, 140);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(25);

            if(dock != lastPassDock) {

                UpdateDockPreference();
                lastPassDock = dock;

            }
            if(defaultTestView != lastPassDefaultTestView) {

                SaveDefaultTestView();
                lastPassDefaultTestView = defaultTestView;
            }

            EditorGUILayout.LabelField("Simply select the arrows below change the order in which tabs render at the top of the Nexus window. Changes are made and saved instantly.", description);
            GUILayout.Space(20);

            GUIStyle windowName = new GUIStyle();
            windowName.fontSize = 15;
            windowName.fixedHeight = 30;
            windowName.alignment = TextAnchor.MiddleLeft;

            GUIStyle upDownArrowButtons = new GUIStyle(GUI.skin.button);
            upDownArrowButtons.fixedHeight = 15;
            upDownArrowButtons.fixedWidth = 25;
            upDownArrowButtons.normal.background = Swat.TabButtonBackgroundTexture;
            upDownArrowButtons.normal.textColor = Color.blue;

            for(int x = 0; x < TabPreferences.Count; x++) {

                stepWrapper = new GUIStyle();
                stepWrapper.fixedHeight = 30;
                if(recentlyReorderedIndex == x) {

                    if((int)currentAlphaStep - 1 < 0) {

                        currentAlphaStep = 60;
                        recentlyReorderedIndex = -1;

                    } else {

                        currentAlphaStep -= (byte)1;
                        stepWrapper.normal.background = Swat.MakeTextureFromColor((Color)new Color32(0, 200, 0, currentAlphaStep));
                        Nexus.Self.Repaint();

                    }

                }

                EditorGUILayout.BeginHorizontal(stepWrapper, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
                GUILayout.Space(15);
                upDownArrowButtons.fontSize = 15;

                EditorGUILayout.BeginVertical();
                if(x == 0) {

                    upDownArrowButtons.normal.textColor = Color.grey;

                } else {

                    upDownArrowButtons.normal.textColor = Color.blue;

                }
                if(GUILayout.Button(Swat.MOVEUP, upDownArrowButtons)) {

                    if(x != 0) {

                        recentlyReorderedIndex = ReOrderAction(x, true);

                    }

                }
                GUILayout.Space(-3f);
                upDownArrowButtons.fontSize = 11;
                if(x == TabPreferences.Count - 1) {

                    upDownArrowButtons.normal.textColor = Color.grey;

                } else {

                    upDownArrowButtons.normal.textColor = Color.blue;

                }
                if(GUILayout.Button(Swat.MOVEDOWN, upDownArrowButtons)) {

                    if(x != TabPreferences.Count - 1) {

                        recentlyReorderedIndex = ReOrderAction(x, false);

                    }

                } else {

                    upDownArrowButtons.normal.textColor = Color.blue;

                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.LabelField(string.Format("{0}   {1}", TabPreferences[x].Value, TabPreferences[x].Key), windowName, new GUILayoutOption[] { GUILayout.Width(175) });
                EditorGUILayout.EndHorizontal();

            }

            GUILayout.Space(20);

        }

        /// <summary>
        /// Change the "chronological" order of a recorded action. Returns the moved object's new index in the entire action list.
        /// </summary>
        /// <param name="currentIndex"> Action's current index in the list. </param>
        /// <param name="moveUp"> Move the action up in the list? False moves the object down in the list. </param>
        public static int ReOrderAction(int currentIndex, bool moveUp) {

            int newIndex = moveUp ? currentIndex - 1 : currentIndex + 1;
            List<KeyValuePair<string, string>> ReorderedList = new List<KeyValuePair<string, string>>();
            for(int x = 0; x < TabPreferences.Count; x++) {

                if(x == newIndex) {

                    if(moveUp) {

                        ReorderedList.Add(TabPreferences[currentIndex]);
                        ReorderedList.Add(TabPreferences[x]);

                    } else {

                        ReorderedList.Add(TabPreferences[x]);
                        ReorderedList.Add(TabPreferences[currentIndex]);

                    }

                } else if(x == currentIndex) {

                    continue; //Ignore, as this is re-added in the above condition.

                } else {

                    ReorderedList.Add(TabPreferences[x]);

                }

            }
            TabPreferences = ReorderedList;
            Nexus.Self.SwitchTabOrder(TabPreferences[currentIndex].Key, TabPreferences[newIndex].Key);
            SaveTabDisplayOrder();
            return newIndex;

        }

        static void SaveDefaultTestView() {

            Customizer.Self.AddKey("nexus_default_test_mode_render", System.Enum.GetName(typeof(DefaultTestView), defaultTestView));
            Customizer.Self.SaveUpdates();

        }

        static void SaveTabDisplayOrder() {

            Customizer.Self.AddKey("nexus_custom_tab_order", string.Join("|", TabPreferences.ExtractListOfKeysFromKeyValList().ToArray()));
            Customizer.Self.SaveUpdates();

        }

        static void UpdateDockPreference() {

            Customizer.Self.AddKey("default_dock_nexus_next_to", System.Enum.GetName(typeof(DockNextTo), dock));
            Customizer.Self.SaveUpdates();

        }

    }

}
