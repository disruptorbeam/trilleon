using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

	public class GameAssert : Assert {

		//TODO: EXTEND CODE?!

		public bool IgnoreErrorPopups { get; set; }
		public GameObject AssertNoErrorPopups() {

			// TODO: Set something in condition to detect if a game specific popup has risen.
			if(!IgnoreErrorPopups && !AutomationMaster.ErrorPopupDetected/* && Q.driver.IsActiveVisibleAndInteractable(ErrorPopupViewTestObject.primary) */) {

				/*
				//Handle expected usages of error popup.
				if(ErrorPopupViewTestObject.text_header.ContainsOrEquals("Insufficient Level")) {

					return null;

				}

				string error = string.Empty;
				bool isUnhandledError = false;

				isUnhandledError = true;
				//error = string.Format("An error panel appeared: {0}", ErrorPopupViewTestObject.text_error);

				//TODO: IS THIS PANEL ALSO USED FOR FRIENDLY "ERROR" MESSAGES?
				//Only assert fail if the message is not a friendly error popup.
				if(false) {

					//isUnhandledError = true;
					//error = string.Format("An error panel appeared: {0}", SOMETHING);

				} else if(false) {

					//isUnhandledError = true;
					//error = string.Format("An error panel appeared: {0}", SOMETHING);

				}

				if(isUnhandledError) {

					if(!AutomationMaster.CurrentTestContext.IsInitialized) {

						Q.assert.Fail(error);

					}

					AutomationMaster.ErrorDetected(error);
					AutomationMaster.ErrorPopupDetected = true;
					AutomationMaster.UnexpectedErrorOccurredDuringCurrentSession = true;

				}
				*/

			}
			return null;

		}

	}

}