using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class CartFromJSON
    {
        [JsonProperty("productList")]
        public ProductFromJSON[] ProductList { get; set; }

        public override string ToString()
        {
            string tostring = "";

            foreach (ProductFromJSON p in this.ProductList)
            {
                tostring += p.ToString() + ";; ";
            }

            return tostring;
        }
    }

    public class ProductFromJSON
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public Decimal Price { get; set; }

        public override string ToString()
        {
            return this.Quantity.ToString() + " " + this.Name;
        }
    }
}
