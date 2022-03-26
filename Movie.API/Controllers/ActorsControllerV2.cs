using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Movie.API.Extensions;
using Movie.API.Models;
using Movie.API.Services;

namespace Movie.API.Controllers;

[ApiVersion("2.0")]
[ApiController]
[Route("api/actors", Order = 2)]
[Produces("application/json", "application/xml", "application/vnd.drill.hateoas+json")]
[Consumes("application/json", "application/json-patch+json", "application/*+json")]
// [ResponseCache(CacheProfileName = "240SecCP")]
[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
[HttpCacheValidation(MustRevalidate = true)]
public class ActorsControllerV2 : ControllerBase
{
    private readonly IActorRepository _actorRepository;
    private readonly ILogger<ActorsControllerV2> _logger;
    private readonly IMapper _mapper;

    public ActorsControllerV2(IActorRepository actorRepository,
        ILogger<ActorsControllerV2> logger, IMapper mapper)
    {
        _actorRepository = actorRepository ?? throw new ArgumentNullException(nameof(actorRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Get all actors
    /// </summary>
    /// <returns>Returns all actors</returns>
    /// <response code="200">Returns all Actors</response>
    /// <response code="404">If actors not found</response>
    [HttpGet(Name = nameof(GetAllActors))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ResponseCache(Duration = 120)]
    public async Task<ActionResult<IEnumerable<Actor>>> GetAllActors(
        [FromHeader(Name = "Accept")] string mediaType,
        [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.GetActors request");
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            return BadRequest(new { Message = "Wrong Accept Header" });
        try
        {
            var actorsFromRepo = await _actorRepository.GetActors();
            if (parsedMediaType.MediaType != "application/vnd.drill.hateoas+json")
                return Ok(_mapper.Map<IEnumerable<Actor>>(actorsFromRepo));
            var shapedActors = _mapper.Map<IEnumerable<Actor>>(actorsFromRepo)
                .ShapeData("");
            var shapedActorWithLinks = shapedActors.Select(actor =>
            {
                var actorAsDictionary = actor as IDictionary<string, object>;
                var actorLinks = CreateLinksForActor((Guid)actorAsDictionary["Id"]);
                actorAsDictionary.Add("links", actorLinks);
                return actorAsDictionary;
            });
            var response = new
            {
                values = shapedActorWithLinks,
                total = actorsFromRepo.Count()
            };
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsControllerV2.GetActors: {Message}", e.Message);
            return NotFound(new { Message = "failed getting all actors" });
        }
    }

    /// <summary>
    /// Return an actor with actorId
    /// </summary>
    /// <param name="actorId">Returns Actor with actorId</param>
    /// <returns></returns>
    /// <response code="200">Returns an actor with actorId</response>
    /// <response code="400">If actorId is wrong</response>
    [HttpGet("{actorId}", Name = nameof(GetActor))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 900)]
    [HttpCacheValidation(MustRevalidate = false)]
    public async Task<IActionResult> GetActor([FromRoute] Guid actorId,
        [FromHeader(Name = "Accept")] string mediaType,
        [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.GetActor request: {ActorId}", actorId);
        if (!TryValidateModel(actorId))
            return BadRequest(new { Message = "Actor Id not valid" });
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            return BadRequest(new { Message = "Wrong Accept Header" });
        try
        {
            var actorFromRepo = await _actorRepository.GetActorById(actorId);
            if (parsedMediaType.MediaType != "application/vnd.drill.hateoas+json")
                return Ok(_mapper.Map<Actor>(actorFromRepo));
            var links = CreateLinksForActor(actorId);
            var response = _mapper.Map<Actor>(actorFromRepo).ShapeDataForActor("") as IDictionary<string, object>;
            response.Add("links", links);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsControllerV2.GetActor: {Message}", e.Message);
            return NotFound(new { Message = "failed getting actor" });
        }
    }

    /// <summary>
    /// Creates an actor
    /// </summary>
    /// <param name="actorForCreation"></param>
    /// <returns>A newly created Actor</returns>
    /// <response code="201">Returns the newly created Actor</response>
    /// <response code="400">If the actorForCreation is null</response>
    /// <response code="500">If the db is down or problem with saving in db</response>
    [HttpPost(Name = nameof(CreateActor))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Actor>> CreateActor(ActorForCreation actorForCreation,
        [FromHeader(Name = "Accept")] string mediaType,
        [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.CreateActor request: {Actor}", actorForCreation);
        if (!TryValidateModel(actorForCreation))
            return BadRequest(new { Message = "Invalid actor to create" });
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            return BadRequest(new { Message = "Wrong Accept Header" });
        try
        {
            var savedActor = await _actorRepository.SaveActor(_mapper.Map<Entity.Actor>(actorForCreation));
            if (parsedMediaType.MediaType != "application/vnd.drill.hateoas+json")
                return Ok(_mapper.Map<Actor>(savedActor));
            var response = _mapper.Map<Actor>(savedActor).ShapeDataForActor("") as IDictionary<string, object>;
            var links = CreateLinksForActor(savedActor.Id);
            response.Add("links", links);
            return CreatedAtRoute(nameof(GetActor), new { actorId = savedActor.Id }, response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsControllerV2.GetActor: {Message}", e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "failed creating actor" });
        }
    }

    /// <summary>
    /// Remove an actor with actorId
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <response code="204">If actor return successfully</response>
    /// <response code="400">If actorId is wrong</response>
    [HttpDelete("{actorId}", Name = nameof(RemoveActor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveActor([FromRoute] Guid actorId,
        [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.RemoveActor request: {AuthorId}", actorId);
        if (!TryValidateModel(actorId))
            return BadRequest(new { Message = $"{actorId} not found" });
        try
        {
            await _actorRepository.DeleteActor(actorId);
            var isDeleted = await _actorRepository.SaveChanges();
            if (!isDeleted)
                throw new InvalidOperationException(nameof(isDeleted));
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsControllerV2.RemoveActor: {Message}", e.Message);
            return NotFound(new { Message = "failed removing actor" });
        }

        return NoContent();
    }

    /// <summary>
    /// Returns updated actor with actorId
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="actorForUpdate"></param>
    /// <returns>Returns updated actor with actorId</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <response code="200">Updated actor</response>
    /// <response code="400">If actorId is wrong</response>
    /// <response code="404">If actor not found with actorId</response>
    [HttpPut("{actorId}", Name = nameof(FullUpdateActor))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Actor>> FullUpdateActor([FromRoute] Guid actorId,
        [FromHeader(Name = "Accept")] string mediaType,
        ActorForUpdate actorForUpdate, [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.FullUpdateActor request: {AuthorId}; {ActorUpdate}",
            actorId,
            actorForUpdate);
        if (!TryValidateModel(actorId) && !TryValidateModel(actorForUpdate))
            return BadRequest(new { Message = "Actor Id or Actor invalid" });
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            return BadRequest(new { Message = "Wrong Accept Header" });
        try
        {
            var actorFromRepo = await _actorRepository.GetActorById(actorId);
            _mapper.Map(actorForUpdate, actorFromRepo);
            _actorRepository.UpdateActor(actorFromRepo);
            var isUpdated = await _actorRepository.SaveChanges();
            if (!isUpdated)
                throw new InvalidOperationException(nameof(isUpdated));
            if (parsedMediaType.MediaType != "application/vnd.drill.hateoas+json")
                return Ok(_mapper.Map<Actor>(actorFromRepo));
            var links = CreateLinksForActor(actorId);
            var response = _mapper.Map<Actor>(actorFromRepo).ShapeDataForActor("") as IDictionary<string, object>;
            response.Add("links", links);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsControllerV2.FullUpdateActor: {Message}", e.Message);
            return NotFound(new { Message = "failed updating actor" });
        }
    }

    /// <summary>
    /// Returns updated actor
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="patchDocument"></param>
    /// <returns></returns>
    /// <response code="204">If actor is updated partially</response>
    [HttpPatch("{actorId}", Name = nameof(PartiallyUpdateActor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<Actor>> PartiallyUpdateActor(
        Guid actorId,
        JsonPatchDocument<ActorForUpdate> patchDocument,
        [FromHeader(Name = "API-Version")] string apiVersion = "1")
    {
        _logger.LogInformation("Received ActorsControllerV2.PartiallyUpdateActor request: {ActorId}", actorId);
        if (!TryValidateModel(patchDocument))
            return BadRequest(new { Message = "Actor is invalid" });
        var actorFromRepo = await _actorRepository.GetActorById(actorId);
        var actorToPatch = _mapper.Map<ActorForUpdate>(actorFromRepo);
        patchDocument.ApplyTo(actorToPatch);
        if (!TryValidateModel(actorToPatch))
            return ValidationProblem(ModelState);
        _mapper.Map(actorToPatch, actorFromRepo);
        _actorRepository.UpdateActor(actorFromRepo);
        await _actorRepository.SaveChanges();
        return NoContent();
    }

    public override ActionResult ValidationProblem(
        [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        var options = HttpContext.RequestServices
            .GetRequiredService<IOptions<ApiBehaviorOptions>>();
        return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
    }

    private IEnumerable<ActorLink> CreateLinksForActor(Guid actorId)
    {
        var links = new List<ActorLink>
        {
            new(Url.Link(nameof(GetActor), new { actorId }), "self", "GET"),
            new(Url.Link(nameof(RemoveActor), new { actorId }), "delete_actor", "DELETE"),
            new(Url.Link(nameof(FullUpdateActor), new { actorId }), "update_actor", "PUT"),
            new(Url.Link(nameof(PartiallyUpdateActor), new { actorId }), "partially_update_actor", "PATCH")
        };
        return links;
    }
}