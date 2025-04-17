namespace RestaurantAPI.DTO
{
    public class TableRequest
    {
        public int TableId {  get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }
        public int RestaurantId { get; set; }
    }
}
