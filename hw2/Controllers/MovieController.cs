using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hw2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        // POST: api/Movie
        [HttpPost]
        public bool Post([FromBody] Movie movie)
        {
            return Movie.Insert(movie);
        }

        // GET: api/Movie
        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            return Movie.Read();
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
