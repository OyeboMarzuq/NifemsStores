import apiClient from '../../utils/api.js';
import { API_CONFIG, CONSTANTS, ERROR_MESSAGES, SUCCESS_MESSAGES } from '../../config.js';
import { setToken, setUser, redirectIfAuthenticated, CONSTANTS as AUTH_CONSTANTS } from '../../utils/auth.js';
import { validateEmail } from '../../utils/validation.js';

// Redirect if already authenticated
redirectIfAuthenticated();

class LoginPage {
  constructor() {
    this.form = document.getElementById('login-form');
    this.emailInput = document.getElementById('email');
    this.passwordInput = document.getElementById('password');
    this.submitBtn = this.form.querySelector('button[type="submit"]');
    this.alertContainer = document.getElementById('alert-container');
    this.errors = {};
  }

  init() {
    this.form.addEventListener('submit', (e) => this.handleSubmit(e));
    this.emailInput.addEventListener('blur', () => this.validateEmail());
    this.passwordInput.addEventListener('blur', () => this.validatePassword());
  }

  validateEmail() {
    const value = this.emailInput.value.trim();

    if (!value) {
      this.setError('email', 'Email is required');
      return false;
    }

    if (!validateEmail(value)) {
      this.setError('email', ERROR_MESSAGES.INVALID_EMAIL);
      return false;
    }

    this.clearError('email');
    return true;
  }

  validatePassword() {
    const value = this.passwordInput.value;

    if (!value) {
      this.setError('password', 'Password is required');
      return false;
    }

    if (value.length < 6) {
      this.setError('password', 'Password must be at least 6 characters');
      return false;
    }

    this.clearError('password');
    return true;
  }

  setError(field, message) {
    this.errors[field] = message;
    const errorEl = document.getElementById(`${field}-error`);
    const inputEl = document.getElementById(field);

    if (errorEl) {
      errorEl.textContent = message;
      errorEl.style.display = 'block';
    }

    if (inputEl) {
      inputEl.classList.add('error');
    }
  }

  clearError(field) {
    delete this.errors[field];
    const errorEl = document.getElementById(`${field}-error`);
    const inputEl = document.getElementById(field);

    if (errorEl) {
      errorEl.textContent = '';
      errorEl.style.display = 'none';
    }

    if (inputEl) {
      inputEl.classList.remove('error');
    }
  }

  async handleSubmit(e) {
    e.preventDefault();

    // Validate form
    const isEmailValid = this.validateEmail();
    const isPasswordValid = this.validatePassword();

    if (!isEmailValid || !isPasswordValid) {
      this.showAlert('Please fix the errors above', 'error');
      return;
    }

    // Prepare data
    const loginData = {
      email: this.emailInput.value.trim(),
      password: this.passwordInput.value
    };

    // Show loading state
    this.setLoading(true);

    try {
      console.log('[v0] Attempting login with:', loginData.email);

      // Make API call
      const response = await apiClient.post(API_CONFIG.ENDPOINTS.LOGIN, loginData);

      if (response.success) {
        console.log('[v0] Login successful:', response.data);

        // Store token and user info
        setToken(response.data.token);
        setUser(response.data.user);

        // Show success message
        this.showAlert(SUCCESS_MESSAGES.LOGIN_SUCCESS, 'success');

        // Redirect based on role
        setTimeout(() => {
          this.redirectByRole(response.data.user.role);
        }, 500);
      } else {
        console.log('[v0] Login failed:', response.message);
        this.showAlert(response.message || ERROR_MESSAGES.SERVER_ERROR, 'error');
      }
    } catch (error) {
      console.error('[v0] Login error:', error);

      if (error.status === 401 || error.message === 'Invalid email or password') {
        this.showAlert(ERROR_MESSAGES.INVALID_CREDENTIALS, 'error');
      } else if (error.message) {
        this.showAlert(error.message, 'error');
      } else {
        this.showAlert(ERROR_MESSAGES.NETWORK_ERROR, 'error');
      }
    } finally {
      this.setLoading(false);
    }
  }

  setLoading(isLoading) {
    this.submitBtn.disabled = isLoading;
    const btnText = this.submitBtn.querySelector('.btn-text');
    const spinner = this.submitBtn.querySelector('.spinner');

    if (isLoading) {
      btnText.style.display = 'none';
      spinner.style.display = 'inline-block';
    } else {
      btnText.style.display = 'inline';
      spinner.style.display = 'none';
    }
  }

  showAlert(message, type = 'info') {
    const alertHTML = `
      <div class="auth-alert auth-alert-${type}">
        <div class="auth-alert-icon">
          ${type === 'success' ? '✓' : type === 'error' ? '✕' : 'ℹ'}
        </div>
        <div class="auth-alert-content">
          ${message}
        </div>
      </div>
    `;

    this.alertContainer.innerHTML = alertHTML;

    // Auto-dismiss success messages
    if (type === 'success') {
      setTimeout(() => {
        this.alertContainer.innerHTML = '';
      }, 3000);
    }
  }

  redirectByRole(role) {
    const redirectMap = {
      [AUTH_CONSTANTS.ROLES.CUSTOMER]: '/pages/customer/home.html',
      [AUTH_CONSTANTS.ROLES.VENDOR]: '/pages/vendor/dashboard.html',
      [AUTH_CONSTANTS.ROLES.ADMIN]: '/pages/admin/dashboard.html'
    };

    const url = redirectMap[role] || '/pages/customer/home.html';
    window.location.href = url;
  }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
  const loginPage = new LoginPage();
  loginPage.init();
});
