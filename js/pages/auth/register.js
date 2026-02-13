import apiClient from '../../utils/api.js';
import { API_CONFIG, CONSTANTS, ERROR_MESSAGES, SUCCESS_MESSAGES } from '../../config.js';
import { redirectIfAuthenticated, CONSTANTS as AUTH_CONSTANTS } from '../../utils/auth.js';
import { validateEmail, validatePassword, validatePasswordMatch, validatePhoneNumber, validatePostalCode, validateForm } from '../../utils/validation.js';

// Redirect if already authenticated
redirectIfAuthenticated();

class RegisterPage {
  constructor() {
    this.form = document.getElementById('register-form');
    this.currentStep = 1;
    this.totalSteps = 3;
    this.formData = {};
    this.errors = {};
    this.alertContainer = document.getElementById('alert-container');
  }

  init() {
    this.setupEventListeners();
    this.setupPasswordStrengthMeter();
  }

  setupEventListeners() {
    // Navigation buttons
    document.getElementById('next-btn').addEventListener('click', () => this.nextStep());
    document.getElementById('prev-btn').addEventListener('click', () => this.prevStep());

    // Password strength meter
    document.getElementById('password').addEventListener('input', () => this.updatePasswordStrength());

    // Form submission
    this.form.addEventListener('submit', (e) => this.handleSubmit(e));

    // Real-time validation
    document.getElementById('email').addEventListener('blur', () => this.validateEmailField());
    document.getElementById('password').addEventListener('blur', () => this.validatePasswordField());
    document.getElementById('confirm-password').addEventListener('blur', () => this.validateConfirmPasswordField());
    document.getElementById('phone').addEventListener('blur', () => this.validatePhoneField());
    document.getElementById('zip').addEventListener('blur', () => this.validateZipField());
  }

  setupPasswordStrengthMeter() {
    const passwordInput = document.getElementById('password');
    passwordInput.addEventListener('input', () => {
      this.updatePasswordStrength();
    });
  }

  updatePasswordStrength() {
    const password = document.getElementById('password').value;
    const strengthBar = document.getElementById('strength-bar');
    const strengthText = document.getElementById('strength-text');

    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;

    let strengthLevel = 'Weak';
    strengthBar.className = 'strength-bar';

    if (strength >= 3) {
      strengthLevel = 'Strong';
      strengthBar.classList.add('strong');
    } else if (strength === 2) {
      strengthLevel = 'Fair';
      strengthBar.classList.add('fair');
    } else {
      strengthLevel = 'Weak';
      strengthBar.classList.add('weak');
    }

    strengthText.textContent = strengthLevel;
    strengthText.className = `strength-text ${strengthLevel.toLowerCase()}`;
  }

  validateEmailField() {
    const email = document.getElementById('email').value.trim();

    if (!email) {
      this.setError('email', 'Email is required');
      return false;
    }

    if (!validateEmail(email)) {
      this.setError('email', ERROR_MESSAGES.INVALID_EMAIL);
      return false;
    }

    this.clearError('email');
    return true;
  }

  validatePasswordField() {
    const password = document.getElementById('password').value;

    if (!password) {
      this.setError('password', 'Password is required');
      return false;
    }

    if (!validatePassword(password)) {
      this.setError('password', ERROR_MESSAGES.WEAK_PASSWORD);
      return false;
    }

    this.clearError('password');
    return true;
  }

  validateConfirmPasswordField() {
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirm-password').value;

    if (!confirmPassword) {
      this.setError('confirm-password', 'Please confirm your password');
      return false;
    }

    if (!validatePasswordMatch(password, confirmPassword)) {
      this.setError('confirm-password', 'Passwords do not match');
      return false;
    }

    this.clearError('confirm-password');
    return true;
  }

  validatePhoneField() {
    const phone = document.getElementById('phone').value;

    if (!phone) {
      this.setError('phone', 'Phone number is required');
      return false;
    }

    if (!validatePhoneNumber(phone)) {
      this.setError('phone', 'Please enter a valid phone number');
      return false;
    }

    this.clearError('phone');
    return true;
  }

  validateZipField() {
    const zip = document.getElementById('zip').value;

    if (!zip) {
      this.setError('zip', 'Postal code is required');
      return false;
    }

    if (!validatePostalCode(zip)) {
      this.setError('zip', 'Please enter a valid postal code');
      return false;
    }

    this.clearError('zip');
    return true;
  }

  validateStep(step) {
    const rules = this.getStepValidationRules(step);
    const formData = this.collectFormData(step);
    const errors = validateForm(formData, rules);

    if (Object.keys(errors).length > 0) {
      for (const [field, message] of Object.entries(errors)) {
        this.setError(field, message);
      }
      return false;
    }

    return true;
  }

  getStepValidationRules(step) {
    if (step === 1) {
      return {
        email: { required: true, type: 'email', label: 'Email' },
        password: { required: true, type: 'password', label: 'Password' },
        confirmPassword: {
          required: true,
          validate: () => {
            const pass = document.getElementById('password').value;
            const confirm = document.getElementById('confirm-password').value;
            return pass === confirm;
          },
          message: 'Passwords do not match',
          label: 'Confirm Password'
        },
        accountType: { required: true, label: 'Account Type' }
      };
    } else if (step === 2) {
      return {
        firstName: { required: true, label: 'First Name' },
        lastName: { required: true, label: 'Last Name' },
        phone: { required: true, type: 'phone', label: 'Phone' },
        address: { required: true, label: 'Address' },
        city: { required: true, label: 'City' },
        state: { required: true, label: 'State' },
        zip: { required: true, label: 'Postal Code' },
        country: { required: true, label: 'Country' }
      };
    } else if (step === 3) {
      return {
        terms: {
          required: true,
          validate: () => document.querySelector('input[name="terms"]').checked,
          message: 'You must agree to the terms',
          label: 'Terms'
        }
      };
    }

    return {};
  }

