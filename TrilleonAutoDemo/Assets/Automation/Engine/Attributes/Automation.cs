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

namespace TrilleonAutomation {

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Automation : Attribute {

		public string CategoryName { get; private set; }

		/// <summary>
		/// A name to categorize the tagged test. Enforces organization. Most, if not all, tests in the same class should share at least one category name.
		/// </summary>
		/// <param name="testCategoryName"> Category name of declared test. </param>
		public Automation(string CategoryName) {

			this.CategoryName = CategoryName;

		}

	}

}