# zChecks
Minimalistic conditions checking library with zero overhead and plentiful diagnostics.

## Sample Usage

```csharp
using static z.Checks;
class Program
{
    static void Test(string myString, int myInt, double myDouble, object[] myArray)
    {
        try
        {
            // Two Checks on the same line result in no detailed dignostics to be printed, just file:line
            Check(!String.IsNullOrEmpty(myString)); Check(!String.IsNullOrEmpty(myString));
            Check(myString.EndsWith("ZeroChecks"), myString);
            Check(myInt > 0,
                "expected positive int",
                myInt);
            Check(myDouble > myInt, myInt, myDouble);
            Check(myArray != null, "array can not be null");
            Check(myArray.Length > 0, myArray.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static void Main(string[] args)
    {
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
```

## Distribution
zChecks is distributed via a nuget package hosted on nugets.org.

## Dependencies

No dependencies other than .NET Framework.

## Supported Platforms

Windows with .NET Framework >= 4.7.2.

## Installation

PM> Install-Package zChecks

No additional steps are required.

# Couple of words on how it works

It looks like a magic how zChecks is able to capture the Check context! But no worires, there is no magic here. When zChecks package is being installed it injects an extra PostBuild step into the target project. During this step zChecks parses all csharp files in the project using Roslyn API and collects context for all Check invocations. Then it embeds this information into the target assembly. In runtime zChecks use the embeded information to generate diagnostics messages. That's it!

# Limitations

If two (or more) Checks are placed to the same line of code, then no detailed diagnostics to be avaialble, just File:Line.

