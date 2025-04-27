// const apiUrlBase = "https://localhost:54200/api"; // Defined globally in HTML now
const postUrl = `${apiUrlBase}/Movie`;
const getUrl = `${apiUrlBase}/Movie`;
const searchUrl = `${apiUrlBase}/Movie/search`; 

let isLoaded = false;
let numberOfMovies = 0;

// Load movies functionality
function init()
{
    if(!isLoaded){

        $("#loadMoviesBTN").click(function(){
            if(isLoaded){
                alert("Movies has already been loaded");
                return;
            }
            loadMovies(); //Load movies to the site and update isLoaded indicator to be true;
        }) 
    }


}


function loadMovies() {

    //  'data' variable holds the whole content of Movies-db as a 
    $.getScript("./JS/Movies-db.js", function(data, textStatus) {
        
        if(textStatus === "success" && typeof movies !== "undefined") {
            
            movies.forEach(movie =>{

                const newMovie = createServerMovie(movie);
                renderMovie(newMovie,"addToCart");
            })
            
            isLoaded = true;
            
            console.log(`Site has finished loading movies: ${isLoaded}`);
               
        } else {
            console.error("Movies data failed to load properly.");
        }
    });
}

// ------------------------------------------------------------------------------------------------------------------
// Helper function in create server movie
function extractNumbersFromString(str)
{
    let onlyNumbers = "";
    for(let char of str)
    {
        if (char >= '0' && char <= '9')
        {
            onlyNumbers += char;
        }
    }
    return onlyNumbers;
}

function createServerMovie(movieData) {
    const requiredFields = [
        "id",
        "primaryTitle",
    ];
    
    for (let field of requiredFields) {
        if (movieData[field] === undefined || movieData[field] === null) {
            console.log(movieData);
            console.error(`Missing required field: ${field}`);
            return; // Stop early if any field is missing
        }
    }
    
    const movieIdOnlyNumbers = extractNumbersFromString(movieData.id);
    console.log(movieIdOnlyNumbers);
    return {
        // Use a temporary client-side ID for rendering before server interaction
        id: numberOfMovies++, 
        url: movieData.url,
        primaryTitle: movieData.primaryTitle,
        description: movieData.description,
        primaryImage: movieData.primaryImage,
        year: movieData.startYear,
        releaseDate: movieData.releaseDate,
        language: movieData.language,
        budget: movieData.budget,
        grossWorldwide: movieData.grossWorldwide,
        genres: movieData.genres.join(),
        isAdult: movieData.isAdult,
        runtimeMinutes: movieData.runtimeMinutes,
        averageRating: movieData.averageRating,
        numVotes: movieData.numVotes,
    };
}

