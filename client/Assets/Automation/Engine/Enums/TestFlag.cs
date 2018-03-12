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

﻿namespace TrilleonAutomation {

	/// <summary>
	/// Wan't specific assertions to not cease test execution during failure? Try `yield return StartCoroutine(Q.assert.Soft.CheckSomeValue())`. This indicates to the compiler NOT to cease execution of the test if just this assertion fails.
	/// Any subsequent assertion failures are ignored when reporting the cause of the test failure, although a future non-Soft assertion failure in the current test execution will still kill current test execution.
	/// </summary>
	public enum TestFlag {
		TryCompleteAfterFail, 
		OnlyLaunchWhenExplicitlyCalled,
		DependencyNoSkip,
		DisregardSetUpClassGlobal,
		DisregardTearDownClassGlobal,
		DisregardSetUpGlobal,
		DisregardTearDownGlobal,
		DisregardSetUpTest,
		DisregardTearDownTest,
	}

}
