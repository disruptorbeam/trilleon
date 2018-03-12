using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[AutomationClass]
	public class ExampleTests : MonoBehaviour {

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

			yield return null;

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