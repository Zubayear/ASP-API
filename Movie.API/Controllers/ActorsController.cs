using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movie.API.Entity;
using Movie.API.Filters;
using Movie.API.Services;

namespace Movie.API.Controllers;

[ApiController]
[Route("api/actors")]
public class ActorsController : ControllerBase
{
    private readonly IActorRepository _actorRepository;
    private readonly ILogger<ActorsController> _logger;
    private readonly IMapper _mapper;

    public ActorsController(IActorRepository actorRepository,
        ILogger<ActorsController> logger, IMapper mapper)
    {
        _actorRepository = actorRepository ?? throw new ArgumentNullException(nameof(actorRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(Name = nameof(GetAllActors))]
    [ActorsResultFilter]
    public async Task<ActionResult<IEnumerable<Actor>>> GetAllActors()
    {
        _logger.LogInformation("Received ActorsController.GetActors request");
        try
        {
            var actorsFromRepo = await _actorRepository.GetActors();
            return Ok(actorsFromRepo);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsController.GetActors: {Message}", e.Message);
            return NotFound(new { e.Message });
        }
    }

    [HttpGet("{actorId}", Name = nameof(GetActor))]
    [ActorResultFilter]
    public async Task<ActionResult<Actor>> GetActor([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received ActorsController.GetActor request: {ActorId}", actorId);
        if (!TryValidateModel(actorId))
            return BadRequest(new { Message = "Actor Id not valid" });
        try
        {
            return Ok(await _actorRepository.GetActorById(actorId));
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsController.GetActor: {Message}", e.Message);
            return NotFound(new { e.Message });
        }
    }

    [HttpPost(Name = nameof(CreateActor))]
    [ActorResultFilter]
    public async Task<ActionResult<Actor>> CreateActor(Models.ActorForCreation actorForCreation)
    {
        _logger.LogInformation("Received ActorsController.CreateActor request: {Actor}", actorForCreation);
        if (!TryValidateModel(actorForCreation))
            return BadRequest(new { Message = "Invalid actor to create" });
        var actorToSave = _mapper.Map<Actor>(actorForCreation);
        try
        {
            var savedActor = await _actorRepository.SaveActor(actorToSave);
            return CreatedAtRoute(nameof(GetActor), new { actorId = savedActor.Id }, savedActor);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsController.GetActor: {Message}", e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { e.Message });
        }
    }

    [HttpDelete("{actorId}", Name = nameof(RemoveActor))]
    public async Task<ActionResult> RemoveActor([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received ActorsController.RemoveActor request: {AuthorId}", actorId);
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
            _logger.LogError("Error Occurred ActorsController.RemoveActor: {Message}", e.Message);
            return NotFound(new { e.Message });
        }

        return NoContent();
    }

    [HttpPut("{actorId}", Name = nameof(FullUpdateActor))]
    [ActorResultFilter]
    public async Task<ActionResult<Actor>> FullUpdateActor([FromRoute] Guid actorId,
        Models.ActorForUpdate actorForUpdate)
    {
        _logger.LogInformation("Received ActorsController.FullUpdateActor request: {AuthorId}; {ActorUpdate}", actorId,
            actorForUpdate);
        if (!TryValidateModel(actorId) && !await TryUpdateModelAsync(actorForUpdate))
            return BadRequest(new { Message = "Actor Id or Actor invalid" });
        try
        {
            var actorFromRepo = await _actorRepository.GetActorById(actorId);
            _mapper.Map(actorForUpdate, actorFromRepo);
            _actorRepository.UpdateActor(actorFromRepo);
            var isUpdated = await _actorRepository.SaveChanges();
            if (!isUpdated)
                throw new InvalidOperationException(nameof(isUpdated));
            return Ok(actorFromRepo);
        }
        catch (Exception e)
        {
            _logger.LogError("Error Occurred ActorsController.FullUpdateActor: {Message}", e.Message);
            return NotFound(new { e.Message });
        }
    }
}