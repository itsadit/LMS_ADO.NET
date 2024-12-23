using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models.Enum;

namespace LibraryManagementSystem.Models.DTO
{
    public class AddBookRequest
    {
        [Required(ErrorMessage = "BookName is required.")]
        [StringLength(100, ErrorMessage = "BookName cannot exceed 100 characters.")]
        public string BookName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "BookPrice must be greater than 0.")]
        public decimal BookPrice { get; set; }

        [Required(ErrorMessage = "Authors are required.")]
        [StringLength(500, ErrorMessage = "Authors cannot exceed 500 characters.")]
        public string Authors { get; set; }

        [Required(ErrorMessage = "Genres are required.")]
        [StringLength(500, ErrorMessage = "Genres cannot exceed 500 characters.")]
        public string Genres { get; set; }

        [Required(ErrorMessage = "Publishers are required.")]
        [StringLength(500, ErrorMessage = "Publishers cannot exceed 500 characters.")]
        public string Publishers { get; set; }
    }

    public class UpdateBookRequest
    {
        [StringLength(100, ErrorMessage = "BookName cannot exceed 100 characters.")]
        public string? BookName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "BookPrice must be greater than 0.")]
        public decimal? BookPrice { get; set; }

        [StringLength(500, ErrorMessage = "AuthorName cannot exceed 500 characters.")]
        public string? AuthorName { get; set; }

        [StringLength(500, ErrorMessage = "GenreName cannot exceed 500 characters.")]
        public string? GenreName { get; set; }

        [StringLength(500, ErrorMessage = "PublisherName cannot exceed 500 characters.")]
        public string? PublisherName { get; set; }
    }

    public class SearchBooksRequest
    {
        [Required(ErrorMessage = "Search criteria is required.")]
        public SearchBy SearchBy { get; set; }  // No StringLength here, it's an enum.

        [Required(ErrorMessage = "Search value is required.")]
        [StringLength(500, ErrorMessage = "SearchValue cannot exceed 500 characters.")]
        public string SearchValue { get; set; }
    }
}
