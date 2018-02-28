using System;
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