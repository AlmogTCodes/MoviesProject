using Microsoft.VisualStudio.TestTools.UnitTesting;
using hw2.Models;
using System.Linq;
using System.Collections.Generic;

namespace hw2Tests
{
    [TestClass]
    public class UserTests
    {
        // Reset the static list before each test for isolation
        [TestInitialize]
        public void TestInitialize()
        {
            User.ResetUsersList();
            // Note: We cannot easily reset the static IdCounter here as it has a private setter.
            // Tests needing specific IDs might be brittle or require different setup.
            // For Insert tests, we mainly care *that* an ID is assigned.
        }

        [TestMethod]
        public void Constructor_Default_CreatesInstance()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(0, user.Id); // Default ID should be 0
        }

        [TestMethod]
        public void Constructor_Parameterized_AssignsProperties()
        {
            // Arrange
            int id = 1;
            string name = "Test Name";
            string email = "test@example.com";
            string password = "password";
            bool active = true;

            // Act
            var user = new User(id, name, email, password, active);

            // Assert
            Assert.AreEqual(id, user.Id);
            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(active, user.Active);
        }

        [TestMethod]
        public void Properties_GetSet_WorkCorrectly()
        {
            // Arrange
            var user = new User();
            int id = 5;
            string name = "Another Name";
            string email = "another@example.com";
            string password = "securepass";
            bool active = false;

            // Act
            user.Id = id;
            user.Name = name;
            user.Email = email;
            user.Password = password;
            user.Active = active;

            // Assert
            Assert.AreEqual(id, user.Id);
            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(active, user.Active);
        }

        [TestMethod]
        public void Insert_ValidNewUser_ReturnsTrueAndAddsUserWithId()
        {
            // Arrange
            var newUser = new User { Name = "New", Email = "new@example.com", Password = "pass", Active = true, Id = 0 }; // Ensure Id is 0
            int initialCount = User.Read().Count();

            // Act
            bool result = User.Insert(newUser);

            // Assert
            Assert.IsTrue(result, "Insert should return true for a valid new user.");
            Assert.AreEqual(initialCount + 1, User.Read().Count(), "User count should increment by 1.");
            Assert.IsTrue(newUser.Id > 0, "Inserted user should have an ID assigned (greater than 0).");
            var insertedUser = User.Read().FirstOrDefault(u => u.Email == "new@example.com");
            Assert.IsNotNull(insertedUser, "User should be found in the list after insertion.");
            Assert.AreEqual(newUser.Name, insertedUser.Name);
        }

        [TestMethod]
        public void Insert_NullUser_ReturnsFalse()
        {
            // Arrange
            User? userToInsert = null; // Declare as nullable
            int initialCount = User.Read().Count();

            // Act
            #pragma warning disable CS8604 // Possible null reference argument.
            bool result = User.Insert(userToInsert);
            #pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            Assert.IsFalse(result, "Insert should return false for a null user.");
            Assert.AreEqual(initialCount, User.Read().Count(), "User count should not change.");
        }

        [TestMethod]
        public void Insert_UserWithNonZeroId_ReturnsFalse()
        {
            // Arrange
            var existingUser = new User { Id = 5, Name = "Existing", Email = "existing@example.com", Password = "pass" };
            int initialCount = User.Read().Count();

            // Act
            bool result = User.Insert(existingUser);

            // Assert
            Assert.IsFalse(result, "Insert should return false for a user with non-zero ID.");
            Assert.AreEqual(initialCount, User.Read().Count(), "User count should not change.");
        }

        [TestMethod]
        public void Insert_UserWithNullOrWhitespaceEmail_ReturnsFalse()
        {
            // Arrange
            // Removed the null email case as Email property is non-nullable in User.cs
            var userEmptyEmail = new User { Id = 0, Name = "Empty Email", Email = "", Password = "pass" };
            var userWhitespaceEmail = new User { Id = 0, Name = "Whitespace Email", Email = "   ", Password = "pass" };
            int initialCount = User.Read().Count();

            // Act
            bool resultEmpty = User.Insert(userEmptyEmail);
            bool resultWhitespace = User.Insert(userWhitespaceEmail);

            // Assert
            Assert.IsFalse(resultEmpty, "Insert should return false for empty email.");
            Assert.IsFalse(resultWhitespace, "Insert should return false for whitespace email.");
            Assert.AreEqual(initialCount, User.Read().Count(), "User count should not change.");
        }

        [TestMethod]
        public void Insert_DuplicateEmail_ReturnsFalse()
        {
            // Arrange
            var user1 = new User { Id = 0, Name = "First", Email = "duplicate@example.com", Password = "pass1" };
            User.Insert(user1); // Insert the first user
            var user2 = new User { Id = 0, Name = "Second", Email = "duplicate@example.com", Password = "pass2" }; // Same email
            int initialCount = User.Read().Count();

            // Act
            bool result = User.Insert(user2);

            // Assert
            Assert.IsFalse(result, "Insert should return false for a duplicate email.");
            Assert.AreEqual(initialCount, User.Read().Count(), "User count should not change.");
        }

        [TestMethod]
        public void Insert_DuplicateEmailCaseInsensitive_ReturnsFalse()
        {
            // Arrange
            var user1 = new User { Id = 0, Name = "First", Email = "case@example.com", Password = "pass1" };
            User.Insert(user1); // Insert the first user
            var user2 = new User { Id = 0, Name = "Second", Email = "CASE@example.com", Password = "pass2" }; // Same email, different case
            int initialCount = User.Read().Count();

            // Act
            bool result = User.Insert(user2);

            // Assert
            Assert.IsFalse(result, "Insert should return false for a duplicate email (case-insensitive).");
            Assert.AreEqual(initialCount, User.Read().Count(), "User count should not change.");
        }

        [TestMethod]
        public void Read_AfterReset_ReturnsEmptyList()
        {
            // Arrange (Setup done in TestInitialize)

            // Act
            var users = User.Read();

            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(0, users.Count());
        }

        [TestMethod]
        public void Read_AfterInserts_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { Id = 0, Name = "User1", Email = "user1@example.com", Password = "p1" };
            var user2 = new User { Id = 0, Name = "User2", Email = "user2@example.com", Password = "p2" };
            User.Insert(user1);
            User.Insert(user2);

            // Act
            var users = User.Read();

            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count());
            Assert.IsTrue(users.Any(u => u.Email == "user1@example.com"));
            Assert.IsTrue(users.Any(u => u.Email == "user2@example.com"));
        }

        [TestMethod]
        public void Read_ReturnsCopyOfList()
        {
            // Arrange
            var user1 = new User { Id = 0, Name = "User1", Email = "user1@example.com", Password = "p1" };
            User.Insert(user1);
            var listBeforeRead = User.UsersList; // Access internal list for comparison (usually not ideal but needed here)
            int countBeforeRead = listBeforeRead.Count;

            // Act
            var readList = User.Read().ToList(); // Call Read and convert to List
            readList.Add(new User { Id = 0, Name = "Temp", Email = "temp@example.com", Password="t"}); // Modify the returned list

            // Assert
            Assert.AreNotSame(listBeforeRead, readList, "Read should return a different list instance.");
            Assert.AreEqual(countBeforeRead, User.Read().Count(), "Modifying the returned list should not affect the internal list.");
        }
    }
}
