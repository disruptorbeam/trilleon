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
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using System.Reflection;

//TODO: Pimp out the logic that places nodes in a more friendly/appealing way so that minimal dragging is necessary.
//TODO: Detect circular dependencies among nodes, and color the breziers for these connections red.
//TODO: Check for DependencyWeb tags on any methods that are NOT tagged as automation too and warn user.

namespace TrilleonAutomation {

	public class DependencyVisualizer : Swat {

		Vector2 scroll = new Vector2();
		Vector2 infoBoxScroll = new Vector2();

		bool lastPassDetectedNoWebTestsSoDisplayDebugAsExample = false;
		int inspectedNodeWindow = -1;
		List<DependencyNode> RenderedNodes = new List<DependencyNode>();
		List<KeyValuePair<string, string[]>> testsAndTheirDependenciesList = new List<KeyValuePair<string, string[]>>();
		List<DependencyNode> DependencyWebs = new List<DependencyNode>();

		const int DEFAULT_HEIGHT = 40;
		float posX = 0;
		float posY = 0;
		float maxWindowX = 500;
		float maxWindowY = 500;
		float lastRenderX = 0;
		float lastRenderY = 0;
		float longestTestNameInInfoBox = 0;
		float infoBoxMinWidth = 250; //Also serves as size of viewable scroll area.
		Rect originRect;

		[MenuItem ("Trilleon/Dependencies/Web")]
		static void Init () {

            ShowWindow<DependencyVisualizer>(typeof(DependencyVisualizer), "Web", DockNextTo.SceneView);

		}

		void OnEnable() {

			MapDependencies();
			originRect = new Rect(100, 100, 0, DEFAULT_HEIGHT);

		}

