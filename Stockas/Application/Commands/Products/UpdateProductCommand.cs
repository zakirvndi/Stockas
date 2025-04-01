using System.Text.Json.Serialization;
using MediatR;
using Stockas.Models.DTOS;

public class UpdateProductCommand : IRequest<ProductDTO>
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? CategoryId { get; set; }
    public int? Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? SellingPrice { get; set; }

    [JsonIgnore]
    public int UserId { get; set; }
}
