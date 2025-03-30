namespace Bookstore.DTO
{
    public class BookInWarehouseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public int ISBN { get; set; }
        public int Quantity { get; set; }
        public int SelectQuantity { get; set; }    
    }
}
