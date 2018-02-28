namespace TrilleonAutomation {

	/// <summary>
	/// Wan't specific assertions to not cease test execution during failure? Try `Q.assert.Soft.CheckSomeValue()`. This indicates to the compiler NOT to cease execution of the test if just this assertion fails.
	/// Any subsequent assertion failures are ignored when reporting the cause of the test failure, although a future non-Soft assertion failure in the current test execution will still kill current test execution.
	/// </summary>
	public enum TestFlag {
		TryCompleteAfterFail, 
		OnlyLaunchWhenExplicitlyCalled,
		DependencyNoSkip,
		DisregardSetUpClassGlobal,
		DisregardTearDownClassGlobal,
		DisregardSetUpGlobal,
		DisregardTearDownGlobal,
		DisregardSetUpTest,
		DisregardTearDownTest,
	}

}