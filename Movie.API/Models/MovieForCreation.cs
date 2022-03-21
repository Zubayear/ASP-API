using System.ComponentModel.DataAnnotations;

namespace Movie.API.Models;

public class MovieForCreation
{
    [Required]
    public string Title { get; set; } = String.Empty;
    [Required]
    public string Description { get; set; } = String.Empty;
    [Required]
    public decimal Rating { get; set; }
    [Required]
    public int ReleaseYear { get; set; }

    public override string ToString()
    {
        return $"{nameof(Title)}: {Title}, {nameof(Description)}: {Description}, {nameof(Rating)}: {Rating}, {nameof(ReleaseYear)}: {ReleaseYear}";
    }
}