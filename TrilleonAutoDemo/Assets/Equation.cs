using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TrilleonAutomation;

public class Equation : MonoBehaviour {

	static int IdCounter = 0;
	float randomizedPixelPerFrameSpeed;
	public string CorrectAnswer = string.Empty;
	public string DuckCall = string.Empty;
	public int Id;
	public bool IsDuck;
	public Text EquationText;
	public Text QuackText;
	public List<char> EquationIndexed = new List<char>();

	int QuackTextYAbsMax = 45;
	int QuackTextYAbsMin = -50;
	int QuackTextXAbsMax = 115;
	int QuackTextXAbsMin = -10;

	static List<char> operators  = new List<char> { '/', '%', 'x', '+', '-' }; //Order by operator precedence!
	static List<KeyValuePair<int,char[]>> rotaryDial = new List<KeyValuePair<int,char[]>> {
		new KeyValuePair<int,char[]>(0, new char[] {} ),
		new KeyValuePair<int,char[]>(1, new char[] { 'A', 'B', 'C' } ),
		new KeyValuePair<int,char[]>(2, new char[] { 'D', 'E', 'F' } ),
		new KeyValuePair<int,char[]>(3, new char[] { 'G', 'H', 'I' } ),
		new KeyValuePair<int,char[]>(4, new char[] { 'J', 'K', 'L' } ),
		new KeyValuePair<int,char[]>(5, new char[] { 'M', 'N', 'O' } ),
		new KeyValuePair<int,char[]>(6, new char[] { 'P', 'Q', 'R' } ),
		new KeyValuePair<int,char[]>(7, new char[] { 'S', 'T', 'U' } ),
		new KeyValuePair<int,char[]>(8, new char[] { 'V', 'W', 'X' } ),
		new KeyValuePair<int,char[]>(9, new char[] { 'Y', 'Z' } )
	};

	static List<string> duckCalls = new List<string> {
		"Quack",
		"Honk",
		"Squeak",
		"Coo",
		"Peep"
	};

	void Start() {

		randomizedPixelPerFrameSpeed = new System.Random().Next(25, 80);
		Id = IdCounter++;

		if(name.Contains("Duck")) {
			
			IsDuck = true;
			CorrectAnswer = string.Empty;
			DuckCall = duckCalls.Random();
			QuackText.text = DuckCall;
			char[] letters = DuckCall.ToUpper().ToCharArray();
			for(int x = 0; x < letters.Length; x++) {

				KeyValuePair<int,char[]> match = rotaryDial.Find(r => r.Value.ToList().Contains(letters[x]));
				CorrectAnswer += match.Key.ToString();

			}
			StartCoroutine(Quack());

		} else {

			System.Random r = new System.Random();
			double firstNum = (double)r.Next(1, 10);
			EquationIndexed.Add(firstNum.ToString().ToCharArray().First());
			char firstOperator = operators[r.Next(0, operators.Count)];
			EquationIndexed.Add(firstOperator);
			double secondNum = (double)r.Next(1, 10);
			EquationIndexed.Add(secondNum.ToString().ToCharArray().First());
			char secondOperator = operators[r.Next(0, operators.Count)];
			EquationIndexed.Add(secondOperator);
			double thirdNum = (double)r.Next(1, 10);
			EquationIndexed.Add(thirdNum.ToString().ToCharArray().First());
			CorrectAnswer = SimpleEvaluate(firstNum, firstOperator, secondNum, secondOperator, thirdNum);
			EquationText = GetComponent<Text>();
			EquationText.text = string.Format("{0} {1} {2} {3} {4}", firstNum, firstOperator, secondNum, secondOperator, thirdNum);

		}

		GameController.Self.ActiveEquations.Add(this);

	}
	
	// Update is called once per frame
	void Update () {

		RectTransform rect = this.GetComponent<RectTransform>();
		rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + (randomizedPixelPerFrameSpeed * Time.deltaTime), -2.5f);

