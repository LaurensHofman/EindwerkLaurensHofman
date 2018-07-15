using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(string input)
        {
            // https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance

            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));

                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));

                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
