using System.Text.Json.Serialization;
using MediatR;
using Stockas.Models.DTOS;

public class UpdateProductCategoryCommand : IRequest<ProductCategoryDto>
{
    [JsonIgnore]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public UpdateProductCategoryCommand(string categoryName)
    {
        CategoryName = categoryName;
    }
}
