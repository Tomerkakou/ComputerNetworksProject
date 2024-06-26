﻿function AddError(errorMsg) {
    const alertPlaceholder = document.getElementById('client-error-placeholder')
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-warning  alert-dismissible" role="alert">`,
        `   <i class="bi bi-exclamation-triangle"></i>`,
        `   ${errorMsg}`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert"></button>',
        '</div>'
    ].join('')
    alertPlaceholder.append(wrapper)
}

function AddErrorNoClose(errorMsg) {
    const alertPlaceholder = document.getElementById('client-error-placeholder')
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-danger  alert-dismissible" role="alert">`,
        `   <i class="bi bi-exclamation-triangle"></i>`,
        `   ${errorMsg}`,
        '</div>'
    ].join('')
    alertPlaceholder.append(wrapper)
}

function ChangeInCheckout(cartId) {
    var currentUrl = window.location.href;
    if (currentUrl.includes('Checkout') && currentUrl.includes(`cartId=${cartId}`)) {
        if (currentUrl.includes('Review')) {
            location.reload()
            return
        }
        $("#checkout-back")?.addClass("disabled")
        $("#checkout-next")?.prop("disabled", true)
        $("#checkout-samePayment")?.prop("disabled", true)
        var reviewUrl = currentUrl.replace("Shipping", "Review").replace("Payment", "Review")
        const html = `Your Cart have been changed 
                    <a class="ms-1 icon-link icon-link-hover" href="${reviewUrl}">
                        Review your cart again
                        <i class="bi bi-arrow-right"></i>
                    </a>`
        AddErrorNoClose(html)
    }
}

function ClearCart() {
    $.ajax({
        url: `/Cart/GetCart`,
        type: 'GET',
        success: function (data) {
            $("#offcanvas-close").click()
            $("#cartoffcanvas").replaceWith(data)
            $("#cartIconCount").addClass("d-none")
        },
        error: function () {
            console.log('Error fetching data');
        }
    });
}
async function increaseArrow(productId) {
    const res = await fetch(`/cart/AddItem?productId=${productId}`)
    if (res.status === 400) {
        window.location.reload(true);
    }
    else {
        const data = await res.json()
        const cartItemContainer = $(`#cartItem-container-${data.cartId}`)
        if (cartItemContainer.length == 0) {
            $("#offcanvas-close").click()
            AddError("Old cart has been removed due to long idle")
            const cartOffCanvas = $("#cartoffcanvas")
            if (cartOffCanvas.length == 0) {
                console.log("error no off canvas")
            }
            $.ajax({
                url: `/Cart/GetCart?cartId=${data.cartId}`,
                type: 'GET',
                success: function (data) {
                    cartOffCanvas.replaceWith(data)
                    const badge = $("#cartIconCount")
                    badge.text(data.cartCount)
                    badge.removeClass("d-none")

                },
                error: function () {
                    console.log('Error fetching data');
                }
            });
        }
    }
}

async function decreaseArrow(productId) {
    const res = await fetch(`/cart/DecreaseItem?productId=${productId}`)
    if (res.status === 403) {
        ClearCart()
        AddError(await res.text())
    }
    else if (res.status === 400) {
        window.location.reload(true);
    }
}

async function deleteItem(productId) {
    const res = await fetch(`/cart/DeleteItem?productId=${productId}`)
    if (res.status === 403) {
        ClearCart()
        AddError(await res.text())
    }
}

async function deleteCart() {
    const res = await fetch(`/cart/DeleteCart`)
}

async function addToCart(btn) {

    var productId = btn.getAttribute("data-product");
    btn.disabled = true;
    var childs = btn.querySelectorAll("span")
    const text = childs[0];
    text.classList.add("d-none");
    const spiner = childs[1];
    spiner.classList.remove("d-none");
    const loading = childs[2];
    loading.classList.remove("d-none");

    const res = await fetch(`cart/AddItem?productId=${productId}`)
    if (res.status === 400) {
        window.location.reload(true);
    }
    else {
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
                    const badge = $("#cartIconCount")
                    badge.text(data.cartCount)
                    badge.removeClass("d-none")

                },
                error: function () {
                    console.log('Error fetching data');
                }
            });
        }

    }
    text.classList.remove("d-none");
    spiner.classList.add("d-none");
    loading.classList.add("d-none")
    btn.disabled = false;

}


const productsHub = new signalR.HubConnectionBuilder().withUrl("/hubs/productshub").build();

productsHub.on("productNewAvailableStock", async (productId, stock) => {
    const productCard = $(`#productCard-${productId}`)
    const productTr = $(`#productTr-${productId}`)
    if (productCard.length > 0) {
        $.ajax({
            url: `/Home/GetProductCardParital?productId=${productId}`,
            type: 'GET',
            success: function (data) {
                const parent = productCard.parent()
                const index = parent.children().index(productCard)
                productCard.remove()
                if (index === parent.children().length) {
                    parent.append(data)
                }
                else {
                    parent.children().eq(index).before(data);
                }
            },
            error: function () {
                console.log('Error fetching data');
            }
        });
    }
    else if (productTr.length > 0) {
        $.ajax({
            url: `/Home/GetProductCardParital?productId=${productId}&type=tr`,
            type: 'GET',
            success: function (data) {
                productTr.children(':not(:first-child)').remove();
                productTr.append(data);
            },
            error: function () {
                console.log('Error fetching data');
            }
        });
    }

    const rightarrow = $(`#rightArrow-${productId}`)
    if (rightarrow.length > 0) {
        //not the same cart but the same product
        rightarrow.prop("disabled", stock == 0)
    }
});

