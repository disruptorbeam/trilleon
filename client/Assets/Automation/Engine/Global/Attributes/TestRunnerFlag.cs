using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
	public class TestRunnerFlag : Attribute {

      public List<TestFlag> Flags { 
         get { 
            return flags;
         }
      }
      List<TestFlag> flags = new List<TestFlag>();

      /// <summary>
      /// Adds special flags to an Automation test that cause important behavioral changes in the test runner (AutomationMaster).
      /// </summary>
      /// <param name="testCategoryName"> TestFlag to add. </param>
   	public TestRunnerFlag(params TestFlag[] Flags) {
         
         this.flags.AddRange(Flags.ToList());

      }

   }
}