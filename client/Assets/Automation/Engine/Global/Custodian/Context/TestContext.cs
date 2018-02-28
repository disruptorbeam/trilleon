using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace TrilleonAutomation {

	public class TestContext {

		//Construct an empty, uninitialized context. Ostensibly initialized later in execution.
		public TestContext() : this(null) {}

		//Construct an immediately-initialized context.
		public TestContext(MethodInfo method) {

			TestInitialize(method, method == null ? false : true);

		}

		/// <summary>
		/// Is test a success?
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Start time.
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// Categories
		/// </summary>
		public List<string> Categories { get; set; }

		/// <summary>
		/// Important messages that should be communicated in reports, but are NOT failures.
		/// </summary>
		public List<string> Notices { get; set; }

		/// <summary>
		/// Warning messages that should be communicated in the Warnings panel of reports, but are NOT considered failures.
		/// </summary>
		public List<string> Warnings { get; set; }

		/// <summary>
		/// Name of test currently being run.
		/// </summary>
		public string TestName { get; set; }

		/// <summary>
		/// Class name of test currently being run.
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// Details of current test's failure (if any).
		/// </summary>
		public string ErrorDetails { 
			get { 
				return _errorDetails;
			} 
			set { 
				_errorDetails = AutomationReport.EncodeCharactersForJson(value);
			}
		}
		private string _errorDetails;

		/// <summary>
		/// Status of test set by failure.
		/// </summary>
		public List<string> Assertions { 
			get { 
				return _assertions;
			} 
			private set {
				_assertions = value;
			}
		}
		private List<string> _assertions;
		public void AddAssertion(string newAssertion) {

			Assertions.Add(AutomationReport.EncodeCharactersForJson(newAssertion));

		}

		/// <summary>
		/// System.Reflection object for current test method.
		/// </summary>
		public MethodInfo Method { get; set; }

		/// <summary>
		/// Is current test's context data initialized, or is this context instance not related to any test?
		/// </summary>
		public bool IsInitialized { get; set; }

		/// <summary>
		/// ID's of test cases which are reported to the QA test case web software (if any integrated into this framework, and marked in tes).
		/// </summary>
		public List<int> TestCaseIds {
			get{ return _testCaseIds; }
			set{ _testCaseIds = value; }
		}
		private List<int> _testCaseIds;

		/// <summary>
		/// Base test initialization. This explicitly ties a new context object to a specific test that is actively being run by the Test Runner.
		/// </summary>
		/// <param name="testName">Test name.</param>
		public void TestInitialize(MethodInfo method) {

			TestInitialize(method, true);

		}

		/// <summary>
		/// Sets each property for this context object.
		/// </summary>
		/// <param name="method">Method.</param>
		/// <param name="initialize">Tells this object that it has been given its test to build context for. Only false if empty constructor.</param>
		private void TestInitialize(MethodInfo method, bool initialize) {

			this.TestCaseIds = new List<int>();
			this.Method = method;
			this.TestName = method == null? string.Empty : method.Name;
			this.ClassName = string.Empty;
			this.Notices = new List<string>();
			this.Warnings = new List<string>();
			this.IsSuccess = true;
			this.Categories = new List<string>();
			this.ErrorDetails = string.Empty;
			this.Assertions = new List<string>();
			this.StartTime = DateTime.UtcNow;
			if(initialize) {
				this.IsInitialized = true;
			}

		}

	}

}