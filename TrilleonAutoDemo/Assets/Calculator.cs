using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TrilleonAutomation;

public class Calculator : MonoBehaviour {

	public static string CurrentAnswer { get; private set; }

	void Start() {

		CurrentAnswer = string.Empty;

	}

	public static void ButtonClick(string button) {
		
		int val = 0;
		if(int.TryParse(button, out val)) {

			CurrentAnswer += button;

		} else {

			switch(button) {
				case "PN":
					CurrentAnswer = CurrentAnswer.Length > 0 ? CurrentAnswer.Substring(0, 1) != "-" ? string.Format("-{0}", CurrentAnswer) : string.Format("{0}", CurrentAnswer.Substring(1, CurrentAnswer.Length - 1)) : "-";
					break;
				case "D":
					if(!CurrentAnswer.Contains(".")) {
						
						CurrentAnswer += ".";

					}
					break;
				case "C":
					CurrentAnswer = string.Empty;
					break;
				case "E":
				List<Equation> matches = GameController.Self.ActiveEquations.FindAll(x => x != null && x.gameObject != null && x.CorrectAnswer == CurrentAnswer && (x.GetComponent<RectTransform>().localPosition.x < Screen.height / 2 || x.GetComponent<RectTransform>().localPosition.x > Screen.height / -2));
					if(matches.Any()) {
							
						for(int x = 0; x < matches.Count; x++) {

							GameController.UpdateScore(true, matches[x].GetComponent<Equation>().IsDuck);
							GameController.Self.ActiveEquations.Remove(matches[x]);
							GameObject destroyed = GameController.Self.DestroyedEquationPrefab;
							destroyed.transform.position = matches[x].gameObject.transform.position;
							if(!matches[x].GetComponent<Equation>().IsDuck) {
								
								DestroyedEquation destroyedEquation = destroyed.GetComponent<DestroyedEquation>();
								destroyedEquation.de1.character = matches[x].EquationIndexed[0].ToString();
								destroyedEquation.do1.character = matches[x].EquationIndexed[1].ToString();
								destroyedEquation.de2.character = matches[x].EquationIndexed[2].ToString();
								if(matches[x].EquationIndexed.Count == 5) {

									destroyedEquation.do2.character = matches[x].EquationIndexed[3].ToString();
									destroyedEquation.de3.character = matches[x].EquationIndexed[4].ToString();

								}
								GameObject destroyedInstance = Instantiate(destroyed) as GameObject;
								destroyedInstance.transform.parent = GameController.Self.transform;

							}
                            GameController.Self.ActiveEquations.Remove(matches[x]);
							Destroy(matches[x].gameObject);

						}

					}
						
					CurrentAnswer = string.Empty;
					break;
			}

		}
		GameController.Self.CalculatorValueText.text = CurrentAnswer;

	}

}