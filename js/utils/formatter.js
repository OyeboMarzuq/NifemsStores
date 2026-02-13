/**
 * Format price/currency
 */
export function formatCurrency(amount, currency = 'USD') {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: currency
  }).format(amount);
}

/**
 * Format number with commas
 */
export function formatNumber(number) {
  return new Intl.NumberFormat('en-US').format(number);
}

/**
 * Format date
 */
export function formatDate(dateString) {
  if (!dateString) return '';
  
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(date);
}

/**
 * Format date and time
 */
export function formatDateTime(dateString) {
  if (!dateString) return '';
  
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
}

/**
 * Format relative time (e.g., "2 hours ago")
 */
export function formatRelativeTime(dateString) {
  if (!dateString) return '';
  
  const date = new Date(dateString);
  const now = new Date();
  const seconds = Math.floor((now - date) / 1000);
  
  if (seconds < 60) return 'just now';
  if (seconds < 3600) return `${Math.floor(seconds / 60)} minutes ago`;
  if (seconds < 86400) return `${Math.floor(seconds / 3600)} hours ago`;
  if (seconds < 604800) return `${Math.floor(seconds / 86400)} days ago`;
  
  return formatDate(dateString);
}

/**
 * Format percentage
 */
export function formatPercentage(value) {
  return `${(value * 100).toFixed(2)}%`;
}

/**
 * Truncate text
 */
export function truncateText(text, maxLength = 100) {
  if (!text) return '';
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
}

/**
 * Capitalize first letter
 */
export function capitalize(string) {
  if (!string) return '';
  return string.charAt(0).toUpperCase() + string.slice(1);
}

/**
 * Convert status to readable format
 */
export function formatStatus(status) {
  return status
    .split('_')
    .map(word => capitalize(word))
    .join(' ');
}

/**
 * Format file size
 */
export function formatFileSize(bytes) {
  if (!bytes) return '0 Bytes';
  
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
}

/**
 * Format phone number
 */
export function formatPhoneNumber(phone) {
  if (!phone) return '';
  
  // Remove non-digits
  const digits = phone.replace(/\D/g, '');
  
  // Format as (XXX) XXX-XXXX if 10 digits
  if (digits.length === 10) {
    return `(${digits.slice(0, 3)}) ${digits.slice(3, 6)}-${digits.slice(6)}`;
  }
  
  return phone;
}

/**
 * Slug-ify string
 */
export function slugify(string) {
  if (!string) return '';
  
  return string
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-');
}

/**
 * Format duration in seconds to readable format
 */
export function formatDuration(seconds) {
  if (seconds < 60) return `${seconds}s`;
  if (seconds < 3600) return `${Math.floor(seconds / 60)}m`;
  if (seconds < 86400) return `${Math.floor(seconds / 3600)}h`;
  return `${Math.floor(seconds / 86400)}d`;
}
