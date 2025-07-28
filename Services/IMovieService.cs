using MovieLibrary.API.Models;

namespace MovieLibrary.API.Services
{
    public interface IMovieService
    {
        List<Movie> GetAll();
        Movie? GetById(Guid id);
        List<Movie> Search(string query);
        void Add(Movie movie);
        bool Update(Movie updatedMovie);
        bool Delete(Guid id);
    }
}