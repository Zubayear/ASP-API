using Microsoft.EntityFrameworkCore;
using Movie.API.Entity;

namespace Movie.API.Context;

public class MovieContext : DbContext
{
    public DbSet<Entity.Movie>? Movies { get; set; }
    public DbSet<Actor>? Actors { get; set; }
    public DbSet<ActorMovie>? ActorMovies { get; set; }
    public MovieContext(DbContextOptions<MovieContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var decimalProps = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => (Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

        foreach (var property in decimalProps)
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }

        modelBuilder.Entity<ActorMovie>()
            .HasOne(actor => actor.Actor)
            .WithMany(actor => actor.ActorMovies)
            .HasForeignKey(movie => movie.ActorId);

        modelBuilder.Entity<ActorMovie>()
            .HasOne(movie => movie.Movie)
            .WithMany(movie => movie.ActorMovies)
            .HasForeignKey(movie => movie.MovieId);
        
        base.OnModelCreating(modelBuilder);
    }
}