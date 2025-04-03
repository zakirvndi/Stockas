using System.Text.Json.Serialization;
using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Commands
{
    public class UpdateProductCategoryCommand : IRequest<ProductCategoryDto>
    {
        [JsonIgnore]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [JsonIgnore]
        public int UserId { get; set; } 

        public UpdateProductCategoryCommand(string categoryName, int userId)
        {
            CategoryName = categoryName;
        }
    }

}