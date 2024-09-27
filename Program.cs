using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                //options.UseSqlServer(builder.Configuration.GetConnectionString("BookstoreDbConnection"));
                options.UseSqlServer(builder.Configuration.GetConnectionString("MonsterAspDbConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);  //poresi s trakovanim pri updatu
            });
            builder.Services.AddScoped<BaseService>();
            builder.Services.AddScoped<PersonService>();
            builder.Services.AddScoped<WarehouseService>();
            builder.Services.AddScoped<BookOnTheWayService>();
            builder.Services.AddScoped<StoreManagementService>();
            builder.Services.AddScoped<StoreService>();
            builder.Services.AddScoped<BooksSectionService>();
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(x =>
            {
                x.Password.RequiredLength = 10;
                x.Password.RequireLowercase = true;
                x.Password.RequireUppercase = true;
                x.Password.RequireDigit = true;
                x.Password.RequireNonAlphanumeric = true;
            });
            builder.Services.ConfigureApplicationCookie(x =>
            {
                x.Cookie.Name = ".AspNetCore.Identity.Application";
                x.ExpireTimeSpan = TimeSpan.FromMinutes(100);
                x.SlidingExpiration = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
