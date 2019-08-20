using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using zChecks;

namespace z
{
    public static partial class Checks
    {
        static readonly object _sync = new object();
        static readonly Dictionary<Assembly, Dictionary<(string file, int line), CheckInfo>> _diagnostics = new Dictionary<Assembly, Dictionary<(string file, int line), CheckInfo>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check(bool condition, DummyArg doNotUse = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check<T>(bool condition, T arg, DummyArg doNotUse = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check<T1, T2>(bool condition, T1 arg1, T2 arg2, DummyArg doNotUse = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2));
        }

        static string Diagnostics(Assembly assembly, string file, int line, params object[] args) =>
            FormatDiagnostics(GetCheckInfo(assembly, file, line) ?? new CheckInfo { File = file, Line = line }, args);

        static CheckInfo GetCheckInfo(Assembly assembly, string sourceFilePath, int lineNumber)
        {
            lock (_sync)
            {
                if (!_diagnostics.TryGetValue(assembly, out Dictionary<(string file, int line), CheckInfo> checksInfo))
                {
                    checksInfo = Util.GetChecksInfo(assembly)?.ToDictionary(keySelector: c => (c.File, c.Line));
                    _diagnostics.Add(assembly, checksInfo);
                }
                if (checksInfo == null) return null;
                checksInfo.TryGetValue((sourceFilePath, lineNumber), out CheckInfo result);
                return result;
            }
        }

        static string FormatDiagnostics(CheckInfo diagnostics, object[] args)
        {
            return String.Format("Check failed:{0}{1}", Environment.NewLine, String.Join(Environment.NewLine,
                Enumerable.Empty<string>()
                .Append(diagnostics.Condition)
                .Concat(DescribeArgs())
                .Append($"file: {diagnostics.File}")
                .Append($"line: {diagnostics.Line}")
                .Where(d => d != null)));

            IEnumerable<string> DescribeArgs()
            {
                if (diagnostics.Condition == null)
                    yield break;
                for (int i = 0; i < diagnostics.Args.Length; i++)
                    yield return $"{diagnostics.Args[i]}: {args[i]}";
            }
        }
    }
}