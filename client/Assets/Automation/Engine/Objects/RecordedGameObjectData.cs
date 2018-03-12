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

﻿#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;

namespace TrilleonAutomation {
   
   public class RecordedGameObjectData {

      public int ID { get; set; }
      public ActableTypes Action { get; set; }
      public bool IsFloatingAssertion { get; set; } //This data will be default except for ID and Assertions list, where assertion is not tied to any specific recorded step.
      public bool IsTry { get; set; }
      public string Name { get; set; }
      public List<string> ParentNames = new List<string>();
      public string Tag { get; set; }
      public string TopLevelParentName { get; set; }
      public string TextValue { get; set; }
      public int RandomStringLength { get; set; }
      public bool RandomStringAllowSpecialCharacters { get; set; }
      public double Duration { get; set; }
      public float ScrollDistance { get; set; }
      public float InitialScrollPosition { get; set; } //Used to determine direction during updates.
      public ScrollDirection ScrollDirection { get; set; }
      public KeyCode KeyDown { get; set; }
      public int AsComponent { get; set; }
      public List<string> Components = new List<string>();
      public List<RecordingAssertionData> Assertions = new List<RecordingAssertionData>();

   }

   public class RecordingAssertionData {

      public RecordingAssertionData(int ID) {
         this.ID = ID;
      }

      public bool AssertionBeforeStep { get; set; }
      public int ID { get; set; }
      public AssertionType Type { get; set; }
      public string AssertionArgument { get; set; }
      public AssertionIsTrue AssertionIsTrue { get; set; }
      public string AssertionMessage { get; set; }
      public bool IsReverseCondition { get; set; }
      public FailureContext FailureContext { get; set; }
   
   }

   public enum AssertionType { IsTrue, IsInteractable, NotNull, FreeForm  }
   public enum AssertionIsTrue { TextEquals, TextContainsOrEquals }
   public enum ScrollDirection { LeftOrUpToRightOrDown, RightOrDownToLeftOrUp }

}
#endif
