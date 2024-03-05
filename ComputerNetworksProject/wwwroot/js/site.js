

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

productsHub.on("productNewAvailableStock", async (productId, stock, cartItemAmount) => {
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
    
    const cartItemQtyParent = $(`#cartItem-qty-${productId}`)
    if (cartItemQtyParent.length > 0 && cartItemQtyParent.children().length == 3) {
        const leftarrow = cartItemQtyParent.children().eq(0);
        const input = cartItemQtyParent.children().eq(1);
        const rightarrow = cartItemQtyParent.children().eq(2);

        leftarrow.prop("disabled", cartItemAmount == 1)
        input.val(cartItemAmount)
        rightarrow.prop("disabled",stock==0)
       
    }

});

function fullfilled() {

}

function rejected() {

}

productsHub.start().then(fullfilled,rejected)