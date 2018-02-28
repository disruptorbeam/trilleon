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