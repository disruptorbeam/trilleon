using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
   public class DependencyTest : Attribute {

      public int order { get; private set; }

      public DependencyTest(int requestedOrder) {
         
         order = requestedOrder;

      }

   }

}