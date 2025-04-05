const port = 5093;
const postUrl = `http://localhost:${port}/api/Movie`;
$(document).ready(init);

function init()
{
    let isLoaded = false;

    $("#loadMoviesBTN").click(function(){
        if(isLoaded){
            alert("Movies has been loaded");
            return;
        }
        loadMovies();
        isLoaded = true;
    })
}


function loadMovies() {

    //  data = The string returned from the JSON File file
    $.getScript("./JS/Movies-db.js", function(data, textStatus) {
        
        if(textStatus === "success" && typeof movies !== "undefined") {
            
            console.log("Movies data loaded successfully.", movies);
            
            movies.forEach(movie =>{

                const newMovie = createServerMovie(movie);
                renderMovie(newMovie);
            })
            
            isLoaded = true;
        
        } else {
            console.error("Movies data failed to load properly.");
        }
    });
}

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
        Id: movieIdOnlyNumbers,
        Url: movieData.url,
        PrimaryTitle: movieData.primaryTitle,
        Description: movieData.description,
        PrimaryImage: movieData.primaryImage,
        Year: movieData.startYear,
        ReleaseDate: movieData.releaseDate,
        Language: movieData.language,
        Budget: movieData.budget,
        GrossWorldwide: movieData.grossWorldwide,
        Genres: movieData.genres[0],
        IsAdult: movieData.isAdult,
        RuntimeMinutes: movieData.runtimeMinutes,
        AverageRating: movieData.averageRating,
        NumVotes: movieData.numVotes,
    };
}

function renderMovie(filteredMovieData) {
    
    if (!filteredMovieData || !filteredMovieData.Id || !filteredMovieData.PrimaryTitle) {
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

    const addToCartBTN = $(`<button class="addToCartBTN">Add to Cart</button>`);
    addToCartBTN.click(() => {
        console.log(`Button has been pressed, parent is: ${Id}`);
        sendToServer(filteredMovieData);
    });
    
    const ratingDiv = $(`<div class="rating">${AverageRating}</div>`);
    const altImgText = `${PrimaryTitle} movie poster`;
    const imgDiv = $(`<img src="${PrimaryImage}" alt="${altImgText}">`);

    upperDiv.append(addToCartBTN, ratingDiv, imgDiv);

    const titleDiv = $(`<div>${PrimaryTitle}</div>`);
    const yearDiv = $(`<div>${Year}</div>`);
    const runtimeDiv = $(`<div>${RuntimeMinutes}</div>`);
    const descriptionDiv = $(`<div>${Description}</div>`);

    lowerDiv.append(titleDiv, yearDiv, runtimeDiv, descriptionDiv);

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

function insertSCB(res)
{
    console.log(`Inserted: ${res}`);
}

function insertECB(err)
{
    console.error(`Error: ${err}`);
}

function sendToServer(movieToServer)
{
    ajaxCall("POST", postUrl, JSON.stringify(movieToServer), insertSCB, insertECB);
}
