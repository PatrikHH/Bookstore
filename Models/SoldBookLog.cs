namespace Bookstore.Models
{
    public class SoldBookLog
    {
        public int Id { get; set; }
        Person Person { get; set; }
        BookInStore Book { get; set; }
        DateTime DateTimeSelling{ get; set; }
    }
}
