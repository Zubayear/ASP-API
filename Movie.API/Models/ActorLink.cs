namespace Movie.API.Models;

public class ActorLink
{
    public string? Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }

    public ActorLink(string? href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}