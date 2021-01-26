using System;

namespace zChecksTest {
  using static z.Checks;
  class Program {
    static void Test(string myString, int myInt, double myDouble, object[] myArray) {
      try {
        // Two Checks on the same line result in no detailed dignostics to be printed, just file:line
        Check(!String.IsNullOrEmpty(myString)); Check(!String.IsNullOrEmpty(myString));
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
      Test(myString: null, myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:12: Check failed.

      Test(myString: "I Love Programming", myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:13: Check failed.
      // >
      // >   Check(myString.EndsWith("ZeroChecks"), myString)
      // >
      // >   [0]: I Love Programming
      // >

      Test(myString: "I Love ZeroChecks", myInt: 0, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:14: Check failed.
      // >
      // >   Check(myInt > 0,
      // >       "expected positive int",
      // >       myInt)
      // >
      // >   [0]: expected positive int
      // >   [1]: 0
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 0, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:17: Check failed.
      // >
      // >   Check(myDouble > myInt, myInt, myDouble)
      // >
      // >   [0]: 1
      // >   [1]: 0
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 1.5, myArray: null);
      // MY_REPOS\zChecks\Sample\Program.cs:18: Check failed.
      // >
      // >   Check(myArray != null, "array can not be null")
      // >
      // >   [0]: array can not be null
      // >

      Test(myString: "I Love ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { });
      // MY_REPOS\zChecks\Sample\Program.cs:19: Check failed.
      // >
      // >   Check(myArray.Length > 0, myArray.Length)
      // >
      // >   [0]: 0
      // >

      Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { 1, 2, 3 });
      // The call above doesn't generate any exceptions and thus produces an empty Console Output.
    }
  }
}






