using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.API.Entity;

public class Movie
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(25)]
    public string Title { get; set; } = String.Empty;
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = String.Empty;
    [Required]
    public decimal Rating { get; set; }
    [Required]
    public int ReleaseYear { get; set; }
    
    public List<ActorMovie> ActorMovies { get; set; }
}