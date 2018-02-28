using System;

namespace TrilleonAutomation {
   
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class SetUpClass : Attribute {

      //Should the Class SetUp be run again if its tests are deferred until the end of a run?
      public bool RunAgainForDeferredTests { get; private set; }

      public SetUpClass(bool RunAgainForDeferredTests = false) {

         this.RunAgainForDeferredTests = RunAgainForDeferredTests;

      }

   }

   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class SetUp : Attribute {}

}