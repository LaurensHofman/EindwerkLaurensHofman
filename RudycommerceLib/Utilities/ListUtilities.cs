using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    public static class ListUtilities<T>
    {
        /// <summary>
        /// Swaps 2 items in a list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <returns></returns>
        public static List<T> Swap(List<T> list, int indexA, int indexB)
        {
            if (list.Count <= indexA || list.Count <= indexB)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                T tmp = list[indexA];
                list[indexA] = list[indexB];
                list[indexB] = tmp;

                return list;
            }            
        }
    }
}
