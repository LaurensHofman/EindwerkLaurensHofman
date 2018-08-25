using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    public static class IntExtensions
    {
        /// <summary>
        /// Tests whether 2 integers is in a certain range ( input in [min ; max] )
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static bool TestRange(int input, int minimum, int maximum)
        {
            return (input >= minimum && input <= maximum);
        }

        /// <summary>
        /// Swaps 2 integers
        /// </summary>
        /// <param name="int1">First old value</param>
        /// <param name="int2">Second old value</param>
        /// <param name="swappedInt1">First new value</param>
        /// <param name="swappedInt2">Second new value</param>
        public static void SwapTwoIntegers(int int1, int int2, out int swappedInt1, out int swappedInt2)
        {
            swappedInt1 = int2;
            swappedInt2 = int1;
        }
    }
}
