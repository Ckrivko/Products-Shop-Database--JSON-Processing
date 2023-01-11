using Newtonsoft.Json;
using ProductShop.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ImportProductDto
    {

        [MinLength(GlobalConstants.ProductNameMinLength)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int SellerId { get; set; }

        public int? BuyerId { get; set; }

    }
}
