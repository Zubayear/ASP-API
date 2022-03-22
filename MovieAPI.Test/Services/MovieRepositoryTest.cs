using System;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Movie.API.Context;
using Movie.API.Entity;
using Movie.API.Services;
using Xunit;

namespace MovieAPI.Test.Services;

public class MovieRepositoryTest
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<MovieContext> _contextOptions;

    public MovieRepositoryTest()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<MovieContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new MovieContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"CREATE VIEW AllResources AS SELECT * FROM Movies;";
            viewCommand.ExecuteNonQuery();
        }

        context.AddRange(new global::Movie.API.Entity.Movie
            {
                Id = Guid.Parse("bc54b41b-7ca5-43d3-80a8-9fd60c4336d8"),
                Description =
                    "Arthur Fleck, a party clown, leads an impoverished life with his ailing mother. However, when society shuns him and brands him as a freak, he decides to embrace the life of crime and chaos.",
                Rating = 8.4m,
                ReleaseYear = 2019,
                Title = "Joker"
            },
            new Actor { Id = Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"), Name = "Joaquin Phoenix", Age = 47 },
            new Actor { Id = Guid.Parse("bdac6f3c-a9f2-4223-8ec4-365cd5f133ec"), Name = "Robert Pattinson", Age = 35 }
        );
        context.SaveChanges();
    }

    MovieContext CreateContext() => new MovieContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task ActorExistsTest()
    {
        var mock = new Mock<ILogger<MovieRepository>>();
        
        await using var context = CreateContext();
        var actorRepository = new MovieRepository(context, mock.Object);
        var expected = await actorRepository.ActorExists(Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"));
        Assert.True(expected);
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => actorRepository.ActorExists(Guid.Empty));
    }

    [Fact]
    public async Task GetMovieByIdTest()
    {
        var mock = new Mock<ILogger<MovieRepository>>();
        
        await using var context = CreateContext();
        var actorRepository = new MovieRepository(context, mock.Object);
        var actual = await actorRepository.GetMovieById(Guid.Parse("bc54b41b-7ca5-43d3-80a8-9fd60c4336d8"));
        Assert.Equal("Joker", actual.Title);
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => actorRepository.GetMovieById(Guid.Empty));
    }
    
    [Fact]
    public async Task SaveMovieWithActorTest()
    {
        var mock = new Mock<ILogger<MovieRepository>>();
        await using var context = CreateContext();
        var actorRepository = new MovieRepository(context, mock.Object);
        var movie = new global::Movie.API.Entity.Movie
        {
            Id = Guid.Parse("34c14875-cf84-4a5a-9cfc-77c9bb459800"),
            Description = "Batman ventures into Gotham City's underworld when a sadistic killer leaves behind a trail of cryptic clues. As the evidence begins to lead closer to home and the scale of the perpetrator's plans become clear, he must forge new relationships, unmask the culprit and bring justice to the abuse of power and corruption that has long plagued the metropolis.",
            Rating = 8.4m,
            ReleaseYear = 2022,
            Title = "The Batman"
        };
        var actual = await actorRepository.SaveMovieWithActor(Guid.Parse("bdac6f3c-a9f2-4223-8ec4-365cd5f133ec"), movie);
        Assert.Equal("The Batman", actual.Title);
        Assert.Equal(movie, actual);
        var actualNumber = await actorRepository.GetMoviesForActor(Guid.Parse("bdac6f3c-a9f2-4223-8ec4-365cd5f133ec"));
        Assert.Equal(1, actualNumber.Count());
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => actorRepository.SaveMovieWithActor(Guid.Empty, movie));
    }

    [Fact]
    public async Task GetMoviesForActorTest()
    {
        var mock = new Mock<ILogger<MovieRepository>>();
        await using var context = CreateContext();
        var actorRepository = new MovieRepository(context, mock.Object);
        var movie1 = new global::Movie.API.Entity.Movie
        {
            Id = Guid.Parse("34c14875-cf84-4a5a-9cfc-77c9bb459800"),
            Description = "Batman ventures into Gotham City's underworld when a sadistic killer leaves behind a trail of cryptic clues. As the evidence begins to lead closer to home and the scale of the perpetrator's plans become clear, he must forge new relationships, unmask the culprit and bring justice to the abuse of power and corruption that has long plagued the metropolis.",
            Rating = 8.4m,
            ReleaseYear = 2022,
            Title = "The Batman"
        };
        var movie2 = new global::Movie.API.Entity.Movie
        {
            Id = Guid.Parse("b7e5c001-157e-4c13-872b-0e04b36455d6"),
            Description = "Commodus takes over power and demotes Maximus, one of the preferred generals of his father, Emperor Marcus Aurelius. As a result, Maximus is relegated to fighting till death as a gladiator.",
            Rating = 8.5m,
            ReleaseYear = 2000,
            Title = "Gladiator"
        };
        var actual1 = await actorRepository.SaveMovieWithActor(Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"), movie1);
        var actual2 = await actorRepository.SaveMovieWithActor(Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"), movie2);
        Assert.Equal("The Batman", actual1.Title);
        Assert.Equal(movie1, actual1);
        Assert.Equal(movie2, actual2);
        var actualNumber = await actorRepository.GetMoviesForActor(Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"));
        Assert.Equal(2, actualNumber.Count());
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => actorRepository.SaveMovieWithActor(Guid.Empty, movie1));
    }
}