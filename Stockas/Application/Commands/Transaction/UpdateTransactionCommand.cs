using MediatR;
using Stockas.Models.DTOS;
using System.Text.Json.Serialization;

namespace Stockas.Application.Commands.Transaction
{
    public class UpdateTransactionCommand : IRequest<TransactionDTO>
    {
        [JsonIgnore]
        public int TransactionId { get; set; }

        public string? CategoryName { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public int? ProductId { get; set; }
    }
}
