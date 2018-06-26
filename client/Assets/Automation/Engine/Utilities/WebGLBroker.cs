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

﻿#if UNITY_WEBGL || UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

namespace TrilleonAutomation {

	/// <summary>
	/// Brokers communication between Trilleon logic and the browser that contains a WebGL canvas game. 
	/// This is an alternative way to communicate with a server running a WebGL test run, as opposed to using Pubsub services.
	/// </summary>
	public class WebGLBroker : MonoBehaviour {

		bool AwaitingScreenshot { get; set; }

		//Outgoing call to browser (Points to Engine > Xtra > Plugin javascript file and functions).
		[DllImport("__Internal")]
		public static extern void ReportXmlResults(string xml);
		[DllImport("__Internal")]
		public static extern void ReportJsonResults(string json);
		[DllImport("__Internal")]
		public static extern void AutomationReady();

		//Send a screenshot request to anything listening on the web page.
		[DllImport("__Internal")]
		private static extern void ScreenshotRequest();
		public void RequestScreenshot(string screenshotName) {

			AwaitingScreenshot = true;
			ScreenshotRequest();

		}

		//Incoming call from browser (Sent by "SendMessage()" Unity javascript method that points to this class and method).
		public void LaunchTest(string test) {

			ConnectionStrategy.ReceiveMessage(string.Format("{{ \"automation_command\": \"rt *{0}\" }}", test));

		}
		public void ScreenshotResponse() {

			AwaitingScreenshot = false;

		}

	}

}
#endif