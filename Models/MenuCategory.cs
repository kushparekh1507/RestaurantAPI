using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestaurantAPI.Models
{
    public class MenuCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuCategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string Description {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int MenuId { get; set; }

        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }

        [Required]
        public int RestaurantId {  get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        
        public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
