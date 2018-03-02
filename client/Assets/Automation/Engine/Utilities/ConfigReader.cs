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
+*/

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrilleonAutomation {

	public class ConfigReader {

		private static string _config;

		private static List<KeyValuePair<string, string>> _requiredConfigs = new List<KeyValuePair<string, string>>();
		private static List<KeyValuePair<string, string>> _customConfigs = new List<KeyValuePair<string, string>>();
		private static bool isRequiredSection;

		private static void Set() {

			_config = FileBroker.GetTextResource(FileResource.TrilleonConfig);
			string[] rawKeys = _config.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
			_requiredConfigs = new List<KeyValuePair<string, string>>();
			_customConfigs = new List<KeyValuePair<string, string>>();
			isRequiredSection = true;

			for(int i = 0; i < rawKeys.Length; i++) {

				if(i != 0) {

					if(rawKeys[i].StartsWith("**")) {
						
						isRequiredSection = false;
						continue;

					}

					string[] thisKey = rawKeys[i].Split('=');

					if(isRequiredSection) {

						_requiredConfigs.Add(new KeyValuePair<string, string>(thisKey[0], thisKey[1]));

					} else {

						_customConfigs.Add(new KeyValuePair<string, string>(thisKey[0], thisKey[1]));

					}

				}

			}

		}

		public static void Refresh() {

			Set();

		}

		public static bool GetBool(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return bool.Parse(GetValueStringRaw(key));

		}

		public static int GetInt(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return int.Parse(GetValueStringRaw(key));

		}

		public static string GetString(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return GetValueStringRaw(key);

		}

		public static List<string> GetStringList(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}
			return GetValueStringRaw(key).Split(',').ToList();

		}

		private static string GetValueStringRaw(string key) {

			KeyValuePair<string, string> result = _requiredConfigs.Find(x => {

				if(x.Key.StartsWith("!")) {
					
					return x.Key.EndsWith(key);

				} else {
					
					return x.Key == key;

				}

			});

			if(!string.IsNullOrEmpty(result.Value)) {
				
				return result.Value;

			}

			result = _customConfigs.Find(x => {

				return x.Key == key;

			});

			if(!string.IsNullOrEmpty(result.Value)) {
				
				return result.Value;

			}

			return string.Empty;

		}

	}

}