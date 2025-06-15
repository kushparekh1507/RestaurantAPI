namespace RestaurantAPI.DTO
{
    public class WaiterMenuRequest
    {
        public int WaiterMenuId { get; set; }
        public int MenuId { get; set; }
        public int WaiterId { get; set; }
    }
}
