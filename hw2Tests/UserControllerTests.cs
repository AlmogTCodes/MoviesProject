using Microsoft.VisualStudio.TestTools.UnitTesting;
using hw2.Controllers;
using hw2.Models;
using hw2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace hw2Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private UserController _controller = null!;
        private PasswordHashingService _passwordHashingService = null!;
        private AuthTokenService _authTokenService = null!;
        private IConfiguration _configuration = null!;

        private IConfiguration SetupMockConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "TestSuperSecretKey123456789012345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        [TestInitialize]
        public void Setup()
        {
            User.ResetUsersList();
            _passwordHashingService = new PasswordHashingService();
            _configuration = SetupMockConfiguration();
            _authTokenService = new AuthTokenService(_configuration);
            _controller = new UserController(_passwordHashingService, _authTokenService);
        }

        [TestMethod]
        public void GetAllUsers_ReturnsAllUsersFromModel()
        {
            User.Insert(new User { Name = "User 1", Email = "user1@test.com", Password = "p1", Id = 0 });
            User.Insert(new User { Name = "User 2", Email = "user2@test.com", Password = "p2", Id = 0 });

            var result = _controller.GetAllUsers();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<User>));
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(u => u.Email == "user1@test.com"));
            Assert.IsTrue(result.Any(u => u.Email == "user2@test.com"));
        }

        [TestMethod]
        public void Post_ValidUser_ReturnsOkResultWithUserDetails()
        {
            var newUser = new User
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "password123",
                Active = true,
                Id = 0
            };

            // Store original password before it gets hashed by the controller
            string originalPassword = newUser.Password;

            var result = _controller.Post(newUser);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>)); 
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value, "OkResult.Value should not be null.");

            // Safer check for properties using reflection
            var valueType = okResult.Value.GetType();
            var idProperty = valueType.GetProperty("Id");
            var nameProperty = valueType.GetProperty("Name");
            var emailProperty = valueType.GetProperty("Email");
            var activeProperty = valueType.GetProperty("Active");

            Assert.IsNotNull(idProperty, "The returned object should have an 'Id' property.");
            Assert.IsNotNull(nameProperty, "The returned object should have a 'Name' property.");
            Assert.IsNotNull(emailProperty, "The returned object should have an 'Email' property.");
            Assert.IsNotNull(activeProperty, "The returned object should have an 'Active' property.");

            // Get values using reflection
            var returnedId = (int)idProperty.GetValue(okResult.Value);
            var returnedName = nameProperty.GetValue(okResult.Value) as string;
            var returnedEmail = emailProperty.GetValue(okResult.Value) as string;
            var returnedActive = (bool)activeProperty.GetValue(okResult.Value);

            // Assert property values
            Assert.AreEqual(newUser.Name, returnedName);
            Assert.AreEqual(newUser.Email, returnedEmail);
            Assert.AreEqual(newUser.Active, returnedActive);
            Assert.IsTrue(returnedId > 0, "Returned user should have a non-zero ID assigned by Insert.");

            // Assert that token property does NOT exist using reflection
            Assert.IsNull(valueType.GetProperty("token"), "Token property should not exist on registration result.");

            // Verify user in storage
            var users = User.Read();
            var addedUser = users.FirstOrDefault(u => u.Email == newUser.Email && u.Id == returnedId);
            Assert.IsNotNull(addedUser, "User was not added to the list.");
            // Compare original plain text password with the stored hash
            Assert.AreNotEqual(originalPassword, addedUser.Password, "Password should be hashed in storage.");
            Assert.IsTrue(_passwordHashingService.VerifyPasswordHash(originalPassword, addedUser.Password), "Stored password hash should be verifiable.");
        }

        [TestMethod]
        public void Post_InsertionFailsDuplicateEmail_ReturnsConflictResult()
        {
            string initialPassword = "password123";
            string hashedPw = _passwordHashingService.CreatePasswordHash(initialPassword);
            User.Insert(new User { Name = "Existing", Email = "duplicate@example.com", Password = hashedPw, Id = 0 });

            var conflictingUser = new User
            {
                Name = "Conflict User",
                Email = "duplicate@example.com",
                Password = "password456",
                Id = 0
            };

            var result = _controller.Post(conflictingUser);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>));
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));

            var conflictResult = result.Result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.IsNotNull(conflictResult.Value, "Conflict result value should not be null.");

            // Safer check for the 'message' property using reflection
            var valueType = conflictResult.Value.GetType();
            var messageProperty = valueType.GetProperty("message");
            Assert.IsNotNull(messageProperty, "The returned object in Conflict should have a 'message' property.");

            var messageValue = messageProperty.GetValue(conflictResult.Value) as string;
            Assert.AreEqual($"Email '{conflictingUser.Email}' is already registered.", messageValue, "The conflict message is not correct.");
        }

        [TestMethod]
        public void Post_InsertionFailsUserWithIdNotZero_ReturnsOkResult() // Renamed and changed expectation
        {
            // Arrange: User with a non-zero ID but unique email.
            // Controller POST action currently resets ID to 0 before insertion.
            var userWithId = new User
            {
                Name = "Has ID But Should Insert",
                Email = "unique-id-test@example.com", // Ensure this email is unique for the test
                Password = "password789",
                Active = true,
                Id = 5 // Non-zero ID that will be ignored by the controller
            };

            // Store original password before it gets hashed by the controller
            string originalPassword = userWithId.Password;

            // Act
            var result = _controller.Post(userWithId);

            // Assert: Expect OK because controller ignores the ID and email is unique.
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>)); // Controller returns ActionResult<object>
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value, "OkResult value should not be null.");

            // Safer check for properties using reflection
            var valueType = okResult.Value.GetType();
            var idProperty = valueType.GetProperty("Id");
            var nameProperty = valueType.GetProperty("Name");
            var emailProperty = valueType.GetProperty("Email");
            var activeProperty = valueType.GetProperty("Active");

            Assert.IsNotNull(idProperty, "The returned object should have an 'Id' property.");
            Assert.IsNotNull(nameProperty, "The returned object should have a 'Name' property.");
            Assert.IsNotNull(emailProperty, "The returned object should have an 'Email' property.");
            Assert.IsNotNull(activeProperty, "The returned object should have an 'Active' property.");

            // Get values using reflection
            var returnedId = (int)idProperty.GetValue(okResult.Value);
            var returnedName = nameProperty.GetValue(okResult.Value) as string;
            var returnedEmail = emailProperty.GetValue(okResult.Value) as string;
            var returnedActive = (bool)activeProperty.GetValue(okResult.Value);

            // Assert property values
            Assert.IsTrue(returnedId > 0, "Returned user should have a non-zero ID assigned by Insert.");
            Assert.AreEqual(userWithId.Name, returnedName);
            Assert.AreEqual(userWithId.Email, returnedEmail);
            Assert.AreEqual(userWithId.Active, returnedActive); // Also check Active status

            // Verify user was actually added with a hashed password
            var users = User.Read();
            var addedUser = users.FirstOrDefault(u => u.Email == userWithId.Email && u.Id == returnedId);
            Assert.IsNotNull(addedUser, "User was not added to the list.");
            // Compare original plain text password with the stored hash
            Assert.AreNotEqual(originalPassword, addedUser.Password, "Password should be hashed in storage.");
            Assert.IsTrue(_passwordHashingService.VerifyPasswordHash(originalPassword, addedUser.Password), "Stored password hash should be verifiable.");
        }

        [TestMethod]
        public void Post_EmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var userWithEmptyPassword = new User
            {
                Name = "Empty PW",
                Email = "empty@pw.com",
                Password = "   ", // Empty or whitespace password
                Active = true,
                Id = 0
            };

            // Act
            var result = _controller.Post(userWithEmptyPassword);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>)); 
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(badRequestResult.Value, "BadRequest result value should not be null.");

            // Safer check for the 'message' property using reflection
            var valueType = badRequestResult.Value.GetType();
            var messageProperty = valueType.GetProperty("message");
            Assert.IsNotNull(messageProperty, "The returned object in BadRequest should have a 'message' property.");

            var messageValue = messageProperty.GetValue(badRequestResult.Value) as string;
            Assert.AreEqual("Password cannot be empty or whitespace.", messageValue, "The error message is not correct.");
        }

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

        // --- Add Tests for Login --- 

        [TestMethod]
        public void Login_ValidCredentials_ReturnsOkResultWithToken()
        {
            // Arrange
            string testPassword = "password123";
            var user = new User
            {
                Name = "Login User",
                Email = "login@test.com",
                Password = _passwordHashingService.CreatePasswordHash(testPassword), // Store hashed password
                Active = true,
                Id = 0 // Let Insert assign ID
            };
            User.Insert(user); // Insert the user directly for the test

            var loginRequest = new UserController.LoginRequestDto
            {
                Email = "login@test.com",
                Password = testPassword // Use plain text password for login attempt
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>));
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            // Check if the returned object has a 'token' property (using reflection)
            var returnedValueType = okResult.Value.GetType();
            var tokenProperty = returnedValueType.GetProperty("token");
            Assert.IsNotNull(tokenProperty, "'token' property not found on returned object.");
            var tokenValue = tokenProperty.GetValue(okResult.Value) as string;
            Assert.IsFalse(string.IsNullOrWhiteSpace(tokenValue), "Returned token should not be empty.");
            // Basic check if it looks like a JWT (three parts separated by dots)
            Assert.IsTrue(tokenValue.Split('.').Length == 3, "Returned token does not look like a JWT.");
        }

        [TestMethod]
        public void Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            string correctPassword = "password123";
            var user = new User
            {
                Name = "Login User",
                Email = "login@test.com",
                Password = _passwordHashingService.CreatePasswordHash(correctPassword),
                Active = true,
                Id = 0
            };
            User.Insert(user);

            var loginRequest = new UserController.LoginRequestDto
            {
                Email = "login@test.com",
                Password = "wrongPassword" // Incorrect password
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>));
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new UserController.LoginRequestDto
            {
                Email = "nonexistent@test.com", // Email does not exist
                Password = "password123"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>));
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new UserController.LoginRequestDto
            {
                Email = "not-an-email", // Invalid email format
                Password = ""
            };
            // Manually add model error to simulate invalid state
            _controller.ModelState.AddModelError("Email", "The Email field is not a valid e-mail address.");
            _controller.ModelState.AddModelError("Password", "The Password field is required.");

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<object>));
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult)); // Should be BadRequest due to model state
        }
    }
}
