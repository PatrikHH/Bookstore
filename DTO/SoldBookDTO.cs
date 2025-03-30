using Bookstore.Models;

namespace Bookstore.DTO
{
    public class SoldBookDTO
    {
        public int Id { get; set; }
        Person? Person { get; set; }
        BookInWarehouse? Book { get; set; }
        DateTime DateTimeSelling { get; set; }
    }
}
