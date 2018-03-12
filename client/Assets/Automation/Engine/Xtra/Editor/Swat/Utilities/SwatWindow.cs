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

public abstract class SwatWindow {

	public abstract void Set(); //Sets data that does not need to be changed with each render pass. Ran only once when window is first opened.
	public abstract void Render(); //Contains GUI elements to render and logic to perform with each pass. Effectively OnGUI.
	public abstract void OnTabSelected(); //Sets data that does not need to be changed with each render pass. Ran every time the tab is selected or re-selected.
	public abstract bool UpdateWhenNotInFocus(); //Should the window update/repaint when it is not in focus.

}
