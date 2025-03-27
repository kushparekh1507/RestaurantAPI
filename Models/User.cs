using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? FullName { get; set; }

        public string? UserType { get; set; }  // Only for Customer User

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public int? RestaurantId { get; set; }  // Null for Super Admin

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
