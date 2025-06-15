namespace RestaurantAPI.DTO
{
    public class MenuRequest
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public int RestaurantId { get; set; }

        public int TableTypeId { get; set; }
    }
}
