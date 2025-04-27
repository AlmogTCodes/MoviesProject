const userApiUrl = `${apiUrlBase}/User`; // Assuming port is defined globally or imported

// --- Session Management ---

function storeUserInfo(userData) {
    // Store necessary info AND the token
    sessionStorage.setItem('loggedInUser', JSON.stringify({
        name: userData.name,
        email: userData.email,
        token: userData.token // Store the token
    }));
}

function getUserInfo() {
    const user = sessionStorage.getItem('loggedInUser');
    return user ? JSON.parse(user) : null;
}

// New function to retrieve the token
function getTokenFromStorage() {
    const userInfo = getUserInfo();
    return userInfo ? userInfo.token : null;
}

function isLoggedIn() {
    return !!getUserInfo();
}

function logoutUser() {
    sessionStorage.removeItem('loggedInUser');
    // Redirect to login page after logout
    window.location.href = 'login.html';
}

// --- API Calls ---

function loginUser(email, password, successCallback, errorCallback) {
    const loginData = { email, password };
    console.log("Attempting login with:", loginData); // Debug log

    ajaxCall(
        "POST",
        `${userApiUrl}/login`, // Adjust endpoint if needed
        JSON.stringify(loginData),
        (response) => {
            console.log("Login successful:", response); // Debug log
            // Ensure the response structure includes the token
            const userData = {
                name: response.userName || 'User', // Use userName from response
                email: response.userEmail || email, // Use userEmail from response
                token: response.token // Get the token from the response
            };
            storeUserInfo(userData); // Store user info including the token
            if (successCallback) successCallback(userData);
        },
        (jqXHR, textStatus, errorThrown) => {
            console.error("Login failed:", textStatus, errorThrown, jqXHR.responseText); // Debug log
            let errorMessage = "Login failed. Please check your credentials.";
            if (jqXHR.responseJSON && jqXHR.responseJSON.message) {
                errorMessage = jqXHR.responseJSON.message;
            } else if (jqXHR.status === 401) {
                 errorMessage = "Invalid email or password.";
            }
             else if (jqXHR.responseText) {
                try {
                    const errorResponse = JSON.parse(jqXHR.responseText);
                    errorMessage = errorResponse.title || errorMessage;
                } catch (e) { /* Ignore parsing error */ }
            }
            if (errorCallback) errorCallback(errorMessage);
        }
    );
}

function registerUser(userData, successCallback, errorCallback) {
     console.log("Attempting registration with:", userData); // Debug log
    // Add default 'active' field if not present
    const registrationData = { ...userData, active: true };

    ajaxCall(
        "POST",
        `${userApiUrl}/register`, // Adjust endpoint if needed
        JSON.stringify(registrationData),
        (response) => {
             console.log("Registration successful:", response); // Debug log
            if (successCallback) successCallback(response);
        },
        (jqXHR, textStatus, errorThrown) => { // Keep original arguments
            console.error("Registration failed:", textStatus, errorThrown, jqXHR.responseText); // Debug log
            let errorMessage = "Registration failed. Please try again.";
             // Extract error message from server response
             if (jqXHR.responseJSON && jqXHR.responseJSON.message) {
                errorMessage = jqXHR.responseJSON.message;
            } else if (jqXHR.responseText) {
                 try {
                    const errorResponse = JSON.parse(jqXHR.responseText);
                    // Check for specific validation error structures if your API provides them
                    if (errorResponse.errors) {
                        // Concatenate multiple validation errors if present
                        errorMessage = Object.values(errorResponse.errors).flat().join(' ');
                    } else {
                        errorMessage = errorResponse.title || errorMessage;
                    }
                } catch (e) { /* Ignore parsing error */ }
            }
            // Call the provided error callback (which updates the form UI)
            if (errorCallback) errorCallback(errorMessage);
        }
    );
}

// --- Form Setup ---

function setupLoginForm() {
    $('#loginForm').submit(function(event) {
        event.preventDefault();
        const email = $('#email').val();
        const password = $('#password').val();
        const $errorMessage = $('#errorMessage');

        $errorMessage.hide(); // Hide previous errors

        loginUser(email, password,
            (userData) => {
                // Redirect to home page on successful login
                window.location.href = 'index.html';
            },
            (errorMsg) => {
                // Display error message
                $errorMessage.text(errorMsg).show();
            }
        );
    });
}

