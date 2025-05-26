using CloudinaryDotNet.Actions;

namespace RestaurantAPI.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder);
    }

}
