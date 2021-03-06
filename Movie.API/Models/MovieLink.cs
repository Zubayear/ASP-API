namespace Movie.API.Models;

public class MovieLink
{
    public string? Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }

    public MovieLink(string? href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}