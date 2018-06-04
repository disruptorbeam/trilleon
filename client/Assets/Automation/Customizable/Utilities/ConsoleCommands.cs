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

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {

	/// <summary>
	/// All game-specific cheat/commands go here. ConsoleCommandsCore and ConsoleCommandsBase cannot be implemented. This class implements the full suite of Cheat Console logic, even if no commands currently exist in this class.
	/// </summary>
	public class ConsoleCommands : ConsoleCommandsCore {

		/// <summary>
		/// Add all valid commands with accompanying logic to launch in region below. Command aliases cannot contains spaces.
		/// </summary>
		void Start() {

			ImplementCore(); //Required to add Core framework commands.

		}

		//Your Commands Here!
		#region Console Command delegates

		#endregion

		#region Console Command support methods

		#endregion

	}

}