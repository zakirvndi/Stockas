using MediatR;
using Stockas.Models.DTOS;
using System.Text.Json.Serialization;

namespace Stockas.Application.Queries.TransactionCategory
{
    public class GetTransactionCategoriesQuery : IRequest<List<TransactionCategoryDTO>>
    {
        [JsonIgnore]
        public int UserId { get; set; }
    }
}
