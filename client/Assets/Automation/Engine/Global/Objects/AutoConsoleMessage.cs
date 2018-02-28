using UnityEngine;
using System;
using System.Collections;

namespace TrilleonAutomation {
   
   public class AutoConsoleMessage {
       
      public MessageLevel level;
      public ConsoleMessageType messageType;
      public DateTime timestamp;
      public string message = string.Empty;
      public string testMethod = string.Empty;

      public AutoConsoleMessage(string message,  MessageLevel level, ConsoleMessageType messageType, string testMethod = "") {
         
         this.message = message;
         this.level = level;
         this.messageType = messageType;
         this.testMethod = testMethod;
         timestamp = DateTime.UtcNow;

      }

   }

   public enum MessageLevel {
      Abridged,
      Verbose,
   }

}