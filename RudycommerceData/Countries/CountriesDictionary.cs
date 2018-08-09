using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Countries
{
    public class CountriesDictionary : Dictionary<string, string>
    {
        public CountriesDictionary()
        {
            this.Add("BE", Countries.BE);
            this.Add("NL", Countries.NL);
            this.Add("FR", Countries.FR);
            this.Add("GB", Countries.GB);
            this.Add("DE", Countries.DE);
        }
    }
}
