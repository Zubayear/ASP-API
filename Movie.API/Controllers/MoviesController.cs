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

    [HttpGet("{movieId}", Name = nameof(GetMovie))]
    [MovieResultFilter]
    public async Task<ActionResult<Entity.Movie>> GetMovie([FromRoute] Guid movieId)
    {
        _logger.LogInformation("Received MoviesController.GetMovie request: {MovieId}", movieId);
        if (movieId == Guid.Empty) 
            throw new InvalidEnumArgumentException(nameof(movieId));
        return Ok(await _movieRepository.GetMovieById(movieId));
    }

    [HttpPost(Name = nameof(CreateMovieWithActor))]
    [MovieResultFilterAttribute]
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

    [HttpGet(Name = nameof(GetMoviesByAuthorId))]
    [MoviesResultFilterAttribute]
    public async Task<ActionResult<IEnumerable<Entity.Movie>>> GetMoviesByAuthorId([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received MoviesController.GetMoviesByAuthorId request: {ActorId}", actorId);
        if (!await _movieRepository.ActorExists(actorId))
            throw new InvalidOperationException(nameof(actorId));

        var moviesFromRepo = await _movieRepository.GetMoviesForActor(actorId);

        return Ok(moviesFromRepo);
    }
}