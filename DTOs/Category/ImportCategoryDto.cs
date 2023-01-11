
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Category
{
    [JsonObject]       
    public class ImportCategoryDto
    {
        [StringLength(maximumLength:15, MinimumLength =3)]
        [Required]
        public string Name { get; set; }

    }
}
