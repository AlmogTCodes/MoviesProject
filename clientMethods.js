// ...existing code...

function renderMyList(movies) {
    // Remove the JSON.parse call since 'movies' is already a JavaScript object
    // The data is already parsed by jQuery's ajax function
    let str = "";
    for (var i = 0; i < movies.length; i++) {
        str += `<div class="col">
                  <div class="card shadow-sm">
                    <img src="${movies[i].image}" class="card-img-top" alt="${movies[i].name}">
                    <div class="card-body">
                      <h5 class="card-title">${movies[i].name}</h5>
                      <p class="card-text">${movies[i].synopsis}</p>
                      <div class="d-flex justify-content-between align-items-center">
                        <div class="btn-group">
                          <button type="button" class="btn btn-sm btn-outline-secondary" onclick="getMovieDetails(${movies[i].id})">View</button>
                          <button type="button" class="btn btn-sm btn-outline-secondary">Edit</button>
                        </div>
                        <small class="text-muted">${movies[i].genre}</small>
                      </div>
                    </div>
                  </div>
                </div>`;
    }
    document.getElementById("moviesContainer").innerHTML = str;
}

// ...existing code...