		if(rect.localPosition.y > Screen.height + 20) {

			GameController.UpdateScore(false, IsDuck);
			GameController.Self.ActiveEquations.Remove(this);
			Destroy(gameObject);

		}

	}

	IEnumerator Quack() {

		while(true) {

			//Choose random place for Quack to appear in relation to Duck.
			int anchorAxis = Q.help.RandomIndex(1); //0 is anchor X, 1 is anchor Y.
			yield return StartCoroutine(Q.driver.WaitRealTime(1f));
			int anchorMaxMin = Q.help.RandomIndex(1);//0 is anchor Max, 1 is anchor Min.

			float xPos = 0;
			float yPos = 0;
			if(anchorAxis == 0 && anchorMaxMin == 0) {

				xPos = QuackTextXAbsMax;

			} else if(anchorAxis == 1 && anchorMaxMin == 0) {

				yPos = QuackTextYAbsMax;

			} else if(anchorAxis == 0 && anchorMaxMin == 1) {

				xPos = QuackTextXAbsMin;

			} else {

				yPos = QuackTextYAbsMin;

			}
				
			if(anchorAxis == 0) {

				yPos = new System.Random().Next(QuackTextYAbsMin, QuackTextYAbsMax + 1);

			} else {

				xPos = new System.Random().Next(QuackTextXAbsMin, QuackTextXAbsMax + 1);

			}

			QuackText.transform.localPosition = new Vector3(xPos, yPos, 0);
			QuackText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)255);

			int alpha = 255;
			while(alpha > 0) {

				alpha -= 5;
				QuackText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
				yield return new WaitForEndOfFrame();

			}
			yield return StartCoroutine(Q.driver.WaitRealTime(1f));

		}

	}

	string SimpleEvaluate(double val1, char op1, double val2, char op2, double val3) {

		if(op1 == '/') {

			return Evaluate(val1 / val2, op2, val3);

		}
		if(op2 == '/') {

			return Evaluate(val1, op1, val2 / val3);

		}
		if(op1 == '%') {

			return Evaluate(val1 % val2, op2, val3);

		}
		if(op2 == '%') {

			return Evaluate(val1, op1, val2 % val3);

		}
		if(op1 == 'x') {

			return Evaluate(val1 * val2, op2, val3);

		}
		if(op2 == 'x') {

			return Evaluate(val1, op1, val2 * val3);

		}
		if(op1 == '+') {

			return Evaluate(val1 + val2, op2, val3);

		}
		if(op1 == '-') {

			return Evaluate(val1 - val2, op2, val3);

		}
		return string.Empty;

	}

	string Evaluate(double val1, char op, double val2) {

		switch(op) {
			case '/':
				return System.Math.Round((double)val1 / val2, 1).ToString();
			case '%':
				return System.Math.Round((double)val1 % val2, 1).ToString();
			case 'x':
				return System.Math.Round((double)val1 * val2, 1).ToString();
			case '+':
				return System.Math.Round((double)val1 + val2, 1).ToString();
			case '-':
				return System.Math.Round((double)val1 - val2, 1).ToString();
			default:
				return string.Empty;
		}

	}

	/*
	string SimpleEvaluate(string expression) {

		List<char> chars = expression.ToCharArray().ToList().FindAll(x => !char.IsWhiteSpace(x));
		List<string> numbers = new List<string>();
		for(int x = 0; x < chars.Count; x++) {

			int z = 0;
			if(int.TryParse(chars[x].ToString(), out z)) {
				
				string thisVal = string.Empty;
				while(x < chars.Count - 1) {

					if(int.TryParse(chars[x].ToString(), out z)) {

						thisVal += chars[x].ToString();
						x++; 

					}

				}
				numbers.Add(thisVal);

			}

			numbers.Add(chars[x]);

		}
		for(int o = 0; 0 < chars.Count; o++) {

			List<int> indexes = new List<int>();
			for(int m = 0; m < numbers.Count; m++) {

				if(numbers[m] == chars[0]) {

					indexes.Add(m);

				}

			} 

			//TODO: Remove calculated pieces from list, and amalgamate values to get expression result.
			List<float> evaluated = new List<float>();
			for(int i = 0; i < indexes.Count; i++) {

				int result = 0;
				switch(chars[0]) {
					case "/":
						result = numbers.GetIndex(indexes[i] - 1) / numbers.GetIndex(indexes[i] + 1);
						break;
					case "%":
						result = numbers.GetIndex(indexes[i] - 1) % numbers.GetIndex(indexes[i] + 1);
						break;
					case "+":
						result = numbers.GetIndex(indexes[i] - 1) + numbers.GetIndex(indexes[i] + 1);
						break;
					case "-":
						result = numbers.GetIndex(indexes[i] - 1) - numbers.GetIndex(indexes[i] + 1);
						break;
				}
				evaluated.Add(result);

			}

		}

	}
	*/

}