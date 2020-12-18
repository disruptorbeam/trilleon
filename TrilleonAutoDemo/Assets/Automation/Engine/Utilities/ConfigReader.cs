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

using System.Text;
using System.Collections.Generic;

namespace TrilleonAutomation {

    public class ConfigReader {

        public FileResource ResourceOrigin { get; private set; }
        public bool IsInternal { get; private set; }

		List<KeyValuePair<string, string>> _requiredConfigs = new List<KeyValuePair<string, string>>();
		List<KeyValuePair<string, string>> _customConfigs = new List<KeyValuePair<string, string>>();
        bool _isRequiredSection;
        string _config;

        public ConfigReader() : this(FileResource.TrilleonConfig, true) { }
        public ConfigReader(FileResource resource, bool isInterla) {

            ResourceOrigin = resource;
            IsInternal = isInterla;

        }

		private void Set() {
            
            _config = IsInternal ? FileBroker.GetTextResource(ResourceOrigin) : FileBroker.GetNonUnityTextResource(ResourceOrigin);
			string[] rawKeys = _config.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
			_requiredConfigs = new List<KeyValuePair<string, string>>();
			_customConfigs = new List<KeyValuePair<string, string>>();
			_isRequiredSection = true;

			for(int i = 0; i < rawKeys.Length; i++) {

                if(rawKeys[i].StartsWith("**")) {

                    if (i != 0) {
                        
                        _isRequiredSection = false;

                    }
					continue;

				}

				string[] thisKey = rawKeys[i].Split('=');

				if(_isRequiredSection) {

					_requiredConfigs.Add(new KeyValuePair<string, string>(thisKey[0], thisKey[1]));

				} else {

					_customConfigs.Add(new KeyValuePair<string, string>(thisKey[0], thisKey[1]));

				}

			}

		}

		public void Refresh() {

			Set();

		}

        public void SaveUpdates() {

            if(ResourceOrigin == FileResource.TrilleonConfig) {

                throw new UnityEngine.UnityException("Cannot save to TrilleonConfig from ConfigReader at this time. Not implemented.");

            }

            StringBuilder configRaw = new StringBuilder();
            for(int i = 0; i < _customConfigs.Count; i++) {

                configRaw.AppendLine(string.Format("{0}={1}", _customConfigs[i].Key, _customConfigs[i].Value));

            }
            for(int i = 0; i < _requiredConfigs.Count; i++) {

                configRaw.AppendLine(string.Format("{0}={1}", _requiredConfigs[i].Key, _requiredConfigs[i].Value));

            }
            FileBroker.SaveNonUnityTextResource(ResourceOrigin, configRaw.ToString());

        }

        public bool Exists(string key) {

            return _requiredConfigs.FindAll(x => x.Key == key).Any() || _customConfigs.FindAll(x => x.Key == key).Any();

        }

        public void AddKey(string key, string newValue) {

            if(Exists(key)) {

                UpdateKey(key, newValue);

            } else {

                _customConfigs.Add(new KeyValuePair<string, string>(key, newValue));

            }

        }

        public void UpdateKey(string key, string newValue) {
           
            if(_requiredConfigs.FindAll(x => x.Key == key).Any()) {

                _requiredConfigs.RemoveAt(_requiredConfigs.FindIndex(x => x.Key == key));
                _requiredConfigs.Add(new KeyValuePair<string,string>(key, newValue));

            } else if(_customConfigs.FindAll(x => x.Key == key).Any()) {

                _customConfigs.RemoveAt(_customConfigs.FindIndex(x => x.Key == key));
                _customConfigs.Add(new KeyValuePair<string, string>(key, newValue));

            } else {

                AutoConsole.PostMessage(string.Format("ConfigReader.UpdateKey() called for key \"{0}\" with value \"{1}\", but key does not exist in the \"{2}\" config!", key, newValue, System.Enum.GetName(typeof(FileResource), ResourceOrigin)), MessageLevel.Abridged);

            }

        }

		public bool GetBool(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return bool.Parse(GetValueStringRaw(key));

		}

		public int GetInt(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return int.Parse(GetValueStringRaw(key));

		}

		public string GetString(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}

			return GetValueStringRaw(key);

		}

		public List<string> GetStringList(string key) {

			if(!_requiredConfigs.Any()) {
				
				Set();

			}
			return GetValueStringRaw(key).Split(',').ToList();

		}

		private string GetValueStringRaw(string key) {

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
