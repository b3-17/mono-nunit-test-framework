using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using NunitMonoTests;

namespace NunitMonoTestRunner
{
	/// <summary>
	/// entry point, set this project as start up or multi start in mono in order to run which ever test library tests set up elsewhere 
	/// in the project.
	/// </summary>
	class MainClass
	{
		public static void Main(string[] args)
		{
			// use test class attribute to get the containing tests assembly. Inject the test assembly into the test runner instance.
			// To use other test assemblies, simply create more test runner instances with the target assemblies.
			TestRunner testRunner = new TestRunner(Assembly.GetAssembly(typeof(NunitMonoTests.TestClassAttribute)));
			testRunner.RunTests();

			WriteTestSummary(testRunner);
		}

		static void WriteTestSummary(TestRunner testRunner)
		{ 
			Console.WriteLine($"Total tests run: { testRunner.TotalTestsRun } for { testRunner.TestLibraryName }");
			Console.WriteLine($"Total tests failed: { testRunner.TotalTestsFailed }");
			Console.WriteLine($"Test run finished in { testRunner.ElapsedRunTime }");
			Console.WriteLine($"Success rate: { testRunner.PercentSuccess }%");
		}
	}

	/// <summary>
	/// Instances of this class are injected with an assembly (class library) which contains test classes and methods
	/// Only classes decorated with the NuintMonoTests.TestDecorators.TestClassAttribute will be collected and their 
	/// child test methods (decorated with NuintMonoTests.TestDecorators.TestMethodAttribute) will be run.
	/// </summary>
	internal class TestRunner
	{
		private Assembly testAssembly { get; set; }
		private Stopwatch timer { get; set; }
		private string divider { get { return "-----------------------------------------------------------------------------------------------------------------------"; } }

		public string ElapsedRunTime { get; set; }
		public string TestLibraryName { get { if (this.testAssembly != null) return this.testAssembly.GetName().Name; else return "Test Library not set"; } }
		public decimal PercentSuccess
		{
			get
			{
				if (this.TotalTestsRun > 0)
					if (this.TotalTestsFailed > 0)
					{
						decimal successRate = ((decimal)(this.TotalTestsRun - this.TotalTestsFailed) / (decimal)this.TotalTestsRun);
						return Math.Round((successRate * 100), 2);
					}
					else return 100;
				else return 0;
			}
		}

		public int TotalTestsRun
		{
			get;
			set;
		}
		public int TotalTestsFailed
		{
			get;
			set;
		}

		public TestRunner(Assembly passedTestAssembly)
		{
			this.testAssembly = passedTestAssembly;
			this.timer = new Stopwatch();
		}

		/// <summary>
		/// Run all the tests from the injected test assembly with the appropriate decorators
		/// </summary>
		public void RunTests()
		{
			this.timer.Start();

			// CLR uses the is static and is sealed to denote static classes. An optimisation, so check that the class is NOT abstract and sealed
			foreach (Type item in this.testAssembly.GetTypes().Where(x => x.IsClass && !(x.IsSealed == true && x.IsAbstract == true)))
			{
				// check for our test fixture classes
				if (item.GetCustomAttributes().OfType<TestClassAttribute>().Any())
					RunClassTestMethods(item);
			}

			timer.Stop();
			TimeSpan timeTestsTaken = timer.Elapsed;

			this.ElapsedRunTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeTestsTaken.Hours, timeTestsTaken.Minutes, timeTestsTaken.Seconds, timeTestsTaken.Milliseconds / 10);
		}

		private void RunClassTestMethods(Type item)
		{
			// only get the methods with our custom test decoration
			MethodInfo[] methods = item.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
			                           .Where(x => x.GetCustomAttributes().OfType<TestMethodAttribute>().Any()).ToArray();

			Console.WriteLine($"Running { methods.Count().ToString() } tests for {item.Name}");
			Console.WriteLine();
			this.TotalTestsRun += methods.Count();

			var currentTestClassType = item as Type;
			var instance = Activator.CreateInstance(currentTestClassType);

			foreach (MethodInfo method in methods)
				this.InvokeTestMethod(currentTestClassType, instance, method);

			Console.WriteLine();
			Console.WriteLine($"Finished running tests for {item.Name}");
			Console.WriteLine(this.divider);
		}

		private void InvokeTestMethod(Type currentTestClassType, object instance, MethodInfo method)
		{
			var setUp = currentTestClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
								.Where(x => x.GetCustomAttributes().OfType<TestInitialiseAttribute>().Any()).FirstOrDefault();

			var cleanUp = currentTestClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
			                                  .Where(x => x.GetCustomAttributes().OfType<TestCleanUpAttribute>().Any()).FirstOrDefault();

			if (setUp != null)
				setUp.Invoke(instance, null);

			try
			{
				method.Invoke(instance, null);
				Console.WriteLine($"Ran {method.Name} ... PASS");

			}
			catch (Exception ex)
			{
				string exceptionMessage = (ex.InnerException != null ? ex.InnerException.Message : "no inner exception exists");
				string stackTrace = (ex.InnerException != null ? ex.InnerException.StackTrace : "no inner exception exists");
				Console.WriteLine();
				Console.WriteLine($"Ran {method.Name} ... FAILED");
				Console.WriteLine();
				Console.WriteLine($"Test failed for { method.Name }. The exception was { exceptionMessage } which happened at { stackTrace }");

				Console.WriteLine();

				this.TotalTestsFailed += 1;
			}
			finally
			{
				if (cleanUp != null)
					cleanUp.Invoke(instance, null);
			}
		}
	}
}
