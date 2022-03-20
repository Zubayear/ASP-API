namespace Movie.API.Models;

public class Actor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int Age { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
    }
}