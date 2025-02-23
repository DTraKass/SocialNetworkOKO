using AutoMapper;
using SocialNetworkOKO.Models;

namespace SocialNetworkOKO.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
        .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.FirstName));
            CreateMap<LoginViewModel, User>()
        .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email));
            CreateMap<UserEditViewModel, User>().ReverseMap();
        }
    }
}
