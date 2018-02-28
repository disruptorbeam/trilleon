using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class TestRails : Attribute {

      public int RunId { get; private set; }

      public TestRails(int RunId) {
         
         this.RunId = RunId;

      }

   }

}