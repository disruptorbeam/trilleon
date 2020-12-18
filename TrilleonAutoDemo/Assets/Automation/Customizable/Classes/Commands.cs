using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace TrilleonAutomation {

	public class Commands : MonoBehaviour {

		Arbiter arbiter { 
			get{ return this.GetComponent<Arbiter>(); }
		}

		const string SOME_CURRENCY = "Gold";
		const string ANOTHER_CURRENCY = "Silver";
		static string[] ALL_FIELDS = { SOME_CURRENCY, ANOTHER_CURRENCY };

		/// <summary>
		/// Grant current account all requested values.
		/// </summary>
		/// <param name="grant">Grant.</param>
		public IEnumerator GrantPlayer(Grant grant) {

			for(int i = 0; i < ALL_FIELDS.Length; i++) {
				switch(ALL_FIELDS[i]) {
				case "Gold":
					if(grant.Gold >= 0) {
						AutoConsole.PostMessage(string.Format("Granting Gold: {0}", grant.Gold));
						yield return StartCoroutine(Grant("GRANT GOLD", grant.Gold));
						yield return StartCoroutine(Q.driver.WaitRealTime(4));
					}
					break;
				case "Silver":
					if(grant.Silver >= 0) {
						AutoConsole.PostMessage(string.Format("Granting Silver: {0}", grant.Silver));
						yield return StartCoroutine(Grant("GRANT SILVER", grant.Silver));
						yield return StartCoroutine(Q.driver.WaitRealTime(4));
					}
					break;
				default:
					throw new UnityException("Grant Command not recognized.");

				}

			}

		}

		public IEnumerator SkipTutorial() {

			//TODO: UnityConsoleBroker.SendCommand("TUTORIALSKIP");
			yield return StartCoroutine(Q.driver.WaitRealTime(5));

		}

		private IEnumerator Grant(string commandCall, int? amount) {

			yield return StartCoroutine(Grant(commandCall, amount.ToString()));

		}

		private IEnumerator Grant(string commandCall, string amount) {

			CommandConsoleBroker.SendCommand(string.Format("{0} {1}", commandCall, amount));
			yield return StartCoroutine(Q.driver.WaitRealTime(5));

		}

	}

}