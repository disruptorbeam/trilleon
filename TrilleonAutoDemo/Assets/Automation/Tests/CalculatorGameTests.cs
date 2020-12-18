using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[AutomationClass]
	[DependencyClass(1)] //Makes this class (or any subset of tests within it) run first when the TestRunner determines order of execution.
	public class CalculatorGameTests : MonoBehaviour {

		[SetUpClass]
		public IEnumerator SetUpClass() {

			yield return null;

		}

		[SetUp]
		public IEnumerator SetUp() {

			yield return null;

		}
		[Automation("Gameplay")]
		public IEnumerator NewTest()
		{


			GameObject parentObject = null;
			parentObject = Q.driver.Find(By.Name, "MainMenu", false);
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "NewGame", false), "Click object with name NewGame"));
			parentObject = Q.driver.Find(By.Name, "Calculator", false);
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "Calc_5", false), "Click object with name Calc_5"));
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "Calc_4", false), "Click object with name Calc_4"));
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "Calc_6", false), "Click object with name Calc_6"));
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "Calc_6", false), "Click object with name Calc_6"));
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "Calc_8", false), "Click object with name Calc_8"));
			parentObject = Q.driver.Find(By.Name, "GameController", false);
			yield return StartCoroutine(Q.driver.Click(Q.driver.FindIn(parentObject, By.Name, "ExitGame", false), "Click object with name ExitGame"));

		}

		[Automation("Gameplay")]
		public IEnumerator CalculatorGameCanBeInitializedFromMainMenu() {

			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_startGame, "Select New Game button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any(), "Equations appear in play area."));

		}

		[Automation("Gameplay")]
		public IEnumerator ScoreCannotBecomeNegativeValue() {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any(), "Equations appear in play area."));
			Equation leadEquation = QwackulatorGameTestObject.objs_activeEquations.First();
			yield return StartCoroutine(Q.driver.Try.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any() && QwackulatorGameTestObject.objs_activeEquations.First().Id != leadEquation.Id, "Oldest equation is destroyed.", 30f));
			yield return StartCoroutine(Q.assert.IsTrue(QwackulatorGameTestObject.int_score == 0, "Score does not decrease below 0 when equation is destroyed before being correctly answered."));

		}

		[Automation("Gameplay/Equations")]
		public IEnumerator EquationCanBeAnsweredToIncreaseScore() {

            Equation leadEquation = QwackulatorGameTestObject.objs_activeEquations.FindAll(x => !x.IsDuck).Last(); //Grab youngest equation.
			yield return StartCoroutine(CalculatorTestObject.steps.AnswerEquation(leadEquation));

		}

		[Automation("Gameplay/Equations")]
		public IEnumerator EquationIsRemovedAndScoreDecreasedWhenAnswerIsNotProvided() {

			int score = (int)(Math.Round(GameController.LevelScoreThreshold / 2d));
			CommandConsoleBroker.SendCommand(string.Format("SetScore {0}", score));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.int_score != score && (score - QwackulatorGameTestObject.int_score) % GameController.PointsLostWhenEquationLeavesScreen == 0, string.Format("Score of {0} decreases by {1} (or x times that value if multiple equations despawn in same second) when equation is left unanswered. Displays {2}.", score, GameController.PointsLostWhenEquationLeavesScreen, QwackulatorGameTestObject.int_score), 20f));

		}

		[Automation("Gameplay/Calculator")]
		public IEnumerator EnteredValueInCalculatorCanBeCleared() {

			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("5"), "Select number 5."));
			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("0"), "Select number 0."));
			yield return StartCoroutine(Q.assert.IsTrue(CalculatorTestObject.string_currentAnswerOnCalculatorDisplay == "50", "Value entered shows 50 on Calculator."));
			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("C"), "Select Clear button.."));
			yield return StartCoroutine(Q.assert.IsTrue(CalculatorTestObject.string_currentAnswerOnCalculatorDisplay == string.Empty, "No value is displayed on Calculator."));

		}

		[Automation("Gameplay/Calculator")]
		public IEnumerator DecimalCanBeAddedToCalculatorAnswer() {

			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("D"), "Select Decimal button."));
			yield return StartCoroutine(Q.assert.IsTrue(CalculatorTestObject.string_currentAnswerOnCalculatorDisplay.Contains("."), "Decimal displays in Calculator text."));

		}

		[Automation("Gameplay/Calculator")]
		public IEnumerator CalculatorAnswerCanBeSwitchedBetweenPositiveAndNegative() {

			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("PN"), "Select +/- button."));
			yield return StartCoroutine(Q.assert.IsTrue(CalculatorTestObject.string_currentAnswerOnCalculatorDisplay.Contains("-"), "Minus sign displays in Calculator text."));
			yield return StartCoroutine(Q.driver.Click(CalculatorTestObject.GetButton("PN"), "Select +/- button."));
			yield return StartCoroutine(Q.assert.IsTrue(!CalculatorTestObject.string_currentAnswerOnCalculatorDisplay.Contains("-"), "Minus sign does not display in Calculator text."));

		}

		[Automation("Gameplay/Equations")]
		public IEnumerator DucksAppearInAdditionToEquations() {

			CommandConsoleBroker.SendCommand("Quack");			
			yield return StartCoroutine(Q.driver.WaitFor(() => CalculatorTestObject.objs_ducks.Any(), "A ducky appears."));
			Equation duck = CalculatorTestObject.objs_ducks.Last().GetComponent<Equation>();
			yield return StartCoroutine(CalculatorTestObject.steps.AnswerEquation(duck));
			yield return StartCoroutine(Q.driver.WaitFor(() => duck == null, "Duck object should be destroyed after correct answer.", 2f));

		}

		[Automation("Gameplay")]
		public IEnumerator LevelCompletesAtScoreThreshold() {

			CommandConsoleBroker.SendCommand(string.Format("SetScore {0}", GameController.LevelScoreThreshold + 5));
			yield return StartCoroutine(Q.driver.WaitFor(() => GameController.Self.GameLevelCompleteText.color.a > 0, "Level end text is visible.", 5f));
			yield return StartCoroutine(Q.driver.WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(GameController.Self.MainMenu), "Main menu re-appears after game completion.", 12f));

		}

		[Automation("Gameplay")]
		[DependencyWeb(1)] //Coupled with DependencyClass tag attribute above, makes this test the first test to run in the entire suite as long as a test run includes this test.
		public IEnumerator UserCanQuitAtAnyTimeUsingExitGameButton() {

			yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_exitGame, "Exit game button should not be visible before a game is started.", true));
			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_startGame, "Select New Game button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any(), "Equations appear in play area."));
			yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_exitGame, "Exit game button should be visible once a game is started."));
			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_exitGame, "Select Exit Game button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_startGame), "Start game button should be visible after quiting game."));

		}

		[TearDown]
		public IEnumerator TearDown() {

			//Reset Calculator value.
			yield return StartCoroutine(Q.driver.Try.Click(CalculatorTestObject.GetButton("C"), timeout: 2f));

		}

		[TearDownClass]
		public IEnumerator TearDownClass() {

			yield return null;

		}

	}

}