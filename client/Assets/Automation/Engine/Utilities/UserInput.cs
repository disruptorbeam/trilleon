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
using UnityEngine;
using System.Runtime.InteropServices;

namespace UnityEngine {

   public static class UserInput {

		[DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
		//Mouse actions
		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;
		private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		private const int MOUSEEVENTF_RIGHTUP = 0x10;

		public static void Click(int x, int y) {
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
		}

		public static void Click(Component com) {
			Vector2 pos = Camera.main.WorldToScreenPoint(com.gameObject.transform.position);
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)pos.x, (uint)pos.y, 0, 0);
		}

		public static void Click(GameObject obj) {
			Vector2 pos = Camera.main.WorldToScreenPoint(obj.transform.position);
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)pos.x, (uint)pos.y, 0, 0);
		}

      private static KeyCode _keyDown { get; set; }
      private static string _keyPressed { get; set; }
      private static float _keyDownTime { get; set; }
      private static float _keyPressedTime { get; set; }
      private static float _keyDownDuration { get; set; }
      private static float _keyPressedDuration { get; set; }

      public static void SimulateKeyDown(KeyCode keyDown, int duration = 1) {

         _keyDown = keyDown;
         _keyDownTime = Time.time;
         _keyDownDuration = duration;

      }

      public static void SimulatePress(string keyPress, int duration = 1) {

         _keyPressed = keyPress;
         _keyPressedTime = Time.time;
         _keyPressedDuration = duration;

      }

      public static bool GetKeyDown(KeyCode key) {

         if(Time.time - _keyDownTime < _keyDownDuration) {

            return _keyDown == key;

         } else {

            return Input.GetKeyDown(key);

         }

      }

      public static bool GetButton(string key) {
         
         float time = Time.time;
         if(time - _keyPressedTime < _keyPressedDuration) {

            return _keyPressed == key;

         } else {

            return Input.GetButton(key);

         }

      }

   }

}
