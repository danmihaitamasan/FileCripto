using AutoMapper;
using BusinessLogic;
using DataAccess.Models;

namespace FileCrypto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, CurrentUserDto>();
            CreateMap<CurrentUserDto, User>();
            CreateMap<RegisterModel, User>();
            CreateMap<User, UserProfileModel>()
               .ForMember(a => a.Email, a => a.MapFrom(s => s.Email))
               .ForMember(a => a.FirstName, a => a.MapFrom(s => s.FirstName))
               .ForMember(a => a.LastName, a => a.MapFrom(s => s.LastName))
               .ForMember(a => a.UserId, a => a.MapFrom(s => s.UserId))
               .ForMember(a => a.UserName, a => a.MapFrom(s => s.UserName))
               .ForMember(a => a.Password, a => a.MapFrom(s => s.Password));
            CreateMap<UserProfileModel, User>()
                .ForMember(a => a.Email, a => a.MapFrom(s => s.Email))
                .ForMember(a => a.FirstName, a => a.MapFrom(s => s.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(s => s.LastName))
                .ForMember(a => a.UserId, a => a.Ignore())
                .ForMember(a => a.Password, a => a.Ignore())
                .ForMember(a => a.UserName, a => a.MapFrom(s => s.UserName));
        }
    }
}