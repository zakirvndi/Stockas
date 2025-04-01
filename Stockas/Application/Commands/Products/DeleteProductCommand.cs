using MediatR;

public class DeleteProductCommand : IRequest
{
    public int ProductId { get; set; }
    public int UserId { get; set; } // Untuk logging siapa yang menghapus
}
