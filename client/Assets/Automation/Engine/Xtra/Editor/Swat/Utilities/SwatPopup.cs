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

﻿using UnityEditor;

public abstract class SwatPopup : EditorWindow {

	public virtual void Pop() {} //Creates new window and calls Set to render. Any number of parameters.
	public virtual void Set() {} //Sets data that does not need to be changed with each render pass. Should call PositionWindow once. Any number of parameters.

	public abstract void PositionWindow(); //Sets position of window within the context of its parent caller.
	public abstract void OnGUI(); //Actual renderable GUI code.
	public abstract bool Visible(); //Is the window currently visible?

}
