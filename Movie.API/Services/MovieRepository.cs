using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Movie.API.Context;
using Movie.API.Entity;

namespace Movie.API.Services;

public class MovieRepository : IMovieRepository, IDisposable
{
    private readonly MovieContext _movieContext;
    private readonly ILogger<MovieRepository> _logger;

    public MovieRepository(MovieContext movieContext, ILogger<MovieRepository> logger)
    {
        _movieContext = movieContext ?? throw new ArgumentNullException(nameof(movieContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveMovieWithActor(Guid actorId, Entity.Movie movie)
    {
        _logger.LogInformation("Received MovieRepository.SaveMovieWithActor request: {ActorId}, {@Movie}", actorId,
            movie);
        if (actorId == Guid.Empty) 
            throw new InvalidEnumArgumentException(nameof(actorId));
        if (movie == null) 
            throw new InvalidEnumArgumentException(nameof(movie));
        if (_movieContext.Movies == null) 
            throw new InvalidOperationException(nameof(_movieContext.Movies));
        var savedMovie = await _movieContext.Movies.AddAsync(movie);
        var actorMovie = new ActorMovie
        {
            ActorId = actorId,
            MovieId = savedMovie.Entity.Id
        };
        if (_movieContext.ActorMovies != null) await _movieContext.ActorMovies.AddAsync(actorMovie);
    }

    public async Task<IEnumerable<Entity.Movie>> GetMoviesForActor(Guid actorId)
    {
        _logger.LogInformation("Received MovieRepository.GetMoviesForActor request: {ActorId}", actorId);
        if (actorId == Guid.Empty) 
            throw new InvalidEnumArgumentException(nameof(actorId));
        if (_movieContext.Movies == null) 
            throw new InvalidOperationException(nameof(_movieContext.Movies));
        return await _movieContext.ActorMovies.Where(actor => actor.ActorId == actorId).Select(movie => movie.Movie).ToListAsync();
    }

    public async Task<bool> SaveChanges()
    {
        return await _movieContext.SaveChangesAsync() > 0;
    }

    public async Task<Entity.Movie> GetMovieById(Guid movieId)
    {
        _logger.LogInformation("Received MovieRepository.GetMovieById request: {MovieId}", movieId);
        if (movieId == Guid.Empty) throw new InvalidEnumArgumentException(nameof(movieId));
        if (_movieContext.Movies == null) throw new InvalidOperationException(nameof(_movieContext.Movies));
        return await _movieContext.Movies.FindAsync(movieId) ??
               throw new InvalidOperationException(nameof(_movieContext.Movies));
    }

    public async Task<bool> ActorExists(Guid actorId)
    {
        _logger.LogInformation("Received MovieRepository.ActorExists request: {ActorId}", actorId);
        if (actorId == Guid.Empty) throw new InvalidEnumArgumentException(nameof(actorId));
        if (_movieContext.Movies == null) throw new InvalidOperationException(nameof(_movieContext.Movies));
        return await (_movieContext.Actors ?? throw new InvalidOperationException()).AnyAsync(actor =>
            actor.Id == actorId);
    }

    protected virtual void Dispose(bool disposing)
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