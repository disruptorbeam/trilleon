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
using System.Collections.Generic;

namespace TrilleonAutomation {
	
	public class Command {

		public Command(string description, ConsoleCommand command, List<KeyValuePair<string,string>> arguments, params string[] aliases) {

			Purpose = description;
			Run = command;
			Args = arguments;

			//Aliases and commands always compared in ToLower state.
			List<string> lowers = new List<string>();
			for(int a = 0; a < aliases.Length; a++) {

				lowers.Add(aliases[a].ToLower());
					
			}
			Aliases = lowers;

		}

		public List<string> Aliases { get; set; } //Names that trigger this command.
		public List<KeyValuePair<string,string>> Args { get; set; } //Expected variables for function (Key: Variable - Value: Description/Purpose). Used for consumers of commands, and not by code.
		public string Purpose { get; set; } //Short description of what this does, or the options available; retrievable from console at request.
		public ConsoleCommand Run { get; set; } //Command functionality, accepting string arguments.

	}
	public delegate string ConsoleCommand(List<string> args); 

}
