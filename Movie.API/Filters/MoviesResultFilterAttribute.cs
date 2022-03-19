using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movie.API.Filters;

public class MoviesResultFilterAttribute : ResultFilterAttribute
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
        if (resultFromAction != null)
            resultFromAction.Value = mapper.Map<IEnumerable<Models.Movie>>(resultFromAction.Value);
        await next();
    }
}