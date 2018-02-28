using System;

namespace TrilleonAutomation {
   
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
   public class Automation : Attribute {
      
      public string CategoryName { get; private set; }

      /// <summary>
      /// A name to categorize the tagged test. Enforces organization. Most, if not all, tests in the same class should share at least one category name.
      /// </summary>
      /// <param name="testCategoryName"> Category name of declared test. </param>
      public Automation(string CategoryName) {
         
         this.CategoryName = CategoryName;

      }
           
   }
}