namespace Bookstore.DTO
{
    public class BookRequestDataDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ISBN { get; set; }
        public int RequestedQuantity { get; set; }
        public int OrderID { get; set; }
    }
}
