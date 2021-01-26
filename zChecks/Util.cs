using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using zChecks;

namespace zChecks {
  public static class Util {
    public static readonly string ChecksResourceName = "zChecks";

    public static byte[] SerializeChecksInfo(CheckInfo[] checks) {
      using (var s = new MemoryStream()) {
        var f = new BinaryFormatter();
        f.Serialize(s, checks);
        return s.ToArray();
      }
    }

    public static CheckInfo[] DeserializeChecksInfo(Stream s) {
      var f = new BinaryFormatter();
      return (CheckInfo[])f.Deserialize(s);
    }

    // Gets Embeded Checks information from an assembly.
    // Returns null, if embeded resource associated with zChecks library doesn't exist.
    public static CheckInfo[] GetChecksInfo(Assembly assembly) {
      using (var s = assembly.GetManifestResourceStream(ChecksResourceName)) {
        if (s == null) return null;
        using (var sr = new StreamReader(s))
          return DeserializeChecksInfo(s);
      }
    }
  }
}