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

﻿using System;
using System.Collections;
using UnityEngine;

namespace TrilleonAutomation {
	
	public class BuildServerBroker : MonoBehaviour {

		const float _responseTimeout = 20f;
		bool _awaitingResponse { get; set; }
		string _awaitingCommand { get; set; }

		public void CommandResponseReceived(string commandCompleted) {

			if(commandCompleted == _awaitingCommand) {

				_awaitingResponse = false;

			}

		}

		public IEnumerator AcceptDeviceAlert() {
			
			yield return StartCoroutine(SendCommand(ServerCommand.AcceptDeviceAlert));

		}

		public IEnumerator DeclineDeviceAlert() {
			
			yield return StartCoroutine(SendCommand(ServerCommand.DeclineDeviceAlert));

		}	

		IEnumerator SendCommand(ServerCommand command) {

			if(AutomationMaster.Arbiter.TestRunId.Length == 0 || Application.isEditor) {

				AutoConsole.PostMessage(string.Format("Ignoring command \"{0}\" as no server is involved in this test run.", Enum.GetName(typeof(ServerCommand), command)), MessageLevel.Verbose);
				yield break;

			}
				
			switch(command) {
				case ServerCommand.AcceptDeviceAlert:
					//AutomationMaster.Arbiter.SendCommunication("SERVER_BROKER_COMMAND|HANDLE_DEVICE_ALERT|", "SERVER_BROKER_VALUE|1|"); // 1 or true, accept alert.
					yield break;
				case ServerCommand.DeclineDeviceAlert:
					//AutomationMaster.Arbiter.SendCommunication("SERVER_BROKER_COMMAND|HANDLE_DEVICE_ALERT|", "SERVER_BROKER_VALUE|0|"); // 0 or false, decline alert.
					yield break;
			}

			/*
			float timer = 0f;
			_awaitingResponse = true;
			while(_awaitingResponse && timer <= _responseTimeout) {

				timer += 1;
				yield return StartCoroutine(Q.driver.WaitRealTime(1f));

			}
			if(timer > _responseTimeout) {
				
				yield return StartCoroutine(Q.assert.Fail("Response timeout occurred waiting for server to complete and respond to a ServerBroker command."));

			}
			*/
			yield return null;

		}

	}

	public enum ServerCommand { AcceptDeviceAlert, DeclineDeviceAlert }

}
