using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Table
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TableId { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        public Boolean HasActiveOrder { get; set; } = false;

        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;

        [Required]
        public int TableTypeId { get; set; }

        [ForeignKey("TableTypeId")]
        public TableType TableType {  get; set; }

        [Required]
        public int RestaurantId {  get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
