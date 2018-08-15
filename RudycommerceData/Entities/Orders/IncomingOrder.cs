using RudycommerceData.Models.ASPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Orders
{
    public class IncomingOrder : BaseEntity<int>
    {
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public string Status { get; set; }

        public string PaymentOption { get; set; }
        public bool PaymentComplete { get; set; }

        public string AddrStreetAndNumber { get; set; }
        public string AddrMailBox { get; set; }
        public string AddrPostalCode { get; set; }
        public string AddrCity { get; set; }
        public string AddrCountry { get; set; }

        public Decimal TotalPrice { get; set; }

        public virtual ICollection<IncomingOrderLines> IncomingOrderLines { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public IncomingOrder()
        {
            IncomingOrderLines = new List<IncomingOrderLines>();
        }

        public IncomingOrder(Delivery delivery)
        {
            IncomingOrderLines = new List<IncomingOrderLines>();

            SetAddress(delivery.StreetAndNumber, delivery.MailBox, delivery.PostalCode, delivery.City, delivery.CountryCode);
        }

        public IncomingOrder(Client client)
        {
            IncomingOrderLines = new List<IncomingOrderLines>();

            SetAddress(client.StreetAndNumber, client.MailBox, client.PostalCode, client.City, client.CountryCode);
        }

        private void SetAddress(string strtandnr, string mailbx, string postalcode, string city, string country)
        {
            AddrStreetAndNumber = strtandnr;
            AddrMailBox = mailbx;
            AddrPostalCode = postalcode;
            AddrCity = city;
            AddrCountry = country;
        }
    }
}