  collectFormData(step) {
    const stepElement = document.querySelector(`.reg-step[data-step="${step}"]`);
    const data = {};

    if (stepElement) {
      const inputs = stepElement.querySelectorAll('input, select');
      inputs.forEach(input => {
        if (input.type === 'checkbox') {
          data[input.name] = input.checked;
        } else {
          data[input.name] = input.value;
        }
      });
    }

    return data;
  }

  nextStep() {
    if (!this.validateStep(this.currentStep)) {
      this.showAlert('Please fix the errors above', 'error');
      return;
    }

    // Save step data
    const stepData = this.collectFormData(this.currentStep);
    this.formData = { ...this.formData, ...stepData };

    if (this.currentStep === 3) {
      // Submit form
      this.submitRegistration();
      return;
    }

    // Move to next step
    this.currentStep++;
    this.updateStepDisplay();

    if (this.currentStep === 3) {
      this.updateReviewInfo();
    }
  }

  prevStep() {
    if (this.currentStep > 1) {
      // Save current step data
      const stepData = this.collectFormData(this.currentStep);
      this.formData = { ...this.formData, ...stepData };

      this.currentStep--;
      this.updateStepDisplay();
    }
  }

  updateStepDisplay() {
    // Hide all steps
    document.querySelectorAll('.reg-step').forEach(step => {
      step.classList.remove('active');
    });

    // Show current step
    document.querySelector(`.reg-step[data-step="${this.currentStep}"]`).classList.add('active');

    // Update step indicators
    document.querySelectorAll('.reg-step-indicator').forEach((indicator, index) => {
      const stepNumber = index + 1;
      indicator.classList.remove('active', 'completed');

      if (stepNumber < this.currentStep) {
        indicator.classList.add('completed');
      } else if (stepNumber === this.currentStep) {
        indicator.classList.add('active');
      }
    });

    // Update buttons
    document.getElementById('prev-btn').disabled = this.currentStep === 1;
    const nextBtn = document.getElementById('next-btn');
    if (this.currentStep === 3) {
      nextBtn.textContent = 'Create Account';
    } else {
      nextBtn.textContent = 'Next →';
    }
  }

  updateReviewInfo() {
    const reviewEl = document.getElementById('review-info');
    const data = this.formData;

    const reviewHTML = `
      <p><strong>Email:</strong> ${data.email}</p>
      <p><strong>Account Type:</strong> ${data.accountType === 'vendor' ? 'Vendor' : 'Customer'}</p>
      <p><strong>Name:</strong> ${data.firstName} ${data.lastName}</p>
      <p><strong>Phone:</strong> ${data.phone}</p>
      <p><strong>Address:</strong> ${data.address}, ${data.city}, ${data.state} ${data.zip}, ${data.country}</p>
    `;

    reviewEl.innerHTML = reviewHTML;
  }

  async submitRegistration() {
    const nextBtn = document.getElementById('next-btn');
    const originalText = nextBtn.textContent;
    nextBtn.disabled = true;
    nextBtn.innerHTML = '<span class="spinner"></span>';

    try {
      console.log('[v0] Submitting registration:', this.formData);

      const registrationData = {
        email: this.formData.email,
        password: this.formData.password,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        phone: this.formData.phone,
        role: this.formData.accountType === 'vendor' ? 'vendor' : 'customer',
        address: this.formData.address,
        city: this.formData.city,
        state: this.formData.state,
        postalCode: this.formData.zip,
        country: this.formData.country,
        subscribeNewsletter: this.formData.newsletter
      };

      const response = await apiClient.post(API_CONFIG.ENDPOINTS.REGISTER, registrationData);

      if (response.success) {
        console.log('[v0] Registration successful');
        this.showAlert(SUCCESS_MESSAGES.REGISTER_SUCCESS, 'success');

        setTimeout(() => {
          window.location.href = '/pages/login.html';
        }, 2000);
      } else {
        this.showAlert(response.message || ERROR_MESSAGES.SERVER_ERROR, 'error');
      }
    } catch (error) {
      console.error('[v0] Registration error:', error);

      if (error.message?.includes('email')) {
        this.showAlert(ERROR_MESSAGES.USER_EXISTS, 'error');
      } else {
        this.showAlert(error.message || ERROR_MESSAGES.NETWORK_ERROR, 'error');
      }
    } finally {
      nextBtn.disabled = false;
      nextBtn.innerHTML = originalText;
    }
  }

  setError(field, message) {
    this.errors[field] = message;
    const errorEl = document.getElementById(`${field}-error`);
    const inputEl = document.getElementById(field.replace(/([A-Z])/g, '-$1').toLowerCase());

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
    const inputEl = document.getElementById(field.replace(/([A-Z])/g, '-$1').toLowerCase());

    if (errorEl) {
      errorEl.textContent = '';
      errorEl.style.display = 'none';
    }

    if (inputEl) {
      inputEl.classList.remove('error');
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

    if (type === 'success') {
      setTimeout(() => {
        this.alertContainer.innerHTML = '';
      }, 3000);
    }
  }

  handleSubmit(e) {
    e.preventDefault();
    // Form submission is handled by the button click handler
  }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
  const registerPage = new RegisterPage();
  registerPage.init();
});
