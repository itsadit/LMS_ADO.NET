namespace LibraryManagementSystem.Models
{
    public class FinePayments
    {
        public int PaymentID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;  // Default to current date
        public string PaymentMethod { get; set; }
        public string TransactionID { get; set; }
    }
}
