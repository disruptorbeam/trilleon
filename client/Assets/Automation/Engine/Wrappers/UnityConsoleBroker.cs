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

using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {

   public static class UnityConsoleBroker  {

      /// <summary>
      /// Run cheat command which has a behavior dictated by a test case or test initialize/tear down.
      /// </summary>
      /// <param name="command">Command.</param>
      /// <param name="argsString">Arguments string with parameters seperated by spaces.</param>
      public static void SendCommand(string command, string argsString = "") {
         
         if(!string.IsNullOrEmpty(argsString)) {
            
            string[] args = argsString.Split(' ');
            Wenzil.Console.Console.ExecuteCommand(command, args);

         } else {
            
            Wenzil.Console.Console.ExecuteCommand(command);

         }
      }

   }

}
