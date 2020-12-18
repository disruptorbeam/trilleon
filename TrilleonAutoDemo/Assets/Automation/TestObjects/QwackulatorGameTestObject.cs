using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TrilleonAutomation {

	public class QwackulatorGameTestObject : MonoBehaviour {

		public static QwackulatorGameTestObject steps {
			get {
				QwackulatorGameTestObject self = AutomationMaster.StaticSelf.GetComponent<QwackulatorGameTestObject>();
				if(self == null) {
					return AutomationMaster.StaticSelf.AddComponent<QwackulatorGameTestObject>();
				}
				return self;
			}
		}

		public static GameController primary {
			get {
				return GameController.Self;
			}
		}

		public static Button button_startGame {
			get {
				return primary.NewGame;
			}
		}

		public static Button button_exitGame {
			get {
				return primary.ExitGame;
			}
		}

		public static Button button_startTutorial {
			get {
				return primary.Tutorial;
			}
		}

		public static Text gui_tutorialText {
			get {
				return TutorialController.Self.TutorialText;
			}
		}

		public static Equation equation_tutorial {
			get {
				return TutorialController.TutorialEquation ==  null ? null : TutorialController.TutorialEquation.GetComponent<Equation>();
			}
		}

		public static Equation duck_tutorial {
			get {
				return TutorialController.TutorialDuck ==  null ? null : TutorialController.TutorialDuck.GetComponent<Equation>();
			}
		}

		public static int int_score {
			get {
				return primary.ScoreText.text.Replace("Score:", string.Empty).ToInt();
			}
		}

		public static List<Equation> objs_activeEquations {
			get {
				return primary.ActiveEquations;
			}
		}

	}

}