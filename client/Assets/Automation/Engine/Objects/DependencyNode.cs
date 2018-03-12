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

﻿using UnityEngine;
using System.Collections.Generic;

namespace TrilleonAutomation {
   
   public class DependencyNode {

      public DependencyNode() : this(string.Empty) {}

      public DependencyNode(string testName) {
         _TestName = testName;
      }

      public string TestName {
         get { return _TestName; }
         set { _TestName = value; }
      }
      private string _TestName = string.Empty;

      public List<KeyValuePair<DependencyNodeConnectionType,string>> Dependencies {
         get {  return _Dependencies; }
         set { _Dependencies = value; }
      }
      private List<KeyValuePair<DependencyNodeConnectionType,string>> _Dependencies = new List<KeyValuePair<DependencyNodeConnectionType,string>>();

      public void AddDependency(DependencyNodeConnectionType type, string DependencyName){
         _Dependencies.Add(new KeyValuePair<DependencyNodeConnectionType,string>(type, DependencyName));
      }
     
      public Rect rect {
         get { return _rect; }
         set { _rect = value; }
      }
      private Rect _rect = new Rect();

   }

}
