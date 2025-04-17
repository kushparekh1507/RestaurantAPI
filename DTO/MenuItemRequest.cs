namespace RestaurantAPI.DTO
{
    public class MenuItemRequest
    {
        public int MenuItemId { get; set; }
        public string ItemName {  get; set; }

        public double Price {  get; set; }

        public string ImageUrl { get; set; }

        public int MenuCategoryId { get; set; }
    }
}
