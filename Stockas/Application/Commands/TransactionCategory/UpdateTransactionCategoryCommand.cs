using MediatR;
using Stockas.Models.DTOS;
using System.Text.Json.Serialization;

namespace Stockas.Application.Commands.TransactionCategory
{
    public class UpdateTransactionCategoryCommand : IRequest<TransactionCategoryDTO>
    {
        [JsonIgnore]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        [JsonIgnore]
        public string? Type { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