function renderMovie(filteredMovieData, btnType) {
    
    if (!filteredMovieData) {
        $("#loadedMovies").append($(`<div class="Movie error">Unable to load movie</div>`));
        return;
    }
    
    const normalizedMovie = {
        id: filteredMovieData.id,
        url: filteredMovieData.url || filteredMovieData.Url,
        primaryTitle: filteredMovieData.primaryTitle || filteredMovieData.PrimaryTitle,
        description: filteredMovieData.description || filteredMovieData.Description || "No description available",
        primaryImage: filteredMovieData.primaryImage || filteredMovieData.PrimaryImage || "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' enable-background='new 0 0 24 24' height='24' viewBox='0 0 24 24' width='24'%3E%3Cg%3E%3Crect fill='none' height='24' width='24'/%3E%3Cpath d='M21.9,21.9l-8.49-8.49l0,0L3.59,3.59l0,0L2.1,2.1L0.69,3.51L3,5.83V19c0,1.1,0.9,2,2,2h13.17l2.31,2.31L21.9,21.9z M5,18 l3.5-4.5l2.5,3.01L12.17,15l3,3H5z M21,18.17L5.83,3H19c1.1,0,2,0.9,2,2V18.17z'/%3E%3C/g%3E%3C/svg%3E",
        year: filteredMovieData.year || filteredMovieData.Year || "Unknown",
        releaseDate: filteredMovieData.releaseDate || filteredMovieData.ReleaseDate,
        language: filteredMovieData.language || filteredMovieData.Language,
        budget: filteredMovieData.budget || filteredMovieData.Budget,
        grossWorldwide: filteredMovieData.grossWorldwide || filteredMovieData.GrossWorldwide,
        genres: filteredMovieData.genres ? (typeof filteredMovieData.genres === 'string' ? filteredMovieData.genres : filteredMovieData.genres.join(',')) : (filteredMovieData.Genres || ''),
        isAdult: filteredMovieData.isAdult !== undefined ? filteredMovieData.isAdult : (filteredMovieData.IsAdult !== undefined ? filteredMovieData.IsAdult : false),
        runtimeMinutes: filteredMovieData.runtimeMinutes || filteredMovieData.RuntimeMinutes || "N/A",
        averageRating: filteredMovieData.averageRating || filteredMovieData.AverageRating || "No rating",
        numVotes: filteredMovieData.numVotes || filteredMovieData.NumVotes
    };
    
    const movieDiv = $(`<div class="Movie" id="${normalizedMovie.id}"></div>`);
    $("#loadedMovies").append(movieDiv);

    const upperDiv = $(`<div class="upperDiv"></div>`);
    const lowerDiv = $(`<div class="lowerDiv"></div>`);
    movieDiv.append(upperDiv, lowerDiv);

    if(btnType === "addToCart"){
        
        const addToCartBTN = $(`<button class="addToCartBTN">Add to Cart</button>`);
        addToCartBTN.click(() => {
            if (!isLoggedIn()) {
                alert("Please log in to add movies to your collection.");
                window.location.href = 'login.html';
            } else {
                console.log(`Add to Cart pressed, movie data:`, normalizedMovie);
                const movieToServer = {
                    Url: normalizedMovie.url,
                    PrimaryTitle: normalizedMovie.primaryTitle,
                    Description: normalizedMovie.description,
                    PrimaryImage: normalizedMovie.primaryImage,
                    Year: normalizedMovie.year,
                    ReleaseDate: normalizedMovie.releaseDate,
                    Language: normalizedMovie.language,
                    Budget: normalizedMovie.budget,
                    GrossWorldwide: normalizedMovie.grossWorldwide,
                    Genres: normalizedMovie.genres,
                    IsAdult: normalizedMovie.isAdult,
                    RuntimeMinutes: normalizedMovie.runtimeMinutes,
                    AverageRating: normalizedMovie.averageRating,
                    NumVotes: normalizedMovie.numVotes
                };
                sendToServer(movieToServer, movieDiv);
            }
        });

        upperDiv.append(addToCartBTN);
    }

    if(btnType === "deleteFromList")
    {
        const deleteFromList = $(`<button class="deleteFromList">Remove</button>`);
        deleteFromList.click(() => {
            console.log(`Button has been pressed, parent is: ${normalizedMovie.id}`);
            if (confirm(`Are you sure you want to remove "${normalizedMovie.primaryTitle}" from your list?`)) {
                deleteFromServer(normalizedMovie.id, movieDiv);
            }        
        });

        upperDiv.append(deleteFromList);
    }
    const ratingDiv = $(`<div class="rating">${normalizedMovie.averageRating}</div>`);
    const altImgText = `${normalizedMovie.primaryTitle} movie poster`;
    const imgDiv = $(`<img src="${normalizedMovie.primaryImage}" alt="${altImgText}">`);
    imgDiv.on('error', function() {
        $(this).attr('src', "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' enable-background='new 0 0 24 24' height='24' viewBox='0 0 24 24' width='24'%3E%3Cg%3E%3Crect fill='none' height='24' width='24'/%3E%3Cpath d='M21.9,21.9l-8.49-8.49l0,0L3.59,3.59l0,0L2.1,2.1L0.69,3.51L3,5.83V19c0,1.1,0.9,2,2,2h13.17l2.31,2.31L21.9,21.9z M5,18 l3.5-4.5l2.5,3.01L12.17,15l3,3H5z M21,18.17L5.83,3H19c1.1,0,2,0.9,2,2V18.17z'/%3E%3C/g%3E%3C/svg%3E");
    });

    upperDiv.append(ratingDiv, imgDiv);

    const titleDiv = $(`<div>${normalizedMovie.primaryTitle}</div>`);
    const yearDiv = $(`<div>${normalizedMovie.year}</div>`);
    const runtimeDiv = $(`<div>${normalizedMovie.runtimeMinutes}</div>`);
    const descriptionDiv = $(`<div>${normalizedMovie.description}</div>`);

    lowerDiv.append(titleDiv, yearDiv, runtimeDiv, descriptionDiv);
}

// ------------------------------------------------------------------------------------------------------------------
// Utility Function
function sendToServer(movieToServer, movieDiv)
{
    ajaxCall(
        "POST",
        postUrl,
        JSON.stringify(movieToServer),
        (returnedMovie) => { // Success Callback - receives the movie object from the server
             // Update UI to show success, passing the movieDiv AND the returned movie data
             insertSCB(movieDiv, returnedMovie); // Call the original success callback with returned data
        },
        insertECB // Use the updated error callback directly
    );
}

