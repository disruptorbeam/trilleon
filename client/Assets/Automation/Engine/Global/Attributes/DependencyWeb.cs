using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {
   
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
   public class DependencyWeb : Attribute {

      public List<string> Dependencies { 
         get { 
            return dependencies;
         }
      }
      List<string> dependencies = new List<string>();

      //Argument cannot be null or of 0 length. This is validated and enforced in AutomationMaster Validator method.
      public DependencyWeb(params string[] testMethods) {

         //Get the string name of all dependencies and store in list.
         for(int x = 0; x < testMethods.Length; x++) {
            
            dependencies.Add(testMethods[x]);

         }

      }

   }

}