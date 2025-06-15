namespace RestaurantAPI.DTO
{
    public class MenuCategoryRequest
    {
        public int MenuCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public int RestaurantId { get; set; }

        public int MenuId { get; set; }
    }
}
