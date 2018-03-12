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
+*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to demonstrate how to use UnityTests to test private Methods and IEnumerators, along with asserting values of private variables in code outside of the Trilleon namespace (ex: In your game code).
/// </summary>
public class ExampleMonoForUnitTest : MonoBehaviour {

	string somePrivateString = string.Empty;
	bool somePrivateBool { get; set; }

	IEnumerator SetSomeValues(string valueToSet) {

		somePrivateString = valueToSet;
		somePrivateBool = true;
		Debug.Log(somePrivateString + somePrivateBool);
		yield return null;

	}

}