using System;
using UnityEngine;

namespace TrilleonAutomation {

   /// <summary>
   /// Link a test to a dependency on a seperate device. Forces these tests to be run last, when a "buddy" device can be found and a relationship can be mapped between them.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class BuddySystem : Attribute {

      public Buddy buddy { get; private set; }
      public string ReactionOf { get; private set; }
    
      public BuddySystem(Buddy buddy) : this(buddy, string.Empty) {}
      public BuddySystem(Buddy buddy, string ReactionOf) {
         
      	this.buddy = buddy;
         this.ReactionOf = ReactionOf;

      }

   }

}