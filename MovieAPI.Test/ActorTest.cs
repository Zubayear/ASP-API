using System;
using System.Collections.Immutable;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Movie.API.Controllers;
using Movie.API.Models;
using Movie.API.Services;
using Xunit;
using Xunit.Abstractions;
using Actor = Movie.API.Entity.Actor;

namespace MovieAPI.Test;

public class ActorTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IActorRepository> _actorRepositoryMock = new();
    private readonly Mock<ILogger<ActorsController>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public ActorTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void GetActorByIdTest()
    {
        Actor actor = new Actor
        {
            Age = 90,
            Id = Guid.Parse("3567d09c-e9c4-49f0-9cd5-61a52c660707"),
            Name = "Name"
        };
        _actorRepositoryMock.Setup(repository =>
                repository.GetActorById(Guid.Parse("3567d09c-e9c4-49f0-9cd5-61a52c660707")))
            .ReturnsAsync(actor);
        ActorsController actorsController =
            new ActorsController(_actorRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        var actionResult = await actorsController.GetActor(Guid.Parse("3567d09c-e9c4-49f0-9cd5-61a52c660707"));
        var result = actionResult.Result as OkObjectResult;
        Assert.Equal(result.Value, actor);
    }
    
    [Fact]
    public async void CreateActorTest()
    {
        var actorForCreation = new ActorForCreation
        {
            Age = 78,
            Name = "Robert De Niro"
        };
        var actor = new Actor
        {
            Age = 78,
            Id = Guid.Parse("3567d09c-e9c4-49f0-9cd5-61a52c660707"),
            Name = "Robert De Niro"
        };
        _actorRepositoryMock.Setup(repository =>
            repository.SaveActor(actor)).ReturnsAsync(actor);
        ActorsController actorsController =
            new ActorsController(_actorRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        var actionResult = await actorsController.CreateActor(actorForCreation);
        // _testOutputHelper.WriteLine(actionResult.Value.ToString());
        var result = actionResult.Result as CreatedAtRouteResult;
        _testOutputHelper.WriteLine(result.Value.ToString());
        // Assert.Equal(result.Value, actor);
    }
    
    [Fact]
    public async void GetAllActorsTest()
    {
        _actorRepositoryMock.Setup(repository =>
            repository.GetActors()).ReturnsAsync(ImmutableList<Actor>.Empty);
        ActorsController actorsController =
            new ActorsController(_actorRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        var actionResult = await actorsController.GetAllActors();
        var result = actionResult.Result as OkObjectResult;
        Assert.Equal(result.Value, ImmutableList<Actor>.Empty);
    }
}