using System.Text.Json.Serialization;
using MediatR;
using Stockas.Models.DTOS;

public class CreateProductCommand : IRequest<ProductDTO>
{
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int Quantity { get; set; }

    [JsonIgnore]
    public int UserId { get; set; }
}
