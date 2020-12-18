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
using System.Collections;

namespace TrilleonAutomation {

	/// <summary>
	/// This class demonstrates how DependencyClass and  DependencyTest attributes used in unison affect test execution.
	/// </summary>
	[AutomationClass]
	[DependencyClass(0)]
	[DebugClass]
	public class DependencyMasterTests : MonoBehaviour {

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[DependencyTest(1)]
		[Validate(Expect.Success)]
		[Validate(Expect.OrderRan, "1")]
		public IEnumerator DependencyMasterTest_01() {

			yield return StartCoroutine(Q.assert.IsTrue(true, "If I am included in a test run AT ALL, I will be the FIRST test run, period. This is, firstly, because my class is marked as a DependencyClass with an order of 1."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "The attribute DependencyClass grants a class priority over all other tests based on the provided order."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "Since I also am marked as DependencyTest 1, I am the first test in the first class that will be run."));

		}

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[DependencyTest(2)]
		[Validate(Expect.Success)]
		[Validate(Expect.OrderRan, "2")]
		public IEnumerator DependencyMasterTest_02() {

			yield return StartCoroutine(Q.assert.IsTrue(true, "Because I have a DependencyTest attribute with an order of 2, I am the second test in the first class that will be run."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "While I appear visually as the second test in this class, if I did not have the DependencyTest attribute, I would not be the second test to run."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "This is because there are other tests below me with a DependencyTest attribute that always take priority."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "Note that \"DependencyMasterTest_03\" does not appear below me anywhere. Don't worry, that's intended. Find it in the ExampleDemoTests class!"));

		}

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[DependencyTest(5)]
		[Validate(Expect.Skipped)]
		[Validate(Expect.OrderRan, "5")]
		public IEnumerator DependencyMasterTest_04() {

			yield return StartCoroutine(Q.assert.IsTrue(true, "I am set to run fifth within the context of this DependencyClass. However, I rely on the tests that ran before me."));
			yield return StartCoroutine(Q.assert.IsTrue(true, "Because \"DependencyMasterTest_05\" failed, my logic will be skipped, and you will not see my assertions in a test report."));

		}

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[DependencyTest(4)]
		[Validate(Expect.Failure)]
		[Validate(Expect.OrderRan, "4")]
		public IEnumerator DependencyMasterTest_05() {

			yield return StartCoroutine(Q.assert.Fail("Iam set to fail, which will affect any tests in this class that run after me and have a DependencyTest attribute!"));

		}

	}

}
