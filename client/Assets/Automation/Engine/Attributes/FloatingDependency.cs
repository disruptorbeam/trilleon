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
using System.Collections.Generic;

namespace TrilleonAutomation {

	/// <summary>
	/// Used to declare another test method as being necessary in a test run.
	/// However, this marked test method does not need to be run before the current test,
	/// and can be requested at any point (or not at all) in the current test.
	/// NOTE: While this functionality could easily be accomplished without the use of an
	/// attribute, there is a need for validation, which is easily accomplished this way.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class FloatingDependency : Attribute {

		public List<string> Dependencies { 
			get { 
				return dependencies;
			}
		}
		List<string> dependencies = new List<string>();

		//Argument cannot be null or of 0 length. This is validated and enforced in AutomationMaster Validator method.
		public FloatingDependency(params string[] testMethods) {

			//Get the string name of all dependencies and store in list.
			for(int x = 0; x < testMethods.Length; x++) {

				dependencies.Add(testMethods[x]);

			}

		}

	}

}
