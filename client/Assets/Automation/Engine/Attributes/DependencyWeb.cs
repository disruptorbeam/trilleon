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

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class DependencyWeb : Attribute {

		public List<string> Dependencies { 
			get { 
				return dependencies;
			}
		}
		List<string> dependencies = new List<string>();

		public List<string> OneOfDependencies { 
			get { 
				return oneOfDependencies;
			}
		}
		List<string> oneOfDependencies = new List<string>();

		//Argument cannot be null or of 0 length. This is validated and enforced in AutomationMaster Validator method. Methods passed into this attribute will be required to run and pass before the current test can execute.
		public DependencyWeb(params string[] testMethods) {

			for(int x = 0; x < testMethods.Length; x++) {

				dependencies.Add(testMethods[x]);

			}

		}

		//Alternative form of dependency where the first argument represents all tests that MUST be run to execute this test (like above), but also includes an argument for a list of test methods where the only requirement is that at least one of the contained tests have passed.
		public DependencyWeb(string[] andTestMethods, string[] orTestMethods) {

			for(int x = 0; x < andTestMethods.Length; x++) {

				dependencies.Add(andTestMethods[x]);

			}

			for(int x = 0; x < orTestMethods.Length; x++) {

				OneOfDependencies.Add(orTestMethods[x]);

			}

		}

	}

}
