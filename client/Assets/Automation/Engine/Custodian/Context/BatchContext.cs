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

﻿using System.Collections.Generic;

namespace TrilleonAutomation {

	public class BatchContext {

		public List<string> CompletedTests {
			get { return _completedTests; }
		}
		private List<string> _completedTests = new List<string>();

		public Passes Passed = new Passes();
		public Ignores Ignored = new Ignores();
		public Fails Failed = new Fails();
		public Skips Skipped = new Skips();
		public GameExceptions Exceptions = new GameExceptions();

		public class Passes {

			public List<string> Tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					Tests.Add(test);
				}
			}

		}

		public class Skips {

			public List<string> Tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					Tests.Add(test);
				}
			}

		}

		public class Fails {

			public List<KeyValuePair<string,string[]>> Tests {
				get { return _tests; }
			}
			private List<KeyValuePair<string,string[]>> _tests = new List<KeyValuePair<string,string[]>>();

			public void Add(string test, string[] values) {
				if(!_tests.KeyValListContainsKey(test)) {
					Tests.Add(new KeyValuePair<string,string[]>(test, values));
				}
			}

		}

		public class Ignores {

			public List<string> Tests {
				get { return _tests; }
			}
			private List<string> _tests = new List<string>();

			public void Add(string test) {
				if(!_tests.Contains(test)) {
					Tests.Add(test);
					string message = string.Format("[{0}] {1}", test, "#Ignore#");
					AutoConsole.PostMessage(message);
				}
			}

		}

		public class GameExceptions {

			public List<GameException> Reported {
				get { return _reported; }
			}
			private List<GameException> _reported = new List<GameException>();

			public void Add(AutoConsole.Log error) {

				GameException ex = new GameException();
				ex.ScreenshotName = string.Format("EXCEPTION_{0}", Reported.Count);
				ex.TimeStamp = System.DateTime.UtcNow.ToLongDateString();
				ex.TestExecutionTime = System.DateTime.UtcNow.Subtract(AutomationMaster.CurrentTestContext.StartTime).TotalSeconds.ToString();
				ex.CurrentRunningTest = AutomationMaster.CurrentTestContext.TestName;
				ex.Error = AutomationReport.EncodeCharactersForJson(error.message).Replace(AutomationMaster.DELIMITER.ToString(), "%7C"); //Encode AutomationMaster.DELIMITER character or errors will occur in data parsing in server.
				ex.ErrorDetails = AutomationReport.EncodeCharactersForJson(error.stackTrace).Replace(AutomationMaster.DELIMITER.ToString(), "%7C"); //Encode AutomationMaster.DELIMITER character or errors will occur in data parsing in server.
				ex.Occurrences = 1;
				for(int r = 0; r < Reported.Count; r++) {

					if(Reported[r].Error == ex.Error && Reported[r].ErrorDetails == ex.ErrorDetails) {

						Reported[r].Occurrences++;
						return;

					}

				}
				AutomationMaster.StaticSelfComponent.TakeScreenshotAsync(false, ex.ScreenshotName); //Only take a screenshot if it is not a duplicate. Spammed errors would lead to spammed screenshots.
				_reported.Add(ex);

			}

		}

		public class GameException {

			public string CurrentRunningTest { get; set; }
			public string TimeStamp { get; set; }
			public string TestExecutionTime { get; set; }
			public string Error { get; set; }
			public string ErrorDetails { get; set; }
			public string ScreenshotName { get; set; }
			public int Occurrences { get; set; }

		}

	}

}
