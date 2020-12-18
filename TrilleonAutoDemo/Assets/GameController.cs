using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController Self { get; private set; }
	public List<Equation> ActiveEquations = new List<Equation>();
	public Button NewGame;
	public Button Tutorial;
	public Button Exit;
	public Button ExitGame;
	public GameObject Background;
	public GameObject MainMenu;
	public Calculator Calculator;
	public Text GetReady;
	public Text LevelText;
	public Text ScoreText;
	public Text ScoreChangeText;
	public Text CalculatorValueText;
	public Text GameLevelCompleteText;
	public GameObject EquationPrefab { get; private set; }
	public GameObject DuckyPrefab { get; private set; }
	public GameObject DestroyedEquationPrefab { get; private set; }

	public static float timer { get; set; }
	public static float interval { get; set; }
	public static bool GameStart { get; set; }
	public static bool GameQuit { get; set; }
	public static bool IsTutorial { get; set; }
	public static bool IntroDone { get; set; }
	public static int Points { get; set; }
	public const float StartWaitTime = 3f;
	public const int PointsForDuckCorrect = 8;
	public const int PointsForCorrectAnswer = 4;
	public const int PointsLostWhenEquationLeavesScreen = 2;
	public const float SuccessChangeFadeTime = 10f;

	public static int LevelScoreThreshold = 50;
	void Start () {
		
		TrilleonAutomation.AutomationMaster.Initialize();
		EquationPrefab = Resources.Load<GameObject>("Equation");
		DuckyPrefab = Resources.Load<GameObject>("Ducky");
		DestroyedEquationPrefab = Resources.Load<GameObject>("DestroyedEquation");
		Self = this;
		Exit.interactable = Application.isEditor ? false : true;

	}

	public void ExitToDesktop() {

		Application.Quit();

	}
		
	void Update () {

		CheckLevel();

		if(GameStart && !GameQuit && !IsTutorial) {

			if(!IntroDone) {

				IntroDone = true;
				GameController.Self.StartCoroutine(ReadySteadyGo());

			}
			timer += Time.deltaTime;
			if(timer >= interval) {

				if(timer < StartWaitTime) {

					return;

				}

				if(new System.Random().Next(0, 25) > 22) {

					SpawnDuck();

				} else {

					SpawnEquation();

				}

			}

		}

	}

	void CheckLevel() {

		//TODO: Add more level with varition
		if(Points > LevelScoreThreshold && GameStart) {

			GameStart = false;
			StartCoroutine(GameOver());

		}

	}

	public void Quit() {

		GameQuit = true;
		IsTutorial = false;
		TutorialController.Self.TutorialText.color = GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);
		TutorialController.Self.StopAllCoroutines();
		StartCoroutine(GameOver());

	}

	public IEnumerator GameOver() {

		IsTutorial = GameStart = ExitGame.interactable = false;
		ExitGame.gameObject.SetActive(false);
		ScoreText.gameObject.SetActive(false);
		LevelText.gameObject.SetActive(false);
		LevelText.text = "Level 1";
		Calculator.gameObject.SetActive(false);
		MainMenu.gameObject.SetActive(false);

		try {
			
			ActiveEquations.ForEach(x => Destroy(x.gameObject));

		} catch { }

		GameLevelCompleteText.text = GameQuit ? "Quitting's For The Birds!" : "Quacktastic. You Win!";
		int alpha = 255;
		while(alpha > 0) {

			GameController.Self.GameLevelCompleteText.color = (Color)new Color32((byte)255, (byte)150, (byte)0, (byte)alpha);
			alpha -= 2;
			yield return new WaitForEndOfFrame();

		}
		GameLevelCompleteText.text = string.Empty;
		MainMenu.gameObject.SetActive(true);
		yield return null;

	}

	public void GameActivate() {

		GameQuit = false;
		GameStart = true;
		ExitGame.interactable = true;
		ExitGame.gameObject.SetActive(true);
		ScoreText.gameObject.SetActive(true);
		LevelText.gameObject.SetActive(true);
		LevelText.text = "Level 1";
		Calculator.gameObject.SetActive(true);
		MainMenu.gameObject.SetActive(false);

	}

	public static void UpdateScore(bool success, bool isDuck) {

		Points += success ? (isDuck ? PointsForDuckCorrect : PointsForCorrectAnswer) : -PointsLostWhenEquationLeavesScreen;
		if(Points < 0) {

			Points = 0;

		}
		GameController.Self.ScoreText.text = string.Format("Score: {0}", Points);
		GameController.Self.ScoreChangeText.text = string.Format("{0} {1}", success? "+" : "-", success ? (isDuck ? PointsForDuckCorrect : PointsForCorrectAnswer) : PointsLostWhenEquationLeavesScreen);
		GameController.Self.StopCoroutine("UpdateScoreChangeText");
		GameController.Self.StartCoroutine(GameController.Self.UpdateScoreChangeText());

	}

	public void SpawnEquation() {
		
		GameObject newEquation = Instantiate(EquationPrefab) as GameObject;
		RectTransform rect = newEquation.GetComponent<RectTransform>();
		rect.localPosition = new Vector3(new System.Random().Next(-Screen.width / 2 + (int)rect.sizeDelta.x + 10, Screen.width / 2 - (int)rect.sizeDelta.x), (-Screen.height / 2) - 125, 0);
		rect.SetParent(this.transform, false);
		interval = new System.Random().Next(40, 75) / 10f;
		timer = 0;

	}

	public void SpawnDuck() {

		GameObject newEquation = Instantiate(DuckyPrefab) as GameObject;
		RectTransform rect = newEquation.GetComponent<RectTransform>();
		rect.localPosition = new Vector3(new System.Random().Next(-Screen.width / 2 + (int)rect.sizeDelta.x + 10, Screen.width / 2 - (int)rect.sizeDelta.x), (-Screen.height / 2) - 125, 0);
		rect.SetParent(this.transform, false);
		interval = 75 / 10f;
		timer = 0;

	}

	public IEnumerator UpdateScoreChangeText() {

		int alpha = 255;
		while(alpha > 0) {

			alpha -= 5;
			GameController.Self.ScoreChangeText.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			yield return new WaitForEndOfFrame();

		}
		yield return null;

	}

	public IEnumerator ReadySteadyGo() {

		int alpha = 255;
		GameController.Self.GetReady.text = "Ready";
		while(alpha > 0) {

			GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 5;
			yield return new WaitForEndOfFrame();

		}
		GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		GameController.Self.GetReady.text = "Steady";
		Canvas.ForceUpdateCanvases();
		while(alpha > 0) {

			GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 5;
			yield return new WaitForEndOfFrame();

		}
		GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		alpha = 255;
		int rand = TrilleonAutomation.Q.help.RandomIndex(10);
		if(rand > 5) {

			GameController.Self.GetReady.text = "Quack!";

		} else {
			
			GameController.Self.GetReady.text = "Go!";

		}

		Canvas.ForceUpdateCanvases();
		while(alpha > 0) {

			GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)alpha);
			alpha -= 5;
			yield return new WaitForEndOfFrame();

		}
		GameController.Self.GetReady.color = (Color)new Color32((byte)255, (byte)255, (byte)255, (byte)0);

		GameController.Self.GetReady.gameObject.SetActive(false);
		GameController.Self.ScoreText.gameObject.SetActive(true);
		yield return null;

	}

}