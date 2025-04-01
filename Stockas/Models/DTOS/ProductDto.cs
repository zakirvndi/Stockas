namespace Stockas.Models.DTOS
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string CategoryName { get; set; }
        public DateOnly InputDate { get; set; }
    }
}
