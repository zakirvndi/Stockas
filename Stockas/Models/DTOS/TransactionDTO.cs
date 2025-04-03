namespace Stockas.Models.DTOS
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public string? ProductName { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
