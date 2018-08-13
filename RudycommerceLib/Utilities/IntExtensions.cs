using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    public static class IntExtensions
    {
        public static bool TestRange(int input, int minimum, int maximum)
        {
            return (input >= minimum && input <= maximum);
        }
    }
}
