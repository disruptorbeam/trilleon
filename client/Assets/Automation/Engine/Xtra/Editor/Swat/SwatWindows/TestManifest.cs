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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Reflection;

namespace TrilleonAutomation {

    public class TestManifest : SwatWindow {

        string RUNNING = "↻";
        public const string FAILED = "✗";
        public const string SKIPPED = "▲";
        public const string PASSED = "✔";
        public const string IGNORED = "//";
        const string NODATA = "♦";
        const string STOP = "▣";
        const string RUNALL = "▶";
        const string COLLAPSE = "◤";
        const string EXPAND = "◢";
        const string PAUSE_ON_FAILURE = "||";
        const string CLEARCACHE = "✤";
        const string TOGGLEBUDDY = "♥";
        const string RELOADLOCKON = "Ⓛ";
        const string RELOADLOCKOFF = "Ⓤ";
        const string SOFTRESET = "☈";
        const string FAVORITE = "★";
        const string VALIDATE = "☤";
        const string VIEW_MODE = "∞";
        const string UNIT_TEST_VIEW = "U";
        const string INTEGRATION_TEST_VIEW = "A";
        const int LOGIC_UPDATE_INTERVAL_SECONDS_FOR_RETAINING_PERFORMANCE = 60;

        const string ignored = "Ignored";
        const string passed = "Passed";
        const string failed = "Failed";
        const string skipped = "Skipped";
        const string noData = "NoData";

        int loopCounter = 2;
        int _updateCounter = 0;
        int _selectedCategory = 0;
        int _redrawRateSeconds = 10;
        float _lastWindowWidth = 0;
		bool automationInactive, editorPlayModeActivationHandled, expandCategories, _hardStop, hideBoxArea, _ignoreBuddyTests, loopModeActive;
		bool showPassed = true;
        bool showFailed = true;
        bool showSkipped = true;
        bool showIgnored = true;
        bool showNoData = true;
        bool _viewModeCategory = true;
        public bool Update_Data = true;
        public bool LockAssemblies;
        string selectedTest = string.Empty;
        string lastSelectedTest = string.Empty;

        Color32 colorIgnore;
        Color32 colorSkipped;
        Color32 colorPassed;
        Color32 colorFailed;
        Color32 colorButtonTextDefault;
        RectOffset paddingShow;
        RectOffset paddingHide;

        public List<bool> Categories {
            get {
                return _categories;
            }
            private set {
                _categories = value;
            }
        }
        public List<bool> _categories = new List<bool>();

        public List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> CategoryTests {
            get {
                return _categoryTests;
            }
            private set {
                _categoryTests = value;
            }
        }
        List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> _categoryTests = new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>>();

        public List<string> CatKeys {
            get {
                return catKeys;
            }
            private set {
                catKeys = value;
            }
        }
        List<string> catKeys = new List<string>();

        string _filterField = string.Empty;
        bool _firstPass, _isEditorRunning, _reget, _resetOnStart;
        List<MethodInfo> _methods = new List<MethodInfo>();
        List<string> _subCategoryNames = new List<string>();
        List<bool> _subCategoryBools = new List<bool>();
        List<bool> _selectedTestCategories = new List<bool>();
        List<string> _allMethods = new List<string>();
        List<string> _allMethodsLast = new List<string>();
        List<KeyValuePair<Type, MethodInfo>> _all = new List<KeyValuePair<Type, MethodInfo>>();
        List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> _categorySubcategoryTests = new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>>();
        List<KeyValuePair<string, string>> _selectedMethodAttributes = new List<KeyValuePair<string, string>>();

        GUIStyle b, closeBox, filter, filterField, fo, h, horizontalToolbarGroup, testStatusBox, toolBarButtons;

        DateTime _lastLogicUpdate = DateTime.UtcNow;
        DateTime _lastResultsRetrieval = DateTime.UtcNow;

        //This window relies on data that may constantly be updated, resulting in a need to repaint/refresh at a high rate.
        public void OnInspectorUpdate() {

            if(Application.isPlaying) {

                if(_updateCounter % 3 == 0) {

                    RUNNING = RUNNING == "↻" ? "↺" : "↻";

                }
                if(_updateCounter > 100) {

                    _updateCounter = 0;

                }
                _updateCounter++;

            }
            Nexus.Self.Repaint();

        }

        public override bool UpdateWhenNotInFocus() {

            return true;

        }

        public override void Set() {

            LockAssemblies = AutomationMaster.ConfigReader.GetBool("NEVER_AUTO_LOCK_RELOAD_ASSEMBLIES") ? false : AutomationMaster.Busy;
            _isEditorRunning = Application.isPlaying;
            _firstPass = true;
            _resetOnStart = Nexus.Overseer.softReset;
            AutomationReport.GetMostRecentsResults();

            List<string> instructions = new List<string>();
            string instructionsText = FileBroker.GetNonUnityTextResource(FileResource.ManifestGUISettings);
            if(!string.IsNullOrEmpty(instructionsText)) {

                instructions = instructionsText.Split('\n').ToList();

            }

            if(instructions.Any()) {

                Categories = new List<bool>();
                string[] bools = instructions.First().Split(AutomationMaster.DELIMITER);
                for(int i = 0; i < bools.Length; i++) {

                    Categories.Add(bool.Parse(bools[i]));

                }

            }

			if(Application.isPlaying) {

				automationInactive = AutomationMaster.StaticSelf == null;

			}

        }

        public override void OnTabSelected() {

            AutomationMaster.UnitTestMode = Customizer.Self.GetString("nexus_default_test_mode_render") == "UnitTests";

        }

