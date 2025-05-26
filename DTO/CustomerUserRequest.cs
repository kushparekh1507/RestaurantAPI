namespace RestaurantAPI.DTO
{
    public class CustomerUserRequest
    {
        public int UserId { get; set; }

        public string FullName {  get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }

        public string MobileNo { get; set; }

        public int RestaurantId { get; set; }
    }
}
