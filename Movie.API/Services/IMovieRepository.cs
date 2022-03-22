namespace Movie.API.Services;

public interface IMovieRepository
{
    Task<Entity.Movie> SaveMovieWithActor(Guid actorId, Entity.Movie movie);
    Task<IEnumerable<Entity.Movie>> GetMoviesForActor(Guid actorId);
    Task<bool> SaveChanges();
    Task<Entity.Movie> GetMovieById(Guid movieId);
    Task<bool> ActorExists(Guid actorId);
}