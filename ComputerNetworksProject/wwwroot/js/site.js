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

async function selectStar(star,productId) {
    var rating = star.getAttribute("data-rating");
    try {
        const res = await fetch(`/products/AddRating?productId=${productId}&rate=${rating}`);
        const newRate = await res.json();
        const grandpa = star.parentNode.parentNode;
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

/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2024 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */

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


function showPassword(element) {
    // Your showPassword implementation
}

function checkPasswordStrength(inputElement) {
    var password = inputElement.value;
    var strength = calculatePasswordStrength(password);
    updateProgressBar(strength);
}

function calculatePasswordStrength(password) {

    var hasCapitalLetter = /[A-Z]/.test(password);
    var hasSpecialCharacter = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    var hasNumber = /\d/.test(password);
    var hasMinLength = password.length >= 6;


    var strength = (hasCapitalLetter + hasSpecialCharacter + hasNumber + hasMinLength) / 4 * 100;

    strength = Math.min(100, Math.max(0, strength));

    return strength;
}

function updateProgressBar(strength) {
    var progressBar = document.getElementById('password-strength-bar');
    progressBar.style.width = strength + '%';

    // Optionally, change the color based on strength
    if (strength < 50) {
        progressBar.classList.remove('bg-warning', 'bg-success');
        progressBar.classList.add('bg-danger');
    } else if (strength < 80) {
        progressBar.classList.remove('bg-danger', 'bg-success');
        progressBar.classList.add('bg-warning');
    } else {
        progressBar.classList.remove('bg-danger', 'bg-warning');
        progressBar.classList.add('bg-success');
    }

    // Show the progress bar
    progressBar.parentElement.classList.remove('d-none');
}