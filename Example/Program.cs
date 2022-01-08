using System;

using static z.Checks;

namespace zChecksTest {
  class Program {
    static void Test(string myString, int myInt, double myDouble, object[] myArray) {
      try {
        Check(!String.IsNullOrEmpty(myString));
        Check(myString.EndsWith("ZeroChecks"), myString);
        Check(myInt > 0,
              "expected positive int",
              myInt);
        Check(myDouble > myInt, myInt, myDouble);
        Check(myArray != null, "array can not be null");
        Check(myArray.Length > 0, myArray.Length);
      } catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    static void Main(string[] args) {
      var timer = System.Diagnostics.Stopwatch.StartNew();
      for (int i = 0; i < 1e9; i++) z.Checks.Check(i < 1e9, 1, 2, 3, 4, 5);
      Console.WriteLine(timer.Elapsed.TotalMilliseconds);

      Test(myString: null, myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Example\Program.cs:9: Check failed.
      // >
      // > Check(!String.IsNullOrEmpty(myString))
      // >

      Test(myString: "I Love Programming", myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Example\Program.cs:10: Check failed.
      // >
      // >   Check(myString.EndsWith("ZeroChecks"), myString)
      // >
      // >   [0]: I Love Programming
      // >

      Test(myString: "I Love ZeroChecks", myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:14: Check failed.
      // >
      // >   Check(myInt > 0, "expected positive int", myInt)
      // >
      // >   [0]: expected positive int
      // >   [1]: 0
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Example\Program.cs:14: Check failed.
      // >
      // >   Check(myDouble > myInt, myInt, myDouble)
      // >
      // >   [0]: 1
      // >   [1]: 0
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 1.5, myArray: null);
      // MY_REPOS\zChecks\Example\Program.cs:15: Check failed.
      // >
      // >   Check(myArray != null, "array can not be null")
      // >
      // >   [0]: array can not be null
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { });
      // MY_REPOS\zChecks\Example\Program.cs:16: Check failed.
      // >
      // >   Check(myArray.Length > 0, myArray.Length)
      // >
      // >   [0]: 0
      // >

      Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { 1, 2, 3 });
      // The call above doesn't generate any exceptions and thus produces no console output.
    }
  }
}
