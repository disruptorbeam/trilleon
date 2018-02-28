namespace TrilleonAutomation {

	/// <summary>
	/// Unbound = Failure is called in a non-test method without the context of an instantiated current method.
	/// </summary>
	public enum FailureContext {

		Default,
		Unbound,
		Skipped,
		SetUpClass,
		SetUpClassGlobal,
		SetUpGlobal,
		SetUp,
		TestMethod,
		TearDown,
		TearDownGlobal,
		TearDownClass,
		TearDownClassGlobal

	}

}