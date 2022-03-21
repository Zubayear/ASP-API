using Microsoft.EntityFrameworkCore;
using Movie.API.Context;
using Actor = Movie.API.Entity.Actor;

namespace Movie.API.Services;

public class ActorRepository : IActorRepository, IDisposable
{
    private readonly MovieContext _movieContext;
    private readonly ILogger<ActorRepository> _logger;

    public ActorRepository(MovieContext movieContext, ILogger<ActorRepository> logger)
    {
        _movieContext = movieContext ?? throw new ArgumentNullException(nameof(movieContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Actor> GetActorById(Guid actorId)
    {
        _logger.LogInformation("Actor id: {ActorId}", actorId);
        if (actorId == Guid.Empty)
            throw new ArgumentNullException(nameof(actorId));
        if (_movieContext.Actors == null)
            throw new InvalidOperationException(nameof(_movieContext.Actors));
        return await _movieContext.Actors.FindAsync(actorId) ??
               throw new InvalidOperationException(nameof(_movieContext.Actors));
    }

    public async Task<Actor> SaveActor(Actor actor)
    {
        _logger.LogInformation("Actor to save in db: {Actor}", actor);
        if (actor == null)
            throw new ArgumentNullException(nameof(actor));
        if (_movieContext.Actors == null)
            throw new ArgumentNullException(nameof(_movieContext.Actors));
        await _movieContext.Actors.AddAsync(actor);
        var isSaved = await SaveChanges();
        if (!isSaved)
            throw new InvalidOperationException(nameof(actor));
        return actor;
    }

    public async Task DeleteActor(Guid actorId)
    {
        _logger.LogInformation("Actor to delete in db: {ActorId}", actorId);
        if (actorId == Guid.Empty)
            throw new ArgumentNullException(nameof(actorId));
        Actor actor;
        try
        {
            actor = await GetActorById(actorId);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorRepository.DeleteActor: {Message}", e.Message);
            throw new Exception("Could not find the actor");
        }

        _movieContext.Remove(actor);
    }

    public void UpdateActor(Actor actor)
    {
        _logger.LogInformation("Actor to update in db: {Actor}", actor);
        if (actor == null)
            throw new ArgumentNullException(nameof(actor));
        _movieContext.Entry(actor).State = EntityState.Modified;
    }

    public async Task<IEnumerable<Actor>> GetActors()
    {
        _logger.LogInformation("Get all actors from repo");
        _movieContext.Actors = _movieContext.Actors ?? throw new InvalidOperationException(nameof(_movieContext));
        return await _movieContext.Actors.ToListAsync();
    }

    public async Task<bool> SaveChanges()
    {
        return await _movieContext.SaveChangesAsync() > 0;
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _movieContext.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}