using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[UnityTestClass]
	public class TrilleonFrameworkTests : UnityTestBase {

		[SetUpClass]
		public IEnumerator SetUpClass() {

			this.gameObject.AddComponent<ExampleMonoForUnitTest>();
			yield return null;

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator LaunchMonoFromGameCodeWorks() {

			string setString = "Set The String!";
			string somePrivateString = GetPrivateVariableValue<ExampleMonoForUnitTest, string>(this.gameObject, "somePrivateString");
			bool somePrivateBool = GetPrivateVariableValue<ExampleMonoForUnitTest, bool>(this.gameObject, "somePrivateBool");
			yield return StartCoroutine(U.assert.IsTrue(somePrivateString == string.Empty, string.Format("Expecting ExampleMonoForUnitTest.somePrivateString value to be empty/null. Returned \"{0}\"", string.IsNullOrEmpty(somePrivateString) ? "empty/null" : somePrivateString)));
			yield return StartCoroutine(U.assert.IsTrue(!somePrivateBool, string.Format("Expecting ExampleMonoForUnitTest.somePrivateBool value to be false. Returned \"{0}\"", somePrivateBool)));
			yield return StartCoroutine(LaunchPrivateCoroutine<ExampleMonoForUnitTest>(this.gameObject, "SetSomeValues", setString));
			somePrivateString = GetPrivateVariableValue<ExampleMonoForUnitTest, string>(this.gameObject, "somePrivateString");
			somePrivateBool = GetPrivateVariableValue<ExampleMonoForUnitTest, bool>(this.gameObject, "somePrivateBool");
			yield return StartCoroutine(U.assert.IsTrue(somePrivateString == setString, string.Format("Expecting ExampleMonoForUnitTest.somePrivateString value to be \"{0}\". Returned \"{1}\"", setString, somePrivateString)));
			yield return StartCoroutine(U.assert.IsTrue(somePrivateBool, string.Format("Expecting ExampleMonoForUnitTest.somePrivateBool value to be true. Returned \"{0}\"", somePrivateBool)));

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator HelpRandomStringReturnsAlphaNumericOnlyWhenExplicitlyRequested() {

			List<char> alphaNumerics = GetFieldValue<HelperFunctions,char[]>("alphaNumerics").ToList();
			List<char> nonAlphaNumerics = GetFieldValue<HelperFunctions,char[]>("nonAlphaNumerics").ToList();
			List<char> val = U.help.RandomString(10, true).ToCharArray().ToList();
			yield return StartCoroutine(U.assert.IsTrue(val.FindAll(x => alphaNumerics.Contains(x)).Any() && !val.FindAll(x => nonAlphaNumerics.Contains(x)).Any(), string.Format("U.driver.RandomString(10, true) should return a string without non-alphanumeric characters. Returned \"{0}\"", new string(val.ToArray()))));
			yield return StartCoroutine(U.assert.IsTrue(val.Count == 10, string.Format("U.driver.RandomString(10, true) should return a string exactly 10 characters in length. Returned \"{0}\"", new string(val.ToArray()))));

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator HelpRandomStringReturnsAlphaAndNonAlphaNumericByDefault() {

			List<char> alphaNumerics = GetFieldValue<HelperFunctions,char[]>("alphaNumerics").ToList();
			List<char> nonAlphaNumerics = GetFieldValue<HelperFunctions,char[]>("nonAlphaNumerics").ToList();
			List<char> val = U.help.RandomString(12).ToCharArray().ToList();
			yield return StartCoroutine(U.assert.IsTrue(val.FindAll(x => alphaNumerics.Contains(x)).Any() || val.FindAll(x => nonAlphaNumerics.Contains(x)).Any(), string.Format("U.driver.RandomString(12, true) should return a string that may contain alphanumeric characters. Returned \"{0}\"", new string(val.ToArray()))));
			yield return StartCoroutine(U.assert.IsTrue(val.Count == 12, string.Format("U.driver.RandomString(12) should return a string exactly 12 characters in length. Returned \"{0}\"", new string(val.ToArray()))));

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator HelpRandomStringPasswordEnforcesOneUpperOneLowerOneNumberAndOneSpecialCharacter() {

			List<char> nonAlphaNumerics = GetFieldValue<HelperFunctions,char[]>("nonAlphaNumerics").ToList();
			List<char> val = U.help.RandomString(8, false, true).ToCharArray().ToList();
			yield return StartCoroutine(U.assert.IsTrue(val.FindAll(x => char.IsUpper(x)).Any() && val.FindAll(x => char.IsLower(x)).Any() && val.FindAll(x => char.IsDigit(x)).Any() && val.FindAll(x => nonAlphaNumerics.Contains(x)).Any(), string.Format("U.driver.RandomString(8, true) should return a string that is guaranteed to have at least one lowercase letter, one uppercase letter, one digit character, and one special character. Returned \"{0}\"", new string(val.ToArray()))));
			yield return StartCoroutine(U.assert.IsTrue(val.Count == 8, string.Format("U.driver.RandomString(8, false, true) should return a string exactly 8 characters in length. Returned \"{0}\"", new string(val.ToArray()))));

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator HelpRandomStringPasswordEnforcesOneUpperOneLowerOneNumberAndNoSpecialCharacterIfSpecialCharacterIsExplicitlyOmitted() {
			 
			List<char> nonAlphaNumerics = GetFieldValue<HelperFunctions,char[]>("nonAlphaNumerics").ToList();
			List<char> val = U.help.RandomString(11, true, true).ToCharArray().ToList();
			yield return StartCoroutine(U.assert.IsTrue(val.FindAll(x => char.IsUpper(x)).Any() && val.FindAll(x => char.IsLower(x)).Any() && val.FindAll(x => char.IsDigit(x)).Any() && !val.FindAll(x => nonAlphaNumerics.Contains(x)).Any(), string.Format("U.driver.RandomString(11, true) should return a string that is guaranteed to have at least one lowercase letter, one uppercase letter, one digit character, and NO special characters. Returned \"{0}\"", new string(val.ToArray()))));
			yield return StartCoroutine(U.assert.IsTrue(val.Count == 11, string.Format("U.driver.RandomString(11, true, true) should return a string exactly 11 characters in length. Returned \"{0}\"", new string(val.ToArray()))));

		}

		[UnityTest("Debug/Trilleon")]
		public IEnumerator HelpRandomStringPasswordEnforcesEightCharacterMinimumLength() {

			List<char> val = U.help.RandomString(7, false, true).ToCharArray().ToList();
			yield return StartCoroutine(U.assert.IsTrue(val.Count == 0, "U.driver.RandomString(7, false, true) should return a string 0 characters in length because the minimum length allowed for password generation is 8 characters."));

		}

		[TearDownClass]
		public IEnumerator TearDownClass() {

			Destroy(this.gameObject.GetComponent<ExampleMonoForUnitTest>());
			yield return null;

		}

	}

}
