using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hw2.Models;

namespace hw2.Tests
{
    [TestClass()]
    public class MovieTests
    {
        [TestInitialize()]
        public void TestInitialize()
        {
            // Reset static list before each test
            Movie.ResetMoviesList();
        }

        #region Constructors Tesing
        // Test for the default constructor
        [TestMethod()]
        public void Movie_DefaultConstructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var movie = new Movie();

            // Assert
            Assert.IsNotNull(movie);
            // Check default values if applicable (e.g., Id might be 0, strings null/empty)
            Assert.AreEqual(0, movie.Id);
            Assert.IsNull(movie.PrimaryTitle);
        }

        // Test for the parameterized constructor
        [TestMethod()]
        public void Movie_Parameterized_Constructor_Should_Set_Properties()
        {
            // Arrange
            int expectedId = 55;
            string expectedUrl = "http://example.com";
            string expectedTitle = "Constructor Test";
            string expectedDescription = "A test description.";
            string expectedPrimaryImage = "http://example.com/image.jpg";
            int expectedYear = 2023;
            DateTime expectedReleaseDate = new DateTime(2023, 1, 15);
            string expectedLanguage = "en-US";
            double expectedBudget = 1000000.50;
            double expectedGross = 5000000.75;
            string expectedGenres = "Action,Test";
            bool expectedIsAdult = false;
            int expectedRuntime = 125;
            float expectedRating = 8.5f;
            int expectedVotes = 1500;


            // Act
            var movie = new Movie(expectedId, expectedUrl, expectedTitle, expectedDescription, expectedPrimaryImage,
                                  expectedYear, expectedReleaseDate, expectedLanguage, expectedBudget, expectedGross,
                                  expectedGenres, expectedIsAdult, expectedRuntime, expectedRating, expectedVotes);

            // Assert
            Assert.AreEqual(expectedId, movie.Id, "ID should be set by constructor.");
            Assert.AreEqual(expectedUrl, movie.Url, "Url should be set by constructor.");
            Assert.AreEqual(expectedTitle, movie.PrimaryTitle, "PrimaryTitle should be set by constructor.");
            Assert.AreEqual(expectedDescription, movie.Description, "Description should be set by constructor.");
            Assert.AreEqual(expectedPrimaryImage, movie.PrimaryImage, "PrimaryImage should be set by constructor.");
            Assert.AreEqual(expectedYear, movie.Year, "Year should be set by constructor.");
            Assert.AreEqual(expectedReleaseDate, movie.ReleaseDate, "ReleaseDate should be set by constructor.");
            Assert.AreEqual(expectedLanguage, movie.Language, "Language should be set by constructor.");
            Assert.AreEqual(expectedBudget, movie.Budget, "Budget should be set by constructor.");
            Assert.AreEqual(expectedGross, movie.GrossWorldwide, "GrossWorldwide should be set by constructor.");
            Assert.AreEqual(expectedGenres, movie.Genres, "Genres should be set by constructor.");
            Assert.AreEqual(expectedIsAdult, movie.IsAdult, "IsAdult should be set by constructor.");
            Assert.AreEqual(expectedRuntime, movie.RuntimeMinutes, "RuntimeMinutes should be set by constructor.");
            Assert.AreEqual(expectedRating, movie.AverageRating, "AverageRating should be set by constructor.");
            Assert.AreEqual(expectedVotes, movie.NumVotes, "NumVotes should be set by constructor.");
        }
        #endregion Constructors Tesing


        #region Insert Static Method Testing