        public override void Render() {

			if(automationInactive) {

				GUIStyle autoInactivate = new GUIStyle(GUI.skin.label);
				autoInactivate.normal.textColor = Color.red;
				autoInactivate.wordWrap = true;
				autoInactivate.padding = new RectOffset(10, 10, 20, 0);
				EditorGUILayout.LabelField("Automation inactive. Make sure you add AutomationMaster.Initialize() to game start up.", autoInactivate);

			}

            colorIgnore = new Color32(0, 210, 225, 255);
            colorSkipped = new Color32(225, 180, 0, 255);
            colorPassed = new Color32(0, 210, 0, 255);
            colorFailed = new Color32(210, 0, 0, 255);
            colorButtonTextDefault = new Color32(165, 165, 165, 255);
            paddingShow = new RectOffset(0, 0, -6, 0);
            paddingHide = new RectOffset(2, 0, -2, 0);

            GUIStyle buttonGroup = new GUIStyle();
            buttonGroup.margin = new RectOffset(8, 0, 0, 0);

            fo = new GUIStyle(EditorStyles.foldout);
            fo.padding = new RectOffset(10, 0, 0, 2);
            fo.normal.textColor = Swat.WindowDefaultTextColor;
            fo.normal.background = Swat.WindowBackgroundTexture;

            filter = new GUIStyle(GUI.skin.label);
            filter.margin = new RectOffset(14, 0, 0, 0);
            filter.normal.textColor = Swat.WindowDefaultTextColor;
            filter.normal.background = Swat.WindowBackgroundTexture;

            filterField = new GUIStyle(GUI.skin.label);
            filterField.normal.textColor = filterField.active.textColor = filterField.focused.textColor = Swat.WindowDefaultTextColor;
            filterField.normal.background = filterField.active.background = filterField.focused.background = Swat.TabButtonBackgroundTexture;

            testStatusBox = new GUIStyle(GUI.skin.box);
            testStatusBox.margin = new RectOffset(10, 10, 0, 0);
            _lastWindowWidth = Nexus.Self.position.width;

            //Flip assembly unlock flag if applicable.
            if(!editorPlayModeActivationHandled && EditorApplication.isPlaying && AutomationMaster.Busy) {

                editorPlayModeActivationHandled = true;
                LockAssemblies = AutomationMaster.ConfigReader.GetBool("NEVER_AUTO_LOCK_RELOAD_ASSEMBLIES") ? false : AutomationMaster.Busy;

            }

            //Application has launched in Editor Play Mode. Retrieve the test results again.
            if((!_isEditorRunning && Application.isPlaying) || (_isEditorRunning && !Application.isPlaying)) {

                _isEditorRunning = !_isEditorRunning;
                AutomationReport.GetMostRecentsResults();
                _lastResultsRetrieval = DateTime.Now;

            }

            if(!Update_Data) {

                //Only perform logic-based operations every `LOGIC_UPDATE_INTERVAL_SECONDS_FOR_RETAINING_PERFORMANCE` seconds.
                Update_Data = System.Math.Abs(_lastLogicUpdate.Subtract(DateTime.UtcNow).TotalSeconds) > LOGIC_UPDATE_INTERVAL_SECONDS_FOR_RETAINING_PERFORMANCE;

            }

            //Must be after the previous conditional in the case that Update_Data has been set to true by outside logic.
            if(Update_Data) {

                _lastLogicUpdate = DateTime.UtcNow;

            }

            //Resize an open alert window if resizing this window.
            if(Nexus.Self.SwatPopups.Find(s => s.GetType() == typeof(RunTestsAlert)) != null && Nexus.Self.SwatPopups.Find(s => s.GetType() == typeof(RunTestsAlert)).Visible() && _lastWindowWidth != Nexus.Self.position.width) {

                Nexus.Self.SwatPopups.Find(s => s.GetType() == typeof(RunTestsAlert)).PositionWindow();

            }

            testStatusBox.normal.background = Swat.BoxAreaBackgroundTexture;

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Filter: ", filter, new GUILayoutOption[] { GUILayout.Width(35) });
            _filterField = EditorGUILayout.TextField(_filterField, filterField, new GUILayoutOption[] { GUILayout.Width(240) });
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            horizontalToolbarGroup = new GUIStyle();
            horizontalToolbarGroup.margin = Nexus.BaseRectOffset;
            EditorGUILayout.BeginHorizontal(horizontalToolbarGroup);
            toolBarButtons = new GUIStyle(GUI.skin.button);

            int extraToolCount = 0;

            //Stop running Automation processes.
            if(AutomationMaster.Busy && Application.isPlaying && !_hardStop) {

                toolBarButtons.fontSize = Nexus.isPc ? 24: 30;
                toolBarButtons.normal.textColor = new Color32(250, 0, 0, 255);

                Nexus.Self.Button(STOP, "Stops running tests.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        GameObject helper = GameObject.Find(TestMonitorHelpers.NAME);
                        if(helper != null) AutomationMaster.Destroy(helper);
                        AutomationMaster.Destroy(AutomationMaster.StaticSelf);
                        AutomationMaster.Initialize();
                        AutomationMaster.StaticSelfComponent.ResetTestRunner();
                        AutoConsole.PostMessage("Automation Hard Stop!", MessageLevel.Abridged);
                        _hardStop = true;
                    }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
                extraToolCount++;

            } else {

                toolBarButtons.fontSize = 14;
                toolBarButtons.normal.textColor = Nexus.TextGreen;

                Nexus.Self.Button(RUNALL, "Launches all tests.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        Nexus.IsRunAll = true;
                        LaunchTests("all", "all");
                    }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
                extraToolCount++;

            }

            toolBarButtons.normal.textColor = Swat.WindowDefaultTextColor;
            toolBarButtons.fontSize = 22;
            if(_viewModeCategory) {

                //Collapse/Expand all foldouts.
                Nexus.Self.Button(expandCategories ? COLLAPSE : EXPAND, "Toggle expansion of all categories.",
                    new Nexus.SwatDelegate(delegate () {
                        expandCategories = !expandCategories;
                        for(int cat = 0; cat < Categories.Count; cat++) {
                            Categories[cat] = expandCategories;
                        }
                        for(int subCat = 0; subCat < _subCategoryBools.Count; subCat++) {
                            _subCategoryBools[subCat] = expandCategories;
                        }
                    }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
                extraToolCount++;

            }

            toolBarButtons.normal.textColor = Nexus.Overseer.pauseOnFailure ? Nexus.TextGreen : Color.red;
            toolBarButtons.fontSize = 16;
            toolBarButtons.fontStyle = FontStyle.Bold;
            string togglePauseOnError = Nexus.Overseer.pauseOnFailure ? "Currently ON - Editor play mode will pause when a test fails." : "Currently OFF - Test failures will not pause Unity Editor.";
            Nexus.Self.Button(PAUSE_ON_FAILURE, togglePauseOnError,
                new Nexus.SwatDelegate(delegate () {
                    Nexus.Overseer.pauseOnFailure = !Nexus.Overseer.pauseOnFailure;
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            //Soft restart. Value defaults to settings key stored in Trilleon config.
            toolBarButtons.normal.textColor = _resetOnStart ? Nexus.TextGreen : Color.red;
            toolBarButtons.fontSize = !Nexus.isPc ? 26 : 18;
            toolBarButtons.fontStyle = FontStyle.Bold;
            toolBarButtons.padding = !Nexus.isPc ? new RectOffset(0, 0, 0, 5) : new RectOffset(0, 4, 0, 0);
            string toggleReset = _resetOnStart ? "Currently ON - Account will reset when editor play mode starts." : "Currently OFF - Account will not be affected when editor play mode starts.";
            Nexus.Self.Button(SOFTRESET, toggleReset,
                new Nexus.SwatDelegate(delegate () {
                    _resetOnStart = !_resetOnStart;
                    Nexus.Overseer.softReset = _resetOnStart;
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            //Clear cached data toolbar button.
            toolBarButtons.normal.textColor = Color.black;
            toolBarButtons.fontSize = 19;
            toolBarButtons.padding = new RectOffset(0, 4, 2, 0);
            Nexus.Self.Button(CLEARCACHE, "Clears cached data, excluding test reports and last results data.",
                new Nexus.SwatDelegate(delegate () {
                    FileBroker.ClearCache();
                    TestMonitorHelpers.CreateTestObjectHelper();
                    SimpleAlert.Pop("Cache cleared.", null);
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            //Toggle Buddy Tests toolbar button.
            if(_ignoreBuddyTests) {
                toolBarButtons.normal.textColor = Color.red;
            } else {
                toolBarButtons.normal.textColor = Nexus.TextGreen;
            }
            toolBarButtons.fontSize = !Nexus.isPc ? 20 : 25;
            toolBarButtons.padding = !Nexus.isPc ? new RectOffset(0, 0, 0, 0) : new RectOffset(0, 0, 0, 5);
            Nexus.Self.Button(TOGGLEBUDDY, string.Format("Buddy tests will{0} be run.", _ignoreBuddyTests ? " NOT" : string.Empty),
                new Nexus.SwatDelegate(delegate () {
                    _ignoreBuddyTests = !_ignoreBuddyTests;
                    AutomationMaster.LockIgnoreBuddyTestsFlag = false;
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            /* TODO: Add when Unity fixes their broken assembly locking.
            //Toggle Buddy Tests toolbar button.
            if(LockAssemblies) {
                toolBarButtons.normal.textColor = Color.red; 
            } else {
                toolBarButtons.normal.textColor = Nexus.TextGreen; 
            }
            toolBarButtons.fontSize = !Nexus.isPc ? 19 : 19;
            toolBarButtons.padding = !Nexus.isPc ? new RectOffset(0, 4, 0, 2): new RectOffset(0, 4, 0, 2);
            Nexus.Self.Button(LockAssemblies ? RELOADLOCKON : RELOADLOCKOFF, string.Format("Toggle Assembly lock, allowing/stopping automatic recompile as you make edits to tests. Currently {0}.", LockAssemblies ? "Locked" : "Unlocked"), 
                new Nexus.SwatDelegate(delegate() {                
                    LockAssemblies = !LockAssemblies;
                    if(LockAssemblies) {

                        EditorApplication.LockReloadAssemblies();

                    } else {

                        EditorApplication.UnlockReloadAssemblies();

                    }
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;
            */

            //Validate framework toolbar button.
            if(AutomationMaster.Validated_Phase1 == null && AutomationMaster.Validated_Phase2 == null) {
                toolBarButtons.normal.textColor = Swat.WindowDefaultTextColor;
            } else if(AutomationMaster.Validated_Phase1 != null && AutomationMaster.Validated_Phase2 != null && (bool)AutomationMaster.Validated_Phase1 && (bool)AutomationMaster.Validated_Phase2) {
                toolBarButtons.normal.textColor = Nexus.TextGreen;
            } else if((AutomationMaster.Validated_Phase1 != null && (bool)AutomationMaster.Validated_Phase1) || (AutomationMaster.Validated_Phase2 != null && (bool)AutomationMaster.Validated_Phase2)) {
                toolBarButtons.normal.textColor = Color.yellow;
            } else {
                toolBarButtons.normal.textColor = Color.red;
            }
            toolBarButtons.fontSize = !Nexus.isPc ? 24 : 20; ;
            toolBarButtons.padding = !Nexus.isPc ? new RectOffset(0, 2, 0, 2) : new RectOffset(0, 4, 0, 0);
            Nexus.Self.Button(VALIDATE, "Validates Framework integrity at this moment using compiled code",
                new Nexus.SwatDelegate(delegate () {
                    Nexus.IsRunAll = false;
                    Nexus.Self.SelectTab(Nexus.Self.Console); //Auto select console tab to display validation progress.
                    AutoConsole.SaveConsoleMessagesForAppPlay();
                    LaunchTests("Trilleon/Validation", "class");
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            toolBarButtons.fontSize = 18;
            toolBarButtons.normal.textColor = Color.black;
            Nexus.Self.Button(FAVORITE, "Show Favorites test run options.",
                new Nexus.SwatDelegate(delegate () {
                    Nexus.Self.SelectTab(Nexus.Self.Favorites);
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            toolBarButtons.fontSize = 18;
            toolBarButtons.padding = new RectOffset(1, 0, 0, 0);
            Nexus.Self.Button(AutomationMaster.UnitTestMode ? INTEGRATION_TEST_VIEW : UNIT_TEST_VIEW, "Toggle between viewing integration tests and unit tests.",
                new Nexus.SwatDelegate(delegate () {
                    AutomationMaster.UnitTestMode = !AutomationMaster.UnitTestMode;
                    _reget = Update_Data = true;
                    Nexus.Self.Favorites.FavoritesList = new List<KeyValuePair<string, List<KeyValuePair<bool, string>>>>();
                    Nexus.Self.Favorites.Set();
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            toolBarButtons.fontSize = 20;
            toolBarButtons.normal.textColor = Swat.ToggleButtonTextColor;
            toolBarButtons.normal.background = Swat.ToggleButtonBackgroundTexture;
            toolBarButtons.padding = new RectOffset(0, 0, -1, 0);
            Nexus.Self.Button(VIEW_MODE, "Toggle between viewing tests in flat list or organized under category names.",
                new Nexus.SwatDelegate(delegate () {
                    _viewModeCategory = !_viewModeCategory;
                }), toolBarButtons, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });
            extraToolCount++;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if(!string.IsNullOrEmpty(selectedTest)) {

                GUILayout.BeginVertical(testStatusBox, new GUILayoutOption[] { GUILayout.MaxWidth(extraToolCount * 25) }); //Begin box border around test details.

                //Close this box/view.
                closeBox = new GUIStyle(GUI.skin.button);
                closeBox.fontSize = 14;
                closeBox.padding = new RectOffset(1, 0, 0, 0);
                closeBox.margin = new RectOffset(260, 0, 0, 0);
                closeBox.normal.textColor = Color.red;
                closeBox.normal.background = Swat.BoxAreaBackgroundTexture;

                Nexus.Self.Button("X", "Show/Hide test status details section.",
                    new Nexus.SwatDelegate(delegate () {
                        selectedTest = string.Empty;
                        lastSelectedTest = string.Empty;
                        hideBoxArea = !hideBoxArea;
                    }), closeBox, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) });

                if(selectedTest != lastSelectedTest || !_selectedMethodAttributes.Any() || _selectedMethodAttributes.Count != _selectedTestCategories.Count) {

                    SelectedMethodsGetData();
                    _selectedTestCategories = new List<bool>();
                    for(int c = 0; c < _selectedMethodAttributes.Count; c++) {
                        _selectedTestCategories.Add(true);
                    }

                }

                GUIStyle boxLabelMargin = new GUIStyle(GUI.skin.label);
                GUIStyle boxFoMargin = new GUIStyle(EditorStyles.foldout);
                boxFoMargin.margin = new RectOffset(10, 0, 2, 2);

                for(int s = 0; s < _selectedMethodAttributes.Count; s++) {

                    boxLabelMargin.padding = new RectOffset(10, 0, 2, 2);
                    int isInt = 0;
                    if(int.TryParse(_selectedMethodAttributes[s].Value, out isInt)) {

                        EditorGUILayout.LabelField(string.Format("{0}: {1}", _selectedMethodAttributes[s].Key, isInt), boxLabelMargin);
                        continue;

                    }

                    _selectedTestCategories[s] = Nexus.Self.Foldout(_selectedTestCategories[s], new GUIContent(_selectedMethodAttributes[s].Key), true, boxFoMargin);
                    if(_selectedTestCategories[s]) {

                        boxLabelMargin.padding = new RectOffset(25, 0, 0, 0);
                        string[] vals = _selectedMethodAttributes[s].Value.Split(',');
                        for(int attrs = 0; attrs < vals.Length; attrs++) {

                            EditorGUILayout.LabelField(vals[attrs], boxLabelMargin);

                        }

                    }

                }

                closeBox.normal.textColor = Swat.TextGreen;
                Nexus.Self.Button(RUNALL, "Run this individual test.",
                    new Nexus.SwatDelegate(delegate () {
                        LaunchTestDependenciesCheck(selectedTest, "test");
                    }), closeBox, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) });

                GUILayout.EndVertical(); //End box border around test details.
            }

            if(!hideBoxArea) {

                GUILayout.BeginVertical(testStatusBox, new GUILayoutOption[] { GUILayout.MaxWidth(extraToolCount * 25) }); //Begin box border around test status data.
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                b = new GUIStyle(GUI.skin.label);
                h = new GUIStyle(GUI.skin.label);
                h.fixedWidth = 50;
                h.fixedHeight = 20;
                Color32 toggleOffColor;
                Color32 toggleOnColor = toggleOffColor = new Color32(150, 150, 150, 255);
                int defaultFontSize = 12;
                b.fontSize = defaultFontSize;
                b.fontStyle = FontStyle.Bold;
                b.margin = Nexus.BaseRectOffset;

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                int passCount = AutomationReport.testsMeta.FindAll(x => {
                    return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "passed").Any();
                }).Count;
                b.normal.textColor = Nexus.TextGreen;
                b.padding = new RectOffset(-1, 0, 0, 0);
                EditorGUILayout.LabelField("Passed: ", b, new GUILayoutOption[] { GUILayout.Width(75) });
                b.padding = new RectOffset(0, 0, 0, 0);
                b.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField(passCount.ToString(), b, new GUILayoutOption[] { GUILayout.Width(40) });
                b.normal.textColor = Nexus.TextGreen;
                b.fontSize = 14;
                if(GUILayout.Button(PASSED, b, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    LaunchTestDependenciesCheck("Passed", "status");
                }
                h.padding = showPassed ? paddingShow : paddingHide;
                h.normal.textColor = showPassed ? toggleOnColor : toggleOffColor;
                h.fontSize = showPassed ? defaultFontSize + 15 : defaultFontSize + 8;
                if(GUILayout.Button(showPassed ? Nexus.TOGGLE_ON : Nexus.TOGGLE_OFF, h, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    showPassed = !showPassed;
                }
                if(Nexus.Self.MouseOver()) {
                    Nexus.Self.SetToolTip("Toggle visibility of tests that passed in their last run.");
                }
                b.fontSize = defaultFontSize;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                int failCount = AutomationReport.testsMeta.FindAll(x => {
                    return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "failed").Any();
                }).Count;
                b.normal.textColor = colorFailed;
                EditorGUILayout.LabelField("Failed: ", b, new GUILayoutOption[] { GUILayout.Width(75) });
                b.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField(failCount.ToString(), b, new GUILayoutOption[] { GUILayout.Width(40) });
                b.normal.textColor = colorFailed;
                b.fontSize = 15;
                if(GUILayout.Button(FAILED, b, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    LaunchTestDependenciesCheck("Failed", "status");
                }
                h.padding = showFailed ? paddingShow : paddingHide;
                h.normal.textColor = showFailed ? toggleOnColor : toggleOffColor;
                h.fontSize = showFailed ? defaultFontSize + 15 : defaultFontSize + 8;
                if(GUILayout.Button(showFailed ? Nexus.TOGGLE_ON : Nexus.TOGGLE_OFF, h, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    showFailed = !showFailed;
                }
                if(Nexus.Self.MouseOver()) {
                    Nexus.Self.SetToolTip("Toggle visibility of tests that failed in their last run.");
                }
                b.fontSize = defaultFontSize;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                int skipCount = AutomationReport.testsMeta.FindAll(x => {
                    return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "skipped").Any();
                }).Count;
                b.normal.textColor = colorSkipped;
                EditorGUILayout.LabelField("Skipped: ", b, new GUILayoutOption[] { GUILayout.Width(75) });
                b.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField(skipCount.ToString(), b, new GUILayoutOption[] { GUILayout.Width(40) });
                b.normal.textColor = colorSkipped;
                b.fontSize = 15;
                if(GUILayout.Button(SKIPPED, b, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    LaunchTestDependenciesCheck("Skipped", "status");
                }
                h.padding = showSkipped ? paddingShow : paddingHide;
                h.normal.textColor = showSkipped ? toggleOnColor : toggleOffColor;
                h.fontSize = showSkipped ? defaultFontSize + 15 : defaultFontSize + 8;
                if(GUILayout.Button(showSkipped ? Nexus.TOGGLE_ON : Nexus.TOGGLE_OFF, h, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    showSkipped = !showSkipped;
                }
                if(Nexus.Self.MouseOver()) {
                    Nexus.Self.SetToolTip("Toggle visibility of tests that were skipped in their last run.");
                }
                b.fontSize = defaultFontSize;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                int ignoreCount = AutomationReport.testsMeta.FindAll(x => {
                    return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "ignored").Any();
                }).Count;
                b.normal.textColor = colorIgnore;
                EditorGUILayout.LabelField("Ignored: ", b, new GUILayoutOption[] { GUILayout.Width(75) });
                b.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField(ignoreCount.ToString(), b, new GUILayoutOption[] { GUILayout.Width(40) });
                b.padding = new RectOffset(0, 0, 4, 0);
                b.normal.textColor = colorIgnore;
                b.fontSize = 10;
                b.fontStyle = FontStyle.Bold;
                if(GUILayout.Button(IGNORED, b, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    LaunchTestDependenciesCheck("Ignored", "status");
                }
                h.padding = showIgnored ? paddingShow : paddingHide;
                h.normal.textColor = showIgnored ? toggleOnColor : toggleOffColor;
                h.fontSize = showIgnored ? defaultFontSize + 15 : defaultFontSize + 8;
                if(GUILayout.Button(showIgnored ? Nexus.TOGGLE_ON : Nexus.TOGGLE_OFF, h, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    showIgnored = !showIgnored;
                }
                if(Nexus.Self.MouseOver()) {
                    Nexus.Self.SetToolTip("Toggle visibility of tests that were ignored in their last run.");
                }
                b.fontSize = defaultFontSize;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                int noDataCount = AutomationReport.testsMeta.Count - (passCount + failCount + ignoreCount + skipCount);
                b.normal.textColor = Color.grey;
                EditorGUILayout.LabelField("No Data: ", b, new GUILayoutOption[] { GUILayout.Width(75) });
                b.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField(noDataCount.ToString(), b, new GUILayoutOption[] { GUILayout.Width(40) });
                b.normal.textColor = Color.grey;
                b.padding = new RectOffset(0, 0, 2, 0);
                b.fontSize = 18;
                if(GUILayout.Button(NODATA, b, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    LaunchTestDependenciesCheck("NoData", "status");
                }
                h.padding = showNoData ? new RectOffset(0, 0, -4, 0) : new RectOffset(2, 0, 0, 0);
                h.normal.textColor = showNoData ? toggleOnColor : toggleOffColor;
                h.fontSize = showNoData ? defaultFontSize + 15 : defaultFontSize + 8;
                if(GUILayout.Button(showNoData ? Nexus.TOGGLE_ON : Nexus.TOGGLE_OFF, h, new GUILayoutOption[] { GUILayout.Width(40) })) {
                    showNoData = !showNoData;
                }
                if(Nexus.Self.MouseOver()) {
                    Nexus.Self.SetToolTip("Toggle visibility of tests that lack previous run data.");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUI.indentLevel--;

                GUILayout.EndVertical(); //End box border around test status data and launch buttons.

            }

            if(_viewModeCategory) {

                if(Update_Data) {

                    GetTestMethods();
                    List<string> methodsToNamesList = new List<string>();
                    for(int m = 0; m < _methods.Count; m++) {

                        methodsToNamesList.Add(_methods[m].Name);

                    }
                    if(_allMethodsLast.GetUniqueObjectsBetween(methodsToNamesList).Any()) {

                        _reget = true;

                    }

                }

                if(_reget) {

                    _reget = false;
		            _firstPass = true;
                    GetTestMethods();
                    _allMethodsLast = _allMethods;
                    SetTestData();
                    int lastCategoryCount = _categoryTests.Count;
                    _categoryTests = new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>>();

                    for(int i = 0; i < _allMethodsLast.Count; i++) {

                        MethodInfo methodInfo = _all.Find(x => x.Value.Name == _allMethodsLast[i]).Value;
                        if(AutomationMaster.UnitTestMode) {

                            UnityTest[] ut = (UnityTest[])Attribute.GetCustomAttributes(methodInfo, typeof(UnityTest));
                            for(int u = 0; u < ut.Length; u++) {

                                //Don not consider reserverved categories.
                                if(AutomationMaster.ConfigReader.GetStringList("RESERVED_CATEGORY_NAMES").Any() && AutomationMaster.ConfigReader.GetStringList("RESERVED_CATEGORY_NAMES").FindAll(x => ut[u].CategoryName.StartsWith(x)).Any()) {

                                    continue;

                                }
                                string[] subCategory = ut[u].CategoryName.Split('/');

                                List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> thisVal = _categoryTests.FindAll(x => x.Key == subCategory[0]);
                                List<KeyValuePair<string, List<KeyValuePair<string, string>>>> existingSubset = thisVal.Any() ? thisVal.First().Value.FindAll(x => x.Key == subCategory[subCategory.Length == 1 ? 0 : 1]) : new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();

                                List<KeyValuePair<string, string>> methods = existingSubset.Any() ? existingSubset.First().Value : new List<KeyValuePair<string, string>>();
                                methods.Add(new KeyValuePair<string, string>(_allMethodsLast[i], string.Empty));
                                methods = methods.OrderByKeys();

                                List<KeyValuePair<string, List<KeyValuePair<string, string>>>> subset = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
                                subset.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(subCategory[subCategory.Length == 1 ? 0 : 1], GetMethodStatuses(methods)));

                                if(thisVal.Any()) {

                                    List<KeyValuePair<string, List<KeyValuePair<string, string>>>> otherSubCategoryGroupingsSansStatuses = _categoryTests.FindAll(x => x.Key == subCategory[0]).First().Value.FindAll(y => y.Key != subCategory[subCategory.Length == 1 ? 0 : 1]);
                                    if(otherSubCategoryGroupingsSansStatuses.Any()) {

                                        for(int s = 0; s < otherSubCategoryGroupingsSansStatuses.Count; s++) {

                                            List<KeyValuePair<string, string>> SubCategoryTests = GetMethodStatuses(otherSubCategoryGroupingsSansStatuses[s].Value).OrderByKeys();
                                            subset.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(otherSubCategoryGroupingsSansStatuses[s].Key, SubCategoryTests));

                                        }

                                    }

                                }
                                subset = subset.OrderByKeys();

                                //Add to _categoryTests, which is used to build the foldouts. The sub cats should not appear as a top level foldout, so we only add the sub category to _categorySubcategoryTests, which is used to build the dropdown for category test launches (the only place where a sub category is treated the same as a top level category).
                                KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>> thisEntry = new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>(subCategory[0], subset);
                                _categoryTests = _categoryTests.FindAll(x => x.Key != subCategory[0]); //Remove the original entry.
                                _categorySubcategoryTests = _categorySubcategoryTests.FindAll(x => x.Key != subCategory[0]); //Remove the original entry.
                                _categoryTests.Add(thisEntry); //Add new or updated entry.
                                _categorySubcategoryTests.Add(thisEntry); //Add new or updated entry.
                                if(subCategory.Length > 1) {

                                    _categorySubcategoryTests = _categorySubcategoryTests.FindAll(x => x.Key != subCategory[1]); //Remove any existing entry for this sub category.
                                    _categorySubcategoryTests.Add(new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>(subCategory[1], subset)); //Add new or updated entry.

                                }

                            }

                        } else {

                            Automation[] aut = (Automation[])Attribute.GetCustomAttributes(methodInfo, typeof(Automation));
                            for(int a = 0; a < aut.Length; a++) {

                                //Don not consider reserverved categories.
                                if(AutomationMaster.ConfigReader.GetStringList("RESERVED_CATEGORY_NAMES").Any() && AutomationMaster.ConfigReader.GetStringList("RESERVED_CATEGORY_NAMES").FindAll(x => aut[a].CategoryName.StartsWith(x)).Any()) {

                                    continue;

                                }
                                string[] subCategory = aut[a].CategoryName.Split('/');

                                List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>> thisVal = _categoryTests.FindAll(x => x.Key == subCategory[0]);
                                List<KeyValuePair<string, List<KeyValuePair<string, string>>>> existingSubset = thisVal.Any() ? thisVal.First().Value.FindAll(x => x.Key == subCategory[subCategory.Length == 1 ? 0 : 1]) : new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();

                                List<KeyValuePair<string, string>> methods = existingSubset.Any() ? existingSubset.First().Value : new List<KeyValuePair<string, string>>();
                                methods.Add(new KeyValuePair<string, string>(_allMethodsLast[i], string.Empty));
                                methods = methods.OrderByKeys();

                                List<KeyValuePair<string, List<KeyValuePair<string, string>>>> subset = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
                                subset.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(subCategory[subCategory.Length == 1 ? 0 : 1], GetMethodStatuses(methods)));

                                if(thisVal.Any()) {

                                    List<KeyValuePair<string, List<KeyValuePair<string, string>>>> otherSubCategoryGroupingsSansStatuses = _categoryTests.FindAll(x => x.Key == subCategory[0]).First().Value.FindAll(y => y.Key != subCategory[subCategory.Length == 1 ? 0 : 1]);
                                    if(otherSubCategoryGroupingsSansStatuses.Any()) {

                                        for(int s = 0; s < otherSubCategoryGroupingsSansStatuses.Count; s++) {

                                            List<KeyValuePair<string, string>> SubCategoryTests = GetMethodStatuses(otherSubCategoryGroupingsSansStatuses[s].Value).OrderByKeys();
                                            subset.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(otherSubCategoryGroupingsSansStatuses[s].Key, SubCategoryTests));

                                        }

                                    }

                                }
                                subset = subset.OrderByKeys();

                                //Add to _categoryTests, which is used to build the foldouts. The sub cats should not appear as a top level foldout, so we only add the sub category to _categorySubcategoryTests, which is used to build the dropdown for category test launches (the only place where a sub category is treated the same as a top level category).
                                KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>> thisEntry = new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>(subCategory[0], subset);
                                _categoryTests = _categoryTests.FindAll(x => x.Key != subCategory[0]); //Remove the original entry.
                                _categorySubcategoryTests = _categorySubcategoryTests.FindAll(x => x.Key != subCategory[0]); //Remove the original entry.
                                _categoryTests.Add(thisEntry); //Add new or updated entry.
                                _categorySubcategoryTests.Add(thisEntry); //Add new or updated entry.
                                if(subCategory.Length > 1) {

                                    _categorySubcategoryTests = _categorySubcategoryTests.FindAll(x => x.Key != subCategory[1]); //Remove any existing entry for this sub category.
                                    _categorySubcategoryTests.Add(new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>(subCategory[1], subset)); //Add new or updated entry.

                                }

                            }

                        }

                    }

                    //Sort all by cateogry name alphabetically.
                    _categoryTests = _categoryTests.OrderByKeys();

                    if((Categories.Count != lastCategoryCount && lastCategoryCount != 0) || _firstPass) {

                        Categories = new List<bool>();

                        for(int x = 0; x < _categoryTests.Count; x++) {

                            Categories.Add(false);

                        }

                    }

                    _firstPass = false;

                }

                if(_categoryTests.Last().Key.ToLower() != "debug" || _categoryTests.Last().Key.ToLower() != "<debug>") {

                    //Pull out Debug test category, and then re-append it to the end of the list. It should always appear last.
                    KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>> debugCategory = _categoryTests.Find(d => d.Key.ToLower() == "debug" || d.Key.ToLower() == "<debug>");
                    _categoryTests.Remove(debugCategory);
                    _categoryTests.Add(new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, string>>>>>("<Debug>", debugCategory.Value));

                }

                EditorGUILayout.Space();

                GUIStyle launchCatLabel = new GUIStyle(GUI.skin.label);
                launchCatLabel.padding = new RectOffset(10, 0, 0, 0);
                launchCatLabel.normal.textColor = Swat.WindowDefaultTextColor;
                EditorGUILayout.LabelField("Launch Category: ", launchCatLabel);

                if(Update_Data) {

                    catKeys = new List<string>();
                    for(int c = 0; c < _categoryTests.Count; c++) {

                        //See if top-level category has any test assigned directly to it, along with sub categories. If this is the case...
                        //... add an option to launch ALL tests recursively under the parent category, or to ignore sub category tests.
                        if(_categoryTests[c].Value.FindAll(x => x.Key == _categoryTests[c].Key).Any() && _categoryTests[c].Value.Count > 1) {

                            catKeys.Add(string.Format("{0} (All)", _categoryTests[c].Key));
                            catKeys.Add(string.Format("{0} (Top)", _categoryTests[c].Key));

                        } else {

                            catKeys.Add(_categoryTests[c].Key);

                        }

                        for(int sc = 0; sc < _categoryTests[c].Value.Count; sc++) {

                            if(_categoryTests[c].Value[sc].Key != _categoryTests[c].Key) {

                                catKeys.Add(string.Format("{0} ({1})", _categoryTests[c].Key, _categoryTests[c].Value[sc].Key));

                            }

                        }

                    }
                    //Prepend Favorites to category list.
                    if(Nexus.Self.Favorites.FavoritesList.Count == 0) {

                        Nexus.Self.Favorites.Set();

                    }
                    List<string> favorites = Nexus.Self.Favorites.FavoritesList.ExtractListOfKeysFromKeyValList();
                    if(favorites.Any()) {

                        for(int f = 0; f < favorites.Count; f++) {

                            favorites[f] = string.Format("*{0}", favorites[f]);

                        }

                        favorites.Sort();
                        catKeys = catKeys.PrependRange(favorites);

                    }

                }
                _selectedCategory = Nexus.Self.DropDown(_selectedCategory, catKeys.ToArray(), 12);
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal(buttonGroup);

                Nexus.Self.Button("Go", "Launches category tests.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        if(!AutomationMaster.Busy) {

                            if(catKeys[_selectedCategory].StartsWith("*")) {

                                //This is a Favorite list, and not a true Category. Gather requested tests/classes.
                                List<KeyValuePair<bool, string>> favoriteList = Nexus.Self.Favorites.FavoritesList.Find(x => x.Key == catKeys[_selectedCategory].Replace("*", string.Empty)).Value;
                                string commandClasses = string.Empty;
                                string commandTests = string.Empty;
                                for(int x = 0; x < favoriteList.Count; x++) {

                                    //Is the next item in the list a test? If so, this category is only meant to define the tests that follow, so ignore it.
                                    if(favoriteList[x].Key && x + 1 < favoriteList.Count ? !favoriteList[x + 1].Key : false) {

                                        continue;

                                    }

                                    if(favoriteList[x].Key) {

                                        //All Tests In This Class
                                        string category = string.Empty;
                                        if(favoriteList[x].Value.Contains("(")) {
                                            category = favoriteList[x].Value.Replace("*", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split('(')[1].Trim(')');
                                        } else {
                                            category = favoriteList[x].Value.Replace("*", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty);
                                        }
                                        commandClasses += string.Format("{0},", category);

                                    } else {

                                        //Just This Test
                                        commandTests += string.Format("{0},", favoriteList[x].Value);

                                    }

                                }
                                string command = string.Format("&&{0}%{1}", commandClasses.Trim(','), commandTests.Trim(',')); ;
                                Nexus.Self.Tests.LaunchTests(command, "mix");

                            } else {

                                string categoryParsed = catKeys[_selectedCategory].Replace("<", string.Empty).Replace(">", string.Empty);
                                if(categoryParsed.Contains("(All)")) {
                                    categoryParsed = categoryParsed.Split('(')[0].Trim();
                                } else if(categoryParsed.Contains("(Top)")) {
                                    categoryParsed = string.Format("{0}^", categoryParsed.Split('(')[0].Trim());
                                } else if(categoryParsed.Contains("(")) {
                                    categoryParsed = categoryParsed.Split('(')[1].Trim(')');
                                }
                                LaunchTestDependenciesCheck(categoryParsed, "category");

                            }

                        } else {

                            if(Application.isPlaying) {

                                SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);

                            }

                        }
                    }), null, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(25) });

                GUILayout.Space(-2f);

                Nexus.Self.Button("Subset", "Launches selected subset of category tests.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        if(!AutomationMaster.Busy) {
                            AutomationMaster.LaunchType = LaunchType.CategoryName;
                            string categoryParsed = catKeys[_selectedCategory].Replace("<", string.Empty).Replace(">", string.Empty);
                            if(categoryParsed.Contains("(")) {
                                categoryParsed = categoryParsed.Split('(')[1].Trim(')');
                            }
                            if(categoryParsed.ToLower() == "all") {
                                categoryParsed = catKeys[_selectedCategory].Split('(').First().Trim();
                            }
                            RunTestsAlert.Pop(Nexus.AutoMaster.GetAllMethodsToRun(GetLaunchCommand(categoryParsed, "category")), null, catKeys[_selectedCategory].Replace("<", string.Empty).Replace(">", string.Empty), "category");
                        } else {
                            if(Application.isPlaying) {
                                SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);
                            }
                        }
                    }), null, new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(25) });

                GUILayout.Space(-2);

                Nexus.Self.Button("Multiple", "Choose multiple categories to launch. Selected categories launch ALL children.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        if(!AutomationMaster.Busy) {
                            AutomationMaster.LaunchType = LaunchType.CategoryName;
                            RunClassesAlert.Pop(catKeys);
                        } else {
                            if(Application.isPlaying) {
                                SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);
                            }
                        }
                    }), null, new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(25) });

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                if(Update_Data) {

                    if(_categoryTests.Any()) {

                        _categoryTests = _categoryTests.OrderByKeys();

                    }

                }

                for(int z = 0; z < Categories.Count; z++) {

                    //Ignore rendering of empty categories
                    if(_categoryTests.Count <= z) {

                        continue;

                    }

                    EditorGUILayout.Space();
                    fo.fontSize = 12;
                    fo.normal.textColor = Swat.WindowDefaultTextColor;
                    fo.margin = new RectOffset(10, 0, 5, 0);

                    //Skip this category's rendering if no test methods contained in it match the filter field string.
                    List<string> allTestsInSingleCat = new List<string>();
                    for(int ca = 0; ca < _categoryTests[z].Value.Count; ca++) {

                        allTestsInSingleCat.AddRange(_categoryTests[z].Value[ca].Value.ExtractListOfKeysFromKeyValList());

                    }

                    if(!string.IsNullOrEmpty(_filterField)) {

                        if(!allTestsInSingleCat.FindAll(x => x.ToLower().Contains(_filterField.ToLower())).Any()) {

                            continue;

                        }

                    }

                    //Skip this category's rendering if no test methods contained in it have a status that has visibility toggled on.
                    List<string> showStatuses = new List<string>();
                    if(showIgnored) {

                        showStatuses.Add(ignored);

                    }
                    if(showPassed) {

                        showStatuses.Add(passed);

                    }
                    if(showSkipped) {

                        showStatuses.Add(skipped);

                    }
                    if(showFailed) {

                        showStatuses.Add(failed);

                    }
                    if(showNoData) {

                        showStatuses.Add(noData);

                    }

                    //TODO: Fix this logic!
                    /*
                    //Skip this category's rendering if no test methods contained in it have a status that has visibility toggled on.
                    if(AutomationReport.testsMeta.Any()) {

                        //If any of these tests are not included in AutomationReport.testsMeta, then they are No Data tests that should be displayed. 
                        if(_categoryTests[z].Value[a].Value.FindAll(x => AutomationReport.testsMeta.FindAll(y => y.Key == x.Key).Count <= _categoryTests[z].Value[a].Value.Count).Any()) {

                            continue;

                        }

                    }
                    */

                    Categories[z] = Nexus.Self.Foldout(Categories[z], string.Format("  {0}", _categoryTests[z].Key), true, fo);

                    EditorGUI.indentLevel++;

                    if(Categories[z]) {

                        if(!AutomationReport.testsMeta.Any() && DateTime.Now.Subtract(_lastResultsRetrieval).TotalSeconds > _redrawRateSeconds) {

                            AutomationReport.GetMostRecentsResults();
                            _lastResultsRetrieval = DateTime.Now;

                        }

                        for(int a = 0; a < _categoryTests[z].Value.Count; a++) {

                            string SubcatName = _categoryTests[z].Key.Replace("<", string.Empty).Replace(">", string.Empty); //Remove debug and system indicator chars from name for string comparison purposes.
                                                                                                                             //Is a sub-category.
                            if(SubcatName != _categoryTests[z].Value[a].Key) {

                                if(!string.IsNullOrEmpty(_filterField)) {

                                    //Skip this category's rendering if no test methods contained in it match the filter field string.
                                    if(!_categoryTests[z].Value[a].Value.ExtractListOfKeysFromKeyValList().FindAll(x => x.ToLower().Contains(_filterField.ToLower())).Any()) {

                                        continue;

                                    }

                                }

                                int index = 0;
                                if(!_subCategoryNames.Contains(_categoryTests[z].Value[a].Key)) {

                                    _subCategoryNames.Add(_categoryTests[z].Value[a].Key);
                                    _subCategoryBools.Add(false);
                                    index = _subCategoryNames.Count - 1;

                                } else {

                                    index = _subCategoryNames.FindIndex(x => x == _categoryTests[z].Value[a].Key);

                                }

                                _subCategoryBools[index] = Nexus.Self.Foldout(_subCategoryBools[index], string.Format("  ↳ {0}", _categoryTests[z].Value[a].Key), true, fo);
                                if(_subCategoryBools[index]) {

                                    for(int m = 0; m < _categoryTests[z].Value[a].Value.Count; m++) {

                                        RenderTest(_categoryTests[z].Value[a].Value[m].Key, fo, Swat.WindowDefaultTextColor, true);

                                    }

                                }

                            } else {

                                for(int m = 0; m < _categoryTests[z].Value[a].Value.Count; m++) {
                                    RenderTest(_categoryTests[z].Value[a].Value[m].Key, fo, Swat.WindowDefaultTextColor, false);
                                }

                            }

                        }

                        EditorGUILayout.Space();

                    }

                    EditorGUI.indentLevel--;

                }

            } else {

                GUILayout.Space(15);
                fo.fontSize = 12;
                if(!_allMethods.Any() || _reget) {

                    GetTestMethods();
                    SetTestData();

                }
                _allMethods.Sort();

                EditorGUILayout.BeginHorizontal(buttonGroup);

                Nexus.Self.Button("Multiple", "Launches selected subset of tests.",
                    new Nexus.SwatDelegate(delegate () {
                        loopModeActive = false;
                        if(!AutomationMaster.Busy) {
                            if(AutomationMaster.AllMethodsInFramework.Count == 0) {
                                AutomationMaster.SetAllMethodsInFramework();
                            }
                            //Filter debug tests from the list provided here.
                            List<KeyValuePair<string, MethodInfo>> allMethods = AutomationMaster.AllMethodsInFramework.FindAll(x => {
                                if(AutomationMaster.UnitTestMode) {
                                    List<Automation> auts = ((Automation[])x.Value.GetCustomAttributes(typeof(Automation), false)).ToList();
                                    return !auts.FindAll(a => a.CategoryName.ToLower().StartsWith("debug")).Any();
                                } else {
                                    List<UnityTest> uts = ((UnityTest[])x.Value.GetCustomAttributes(typeof(UnityTest), false)).ToList();
                                    return !uts.FindAll(u => u.CategoryName.ToLower().StartsWith("debug")).Any();
                                }
                            });
                            RunTestsAlert.Pop(allMethods, null, "all", "Multiple Tests", isTestSubset: true);
                        } else {
                            if(Application.isPlaying) {
                                SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);
                            }
                        }
                    }), null, new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(25) });

                Nexus.Self.Button("Loop", "Loops selected test the requested number of times.",
                    new Nexus.SwatDelegate(delegate () {
                        if(!AutomationMaster.Busy) {
                            loopModeActive = !loopModeActive;
                        } else {
                            if(Application.isPlaying) {
                                SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);
                            }
                        }
                    }), null, new GUILayoutOption[] { GUILayout.Width(70), GUILayout.Height(25) });
                EditorGUILayout.EndHorizontal();

                if(loopModeActive) {

                    GUIStyle loopArea = new GUIStyle();
                    loopArea.fixedWidth = 50;
                    loopArea.margin = new RectOffset(10, 0, 5, 0);
                    EditorGUILayout.BeginHorizontal(loopArea);
                    EditorGUILayout.LabelField("Loop Times", new GUILayoutOption[] { GUILayout.MaxWidth(60) });
                    GUIStyle loopCountField = new GUIStyle(GUI.skin.textField);
                    loopCountField.fixedWidth = 20;
                    loopCounter = EditorGUILayout.IntField(loopCounter, loopCountField);
                    GUIStyle notice = new GUIStyle(GUI.skin.label);
                    notice.normal.textColor = Color.magenta;
                    notice.margin = new RectOffset(-5, 0, 0, 0);
                    GUILayout.Space(-30);
                    EditorGUILayout.LabelField("Select test to loop.", notice);
                    EditorGUILayout.EndHorizontal();

                }
                GUILayout.Space(15);

                //If test view.
                for(int t = 0; t < _allMethods.Count; t++) {

                    RenderTest(_allMethods[t], fo, Swat.WindowDefaultTextColor, false, true);

                }

            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Update_Data = false; //Reset;

            if(AutomationMaster.Busy && Application.isPlaying && !_hardStop) {

                GUIStyle load = new GUIStyle(GUI.skin.box);
                load.normal.background = Swat.MakeTextureFromColor((Color)new Color32(75, 75, 75, 255));
                load.normal.textColor = Color.white;
                load.fontSize = 24;
                load.alignment = TextAnchor.MiddleLeft;

                //This button is the height of the window. Subtract the height of the (3) characters "<". 
                //Then, divide by how many new-line paddings are needed (4) between each character and the top/bottom of the window.
                int newLineCountPieces = Convert.ToInt32(Math.Round(((Nexus.Self.position.height - load.fontSize * 3) / 4) / load.fontSize, 0));
                string newLinePadding = ("\n").Duplicate(newLineCountPieces);
                string buttonText = string.Format("{0}<{0}<{0}<{0}", newLinePadding);
                //, new GUILayoutOption[] { GUILayout.Width(35), GUILayout.Height(Nexus.Self.position.height) }
                if(GUI.Button(new Rect(Nexus.Self.position.width - 35, 0, 35, Nexus.Self.position.height), buttonText, load)) {

                    Nexus.Self.SelectTab(Nexus.Self.Manifest);

                }

            }

        }

        void GetTestMethods() {

            AutomationMaster.SetAllMethodsInFramework();
            _all = new List<KeyValuePair<Type, MethodInfo>>();
            _allMethods = new List<string>();
            _methods = new List<MethodInfo>();
            List<Type> types = AutomationMaster.UnitTestMode ? AutomationMaster.GetUnityTestClasses() : AutomationMaster.GetAutomationClasses(true);
            for(int i = 0; i < types.Count; i++) {

                if(AutomationMaster.UnitTestMode) {

                    _methods.AddRange(types[i].GetMethods().ToList().FindAll(y => y.GetCustomAttributes(typeof(UnityTest), false).Length > 0));

                } else {

                    _methods.AddRange(types[i].GetMethods().ToList().FindAll(y => y.GetCustomAttributes(typeof(Automation), false).Length > 0));

                }

            }

        }

        /// <summary>
        /// Add test methods and their declaring types to list for later use.
        /// </summary>
        void SetTestData() {

            for(int x = 0; x < _methods.Count; x++) {

                _all.Add(new KeyValuePair<Type, MethodInfo>(_methods[x].DeclaringType, _methods[x]));
                _allMethods.Add(_methods[x].Name);

            }

        }

        /// <summary>
        /// Gather all primary test and class attributes applied to the selected test. This will be used for display in the test details box.
        /// </summary>
        void SelectedMethodsGetData() {

            _selectedMethodAttributes = new List<KeyValuePair<string, string>>();
            lastSelectedTest = selectedTest;
            if(!_all.Any()) {

                SetTestData();

            }
            if(!_all.Any()) {

                return;

            }

            hideBoxArea = true;
            MethodInfo method = _all.Find(x => x.Value.Name == selectedTest).Value;
            Type testClass = _all.Find(x => x.Value.Name == selectedTest).Key;

            Tag t = (Tag)Attribute.GetCustomAttributes(method, typeof(Tag)).First();
            if(t != null) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Tags", string.Join(",", t.Notifications.ToArray())));

            }

            if(AutomationMaster.UnitTestMode) {

                List<UnityTest> ut = Attribute.GetCustomAttributes(method, typeof(UnityTest)).ToList().ConvertAll(x => (UnityTest)x);
                if(ut.Any()) {

                    List<string> tests = new List<string>();
                    for(int u = 0; u < ut.Count; u++) {

                        tests.Add(ut[u].CategoryName);

                    }
                    _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Automation", string.Join(",", tests.ToArray())));

                }

            } else {

                List<Automation> aut = Attribute.GetCustomAttributes(method, typeof(Automation)).ToList().ConvertAll(x => (Automation)x);
                if(aut.Any()) {

                    List<string> tests = new List<string>();
                    for(int a = 0; a < aut.Count; a++) {

                        tests.Add(aut[a].CategoryName);

                    }
                    _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Automation", string.Join(",", tests.ToArray())));

                }

            }

            List<DependencyWeb> dw = Attribute.GetCustomAttributes(method, typeof(DependencyWeb)).ToList().ConvertAll(x => (DependencyWeb)x);
            if(dw.Any()) {

                List<string> dependencies = new List<string>();
                for(int d = 0; d < dw.Count; d++) {

                    dependencies.AddRange(dw[d].Dependencies);
                    dependencies.AddRange(dw[d].OneOfDependencies);

                }
                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Dependency Web", string.Join(",", dependencies.ToArray())));

            }
            List<DependencyClass> dc = testClass.GetCustomAttributes(typeof(DependencyClass), false).ToList().ConvertAll(x => (DependencyClass)x);
            if(dc.Any()) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Dependency Class", dc.First().order.ToString()));

            }
            List<DependencyTest> dt = Attribute.GetCustomAttributes(method, typeof(DependencyTest)).ToList().ConvertAll(x => (DependencyTest)x);
            if(dt.Any()) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Dependency Test", dt.First().order.ToString()));

            }
            List<BuddySystem> bs = Attribute.GetCustomAttributes(method, typeof(BuddySystem)).ToList().ConvertAll(x => (BuddySystem)x);
            if(bs.Any()) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Buddy System", string.Format("{0}{1}{2}", Enum.GetName(typeof(Buddy), bs.First().buddy), bs.First().buddy == Buddy.Action ? string.Empty : ": ", bs.First().ReactionOf)));

            }
            List<TestRunnerFlags> tf = Attribute.GetCustomAttributes(method, typeof(TestRunnerFlags)).ToList().ConvertAll(x => (TestRunnerFlags)x);
            if(tf.Any()) {

                List<string> flags = new List<string>();
                for(int a = 0; a < tf.First().Flags.Count; a++) {

                    flags.Add(Enum.GetName(typeof(TestFlag), tf.First().Flags[a]));

                }
                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Test Flags", string.Join(",", flags.ToArray())));

            }
            Tag tag = (Tag)Attribute.GetCustomAttributes(method, typeof(Tag)).First();
            if(tag != null) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Tags", string.Join(",", tag.Notifications.ToArray())));

            }
            TestRails tr = (TestRails)Attribute.GetCustomAttributes(method, typeof(TestRails)).First();
            if(tr != null) {

                _selectedMethodAttributes.Add(new KeyValuePair<string, string>("Test Rails", tr.RunId.ToString()));

            }

        }

