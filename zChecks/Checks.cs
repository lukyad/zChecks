﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Diagnostics;
using zChecks;

namespace z {
  public static class Checks {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(
        bool condition,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null, 
        [CallerFilePath] string file = null, 
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(conditionStr, file, line);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T>(
        bool condition,
        T arg,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null,
        [CallerArgumentExpression("arg")] string argStr = null,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(arg, conditionStr, argStr, file, line);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2>(
        bool condition,
        T1 arg1,
        T2 arg2,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null,
        [CallerArgumentExpression("arg1")] string argStr1 = null,
        [CallerArgumentExpression("arg2")] string argStr2 = null,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(arg1, arg2, conditionStr, argStr1, argStr2, file, line);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3>(
        bool condition,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null,
        [CallerArgumentExpression("arg1")] string argStr1 = null,
        [CallerArgumentExpression("arg2")] string argStr2 = null,
        [CallerArgumentExpression("arg3")] string argStr3 = null,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(arg1, arg2, arg3, conditionStr, argStr1, argStr2, argStr3, file, line);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4>(
        bool condition,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null,
        [CallerArgumentExpression("arg1")] string argStr1 = null,
        [CallerArgumentExpression("arg2")] string argStr2 = null,
        [CallerArgumentExpression("arg3")] string argStr3 = null,
        [CallerArgumentExpression("arg4")] string argStr4 = null,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(arg1, arg2, arg3, arg4, conditionStr, argStr1, argStr2, argStr3, argStr4, file, line);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check<T1, T2, T3, T4, T5>(
        bool condition,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        DoNotUseArg doNotUse = default,
        [CallerArgumentExpression("condition")] string conditionStr = null,
        [CallerArgumentExpression("arg1")] string argStr1 = null,
        [CallerArgumentExpression("arg2")] string argStr2 = null,
        [CallerArgumentExpression("arg3")] string argStr3 = null,
        [CallerArgumentExpression("arg4")] string argStr4 = null,
        [CallerArgumentExpression("arg5")] string argStr5 = null,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
      if (!condition) CheckSlow(arg1, arg2, arg3, arg4, arg5, conditionStr, argStr1, argStr2, argStr3, argStr4, argStr5, file, line);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void CheckSlow(
        string conditionStr,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr), file, line));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CheckSlow<T>(
        T arg,
        string conditionStr,
        string argStr,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr, argStr), file, line, arg));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void CheckSlow<T1, T2>(
        T1 arg1,
        T2 arg2,
        string conditionStr,
        string argStr1,
        string argStr2,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr, argStr1, argStr2), file, line, arg1, arg2));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CheckSlow<T1, T2, T3>(
        T1 arg1,
        T2 arg2,
        T3 arg3,
        string conditionStr,
        string argStr1,
        string argStr2,
        string argStr3,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr, argStr1, argStr2, argStr3), file, line, arg1, arg2, arg3));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CheckSlow<T1, T2, T3, T4>(
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        string conditionStr,
        string argStr1,
        string argStr2,
        string argStr3,
        string argStr4,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr, argStr1, argStr2, argStr3, argStr4), file, line, arg1, arg2, arg3, arg4));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CheckSlow<T1, T2, T3, T4, T5>(
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        string conditionStr,
        string argStr1,
        string argStr2,
        string argStr3,
        string argStr4,
        string argStr5,
        string file,
        int line) {
      Throw(Diagnostics(FormatCheck(conditionStr, argStr1, argStr2, argStr3, argStr4, argStr5), file, line, arg1, arg2, arg3, arg4, arg5));
    }

    static void Throw(string message) => throw new CheckException(message);

    static string FormatCheck(params string[] args) => $"Check({String.Join(", ", args)})";

    static readonly int IndentSize = 4;
    static readonly string Indent = ">".PadRight(IndentSize, ' ');

    static string Diagnostics(string check, string file, int line, params object[] args) {
      return String.Format("{0}:{1}: Check failed. {2}", file, line, FullDescription());

      string FullDescription() => check == null ? null : String.Join(Environment.NewLine, Enumerable.Empty<string>()
          .Append(String.Empty)
          .Append(Indent)
          .Append(check.FormatMultiline().IndentMultiline())
          .Append(Indent)
          .Append(args.Length == 0 ? null : ArgsDescription().IndentMultiline())
          .Where(l => l != null))
          .TrimEnd();

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
          str.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(s => Indent + s)).TrimEnd();
    }
  }
}