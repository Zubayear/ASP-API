using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movie.API.Context;
using Movie.API.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpCacheHeaders(options =>
{
    options.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
    options.MaxAge = 600;
}, options => { options.MustRevalidate = true; });
builder.Services.AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
        options.CacheProfiles.Add("240SecCP", new CacheProfile
        {
            Duration = 240
        });
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    })
    .AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Video API",
        Description = "An ASP.NET Core Web API for managing Video Solution",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.ResolveConflictingActions(apiDescriptions =>
    {
        var descriptions = apiDescriptions as ApiDescription[] ?? apiDescriptions.ToArray();
        var first = descriptions.First(); // build relative to the 1st method
        var parameters = descriptions.SelectMany(d => d.ParameterDescriptions).ToList();

        first.ParameterDescriptions.Clear();
        // add parameters and make them optional
        foreach (var parameter in parameters)
            if (first.ParameterDescriptions.All(x => x.Name != parameter.Name))
            {
                first.ParameterDescriptions.Add(new ApiParameterDescription
                {
                    ModelMetadata = parameter.ModelMetadata,
                    Name = parameter.Name,
                    ParameterDescriptor = parameter.ParameterDescriptor,
                    Source = parameter.Source,
                    IsRequired = false,
                    DefaultValue = null
                });
            }

        return first;
    });
});

builder.Services.Configure<MvcOptions>(options =>
{
    var newtonsoftJsonOutputFormatter =
        options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
    newtonsoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.drill.hateoas+json");
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("API-Version");
});

builder.Services.AddResponseCaching();

// Custom Configuration starts here
builder.Services.AddDbContext<MovieContext>(optionsBuilder =>
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MoviesDBConnString")));

builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Audience = "movie-api";
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
var app = builder.Build();

// app.UseResponseCaching();

app.UseHttpCacheHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();