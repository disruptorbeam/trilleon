/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using UnityEngine;
using UnityEditor;

namespace TrilleonAutomation {

	public class Manifest : SwatWindow {

		public override void Set() {


		}

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void OnTabSelected() { }

		public override void Render() {

			GUIStyle tests = new GUIStyle(EditorStyles.foldout);
			tests.margin = new RectOffset(15, 15, 2, 2);
			GUILayout.Space(25);
			for(int x = 0; x < AutomationMaster.Methods.Count; x++) {

				EditorGUILayout.Foldout(false, AutomationMaster.Methods[x].Value.Name, tests);

			}

		}

	}

}
