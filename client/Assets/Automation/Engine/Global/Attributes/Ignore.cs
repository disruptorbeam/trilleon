using System;

namespace TrilleonAutomation {
   
   /// <summary>
   /// Apply to any AutomationClass or Automation test method that should be ignored by the AutomationMaster when running tests.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
   public class Ignore : Attribute {

      public string IgnoredBecauseReason { get; private set; }

      public Ignore(string reasonForIgnore) {
         
         IgnoredBecauseReason = reasonForIgnore;

      }

   }

}