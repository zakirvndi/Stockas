using System.Text.Json.Serialization;
using MediatR;

namespace Stockas.Application.Commands
{
    public class DeleteProductCategoryCommand : IRequest<Unit>
    {
        public int CategoryId { get; set; }
        [JsonIgnore]
        public int UserId { get; set; } 
    }
}