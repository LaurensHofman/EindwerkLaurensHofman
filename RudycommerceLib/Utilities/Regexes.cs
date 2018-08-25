using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Utilities
{
    /// <summary>
    /// Contains custom Regex strings
    /// </summary>
    public static class Regexes
    {
        /// <summary>
        /// Contains a Regex for Email addresses
        /// </summary>
        public const string EmailRegex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                        + "@"
                        + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
    }
}
