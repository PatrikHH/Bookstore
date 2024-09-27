namespace Bookstore.DTO
{
    public class BookOnTheWayDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int ISBN { get; set; }
        public int Quantity { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public bool IsSent { get; set; }
        public int ContainerId { get; set; }
    }
}
