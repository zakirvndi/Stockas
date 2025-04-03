using System.Text.Json.Serialization;
using MediatR;

public class DeleteProductCommand : IRequest
{
    public int ProductId { get; set; }
    [JsonIgnore]
    public int UserId { get; set; } 
}
