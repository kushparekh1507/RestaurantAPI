namespace RestaurantAPI.DTO
{
    public class OrderRequest
    {
        public int OrderId { get; set; }

        public int TableId { get; set; }

        public int? CustomerUserId { get; set; }

        public List<OrderItemRequest> Items { get; set; }
    }
}
