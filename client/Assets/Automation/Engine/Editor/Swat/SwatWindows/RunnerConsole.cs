/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Reflection;

namespace TrilleonAutomation {

	public class RunnerConsole : SwatWindow {

		public DateTime OpenedTestReport { get; private set; }
		AutoConsoleMessage _selectedConsoleMessage = null;
		List<ConsoleMessageType> _consoleAbridgedReportTypes = new List<ConsoleMessageType>() { ConsoleMessageType.AssertionFail, ConsoleMessageType.AssertionPass };
		int _lastPassConsoleMessageCount = 0;
		DateTime _lastConsoleMessageButtonClick = new DateTime();
		bool _autoScroll, _runningTestListFoldout, _pubsubMode;
		Vector2 _scroll = new Vector2();

		public override void Set() {

			_autoScroll = true;

		}

		public override bool UpdateWhenNotInFocus() {

			return true;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle runner, sl;

			sl = new GUIStyle(GUI.skin.label);
			sl.normal.textColor = Nexus.TextGreen;
			sl.padding = Nexus.BaseRectOffset;

			GUILayout.Space(20);

			//If an editor-launched test or category of tests is running, display a notification here.
			if(!string.IsNullOrEmpty(Nexus.Overseer.Master_Editor_Override.Key)) {

				if(!AutomationMaster.Busy && Application.isPlaying) {
					AutomationMaster.Busy = true;
				} else if(AutomationMaster.Busy && Application.isPlaying) {

					EditorGUILayout.Space();

					List<string> RunningTests = Nexus.Overseer.Master_Editor_Override.Key.Replace("*", string.Empty).Split(',').ToList();
					if(RunningTests.Count > 1) {

						runner = new GUIStyle(EditorStyles.foldout);
						runner.normal.textColor = Nexus.TextGreen;
						runner.onNormal.textColor = Nexus.TextGreen;
						runner.margin = sl.margin = Nexus.BaseRectOffset;
						_runningTestListFoldout = Nexus.Self.Foldout(_runningTestListFoldout, "Running Tests", true, runner);
						EditorGUI.indentLevel++;
						if(_runningTestListFoldout) {
							for(int rt = 0; rt < RunningTests.Count; rt++) {
								EditorGUILayout.LabelField(RunningTests[rt], sl);
							}
						}
						EditorGUI.indentLevel--;

					} else {

						string runName = Nexus.Overseer.Master_Editor_Override.Key;
						if(AutomationMaster.LaunchType == LaunchType.All) {

							runName = "all";

						} else if(!runName.Contains("*")) {

							//TODO: Get cateogry name: runName =  _categorySubcategoryTests[_selectedCategory].Key;

						} else {

							runName = runName.Replace("*", string.Empty);

						}

						runner = new GUIStyle(GUI.skin.label);
						runner.normal.textColor = Nexus.TextGreen;
						runner.onNormal.textColor = Nexus.TextGreen;
						runner.padding = Nexus.BaseRectOffset;
						EditorGUILayout.LabelField(string.Format("Running: {0}", runName, sl), runner);

					}

					EditorGUILayout.Space();

				} else if(AutomationMaster.Busy && !Application.isPlaying) {

					//If play mode was launched, but has been stopped, do not show this as running.
					AutomationMaster.Busy = false;
					Nexus.Overseer.Master_Editor_Override = new KeyValuePair<string,string>();

				} 

			} else {

				if(Application.isPlaying && AutomationMaster.Busy) {

					EditorGUILayout.LabelField(string.Format("Running: {0}", AutomationMaster.CurrentTestContext.TestName), sl);

				} else {

					AutomationMaster.Busy = false;

				}

			}

			GUIStyle horizontal = new GUIStyle();
			horizontal.margin = new RectOffset(10, 11, 0, 0);

			GUIStyle statusColor = new GUIStyle(GUI.skin.label);
			statusColor.margin = new RectOffset(0, 0, 3, 0);
			statusColor.normal.textColor = AutoConsole.Paused ? Color.red : Nexus.TextGreen;
			statusColor.fixedWidth = 125;
			statusColor.fontStyle = FontStyle.Bold;

			GUIStyle scrollStatus = new GUIStyle(GUI.skin.button);
			scrollStatus.margin = new RectOffset(0, 0, 0, 0);
			scrollStatus.fontStyle = _autoScroll ? FontStyle.Bold : FontStyle.Normal;
			scrollStatus.normal.background = Swat.ToggleButtonBackgroundSelectedTexture;
			scrollStatus.normal.textColor = _pubsubMode ? Nexus.TextGreen : Swat.WindowDefaultTextColor;

			GUILayout.BeginHorizontal(horizontal);
			EditorGUILayout.LabelField(string.Format("{0}  -  {1}", AutoConsole.Paused ? "Paused" : "Active", AutoConsole.FilterLevel == MessageLevel.Verbose ? "Verbose" : "Abridged") , statusColor);
			EditorGUILayout.Space();
			GUILayout.Space(-100);
			Nexus.Self.Button("Pubsub", "Show received Pubsub messages.", 
				new Nexus.SwatDelegate(delegate() {                
					_pubsubMode = !_pubsubMode;
				}), scrollStatus, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(22) });
			GUILayout.Space(0.65f);
			scrollStatus.normal.textColor = _autoScroll ? Nexus.TextGreen : Color.red;
			Nexus.Self.Button("Auto Scroll", "Remove all messages in the console.", 
				new Nexus.SwatDelegate(delegate() {                
					_autoScroll = !_autoScroll;
				}), scrollStatus, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(22) });
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal(horizontal);
			GUIStyle button = new GUIStyle(GUI.skin.button);  
			button.margin = new RectOffset(0, 0, 0, 0);
			button.normal.textColor = Swat.ToggleButtonTextColor;
			button.normal.background = Swat.ToggleButtonBackgroundTexture;

