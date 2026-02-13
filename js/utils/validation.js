import { ERROR_MESSAGES } from '../config.js';

/**
 * Validate email format
 */
export function validateEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

/**
 * Validate password strength
 * Requirements: At least 8 chars, 1 uppercase, 1 lowercase, 1 number
 */
export function validatePassword(password) {
  const minLength = password.length >= 8;
  const hasUppercase = /[A-Z]/.test(password);
  const hasLowercase = /[a-z]/.test(password);
  const hasNumber = /[0-9]/.test(password);

  return minLength && hasUppercase && hasLowercase && hasNumber;
}

/**
 * Validate passwords match
 */
export function validatePasswordMatch(password, confirmPassword) {
  return password === confirmPassword;
}

/**
 * Validate required field
 */
export function validateRequired(value) {
  if (typeof value === 'string') {
    return value.trim().length > 0;
  }
  return !!value;
}

/**
 * Validate min length
 */
export function validateMinLength(value, minLength) {
  return value.toString().length >= minLength;
}

/**
 * Validate max length
 */
export function validateMaxLength(value, maxLength) {
  return value.toString().length <= maxLength;
}

/**
 * Validate number range
 */
export function validateRange(value, min, max) {
  const num = parseFloat(value);
  return num >= min && num <= max;
}

/**
 * Validate phone number
 */
export function validatePhoneNumber(phone) {
  const phoneRegex = /^[+]?[(]?[0-9]{3}[)]?[-\s.]?[0-9]{3}[-\s.]?[0-9]{4,6}$/;
  return phoneRegex.test(phone.replace(/\s/g, ''));
}

/**
 * Validate postal code/ZIP code
 */
export function validatePostalCode(postalCode) {
  // Basic validation - accepts 5-9 digit codes and some international formats
  const postalRegex = /^[a-zA-Z0-9\s-]{5,9}$/;
  return postalRegex.test(postalCode);
}

/**
 * Validate URL
 */
export function validateURL(url) {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
}

/**
 * Validate credit card number (Luhn algorithm)
 */
export function validateCreditCard(cardNumber) {
  const cleanNumber = cardNumber.replace(/\D/g, '');
  
  if (cleanNumber.length < 13 || cleanNumber.length > 19) {
    return false;
  }

  let sum = 0;
  let isEven = false;

  for (let i = cleanNumber.length - 1; i >= 0; i--) {
    let digit = parseInt(cleanNumber[i], 10);

    if (isEven) {
      digit *= 2;
      if (digit > 9) {
        digit -= 9;
      }
    }

    sum += digit;
    isEven = !isEven;
  }

  return sum % 10 === 0;
}

/**
 * Validate credit card expiry
 */
export function validateCardExpiry(month, year) {
  const currentDate = new Date();
  const currentYear = currentDate.getFullYear();
  const currentMonth = currentDate.getMonth() + 1;

  const expiryYear = parseInt(year, 10);
  const expiryMonth = parseInt(month, 10);

  if (expiryYear < currentYear) return false;
  if (expiryYear === currentYear && expiryMonth < currentMonth) return false;

  return true;
}

/**
 * Validate CVV/CVC
 */
export function validateCVV(cvv) {
  return /^[0-9]{3,4}$/.test(cvv);
}

/**
 * Validate form object
 */
export function validateForm(formData, rules) {
  const errors = {};

  for (const field in rules) {
    const value = formData[field];
    const rule = rules[field];

    // Required validation
    if (rule.required && !validateRequired(value)) {
      errors[field] = `${rule.label || field} is required`;
      continue;
    }

    if (!value) continue; // Skip other validations if empty and not required

    // Email validation
    if (rule.type === 'email' && !validateEmail(value)) {
      errors[field] = ERROR_MESSAGES.INVALID_EMAIL;
    }

    // Password validation
    if (rule.type === 'password' && !validatePassword(value)) {
      errors[field] = ERROR_MESSAGES.WEAK_PASSWORD;
    }

    // Min length validation
    if (rule.minLength && !validateMinLength(value, rule.minLength)) {
      errors[field] = `${rule.label || field} must be at least ${rule.minLength} characters`;
    }

    // Max length validation
    if (rule.maxLength && !validateMaxLength(value, rule.maxLength)) {
      errors[field] = `${rule.label || field} must not exceed ${rule.maxLength} characters`;
    }

    // Phone validation
    if (rule.type === 'phone' && !validatePhoneNumber(value)) {
      errors[field] = 'Please enter a valid phone number';
    }

    // Custom validation
    if (rule.validate && !rule.validate(value)) {
      errors[field] = rule.message || `${rule.label || field} is invalid`;
    }
  }

  return errors;
}

/**
 * Sanitize HTML input (prevent XSS)
 */
export function sanitizeHTML(html) {
  const textarea = document.createElement('textarea');
  textarea.textContent = html;
  return textarea.innerHTML;
}

/**
 * Validate file upload
 */
export function validateFile(file, options = {}) {
  const errors = [];

  if (!file) {
    errors.push('File is required');
    return errors;
  }

  // Max file size (default 5MB)
  const maxSize = options.maxSize || 5 * 1024 * 1024;
  if (file.size > maxSize) {
    errors.push(`File size must not exceed ${maxSize / 1024 / 1024}MB`);
  }

  // Allowed types
  if (options.allowedTypes && !options.allowedTypes.includes(file.type)) {
    errors.push(
      `File type must be one of: ${options.allowedTypes.join(', ')}`
    );
  }

  // Allowed extensions
  if (options.allowedExtensions) {
    const ext = file.name.split('.').pop().toLowerCase();
    if (!options.allowedExtensions.includes(ext)) {
      errors.push(
        `File extension must be one of: ${options.allowedExtensions.join(', ')}`
      );
    }
  }

  return errors;
}