// ------------------------------------------------------------------------------------------------------------------
// Callback functions


// Callback function executed after a successful POST request to the server.
// It updates the movie's UI to indicate it has been added to the cart and updates its ID.
function insertSCB(movieDiv, returnedMovie) // Accept the returned movie data
{
    // Update the movieDiv's ID attribute with the ID assigned by the server
    if (returnedMovie && returnedMovie.id) {
        movieDiv.attr('id', returnedMovie.id);
        console.log(`Updated movie div ID to server-assigned ID: ${returnedMovie.id}`); // Debugging line
    } else {
        console.warn("Server did not return a movie object with an ID on successful POST."); // Warning if something unexpected happens
    }

    movieDiv.addClass("success");

    movieDiv.find(".addToCartBTN")
            .text("Added")
            .prop("disabled", true);
}

// Callback function executed after a failed POST request to the server.
function insertECB(jqXHR, textStatus, errorThrown) // Update parameters
{
    // Log more details from the error
    console.error(`Error adding movie: ${textStatus}`, {
        status: jqXHR.status,
        statusText: jqXHR.statusText,
        response: jqXHR.responseText,
        errorThrown: errorThrown
    });
    // Optionally, display a user-friendly message, perhaps using the status code
    let userMessage = "Failed to add movie to your collection.";
    if (jqXHR.status === 401) {
        userMessage = "Authentication failed. Please log in again.";
        // Consider calling logoutUser() here if appropriate
    } else if (jqXHR.status === 409) {
        userMessage = "This movie might already be in your collection.";
    } else if (jqXHR.status >= 500) {
        userMessage = "Server error. Please try again later.";
    }
    // Display the message (e.g., in a dedicated status area or an alert, though avoid alerts if possible)
    console.error(userMessage);
}

function ajaxCall(method, api, data, successCB, errorCB) {
    const token = getTokenFromStorage(); // Get token using the function from auth.js
    const headers = {};

    // Add Authorization header if token exists AND it's not a login/register request
    if (token && !api.endsWith('/login') && !api.endsWith('/register')) {
        headers['Authorization'] = `Bearer ${token}`;
        console.log("Adding Authorization header:", headers['Authorization']); // DEBUGGING LINE
    } else {
        console.log("NOT adding Authorization header. Token:", token, "API:", api); // DEBUGGING LINE
    }


    $.ajax({
        type: method,
        url: api,
        headers: headers, // Add the headers object
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: function(jqXHR, textStatus, errorThrown) {
             console.error(`AJAX Error (${method} ${api}): ${textStatus}`, errorThrown, jqXHR.responseText, "Headers Sent:", jqXHR.getAllResponseHeaders()); // DEBUGGING LINE - Note: This shows RESPONSE headers, not request headers. Need browser dev tools for request headers.

             // Check for unauthorized error ONLY if no specific error callback is provided
             // OR if the error is not on login/register (already handled by specific callback)
            if (jqXHR.status === 401 && !errorCB && !api.endsWith('/login') && !api.endsWith('/register')) {
                 alert("Your session may have expired or is invalid. Please log in again.");
                 logoutUser(); // Use the logout function from auth.js
                 return;
            }

            // General error handling
            if (errorCB) {
                 let errorMsg = `Error: ${textStatus}.`;
                 if (jqXHR.responseJSON && jqXHR.responseJSON.message) {
                     errorMsg = jqXHR.responseJSON.message;
                 } else if (jqXHR.responseText) {
                     try {
                         const errData = JSON.parse(jqXHR.responseText);
                         errorMsg = errData.title || errData.detail || errorMsg;
                     } catch(e) { /* ignore parsing error */ }
                 }
                 // Pass original jqXHR object to allow specific callbacks to check status code etc.
                 errorCB(jqXHR, textStatus, errorThrown);
            } else {
                 alert(`An error occurred: ${textStatus}`);
            }
        }
    });
}

//---------------------------------------------- Page 2 Methods ----------------------------------------------------

//Being executed in MyCollection Page
function loadMyList()
{
    console.log("Setting up search functionality...");
    
    setupSearchFunctionality();
    setupDateSearchFunctionality();

    getAllMoviesListFromServer();
}

function setupSearchFunctionality()
{
    $(`#searchByTitleForm`).submit((event) => {
        event.preventDefault();

        const movieTitle = $("input[name='movieSearch']").val();
        console.log(`Searching for movies with title: ${movieTitle}`);

        searchMoviesByTitle(movieTitle);
    })
}

