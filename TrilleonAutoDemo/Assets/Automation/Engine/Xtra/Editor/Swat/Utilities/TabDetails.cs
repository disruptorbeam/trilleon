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
using UnityEngine;

namespace TrilleonAutomation {
   
   public enum TabSize { Small, Medium, Large }

   public class TabDetails {

      public TabDetails(SwatWindow parent, int priorityID) {

         Window = parent;
         Tabs = new List<SizedTab>();
         Shortcut = new List<KeyCode> { };
         PriorityID = priorityID;

      }

      /// <summary>
      /// Rank/Priority for tab visibility and order when being displayed.
      /// Rank Zero always appears last in the tab list and is never hidden. 
      /// It should be a tools window that allows access to hidden tabs.
      /// </summary>
      public int PriorityID { get; set; } 
      public bool OverrideScroll { get; set; } 
      public SwatWindow Window { get; set; }
      public List<SizedTab> Tabs { get; set; }
      public List<KeyCode> Shortcut { get; set; } //Default key is KeyCode.LeftShift. All added keys will need to be clicked in addition to this.
      public SizedTab Get(TabSize tab) {

         return Tabs.Find(x => x.Size == tab);

      }

   }

   public class SizedTab {

      public SizedTab(string tabText, TabSize size, Color textColor) : this(tabText, size, textColor, Swat.WindowDefaultTextSize) {}
      public SizedTab(string tabText, TabSize size, int fontSize) : this(tabText, size, Swat.WindowDefaultTextColor, fontSize) {}
      public SizedTab(string tabText, TabSize size) : this(tabText, size, Swat.WindowDefaultTextColor, Swat.WindowDefaultTextSize) {}
      public SizedTab(string tabText, TabSize size, Color textColor, int fontSize) {

         TabText = tabText;
         Size = size;
         TextColor = textColor;
         FontSize = fontSize;

      }

      public string TabText { get; set; }
      public TabSize Size { get; set; }
      public Color TextColor { get; set; }
      public int FontSize { get; set; }

   }

}
