using AutoMapper;
using ServerAPI.Models;

namespace ServerAPI.Automapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
