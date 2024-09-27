using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
        public DbSet<BookRequestHead> BooksRequestHead { get; set; }
        public DbSet<BookRequestData> BooksRequestData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookRequestData>()
                .HasOne(x => x.BookRequestHead)
                .WithMany(x => x.BookRequestData)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
