using AutoMapper;
using test_crud.Entities;
using test_crud.Models;

namespace test_crud.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Users, UserDetailsDto>();
            CreateMap<Users, UserDetailsWithTokenDto>();
            CreateMap<RegisterDto, Users>();
        }
    }
}
