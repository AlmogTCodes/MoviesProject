using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace hw2.Models
{
    public class User
    {
        #region Properties
        //---------------------------------------- Properties ----------------------------------------//
        private int _id = -1; //default -1 in the insert method meaning not registered yet
        private string _name;
        private string _email;
        private string _password;
        private bool _active;

        private static List<User> _usersList = new List<User>();
        private static int _idCounter = 0;
        //--------------------------------------------------------------------------------------------//
        #endregion Properties


        #region Get-Set Methods
        //---------------------------------------- Get-Set Methods ----------------------------------------//

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public bool Active
        {
            get => _active;
            set => _active = value;
        }
        public static int IdCounter
        {
            get => _idCounter;
            private set => _idCounter = value;
        }
        public static List<User> UsersList
        {
            get => _usersList;
            set => _usersList = value;
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Get-Set Methods


        #region Temporary Tests Methods
        //---------------------------------------- Temporary Tests Methods ----------------------------------------//

        /// <summary>
        /// Clears the static users list. Intended for testing purposes.
        /// </summary>
        public static void ResetUsersList()
        {
            UsersList.Clear();
        }

        //--------------------------------------------------------------------------------------------//
        #endregion Temporary Tests Methods


        #region Constructors
        //---------------------------------------- Constructors ----------------------------------------//

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with specified details.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password for the user.</param>
        /// <param name="active">A value indicating whether the user is active.</param>
        public User(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Active = active;
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Constructors


        #region Methods
        //---------------------------------------- Methods ----------------------------------------//

        /// <summary>
        /// Inserts a user into the static usersList if the user is valid, new and the email is unique.
        /// Assigns a unique ID if the user's ID is -1 (default for new user).
        /// </summary>
        /// <param name="userToInsert">The user object to insert.</param>
        /// <returns>
        /// True if the user was inserted successfully; otherwise, false.
        /// </returns>
        public static bool Insert(User userToInsert)
        {
            // Check for null user or not a new user or invalid email
            if (userToInsert == null || userToInsert.Id != -1 || string.IsNullOrWhiteSpace(userToInsert.Email))
            {
                return false; // Cannot insert null user or not new user or user with empty/whitespace email.
            }

            // Check if a user with the same email already exists (case-insensitive)
            if (UsersList.Any(user => user.Email.Equals(userToInsert.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // User with this email already exists
            }

            // If the user ID is the default -1, assign a new unique ID
            userToInsert.Id = IdCounter++; // Assign current counter value, then increment.

            // Add the user to the list
            UsersList.Add(userToInsert);
            return true; // Insertion successful
        }

        /// <summary>
        /// Retrieves a copy of the complete list of users.
        /// </summary>
        /// <returns>
        /// An enumerable collection containing all users.
        /// </returns>
        public static IEnumerable<User> Read()
        {
            // Return a copy of the list to prevent modification of the original static list
            return UsersList.ToList();
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Methods
    }
}
