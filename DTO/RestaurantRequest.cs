using RestaurantAPI.ENUM;

namespace RestaurantAPI.DTO
{
    public class RestaurantRequest
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string MobileNo { get; set; }

        public RestaurantStatus Status { get; set; } = RestaurantStatus.Pending;
    }
}
