using System;
namespace NunitMonoTests
{
	/// <summary>
	/// wrapper attribute for test classes for the test runner
	/// </summary>
	public class TestClassAttribute : System.Attribute { }

	/// <summary>
	/// wrapper attribute for test initialisations for the test runner
	/// methods with this attribute will run before all the test methods contained in a specific
	/// test class
	/// </summary>
	public class TestInitialiseAttribute : System.Attribute { }

	/// <summary>
	/// wrapper attribute for test teardowns for the test runner
	/// methods with this attribute will run after all the test methods contained in a specific
	/// test class
	/// </summary>
	public class TestCleanUpAttribute : System.Attribute { }

	/// <summary>
	/// wrapper attribute for collecting all the test methods in a test class for the test runner
	/// </summary>
	public class TestMethodAttribute : System.Attribute { }
}
