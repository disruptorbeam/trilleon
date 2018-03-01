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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TrilleonAutomation {

	public class ExampleTestObject : MonoBehaviour {

		public static ExampleTestObject steps {
			get {
				ExampleTestObject self = AutomationMaster.StaticSelf.GetComponent<ExampleTestObject>();
				if(self == null) {
					return AutomationMaster.StaticSelf.AddComponent<ExampleTestObject>();
				}
				return self;
			}
		}

		public static ExampleTopLevelClass primary {
			get {
				//return MyGamesSingletonManager.GetTopLevelClassReference<ExampleTopLevelClass>();
				return new GameObject().GetComponent<ExampleTopLevelClass>();
			}
		}

		public static Button button_register {
			get {
				return primary.Some_button;
			}
		}

		public static InputField input_register {
			get {
				return primary.Some_field;
			}
		}

	}

	//This is an example class that represents a top-level script attached to many top-level GameObjects in the game code.
	public class ExampleTopLevelClass : MonoBehaviour {

		public Button Some_button { get; set; }
		private Button _some_button { get; set; }

		public InputField Some_field { get; set; }
		private InputField _some_field { get; set; }

	}

}
