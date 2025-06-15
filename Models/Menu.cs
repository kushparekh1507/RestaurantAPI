using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }

        [Required]
        public string MenuName { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        [Required]
        public int TableTypeId { get; set; }

        [ForeignKey("TableTypeId")]
        public TableType TableType { get; set; }

        public ICollection<MenuCategory> Categories { get; set; }

        public ICollection<WaiterMenu> WaiterMenus { get; set; }= new List<WaiterMenu>();
    }
}
