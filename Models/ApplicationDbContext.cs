using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Bookstore.Services;

namespace Bookstore.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<BookInWarehouse> BooksInWarehouse { get; set; }
        public DbSet<BooksSection> BooksSections { get; set; }
        public DbSet<SoldBookLog> SoldBooks { get; set; }
        public DbSet<BookInStore> BooksInStore { get; set; }
        public DbSet<BookOnTheWay> BooksOnTheWay { get; set; }     
        public DbSet<StoreManagement> StoresManagement { get; set; }
    }
}
