using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {
   
   public class BuddyCommands : MonoBehaviour {

      private List<KeyValuePair<string, IEnumerator>> commands = new List<KeyValuePair <string, IEnumerator>>();

      //Declare all known commands.
      void Start() {
         SetCommands();
      }

      void SetCommands(){
         //commands.Add(new KeyValuePair<string, IEnumerator>("LEAVE_ANY_ENROLLED_FLEET", LeaveAnyEnrolledFleet()));
      }
      
      public IEnumerator HandleCommand(List<string> commands) {

         for(int l = 0; l < commands.Count; l++) {
            yield return StartCoroutine(HandleCommand(commands[l]));
         }

         yield return null;

      }

      public IEnumerator HandleCommand(string command) {

         if(!commands.Any()) {
            SetCommands();
         }

         List<KeyValuePair<string, IEnumerator>> match = commands.FindAll(x => x.Key == command);
         if(match.Any()) {
            yield return StartCoroutine(match.First().Value);
         } else {
            //TODO: Auto fail test if its pre-run commands cannot be accomodated.
         }

         yield return null;

      }


   }

}