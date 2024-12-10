namespace LibraryAPI.DataAccess.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public decimal BookPrice { get; set; }
        public string Authors { get; set; }
        public string Genres { get; set; }
        public string Publishers { get; set; }
    }
}
