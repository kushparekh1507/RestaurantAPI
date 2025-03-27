using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId {  get; set; }

        [Required]
        public int Quantity {  get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Status {  get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int? ItemId {  get; set; }

        [ForeignKey("OrderId")]
        public Order Order {  get; set; }

        [ForeignKey("ItemId")]
        public MenuItem? MenuItem { get; set; }
    }
}
