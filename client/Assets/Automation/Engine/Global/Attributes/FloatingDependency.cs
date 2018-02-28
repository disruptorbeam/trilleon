using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {

   /// <summary>
   /// Used to declare another test method as being necessary in a test run.
   /// However, this marked test method does not need to be run before the current test,
   /// and can be requested at any point (or not at all) in the current test.
   /// NOTE: While this functionality could easily be accomplished without the use of an
   /// attribute, there is a need for validation, which is easily accomplished this way.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method)]
	public class FloatingDependency : Attribute {

      public List<string> Dependencies { 
         get { 
            return dependencies;
         }
      }
      List<string> dependencies = new List<string>();

      //Argument cannot be null or of 0 length. This is validated and enforced in AutomationMaster Validator method.
   	public FloatingDependency(params string[] testMethods) {

         //Get the string name of all dependencies and store in list.
      	for(int x = 0; x < testMethods.Length; x++) {
            
            dependencies.Add(testMethods[x]);

         }

      }

   }

}