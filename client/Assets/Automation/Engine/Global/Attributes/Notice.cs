using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
   public class Notice : Attribute {

      public List<string> Notices { 
         get { 
            return notices;
         }
      }
      List<string> notices = new List<string>();

      public Notice(string Notice, NoticeType NoticeType = NoticeType.Message) {

         notices.Add(string.Format("*{0}* {1}", NoticeType == NoticeType.Message ? "NOTICE" : "TEST INCOMPLETE", Notice));

      }

   }

   public enum NoticeType {
      Message, //Regular Notice. Simply prepended to details for this test in test results.
      IncompleteTest //Is test only partially complete, but there is value in having it run while incomplete. This will notify users that interpret the results.
   }

}