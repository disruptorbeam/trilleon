/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
