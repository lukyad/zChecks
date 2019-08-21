# zChecks
Minimalistic conditions checking library with zero overhead and plentiful diagnostics.

## Sample Usage

```csharp
using static z.Checks;
class Program
{
    static void Main(string[] args)
    {
        Test(myString: null, myInt: 0, myDouble: 0, myArray: null);
        // Console Output
        // Check failed: `!String.IsNullOrEmpty(myString)`
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 52
        Test(myString: "I ♥ Programming", myInt: 0, myDouble: 0, myArray: null);
        // Console Output
        // Check failed: `myString.EndsWith("ZeroChecks")`
        // myString: I ♥ Programming
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 53
        Test(myString: "I ♥ ZeroChecks", myInt: 0, myDouble: 0, myArray: null);
        // Console Output
        // Check failed: `myInt > 0` - expected positive int
        // myInt: 0
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 54
        Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 0, myArray: null);
        // Console Output
        // Check failed: `myDouble > myInt`
        // myInt: 1
        // myDouble: 0
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 55
        Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 1.5, myArray: null);
        // Console Output
        // Check failed: `myArray != null` - array can not be null
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 56
        Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { });
        // Console Output
        // Check failed: `myArray.Length > 0`
        // myArray.Length: 0
        // file: MY_REPOS\zChecks\Sample\Program.cs
        // line: 57

        Test(myString: "I ♥ ZeroChecks", myInt: 1, myDouble: 1.5, myArray: new object[] { 1, 2, 3 });
        // The call above doesn't generate any exceptions and thus produces an empty Console Output.
    }

    public static void Test(string myString, int myInt, double myDouble, object[] myArray)
    {
        try
        {
            Check(!String.IsNullOrEmpty(myString));
            Check(myString.EndsWith("ZeroChecks"), myString);
            Check(myInt > 0, myInt, msg: "expected positive int");
            Check(myDouble > myInt, myInt, myDouble);
            Check(myArray != null, msg: "array can not be null");
            Check(myArray.Length > 0, myArray.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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

