using MediatR;
using Stockas.Models.DTOS;
using System.Text.Json.Serialization;

namespace Stockas.Application.Commands.Transaction
{
    public class CreateTransactionCommand : IRequest<TransactionDTO>
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public int? ProductId { get; set; } = null;
    }

}
