/* 
+   This file is part of Trilleon.  Trilleon is a client automation framework.
+  
+   Copyright (C) 2017 Disruptor Beam
+  
+   Trilleon is free software: you can redistribute it and/or modify
+   it under the terms of the GNU Lesser General Public License as published by
+   the Free Software Foundation, either version 3 of the License, or
+   (at your option) any later version.
+
+   This program is distributed in the hope that it will be useful,
+   but WITHOUT ANY WARRANTY; without even the implied warranty of
+   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
+   GNU Lesser General Public License for more details.
+
+   You should have received a copy of the GNU Lesser General Public License
+   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max) {

            lock(syncLock) {

                return random.Next(min, max);

            }

        }

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

        public bool IsObjectVisibleByCameraUsingMeshRenderer(GameObject obj) {

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if(renderer == null) {

                Q.assert.StartCoroutine(Q.assert.Fail("Expected supplied object to have a MeshRenderer component to check if object is currently visible to camera."));
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

            return random.Next(0, 3);

        }

        /// <summary>
        /// Returns a random number between 0 and x (literal) for indexes. 
        /// Ex: supplying a min of 1 and a max of 3 will allow for 1, 2, or 3 to be returned.
        /// </summary>
        /// <returns> A random number between 0 and x. </returns>
        public int RandomIndex(int max) {

            return random.Next(0, max + 1);

        }

        public int RandomVal(int min, int max) {

            return random.Next(min, max + 1);

        }

        public string LoremIpsum(int length) {

            const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            int ipsumLength = loremIpsum.Length;
            int currentLength = 0;
            StringBuilder finalIpsum = new StringBuilder(); ;
            while(finalIpsum.Length < length) {

                int remainingLength = length - currentLength;
                if(remainingLength > ipsumLength) {

                    finalIpsum.AppendLine(loremIpsum);
                    currentLength += ipsumLength;

                } else {

                    finalIpsum.AppendLine(loremIpsum.Substring(0, remainingLength));
                    currentLength = 0; //Reset to beginning of Lorem Ipsum sample, and begin appending same text until requested length is satisfied.

                }

            }
            return finalIpsum.ToString();

        }

        char[] alphaNumerics = { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        char[] nonAlphaNumerics = { '@', '#', '^', '&', '(', ')', '+' };
        public string RandomString(int length, bool alphaNumericOnly = false, bool isPassword = false) {

            List<char> useChars = new List<char>();
            if(!alphaNumericOnly) {

                useChars.AddRange(nonAlphaNumerics);

            }
            useChars.AddRange(alphaNumerics);
            StringBuilder randomString = new StringBuilder();

            if(isPassword) {

                if(length < 8) {

                    if(!AutomationMaster.UnitTestMode) {

                        Q.assert.StartCoroutine(Q.assert.Fail("Passwords must be at least 8 characters long!"));

                    }
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

                        randomString.Append(nonAlphaNumerics[random.Next(0, nonAlphaNumerics.Length - 1)]);
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

        /// <summary>
        /// Generate a valid credit card number based on provider using Luhn's (Mod10) formula.
        /// </summary>
        /// <returns>The random valid credit card number.</returns>
        public string GetRandomValidCreditCardNumber(CreditCard provider) {

            bool isValid = false;
            string fakeCreditCardNumber = "";
            while(!isValid) {

                System.Random r = new System.Random();
                fakeCreditCardNumber = provider.Prefix.ToString();

                //Build out randomized number that is 16 digits long.
                while(fakeCreditCardNumber.Length < 16) {

                    fakeCreditCardNumber += r.Next(0, 10);

                }

                List<char> ccChar = fakeCreditCardNumber.ToCharArray().ToList();
                List<char> reversedNumber = ccChar;
                reversedNumber.Reverse();
                char check_digit = reversedNumber.First();
                reversedNumber.RemoveAt(0);

                var sum = 0;
                for(var x = 0; x < reversedNumber.Count; x++) {

                    var current_sum = 0;
                    //Double every other value.
                    if(x % 2 == 0) {

                        var doubled_sum = reversedNumber[x].ToString().ToInt() * 2;
                        if(doubled_sum >= 10) {

                            current_sum = doubled_sum.ToString()[0].ToString().ToInt() + doubled_sum.ToString()[1].ToString().ToInt();

                        } else {

                            current_sum = doubled_sum;

                        }

                    } else {

                        current_sum = reversedNumber[x].ToString().ToInt();

                    }

                    sum += current_sum;

                }

                string new_sum = (sum * 9).ToString();
                if(new_sum[new_sum.Length - 1].ToString().ToInt() == check_digit.ToString().ToInt()) {

                    isValid = true;
                    break;

                }

                fakeCreditCardNumber = "";

            }

            return fakeCreditCardNumber;

        }

    }

}

public class CreditCard {

    public CreditCard(Cards card) {

        Cards choice = card;
        if(card == Cards.Random) {

            int r = new System.Random().Next(0, 4);
            switch(r) {
                case 0:
                    choice = Cards.Visa;
                    break;
                case 1:
                    choice = Cards.MasterCard;
                    break;
                case 2:
                    choice = Cards.Discover;
                    break;
                case 3:
                    choice = Cards.Amex;
                    break;
            }

        }
        switch(card) {
            case Cards.Visa:
                Prefix = 4;
                break;
            case Cards.MasterCard:
                Prefix = 50;
                break;
            case Cards.Discover:
                Prefix = 65;
                break;
            case Cards.Amex:
                Prefix = 34;
                break;
        }

    }
    public int Prefix { get; set; }
    public enum Cards { Visa, MasterCard, Discover, Amex, Random }

}
