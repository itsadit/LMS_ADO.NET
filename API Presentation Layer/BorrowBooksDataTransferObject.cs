using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DataTransferObjects
{
    public class Request
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
    public class FinePaymentRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public string TransactionID { get; set; }
    }

    public class ByUserIDRequest
    {
        [Required]
        public int UserID { get; set; }
    }
    public class ByBookIDRequest
    {
        [Required]
        public int BookID { get; set; }
    }
    public class ByBookNameRequest
    {
        [Required]
        public string BookName { get; set; }
    }

}
