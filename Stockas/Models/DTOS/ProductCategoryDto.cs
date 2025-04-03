using System.Text.Json.Serialization;

namespace Stockas.Models.DTOS
{
    public class ProductCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        [JsonIgnore] 
        public int UserId { get; set; }
    }
}
