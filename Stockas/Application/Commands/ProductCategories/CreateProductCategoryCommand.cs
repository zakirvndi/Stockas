using System.Text.Json.Serialization;
using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Commands
{
    public class CreateProductCategoryCommand : IRequest<ProductCategoryDto>
    {
        public string CategoryName { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }

}
