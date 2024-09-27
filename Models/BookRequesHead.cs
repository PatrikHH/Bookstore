namespace Bookstore.Models
{
    public class BookRequestHead
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string Status { get; set; }
        public ICollection<BookRequestData> BookRequestData { get; set; }
    }
}
