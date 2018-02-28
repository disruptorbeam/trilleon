using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   public class BuddyCommand : Attribute {

      public List<string> Commands { 
         get { 
            return commands;
         }
      }
      List<string> commands = new List<string>();

      public BuddyCommand(params string[] commands) {
         
         if(commands.Length > 0) {
            
            this.commands.AddRange(commands);

         }
      }

   }

}