using Newtonsoft.Json;
using ProductShop.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ImportUserDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        [MinLength(GlobalConstants.UserLastNameMinLength)]
        public string LastName { get; set; }
       
        [JsonProperty("age")]
        public int? Age { get; set; }

    }
}
