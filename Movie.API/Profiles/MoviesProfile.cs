using AutoMapper;

namespace Movie.API.Profiles;

public class MoviesProfile : Profile
{
    public MoviesProfile()
    {
        CreateMap<Entity.Movie, Models.Movie>().ReverseMap();
        CreateMap<Entity.Movie, Models.MovieForCreation>().ReverseMap();
    }
}