using Movie.API.Entity;

namespace Movie.API.Services;

public interface IActorRepository
{
    Task<Actor> GetActorById(Guid actorId);
    Task<Actor> SaveActor(Actor actor);
    Task DeleteActor(Guid actorId);
    void UpdateActor(Actor actor);
    Task<IEnumerable<Actor>> GetActors();
    Task<bool> SaveChanges();
}