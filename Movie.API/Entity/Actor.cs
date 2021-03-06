using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Movie.API.Entity;

public class Actor
{
    [Key] public Guid Id { get; set; }
    [Required] [MaxLength(50)] public string Name { get; set; } = String.Empty;
    [Required] public int Age { get; set; }

    [JsonIgnore]
    public List<ActorMovie> ActorMovies { get; set; }
    
    public override string ToString()
    {
        return
            $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}, {nameof(ActorMovies)}: {ActorMovies}";
    }
}