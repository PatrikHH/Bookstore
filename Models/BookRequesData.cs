namespace Bookstore.Models
{
    public class BookRequestData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ISBN { get; set; }
        public int RequestedQuantity { get; set; }
        public int OrderId { get; set; }
        public virtual BookRequestHead BookRequestHead { get; set; }
    }
}