productsHub.on("newCart", async (cartId) => {
    const cartOffCanvas = $("#cartoffcanvas")
    if (cartOffCanvas.length == 0) {
        console.log("error no off canvas")
    }
    $.ajax({
        url: `/Cart/GetCart?cartId=${cartId}`,
        type: 'GET',
        success: function (data) {
            cartOffCanvas.replaceWith(data)
            const badge = $("#cartIconCount")
            badge.text(data.cartCount)
            badge.removeClass("d-none")
        },
        error: function () {
            console.log('Error fetching data');
        }
    });
})

productsHub.on("cartChanged", async (productId, cartItemAmount, cartItemPrice, cartId, cartPrice, cartItemCount) => {
    const cartItemContainer = $(`#cartItem-container-${cartId}`)
    if (cartItemContainer.length > 0) {
        const cartItemParent = $(`#cartItem-${productId}-${cartId}`)
        if (cartItemParent.length > 0 && cartItemParent.children().length == 3) {
            //same cart and cartitems exists
            const itemPrice = cartItemParent.children().eq(1);
            const cartItemQtyParent = cartItemParent.children().eq(2);
            const leftarrow = cartItemQtyParent.children().eq(0);
            const input = cartItemQtyParent.children().eq(1);
            itemPrice.text(`${cartItemPrice}$`)
            leftarrow.prop("disabled", cartItemAmount == 1)
            input.val(cartItemAmount)
        }
        else {
            //same cart not such item so we will send ajax call
            $.ajax({
                url: `/Cart/GetCartItem?productId=${productId}&cartId=${cartId}`,
                type: 'GET',
                success: function (data) {
                    cartItemContainer.append(data)
                },
                error: function () {
                    console.log('Error fetching data');
                }
            });
            const totalItems = $("#totalItems")
            totalItems.text(`Total items: ${cartItemCount}`);
            const cartIconBadge = document.getElementById("cartIconCount")
            cartIconBadge.classList.remove("d-none")
            cartIconBadge.innerText = cartItemCount
        }

        const totalPrice = $("#totalPrice")
        totalPrice.text(`Total price: ${cartPrice}$`)
    }

    ChangeInCheckout(cartId);

});


productsHub.on("cartItemRemove", async (productId, cartId, cartPrice, cartItemCount) => {
    const cartItemContainer = $(`#cartItem-container-${cartId}`)
    if (cartItemContainer.length > 0) {
        const cartItemParent = $(`#cartItem-${productId}-${cartId}`)
        if (cartItemParent.length > 0 && cartItemParent.children().length == 3) {
            //same cart and cartitems exists
            const parent = cartItemParent.parent().parent().remove()
        }
        const totalItems = $("#totalItems")
        totalItems.text(`Total items: ${cartItemCount}`);
        const cartIconBadge = document.getElementById("cartIconCount")
        cartIconBadge.innerText = cartItemCount
        const totalPrice = $("#totalPrice")
        totalPrice.text(`Total price: ${cartPrice}$`)
    }

    ChangeInCheckout(cartId);
})

productsHub.on("clearCart", async (cartId, redirect = true) => {
    const cartItemContainer = $(`#cartItem-container-${cartId}`)
    if (cartItemContainer.length > 0) {
        const offcanvas = cartItemContainer.parent().parent()
        $.ajax({
            url: `/Cart/GetCart`,
            type: 'GET',
            success: function (data) {
                $("#offcanvas-close").click()
                offcanvas.replaceWith(data)
                $("#cartIconCount").addClass("d-none")
            },
            error: function () {
                console.log('Error fetching data');
            }
        });
    }
    if (redirect) {
        var currentUrl = window.location.href;
        if (currentUrl.includes('Checkout') && currentUrl.includes(`cartId=${cartId}`)) {
            var homeUrl = "/?msg=Your cart has been deleted while you were in the checkout process."
            location.href = homeUrl;
        }
    }

})

productsHub.on("productPriceChanged", async (cartIds) => {
    var currentUrl = window.location.href;
    for (const cartId of cartIds) {
        if (currentUrl.includes('Checkout') && currentUrl.includes(`cartId=${cartId}`)) {
            if (currentUrl.includes('Review')) {
                location.reload()
                return
            }
            $("#checkout-back")?.addClass("disabled")
            $("#checkout-next")?.prop("disabled", true)
            $("#checkout-samePayment")?.prop("disabled", true)
            var reviewUrl = currentUrl.replace("Shipping", "Review").replace("Payment", "Review")
            const html = `Admin changed the price of product in your cart please go back and 
                        <a class="ms-1 icon-link icon-link-hover" href="${reviewUrl}">
                          Review your cart again
                          <i class="bi bi-arrow-right"></i>
                        </a>`
            AddErrorNoClose(html)
        }
    }
})


function fullfilled() {

}

function rejected() {

}

productsHub.start().then(fullfilled, rejected)