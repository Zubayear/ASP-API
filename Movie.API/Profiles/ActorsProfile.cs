using AutoMapper;

namespace Movie.API.Profiles;

public class ActorsProfile : Profile
{
    public ActorsProfile()
    {
        CreateMap<Entity.Actor, Models.Actor>().ReverseMap();
        CreateMap<Entity.Actor, Models.ActorForCreation>().ReverseMap();
        CreateMap<Entity.Actor, Models.ActorForUpdate>().ReverseMap();
    }
}