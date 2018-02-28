using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Map.Tiles.Objects;
using Core.Utility.GameEvents;
using Map.Input;

namespace TrilleonAutomation {

   /// <summary>
   /// Put game-specific driver code here!
   /// </summary>
	public class GameDriver : Driver {

   	public void SendKeys(TMP_InputField field, string keysToSend){

      	if(field != null && field.isActiveAndEnabled) {

         	Click(field.gameObject); //Send focus to input.
         	field.text = keysToSend;
         	Click(field.gameObject.transform.parent.gameObject); //Remove focus from input.

         } else {

         	Q.assert.Fail(string.Format("Cannot enter text in field {0}, as it is not available", field != null ? field.name : "[provided field is null]"));

         }

      }
         
   	public IEnumerator TapNode(GameObject node) {

      	if(node == null) {
         	Q.assert.Fail("Supplied game object for TapNode is null.");
         	yield break;
         }

      	MapTileNodeObject tileNode = node.GetComponent<MapTileNodeObject>();
      	Collider collider;
      	if(tileNode == null) {
            
         	collider = node.GetComponent<Collider>();
         	if (collider == null) {
               
            	BoxCollider boxCollider = node.GetComponent<BoxCollider>();
            	if (boxCollider == null) {
                  
               	Q.assert.Fail ("Supplied object lacks a collider and cannot be tapped.");
               	yield break;

               } else {
                  
               	StartCoroutine(Q.driver.Click(boxCollider.gameObject));
               }

            }

         } else {
            
         	collider = tileNode.TapCollider;

         }

      	IMapSelectableObject selectedObject = collider.gameObject.GetComponentInParent(typeof(IMapSelectableObject)) as IMapSelectableObject;
      	if (selectedObject != null) {
         	GameEventManager.Raise<MapTouchedEvent>(new MapTouchedEvent(selectedObject));
         } else if(collider != null) {
         	StartCoroutine(Q.driver.Click(collider.gameObject));
         }

      	yield return StartCoroutine(Q.driver.WaitRealTime(1));

      }

   }

}