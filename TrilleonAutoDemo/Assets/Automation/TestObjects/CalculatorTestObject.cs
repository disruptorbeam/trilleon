using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TrilleonAutomation {

	public class CalculatorTestObject : MonoBehaviour {

		public static CalculatorTestObject steps {
			get {
				CalculatorTestObject self = AutomationMaster.StaticSelf.GetComponent<CalculatorTestObject>();
				if(self == null) {
					return AutomationMaster.StaticSelf.AddComponent<CalculatorTestObject>();
				}
				return self;
			}
		}

		public static GameController primary {
			get {
				return GameController.Self;
			}
		}

		public static List<CalculatorButton> buttons_calculatorButtons {
			get {
                if(_buttons_calculatorButtons == null || _buttons_calculatorButtons.Count == 0) {

					_buttons_calculatorButtons = Q.driver.FindAll<CalculatorButton>();

				}
				return _buttons_calculatorButtons;
			}
		}
		private static List<CalculatorButton> _buttons_calculatorButtons;

		public static List<GameObject> objs_ducks {
			get {
				return Q.driver.FindAllIn(primary, By.Name, "Ducky");
			}
		}

		public static string string_currentAnswerOnCalculatorDisplay {
			get {
				return Calculator.CurrentAnswer;
			}
		}

		public static GameObject GetButton(string buttonIdentifier) {

			CalculatorButton match = buttons_calculatorButtons.Find(x => x.Value == buttonIdentifier);
			return match == null ? null : match.gameObject;

		}

		public IEnumerator AnswerEquation(Equation equation) {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any(), "Equations appear in play area."));
			yield return StartCoroutine(Q.assert.IsTrue(equation != null && !string.IsNullOrEmpty(equation.CorrectAnswer), "Supplied equation is null."));

			List<string> calcButtonsToPress = Regex.Split(equation.CorrectAnswer, string.Empty).ToList().RemoveNullAndEmpty();
			List<string> calcButtonsPressed = new List<string>();
			for(int b = 0; b < calcButtonsToPress.Count; b++) {

				string button = calcButtonsToPress[b];
				if(button == "-") {

					button = "PN";

				} else if(button == ".") {

					button = "D";

				}
				yield return StartCoroutine(Q.driver.Click(GetButton(button), string.Format("Select Calculator button \"{0}\"", calcButtonsToPress[b])));
				calcButtonsPressed.Add(calcButtonsToPress[b]);
				string expectedVal = string.Join(string.Empty, calcButtonsPressed.ToArray());
				yield return StartCoroutine(Q.assert.IsTrue(string_currentAnswerOnCalculatorDisplay.Trim() == expectedVal, string.Format("Displayed value \"{0}\" on Calculator should match the currently-entered value \"{1}\".", string_currentAnswerOnCalculatorDisplay, expectedVal)));

			}

            GameObject e = GetButton("E");
			int currentScore = QwackulatorGameTestObject.int_score;
            yield return StartCoroutine(Q.driver.Click(e,  "Press enter on calculator to submit answer."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.int_score > currentScore, string.Format("Score of \"{0}\" should increase. Now displaying \"{1}\".", currentScore, 1f), 2.5f));
			yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(equation, "Correctly answered equation is removed from play area.", true));

		}

	}

}