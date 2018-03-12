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
+*/

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

      void SetCommands() {
         
         //TODO: commands.Add(new KeyValuePair<string, IEnumerator>("EXAMPLE_COMMAND", RunThatExampleCommand()));

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

            AutoConsole.PostMessage(string.Format("Running Buddy command \"{0}\"", command), MessageLevel.Abridged);
            yield return StartCoroutine(match.First().Value);

         } else {

            AutoConsole.PostMessage(string.Format("Cannot find Buddy command \"{0}\"", command), MessageLevel.Abridged);
            //TODO: Auto fail test if its pre-run commands cannot be accomodated.

         }

         yield return null;

      }
			
   }

}