using System.ComponentModel.DataAnnotations;

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

        #region Methods
        //---------------------------------------- Methods ----------------------------------------//
        
        /// <summary>
        /// Inserts this user into the static usersList if a user with the same id does not already exist.
        /// </summary>
        /// <returns>
        /// True if the user was inserted; otherwise, false.
        /// </returns>
        public bool Insert()
        {
            foreach (var user in usersList)
            {
                if (user.id == this.id)
                {
                    return false; // User already exists
                }
            }
            usersList.Add(this);
            return true; // Insertion successful
        }

        /// <summary>
        /// Retrieves the complete list of users.
        /// </summary>
        /// <returns>
        /// A reference to the static list containing all users.
        /// </returns>
        public List<User> Read()
        {
            return usersList;
        }
        //--------------------------------------------------------------------------------------------//
        #endregion Methods
    }
}
