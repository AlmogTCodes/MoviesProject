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
        
    $.getScript("./JS/Movies-db.js", function(data, textStatus, jqxhr) {
        
        if(textStatus === "success" && typeof movies !== "undefined") {
            
            console.log("Movies data loaded successfully.", movies);
            
            movies.forEach(movie =>{

                let id = movie.id; // should be the id of the WHOLE div to find it easily
                
                let url = movie.url;    //Clicking on Rating will forward to this
                let primaryTitle = movie.primaryTitle;  //Movie Title h3 div?
                let description = movie.description;    //Movie Description maybe add hide/show css class
                let primaryImage = movie.primaryImage;  //Movie Poster image
                let year = movie.startYear;     //Release Year of the movie
                let releaseDate = new Date(movie.releaseDate);  //Exact date been released with yyyy-mm-dd format
                let language = movie.language;
                let budget = movie.budget;
                let grossWorldwide = movie.grossWorldwide;
                let genres = movie.genres[0];//ITS AN ARRAY SO CHOOSING THE FIRST ONE cause genres in C# IS A STRING!
                let isAdult = movie.isAdult;
                let runtimeMinutes = movie.runtimeMinutes;
                let averageRating = movie.averageRating;
                let numVotes = movie.numVotes;

                let movieDiv = $(`<div class= "Movie" id=${id}></div>`);
                $("#loadedMovies").append(movieDiv);

                let upperDiv = $(`<div class="upperDiv"></<div>`);
                let lowerDiv = $(`<div class="lowerDiv"></<div>`);
                movieDiv.append(upperDiv, lowerDiv);

                    //upper should include [
                    // BTN,
                let addToCartBTN = $(`<button class="addToCartBTN"> Add to Cart</button>`);
                    // Rating,
                let ratingDiv = $(`<div class="rating">${averageRating}</div>`);
                    // Background image
                let altImgText = primaryTitle + " " + "movie poster";
                let imgDiv = $(`<img src="${primaryImage}" alt="${altImgText}"></img>`);

                upperDiv.append(addToCartBTN, ratingDiv, imgDiv);

                    //Lower should include [
                    // Title, 
                let titleDiv = $(`<div>"${primaryTitle}"</div>`);
                    // year, 
                let yearDiv = $(`<div>"${year}"</div>`);
                    // runtime, 
                let runtimeDiv = $(`<div>"${runtimeMinutes}"</div>`);
                    // description]
                let descriptionDiv = $(`<div>"${description}"</div>`);

                lowerDiv.append(titleDiv, yearDiv, runtimeDiv, descriptionDiv);
            })
            
            isLoaded = true;
        
        } else {
            console.error("Movies data failed to load properly.");
        }
    });
}

// function create(movies){
//     movies.forEach(movie =>{
//         let movieDiv = $('<div class= "Movie"></div>');
//         $("loadedMovies").append(movieDiv);
//         movieDiv.$(<div> Hello</div>);
//     })
// }