import { CONSTANTS } from '../config.js';

/**
 * Get stored JWT token
 */
export function getToken() {
  return localStorage.getItem(CONSTANTS.TOKEN_KEY);
}

/**
 * Set JWT token
 */
export function setToken(token) {
  localStorage.setItem(CONSTANTS.TOKEN_KEY, token);
}

/**
 * Get stored user info
 */
export function getUser() {
  const userJson = localStorage.getItem(CONSTANTS.USER_KEY);
  return userJson ? JSON.parse(userJson) : null;
}

/**
 * Set user info
 */
export function setUser(user) {
  localStorage.setItem(CONSTANTS.USER_KEY, JSON.stringify(user));
}

/**
 * Check if user is authenticated
 */
export function isAuthenticated() {
  return !!getToken();
}

/**
 * Get user role
 */
export function getUserRole() {
  const user = getUser();
  return user?.role || null;
}

/**
 * Check if user has specific role
 */
export function hasRole(role) {
  return getUserRole() === role;
}

/**
 * Decode JWT token (basic implementation - server should validate)
 */
export function decodeToken(token) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Token decode error:', error);
    return null;
  }
}

/**
 * Check if token is expired
 */
export function isTokenExpired() {
  const token = getToken();
  if (!token) return true;

  const decoded = decodeToken(token);
  if (!decoded || !decoded.exp) return true;

  // Check if token expires in next 5 minutes
  const expiryTime = decoded.exp * 1000; // Convert to milliseconds
  const currentTime = Date.now();
  return currentTime >= expiryTime - 5 * 60 * 1000;
}

/**
 * Clear authentication data
 */
export function clearAuth() {
  localStorage.removeItem(CONSTANTS.TOKEN_KEY);
  localStorage.removeItem(CONSTANTS.USER_KEY);
}

/**
 * Refresh token (call API to get new token)
 */
export async function refreshToken() {
  try {
    const token = getToken();
    if (!token) {
      clearAuth();
      return false;
    }

    const response = await fetch(`${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REFRESH_TOKEN}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ token })
    });

    if (!response.ok) {
      clearAuth();
      return false;
    }

    const data = await response.json();
    if (data.success && data.data.token) {
      setToken(data.data.token);
      return true;
    }

    clearAuth();
    return false;
  } catch (error) {
    console.error('Token refresh error:', error);
    clearAuth();
    return false;
  }
}

/**
 * Require authentication - redirect to login if not authenticated
 */
export function requireAuth() {
  if (!isAuthenticated()) {
    window.location.href = '/pages/login.html';
    return false;
  }
  return true;
}

/**
 * Require specific role - redirect if user doesn't have role
 */
export function requireRole(role) {
  if (!isAuthenticated()) {
    window.location.href = '/pages/login.html';
    return false;
  }

  if (!hasRole(role)) {
    window.location.href = '/pages/customer/home.html';
    return false;
  }

  return true;
}

/**
 * Redirect authenticated users away from auth pages
 */
export function redirectIfAuthenticated() {
  if (isAuthenticated()) {
    const user = getUser();
    const roleDefaultPages = {
      [CONSTANTS.ROLES.CUSTOMER]: '/pages/customer/home.html',
      [CONSTANTS.ROLES.VENDOR]: '/pages/vendor/dashboard.html',
      [CONSTANTS.ROLES.ADMIN]: '/pages/admin/dashboard.html'
    };

    const redirectUrl = roleDefaultPages[user?.role] || '/pages/customer/home.html';
    window.location.href = redirectUrl;
  }
}

export { CONSTANTS };
