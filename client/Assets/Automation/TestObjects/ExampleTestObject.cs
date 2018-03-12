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