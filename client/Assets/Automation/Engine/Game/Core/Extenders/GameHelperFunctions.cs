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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using VirtualList;

namespace TrilleonAutomation {

   public class GameHelperFunctions : HelperFunctions {

   	public IEnumerator TakePanelTestScreenshot(string name) {

         //Prepend "PT_STAGE_" for use in special html panel. Coincides with javascript variable "SCREENSHOT_STAGE_DELIMITER".
      	yield return StartCoroutine(Q.driver.TakeTestScreenshot(string.Format("PT_STAGE_{0}", name)));

      }

   	public IEnumerator DismissOpenPopups() {
         
      	List<GameObject> activeEscapeButtons = Q.driver.FindAll(By.Name, "BackButton").FindAll(x => Q.driver.IsActiveVisibleAndInteractable(x));
      	for (int e = 0; e < activeEscapeButtons.Count; e++) {
         	yield return StartCoroutine(Q.driver.Try.Click(activeEscapeButtons[e], 0.5f));
         }
      	if(Q.driver.IsActiveVisibleAndInteractable(ConfirmationPopupPanelTestObject.primary)) {
            yield return StartCoroutine(Q.driver.Try.Click(ConfirmationPopupPanelTestObject.button_normalConfirm, 0.5f));
            yield return StartCoroutine(Q.driver.Try.Click(ConfirmationPopupPanelTestObject.button_bigConfirm, 0.5f));
         }

      }

   	public IEnumerator ClickThroughAllNarrativeContent() {
         
         //If the previous test failed on a narrative sequence, close all narrative popups.
      	int maxTries = 25;
      	while(maxTries >= 0 && (Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_continueNarrative) || Q.driver.IsActiveVisibleAndInteractable(StoryFlowPanelTestObject.button_continueNarrative))) {
         	yield return StartCoroutine(Q.driver.Try.Click(HudPanelTestObject.button_continueNarrative, 0.5f));
         	yield return StartCoroutine(Q.driver.Try.Click(StoryFlowPanelTestObject.button_continueNarrative, 0.5f));
         	maxTries--;
         }
      	if (maxTries < 0) {
         	Q.assert.Fail("Automation was unable to dismiss narrative panels. The game object being clicked may not have a button script attached.");
         }

      }

   	public void GoToCurrentPlayersBase() {
      	MapCamera.Instance.GotoInstant(Character.Instance.worldTile);
      }

   	public void SimulateDrag(int xTile, int yTile) {
      	WorldTile tileToGoTo = new WorldTile(xTile, yTile);
         Q.driver.FindAll(By.ContainsComponent, "MapCamera").First().GetComponent<MapCamera>().GotoInstant(tileToGoTo);
      }

   	public IEnumerator FillSurvivorPartySlotsToMax() {

      	int maxTries = 10;
      	int currentTries = 0;
      	bool anyToFill = true;
      	while(anyToFill) {
         	for (int x = 0; x < RaidingPartyPanelTestObject.buttons_survivorSlots.Count; x++) {
            	GameObject empty = Q.driver.FindIn(RaidingPartyPanelTestObject.buttons_survivorSlots[x], By.Name, "empty_group");
            	if (empty == null) {
               	Q.assert.Fail("Could not find the required empty group object under crew slot object.");
               }
            	anyToFill = empty.activeSelf;
            	if (anyToFill) {
               	break;
               }
            } 
         	List<GameObject> currentChoices = RaidingPartyPanelTestObject.buttons_survivorChoices;
            if (!anyToFill || !currentChoices.Any() || currentTries > maxTries) {
            	break;
            }
            yield return StartCoroutine(Q.driver.Click(currentChoices.First(), "Could not add survivor to empty node"));
         	currentTries++;
         }
      	yield return null;

      }

