//date range input
/*
const start = $('input[name="FilterInput.StartDate"]').val()
const end = $('input[name="FilterInput.EndDate"]').val()
locale = {
    "format": "DD/MM/YYYY",
    "separator": " - ",
    "applyLabel": "Apply",
    "cancelLabel": "Clear",
}
if (start != '') {
    locale.startDate = start
}
if (end != '') {
    locale.endDate = end
}
$('input[name="FilterDate"]').daterangepicker({
    "locale": locale,
    "autoUpdateInput": !(end && start),
    "startDate": start,
    "endDate": end,
    "parentEl": "filter-body",
    "opens": "center",
    "cancelClass": "btn btn-secondary"
}, function (start, end, label) {
    $('input[name="FilterInput.StartDate"]').val(start.format('YYYY-MM-DDTHH:mm'))
    $('input[name="FilterInput.EndDate"]').val(end.format('YYYY-MM-DDTHH:mm'))
});
$('input[name="FilterDate"]').on('apply.daterangepicker', function (ev, picker) {
    $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
    $('input[name="FilterInput.StartDate"]').val(picker.startDate.format('YYYY-MM-DDTHH:mm'))
    $('input[name="FilterInput.EndDate"]').val(picker.endDate.format('YYYY-MM-DDTHH:mm'))
});

$('input[name="FilterDate"]').on('cancel.daterangepicker', function (ev, picker) {
    $(this).val('');
});
*/
//stars select section
/*
$(document).ready(function () {
    // Function to load and display all cookies
    function loadCookies() {
        var allCookies = document.cookie.split('; ');

        for (var i = 0; i < allCookies.length; i++) {
            var cookieParts = allCookies[i].split('=');
            var cookieName = cookieParts[0];
            if (cookieName.startsWith('products-rating-')) {
                const rating = document.getElementById(cookieName)
                if (rating != null) {
                    rating.remove();
                }
            }
            
        }
    }

    loadCookies();
});
*/
function updateValue(value) {
    document.getElementById('filter-rate').innerText = value;
}

function highlightStars(star) {
    var rating = star.getAttribute("data-rating");
    
    var stars = star.parentNode.querySelectorAll(".star");

    stars.forEach(function (s) {
        s.classList.remove("active");
        if (s.getAttribute("data-rating") <= rating) {
            s.classList.add("active");
        }
    });
}

function resetStars() {
    const rating = document.getElementById("rating")
    if (rating == null) {
        return;
    }
    const rate = rating.value
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

async function selectStar(star, productId) {
    var rating = star.getAttribute("data-rating");
    try {
        const res = await fetch(`/products/AddRating?productId=${productId}&rate=${rating}`);
        const newRate = await res.json();
        const grandpa = star.parentNode;
        while (grandpa.firstChild) {
            grandpa.removeChild(grandpa.firstChild);
        }
        var newSpan = document.createElement("span");
        newSpan.appendChild(document.createTextNode("Thank you!"));
        newSpan.classList.add("fw-bold");
        grandpa.appendChild(newSpan);
        document.getElementById(`product-${productId}-rate`).innerText = newRate;
    } catch (err) {
        console.log(err);
    }

}

function updateModal(productId,productName) {
    $("#notify-productId").val(productId)
    $("#notify-product-name").text(productName)
}

async function RegisterNotify() {    
    const userEmail = $("#notify-userEmail").val();
    if (userEmail === null || userEmail == '' || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(userEmail)) {
        $("#notify-userEmail").addClass("is-invalid")
        return;
    }
    const formData = $('#notify-form').serialize();
    $.ajax({
        type: 'POST',
        url: '/products/notify',
        data: formData,
        success: function (response) {
            $('#NotifyModal').modal('hide');
            alert(response)
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

