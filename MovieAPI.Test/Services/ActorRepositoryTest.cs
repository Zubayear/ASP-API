using System;
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

namespace MovieAPI.Test;

public class ActorRepositoryTest
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<MovieContext> _contextOptions;

    public ActorRepositoryTest()
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
            viewCommand.CommandText = @"CREATE VIEW AllResources AS SELECT Age FROM Actor;";
            viewCommand.ExecuteNonQuery();
        }

        context.AddRange(
            new Actor { Id = Guid.Parse("bc54b41b-7ca5-43d3-80a8-9fd60c4336d8"), Name = "Al Pacino", Age = 81 },
            new Actor { Id = Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"), Name = "Jack Nicholson", Age = 84 });
        context.SaveChanges();
    }

    MovieContext CreateContext() => new MovieContext(_contextOptions);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetActorsTest()
    {
        var mock = new Mock<ILogger<ActorRepository>>();
        await using var context = CreateContext();
        var actorRepository = new ActorRepository(context, mock.Object);
        var actors = await actorRepository.GetActors();
        Assert.Equal(2, actors.Count());
    }

    [Fact]
    public async Task SaveActorTest()
    {
        var mock = new Mock<ILogger<ActorRepository>>();
        await using var context = CreateContext();
        var id = Guid.NewGuid();
        var input = new Actor
        {
            Id = id,
            Age = 78,
            Name = "Robert De Niro"
        };
        var actorRepository = new ActorRepository(context, mock.Object);
        var expecting = await actorRepository.SaveActor(input);
        // await actorRepository.SaveChanges();
        var allActors = await actorRepository.GetActors();
        Assert.Equal(3, allActors.Count());
        Assert.Equal(expecting, input);
        // null check
        Actor nilActor = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => actorRepository.SaveActor(nilActor));
    }

    [Fact]
    public async Task DeleteActorTest()
    {
        var mock = new Mock<ILogger<ActorRepository>>();
        await using var context = CreateContext();
        var actorRepository = new ActorRepository(context, mock.Object);
        await actorRepository.DeleteActor(Guid.Parse("253780f1-1950-4162-99b1-f03d28efbfb2"));
        await actorRepository.SaveChanges();
        var actors = await actorRepository.GetActors();
        Assert.Equal(1, actors.Count());
        // null check
        await Assert.ThrowsAsync<ArgumentNullException>(() => actorRepository.DeleteActor(Guid.Empty));
    }

    [Fact]
    public async Task UpdateActorTest()
    {
        var mock = new Mock<ILogger<ActorRepository>>();
        await using var context = CreateContext();
        var actorRepository = new ActorRepository(context, mock.Object);
        var input = new Actor { Id = Guid.Parse("bc54b41b-7ca5-43d3-80a8-9fd60c4336d8"), Name = "Al Pacino", Age = 84 };
        actorRepository.UpdateActor(input);
        await actorRepository.SaveChanges();
        var actors = await actorRepository.GetActors();
        Assert.Equal(2, actors.Count());
        var expected = await actorRepository.GetActorById(Guid.Parse("bc54b41b-7ca5-43d3-80a8-9fd60c4336d8"));
        Assert.Equal(expected, input);
        Assert.Equal(84, expected.Age);
        // null check
        Actor nilActor = null;
        Assert.Throws<ArgumentNullException>(() => actorRepository.UpdateActor(nilActor));
    }
}