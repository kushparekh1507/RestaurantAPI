using Microsoft.EntityFrameworkCore;

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

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasOne(t => t.Restaurant)
                .WithMany(r => r.Tables)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuCategory>()
                .HasOne(m => m.Restaurant)
                .WithMany(a => a.MenuCategories)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.MenuCategory)
                .WithMany(i => i.MenuItems)
                .HasForeignKey(a => a.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithMany(u => u.Users)
                .HasForeignKey(a => a.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(a => a.Role)
                .WithMany(a => a.Users)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(a=>a.Table)
                .WithMany(o=>o.Orders)
                .HasForeignKey(a => a.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(a=>a.CustomerUser)
                .WithMany(o=>o.Orders)
                .HasForeignKey(a => a.CustomerUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderItem>()
                .HasOne(a=>a.Order)
                .WithMany(o=> o.OrderItems)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(a=>a.MenuItem)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
