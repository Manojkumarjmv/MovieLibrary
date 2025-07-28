// Program.cs
using MovieLibrary.API.Models;
using MovieLibrary.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

var movieService = app.Services.GetRequiredService<IMovieService>();

app.MapGet("/api/movies", () => movieService.GetAll());

app.MapGet("/api/movies/{id:guid}", (Guid id) =>
    movieService.GetById(id) is Movie m ? Results.Ok(m) : Results.NotFound());

app.MapGet("/api/movies/search/{query}", (string query) =>
    Results.Ok(movieService.Search(query)));

app.MapPost("/api/movies", (Movie movie) =>
{
    movieService.Add(movie);
    return Results.Created($"/api/movies/{movie.Id}", movie);
});

app.MapPut("/api/movies/{id:guid}", (Guid id, Movie updatedMovie) =>
{
    updatedMovie.Id = id;
    return movieService.Update(updatedMovie) ? Results.Ok(updatedMovie) : Results.NotFound();
});

app.MapDelete("/api/movies/{id:guid}", (Guid id) =>
    movieService.Delete(id) ? Results.Ok() : Results.NotFound());

app.Run();