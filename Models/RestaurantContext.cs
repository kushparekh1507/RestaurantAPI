using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Models
{
    public partial class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Restaurant> Restaurant { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Table> Tables { get; set; }

        public virtual DbSet<MenuCategory> MenuCategories { get; set; }

        public virtual DbSet<MenuItem> MenuItem { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public DbSet<RestaurantAPI.Models.Order> Order { get; set; } = default!;
        public DbSet<RestaurantAPI.Models.OrderItem> OrderItem { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasOne(t => t.Restaurant)
                .WithMany(r => r.Tables)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MenuCategory>()
                .HasOne(m => m.Restaurant)
                .WithMany(a => a.MenuCategories)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.MenuCategory)
                .WithMany(i => i.MenuItems)
                .HasForeignKey(a => a.MenuCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithMany(u => u.Users)
                .HasForeignKey(a => a.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(a => a.Role)
                .WithMany(a => a.Users)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(a => a.Table)
                .WithMany(o => o.Orders)
                .HasForeignKey(a => a.TableId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(a => a.CustomerUser)
                .WithMany(o => o.Orders)
                .HasForeignKey(a => a.CustomerUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItem>()
                .HasOne(a => a.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItem>()
                .HasOne(a => a.MenuItem)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>().HasData(
                    new Role { RoleId = 1, RoleName = "SuperAdmin" },
                    new Role { RoleId = 2, RoleName = "CustomerAdmin" },
                    new Role { RoleId = 3, RoleName = "CustomerUser" }
                );

            string hashedPassword = "$2a$11$OZsGF7lj/fx2uKwVlDKUCu6yugoftyY0FaSbLD8gIrH0DHrzonZ5q";

            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        UserId = 1,
                        FullName = "Super Admin",
                        Email = "kushparekh943@gmail.com",
                        Password = hashedPassword,
                        IsFirstLogin = false,
                        RoleId = 1
                    }
                );

        }
        
    }
}
