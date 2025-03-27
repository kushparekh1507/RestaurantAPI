namespace RestaurantAPI.DTO
{
    public class MenuCategoryRequest
    {
        public int MenuCategoryId {  get; set; }

        public string CategoryName { get; set; }

        public int RestaurantId { get; set; }
    }
}
