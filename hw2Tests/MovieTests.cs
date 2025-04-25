using Microsoft.VisualStudio.TestTools.UnitTesting;
using hw2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2.Tests
{
    [TestClass()]
    public class MovieTests
    {
        [TestInitialize()]
        public void TestInitialize()
        {
            //Using Temporary Methods from Movies.cs that will be deleted to access and
            //reset those fields with each test run.

            Movie.ResetMoviesList();
            Movie.ResetNumberOfMovies();
        }

        // Test for the default constructor
        [TestMethod()]
        public void Movie_DefaultConstructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var movie = new Movie();

            // Assert
            Assert.IsNotNull(movie);
        }

        // Test for the parameterized constructor
        [TestMethod()]
        public void Movie_Parameterized_Constructor_Should_Set_Properties()
        {
            // Arrange
            var releaseDate = new DateTime(2023 - 1 - 1);

            // Act - Note: The constructor uses NumberOfMovies++ for Id, so the passed id is ignored.
            var movie = new Movie(0, "url", "Test Title", "desc", "img", 2023, releaseDate, "en", 100, 200, "Action", false, 120, 8.5f, 1000);

            // Assert
            Assert.AreEqual(0, movie.Id); // First movie gets ID 0
            Assert.AreEqual("Test Title", movie.PrimaryTitle);
            Assert.AreEqual("desc", movie.Description);
            Assert.AreEqual("img", movie.PrimaryImage);
            Assert.AreEqual(2023, movie.Year);
            Assert.AreEqual(releaseDate, movie.ReleaseDate);
            Assert.AreEqual("en", movie.Language);
            Assert.AreEqual(100, movie.Budget);
            Assert.AreEqual(200, movie.GrossWorldwide);
            Assert.AreEqual("Action", movie.Genres);
            Assert.AreEqual(false, movie.IsAdult);
            Assert.AreEqual(120, movie.RuntimeMinutes);
            Assert.AreEqual(8.5f, movie.AverageRating);
            Assert.AreEqual(1000, movie.NumVotes);
            Assert.AreEqual(1, Movie.NumberOfMovies); // Counter should be incremented
        }

        [TestMethod()]
        public void Insert_Static_New_Movie_Should_Return_True_And_Add_To_List()
        {
            // Arrange
            // Constructor assigns ID using NumberOfMovies++, so first movie gets ID 0
            var movie = new Movie(99, "url1", "Insert Static Test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            int initialCount = Movie.Read().Count(); // Should be 0 after TestInitialize

            // Act
            bool result = Movie.Insert(movie); // Use the static Insert method

            // Assert
            Assert.IsTrue(result, "Static Insert should return true for a new movie.");
            Assert.AreEqual(initialCount + 1, Movie.Read().Count(), "Movie list count should increase by 1.");
            Assert.AreEqual(0, movie.Id, "Movie ID should have been assigned by constructor."); // Verify ID assignment
            Assert.IsTrue(Movie.Read().Contains(movie), "Movie list should contain the inserted movie.");
            Assert.AreEqual(1, Movie.NumberOfMovies, "NumberOfMovies should be incremented by constructor.");
        }

        [TestMethod()]
        public void Insert_Static_Existing_Movie_Title_Should_Return_False()
        {
            // Arrange
            // First movie gets ID 0 from constructor
            var movie1 = new Movie(0, "url1", "Existing Title Test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            Movie.Insert(movie1); // Insert the first movie (List count: 1, NumberOfMovies: 1)

            // Create another movie instance. Constructor assigns ID 1. Title is the same as movie1.
            var movie2 = new Movie(1, "url2", "Existing Title Test", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            // NumberOfMovies is now 2

            int listCountBeforeInsert = Movie.Read().Count(); // Should be 1
            int movieCountBeforeInsert = Movie.NumberOfMovies; // Should be 2

            // Act
            bool result = Movie.Insert(movie2); // Attempt to insert movie with existing Title

            // Assert
            Assert.IsFalse(result, "Static Insert should return false for a movie with an existing Title.");
            Assert.AreEqual(listCountBeforeInsert, Movie.Read().Count(), "Movie list count should not change.");
            Assert.AreEqual(movieCountBeforeInsert, Movie.NumberOfMovies, "NumberOfMovies should not be changed by failed Insert.");
            Assert.AreEqual(1, movie2.Id, "Second movie ID should remain 1."); // Verify ID wasn't somehow changed
            Assert.IsFalse(Movie.Read().Any(m => m.Id == 1), "Movie list should not contain the second movie."); // Ensure movie2 wasn't added
        }

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
            var movie1 = new Movie(0, "url1", "Movie 1", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            var movie2 = new Movie(1, "url2", "Movie 2", "desc", "img", 2024, DateTime.Now, "en", 1, 2, "Test", false, 90, 7.0f, 50);
            Movie.Insert(movie1);
            Movie.Insert(movie2);

            // Act
            var movies = Movie.Read();

            // Assert
            Assert.IsNotNull(movies);
            Assert.AreEqual(2, movies.Count(), "Read should return the correct number of movies.");
            Assert.IsTrue(movies.Any(m => m.Id == 0 && m.PrimaryTitle == "Movie 1"), "Read should contain the first inserted movie.");
            Assert.IsTrue(movies.Any(m => m.Id == 1 && m.PrimaryTitle == "Movie 2"), "Read should contain the second inserted movie.");
        }
    }
}