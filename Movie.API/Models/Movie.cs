namespace Movie.API.Models;

public class Movie
{
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Rating { get; set; }
    public int ReleaseYear { get; set; }
}