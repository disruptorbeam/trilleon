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

using System.Collections.Generic;
using UnityEngine;

namespace TrilleonAutomation {

	public class BatchContext {

		public List<string> completedTests {
			get { return _completedTests; }
		}
		private List<string> _completedTests = new List<string>();

		public Passes passes = new Passes();
		public Ignored ignored = new Ignored();
		public Fails fails = new Fails();
		public Skipped skipped = new Skipped();
		public GameExceptions exceptions = new GameExceptions();

		public class Passes {

			public List<string> tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					tests.Add(test);
				}
			}

		}

		public class Skipped {

			public List<string> tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					tests.Add(test);
				}
			}

		}

		public class Fails {

			public List<KeyValuePair<string,string[]>> tests {
				get { return _tests; }
			}
			private List<KeyValuePair<string,string[]>> _tests = new List<KeyValuePair<string,string[]>>();

			public void Add(string test, string[] values) {
				if(!_tests.KeyValListContainsKey(test)) {
					tests.Add(new KeyValuePair<string,string[]>(test, values));
				}
			}

		}

		public class Ignored {

			public List<string> tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					tests.Add(test);
					string message = string.Format("[{0}] {1}", test, "#Ignore#");
					AutoConsole.PostMessage(message);
				}
			}

		}

		public class GameExceptions {

			public List<GameException> reported {
				get { return _reported; }
			}
			private List<GameException> _reported = new List<GameException>();

			public void Add(string error, string stackTrace) {
				GameException ex = new GameException();
				ex.TimeStamp = System.DateTime.UtcNow.ToLongDateString();
				ex.TestExecutionTime = System.DateTime.UtcNow.Subtract(AutomationMaster.CurrentTestContext.StartTime).TotalSeconds.ToString();
				ex.CurrentRunningTest = AutomationMaster.CurrentTestContext.TestName;
				ex.Error = AutomationReport.EncodeCharactersForJson(error);
				ex.ErrorDetails = AutomationReport.EncodeCharactersForJson(stackTrace);
				_reported.Add(ex);
			}

		}

		public class GameException {

			public string CurrentRunningTest { get; set; }
			public string TimeStamp { get; set; }
			public string TestExecutionTime { get; set; }
			public string Error { get; set; }
			public string ErrorDetails { get; set; }

		}

	}

}