        /// <summary>
        /// Find whatever dependencies are declared for the given test(s), and present them to the user for potential inclusion in the run.
        /// </summary>
        /// <param name="name">Launch command name (May be a single test, a category name, or a comma-delmited list of either.</param>
        /// <param name="type">Type of run, telling logic if the name is a test, a category, a comma-delmited list of either, or a reserved command such as "all".</param>
        void LaunchTestDependenciesCheck(string name, string type) {

            string launchCommand = GetLaunchCommand(name, type);

            //If a status was selected that has no tests matching the status, then do nothing.
            if(string.IsNullOrEmpty(launchCommand)) {

                return;

            }

            if(launchCommand.Substring(0, 1) == "*") {

                launchCommand = launchCommand.Replace("*", string.Empty);
                AutomationMaster.LaunchType = LaunchType.MethodName;

            } else if(launchCommand.Contains(",")) {

                AutomationMaster.LaunchType = LaunchType.MultipleMethodNames;

            } else if(type == "all") {

                AutomationMaster.LaunchType = LaunchType.All;

            } else {

                AutomationMaster.LaunchType = LaunchType.CategoryName;

            }

            AutomationMaster.DisregardDependencies = false;
            List<KeyValuePair<string, MethodInfo>> requestedMethods = Nexus.AutoMaster.GetAllMethodsToRun(launchCommand);
            List<KeyValuePair<string, MethodInfo>> methodsToRunAfterMappingDependencies = Nexus.AutoMaster.GatherAllTestsThatNeedToBeRunToSatisfyAllDependenciesForPartialTestRun(requestedMethods);
            List<KeyValuePair<string, MethodInfo>> dependents = methodsToRunAfterMappingDependencies.GetUniqueObjectsBetween(requestedMethods);

            if(dependents.Count > 0) {

                //If the only resulting dependencies are tests that have already run, then do not show the alert.
                List<string> completedTestMethods = new List<string>();
                for(int x = 0; x < dependents.Count; x++) {

                    completedTestMethods.Add(dependents[x].Value.Name);

                }

                if(AutomationMaster.EntireUnitySessionCompletedTests.GetUniqueObjectsBetween(completedTestMethods).Count > 0) {

                    RunTestsAlert.Pop(requestedMethods, methodsToRunAfterMappingDependencies.GetUniqueObjectsBetween(requestedMethods), name, type);

                } else {

                    Nexus.IsRunAll = AutomationMaster.LaunchType == LaunchType.All;
                    AutomationMaster.DisregardDependencies = true;
                    LaunchTests(name, type);

                }

            } else {

                Nexus.IsRunAll = false;
                LaunchTests(name, type);

            }

        }

