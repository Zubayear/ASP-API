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
    public async Task<IActionResult> GetMovie([FromRoute] Guid movieId)
    {
        _logger.LogInformation("Received MoviesController.GetMovie request: {MovieId}", movieId);
        if (movieId == Guid.Empty) throw new InvalidEnumArgumentException(nameof(movieId));
        return Ok(await _movieRepository.GetMovieById(movieId));
    }

    [HttpPost(Name = nameof(CreateMovieWithActor))]
    [MovieResultFilterAttribute]
    public async Task<IActionResult> CreateMovieWithActor([FromRoute] Guid actorId,
        Models.MovieForCreation movieForCreation)
    {
        _logger.LogInformation("Received MoviesController.CreateMovieWithActor request: {ActorId}, {@Movie}", actorId,
            movieForCreation);
        if (!await _movieRepository.ActorExists(actorId)) 
            return NotFound(new { Message = "Actor Not Found" });
        var movieToSave = _mapper.Map<Entity.Movie>(movieForCreation);
        await _movieRepository.SaveMovieWithActor(actorId, movieToSave);
        if (!await _movieRepository.SaveChanges()) return NoContent();
        var movieToReturn = _mapper.Map<Models.Movie>(movieToSave);
        return CreatedAtRoute(nameof(GetMovie), new { actorId, movieId = movieToSave.Id }, movieToReturn);
    }

    [HttpGet(Name = nameof(GetMoviesByAuthorId))]
    [MoviesResultFilterAttribute]
    public async Task<IActionResult> GetMoviesByAuthorId([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received MoviesController.GetMoviesByAuthorId request: {ActorId}", actorId);
        if (!await _movieRepository.ActorExists(actorId)) 
            return NotFound(new { Message = "Actor Not Found" });
        var moviesFromRepo = await _movieRepository.GetMoviesForActor(actorId);
        return Ok(moviesFromRepo);
    }
}