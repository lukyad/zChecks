using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using zChecks;

namespace PostBuildUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidateArgs(args);
            var checksInfo = CollectChecksInfo(projectDir: args[0]);
            EmbedChecksInfo(args[1], checksInfo);
            Console.Out.WriteLine($"Successfully embeded info on {checksInfo.Count()} check(s) into {args[1]}.");
        }

        static void ValidateArgs(string[] args)
        {
        }

        static void EmbedChecksInfo(string targetAssembly, IEnumerable<CheckInfo> checks)
        {
            var readerParams = new ReaderParameters { ReadSymbols = true };
            var writerParams = new WriterParameters { WriteSymbols = true };
            using (var def = AssemblyDefinition.ReadAssembly(targetAssembly, readerParams))
            {
                var checksResource = new EmbeddedResource(Util.ChecksResourceName, ManifestResourceAttributes.Public, Util.SerializeChecksInfo(checks.ToArray()));
                var resourses = def.MainModule.Resources;
                var existingResource = resourses.FirstOrDefault(r => r.Name == Util.ChecksResourceName);
                if (existingResource != null)
                {
                    Console.WriteLine($"{targetAssembly} already contains an embeded resourse {Util.ChecksResourceName}. Replacing it.");
                    resourses.Remove(existingResource);
                }
                resourses.Add(checksResource);
                def.Write(Tmp(targetAssembly), writerParams);
            }
            MoveFile(Tmp(targetAssembly), targetAssembly);
        }

        static void MoveFile(string source, string dest)
        {
            File.Copy(source, dest, overwrite: true);
            File.Delete(source);
        }

        static string Tmp(string file) => file + ".tmp";

        static CustomAttribute CreateDubuggableAttr(AssemblyDefinition def)
        {
            var attr = new CustomAttribute(def.MainModule.ImportReference(typeof(DebuggableAttribute).GetConstructor(new[] { typeof(bool), typeof(bool) })));
            attr.ConstructorArguments.Add(new CustomAttributeArgument(def.MainModule.ImportReference(typeof(bool)), true));
            attr.ConstructorArguments.Add(new CustomAttributeArgument(def.MainModule.ImportReference(typeof(bool)), true));
            return attr;
        }

        static IEnumerable<CheckInfo> CollectChecksInfo(string projectDir)
        {
            foreach (var file in Directory.EnumerateFiles(projectDir, "*.cs", SearchOption.AllDirectories))
                foreach (var check in ChecksInfo(file))
                    yield return check;
        }

        static IEnumerable<CheckInfo> ChecksInfo(string file)
        {
            using (var sr = new StreamReader(file))
            {
                var code = sr.ReadToEnd();
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetCompilationUnitRoot();
                var checks = root.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(s => s.Expression is IdentifierNameSyntax identSyntax && identSyntax.Identifier.Text == "Check");
                foreach (var check in checks)
                {
                    var line = check.SyntaxTree.GetLineSpan(check.Span).StartLinePosition.Line;
                    var argList = check.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();
                    yield return new CheckInfo
                    {
                        File = file,
                        Line = line + 1, // line is zero based
                        Condition = argList.First(),
                        Args = argList.Skip(1).ToArray(),
                    };
                }
            }
        }
    }
}
