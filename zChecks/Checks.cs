using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Diagnostics;
using zChecks;

namespace z {
  public static partial class Checks {
    static readonly object _sync = new object();
    static readonly Dictionary<Assembly, Dictionary<(string file, int line), CheckInfo>> _diagnostics = new Dictionary<Assembly, Dictionary<(string file, int line), CheckInfo>>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(bool condition, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T>(bool condition, T arg, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2>(bool condition, T1 arg1, T2 arg2, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3>(bool condition, T1 arg1, T2 arg2, T3 arg3, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4>(bool condition, T1 arg1, T2 arg2, T3 arg3, T4 arg4, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3, arg4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4, T5>(bool condition, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0, params object[] rest) {
      if (!condition)
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3, arg4, arg5, rest));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(bool? condition, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T>(bool? condition, T arg, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2>(bool? condition, T1 arg1, T2 arg2, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3>(bool? condition, T1 arg1, T2 arg2, T3 arg3, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4>(bool? condition, T1 arg1, T2 arg2, T3 arg3, T4 arg4, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3, arg4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4, T5>(bool? condition, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, DoNotUseArg doNotUse = default, [CallerFilePath]string file = null, [CallerLineNumber]int line = 0, params object[] rest) {
      if (!(condition ?? false))
        throw new CheckException(Diagnostics(Assembly.GetCallingAssembly(), file, line, arg1, arg2, arg3, arg4, arg5, rest));
    }

    static string Diagnostics(Assembly assembly, string file, int line, params object[] args) =>
        FormatDiagnostics(GetCheckInfo(assembly, file, line) ?? new CheckInfo { File = file, Line = line }, args);

    static CheckInfo GetCheckInfo(Assembly assembly, string file, int line) {
      lock (_sync) {
        if (!_diagnostics.TryGetValue(assembly, out Dictionary<(string file, int line), CheckInfo> checksInfo)) {
          checksInfo = new Dictionary<(string file, int line), CheckInfo>();
          var embededChecksInfo = Util.GetChecksInfo(assembly);
          if (embededChecksInfo != null) {
            foreach (var item in embededChecksInfo) {
              // If we got multiple checks in a single line, 
              // we would not be able to identify which of the checks has failed and get correct diagnostics.
              // Thus, nullify diagnostics for lines with multiple checks.
              var key = (item.File, item.Line);
              if (!checksInfo.ContainsKey(key))
                checksInfo.Add(key, item);
              else
                checksInfo[key] = null;
            }
          }
          _diagnostics.Add(assembly, checksInfo);
        }
        checksInfo.TryGetValue((file, line), out CheckInfo result);
        return result;
      }
    }

    static readonly int IndentSize = 4;
    static readonly string Indent = ">".PadRight(IndentSize, ' ');

    static string FormatDiagnostics(CheckInfo diagnostics, object[] args) {
      return String.Format("{0}:{1}: Check failed. {2}", diagnostics.File, diagnostics.Line, FullDescription());

      string FullDescription() => diagnostics.Check == null ? null : String.Join(Environment.NewLine, Enumerable.Empty<string>()
          .Append(String.Empty)
          .Append(Indent)
          .Append(diagnostics.Check.FormatMultiline().IndentMultiline())
          .Append(Indent)
          .Append(args.Length == 0 ? null : ArgsDescription().IndentMultiline())
          .Where(l => l != null));

      // Returns null if there are no non-literal args.
      string ArgsDescription() {
        var description = new StringBuilder();
        for (int i = 0; i < args.Length; i++)
          description.AppendLine(String.Format("[{0}]: {1}", i, args[i]).FormatMultiline());

        return description.ToString();
      }
    }

    static string FormatMultiline(this string str) {
      var lines = str.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      return String.Join(
          Environment.NewLine,
          lines.Skip(1).Select(s => new string(' ', IndentSize) + s.Trim()).Prepend(lines[0]));
    }

    static string IndentMultiline(this string str) {
      return String.Join(
          Environment.NewLine,
          str.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(s => Indent + s));
    }
  }
}