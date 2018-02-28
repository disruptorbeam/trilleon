using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TrilleonAutomation {

	public class HelperFunctions : MonoBehaviour {

		public IEnumerator RunFloatingDependency(string dependencyName) {

			yield return StartCoroutine(AutomationMaster.StaticSelfComponent.RunFloatingDependency(dependencyName));
		}

		public GameObject RetrieveFirstParentThatHasButtonScript(GameObject child) {

			GameObject obj = child;

			while(true) {

				if(obj.GetComponent<Button>() != null) {
					return obj;
				}

				if(obj.transform != null && obj.transform.parent != null) {
					obj = obj.transform.parent.gameObject;
				} else {
					break;
				}

			}

			return null;

		}

		public GameObject GetDirectParentWithMatchingName(GameObject obj, string parentName) {

			if (obj == null) {
				return null;
			}
			GameObject match = null;
			GameObject currentObjectToInspect = obj.transform.parent != null ? obj.transform.parent.gameObject : null;
			while (match == null && currentObjectToInspect != null) {
				if (currentObjectToInspect.name == parentName) {
					match = currentObjectToInspect;
					break;
				} else {
					currentObjectToInspect = currentObjectToInspect.transform.parent != null ? currentObjectToInspect.transform.parent.gameObject : null;
				}
			}
			return match;

		}

		public bool IsObjectVisibleByCameraUsingMeshRenderer(GameObject obj) {

			MeshRenderer renderer = obj.GetComponent<MeshRenderer> ();
			if (renderer == null) {
				Q.assert.Fail("Expected supplied object to have a MeshRenderer component to check if object is currently visible to camera.");
				return false;
			}
			return renderer.isVisible;

		}

		public static bool TextContainsOrEquals(string textActual, string textExpected) {
			return textActual.Contains(textExpected) || textActual == textExpected;
		}

		/// <summary>
		/// Returns a random number between 0 and 2 for indexes which constitute a common number of choice options.
		/// </summary>
		/// <returns> Random number betweeen 0 and 2. </returns>
		public int RandomSmallIndex() {
			
			Random random = new Random();
			return random.Next(0, 3);

		}

		/// <summary>
		/// Returns a random number between 0 and x (literal) for indexes. 
		/// Ex: supplying a min of 1 and a max of 3 will allow for 1, 2, or 3 to be returned.
		/// </summary>
		/// <returns> A random number between 0 and x. </returns>
		public int RandomIndex(int max) {
			
			Random random = new Random();
			return random.Next(0, max+1);

		}

		public int RandomVal(int min, int max) {
			
			Random random = new Random();
			return random.Next(min, max+1);

		}

		private Random random = new Random();
		public string LoremIpsum(int length) {
			const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
			int ipsumLength = loremIpsum.Length;
			int currentLength = 0;
			StringBuilder finalIpsum = new StringBuilder();;
			while(currentLength < length) {
				int remainingLength = length - currentLength;
				if(remainingLength > ipsumLength) {
					finalIpsum.AppendLine(loremIpsum);
					currentLength += ipsumLength;
				} else {
					finalIpsum.AppendLine(loremIpsum.Substring(0, remainingLength));
					break;
				}
			}
			return finalIpsum.ToString();
		}

		char[] alphaNumerics = { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		char[] nonAplphaNumerics  = { '@', '#', '^', '&', '(', ')', '+' };
		public string RandomString(int length, bool alphaNumericOnly = false, bool isPassword = false) {

			List<char> useChars = new List<char>();
			if(!alphaNumericOnly) {
				
				useChars.AddRange(nonAplphaNumerics);

			}
			useChars.AddRange(alphaNumerics);
			StringBuilder randomString = new StringBuilder();

			if (isPassword) {

				if (length < 8) {

					Q.assert.Fail("Passwords must be at least 8 characters long!");
					return string.Empty;

				}
				//If this is a password, these must be satisfied.
				bool hasCapitalLetter = false;
				bool hasLowercaseLetter = false;
				bool hasNumber = false;
				bool hasSpecialChar = false;

				for(int c = 0; c < length; c++) {

					List<char> LimitedSet = new List<char>();
					if(!hasCapitalLetter) {

						LimitedSet = useChars.FindAll(x => char.IsUpper(x));
						randomString.Append(LimitedSet[random.Next(0, LimitedSet.Count - 1)]);
						hasCapitalLetter = true;

					} else if(!hasLowercaseLetter) {

						LimitedSet = useChars.FindAll(x => !char.IsUpper(x));
						randomString.Append(LimitedSet[random.Next(0, LimitedSet.Count - 1)]);
						hasLowercaseLetter = true;

					} else if(!hasNumber) {

						LimitedSet = useChars.FindAll(x => char.IsDigit(x));
						randomString.Append(LimitedSet[random.Next(0, LimitedSet.Count - 1)]);
						hasNumber = true;

					} else if(!hasSpecialChar && !alphaNumericOnly) {

						randomString.Append(nonAplphaNumerics[random.Next(0, nonAplphaNumerics.Length - 1)]);
						hasSpecialChar = true;

					} else {
						
						randomString.Append(useChars[random.Next(0, useChars.Count - 1)]);

					}

				}

			} else {

				for(int c = 0; c < length; c++) {

					randomString.Append(useChars[random.Next(0, useChars.Count - 1)]);

				}

			}

			return randomString.ToString();

		}

		/// <summary>
		/// Removes all non alpha numeric characters except for periods and dashes.
		/// </summary>
		/// <returns>The string as alpha numeric.</returns>
		/// <param name="rawString">Raw string.</param>
		public string ReturnStringAsAlphaNumericWithExceptions(string rawString) {

			char[] arr = rawString.ToCharArray();
			arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) 
				|| char.IsWhiteSpace(c) 
				|| c == '-'
				|| c == '.')));
			return new string(arr).Replace(" ", string.Empty);

		}

		/// <summary>
		/// Returns if supplied string is alpha numeric (with additional characters allowed).
		/// </summary>
		/// <returns>The string as alpha numeric.</returns>
		/// <param name="rawString">Raw string.</param>
		public bool IsAlphaNumeric(string rawString, params char[] specialCharactersAllowed) {

			char[] arr = rawString.ToCharArray();
			arr = Array.FindAll<char>(arr, (c => (!char.IsLetterOrDigit(c) 
				&& !specialCharactersAllowed.ToList().Contains(c))));
			return !arr.ToList().Any() && !string.IsNullOrEmpty(rawString);

		}

		/*	
		 	This is a modification of the port of of the Javascript credit card number generator now in C#
        	by Kev Hunter https://kevhunter.wordpress.com
        	Also edited from user Siaynoq.
        */
		public static string GetRandomValidCreditCardNumber() {

			bool isValid = false;
			string fakeCreditCardNumber = string.Empty;
			while(!isValid) {

				System.Random r = new System.Random();
				fakeCreditCardNumber = "4"; //Visa Credit Card must always start with the number 4.

				//Build out randomized number that is 16 digits long.
				while(fakeCreditCardNumber.Length < 16) {

					double rnd = (r.NextDouble() * 1.0f - 0f);
					fakeCreditCardNumber += Math.Floor(rnd * 10);

				}

				try {

					char[] ccChar = fakeCreditCardNumber.ToCharArray();
					Array.Reverse(ccChar);
					List<char> reversedNumber = ccChar.ToList();

					int mod10Count = 0;
					for (int i = 0; i < reversedNumber.Count; i++) {

						int augend = Convert.ToInt32(reversedNumber[i].ToString());

						if(((i + 1) % 2) == 0) {

							string productstring = (augend * 2).ToString ();
							augend = 0;
							for (int j = 0; j < productstring.Length; j++) {

								augend += Convert.ToInt32(productstring[j].ToString());

							}

						}

						mod10Count += augend;

					}

					if((mod10Count % 10) == 0) {

						isValid = true;
						break;

					}

				} catch{ }

				fakeCreditCardNumber = string.Empty;

			}

			return fakeCreditCardNumber;

		}
			
	}

}