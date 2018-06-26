/* 
+   This file is part of Trilleon.  Trilleon is a client automation framework.
+  
+   Copyright (C) 2017 Disruptor Beam
+  
+   Trilleon is free software: you can redistribute it and/or modify
+   it under the terms of the GNU Lesser General Public License as published by
+   the Free Software Foundation, either version 3 of the License, or
+   (at your option) any later version.
+
+   This program is distributed in the hope that it will be useful,
+   but WITHOUT ANY WARRANTY; without even the implied warranty of
+   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
+   GNU Lesser General Public License for more details.
+
+   You should have received a copy of the GNU Lesser General Public License
+   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
