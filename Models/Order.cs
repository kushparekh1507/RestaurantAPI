using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using RestaurantAPI.ENUM;

namespace RestaurantAPI.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.pending;

        [Required]
        public double TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int TableId { get; set; }

        [Required]
        public int? CustomerUserId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("TableId")]
        public Table Table { get; set; }

        [ForeignKey("CustomerUserId")]
        public User? CustomerUser { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