        public void LaunchTests(string name, string type) {

            Nexus.Self.ShowLoading("Launching...");
            _hardStop = false;
            string launchLine = GetLaunchCommand(name, type);

            //If editor is not playing, save launch instructions and launch editor play mode.
            if(!EditorApplication.isPlaying) {

                SaveEditorSettings();
                FileBroker.SaveNonUnityTextResource(FileResource.LaunchInstructions, string.Format("{0}{1}{2}{3}{1}{4}", launchLine, AutomationMaster.DELIMITER, Nexus.Overseer.Master_Editor_Override.Value, loopModeActive ? string.Format("|{0}@{1}", name, loopCounter) : string.Empty, AutomationMaster.UnitTestMode ? "U" : "A"), true);
                AssetDatabase.Refresh();
                EditorApplication.ExecuteMenuItem("Edit/Play");

            } else {

                //Since editor is in play mode, simply send command for test launch.
                FileBroker.SaveNonUnityTextResource(FileResource.LaunchInstructions, string.Empty);
                AssetDatabase.Refresh();
                Arbiter.LocalRunLaunch = true;
                ConnectionStrategy.ReceiveMessage(string.Format("[{{\"automation_command\": \"rt {0}\"}}{1}]", launchLine, loopModeActive ? string.Format(",{{\"loop_tests\": \"{0}@{1}\"}}", name, loopCounter) : string.Empty));

            }

        }

