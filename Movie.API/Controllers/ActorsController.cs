using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetAllActors()
    {
        _logger.LogInformation("Received ActorsController.GetActors request");
        return Ok(await _actorRepository.GetActors());
    }

    [HttpGet("{actorId}", Name = nameof(GetActor))]
    [ActorResultFilter]
    public async Task<IActionResult> GetActor([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received ActorsController.GetActor request: {ActorId}", actorId);
        return Ok(await _actorRepository.GetActorById(actorId));
    }

    [HttpPost(Name = nameof(CreateActor))]
    [ActorResultFilter]
    public async Task<IActionResult> CreateActor(Models.ActorForCreation actorForCreation)
    {
        _logger.LogInformation("Received ActorsController.CreateActor request: {@Author}", actorForCreation);
        var actorToSave = _mapper.Map<Entity.Actor>(actorForCreation);
        await _actorRepository.SaveActor(actorToSave);
        var isSaved = await _actorRepository.SaveChanges();
        if (!isSaved)
        {
            throw new InvalidOperationException(nameof(isSaved));
        }

        return CreatedAtRoute(nameof(GetActor), new { actorId = actorToSave.Id }, actorToSave);
    }

    [HttpDelete("{actorId}", Name = nameof(RemoveActor))]
    public async Task<IActionResult> RemoveActor([FromRoute] Guid actorId)
    {
        _logger.LogInformation("Received ActorsController.RemoveActor request: {AuthorId}", actorId);
        await _actorRepository.DeleteActor(actorId);
        var isDeleted = await _actorRepository.SaveChanges();
        if (!isDeleted) throw new InvalidOperationException(nameof(isDeleted));
        return NoContent();
    }

    [HttpPut("{actorId}", Name = nameof(FullUpdateActor))]
    [ActorResultFilter]
    public async Task<IActionResult> FullUpdateActor([FromRoute] Guid actorId, Models.ActorForUpdate actorForUpdate)
    {
        _logger.LogInformation("Received ActorsController.FullUpdateActor request: {AuthorId}; {@ActorUpdate}", actorId,
            actorForUpdate);
        var actorFromRepo = await _actorRepository.GetActorById(actorId);
        _mapper.Map(actorForUpdate, actorFromRepo);
        _actorRepository.UpdateActor(actorFromRepo);
        var isUpdated = await _actorRepository.SaveChanges();
        if (!isUpdated) throw new InvalidOperationException(nameof(isUpdated));
        return Ok(actorFromRepo);
    }
}