		private void OnGUI() {

			if(lastPassDetectedNoWebTestsSoDisplayDebugAsExample && testsAndTheirDependenciesList.Count == 0) {

				MapDependencies();

			}

			GUIStyle refButton = new GUIStyle(GUI.skin.button);
			refButton.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			Color defaultBgColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.gray;
			if(GUI.Button(new Rect(0,0,75, 25), "Refresh", refButton)) {
				
				inspectedNodeWindow = -1;
				RenderedNodes = new List<DependencyNode>();
				testsAndTheirDependenciesList = new List<KeyValuePair<string, string[]>>();
				DependencyWebs = new List<DependencyNode>();
				MapDependencies();

			}
			GUI.backgroundColor = defaultBgColor;

			Rect allSize = new Rect(0, 0, lastRenderX + infoBoxMinWidth, maxWindowY + infoBoxMinWidth);
			scroll = GUI.BeginScrollView(new Rect(0, 0, lastRenderX, lastRenderY), scroll, new Rect(0, 0, lastRenderX + infoBoxMinWidth, lastRenderY + infoBoxMinWidth), GUIStyle.none, GUIStyle.none);
			GUI.Box(allSize, string.Empty);

			for(int all = 0; all < DependencyWebs.Count; all++) {

				int index = 0;
				DependencyNode parentNode = DependencyWebs[all];
				if(!RenderedNodes.Where(x => x.TestName == parentNode.TestName).Any()) {
					RenderedNodes.Add(parentNode);
					index = RenderedNodes.Count - 1;
					float width = DetermineRectWidthBasedOnLengthOfString(RenderedNodes[index].TestName);
					Rect newRect = originRect;
					newRect.width = width; 
					RenderedNodes[index].rect = GenerateNewNonOverlappingRectPositionForNewNode(newRect);
				} else { 
					index = RenderedNodes.FindIndex(a => a.TestName == parentNode.TestName);
				}

				//Create new node, or find existing one, and draw lines between this node and the dependency.
				List<KeyValuePair<DependencyNodeConnectionType,string>> LinkedNodeTestNames = RenderedNodes[index].Dependencies.ToList();
				for(int s = 0; s < LinkedNodeTestNames.Count; s++) {

					if(LinkedNodeTestNames[s].Key == DependencyNodeConnectionType.Incoming) {

						string testName = LinkedNodeTestNames[s].Value;
						DependencyNode thisNode = new DependencyNode();

						int indexChild = 0;

						List<DependencyNode> match = RenderedNodes.Where(x => x.TestName == testName).ToList();
						if(!match.Any()) {
							thisNode.TestName = testName;
							thisNode.Dependencies = DependencyWebs.Where(x => x.TestName == thisNode.TestName).Select(x => x.Dependencies).Single();
							RenderedNodes.Add(thisNode);
							indexChild = RenderedNodes.Count - 1;
							float width = DetermineRectWidthBasedOnLengthOfString(RenderedNodes[indexChild].TestName);
							Rect newRect = RenderedNodes[all].rect;
							newRect.width = width; 
							RenderedNodes[indexChild].rect = GenerateNewNonOverlappingRectPositionForNewNode(newRect);
						} else {
							indexChild = RenderedNodes.FindIndex(a => a.TestName == testName);
						}

						Handles.BeginGUI();
						Handles.DrawBezier(RenderedNodes[index].rect.center, RenderedNodes[indexChild].rect.center, new Vector2(RenderedNodes[index].rect.xMax + 50f, RenderedNodes[index].rect.center.y), new Vector2(RenderedNodes[indexChild].rect.xMin - 50f, RenderedNodes[indexChild].rect.center.y), Color.cyan, null, 5f);
						Handles.EndGUI();

					}

				}

			}

			GUIStyle f = new GUIStyle(EditorStyles.foldout);
			f.richText = true;

			//Render each node window object.
			BeginWindows();
			for(int i = 0; i < RenderedNodes.Count; i++) {

				RenderedNodes[i].rect = GUI.Window(i, RenderedNodes[i].rect, WindowEvents, RenderedNodes[i].TestName);

			}
			EndWindows();

			string nodeDetails = inspectedNodeWindow >= 0 ? GetNodeDetails(RenderedNodes[inspectedNodeWindow]) : GetNodeDetails(new DependencyNode());
			float boxHeight = TestMonitorHelpers.DetermineRectHeightBasedOnLinesInNodeDetails(nodeDetails) + 50;
			float boxWidth = longestTestNameInInfoBox > infoBoxMinWidth ? longestTestNameInInfoBox : infoBoxMinWidth;

			bool overflowX = boxWidth > infoBoxMinWidth;
			//Account for size of scroll bar in scrollable space
			if(overflowX) {

				boxHeight += 40;

			}
			float scrollViewHeight = boxHeight < position.height ? boxHeight : position.height;
			bool overflowY = scrollViewHeight == position.height;
			if(overflowY) {

				boxWidth += 40;

			}

			GUI.EndScrollView();

			GUIStyle verticalScrollBar = new GUIStyle(GUI.skin.verticalScrollbar);
			GUIStyle horizontalScrollBar = overflowX ? new GUIStyle(GUI.skin.horizontalScrollbar) : GUIStyle.none;

			infoBoxScroll = GUI.BeginScrollView(new Rect(position.width - infoBoxMinWidth, 0, infoBoxMinWidth, scrollViewHeight), infoBoxScroll, new Rect(new Vector2(position.width - (infoBoxMinWidth - 5), 4), new Vector2(boxWidth, boxHeight)), horizontalScrollBar, verticalScrollBar);

			//Display selected node's details in details panel.
			GUIStyle infoBox = new GUIStyle(GUI.skin.box);
			infoBox.richText = true;
			infoBox.normal.background = MakeTextureFromColor(new Color(0.175f, 0.175f, 0.175f, 1f));
			infoBox.alignment = TextAnchor.UpperLeft;
			GUI.Box(new Rect(new Vector2(position.width - (infoBoxMinWidth - 5), 4), new Vector2(boxWidth, boxHeight)), nodeDetails, infoBox);

			lastRenderX = position.width + infoBoxMinWidth;
			lastRenderY = position.height;

			GUI.EndScrollView();

		}

