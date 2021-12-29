using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using zChecks;

namespace PostBuildUtil {
  /// <summary>
  /// Command line utility that collects information on all x.Checks.Check() invovations
  /// and embed that information into the target assembly.
  /// Takes projectDir and targetAssemblyPath as parameters.
  /// When zChecks nuget package is being installed, PostBuildUtil.exe is automatically injected into the target csproj file
  /// as a PostBuild command.
  /// </summary>
  class Program {
    static void Main(string[] args) {
      ValidateArgs(args);
      var projectDir = args[0];
      var targetAssembly = args[1];
      var checks = CollectChecksInfo(projectDir);
      EmbedChecksInfo(targetAssembly, checks: checks);
      Console.Out.WriteLine($"Successfully embeded info on {checks.Count()} check(s) into {args[1]}.");
    }

    static void ValidateArgs(string[] args) {
      if (args == null && args.Length != 2) throw new ApplicationException($"Invalid arguments. Usage: {nameof(PostBuildUtil)}.exe <targetProjectDir> <targetAssemplyPath>");
      var projectDir = args[0];
      var targetAssembly = args[1];
      if (!Directory.Exists(args[0])) throw new ApplicationException($"Project directory \"${projectDir}\" doesn't exist.");
      if (!File.Exists(targetAssembly)) throw new ApplicationException($"Target assembly \"${targetAssembly}\" can not be found.");
    }

    static void EmbedChecksInfo(string targetAssembly, IEnumerable<CheckInfo> checks) {
      var readerParams = new ReaderParameters { ReadSymbols = true };
      var writerParams = new WriterParameters { WriteSymbols = true };
      using (var def = AssemblyDefinition.ReadAssembly(targetAssembly, readerParams)) {
        var checksResource = new EmbeddedResource(Util.ChecksResourceName, ManifestResourceAttributes.Public, Util.SerializeChecksInfo(checks.ToArray()));
        var resourses = def.MainModule.Resources;
        var existingResource = resourses.FirstOrDefault(r => r.Name == Util.ChecksResourceName);
        if (existingResource != null) {
          Console.WriteLine($"{targetAssembly} already contains an embeded resourse {Util.ChecksResourceName}. Replacing it.");
          resourses.Remove(existingResource);
        }
        resourses.Add(checksResource);
        def.Write(Tmp(targetAssembly), writerParams);
      }
      MoveFile(Tmp(targetAssembly), targetAssembly);
      File.WriteAllText(targetAssembly + ".zchecks", "");
    }

    static void MoveFile(string source, string dest) {
      File.Delete(dest);
      File.Move(source, dest);
    }

    static string Tmp(string file) => file + ".tmp";

    static IEnumerable<CheckInfo> CollectChecksInfo(string projectDir) {
      foreach (var file in Directory.EnumerateFiles(projectDir, "*.cs", SearchOption.AllDirectories))
        foreach (var check in ChecksInfo(file))
          yield return check;
    }

    static IEnumerable<CheckInfo> ChecksInfo(string file) {
      using (var sr = new StreamReader(file)) {
        var code = sr.ReadToEnd();
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetCompilationUnitRoot();
        var checks = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(s => s.Expression is IdentifierNameSyntax identSyntax && identSyntax.Identifier.Text == "Check");
        foreach (var check in checks) {
          var line = check.SyntaxTree.GetLineSpan(check.Span).StartLinePosition.Line;
          yield return new CheckInfo {
            File = file,
            Line = line + 1, // line is zero based
            Check = check.ToString(),
          };
        }
      }
    }
  }
}
