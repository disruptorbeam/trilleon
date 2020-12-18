using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TrilleonAutomation;

public class TutorialController : MonoBehaviour {

	public static TutorialController Self { get; private set; }
	public Text TutorialText;

	public static bool TutorialStart { get; set; }
	public static bool IsTutorial { get; set; }
	public static int TutorialStep { get; set; }
	public static bool NextTutorialStep { get; set; }
	public static bool IntroDone { get; set; }
	public static GameObject TutorialEquation { get; set; }
	public static GameObject TutorialDuck { get; set; }

	void Start () {

		Self = this;

	}

	void Update () {

		if(TutorialStart && IsTutorial) {

			if(NextTutorialStep) {

				StartCoroutine(TutorialNext());

			}

		}

	}

	void TutorialOver() {

		GameController.Self.ExitGame.interactable = false;
		GameController.Self.ExitGame.gameObject.SetActive(false);
		TutorialStart = false;
		GameController.IsTutorial = false;
		StartCoroutine(GameController.Self.GetComponent<GameController>().GameOver());

	}

	public void TutorialActivate() {

		TutorialStart = GameController.IsTutorial = GameController.Self.ExitGame.interactable = true;
		GameController.Self.ExitGame.gameObject.SetActive(true);
		GameController.Self.ScoreText.gameObject.SetActive(true);
		GameController.Self.LevelText.gameObject.SetActive(true);
		GameController.Self.LevelText.text = "Level 1";
		GameController.Self.Calculator.gameObject.SetActive(true);
		GameController.Self.MainMenu.gameObject.SetActive(false);
		StartCoroutine(TutorialNext());

	}

	IEnumerator TutorialNext() {

		switch(TutorialStep) {
			case 0:
				yield return StartCoroutine(StepOne_Intro());
				break;
			case 1:

				break;
			case 2:

				break;
			case 3:

				break;
			case 4:

				break;
			case -1:
				TutorialOver();
				break;
		}
		NextTutorialStep = false;
		yield return null;

	}

	IEnumerator StepOne_Intro() {

		int alpha = 255;
		TutorialText.text = "Welcome to Quackulator!";
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 3;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.007f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Equations will appear from the bottom of the screen.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Type the answer in the calculator and press enter.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "All division arithmetic is rounded to the first decimal place.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Give it a try now.";
		yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 4;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.005f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		int pointsAtStart = GameController.Points;
		int triesIndex = 0;
		bool success = false;
		while(!success) {
			
			TutorialEquation = Instantiate(GameController.Self.EquationPrefab) as GameObject;
			TutorialEquation.GetComponent<Equation>().Id = triesIndex;
			RectTransform rect = TutorialEquation.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(new System.Random().Next(-Screen.width / 2 + (int)rect.sizeDelta.x + 10, Screen.width / 2 - (int)rect.sizeDelta.x), (-Screen.height / 2) - 125, 0);
			rect.SetParent(this.transform, false);

			yield return StartCoroutine(Q.driver.Try.WaitFor(() => TutorialEquation == null, timeout: 45f));
			if(GameController.Points <= pointsAtStart) {

				alpha = 255;
				TutorialText.text = "Let's try again.";
				yield return new WaitForEndOfFrame();
				while(alpha > 0) {

					TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
					alpha -= 1;
					yield return StartCoroutine(Q.driver.WaitRealTime(0.005f));

				}
				TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);
				pointsAtStart = GameController.Points;

			} else {

				success = true;
				break;

			}
			triesIndex++;

		}

		alpha = 255;
		TutorialText.text = "Great! It's relatively simple.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.007f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = string.Format("You lose {0} points when unanswered questions reach the top of the screen.", GameController.PointsLostWhenEquationLeavesScreen);
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.015f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = string.Format("You gain {0} points when answering questions correctly.", GameController.PointsForCorrectAnswer);
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 2;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Next are the ducks.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 3;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.005f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Ducks randomly appear. Wait for them to say something.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 1;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "When you know what noise this duck makes, you can answer their simple riddle.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 1;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = "Use the rotary dial system of old, starting at the number 1, which represents a, b, and c.";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 1;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.015f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		TutorialText.text = string.Format("When you type the number representing the duck noise, you receive {0} points", GameController.PointsForDuckCorrect);
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 1;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.015f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		triesIndex = 0;
		success = false;
	 	pointsAtStart = GameController.Points;
		while(!success) {

			TutorialDuck = Instantiate(GameController.Self.DuckyPrefab) as GameObject;
			TutorialDuck.GetComponent<Equation>().Id = triesIndex;
			RectTransform rect = TutorialDuck.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(new System.Random().Next(-Screen.width / 2 + (int)rect.sizeDelta.x + 10, Screen.width / 2 - (int)rect.sizeDelta.x), (-Screen.height / 2) - 125, 0);
			rect.SetParent(this.transform, false);

			yield return StartCoroutine(Q.driver.Try.WaitFor(() => TutorialDuck == null, timeout: 45f));
			if(GameController.Points <= pointsAtStart) {

				alpha = 255;
				TutorialText.text = "Let's try again.";
				yield return new WaitForEndOfFrame();
				while(alpha > 0) {

					TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
					alpha -= 1;
					yield return StartCoroutine(Q.driver.WaitRealTime(0.005f));

				}
				TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);
				pointsAtStart = GameController.Points;

			} else {

				success = true;
				break;

			}
			triesIndex++;

		}

		alpha = 255;
		TutorialText.text = "There! You've got a hang of it. Get playing!";
		yield return new WaitForEndOfFrame();
		while(alpha > 0) {

			TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 1;
			yield return StartCoroutine(Q.driver.WaitRealTime(0.01f));

		}
		TutorialText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);
		TutorialOver();
		yield return null;

	}

}