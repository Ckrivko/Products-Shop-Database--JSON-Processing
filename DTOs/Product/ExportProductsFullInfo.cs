using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ExportProductsFullInfo
    {
        [JsonProperty("count")]
        public int ProductsCount => Products.Any() ? Products.Length : 0;
        
        [JsonProperty("products")]
        public ExportProductsShortInfo[] Products { get; set; }
    }
}
