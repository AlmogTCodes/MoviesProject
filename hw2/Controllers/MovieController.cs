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
        [HttpPost]
        public ActionResult<Movie> Post([FromBody] Movie movie)
        {
            bool inserted = Movie.Insert(movie);
            if (!inserted)
            {
                // Return Conflict (409) if the ID already exists
                return Conflict("Something went wrong.");
            }
            // Return 201 Created with the location of the new resource and the resource itself
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        // GET: api/Movie/search?title={title}
        [HttpGet("search")] // this uses the QueryString
        public IEnumerable<Movie> GetByTitle(string title)
        {
            return Movie.GetByTitle(title);
        }

        // GET: api/Movie/from/{startDate}/until/{endDate}
        [HttpGet("searchByDate")] // this uses resource routing
        public IEnumerable<Movie> GetByReleaseDate(DateTime startDate, DateTime endData)
        {
            return Movie.GetByReleaseDate(startDate,endData);  
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
