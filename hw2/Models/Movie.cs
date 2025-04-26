namespace hw2.Models
{
    public class Movie
    {

        #region Properties
        //---------------------------------------- Properties ----------------------------------------//
        private int _id;
        private string _url;
        private string _primaryTitle;
        private string _description;
        private string _primaryImage;
        private int _year;
        private DateTime _releaseDate;
        private string _language;
        private double _budget;
        private double _grossWorldwide;
        private string _genres;
        private bool _isAdult;
        private int _runtimeMinutes;
        private float _averageRating;
        private int _numVotes;

        private static List<Movie> _moviesList = new List<Movie>();

        //--------------------------------------------------------------------------------------------//
        #endregion Properties

        #region Get-Set Methods
        //---------------------------------------- Get-Set Methods ----------------------------------------//

        /// <summary>
        /// Gets or sets the movie identifier.
        /// </summary>
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// Gets or sets the URL associated with the movie.
        /// </summary>
        public string Url
        {
            get => _url;
            set => _url = value;
        }

        /// <summary>
        /// Gets or sets the primary title of the movie.
        /// </summary>
        public string PrimaryTitle
        {
            get => _primaryTitle;
            set => _primaryTitle = value;
        }

        /// <summary>
        /// Gets or sets the movie description.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>
        /// Gets or sets the URL for the movie's primary image.
        /// </summary>
        public string PrimaryImage
        {
            get => _primaryImage;
            set => _primaryImage = value;
        }

        /// <summary>
        /// Gets or sets the release year of the movie.
        /// </summary>
        public int Year
        {
            get => _year;
            set => _year = value;
        }

        /// <summary>
        /// Gets or sets the release date of the movie.
        /// </summary>
        public DateTime ReleaseDate
        {
            get => _releaseDate;
            set => _releaseDate = value;
        }

        /// <summary>
        /// Gets or sets the language of the movie.
        /// </summary>
        public string Language
        {
            get => _language;
            set => _language = value;
        }

        /// <summary>
        /// Gets or sets the budget of the movie.
        /// </summary>
        public double Budget
        {
            get => _budget;
            set => _budget = value;
        }

        /// <summary>
        /// Gets or sets the worldwide gross earnings of the movie.
        /// </summary>
        public double GrossWorldwide
        {
            get => _grossWorldwide;
            set => _grossWorldwide = value;
        }

        /// <summary>
        /// Gets or sets the genres of the movie.
        /// </summary>
        public string Genres
        {
            get => _genres;
            set => _genres = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the movie is for adult audiences.
        /// </summary>
        public bool IsAdult
        {
            get => _isAdult;
            set => _isAdult = value;
        }

        /// <summary>
        /// Gets or sets the runtime of the movie in minutes.
        /// </summary>
        public int RuntimeMinutes
        {
            get => _runtimeMinutes;
            set => _runtimeMinutes = value;
        }

        //----- Get-Set AverageRating -----//
        /// <summary>
        /// Gets or sets the average rating of the movie.
        /// </summary>
        public float AverageRating
        {
            get => _averageRating;
            set => _averageRating = value;
        }

        /// <summary>
        /// Gets or sets the number of votes the movie has received.
        /// </summary>
        public int NumVotes
        {
            get => _numVotes;
            set => _numVotes = value;
        }

        private static List<Movie> MoviesList 
        {
            get => _moviesList;
            set => _moviesList = value;
        }

        //--------------------------------------------------------------------------------------------//
        #endregion Get-Set Methods

        #region Constructors
        //---------------------------------------- Constructors ----------------------------------------//

        /// <summary>
        /// Initializes a new instance of the <see cref="Movie"/> class.
        /// </summary>
        public Movie() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Movie"/> class with specified details.
        /// </summary>
        public Movie(int id, string url, string primaryTitle, string description, string primaryImage,
                     int year, DateTime releaseDate, string language, double budget, double grossWorldwide,
                     string genres, bool isAdult, int runtimeMinutes, float averageRating, int numVotes)
        {
            Id = id;
            Url = url;
            PrimaryTitle = primaryTitle;
            Description = description;
            PrimaryImage = primaryImage;
            Year = year;
            ReleaseDate = releaseDate;
            Language = language;
            Budget = budget;
            GrossWorldwide = grossWorldwide;
            Genres = genres;
            IsAdult = isAdult;
            RuntimeMinutes = runtimeMinutes;
            AverageRating = averageRating;
            NumVotes = numVotes;
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Constructors

        #region Temporary Tests Methods
        //---------------------------------------- Temporary Tests Methods ----------------------------------------//

        public static void ResetMoviesList()
        {
            MoviesList.Clear();
        }

        //--------------------------------------------------------------------------------------------//
        #endregion Temporary Tests Methods

        #region Methods
        //---------------------------------------- Methods ----------------------------------------//

        /// <summary>
        /// Inserts a movie into the static list if a movie with the same ID or Primary Title does not already exist.
        /// Note: This method modifies a static collection and is not thread-safe.
        /// </summary>
        /// <param name="movieToInsert">The movie object to insert.</param>
        /// <returns>True if the movie was inserted; otherwise, false (if ID/PrimaryTitle already exists).</returns>
        public static bool Insert(Movie movieToInsert)
        {
            if (movieToInsert == null || movieToInsert.PrimaryTitle == null)
            {
                return false;
            }

            if (MoviesList.Any(m => m.Id == movieToInsert.Id || m.PrimaryTitle != null && m.PrimaryTitle.Equals(movieToInsert.PrimaryTitle, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Movie with this ID/PrimaryTitle already exists
            }

            MoviesList.Add(movieToInsert);
            return true; // Insertion successful
        }


        /// <summary>
        /// Retrieves a copy of the complete list of movies.
        /// </summary>
        /// <returns>An enumerable collection containing all movies.</returns> 
        public static IEnumerable<Movie> Read()
        {
            // Return a read-only wrapper or a copy to prevent modification of the original list
            // return moviesList.AsReadOnly();
            // Or return a copy if the caller might need to modify their copy:
            return MoviesList.ToList(); // Returning a copy is often safer for web scenarios
        }


        /// <summary>
        /// Retrieves a list of movies where the primary title contains the specified search string (case-insensitive).
        /// Handles leading/trailing whitespace in the search string.
        /// </summary>
        /// <param name="titleSubstring">The substring to search for within the movie titles.</param>
        /// <returns>An enumerable collection of movies whose titles contain the search string.</returns>
        public static IEnumerable<Movie> GetByTitle(string titleSubstring)
        {
            // Trim whitespace on the server-side as well for robustness.
            // Handle potential null input first.
            string trimmedSubstring = titleSubstring?.Trim() ?? string.Empty;

            // Check if the trimmed string is null or empty.
            if (string.IsNullOrEmpty(trimmedSubstring))
            {
                // Return the entire list if the search string is empty or null.
                return Read(); // Changed from Enumerable.Empty<Movie>()
            }

            // Use LINQ Where() with Contains() and StringComparison.OrdinalIgnoreCase.
            // Use the trimmedSubstring for the comparison.
            return MoviesList.Where(m => m.PrimaryTitle != null && m.PrimaryTitle.Contains(trimmedSubstring, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Retrieves a list of movies released within the specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A list of movies that were released between the specified dates.</returns>
        public static List<Movie> GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            List<Movie> selectedList = new List<Movie>();
            foreach (Movie m in MoviesList)
            {
                if (m.ReleaseDate >= startDate && m.ReleaseDate <= endDate)
                {
                    selectedList.Add(m);
                }
            }
            return selectedList;
        }


        /// <summary>
        /// Deletes the movie with the specified id from the moviesList.
        /// </summary>
        /// <param name="id">The movie identifier to be deleted.</param>
        /// <returns>True if the movie was found and removed; otherwise, false.</returns>
        public static bool Delete(int id)
        {
            // Check if the movie already exists in the list by id or primaryTitle
            foreach (var movie in MoviesList)
            {
                if (movie.Id == id)
                {
                    MoviesList.Remove(movie);
                    return true; // Movie has been successfully removed
                }
            }

            return false; // Failed to remove movie
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Methods
    }
}
