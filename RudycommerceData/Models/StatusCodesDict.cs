using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class StatusCodesDict : Dictionary<int, string>
    {
        public StatusCodesDict()
        {
            this.Add(0, LangResources.Data.DeliveryOrdered);
            this.Add(1, LangResources.Data.DeliveryReadyForPickup);
            this.Add(2, LangResources.Data.DeliveryUnderWay);
            this.Add(3, LangResources.Data.DeliveryDelivered);
        }
    }
}
