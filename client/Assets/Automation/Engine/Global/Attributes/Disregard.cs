using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
   public class Disregard : Attribute {

      public List<DisregardSelection> DisregardSelections = new List<DisregardSelection>();
      public Disregard(params DisregardSelection[] disregardSelection){
         DisregardSelections.AddRange(disregardSelection.ToList());
      }

   }

}