function searchMoviesByTitle(movieTitle) {
    const normalizedTitle = movieTitle.toLowerCase();
    const titleSearchUrl = `${searchUrl}?title=${encodeURIComponent(normalizedTitle)}`;
    ajaxCall(
        "GET",
        titleSearchUrl,
        "",
        (searchResult) => {
            console.log("Search Result: ", searchResult);
            $("#loadedMovies").empty();
                        
            if (!searchResult || searchResult.length === 0) {
                $("#loadedMovies").append("<p>No matching movies have been found.</p>");
                return;
            }
            renderMyList(searchResult);
        },
        (err) => {
            console.error(`Error searching movies: ${err}`);
            alert("Error occurred while searching. Please try again.");
        }
    );
}

function setupDateSearchFunctionality() {
    $('#searchByDateForm').submit((event) => {
        event.preventDefault();

        const startDate = $('#startDate').val();
        const endDate = $('#endDate').val();

        if (!startDate || !endDate) {
            alert("Please select both a start and end date.");
            return; 
        }
        if (startDate > endDate) {
            alert("Start date cannot be after end date.");
            return;
        }

        console.log(`Searching for movies between ${startDate} and ${endDate}`);

        searchMoviesByDate(startDate, endDate); 
    });
}

function searchMoviesByDate(startDate, endDate){
    const dateSearchUrl = `${getUrl}/from/${encodeURIComponent(startDate)}/until/${encodeURIComponent(endDate)}`;

    console.log(`Making AJAX call to: ${dateSearchUrl}`);

    ajaxCall(
        "GET",
        dateSearchUrl, 
        "",
        (searchResult) => {
            console.log("Date Search Result: ", searchResult);
            $("#loadedMovies").empty();
                        
            if (!searchResult || searchResult.length === 0) {
                $("#loadedMovies").append("<p>No matching movies have been found.</p>");
                return;
            }
            renderMyList(searchResult); 
        },
        (err) => {
            console.error(`Error searching movies by date:`, err); 
            alert("Error occurred while searching by date. Please check the console and try again.");
        }
    );
}

function getAllMoviesListFromServer()
{
    ajaxCall(
        "GET", 
        getUrl,
        "", 
        (moviesFromServer) => {
            console.log("Successfully retrieved movies from server:", moviesFromServer);
            renderMyList(moviesFromServer);
        }, 
        insertECB);
}

function renderMyList(moviesFromServer) {
    $("#loadedMovies").empty();
    console.log("Rendering my list:", moviesFromServer);
    
    if (!moviesFromServer || moviesFromServer.length === 0) {
        $("#loadedMovies").append("<p>No movies in your collection yet.</p>");
        return;
    }
    
    moviesFromServer.forEach(movie => {
        const normalizedMovie = {
            id: movie.id,
            primaryTitle: movie.primaryTitle || movie.PrimaryTitle,
            description: movie.description || movie.Description,
            primaryImage: movie.primaryImage || movie.PrimaryImage,
            year: movie.year,
            runtimeMinutes: movie.runtimeMinutes || movie.RuntimeMinutes,
            averageRating: movie.averageRating || movie.AverageRating,
            language: movie.language || movie.Language,
            genres: movie.genres ? (typeof movie.genres === 'string' ? movie.genres : movie.genres.join(',')) : (movie.Genres || ''),
            isAdult: movie.isAdult !== undefined ? movie.isAdult : (movie.IsAdult !== undefined ? movie.IsAdult : false),
            numVotes: movie.numVotes || movie.NumVotes
        };

        renderMovie(normalizedMovie, "deleteFromList");
    });
}
//-------------------- END OF SEARCH METHODS ------------------------------

// Utility function to delete a movie from the server
function deleteFromServer(movieId, movieDiv) {
    // Ensure movieId is treated as a number if it comes from the div's ID attribute
    const idToDelete = parseInt(movieId, 10);
    if (isNaN(idToDelete)) {
        console.error("Invalid movie ID for deletion:", movieId);
        alert("Could not delete movie: Invalid ID.");
        return;
    }

    ajaxCall(
        "DELETE",
        `${getUrl}/${idToDelete}`, // Use the potentially updated ID
        "",
        () => {
            movieDiv.fadeOut(300, function() {
                $(this).remove();

                if ($("#loadedMovies .Movie").length === 0) {
                    $("#loadedMovies").append("<p>Your movie collection is now empty.</p>");
                }
            });
        },
        (jqXHR, textStatus, errorThrown) => { // Adjusted error callback parameters
            console.error(`Error deleting movie ${idToDelete}:`, textStatus, errorThrown, jqXHR);
            alert(`Failed to delete the movie. Please try again. Status: ${textStatus}`);
        }
    );
}