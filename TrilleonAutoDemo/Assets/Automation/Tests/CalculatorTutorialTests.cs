using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[Deferr]
	[AutomationClass]
	[DependencyClass(1)] //Makes this class the same DependencyClass ID as CalculatorGameTests, and contains ID test 2, which will run second in a test run's execution.
	public class CalculatorTutorialTests : MonoBehaviour {

		[SetUpClass]
		public IEnumerator SetUpClass() {

			yield return null;

		}

		[SetUp]
		public IEnumerator SetUp() {

			yield return null;

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialCanBeInitializedFromMainMenu() {

			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_startTutorial, "Select Tutorial button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.gui_tutorialText.gameObject), "Tutorial text sequence begins."));

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialExampleEquationRegeneratedIfFirstNotAnswered() {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.equation_tutorial != null, "Tutorial text sequence begins that describes game concepts and regular equations.", 30f));
			int currentEquation = QwackulatorGameTestObject.equation_tutorial.Id;
			yield return StartCoroutine(Q.assert.IsTrue(!QwackulatorGameTestObject.equation_tutorial.IsDuck, "Equation should not be a Duck."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.equation_tutorial != null && currentEquation != QwackulatorGameTestObject.equation_tutorial.Id, "If the tutorial eqaution is not answered in time, it is destroyed, and a new one appears for the user.", 45f));

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialExampleEquationCanBeAnswered() {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.equation_tutorial != null, "Equation exists."));
			yield return StartCoroutine(CalculatorTestObject.steps.AnswerEquation(QwackulatorGameTestObject.equation_tutorial));

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialExampleDuckRegeneratedIfFirstNotSolved() {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.duck_tutorial != null, "Tutorial text sequence begins that describes Ducks.", 120f));
			int currentEquation = QwackulatorGameTestObject.duck_tutorial.Id;
			yield return StartCoroutine(Q.assert.IsTrue(QwackulatorGameTestObject.duck_tutorial.IsDuck, "Equation should be a Duck."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.duck_tutorial != null && currentEquation != QwackulatorGameTestObject.duck_tutorial.Id, "If the tutorial eqaution is not answered in time, it is destroyed, and a new one appears for the user.", 45f));

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialExampleDuckCanBeSolved() {

			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.duck_tutorial != null, "Duck equation exists.", 45f));
			yield return StartCoroutine(CalculatorTestObject.steps.AnswerEquation(QwackulatorGameTestObject.duck_tutorial));

		}

		[Automation("Tutorial")]
		public IEnumerator TutorialReturnsToMainMenuObCompletion() {

			yield return StartCoroutine(Q.driver.WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(GameController.Self.MainMenu), "Main menu re-appears after tutorial completion.", 25f));

		}

		[Automation("Tutorial")]
		[DependencyTest(2)] //DependencyTest ID 1 is located in CalculatorGameTests, and since this class shares the DependencyClass ID of 1, this test is tied to the shared order. Test ID 3 could be either in this class or that class, for example.
		public IEnumerator UserCanQuitTutorialAtAnyTimeUsingExitGameButton() {

			yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_exitGame, "Exit game button should not be visible before a game is started.", true));
			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_startTutorial, "Select New Game button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => QwackulatorGameTestObject.objs_activeEquations.Any(), "Equations appear in play area."));
			yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_exitGame, "Exit game button should be visible once a game is started."));
			yield return StartCoroutine(Q.driver.Click(QwackulatorGameTestObject.button_exitGame, "Select Exit Game button."));
			yield return StartCoroutine(Q.driver.WaitFor(() => Q.driver.IsActiveVisibleAndInteractable(QwackulatorGameTestObject.button_startGame), "Start game button should be visible after quiting game." ));

		}

		[TearDown]
		public IEnumerator TearDown() {

			yield return null;

		}

		[TearDownClass]
		public IEnumerator TearDownClass() {

			yield return null;

		}

	}

}