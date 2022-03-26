using Microsoft.AspNetCore.Mvc;
using Movie.API.Models;

namespace Movie.API.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase
{
    [HttpGet(Name = nameof(GetRoot))]
    public IActionResult GetRoot()
    {
        var actorLinks = new List<ActorLink>
        {
            
            new(Url.Link(nameof(GetRoot), new { }), "self", "GET"),
            new(Url.Link("GetAllActors", new { }), "actors", "GET"),
            new(Url.Link("CreateActor", new {  }), "create_actor", "POST"),
        };
        return Ok(actorLinks);
    }
}