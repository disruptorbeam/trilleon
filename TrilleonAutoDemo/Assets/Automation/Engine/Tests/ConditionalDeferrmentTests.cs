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

	[AutomationClass]
	[DebugClass]
	public class ConditionalDeferrmentTests : MonoBehaviour {

		static bool RequiredDummyValueIsSet = false;

		[SetUpClass]
		public IEnumerator SetUpClass() {

			Q.AddConditionalDeferrment(new ConditionalDeferrment(this.ConditionalDeferrment_DeferredUntilRequiredValueIsSet(), () => RequiredDummyValueIsSet, 1));
			yield return null;

		}

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[Validate(Expect.Success)]
		public IEnumerator ConditionalDeferrment_DeferredUntilRequiredValueIsSet() {

			yield return StartCoroutine(Q.assert.IsTrue(RequiredDummyValueIsSet && AutomationMaster.TestRunContext.CompletedTests.Contains("ConditionalDeferrment_SetRequiredValue"), "Required condition is set correctly. ConditionalDeferrment_SetRequiredValue ran first because this test was deferred."));

		}

		[Automation("Debug/Dependencies")]
		[Automation("Trilleon/Validation")]
		[Validate(Expect.Success)]
		public IEnumerator ConditionalDeferrment_SetRequiredValue() {

			RequiredDummyValueIsSet = true;
			yield return StartCoroutine(Q.assert.Pass("Setting RequiredDummyValueIsSet to true. This test should run before ConditionalDeferrment_DeferredUntilRequiredValueIsSet() because of the ConditionalDeferrment supplied in SetUpClass."));

		}

	}

}
