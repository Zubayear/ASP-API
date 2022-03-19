using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.API.Entity;

public class Actor
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = String.Empty;
    [Required]
    public int Age { get; set; }

    public List<ActorMovie> ActorMovies { get; set; }
}