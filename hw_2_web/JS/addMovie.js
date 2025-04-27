// hw_2_web/JS/addMovie.js

function setupAddMovieForm() {
    const $form = $('#addMovieForm');
    const $statusMessage = $('#formStatusMessage');

    // --- Input Field References --- // Define field objects with input, error div, required status, validation function, and error message
    const fields = {
        primaryTitle: { input: $('#primaryTitle'), error: $('#primaryTitleError'), required: true, validate: (val) => val.trim().length > 0, msg: "Title is required." },
        primaryImage: { input: $('#primaryImage'), error: $('#primaryImageError'), required: true, validate: isValidUrl, msg: "Valid Image URL is required." },
        year: { input: $('#year'), error: $('#yearError'), required: true, validate: (val) => /^\d{4}$/.test(val) && parseInt(val) >= 1888 && parseInt(val) <= new Date().getFullYear() + 5, msg: "Valid 4-digit year is required." }, // Allow a bit into future
        releaseDate: { input: $('#releaseDate'), error: $('#releaseDateError'), required: true, validate: (val) => !isNaN(Date.parse(val)), msg: "Release date is required." },
        language: { input: $('#language'), error: $('#languageError'), required: true, validate: (val) => val.trim().length > 0, msg: "Language is required." },
        runtimeMinutes: { input: $('#runtimeMinutes'), error: $('#runtimeMinutesError'), required: true, validate: (val) => /^\d+$/.test(val) && parseInt(val) > 0, msg: "Valid positive runtime is required." },
        url: { input: $('#url'), error: $('#urlError'), required: false, validate: (val) => val.trim().length === 0 || isValidUrl(val), msg: "Please enter a valid URL or leave blank." },
        description: { input: $('#description'), error: $('#descriptionError'), required: false }, // No specific validation needed beyond textarea
        budget: { input: $('#budget'), error: $('#budgetError'), required: false, validate: (val) => val === '' || (parseFloat(val) >= 100000), msg: "Budget must be at least 100,000 or blank." },
        grossWorldwide: { input: $('#grossWorldwide'), error: $('#grossWorldwideError'), required: false, validate: (val) => val === '' || parseFloat(val) >= 0, msg: "Gross must be non-negative or blank." },
        genres: { input: $('#genres'), error: $('#genresError'), required: false, validate: (val) => val === '' || /^[a-zA-Z\s,-]+$/.test(val), msg: "Genres should be comma-separated words." }, // Basic check
        averageRating: { input: $('#averageRating'), error: $('#averageRatingError'), required: false, validate: (val) => val === '' || (parseFloat(val) >= 0 && parseFloat(val) <= 10), msg: "Rating must be between 0 and 10 or blank." },
        numVotes: { input: $('#numVotes'), error: $('#numVotesError'), required: false, validate: (val) => val === '' || (parseInt(val) >= 0 && Number.isInteger(Number(val))), msg: "Votes must be a non-negative integer or blank." },
        isAdult: { input: $('#isAdult'), error: null, required: false } // Checkbox
    };

    // Helper function to validate URL format
    function isValidUrl(string) {
        try {
            new URL(string);
            return true;
        } catch (_) {
            return false;
        }
    }

    // Function to validate the entire form
    function validateForm() {
        let isValid = true;
        $statusMessage.hide().text(''); // Clear previous status
        $('.error-message').hide(); // Hide all error messages
        $('input, textarea').removeClass('invalid'); // Clear invalid styles

        // Iterate through each field defined in the fields object
        for (const key in fields) {
            const field = fields[key];
            // Get value, handling checkbox differently
            const value = field.input.is(':checkbox') ? field.input.prop('checked') : field.input.val();

            // Check if required field is empty
            if (field.required && (value === '' || value === null || value === undefined)) {
                if (field.error) field.error.text(`${field.input.prev('label').text().replace(' *:', '')} is required.`).show();
                field.input.addClass('invalid');
                isValid = false;
            } 
            // Check if field has a validation function and if the value fails it
            else if (field.validate && !field.validate(value)) {
                 if (field.error) field.error.text(field.msg).show();
                 field.input.addClass('invalid');
                 isValid = false;
            }
        }
        return isValid;
    }

    // Handle form submission
    $form.submit(function(event) {
        event.preventDefault(); // Prevent default browser submission

        // Validate the form before proceeding
        if (!validateForm()) {
            $statusMessage.text("Please fix the errors above.").css('color', 'red').show();
            return;
        }

        // Collect data from form fields if validation passes
        const movieData = {};
        for (const key in fields) {
             const field = fields[key];
             let value = field.input.is(':checkbox') ? field.input.prop('checked') : field.input.val();

             // Convert numeric types from string to number
             if (field.input.attr('type') === 'number' && value !== '') {
                 value = Number(value); // Convert to number
                 // Additional check for NaN after conversion
                 if (isNaN(value)) { 
                     if (field.error) field.error.text("Invalid number format.").show();
                     field.input.addClass('invalid');
                     $statusMessage.text("Invalid number format detected.").css('color', 'red').show();
                     return; // Stop submission if conversion results in NaN
                 }
             }
              // Map JS field names (camelCase) to C# property names (PascalCase) for the server payload
             const serverKey = key.charAt(0).toUpperCase() + key.slice(1);
             movieData[serverKey] = value;
        }

         // Set default values for optional fields if they are empty or not provided
         movieData.GrossWorldwide = movieData.GrossWorldwide === '' ? 0 : movieData.GrossWorldwide;
         movieData.AverageRating = movieData.AverageRating === '' ? 0 : movieData.AverageRating;
         movieData.NumVotes = movieData.NumVotes === '' ? 0 : movieData.NumVotes;
         movieData.IsAdult = movieData.IsAdult === '' ? false : movieData.IsAdult; // Checkbox value is boolean
         // Allow null for budget if empty and not required, otherwise use the number or default
         movieData.Budget = movieData.Budget === '' ? null : movieData.Budget; 


        console.log("Submitting movie data:", movieData);
        $statusMessage.text("Submitting...").css('color', 'black').show();


        // Use the existing ajaxCall function (ensure it's globally accessible from clientMethods.js)
        ajaxCall(
            "POST",
            postUrl, // Use the global postUrl defined in clientMethods.js
            JSON.stringify(movieData),
            (response) => {
                // Success callback: Show success message, reset form
                console.log("Movie added successfully:", response);
                $statusMessage.text("Movie added successfully!").css('color', 'green').show();
                alert("Movie added successfully!");
                $form[0].reset(); // Reset the form fields
                 $('.error-message').hide(); // Hide any previous error messages
                 $('input, textarea').removeClass('invalid'); // Remove invalid styling
            },
            (errorMsg, jqXHR) => {
                 // Error callback: Show error message
                 console.error("Failed to add movie:", errorMsg, jqXHR);
                 let displayError = "Failed to add movie. ";
                 if (typeof errorMsg === 'string') {
                     displayError += errorMsg;
                 } else if (jqXHR && jqXHR.status) {
                     displayError += `(Error ${jqXHR.status})`;
                 }
                $statusMessage.text(displayError).css('color', 'red').show();
                alert(displayError); // Also show alert for immediate feedback
            }
        );
    });

     // Optional: Add real-time validation feedback on input/change events for better UX
     for (const key in fields) {
         const field = fields[key];
         // Attach event listener if a validation function exists for the field
         if (field.validate) {
             field.input.on('input change', function() {
                 const value = $(this).is(':checkbox') ? $(this).prop('checked') : $(this).val();
                 // Validate the current field's value
                 if (!field.validate(value)) {
                     // Show error message and mark as invalid if validation fails
                     if (field.error) field.error.text(field.msg).show();
                     $(this).addClass('invalid');
                 } else {
                      // Hide error message and remove invalid mark if validation passes
                      if (field.error) field.error.hide();
                      $(this).removeClass('invalid');
                 }
                 // Also re-check required status if field becomes empty after input
                 if (field.required && value === '') {
                      if (field.error) field.error.text(`${field.input.prev('label').text().replace(' *:', '')} is required.`).show();
                      $(this).addClass('invalid');
                 }
             });
         }
     }
}