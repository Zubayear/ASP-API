using System.ComponentModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movie.API.Filters;
using Movie.API.Services;

namespace Movie.API.Controllers;

[ApiController]
[Route("api/actors/{actorId}/movies/")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<MoviesController> _logger;
    private readonly IMapper _mapper;

    public MoviesController(IMovieRepository movieRepository,
        ILogger<MoviesController> logger, IMapper mapper)
    {
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Returns a movie by movieId
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns>Returns a movie by movieId</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <response code="200">If a movie returned successfully</response>
    /// <response code="400">If the movieId is wrong</response>
    [HttpGet("{movieId}", Name = nameof(GetMovie))]
    [MovieResultFilter]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Entity.Movie>> GetMovie([FromRoute] Guid movieId)
    {
        _logger.LogInformation("Received MoviesController.GetMovie request: {MovieId}", movieId);
        if (movieId == Guid.Empty)
            throw new InvalidEnumArgumentException(nameof(movieId));
        return Ok(await _movieRepository.GetMovieById(movieId));
    }

    /// <summary>
    /// Returns a newly created
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="movieForCreation"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <response code="201">If a movie is created successfully</response>
    /// <response code="400">If actorId is wrong</response>
    /// <response code="404">If actor doesn't exists</response>
    [HttpPost(Name = nameof(CreateMovieWithActor))]
    [MovieResultFilterAttribute]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Entity.Movie>> CreateMovieWithActor([FromRoute] Guid actorId,
        Models.MovieForCreation movieForCreation)
    {
        _logger.LogInformation("Received MoviesController.CreateMovieWithActor request: {ActorId}, {Movie}", actorId,
            movieForCreation);

        if (Guid.Empty == actorId || movieForCreation == null)
            throw new InvalidEnumArgumentException(nameof(actorId));
        // return BadRequest(new { Message = "Actor Id or Movie request is invalid" });

        if (!await _movieRepository.ActorExists(actorId))
            throw new InvalidEnumArgumentException(nameof(actorId));
        // return NotFound(new { Message = "Actor Not Found" });

        var movieToSave = _mapper.Map<Entity.Movie>(movieForCreation);

        var savedMovie = await _movieRepository.SaveMovieWithActor(actorId, movieToSave);

        return CreatedAtRoute(nameof(GetMovie), new { actorId, movieId = savedMovie.Id }, savedMovie);
    }

    /// <summary>
    /// Returns a list of movies with given actorId
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <response code="200">If a movie returned successfully</response>
    /// <response code="400">If actorId is wrong</response>
    /// <response code="404">If actor doesn't exists</response>
    [HttpGet(Name = nameof(GetMoviesByAuthorId))]
    [MoviesResultFilterAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Entity.Movie>>> GetMoviesByAuthorId([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received MoviesController.GetMoviesByAuthorId request: {ActorId}", actorId);
        if (!await _movieRepository.ActorExists(actorId))
            throw new InvalidOperationException(nameof(actorId));

        var moviesFromRepo = await _movieRepository.GetMoviesForActor(actorId);

        return Ok(moviesFromRepo);
    }

    /// <summary>
    /// Remove a movie with actorId and movieId
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="movieId"></param>
    /// <returns></returns>
    /// <response code="204">If movie is deleted successfully</response>
    [HttpDelete("{movieId}", Name = nameof(RemoveMovieWithActorId))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveMovieWithActorId(Guid actorId, Guid movieId)
    {
        await _movieRepository.DeleteMovieWithActor(actorId, movieId);
        var isDeleted = await _movieRepository.SaveChanges();
        if (!isDeleted)
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "failed deleting movie" });
        return NoContent();
    }
}