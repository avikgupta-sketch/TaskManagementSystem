using AutoMapper;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Entities;

namespace TMS.ServiceLogic.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // RegisterRequestDto → User
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // User → AuthResponseDto
            CreateMap<User, AuthResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt =>
                    opt.MapFrom(src => src.Role.ToString()));
        }
    }
}

