using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RestaurantId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone {  get; set; }

        [Required]
        public string Address { get; set; }

        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Table> Tables { get; set; } = new List<Table>();

        public IEnumerable<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();

        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}
