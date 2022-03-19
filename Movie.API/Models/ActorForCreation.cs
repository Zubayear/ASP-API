using System.ComponentModel.DataAnnotations;

namespace Movie.API.Models;

public class ActorForCreation
{
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public int Age { get; set; }
}