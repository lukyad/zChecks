using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zChecks {
  /// <summary>
  /// Exception being thrown whenever condition passed to the Check() methods is evaluated to false. 
  /// </summary>
  public class CheckException : Exception {
    internal CheckException(string diagnostics) : base(diagnostics) {
    }
  }

  /// <summary>
  /// Used in z.Checks.Check() method overloads.
  /// Required to get correct overload of the Check method invoked when first argument is of the System.String type.
  /// </summary>
  public struct DoNotUseArg { }
}
