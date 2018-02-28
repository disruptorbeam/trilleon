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
using UnityEditor;
using System.Collections.Generic;

namespace TrilleonAutomation  {
      
   public static class ReactGUI {

      #region Buttons

      //static List<KeyValuePair<string,bool>> Buttons = new List<KeyValuePair<string,bool>>();
      public static void DeclareButton(string ID, CSS Css) {

        

      }
      public static bool Button() {

         return false;

      }

      #endregion

   }

   public class CSS {

      public Dictionary<string,string> attributes = new Dictionary<string,string>();
      public void Add(string attribute, string value) {
         //As in real CSS, youngest declarations take precedence.
         if(attributes.ContainsKey(attribute)) {
            attributes.Remove(attribute);
         }
         attributes.Add(attribute, value);
      }
      /// <summary>
      /// Add a string attribute and value seperated by a colon, as in normal css.
      /// Example: "max-width:55px"
      /// </summary>
      /// <param name="attributeValues">Attribute values.</param>
      public void Add(params string[] attributeValues) {

         for(int c = 0; c < attributeValues.Length; c++) {

            string[] split = attributeValues[c].Split(':');
            string attr = split[0].Trim();
            string val = split[1].Trim();
            //As in real CSS, youngest declarations take precedence.
            if(attributes.ContainsKey(attr)) {
               attributes.Remove(attr);
            }
            attributes.Add(attr, val);

         }

      }

   }


   public enum ReactButton {

   }

}
