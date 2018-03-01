/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace TrilleonAutomation {

   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
   public class Notice : Attribute {

      public List<string> Notices { 
         get { 
            return notices;
         }
      }
      List<string> notices = new List<string>();

      public Notice(string Notice, NoticeType NoticeType = NoticeType.Message) {

         notices.Add(string.Format("*{0}* {1}", NoticeType == NoticeType.Message ? "NOTICE" : "TEST INCOMPLETE", Notice));

      }

   }

   public enum NoticeType {
      Message, //Regular Notice. Simply prepended to details for this test in test results.
      IncompleteTest //Is test only partially complete, but there is value in having it run while incomplete. This will notify users that interpret the results.
   }

}
