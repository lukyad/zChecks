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
        public static void Check(bool condition, DoNotUseArg doNotUse = default, string msg = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), msg, file, line));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check<T>(bool condition, T arg, DoNotUseArg doNotUse = default, string msg = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), msg, file, line, arg));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check<T1, T2>(bool condition, T1 arg1, T2 arg2, DoNotUseArg doNotUse = default, string msg = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), msg, file, line, arg1, arg2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check<T1, T2, T3>(bool condition, T1 arg1, T2 arg2, T3 arg3, DoNotUseArg doNotUse = default, string msg = null, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0)
        {
            if (!condition)
                throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), msg, file, line, arg1, arg2, arg3));
        }

        static string Diagnostics(Assembly assembly, string msg, string file, int line, params object[] args) =>
            FormatDiagnostics(GetCheckInfo(assembly, file, line) ?? new CheckInfo { File = file, Line = line }, msg, args);

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

        static string FormatDiagnostics(CheckInfo diagnostics, string msg, object[] args)
        {
            int msgCount = msg != null ? 1 : 0;
            if (diagnostics.Args != null && diagnostics.Args.Length - msgCount != args.Length) throw new ApplicationException($"Internal {nameof(zChecks)} error: diagnostics.Args.Length != args.Length");

            return String.Format("Check failed: {0}{1}{2}", DescribeCondition(), Environment.NewLine, String.Join(Environment.NewLine,
                Enumerable.Empty<string>()
                .Concat(DescribeArgs())
                .Append($"file: {diagnostics.File}")
                .Append($"line: {diagnostics.Line}")
                .Where(d => d != null)));

            string DescribeCondition()
            {
                if (diagnostics.Condition == null) return msg;
                else return ($"`{diagnostics.Condition}`" + " - " + msg).Trim(' ', '-');
            }

            IEnumerable<string> DescribeArgs()
            {
                if (diagnostics.Condition == null)
                    yield break;

                var nonMsgDiagnosticArgs = diagnostics.Args.Where(a => !a.StartsWith(nameof(msg) + ':')).ToArray();
                for (int i = 0; i < args.Length; i++)
                    yield return $"{nonMsgDiagnosticArgs[i]}: {args[i]}";
            }
        }
    }
}