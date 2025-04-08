const port = 54201;
const postUrl = `http://localhost:${port}/api/Movie`;
const getUrl = `http://localhost:${port}/api/Movie`;
const searchUrl = `http://localhost:${port}/api/Movie/search`; 

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
        Id: numberOfMovies++,
        Url: movieData.url,
        PrimaryTitle: movieData.primaryTitle,
        Description: movieData.description,
        PrimaryImage: movieData.primaryImage,
        Year: movieData.startYear,
        ReleaseDate: movieData.releaseDate,
        Language: movieData.language,
        Budget: movieData.budget,
        GrossWorldwide: movieData.grossWorldwide,
        Genres: movieData.genres.join(),
        IsAdult: movieData.isAdult,
        RuntimeMinutes: movieData.runtimeMinutes,
        AverageRating: movieData.averageRating,
        NumVotes: movieData.numVotes,
    };
}

function renderMovie(filteredMovieData, btnType) {
    
    if (!filteredMovieData) {
        $("#loadedMovies").append($(`<div class="Movie error">Unable to load movie</div>`));
        return;
    }
    
    const { 
        Id, 
        PrimaryTitle, 
        Description = "No description available", // Default value for missing description, 
        PrimaryImage = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' enable-background='new 0 0 24 24' height='24' viewBox='0 0 24 24' width='24'%3E%3Cg%3E%3Crect fill='none' height='24' width='24'/%3E%3Cpath d='M21.9,21.9l-8.49-8.49l0,0L3.59,3.59l0,0L2.1,2.1L0.69,3.51L3,5.83V19c0,1.1,0.9,2,2,2h13.17l2.31,2.31L21.9,21.9z M5,18 l3.5-4.5l2.5,3.01L12.17,15l3,3H5z M21,18.17L5.83,3H19c1.1,0,2,0.9,2,2V18.17z'/%3E%3C/g%3E%3C/svg%3E", 
        Year = "Unknown", // Default value for missing year, 
        RuntimeMinutes = "N/A", // Default value for missing runtime
        AverageRating = "No rating yet" // Default value for missing rating
    } = filteredMovieData;
    
    const movieDiv = $(`<div class="Movie" id="${Id}"></div>`);
    $("#loadedMovies").append(movieDiv);

    const upperDiv = $(`<div class="upperDiv"></div>`);
    const lowerDiv = $(`<div class="lowerDiv"></div>`);
    movieDiv.append(upperDiv, lowerDiv);

    if(btnType === "addToCart"){
        
        const addToCartBTN = $(`<button class="addToCartBTN">Add to Cart</button>`);
        addToCartBTN.click(() => {
            console.log(`Button has been pressed, parent is: ${Id}`);
            sendToServer(filteredMovieData, movieDiv);
        });

        upperDiv.append(addToCartBTN);
    }

    if(btnType === "deleteFromList")
    {
        const deleteFromList = $(`<button class="deleteFromList">Remove</button>`);
        deleteFromList.click(() => {
            console.log(`Button has been pressed, parent is: ${Id}`);
            // Confirm before deleting
            if (confirm(`Are you sure you want to remove "${PrimaryTitle}" from your list?`)) {
                deleteFromServer(Id, movieDiv);
            }        
        });

        upperDiv.append(deleteFromList);
    }
    const ratingDiv = $(`<div class="rating">${AverageRating}</div>`);
    const altImgText = `${PrimaryTitle} movie poster`;
    const imgDiv = $(`<img src="${PrimaryImage}" alt="${altImgText}">`);

    upperDiv.append(ratingDiv, imgDiv);

    const titleDiv = $(`<div>${PrimaryTitle}</div>`);
    const yearDiv = $(`<div>${Year}</div>`);
    const runtimeDiv = $(`<div>${RuntimeMinutes}</div>`);
    const descriptionDiv = $(`<div>${Description}</div>`);

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
        () => {
            if (!movieDiv.hasClass("success")) { // Only process if not already been added
                insertSCB(movieDiv);
            }
        },
        insertECB); 
}

// ------------------------------------------------------------------------------------------------------------------
// Callback functions


// Callback function executed after a successful POST request to the server.
// It updates the movie's UI to indicate it has been added to the cart.
function insertSCB(movieDiv)
{
    movieDiv.addClass("success");
    
    movieDiv.find(".addToCartBTN")
            .text("Added")
            .prop("disabled", true); // Update button text and disable it
}

function insertECB(err)
{
    console.error(`Error: ${err}`);
}

function ajaxCall(method, api, data, successCB, errorCB) {
    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: errorCB
    });
}

//---------- Page 2 Methods ----------

function loadMyList()
{

    //Set up action when searhc bar is active
    //$("#searchForm").attr("action", `${getUrl}/search`);
    console.log("Hello");
    setupSearchFunctionality();

    //Need to add logic to use Get api that return list of all movies and load it into divs using helper functions

    // Get all movies from server and render them
    getAllMoviesListFromServer();
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
    // Clear existing movies first to avoid duplicates
    $("#loadedMovies").empty();
    
    console.log("Rendering movies from server:", moviesFromServer);
    
    if (!moviesFromServer || moviesFromServer.length === 0) {
        $("#loadedMovies").append("<p>No movies in your collection yet.</p>");
        return;
    }
    
    // Render each movie with the delete button (fixed the typo "dedeleteFromListke")
    moviesFromServer.forEach(movie => {

        const normalizedMovie = {
            Id: movie.id,
            PrimaryTitle: movie.primaryTitle || movie.PrimaryTitle,
            Description: movie.description || movie.Description,
            PrimaryImage: movie.primaryImage || movie.PrimaryImage,
            Year: movie.startYear || movie.Year,
            RuntimeMinutes: movie.runtimeMinutes || movie.RuntimeMinutes,
            AverageRating: movie.averageRating || movie.AverageRating,
            Language: movie.language || movie.Language,
            Genres: movie.genres ? (typeof movie.genres === 'string' ? movie.genres : movie.genres.join(',')) : (movie.Genres || ''),
            IsAdult: movie.isAdult !== undefined ? movie.isAdult : (movie.IsAdult !== undefined ? movie.IsAdult : false),
            NumVotes: movie.numVotes || movie.NumVotes
        };

        renderMovie(normalizedMovie, "deleteFromList");
    });
}

// Utility function to delete a movie from the server
function deleteFromServer(movieId, movieDiv) {
    ajaxCall(
        "DELETE", 
        `${getUrl}/${movieId}`, 
        "", 
        () => {
            // On success, remove the movie from the UI
            movieDiv.fadeOut(300, function() {
                $(this).remove();
                
                // Check if there are any movies left
                if ($("#loadedMovies .Movie").length === 0) {
                    $("#loadedMovies").append("<p>No movies in your collection yet.</p>");
                }
            });
        },
        (err) => {
            console.error(`Error deleting movie: ${err}`);
            alert("Failed to delete movie. Please try again.");
        }
    );
}

function searchMoviesByTitle(movieTitle)
{
    ajaxCall(
        "GET",
        `${searchUrl}?title=${movieTitle}`,
        "",
        (searchResult) => {
            console.log("Search Result: ", searchResult);
            renderMyList(searchResult);
        },

        (err) => {
            console.error(`Error searching movies: ${err}`);
            alert("Error occurred while searching. Please try again.");
        }
    );
}

function setupSearchFunctionality()
{
    $(`#titleSearchBTN`).click(() => {

        const movieTitle = $("input[name='movieSearch']").val();
        console.log(`Searching for movies with title: ${movieTitle}`);

        searchMoviesByTitle(movieTitle);
    })
}

function newfunc()
{
    
}
