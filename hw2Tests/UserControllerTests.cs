using Microsoft.VisualStudio.TestTools.UnitTesting;
using hw2.Controllers;
using hw2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace hw2Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private UserController _controller = null!; // Initialize with null-forgiving operator

        // Reset static user list before each test
        [TestInitialize]
        public void Setup()
        {
            User.ResetUsersList();
            // Note: We cannot easily reset the static IdCounter here.
            _controller = new UserController(); // Controller is definitely assigned here
        }

        [TestMethod]
        public void GetAllUsers_ReturnsAllUsersFromModel()
        {
            // Arrange
            // Add some users directly via the model's static method for the test
            User.Insert(new User { Name = "User 1", Email = "user1@test.com", Password = "p1", Id = 0 });
            User.Insert(new User { Name = "User 2", Email = "user2@test.com", Password = "p2", Id = 0 });

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<User>));
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(u => u.Email == "user1@test.com"));
            Assert.IsTrue(result.Any(u => u.Email == "user2@test.com"));
        }

        [TestMethod]
        public void Post_ValidUser_ReturnsOkResultWithUserWithId()
        {
            // Arrange
            var newUser = new User
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "password123",
                Active = true,
                Id = 0 // Ensure Id is 0 for insertion
            };

            // Act
            var result = _controller.Post(newUser);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<User>));
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(User));

            var returnedUser = okResult.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreEqual(newUser.Email, returnedUser.Email);
            Assert.AreEqual(newUser.Name, returnedUser.Name);
            Assert.IsTrue(returnedUser.Id > 0, "Returned user should have a non-zero ID assigned by Insert.");

            // Verify user was actually added to the static list
            var users = User.Read(); // Corrected: Removed 'Models.' prefix
            Assert.IsTrue(users.Any(u => u.Email == newUser.Email && u.Id == returnedUser.Id));
        }

        [TestMethod]
        public void Post_InsertionFailsDuplicateEmail_ReturnsConflictResult()
        {
            // Arrange
            // Insert a user first
            User.Insert(new User { Name = "Existing", Email = "duplicate@example.com", Password = "pw", Id = 0 });

            // Try to insert another with the same email
            var conflictingUser = new User
            {
                Name = "Conflict User",
                Email = "duplicate@example.com", // Duplicate email
                Password = "password456",
                Id = 0
            };

            // Act
            var result = _controller.Post(conflictingUser);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<User>));
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));

            var conflictResult = result.Result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual("User could not be inserted. Email may already exist or data is invalid.", conflictResult.Value);
        }

        [TestMethod]
        public void Post_InsertionFailsUserWithIdNotZero_ReturnsConflictResult()
        {
            // Arrange
            var userWithId = new User
            {
                Name = "Has ID",
                Email = "hasid@example.com",
                Password = "password789",
                Id = 5 // Non-zero ID
            };

            // Act
            var result = _controller.Post(userWithId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<User>));
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));

            var conflictResult = result.Result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual("User could not be inserted. Email may already exist or data is invalid.", conflictResult.Value);
        }

        // ... Keep placeholder tests for Get(id), Put, Delete ...
        [TestMethod]
        public void GetById_NotImplemented_ReturnsPlaceholder()
        {
            // Arrange
            int testId = 1;

            // Act
            var result = _controller.Get(testId);

            // Assert
            Assert.AreEqual("value", result);
            // TODO: Update this test when the Get(id) method is implemented
        }

        [TestMethod]
        public void Put_NotImplemented_DoesNothing()
        {
            // Arrange
            int testId = 1;
            string testValue = "test data";

            // Act
            // Call the method - it currently has no return type or logic to assert against
            _controller.Put(testId, testValue);

            // Assert
            // No assertion possible until implemented. Test passes if no exception.
            Assert.IsTrue(true);
            // TODO: Update this test when the Put method is implemented
        }

        [TestMethod]
        public void Delete_NotImplemented_DoesNothing()
        {
            // Arrange
            int testId = 1;

            // Act
            // Call the method - it currently has no return type or logic to assert against
            _controller.Delete(testId);

            // Assert
            // No assertion possible until implemented. Test passes if no exception.
            Assert.IsTrue(true);
            // TODO: Update this test when the Delete method is implemented
        }
    }
}
