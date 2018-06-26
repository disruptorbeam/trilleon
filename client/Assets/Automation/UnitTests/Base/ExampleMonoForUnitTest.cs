using System.Collections;
using UnityEngine;

/// <summary>
/// Used to demonstrate how to use UnityTests to test private Methods and IEnumerators, along with asserting values of private variables in code outside of the Trilleon namespace (ex: In your game code).
/// </summary>
public class ExampleMonoForUnitTest : MonoBehaviour {

	string somePrivateString = string.Empty;
	bool somePrivateBool { get; set; }

	IEnumerator SetSomeValues(string valueToSet) {

		somePrivateString = valueToSet;
		somePrivateBool = true;
		Debug.Log(somePrivateString + somePrivateBool);
		yield return null;

	}

}
