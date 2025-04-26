using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hw2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        // GET: api/Movie
        /// <summary>
        /// Retrieves a collection of all movies in the collection.
        /// </summary>
        /// <returns>An enumerable collection containing all movies.</returns>
        [HttpGet]
        public IEnumerable<Movie> GetAllMovies()
        {
            return Movie.Read();
        }

        // POST: api/Movie
        /// <summary>
        /// Creates a new movie resource based on the provided data.
        /// Checks if a movie with the same ID or Primary Title already exists before inserting.
        /// </summary>
        /// <param name="movie">The movie object containing the data for the new movie. Received from the request body.</param>
        /// <returns>
        /// An <see cref="ActionResult"/>.
        /// Returns <see cref="OkObjectResult"/> (HTTP 200 OK) with the new movie if successful.
        /// Returns <see cref="ConflictObjectResult"/> (HTTP 409 Conflict) if a movie with the same ID or Primary Title already exists.
        /// </returns>
        [HttpPost]
        public ActionResult<Movie> Post([FromBody] Movie movie)
        {
            bool inserted = Movie.Insert(movie);
            if (!inserted)
            {
                // Return Conflict (409) if the ID or Title already exists
                return Conflict("An error occurred while processing your request.");
            }
            // Return 200
            return Ok(movie);
        }

        // GET: api/Movie/search?title={title}
        /// <summary>
        /// Searches for movies by title using a query string parameter.
        /// </summary>
        /// <param name="title">The movie title (or part of it) to search for. Case-insensitive.</param>
        /// <returns>A collection of movies matching the title search criteria.</returns>
        [HttpGet("search")] // Route: /api/Movie/search
        public IEnumerable<Movie> GetByTitle(string? title) // Binds 'title' from query string (?title=...)
        {
            // If the 'title' parameter from the query string is null (not provided when sent to the API),
            // the null-coalescing operator (??) provides an empty string to the Movie.GetByTitle method.
            // The Movie.GetByTitle method handles the logic for an empty or null search string and return all movies.
            return Movie.GetByTitle(title ?? string.Empty);
        }

        // GET: api/Movie/from/{startDate}/until/{endDate}
        /// <summary>
        /// Retrieves movies released within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the release period (inclusive).</param>
        /// <param name="endDate">The end date of the release period (inclusive).</param>
        /// <returns>An enumerable collection of movies released between the start and end dates.</returns>
        [HttpGet("from/{startDate}/until/{endDate}")]
        public IEnumerable<Movie> GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            return Movie.GetByReleaseDate(startDate,endDate);
        }

        // GET: api/Movie/{id}
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT: api/Movie/{id}
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Movie/{id}
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return Movie.Delete(id);
        }
        
    }
}
