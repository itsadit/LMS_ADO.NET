
using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models.Enum;

namespace LibraryManagementSystem.Models
{

    public class Request
    {
        /// <summary>
        /// Input BookID(Integer value) to get required Result. To Know BookID Check on the 1st Page of Book
        /// </summary>
        [Required(ErrorMessage = "BookID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BookID must be greater than 0 .")]
        public int BookID { get; set; }
        /// <summary>
        /// Input UserID(Integer Value) to get required Result
        /// </summary>
        [Required(ErrorMessage = "UserID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserID must be greater than 0 .")]
        public int UserID { get; set; }
    }
    public class FinePaymentRequest
    {
        /// <summary>
        /// Input UserID(Integer value) to get required Result
        /// </summary>
        [Required(ErrorMessage = "UserID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserID must be greater than 0 .")]
        public int UserID { get; set; }
        /// <summary>
        /// Input exact Fine Amount that need to pay. To Know your Amount Goto GetUser API
        /// </summary>
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0 .")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Select the Type of Payment Done by the User
        /// </summary>
        [Required(ErrorMessage = "PaymentMethod is required.")]
        public TransactionTypes PaymentMethod { get; set; }
        /// <summary>
        /// Enter TransactionID for Reference
        /// </summary>
        [Required(ErrorMessage = "TransactionID is required.")]
        public string TransactionID { get; set; }
    }

    public class ByUserIDRequest
    {
        /// <summary>
        /// Input UserID(Integer value) to get required Result
        /// </summary>
        [Required(ErrorMessage = "UserID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserID must be greater than 0 .")]
        public int UserID { get; set; }
    }
    public class ByBookIDRequest
    {
        /// <summary>
        /// Input BookID(Integer value) to get required Result. To Know BookID Check on the 1st Page of Book.
        /// </summary>
        [Required(ErrorMessage = "BookID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BookID must be greater than 0 .")]
        public int BookID { get; set; }
    }
    public class ByBookNameRequest
    {
        /// <summary>
        /// Input Exact BookName to get required Result. To Know BookName Check out the Cover Page
        /// </summary>
        [Required(ErrorMessage = "BookName is required.")]
        public string BookName { get; set; }
    }
}
