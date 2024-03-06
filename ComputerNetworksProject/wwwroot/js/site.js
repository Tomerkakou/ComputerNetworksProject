

//color mode
(() => {
    'use strict'

    const getStoredTheme = () => localStorage.getItem('theme')
    const setStoredTheme = theme => localStorage.setItem('theme', theme)

    const getPreferredTheme = () => {
        const storedTheme = getStoredTheme()
        if (storedTheme) {
            return storedTheme
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
    }

    const setTheme = theme => {
        if (theme === 'auto') {
            document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'))
        } else {
            document.documentElement.setAttribute('data-bs-theme', theme)
        }
    }

    setTheme(getPreferredTheme())

    const showActiveTheme = (theme, focus = false) => {
        const themeSwitcher = document.querySelector('#bd-theme')

        if (!themeSwitcher) {
            return
        }

        const themeSwitcherText = document.querySelector('#bd-theme-text')
        const activeThemeIcon = document.querySelector('.theme-icon-active','i')
        const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)
        //error line
        //const svgOfActiveBtn = btnToActive.querySelector('svg use').getAttribute('href')
        const svgOfActiveBtn = btnToActive.querySelector('i').className

        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active')
            element.querySelector(".bi-check2").classList.add('d-none')
        })

        btnToActive.classList.add('active')
        btnToActive.querySelector(".bi-check2").classList.remove("d-none")
        activeThemeIcon.className = svgOfActiveBtn +" theme-icon-active"

        if (focus) {
            themeSwitcher.focus()
        }
    }

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        const storedTheme = getStoredTheme()
        if (storedTheme !== 'light' && storedTheme !== 'dark') {
            setTheme(getPreferredTheme())
        }
    })

    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme())

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')
                    setStoredTheme(theme)
                    setTheme(theme)
                    showActiveTheme(theme, true)
                })
            })
    })
})()


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

productsHub.on("cartChanged", async (productId,cartItemAmount, cartItemPrice, cartId, cartPrice, cartItemCount) => {
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
});

function fullfilled() {

}

function rejected() {

}

productsHub.start().then(fullfilled,rejected)