        [TestMethod()]
        public void Insert_Static_New_Movie_Unique_Id_And_Title_Should_Return_True_And_Add()
        {
            // Arrange
            var movie = new Movie(10, "url1", "Unique Insert Test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            int initialCount = Movie.Read().Count(); // 0

            // Act
            bool result = Movie.Insert(movie);

            // Assert
            Assert.IsTrue(result, "Insert should return true for a new movie.");
            Assert.AreEqual(initialCount + 1, Movie.Read().Count(), "Movie list count should increase by 1.");
            Assert.IsTrue(Movie.Read().Any(m => m.Id == 10 && m.PrimaryTitle == "Unique Insert Test"), "Movie list should contain the inserted movie.");
        }

        [TestMethod()]
        public void Insert_Static_Null_Movie_Object_Should_Return_False()
        {
            // Arrange
            Movie? movieToInsert = null;
            int initialCount = Movie.Read().Count(); // 0

            // Act
            bool result = Movie.Insert(movieToInsert);

            // Assert
            Assert.IsFalse(result, "Insert should return false for a null movie object.");
            Assert.AreEqual(initialCount, Movie.Read().Count(), "Movie list count should not change.");
        }

        [TestMethod()]
        public void Insert_Static_Movie_With_Null_Title_Should_Return_False()
        {
            // Arrange
            var movie = new Movie(15, "url1", null, "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            int initialCount = Movie.Read().Count(); // 0

            // Act
            bool result = Movie.Insert(movie);

            // Assert
            Assert.IsFalse(result, "Insert should return false for a movie with a null title.");
            Assert.AreEqual(initialCount, Movie.Read().Count(), "Movie list count should not change.");
        }


        [TestMethod()]
        public void Insert_Static_Existing_Movie_ID_Should_Return_False()
        {
            // Arrange
            var movie1 = new Movie(20, "url1", "First Movie Title", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            Movie.Insert(movie1); // Insert successful, movie has ID 20

            // Create another movie with the SAME ID but different title
            var movie2 = new Movie(20, "url2", "Second Movie Title", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);

            int listCountBeforeInsert = Movie.Read().Count(); // Should be 1

            // Act
            bool result = Movie.Insert(movie2); // Attempt to insert movie with existing ID (20)

            // Assert
            Assert.IsFalse(result, "Insert should return false for a movie with an existing ID.");
            Assert.AreEqual(listCountBeforeInsert, Movie.Read().Count(), "Movie list count should not change when ID exists.");
            Assert.IsFalse(Movie.Read().Any(m => m.PrimaryTitle == "Second Movie Title"), "Movie list should not contain the second movie.");
        }

        [TestMethod()]
        public void Insert_Static_Existing_Movie_Title_Case_Insensitive_Should_Return_False()
        {
            // Arrange
            var movie1 = new Movie(30, "url1", "Existing Title Test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            Movie.Insert(movie1); // Insert successful, movie has ID 30

            // Create another movie with a DIFFERENT ID but the SAME title (different case)
            var movie2 = new Movie(35, "url2", "existing title test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);

            int listCountBeforeInsert = Movie.Read().Count(); // Should be 1

            // Act
            bool result = Movie.Insert(movie2); // Attempt to insert movie with existing Title (case-insensitive)

            // Assert
            Assert.IsFalse(result, "Insert should return false for a movie with an existing Title (case-insensitive).");
            Assert.AreEqual(listCountBeforeInsert, Movie.Read().Count(), "Movie list count should not change when Title exists.");
            Assert.IsFalse(Movie.Read().Any(m => m.Id == 35), "Movie list should not contain the second movie (ID 35).");
        }

        #endregion Insert Static Method Testing

        #region Read Static Method Testing
        [TestMethod()]
        public void Read_Static_When_List_Is_Empty_Should_Return_Empty_Enumerable()
        {
            // Arrange (List is reset in TestInitialize)

            // Act
            var movies = Movie.Read();

            // Assert
            Assert.IsNotNull(movies, "Read should return an enumerable, not null.");
            Assert.AreEqual(0, movies.Count(), "Read should return an empty enumerable when the list is empty.");
        }

        [TestMethod()]
        public void Read_Static_When_List_Has_Movies_Should_Return_All_Movies()
        {
            // Arrange
            var movie1 = new Movie(100, "url1", "Movie 1", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            var movie2 = new Movie(101, "url2", "Movie 2", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            Movie.Insert(movie1);
            Movie.Insert(movie2);

            // Act
            var movies = Movie.Read();

            // Assert
            Assert.IsNotNull(movies);
            Assert.AreEqual(2, movies.Count(), "Read should return the correct number of movies.");
            Assert.IsTrue(movies.Any(m => m.Id == 100 && m.PrimaryTitle == "Movie 1"), "Read should contain the first inserted movie.");
            Assert.IsTrue(movies.Any(m => m.Id == 101 && m.PrimaryTitle == "Movie 2"), "Read should contain the second inserted movie.");
        }
        #endregion Read Static Method Testing

        #region GetByTitle Static Method Testing

        // Helper method to add test data for title/date tests
        private void SetupTestDataForSearch()
        {
            // Use specific, distinct IDs
            Movie.Insert(new Movie(200, "url", "The Shawshank Redemption", "desc", "img", 1994, new DateTime(1994, 10, 14), "en", 0, 0, "Drama", false, 142, 9.3f, 2000000));
            Movie.Insert(new Movie(201, "url", "The Godfather", "desc", "img", 1972, new DateTime(1972, 3, 24), "en", 0, 0, "Crime", false, 175, 9.2f, 1500000));
            Movie.Insert(new Movie(202, "url", "The Dark Knight", "desc", "img", 2008, new DateTime(2008, 7, 18), "en", 0, 0, "Action", false, 152, 9.0f, 2200000));
            Movie.Insert(new Movie(203, "url", "Pulp Fiction", "desc", "img", 1994, new DateTime(1994, 10, 14), "en", 0, 0, "Crime", false, 154, 8.9f, 1700000));
            Movie.Insert(new Movie(204, "url", "Schindler's List", "desc", "img", 1993, new DateTime(1994, 2, 4), "en", 0, 0, "Biography", false, 195, 8.9f, 1100000)); // Released early 1994
        }

        [TestMethod()]
        public void GetByTitle_Partial_Match_Returns_Matching_Movies()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "dark"; // Should match "The Dark Knight"

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("The Dark Knight", results[0].PrimaryTitle);
            Assert.AreEqual(202, results[0].Id); // Verify correct movie ID
        }

        [TestMethod()]
        public void GetByTitle_Multiple_Matches_Returns_All_Matching_Movies()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "the"; // Should match "The Shawshank...", "The Godfather", "The Dark Knight"

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(3, results.Count);
            Assert.IsTrue(results.Any(m => m.Id == 200), "Should contain The Shawshank Redemption (ID 200)");
            Assert.IsTrue(results.Any(m => m.Id == 201), "Should contain The Godfather (ID 201)");
            Assert.IsTrue(results.Any(m => m.Id == 202), "Should contain The Dark Knight (ID 202)");
        }

        [TestMethod()]
        public void GetByTitle_Exact_Match_Case_Insensitive_Returns_Matching_Movie()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "the godfather"; // Lowercase

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("The Godfather", results[0].PrimaryTitle);
            Assert.AreEqual(201, results[0].Id); // Verify correct movie ID
        }

        [TestMethod()]
        public void GetByTitle_No_Match_Returns_Empty_List()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "NonExistentMovie";

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod()]
        public void GetByTitle_Empty_String_Returns_All_Movies()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "";
            int expectedCount = Movie.Read().Count();

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(expectedCount, results.Count);
        }

        [TestMethod()]
        public void GetByTitle_Null_String_Returns_All_Movies()
        {
            // Arrange
            SetupTestDataForSearch();
            string? searchTerm = null;
            int expectedCount = Movie.Read().Count();

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(expectedCount, results.Count);
        }

        [TestMethod()]
        public void GetByTitle_Whitespace_String_Returns_All_Movies()
        {
            // Arrange
            SetupTestDataForSearch();
            string searchTerm = "   ";
            int expectedCount = Movie.Read().Count();

            // Act
            var results = Movie.GetByTitle(searchTerm).ToList();

            // Assert
            Assert.AreEqual(expectedCount, results.Count);
        }
        #endregion GetByTitle Static Method Testing

        #region GetByReleaseDate Static Method Testing

        [TestMethod()]
        public void GetByReleaseDate_RangeIncludesMultipleMovies_ReturnsMatchingMovies()
        {
            // Arrange
            SetupTestDataForSearch();
            DateTime startDate = new DateTime(1994, 1, 1);
            DateTime endDate = new DateTime(1994, 12, 31);

            // Act
            // Note: Your implementation returns List<Movie>, not IEnumerable<Movie>
            var results = Movie.GetByReleaseDate(startDate, endDate);

            // Assert
            Assert.AreEqual(3, results.Count); // Shawshank (Oct 14), Pulp Fiction (Oct 14), Schindler's List (Feb 4)
            Assert.IsTrue(results.Any(m => m.Id == 200)); // Shawshank
            Assert.IsTrue(results.Any(m => m.Id == 203)); // Pulp Fiction
            Assert.IsTrue(results.Any(m => m.Id == 204)); // Schindler's List
        }

        [TestMethod()]
        public void GetByReleaseDate_RangeIncludesOneMovie_ReturnsSingleMovie()
        {
            // Arrange
            SetupTestDataForSearch();
            DateTime startDate = new DateTime(2008, 1, 1);
            DateTime endDate = new DateTime(2008, 12, 31);

            // Act
            var results = Movie.GetByReleaseDate(startDate, endDate);

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(202, results[0].Id); // The Dark Knight (July 18, 2008)
        }

        [TestMethod()]
        public void GetByReleaseDate_ExactDateMatch_ReturnsMatchingMovies()
        {
            // Arrange
            SetupTestDataForSearch();
            // Test inclusive behavior of >= and <=
            DateTime exactDate = new DateTime(1994, 10, 14);

            // Act
            var results = Movie.GetByReleaseDate(exactDate, exactDate);

            // Assert
            Assert.AreEqual(2, results.Count); // Shawshank, Pulp Fiction released on this exact date
            Assert.IsTrue(results.Any(m => m.Id == 200)); // Shawshank
            Assert.IsTrue(results.Any(m => m.Id == 203)); // Pulp Fiction
        }

        [TestMethod()]
        public void GetByReleaseDate_RangeIncludesNoMovies_ReturnsEmptyList()
        {
            // Arrange
            SetupTestDataForSearch();
            DateTime startDate = new DateTime(2020, 1, 1);
            DateTime endDate = new DateTime(2021, 12, 31);

            // Act
            var results = Movie.GetByReleaseDate(startDate, endDate);

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod()]
        public void GetByReleaseDate_StartDateAfterEndDate_ReturnsEmptyList()
        {
            // Arrange
            SetupTestDataForSearch();
            DateTime startDate = new DateTime(2000, 1, 1);
            DateTime endDate = new DateTime(1990, 1, 1); // End date is before start date

            // Act
            var results = Movie.GetByReleaseDate(startDate, endDate);

            // Assert
            // Your current implementation's loop condition (m.ReleaseDate >= startDate && m.ReleaseDate <= endDate)
            // will naturally result in no matches when startDate > endDate.
            Assert.AreEqual(0, results.Count);
        }

        #endregion GetByReleaseDate Static Method Testing
    }
}