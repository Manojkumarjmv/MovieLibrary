using System.Text.Json;
using MovieLibrary.API.Models;

namespace MovieLibrary.API.Services
{
    public class MovieService : IMovieService
    {
        private readonly string _filePath = "movies1.json";

        public List<Movie> GetAll() =>
            File.Exists(_filePath)
                ? JsonSerializer.Deserialize<List<Movie>>(File.ReadAllText(_filePath), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new()
                : new();

        public Movie? GetById(Guid id) =>
            GetAll().FirstOrDefault(m => m.Id == id);

        public List<Movie> Search(string query) =>
            GetAll().Where(m =>
                m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                m.Genre.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                m.Actors.Any(a => a.Contains(query, StringComparison.OrdinalIgnoreCase))
            ).ToList();

        public void Add(Movie movie)
        {
            var movies = GetAll();
            movie.Id = Guid.NewGuid();
            movies.Add(movie);
            Save(movies);
        }

        public bool Update(Movie updatedMovie)
        {
            var movies = GetAll();
            var idx = movies.FindIndex(m => m.Id == updatedMovie.Id);
            if (idx == -1) return false;
            movies[idx] = updatedMovie;
            Save(movies);
            return true;
        }

        public bool Delete(Guid id)
        {
            var movies = GetAll();
            var countBefore = movies.Count;
            movies = movies.Where(m => m.Id != id).ToList();
            Save(movies);
            return movies.Count < countBefore;
        }

        private void Save(List<Movie> movies) =>
            File.WriteAllText(_filePath, JsonSerializer.Serialize(movies));
    }
}