   	public IEnumerator OpenMapViewIfNotAlreadyOpen() {

      	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_map)) {
         	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_map, "Could not select visible Map button on HUD footer."));
         }

      	yield return null;

      }

   	public IEnumerator OpenBlueprintBaseViewIfNotAlreadyOpen() {

      	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_base)) {
         	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_base, "Could not select visible Base button on HUD footer."));
         }

      	yield return null;

      }

   	public bool CurrentObjectiveComplete { get; set; }
   	public IEnumerator SelectNextObjective(string context, string assertObjTextAllLowercase = "", bool claim = false, bool dismissTriggeredNarrative = true) {

         yield return StartCoroutine(Q.driver.WaitRealTime(1.5f));

      	if(claim) {

         	int timeout = 15; 
         	int timer = 0;
         	bool notSuccessful = true;
         	while(notSuccessful && timer <= timeout) {
            	if(!Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.gui_narrativeObjectiveDescription)) {
               	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_narrativeObjective, string.Format("Could not select narrative objective icon button to '{0}'.", context)));
               }
            	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_objectiveGoTo)) {
               	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_objectiveGoTo, string.Format("Could not activate Go To button for '{0}' from Objective menu.", context)));
               }
            	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_objectiveClaim)) {
               	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_objectiveClaim, string.Format("Could not select narrative objective claim button for objective '{0}'.", context)));
               	yield return StartCoroutine(Q.driver.WaitRealTime(2.5f));
                  yield return StartCoroutine(Q.driver.Try.Click(RewardsPanelTestObject.button_claim, 0.5f));
                  yield return StartCoroutine(Q.driver.Try.Click(RewardsPanelTestObject.button_confirm, 0.5f));

               	notSuccessful = false;
               	CurrentObjectiveComplete = true; 
               	yield return StartCoroutine(Q.driver.WaitRealTime(2));
               }
            	timer += 1;
            	yield return StartCoroutine(Q.driver.WaitRealTime(1));

            }

         } else {

         	yield return StartCoroutine(Q.driver.WaitRealTime(1));
         	if(!Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.gui_narrativeObjectiveDescription)) {
            	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_narrativeObjective, string.Format("Could not select narrative objective icon button to '{0}'.", context)));
            }
         	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_objectiveClaim)) {
            	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_objectiveClaim, string.Format("Could not select narrative objective claim button for objective '{0}'.", context)));
            	yield return StartCoroutine(Q.driver.WaitRealTime(2.5f));
            	yield return StartCoroutine(Q.driver.Try.Click(RewardsPanelTestObject.button_confirm, 1));
            	if(!Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.gui_narrativeObjectiveDescription)) {
               	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_narrativeObjective, string.Format("Could not select narrative objective icon button to '{0}'.", context)));
               }
            }
           
            yield return StartCoroutine(Q.driver.WaitRealTime(1));
         	if(!string.IsNullOrEmpty(assertObjTextAllLowercase)) {
            	Q.assert.IsTrue(HudPanelTestObject.gui_narrativeObjectiveDescription.text.ToLower().Replace("\n", " ").Contains(assertObjTextAllLowercase), string.Format("Player is not instructed to '{0}' for their next objective.", assertObjTextAllLowercase));
            } 
         	if(Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.button_objectiveClaim)) {
            	Q.assert.Fail("Expected Go To button to be visible in Objective float text box. However, the Claim button is visible instead.");
            } else {
            	if(!Q.driver.IsActiveVisibleAndInteractable(HudPanelTestObject.gui_narrativeObjectiveDescription)) {
               	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_narrativeObjective, string.Format("Could not select narrative objective icon button to '{0}'.", context)));
               }
            	yield return StartCoroutine(Q.driver.Click(HudPanelTestObject.button_objectiveGoTo, string.Format("Could not activate Go To button for '{0}' from Objective menu.", context)));
            }
         	if(dismissTriggeredNarrative) {
            	yield return StartCoroutine(Q.help.ClickThroughAllNarrativeContent());
            }
         	yield return StartCoroutine(Q.driver.WaitRealTime(1));

         }

      	yield return StartCoroutine (Q.driver.WaitRealTime(2f));

      }

   	public IEnumerator ForceObjectiveStatusUpdate() {

      	yield return StartCoroutine(Q.driver.WaitRealTime(2));
      	ObjectiveMessages.GetObjectiveChainStatus(null,null); //This will force the Objective to update when called.
      	yield return StartCoroutine(Q.driver.WaitRealTime(2));

      }

   }

}
