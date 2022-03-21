namespace Movie.API.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Rating { get; set; }
    public int ReleaseYear { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Title)}: {Title}, {nameof(Description)}: {Description}, {nameof(Rating)}: {Rating}, {nameof(ReleaseYear)}: {ReleaseYear}";
    }
}