using System;
using NUnit.Framework;

namespace NunitMonoTests
{
	/// <summary>
	/// An example test class which uses Nunit as the assertion framework with a couple of custom attributes as a wrapper
	/// test runner wrapper on top which is executed by the NunitMonoTestRunner console app
	/// 
	/// </summary>
	[TestClass]
	public class UnitTestExampleClass
	{
		[TestInitialise]
		public void SetUp()
		{
			Console.WriteLine("Initialising test");
		}

		[TestCleanUp]
		public void CleanUp()
		{
			Console.WriteLine("Cleaning up test");
		}

		[TestMethod]
		public void PassingUnitTestExample()
		{
			Console.WriteLine("here is a passing unit test");
			Assert.AreEqual(1, 1, "test failed 1 to 1 comparison");
			Console.WriteLine();
		}

		[TestMethod]
		public void FailingUnitTestExample()
		{
			Console.WriteLine("here is a failing unit test");
			Assert.Fail("uh oh, you'd better fix this test");
			Console.WriteLine();
		}
	}
}
