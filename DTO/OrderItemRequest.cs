namespace RestaurantAPI.DTO
{
    public class OrderItemRequest
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public int ItemId { get; set; }
    }
}
