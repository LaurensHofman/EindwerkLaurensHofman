using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    public static class ObjectExtensions<T>
    {
        /// <summary>
        /// Swaps the values of 2 objects
        /// </summary>
        /// <param name="obj1">First old value</param>
        /// <param name="obj2">Second old value</param>
        /// <param name="swappedObj1">First new value</param>
        /// <param name="swappedObj2">Second new value</param>
        public static void SwapTwoObjects(T obj1, T obj2, out T swappedObj1, out T swappedObj2)
        {
            swappedObj1 = obj2;
            swappedObj2 = obj1;
        }
    }
}
