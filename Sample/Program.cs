using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    using static z.Checks;

    class Program
    {
        static void Main(string[] args)
        {
            Check(args != null);
            Check(args.Length > 0, args.Length);
        }
    }
}