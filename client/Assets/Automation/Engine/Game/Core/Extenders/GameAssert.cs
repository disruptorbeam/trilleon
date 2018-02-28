using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

	public class GameAssert : Assert {

   	public GameObject AssertNoErrorPopups() {


      	if(!AutomationMaster.ErrorPopupDetected && (ErrorPanelTestObject.primary != null && Q.driver.IsActiveVisibleAndInteractable(ErrorPanelTestObject.button_close)) || Q.driver.IsActiveVisibleAndInteractable(ConfirmationPopupPanelTestObject.primary)) {

         	string error = string.Empty;
         	bool isUnhandledError = false;

            //Only assert fail if the message is not a friendly error popup.
         	if (ErrorPanelTestObject.primary != null) {
               
            	isUnhandledError = true;
            	error = string.Format ("An error panel appeared: {0}", ErrorPanelTestObject.string_message);

            } else if (ConfirmationPopupPanelTestObject.text_confirmText == "Purchase Failed") {
               
            	isUnhandledError = true;
            	error = string.Format ("An error panel appeared: {0}", ConfirmationPopupPanelTestObject.text_confirmText);

            }
               
         	if (isUnhandledError) {
               
            	if (!AutomationMaster.CurrentTestContext.IsInitialized) {
                  
               	Q.assert.Fail (error);

               }

            	AutomationMaster.ErrorDetected (error);
            	AutomationMaster.ErrorPopupDetected = true;
            	AutomationMaster.UnexpectedErrorOccurredDuringCurrentSession = true;

            }
         }

      	return null;

      }

   }

}