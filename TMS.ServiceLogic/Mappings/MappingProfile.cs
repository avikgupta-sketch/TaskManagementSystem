using AutoMapper;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Entities;

namespace TMS.ServiceLogic.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // RegisterRequestDto -> User
            CreateMap<RegisterRequest, User>()
             .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());



            // User -> AuthResponseDto
            CreateMap<User, AuthResponse>();
                

            
                CreateMap<CreateTaskRequest, TaskItem>()
    .ForMember(dest => dest.AssignedToUserId,
               opt => opt.MapFrom(src => src.AssignedToUserId));
            CreateMap<TaskItem, TaskResponse>()
            .ForMember(dest => dest.CreatedByUsername,
                opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.AssignedToUsername,
                    opt => opt.MapFrom(src => src.AssignedTo != null
            ? src.AssignedTo.Username : null));
            CreateMap<UpdateTaskRequest, TaskItem>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());


            CreateMap<Comment, CommentResponse>()
                .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.Username));  

            // CreateCommentRequest -> Comment
            CreateMap<CreateCommentRequest, Comment>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.TaskItemId, opt => opt.Ignore());
        }


    }
}

