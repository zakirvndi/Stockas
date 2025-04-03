using MediatR;
using Stockas.Models.DTOS;
using System.Text.Json.Serialization;

namespace Stockas.Application.Commands.TransactionCategory
{
    public class CreateTransactionCategoryCommand : IRequest<TransactionCategoryDTO>
    {
        public string CategoryName { get; set; }
        public string Type { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}
