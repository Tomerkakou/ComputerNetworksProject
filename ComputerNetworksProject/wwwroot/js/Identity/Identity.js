
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

    progressBar.parentElement.classList.remove('d-none');
}

function showPassword(clickedButton) {
    const inputContainer = clickedButton.closest('.input-group');
    const passwordInput = inputContainer.querySelector('input');
    const iconElement = clickedButton.querySelector('i');
    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordInput.setAttribute('type', type);
    if (type === 'password') {
        iconElement.classList.remove('bi-eye-slash-fill');
        iconElement.classList.add('bi-eye');
    } else {
        iconElement.classList.remove('bi-eye-fill');
        iconElement.classList.add('bi-eye-slash-fill');
    }
}