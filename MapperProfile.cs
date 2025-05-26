using AutoMapper;
using RestaurantAPI.DTO;
using RestaurantAPI.Models;

namespace RestaurantAPI
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RestaurantRequest, Restaurant>();

            CreateMap<MenuCategoryRequest, MenuCategory>();
        }
    }
}