        string GetLaunchCommand(string name, string type) {

            string listOfTestNames = string.Empty;
            if(type == "status") {

                List<KeyValuePair<string, string[]>> tests = new List<KeyValuePair<string, string[]>>();
                switch(name) {
                    case "Ignored":
                        tests = AutomationReport.testsMeta.FindAll(x => {
                            return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "ignored").Any();
                        });
                        List<string> ignored = new List<string>();
                        for(int t = 0; t < tests.Count; t++) {

                            ignored.Add(tests[t].Key);

                        }
                        listOfTestNames = string.Join(",", ignored.ToArray());
                        break;
                    case "Passed":
                        tests = AutomationReport.testsMeta.FindAll(x => {
                            return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "passed").Any();
                        });
                        List<string> passed = new List<string>();
                        for(int t = 0; t < tests.Count; t++) {

                            passed.Add(tests[t].Key);

                        }
                        listOfTestNames = string.Join(",", passed.ToArray());
                        break;
                    case "Failed":
                        tests = AutomationReport.testsMeta.FindAll(x => {
                            return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "failed").Any();
                        });
                        List<string> failed = new List<string>();
                        for(int t = 0; t < tests.Count; t++) {

                            failed.Add(tests[t].Key);

                        }
                        listOfTestNames = string.Join(",", failed.ToArray());
                        break;
                    case "Skipped":
                        tests = AutomationReport.testsMeta.FindAll(x => {
                            return x.Value.ToList().FindAll(y => y.Split(':')[0].ToLower() == "status" && y.Split(':')[1].ToLower() == "skipped").Any();
                        });
                        List<string> skipped = new List<string>();
                        for(int t = 0; t < tests.Count; t++) {

                            skipped.Add(tests[t].Key);

                        }
                        listOfTestNames = string.Join(",", skipped.ToArray());
                        break;
                    case "NoData":
                        tests = AutomationReport.testsMeta.FindAll(x => {
                            return x.Value.ToList().FindAll(y => {
                                bool match = false;
                                if(y.Split(':')[0].ToLower() == "status") {

                                    string status = y.Split(':')[1].ToLower();
                                    match = status != "skipped" && status != "failed" && status != "passed" && status != "ignored";

                                }
                                return match;
                            }).Any();
                        });
                        List<string> noData = new List<string>();
                        for(int t = 0; t < tests.Count; t++) {

                            noData.Add(tests[t].Key);

                        }
                        listOfTestNames = string.Join(",", noData.ToArray());
                        break;
                }

                //No tests with the selected status.
                if(string.IsNullOrEmpty(listOfTestNames)) {

                    return string.Empty;

                } else {

                    //If single test, run as a single test.
                    if(!listOfTestNames.Contains(",")) {

                        listOfTestNames = string.Format("*{0}", listOfTestNames);

                    }

                }

            }

            KeyValuePair<string, string> launchInstructions = new KeyValuePair<string, string>(name, type);
            Nexus.Overseer.Master_Editor_Override = launchInstructions;

            return string.Format("{0}{1}", launchInstructions.Value == "test" ? "*" : string.Empty, string.IsNullOrEmpty(listOfTestNames) ? launchInstructions.Key : listOfTestNames);

        }

        void SaveEditorSettings() {

            List<string> bools = new List<string>();
            for(int b = 0; b < Categories.Count; b++) {

                bools.Add(Categories[b].ToString());

            }
            FileBroker.SaveNonUnityTextResource(FileResource.ManifestGUISettings, string.Join(AutomationMaster.DELIMITER.ToString(), bools.ToArray()));
            AssetDatabase.Refresh();

        }

        public void OnDestroy() {

            FileBroker.SaveNonUnityTextResource(FileResource.ManifestGUISettings, string.Empty);
            AssetDatabase.Refresh();
            if(TestMonitorHelpers.Helper != null) {

                Nexus.DestroyImmediate(TestMonitorHelpers.Helper);

            }

        }

        private List<KeyValuePair<string, string>> GetMethodStatuses(List<KeyValuePair<string, string>> methods) {

            List<KeyValuePair<string, string>> methodsAndStatuses = new List<KeyValuePair<string, string>>();
            for(int m = 0; m < methods.Count; m++) {

                List<KeyValuePair<string, string[]>> match = AutomationReport.testsMeta.FindAll(g => g.Value.ToList().FindAll(p => p == string.Format("name:{0}", methods[m])).Count > 0);
                if(match.Count == 1) {

                    List<string> attributes = match.First().Value.ToList();
                    if(attributes.Any()) {

                        string status = attributes.Find(t => t.Contains("status:"));
                        if(!string.IsNullOrEmpty(status)) {

                            methodsAndStatuses.Add(new KeyValuePair<string, string>(methods[m].Key, status));

                        }

                    }

                } else {

                    //If this is a brand new test, then go ahead and render it.
                    methodsAndStatuses.Add(new KeyValuePair<string, string>(methods[m].Key, "NoData"));

                }

            }

            return methodsAndStatuses;

        }


        void RenderTest(string testName, GUIStyle fo, Color colorDefault, bool isSubCat, bool isTestView = false) {

            bool isDebug = _methods.Any() && _methods.Find(x => x.Name == testName).DeclaringType.GetCustomAttributes(typeof(DebugClass), false).Length > 0;

            //Do not render this test if a filter string is entered and the test name does not contain the filter value.
            if(!string.IsNullOrEmpty(_filterField)) {

                if(!testName.ToLower().Contains(_filterField.ToLower())) {

                    return;

                }

            }

            List<KeyValuePair<string, string[]>> match = AutomationReport.testsMeta.FindAll(x => x.Key == testName);
            string existingTestStatus = string.Empty;
            if(match.Any()) {

                //TODO: Use Category data.
                existingTestStatus = match.First().Value.ToList().Find(x => x.Split(':')[0] == "status").Split(':')[1];

            }

            bool isRunningTest = false;
            if(AutomationMaster.CurrentTestContext != null) {

                isRunningTest = AutomationMaster.CurrentTestContext.TestName == testName;

            }

            fo = new GUIStyle(GUI.skin.label);
            fo.margin = new RectOffset(isTestView ? 15 : isSubCat ? 40 : 25, 0, 0, 0);
            fo.fontSize = 14;
            fo.fixedHeight = 18;

            string buttonSymbol = string.Empty;
            if(EditorApplication.isPlaying && Nexus.Overseer.Master_Editor_Override.Key != null && !string.IsNullOrEmpty(Nexus.Overseer.Master_Editor_Override.Key) && AutomationMaster.Methods.FindAll(x => x.Value.Name == testName).Any() && !AutomationMaster.TestRunContext.CompletedTests.Contains(testName)) {

                //This test is currently being run. Add label to signify this.
                if(isRunningTest) {

                    fo.normal.textColor = colorPassed;

                } else {

                    fo.normal.textColor = Color.magenta;

                }
                if(AutomationMaster.CurrentTestContext != null && AutomationMaster.CurrentTestContext.TestName != testName) {

                    //Show the order of tests to be executed in the active test run. Grab this unhandled test from the master list, and take its index as the current order in the queue.
                    buttonSymbol = AutomationMaster.Methods.FindAll(x => !AutomationMaster.TestRunContext.CompletedTests.Contains(x.Value.Name)).IndexOf(AutomationMaster.Methods.Find(x => x.Value.Name == testName)).ToString();
                    if(buttonSymbol == "0") {

                        buttonSymbol = RUNNING;

                    }

                } else {

                    buttonSymbol = RUNNING;

                }

            } else {
                if(existingTestStatus == "Failed") {

                    if(!showFailed) {

                        return;

                    }
                    fo.normal.textColor = colorFailed;
                    buttonSymbol = FAILED;

                } else if(existingTestStatus == "Skipped") {

                    if(!showSkipped) {

                        return;

                    }
                    fo.padding = new RectOffset(-1, 0, 0, 0);
                    fo.normal.textColor = colorSkipped;
                    buttonSymbol = SKIPPED;

                } else if(existingTestStatus == "Passed") {

                    if(!showPassed) {

                        return;

                    }
                    fo.normal.textColor = colorPassed;
                    buttonSymbol = PASSED;

                } else if(existingTestStatus == "Ignored") {

                    if(!showIgnored) {

                        return;

                    }
                    fo.normal.textColor = colorIgnore;
                    fo.fontSize = 10;
                    fo.fontStyle = FontStyle.Bold;
                    fo.padding = new RectOffset(2, 0, 4, 4);
                    buttonSymbol = IGNORED;

                } else {
                    if(!showNoData) {

                        return;

                    }
                    fo.normal.textColor = Color.grey;
                    buttonSymbol = NODATA;
                    int offsetNoData = isSubCat ? 40 : _viewModeCategory ? 25 : 15;
                    fo.margin = new RectOffset(offsetNoData, 0, 0, 10);
                    fo.padding = new RectOffset(2, 0, -2, 0);
                    fo.fontSize = 18;
                }

            }

            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button(buttonSymbol, fo, GUILayout.Width(25))) {

                if(!AutomationMaster.Busy) {

                    LaunchTestDependenciesCheck(testName, "test");

                } else {

                    SimpleAlert.Pop("The test runner is currently active. Stop test execution, or wait for it to complete, before launching new test runs.", null);

                }

            }

            GUIStyle testNameStyle = new GUIStyle(GUI.skin.label);
            testNameStyle.margin = new RectOffset(5, 0, 0, 0);
            testNameStyle.padding = new RectOffset(0, 0, 2, 0);
            testNameStyle.fontSize = 12;
            testNameStyle.fontStyle = FontStyle.Normal;
            if(selectedTest == testName) {

                testNameStyle.fontStyle = FontStyle.Bold;
                if(isRunningTest) {

                    testNameStyle.normal.textColor = colorPassed;

                } else {

                    testNameStyle.normal.textColor = colorButtonTextDefault;

                }

            } else {

                if(isRunningTest) {

                    testNameStyle.fontStyle = FontStyle.Bold;
                    testNameStyle.normal.textColor = colorPassed;

                } else {

                    testNameStyle.fontStyle = FontStyle.Normal;
                    testNameStyle.normal.textColor = colorDefault;

                }

            }

            if(isDebug) {

                testNameStyle.normal.textColor = new Color32(100, 175, 255, 255);
                testName += " (Debug)";

            }
            if(GUILayout.Button(testName, testNameStyle)) {

                selectedTest = testName.Replace("(Debug)", string.Empty).Trim();

            }

            EditorGUILayout.EndHorizontal();

            //Remove loading if compilation error occurs on test launch.
            if(AutoConsole.Logs.FindAll(x => x.message.ToLower().ContainsOrEquals("all compiler errors have to be fixed")).Any()) {

                Nexus.Self.HideLoading();

            }

        }

    }

}
