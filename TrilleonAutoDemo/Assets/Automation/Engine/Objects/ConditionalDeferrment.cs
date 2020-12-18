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

namespace TrilleonAutomation {

	/// <summary>
	/// IMPORTANT! Any variables you set from the context of an instance of a test class will be lost by the time a deferred test is run. 
	/// For example: If you have three tests, and once is deferred, the MonoBehaviour is destroyed and removed from its GameObject. This will result in 
	/// your instance variables to be lost. Make sure you save data necessary between tests in a static variable within your test class, or save it in the 
	/// Q.hash system so that the test data is always available from anywhere.
	/// </summary>
	public class ConditionalDeferrment {

		private const int DEFAULT_DEFERRMENTS_ALLOWED = 2;
		public string TestMethod { get; private set; }
		public Func<bool> Condition { get; private set; }
		public int NumberOfTimesToAllowsTestToBeDeferred { get; private set; }
		public int NumberOfTimesAlreadyDeferred { get; set; }

		public ConditionalDeferrment(IEnumerator testMethod, Func<bool> conditionalPredicate, int numberOfTimesToAllowsTestToBeDeferred = DEFAULT_DEFERRMENTS_ALLOWED) {

			TestMethod = testMethod.ToString().Split('<').ToList().Last().Split('>').ToList().First();
			Condition = conditionalPredicate;
			NumberOfTimesToAllowsTestToBeDeferred = numberOfTimesToAllowsTestToBeDeferred;

		}
 
	}

}
