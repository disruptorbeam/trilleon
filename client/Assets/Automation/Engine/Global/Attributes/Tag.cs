using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   /// <summary>
   /// Apply to any AutomationClass or Automation test method that should display a notification (reminder? important note? etc) in automation report.
   /// Will appear in description of test, and will also be noted in Warnings section of report.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
   public class Tag : Attribute {

      public List<string> Notifications { 
         get { 
            return notifications;
         }
      }
      List<string> notifications = new List<string>();

      public Tag(params string[] notifications) {
         
         this.notifications.AddRange(notifications);

      }

   }

}