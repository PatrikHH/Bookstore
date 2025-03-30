namespace Bookstore.DTO
{
    public class BookInStoreDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public int ISBN { get; set; }
        public int Quantity { get; set; }
        public string? StorePlacement { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public int SelectQuantity { get; set; }
        public int BooksSectionId { get; set; }
        public string? BooksSection { get; set; }
        public int Price { get; set; }
    }
}