function setupRegistrationForm() {
    const $name = $('#name');
    const $email = $('#email');
    const $password = $('#password');
    const $nameError = $('#nameError');
    const $emailError = $('#emailError');
    const $passwordError = $('#passwordError');
    const $serverErrorMessage = $('#serverErrorMessage');

    function validateField($field, $errorField, validationFn, errorMessage) {
        const value = $field.val();
        if (!validationFn(value)) {
            $errorField.text(errorMessage).show();
            $field.addClass('invalid');
            return false;
        } else {
            $errorField.hide();
            $field.removeClass('invalid');
            return true;
        }
    }

    function validateName(name) {
        return /^[a-zA-Z]{2,}$/.test(name); // Letters only, min 2 chars
    }

    function validatePassword(password) {
        // At least 8 chars, 1 number, 1 uppercase
        return password.length >= 8 && /\d/.test(password) && /[A-Z]/.test(password);
    }
     function validateEmail(email) {
        // Basic email format check
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    $('#registerForm').submit(function(event) {
        event.preventDefault();
        $serverErrorMessage.hide(); // Hide previous server errors

        // Clear previous errors and invalid states
        $('.error-message').hide();
        $('input').removeClass('invalid');


        // Perform validation
        const isNameValid = validateField($name, $nameError, validateName, "Name must be letters only, minimum 2 characters.");
        const isEmailValid = validateField($email, $emailError, validateEmail, "Please enter a valid email address.");
        const isPasswordValid = validateField($password, $passwordError, validatePassword, "Password must be at least 8 characters, include one number and one uppercase letter.");


        if (!isNameValid || !isEmailValid || !isPasswordValid) {
            return; // Stop submission if validation fails
        }

        const userData = {
            name: $name.val(),
            email: $email.val(),
            password: $password.val()
            // Add other fields from your form if necessary
        };

        registerUser(userData,
            () => {
                alert('Registration successful! Please login.'); // Keep alert for success feedback before redirect
                window.location.href = 'login.html'; // Redirect to login page
            },
            (errorMsg) => {
                 // Display server error message within the form
                 $serverErrorMessage.text(errorMsg).show(); // This now correctly handles the message
            }
        );
    });

     // Add real-time validation feedback (optional but good UX)
    $name.on('input', () => validateField($name, $nameError, validateName, "Name must be letters only, minimum 2 characters."));
    $email.on('input', () => validateField($email, $emailError, validateEmail, "Please enter a valid email address."));
    $password.on('input', () => validateField($password, $passwordError, validatePassword, "Password must be at least 8 characters, include one number and one uppercase letter."));
}

// --- Utility for updating UI based on login state ---
function updateUserDisplay() {
    const userInfo = getUserInfo();
    const $userInfoDiv = $('#userInfo'); // Assume a div with id="userInfo" exists
    const $loginLink = $('#loginLink'); // Assume a link/button with id="loginLink"
    const $logoutButton = $('#logoutButton'); // Assume a button with id="logoutButton"
    const $myCollectionLink = $('#myCollectionLink'); // Assume the 'My Collection' link has id="myCollectionLink"
    const $addMovieLink = $('#addMovieLink'); // Assume an 'Add Movie' link/button with id="addMovieLink"

    if (userInfo) {
        // Logged in
        if ($userInfoDiv.length) {
            $userInfoDiv.html(`Welcome, ${userInfo.name}!`).show(); // Display username
        }
        if ($loginLink.length) $loginLink.hide();
        if ($logoutButton.length) {
             $logoutButton.show().off('click').on('click', logoutUser); // Ensure click handler is attached
        }
        if ($myCollectionLink.length) $myCollectionLink.attr('href', 'MyMovies.html').off('click'); // Enable link
        if ($addMovieLink.length) $addMovieLink.show(); // Show Add Movie link/button


    } else {
        // Not logged in
        if ($userInfoDiv.length) $userInfoDiv.hide();
        if ($loginLink.length) $loginLink.show();
        if ($logoutButton.length) $logoutButton.hide();
        if ($myCollectionLink.length) {
             $myCollectionLink.attr('href', '#').on('click', (e) => { // Disable link and redirect
                e.preventDefault();
                alert("Please log in to view your collection.");
                window.location.href = 'login.html';
            });
        }
         if ($addMovieLink.length) $addMovieLink.hide(); // Hide Add Movie link/button
    }
}