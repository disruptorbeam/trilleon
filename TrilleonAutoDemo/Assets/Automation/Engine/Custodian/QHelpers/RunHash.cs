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
using System.Text;

namespace TrilleonAutomation {

   public class RunHash {
      
      /// Store key value pairs that persist across all tests in a test run.
      private List<KeyValuePair<string, string>> _cache = new List<KeyValuePair<string, string>>();
      private char[] reservedCharacters = { AutomationMaster.DELIMITER, ',' }; //Do not modify without considering usages of indices.

      /// <summary>
      /// Add item to cache. Throw exception if it already exists.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <param name="value">Value.</param>
      public void Add(string key, string value) {
         
         if(key.IndexOf(new string(reservedCharacters)) >= 0 && value.IndexOf(new string(reservedCharacters)) >= 0) {
            
            throw new UnityException(string.Format("Supplied value \"{0}\" contains reserved characters ({1}).", value, new string(reservedCharacters)));
         
         }

         if(!_cache.KeyValListContainsKey(key)) {
            
            //Add key/value.
            _cache.Add(new KeyValuePair<string,string>(key, value));

         } else {
            
            throw new UnityException(string.Format("The requested storage values ({0},{1}) already exist in storage.", key, value));
         
         }
      }

      /// <summary>
      /// Modify item in cache. Throw exception if it does not exist in cache.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <param name="value">Value.</param>
      public void Modify(string key, string value) {
         
         if(key.IndexOf(new string(reservedCharacters)) >= 0 && value.IndexOf(new string(reservedCharacters)) >= 0) {
            
            throw new UnityException(string.Format("Supplied value \"{0}\" contains reserved characters ({1}).", value, new string(reservedCharacters)));
         
         }

         if(!_cache.KeyValListContainsKey(key)) {
            
            throw new UnityException(string.Format("The requested storage values ({0},{1}) cannot be modified as they do not exist in storage.", key, value));
         
         } else {
            
            //"Edit" key/value.
            Remove(key);
            Add(key, value);

         }

      }

      /// <summary>
      /// Add item to cache if it does not exist, or modify the existing item if it already exists.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <param name="value">Value.</param>
      public void AddOrModify(string key, string value) {
         
         if(key.IndexOf(new string(reservedCharacters)) >= 0 && value.IndexOf(new string(reservedCharacters)) >= 0) {
            
            throw new UnityException(string.Format("Supplied value \"{0}\" contains reserved characters ({1}).", value, new string(reservedCharacters)));
         
         }

         if(!_cache.KeyValListContainsKey(key)) {
            
            //Add key/value.
            _cache.Add(new KeyValuePair<string,string>(key, value));

         } else {
            
            //"Edit" key/value.
            Remove(key);
            Add(key, value);

         }

      }

      /// <summary>
      /// Return the value from a provided key. Throw exception if the key does not exist in the cache and if calling method does not explicitly allow this iperation to fail.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <param name="isTry">If set to <c>true</c> is try.</param>
      public string Get(string key, bool isTry = true) {
         
         if(_cache.KeyValListContainsKey(key)) {
            
            return _cache.Find(x => x.Key == key).Value;

         } else if(!isTry) {
            
			Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Key \"{0}\" does not exist in cache. Cannot retrieve a value.", key)));

         }

         return string.Empty;

      }

      /// <summary>
      /// Remove the value from the cache for the provided key. Throw exception if the key does not exist in the cache and if calling method does not explicitly allow this iperation to fail.
      /// </summary>
      /// <param name="key">Key.</param>
      /// <param name="isTry">If set to <c>true</c> is try.</param>
      public void Remove(string key, bool isTry = true) {
         
         if(_cache.KeyValListContainsKey(key)) {
            
            _cache.Remove(_cache.Find(x => x.Key == key));

         } else if(!isTry) {
            
			Q.assert.StartCoroutine(Q.assert.Fail(string.Format("Key \"{0}\" does not exist in cache. Cannot complete remove command.", key)));

         }

      }

      /// <summary>
      /// Get all has key/values as a single delimited string.
      /// </summary>
      /// <returns>The all.</returns>
      public string GetAll() {
         
         StringBuilder hash = new StringBuilder();
         for(int i = 0; i < _cache.Count; i++) {
            
            hash.Append(string.Format("{0}{1},{2}{0}", AutomationMaster.DELIMITER, _cache[i].Key, _cache[i].Value));
         
         }

         return hash.ToString();

      }

      /// <summary>
      /// Sets cache to values stored in delimited string.
      /// </summary>
      /// <param name="hash">Hash.</param>
      public void SetAll(string hash) {
         
         string[] rawEach = hash.Split(reservedCharacters[0]);
         _cache = new List<KeyValuePair<string,string>>();

         for(int i = 0; i < rawEach.Length; i++) {

            string[] keyValPair = rawEach[i].Split(reservedCharacters[1]);
            _cache.Add(new KeyValuePair<string,string>(keyValPair[0], keyValPair[1]));

         }
      
      }
         
   }

}
