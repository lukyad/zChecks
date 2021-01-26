using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zChecks {
  /// <summary>
  /// Contains diagnostic information for a single Check invocation.
  /// Used internally by the library.
  /// </summary>
  [Serializable]
  public class CheckInfo {
    public string File;
    public int Line;
    public string Check;
  }
}
