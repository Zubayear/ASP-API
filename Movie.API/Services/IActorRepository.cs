namespace Movie.API.Services;

public interface IActorRepository
{
    Task<Entity.Actor> GetActorById(Guid actorId);
    Task<Entity.Actor> SaveActor(Entity.Actor actor);
    Task DeleteActor(Guid actorId);
    void UpdateActor(Entity.Actor actor);
    Task<IEnumerable<Entity.Actor>> GetActors();
    Task<bool> SaveChanges();
}