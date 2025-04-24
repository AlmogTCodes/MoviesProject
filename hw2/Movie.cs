namespace hw2
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

        private static List<Movie> moviesList = new List<Movie>();
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

        #region Methods
        //---------------------------------------- Methods ----------------------------------------//

        /// <summary>
        /// Inserts this movie into the static moviesList if a movie with the same id and primary title does not already exist.
        /// </summary>
        /// <returns>
        /// True if the movie was inserted; otherwise, false.
        /// </returns>
        public bool Insert()
        {
            foreach (var movie in moviesList)
            {
                if (movie.Id == this.Id || movie.PrimaryTitle == this.PrimaryTitle)
                {
                    return false; // Movie already exists
                }
            }

            moviesList.Add(this);
            return true; // Insertion successful
        }


        /// <summary>
        /// Retrieves the complete list of movies.
        /// </summary>
        /// <returns>A reference to the static list containing all movies.</returns>
        public static List<Movie> Read()
        {
            return moviesList;
        }


        /// <summary>
        /// Retrieves a list of movies that match the specified primary title.
        /// </summary>
        /// <param name="title">The title to search for.</param>
        /// <returns>A list of movies with the matching title.</returns>
        public static List<Movie> GetByTitle(string title)
        {
            List<Movie> selectedList = new List<Movie>();
            foreach (Movie m in moviesList)
            {
                // Convert both the search title and movie title to lowercase for comparison
                if (m.PrimaryTitle.ToLower() == title.ToLower())
                {
                    selectedList.Add(m);
                }
            }
            return selectedList;
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
            foreach (Movie m in moviesList)
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
            foreach (var movie in moviesList)
            {
                if (movie.Id == id)
                {
                    moviesList.Remove(movie);
                    return true; // Movie has been successfully removed
                }
            }

            return false; // Failed to remove movie
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Methods
    }
}
