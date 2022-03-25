using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movie.API.Models;

namespace Movie.API.Filters;

public class ActorResultFilterAttribute : ResultFilterAttribute
{
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var resultFromAction = context.Result as ObjectResult;
        if (resultFromAction?.Value == null || resultFromAction.StatusCode < 200 ||
            resultFromAction.StatusCode >= 300)
        {
            await next();
        }

        var mapper = context.HttpContext.RequestServices.GetRequiredService<IMapper>();

        // storing the odj with links
        var expandoObjectWithLinks = resultFromAction.Value;
        if (resultFromAction != null)
            resultFromAction.Value = mapper.Map<Actor>(resultFromAction.Value);
        resultFromAction.Value = expandoObjectWithLinks;
        await next();
    }
}