		string GetNodeDetails(DependencyNode node) {

			StringBuilder nodeInfo = new StringBuilder();
			nodeInfo.AppendLine("   <color=#ffffffff><b><size=14>NODE INFO</size></b>");
			nodeInfo.AppendLine("  _______________");
			nodeInfo.AppendLine(string.Empty);

			string[] dependentTests = node.Dependencies.Where(x => x.Key == DependencyNodeConnectionType.Outgoing).Select(x => x.Value).ToArray();
			string[] requiredByTests = node.Dependencies.Where(x => x.Key == DependencyNodeConnectionType.Incoming).Select(x => x.Value).ToArray();

			//Determine minimum Node Info box width needed to allow scrolling to view entire window.
			longestTestNameInInfoBox = DetermineRectWidthBasedOnLengthOfString(node.TestName) + 25; //Account for indent of title test in Node Info window.
			for(int d = 0; d < dependentTests.Length; d++){
				float neededWidth = DetermineRectWidthBasedOnLengthOfString(dependentTests[d]);
				if(neededWidth > longestTestNameInInfoBox) {
					longestTestNameInInfoBox = neededWidth;
				}
			}
			for(int r = 0; r < requiredByTests.Length; r++){
				float neededWidth = DetermineRectWidthBasedOnLengthOfString(requiredByTests[r]);
				if(neededWidth > longestTestNameInInfoBox) {
					longestTestNameInInfoBox = neededWidth;
				}
			}

			if(!string.IsNullOrEmpty(node.TestName)) {

				string testName = string.Format("    <color=#adadad><b><size=13>{0}</size></b></color>", node.TestName);
				string dependents = string.Join("\n\t", dependentTests);
				string dependentOn = string.Format("    <b>Dependent On:</b>\n\t{0}", string.IsNullOrEmpty(dependents) ? "none" : dependents);
				string requires = string.Join("\n\t", requiredByTests);
				string requiredBy = string.Format("    <b>Required By:</b>\n\t{0}", string.IsNullOrEmpty(requires) ? "none" : requires);

				nodeInfo.AppendLine(string.Empty);
				nodeInfo.AppendLine(testName);
				nodeInfo.AppendLine(string.Empty);
				nodeInfo.AppendLine(string.Empty);
				nodeInfo.AppendLine(dependentOn);
				nodeInfo.AppendLine(string.Empty);
				nodeInfo.AppendLine(requiredBy);

			} else {
				nodeInfo.AppendLine(string.Empty);
				nodeInfo.AppendLine("  <i>Select a node to display details.</i>");
			}

			nodeInfo.AppendLine("</color>");

			return nodeInfo.ToString();

		}

		void WindowEvents (int windowID) {

			if(inspectedNodeWindow == windowID) {
				GUI.backgroundColor = Color.magenta;
			}
			if(GUILayout.Button("Details")) {
				inspectedNodeWindow = windowID;
			}

			GUI.DragWindow();

		}

		Rect GenerateNewNonOverlappingRectPositionForNewNode(Rect parentNodeRect) {

			System.Random random = new System.Random();
			int numPos = 75;
			int numNeg = -75;
			Rect newRect = parentNodeRect;

			//Keep repositioning the new node nearby until it does not overlap with any other nodes.
			while(RenderedNodes.Where(x => x.rect.Overlaps(newRect)).Any() && newRect.x > 50 && newRect.y > 50) {

				if(random.Next(0, 2) == 1) {
					posX = numPos;
				} else {
					posX = numNeg;
				}
				if(random.Next(0, 2) == 1) {
					posY = numPos;
				} else {
					posY = numNeg;
				}
				if(posX + newRect.x > 50) {
					newRect.x += posX;
				}
				if(posY + newRect.y > 50) {
					newRect.y += posY;
				}

			}

			if(maxWindowX < newRect.x) {
				maxWindowX = newRect.x;
			}
			if(maxWindowY < newRect.y) {
				maxWindowY = newRect.y;
			}

			return newRect;

		}