			Nexus.Self.Button("Clear", "Remove all messages in the console.", 
				new Nexus.SwatDelegate(delegate() {                
					FileBroker.SaveUnboundFile(string.Format("{0}{1}{2}/console_before_start.txt", FileBroker.BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_CONSOLE_LOG_DIRECTORY"), GameMaster.GAME_NAME), string.Empty);
					AutoConsole.messageListDisplayed = new List<AutoConsoleMessage>();
					AutoConsole.messageListQueued = new List<AutoConsoleMessage>();
					_selectedConsoleMessage = null;
				}), button, new GUILayoutOption[] { GUILayout.Width(60), GUILayout.Height(22) });

			GUILayout.Space(0.5f);

			Nexus.Self.Button(AutoConsole.Paused ? "Resume" : "Pause", "Pause/Continue rendering of new messages in the console. Messages are still received and stored when paused, and will appear immediately when resuming.", 
				new Nexus.SwatDelegate(delegate() {                
					AutoConsole.Paused = !AutoConsole.Paused;
				}), button, new GUILayoutOption[] { GUILayout.Width(60), GUILayout.Height(22) });

			EditorGUILayout.Space();

			Nexus.Self.Button("Verbose", "Post ALL messages from the automation framework in the console.", 
				new Nexus.SwatDelegate(delegate() {                
					AutoConsole.FilterLevel = MessageLevel.Verbose; 
					if(_autoScroll) {

						_scroll.y = 99999;
						_scroll.x = 0;

					}
				}), button, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(22) });

			GUILayout.Space(0.65f);

			GUIStyle abridged = new GUIStyle(GUI.skin.button);  
			abridged.margin = new RectOffset(0, 0, 0, 0);
			abridged.normal.textColor = Swat.ToggleButtonTextColor;
			abridged.normal.background = Swat.ToggleButtonBackgroundTexture;
			Nexus.Self.Button("Abridged", "Post only automation messages marked as high priority in the console.", 
				new Nexus.SwatDelegate(delegate() {                
					AutoConsole.FilterLevel = MessageLevel.Abridged; 
					if(_autoScroll) {

						_scroll.y = 99999;
						_scroll.x = 0;

					}
				}), abridged, new GUILayoutOption[] { GUILayout.Width(75), GUILayout.Height(22) });

			GUILayout.EndHorizontal();

			float messageHeight = 15;

			GUIStyle consoleBox = new GUIStyle(GUI.skin.box);
			consoleBox.fixedHeight = Nexus.Self.position.height - 115;
			consoleBox.fixedWidth = Nexus.Self.position.width - 20;
			consoleBox.margin = new RectOffset(10, 0, 0, 10);
			consoleBox.normal.background = Swat.TabButtonBackgroundTexture;

			GUILayout.BeginVertical(consoleBox); //Begin box border around console.
			_scroll = GUILayout.BeginScrollView(_scroll);

			GUIStyle messageBox = new GUIStyle(GUI.skin.box);
			messageBox.normal.background = Swat.TabButtonBackgroundSelectedTexture;
			messageBox.margin = new RectOffset(2, 2, 1, 1);
			GUIStyle messageBoxText = new GUIStyle(GUI.skin.label);
			messageBoxText.normal.textColor = Swat.WindowDefaultTextColor;
			messageBox.fixedWidth = consoleBox.fixedWidth - 10;
			messageBoxText.fixedWidth = messageBox.fixedWidth - 5;

			string lastConsoleMessage = string.Empty;
			int duplicateCount = 0;
			List<AutoConsoleMessage> consoleMessages = AutoConsole.messageListDisplayed;
			for(int m = 0; m < consoleMessages.Count; m++) {

				if((_pubsubMode && consoleMessages[m].messageType != ConsoleMessageType.ArbiterCommandReceived) || (!_pubsubMode && consoleMessages[m].messageType == ConsoleMessageType.ArbiterCommandReceived)) {

					continue;

				}

				//If this is a new console message, and auto scroll is on, scroll down automatically.
				if(_autoScroll && m == _lastPassConsoleMessageCount) {

					_scroll.y = 99999;
					_scroll.x = 0;

				}

				//Render only messages of the requested filter level.
				if(_pubsubMode || (AutoConsole.FilterLevel == MessageLevel.Abridged && consoleMessages[m].level == MessageLevel.Abridged) || AutoConsole.FilterLevel == MessageLevel.Verbose) {

					messageBoxText.normal.textColor = AutoConsole.MessageColor(consoleMessages[m].messageType);
					GUILayout.BeginHorizontal(messageBox, new GUILayoutOption[] {
						GUILayout.MinWidth(225),
						GUILayout.MaxHeight(messageHeight)
					});

					if(lastConsoleMessage == consoleMessages[m].message) {

						duplicateCount++;
						consoleMessages.RemoveAt(m - 1);

					} else {

						lastConsoleMessage = consoleMessages[m].message;
						duplicateCount = 0;

					}

					if(GUILayout.Button(string.Format("{0} {1}", duplicateCount > 0 ? (duplicateCount + 1).ToString() : string.Empty, consoleMessages[m].message), messageBoxText)) {

						if(_selectedConsoleMessage == consoleMessages[m]) {

							//If the message has been double-clicked, open this test failure as a single-test, temporary report.
							if(DateTime.Now.Subtract(_lastConsoleMessageButtonClick).TotalSeconds < 0.75d && !string.IsNullOrEmpty(consoleMessages[m].testMethod)) {

								if(_consoleAbridgedReportTypes.Contains(consoleMessages[m].messageType)) {

									_selectedConsoleMessage = null; //Prevents details panel from appearing. We only want it to appear for a single click, not a double click.
									AutomationReport.BuildTemporaryReportForSingleConsoleFailure(consoleMessages[m].testMethod);
									System.Diagnostics.Process.Start(string.Format("{0}{1}SingleTestAsReportTemp.html", FileBroker.BASE_NON_UNITY_PATH, ConfigReader.GetString("EDITOR_RESOURCE_FILES_DIRECTORY")));
									OpenedTestReport = DateTime.UtcNow;

								} 

							} else {

								ConsoleMessage.Pop(_selectedConsoleMessage.message, Enum.GetName(typeof(ConsoleMessageType), _selectedConsoleMessage.messageType), Enum.GetName(typeof(MessageLevel), _selectedConsoleMessage.level), _selectedConsoleMessage.timestamp.ToString());
								_selectedConsoleMessage = null;

							}

						}

						_selectedConsoleMessage = consoleMessages[m];
						_lastConsoleMessageButtonClick = DateTime.Now;

					}

					GUILayout.EndHorizontal();
				}

			}

			_lastPassConsoleMessageCount = consoleMessages.Count;

			GUILayout.EndScrollView();
			GUILayout.EndVertical();

		}

	}

}
