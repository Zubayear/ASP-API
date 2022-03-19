using System.ComponentModel.DataAnnotations;

namespace Movie.API.Models;

public class ActorForUpdate
{
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public int Age { get; set; }
}