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

namespace TrilleonAutomation {

	/// <summary>
	/// Q: The custodian wrapper object to centralize all test functionality.
	/// This is the master abstraction for all functionality that a tester writing an automation script will need.
	/// If a function is not available through Q, then a tester should not be using it in their automation script.
	/// </summary>
	public static class Q {

		#region Driver

		public static GameDriver driver {
			get { 
				if(_driver == null) {
					_driver = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME).GetComponent<GameDriver>();
				}
				return _driver;
			}
		}
		private static GameDriver _driver;

		#endregion

		#region Helper

		public static GameHelperFunctions help {
			get { 
				if(_help == null) {
					_help = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME).GetComponent<GameHelperFunctions>();
				}
				return _help;
			}
		}
		private static GameHelperFunctions _help;

		#endregion

		#region Assert

		public static GameAssert assert {
			get { 
				if(_assert == null) {
					_assert = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME).GetComponent<GameAssert>();
				}
				return _assert;
			}
		}
		private static GameAssert _assert;

		#endregion

		#region Global Storage

		public static RunHash storage {
			get { 
				if(_storage == null) {
					_storage = new RunHash();
				}
				return _storage;
			}
		}
		private static RunHash _storage;

		#endregion

		public static void SendConsoleCommand(string command) {
			
			CommandConsoleBroker.SendCommand(command);

		}

		#region Game Instance Library

		/// <summary>
		/// The Game object allowing interaction with GameMaster, MonoBehaviour, game-specific methods and variables.
		/// </summary>
		/// <value>The game.</value>
		public static GameMaster game {
			get { 
				if(_game == null) {
					_game = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME).GetComponent<GameMaster>();
				}
				return _game;
			}
		}
		private static GameMaster _game;

		#endregion

		#region Server Command Broker

		/// <summary>
		/// Broker that sends commands to the server (if any) that launched the current test run.
		/// </summary>
		/// <value>The game.</value>
		public static BuildServerBroker request {
			get { 
				if(_request == null) {
					_request = GameObject.Find(AutomationMaster.AUTOMATION_CUSTODIAN_NAME).GetComponent<BuildServerBroker>();
				}
				return _request;
			}
		}
		private static BuildServerBroker _request;

		#endregion

		/// <summary>
		/// Queues a deferrment scenario. Based on the predicate supplied, the supplied test will be deferred (BASED ON SET NUMBER OF TIMES) if the condition returns false.
		/// </summary>
		public static void AddConditionalDeferrment(ConditionalDeferrment scenario) {

			if(!AutomationMaster.ConditionalDeferrments.FindAll(x => x.TestMethod == scenario.TestMethod).Any()) {
				
				AutomationMaster.ConditionalDeferrments.Add(scenario);

			}

		}

	}

}
