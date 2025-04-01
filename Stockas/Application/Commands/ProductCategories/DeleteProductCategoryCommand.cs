using MediatR;

public class DeleteProductCategoryCommand : IRequest<Unit>
{
    public int CategoryId { get; set; }

    public DeleteProductCategoryCommand(int categoryId)
    {
        CategoryId = categoryId;
    }
}
