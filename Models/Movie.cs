// Models/Movie.cs
namespace MovieLibrary.API.Models;

using System.ComponentModel.DataAnnotations;

public class Movie
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Genre { get; set; } = null!;

    [Required]
    public List<string> Actors { get; set; } = new();

    [Required]
    public DateTime ReleaseDate { get; set; }

    [Required]
    public string PosterUrl { get; set; } = null!;

    [Required]
    public string Details { get; set; } = null!;
}