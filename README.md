# mono-nunit-test-framework
Since mono is bonkers and has issues with anything to do with unit testing, create my own test runner on top of the nunit framework

Getting test runners to work in mono is a huge pain, and seems to break with every release of Nunit. Visual studio testing tools does not work at all. For an easy life, just wrap the important stuff (the Nunit testing framework) with a VERY simple console app test runner which works and you can modify it to your taste. Hell, you can customise the output to whatever you want!

This could also easily be ported into a CI set up since the console app would easily break down into an .exe which could be triggered in a custom script or service.

Simply decorate test classes, test setup and tear down methods and test methods with the decorators in NunitMonoTests assembly, and the console application will discover test classes, collect their test methods and run the initialise, test method, then teardown sequentially.
