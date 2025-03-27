using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        public double TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int TableId { get; set; }

        [Required]
        public int? CustomerUserId { get; set; }

        [ForeignKey("TableId")]
        public Table Table { get; set; }

        [ForeignKey("CustomerUserId")]
        public User? CustomerUser { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }=new List<OrderItem>();
    }
}
