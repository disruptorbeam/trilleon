using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[AutomationClass]
	public class ExampleTests : MonoBehaviour {

		//Please review the example project with extensive example automation located at the top level of the code repo!
		[SetUpClass]
		public IEnumerator SetUpClass() {

			yield return null;

		}

		[SetUp]
		public IEnumerator SetUp() {

			yield return null;

		}

		[Automation("Example")]
		public IEnumerator ExampleTest() {

			yield return StartCoroutine(Q.assert.IsTrue(true, "This will pass."));
			yield return StartCoroutine(Q.assert.IsTrue(false, "This will fail."));

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
