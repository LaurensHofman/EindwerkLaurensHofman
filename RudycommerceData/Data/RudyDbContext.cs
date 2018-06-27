using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Data
{
    public class RudyDbContext : DbContext
    {
        public RudyDbContext() : base("name=CProefCS")
        {

        }
    }
}
