namespace Bookstore.Models
{
    public class BookInStore
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public int ISBN { get; set; }
        public int Quantity { get; set; }
        public string? StorePlacement { get; set; }
        public string? BooksSection { get; set; }
        public int Price { get; set; }
    }
}
