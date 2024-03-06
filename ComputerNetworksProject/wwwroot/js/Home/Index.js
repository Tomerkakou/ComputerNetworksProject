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

async function addToCart(btn) {
    
    var productId = btn.getAttribute("data-product");
    btn.disabled = true;
    var childs=btn.querySelectorAll("span")
    const text = childs[0];
    text.classList.add("d-none");
    const spiner = childs[1];
    spiner.classList.remove("d-none");
    const loading = childs[2];
    loading.classList.remove("d-none");

    const res = await fetch(`cart/AddItem?productId=${productId}`)
    const data = await res.json()

    const cartItemContainer = $(`#cartItem-container-${data.cartId}`)
    if (cartItemContainer.length == 0) {
        const cartOffCanvas = $("#cartoffcanvas")
        if (cartOffCanvas.length == 0) {
            console.log("error no off canvas")
        }
        $.ajax({
            url: `/Cart/GetCart?cartId=${data.cartId}`,
            type: 'GET',
            success: function (data) {
                cartOffCanvas.replaceWith(data)
            },
            error: function () {
                console.log('Error fetching data');
            }
        });
    }
    

    text.classList.remove("d-none");
    spiner.classList.add("d-none");
    loading.classList.add("d-none")
    btn.disabled = false;

}
