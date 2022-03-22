using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Movie.API.Controllers;
using Movie.API.Models;
using Movie.API.Services;
using Xunit;

namespace MovieAPI.Test.Controllers;

public class MoviesControllerTest
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock = new();
    private readonly Mock<ILogger<MoviesController>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task GetMovieTest()
    {
        var moviesController =
            new MoviesController(_movieRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        var id = Guid.NewGuid();
        var movie = new Movie.API.Entity.Movie
        {
            Id = id,
            Description =
                "Arthur Fleck, a party clown, leads an impoverished life with his ailing mother. However, when society shuns him and brands him as a freak, he decides to embrace the life of crime and chaos.",
            Rating = 8.4m,
            ReleaseYear = 2019,
            Title = "Joker"
        };
        _movieRepositoryMock.Setup(repository =>
            repository.GetMovieById(id)).ReturnsAsync(movie);
        var result = await moviesController.GetMovie(id);
        var expected = result.Result as OkObjectResult;
        Debug.Assert(expected != null, nameof(expected) + " != null");
        Assert.Equal(expected.Value, movie);
        _movieRepositoryMock.Setup(repository =>
            repository.GetMovieById(Guid.Empty)).Throws<InvalidEnumArgumentException>();
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => moviesController.GetMovie(Guid.Empty));
    }

    [Fact]
    public async Task CreateMovieWithActorTest()
    {
        var moviesController =
            new MoviesController(_movieRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);

        var id = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var movie = new Movie.API.Entity.Movie
        {
            Id = id,
            Description =
                "Arthur Fleck, a party clown, leads an impoverished life with his ailing mother. However, when society shuns him and brands him as a freak, he decides to embrace the life of crime and chaos.",
            Rating = 8.4m,
            ReleaseYear = 2019,
            Title = "Joker"
        };
        var input = new MovieForCreation
        {
            Description =
                "Arthur Fleck, a party clown, leads an impoverished life with his ailing mother. However, when society shuns him and brands him as a freak, he decides to embrace the life of crime and chaos.",
            Rating = 8.4m,
            ReleaseYear = 2019,
            Title = "Joker"
        };
        _movieRepositoryMock.Setup(repository =>
            repository.SaveMovieWithActor(actorId, movie)).Throws<InvalidEnumArgumentException>();
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            moviesController.CreateMovieWithActor(actorId, input));

        _movieRepositoryMock.Setup(repository =>
            repository.SaveMovieWithActor(Guid.Empty, movie)).Throws<InvalidEnumArgumentException>();
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            moviesController.CreateMovieWithActor(Guid.Empty, input));
        _movieRepositoryMock.Setup(repository =>
            repository.SaveMovieWithActor(actorId, movie)).Throws<InvalidEnumArgumentException>();
    }
}