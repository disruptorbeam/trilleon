using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {

   public static class UnityConsoleBroker  {

      /// <summary>
      /// Run cheat command which has a behavior dictated by a test case or test initialize/tear down.
      /// </summary>
      /// <param name="command">Command.</param>
      /// <param name="argsString">Arguments string with parameters seperated by spaces.</param>
      public static void SendCommand(string command, string argsString = "") {
         
         if(!string.IsNullOrEmpty(argsString)) {
            
            string[] args = argsString.Split(' ');
            Wenzil.Console.Console.ExecuteCommand(command, args);

         } else {
            
            Wenzil.Console.Console.ExecuteCommand(command);

         }
      }

   }

}