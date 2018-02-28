using System;
using Debug = UnityEngine.Debug;

namespace TrilleonAutomation {

   //Run AFTER GlobalTearDown (defined in GameMaster) but BEFORE current test's TearDown (if one exists).
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class TearDownClass : Attribute {

      //Should the Class SetUp be run again if its tests are deferred until the end of a run?
      public bool RunAgainForDeferredTests { get; private set; }

      public TearDownClass(bool RunAgainForDeferredTests = false) {

         this.RunAgainForDeferredTests = RunAgainForDeferredTests;

      }

   }

   //Run AFTER GlobalTearDown (defined in GameMaster) but then AFTER current test's TearDownClass (if one exists).
   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class TearDown : Attribute {}

}