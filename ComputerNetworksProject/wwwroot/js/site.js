// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function highlightStars(star) {
    var rating = star.getAttribute("data-rating");
    var stars = document.querySelectorAll(".star");
    
    stars.forEach(function (s) {
        s.classList.remove("active");
        if (s.getAttribute("data-rating") <= rating) {
            s.classList.add("active");
        }
    });
}

function resetStars() {
    const rate = document.getElementById("rating").value
    if (rate == 0) {
        var activeStars = document.querySelectorAll(".star.active");
        activeStars.forEach(function (s) {
            s.classList.remove("active");
        });

        var selectedStar = activeStars[activeStars.length];
        if (selectedStar) {
            selectedStar.classList.add("active");
        }
    }
}

function selectStar(star) {
    var rating = star.getAttribute("data-rating");
    document.getElementById("rating").value = rating;
}
