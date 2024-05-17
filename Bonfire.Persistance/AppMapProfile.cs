using AutoMapper;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;

namespace Bonfire.Persistance;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<Message, MessageResponseDto>().IncludeAllDerived();;
        
        CreateMap<User, UserResponseDto>();
        
        CreateMap<DirectChat, DirectChatResponseDto>();
    }
}