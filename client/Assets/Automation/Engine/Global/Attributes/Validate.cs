using System;
using System.Collections;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
   public class Validate : Attribute {

      public List<KeyValuePair<Expect,string>> Expectations { 
         get { 
            return expectations;
         }
      }
      List<KeyValuePair<Expect,string>> expectations = new List<KeyValuePair<Expect,string>>();

      /// <summary>
      /// Used for Expectations that do not require additional information. Duplicate Expectations are ignored.
      /// </summary>
      /// <param name="Expectations">Expectations.</param>
      public Validate(params Expect[] Expectations) {

         for(int x = 0; x < Expectations.Length; x++) {
            
            this.expectations.Add(new KeyValuePair<Expect,string>(Expectations[x], string.Empty));

         }

      }

      /// <summary>
      /// Use for Expectations that require an accompanying argument value. Duplicate Expectations are ignored.
      /// </summary>
      /// <param name="Expectations">Expectations.</param>
      /// <param name="OrderExpectation">Order expectation.</param>
      public Validate(Expect Expectation, string Argument) {
         
         this.expectations.Add(new KeyValuePair<Expect,string>(Expectation, Argument));

      }

   }
}