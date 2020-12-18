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
	public class TestRunnerFlags : Attribute {

		public List<TestFlag> Flags { 
			get { 
				return flags;
			}
		}
		List<TestFlag> flags = new List<TestFlag>();

		/// <summary>
		/// Adds special flags to an Automation test that cause important behavioral changes in the test runner (AutomationMaster).
		/// </summary>
		/// <param name="testCategoryName"> TestFlag to add. </param>
		public TestRunnerFlags(params TestFlag[] Flags) {

			this.flags.AddRange(Flags.ToList());

		}

	}
}
