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
	///   These Demo tests will always skip if the current test run involves a single device. 
	///   Required:
	///   1) Communication tool for multiple devices/servers to communicate on.
	///   2) Another client to communicate with this one.
	///   3) Explicit or implicit avenue for these two devices to determine that they should form a "Buddy" relationship.
	/// </summary>
	[AutomationClass]
	[DebugClass]
	public class BuddySystemTests : MonoBehaviour {

		[Automation("Debug/Demo")]
		[BuddySystem(Buddy.Action)]
		public IEnumerator BuddySystemTest_Action() {

			string requiredValue = "I AM A BUDDY PRIMARY";
			BuddyHandler.SendInfoForBuddyTests("REQUIRED_VALUE", requiredValue);
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

		}

		//You can have any number of Reactions or CounterReactions that point to the single, above Action test.
		[Automation("Debug/Demo")]
		[BuddySystem(Buddy.Reaction, "BuddySystemTest_Action")]
		public IEnumerator BuddySystemTest_Reaction() {

			//Verify that the required test data was passed from the Primary buddy to the Secondary Buddy.
			string expectedValue = BuddyHandler.GetValueFromBuddyPrimaryTestForSecondaryTest("REQUIRED_VALUE");
			yield return StartCoroutine(Q.assert.IsTrue(expectedValue == "I AM A BUDDY PRIMARY", "Buddy primary failed to provide required information for secondary reaction test."));
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

		}

		[Automation("Debug/Demo")]
		[BuddySystem(Buddy.CounterReaction, "BuddySystemTest_Action")]
		public IEnumerator BuddySystemTest_CounterReaction() {

			//Some logic to verify that Reaction test had expected effect.
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

		}

	}

}
