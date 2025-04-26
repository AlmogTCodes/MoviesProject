using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace hw2
{
    public class User
    {
        #region Properties
        //---------------------------------------- Properties ----------------------------------------//
        private int id;
        private string name;
        private string email;
        private string password;
        private bool active;

        private static List<User> usersList = new List<User>();
        //--------------------------------------------------------------------------------------------//
        #endregion Properties

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

        #region Get-Set Methods
        //---------------------------------------- Get-Set Methods ----------------------------------------//
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int Id
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email
        {
            get => email;
            set => email = value;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get => password;
            set => password = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool Active
        {
            get => active;
            set => active = value;
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
            usersList.Clear();
        }

        //--------------------------------------------------------------------------------------------//
        #endregion Temporary Tests Methods

        #region Methods
        //---------------------------------------- Methods ----------------------------------------//

        /// <summary>
        /// Inserts a user into the static usersList if a user with the same id does not already exist.
        /// </summary>
        /// <param name="userToInsert">The user object to insert.</param>
        /// <returns>
        /// True if the user was inserted; otherwise, false (if null or ID already exists).
        /// </returns>
        public static bool Insert(User userToInsert)
        {
            // Check if the user object itself is null
            if (userToInsert == null)
            {
                return false; // Cannot insert a null user
            }

            // Check if a user with the same ID already exists
            if (usersList.Any(user => user.Id == userToInsert.Id))
            {
                return false; // User with this ID already exists
            }

            // If ID is unique and user is not null, add to the list
            usersList.Add(userToInsert);
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
            return usersList.ToList();
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Methods
    }
}
