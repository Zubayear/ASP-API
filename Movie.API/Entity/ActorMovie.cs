namespace Movie.API.Entity;

public class ActorMovie
{
    public Guid Id { get; set; }
    public Guid ActorId { get; set; }
    public Actor? Actor { get; set; }
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(ActorId)}: {ActorId}, {nameof(Actor)}: {Actor}, {nameof(MovieId)}: {MovieId}, {nameof(Movie)}: {Movie}";
    }
}