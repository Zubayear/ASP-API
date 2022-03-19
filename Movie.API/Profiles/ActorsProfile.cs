using AutoMapper;

namespace Movie.API.Profiles;

public class ActorsProfile : Profile
{
    public ActorsProfile()
    {
        CreateMap<Entity.Actor, Models.Actor>().ReverseMap();
        CreateMap<Models.ActorForCreation, Entity.Actor>().ReverseMap();
        CreateMap<Entity.Actor, Models.ActorForUpdate>().ReverseMap();
    }
}