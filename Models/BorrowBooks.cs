
﻿namespace LibraryManagementSystem.Models
{
    public class BorrowBooks
    {
        public int BorrowID { get; set; }
        public int BookID { get; set; }
        public int UserID { get; set; }
        public string BookName { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;  // Default to current date
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }  // Nullable since the book may not have been returned yet
        public int RenewalCount { get; set; } = 0;  // Default value is 0
        public int FineAmount { get; set; } = 0;  // Default value is 0
        public bool? IsFinePaid { get; set; } // Default value is 0
    }
}