		//DependencyWebs consists of a Key (the id of the web list) and a Value (a list of KeyValuePairs which consist of a DependencyWeb method and the method that this declares as a dependency.
		void MapDependencies() {

			//Get all automation test methods.
			List<KeyValuePair<string, MethodInfo>> allTestMethods = new List<KeyValuePair<string, MethodInfo>>();
			List<Type> AutomationClasses = AutomationMaster.GetAutomationClasses().FindAll(x => lastPassDetectedNoWebTestsSoDisplayDebugAsExample || !x.GetCustomAttributes(false).OfType<DebugClass>().Any()); //Ignore debug classes.
			for(int i = 0; i < AutomationClasses.Count(); i++) {

				List<MethodInfo> methods = AutomationClasses[i].GetMethods().Where(y => y.GetCustomAttributes(false).OfType<Automation>().Any()).ToList();
				for(int x = 0; x < methods.Count(); x++) {

					allTestMethods.Add(new KeyValuePair<string, MethodInfo>(methods[x].Name, methods[x]));

				}   

			}

			//From all methods, get methods that declare dependencies.
			List<KeyValuePair<string, MethodInfo>> allDependencyTests = allTestMethods.Where(x => {
				return x.Value.GetCustomAttributes(typeof(DependencyWeb), false).Any();
			}).ToList();
			testsAndTheirDependenciesList = new List<KeyValuePair<string, string[]>>();

			if(allDependencyTests.Count == 0) {

				lastPassDetectedNoWebTestsSoDisplayDebugAsExample = true; //Show DependencyTests marked as Debug, as a demo for what this window shows when a user has their own DependencyTests.
				return;

			}

			//Get list of every test name associated with its declared dependencies.
			for(int i = 0; i < allDependencyTests.Count; i++) {

				DependencyWeb dw = (DependencyWeb)Attribute.GetCustomAttribute(allDependencyTests[i].Value, typeof(DependencyWeb));
				List<string> dtNames = dw.Dependencies;
				dtNames.AddRange(dw.OneOfDependencies);
				testsAndTheirDependenciesList.Add(new KeyValuePair<string, string[]>(allDependencyTests[i].Key, dtNames.ToArray()));

			}

			//List of all methods.
			List<string> allInvolvedTestMethods = new List<string>();
			//Build dependency web connections.
			for(int t = 0; t < testsAndTheirDependenciesList.Count; t++) {
				allInvolvedTestMethods.Add(testsAndTheirDependenciesList[t].Key);
				allInvolvedTestMethods.AddRange(testsAndTheirDependenciesList[t].Value);
			}
			allInvolvedTestMethods = allInvolvedTestMethods.Distinct().ToList();

			for(int all = 0; all < allInvolvedTestMethods.Count; all++) {

				string method = allInvolvedTestMethods[all];
				DependencyNode node = new DependencyNode();
				node.TestName = method;

				List<KeyValuePair<string, string[]>> matchChild = testsAndTheirDependenciesList.Where(x => x.Key == method).ToList();
				if(matchChild.Any()) {
					List<string> children = matchChild.Single().Value.ToList();
					for(int c = 0; c < children.Count; c++) {
						node.AddDependency(DependencyNodeConnectionType.Outgoing, children[c]);
					}
				}

				List<string> parents = testsAndTheirDependenciesList.Where(x => x.Value.Contains(method)).Select(x => x.Key).ToList();
				if(parents.Any()) {
					for(int p = 0; p < parents.Count; p++) {
						node.AddDependency(DependencyNodeConnectionType.Incoming, parents[p]);
					}
				}

				DependencyWebs.Add(node);

			}

		}

	}

}
