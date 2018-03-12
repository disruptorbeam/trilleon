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
+*/

using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {

	/// <summary>
	/// Simple wrapper class that allows you to seemlessly switch between using your own command/cheat console to invoke cheats, or you can use the native Trilleon CommandConsole (default).
	/// </summary>
	public static class CommandConsoleBroker  {

		public static ConsoleCommands ConsoleInstance {
			get {
				return AutomationMaster.StaticSelf.GetComponent<ConsoleCommands>(); //Get most derived console class (located in Game > Utilities folder) so that all IEnumerators in the base classes can also be accessed.
			}
		}

		//Use custom extended command console. Replace `ConsoleCommands` type with yours.
		public static ConsoleCommands Extended { 
			get {
				//TODO: Your code here! Remove all but boolean if invoked by static method. Else, use this for instance.
				if(_extended == null) {
					_extended = null;
				}
				extend = true;
				return _extended;
			}
		}
		private static ConsoleCommands _extended;

		//All commands that read CommandConsoleBroker.Extend.SendCommand will use extended logic, while invoking by CommandConsoleBroker.SendCommand will use the native Trilleon logic.
		private static bool extend = false; 

		/// <summary>
		/// Take provided command string and delegate it to the requested command console logic.
		/// </summary>
		public static void SendCommand(string fullRawCommand) {

			if (extend) {

				//TODO: Add your custome code to send this command directly/programmatically to your command console's processor logic.

			} else {

				ConsoleCommands.SendCommand(fullRawCommand);

			}

			extend = false; //Return to false. 

		}

	}

}