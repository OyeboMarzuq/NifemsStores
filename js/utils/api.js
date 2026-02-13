import { API_CONFIG, CONSTANTS, ERROR_MESSAGES } from '../config.js';
import { getToken, refreshToken, clearAuth } from './auth.js';

class APIClient {
  constructor(baseURL = API_CONFIG.BASE_URL) {
    this.baseURL = baseURL;
    this.timeout = API_CONFIG.TIMEOUT;
  }

  /**
   * Make an API request
   */
  async request(method, endpoint, data = null, isFormData = false) {
    const url = `${this.baseURL}${endpoint}`;
    const token = getToken();

    const headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };

    // Add authorization header if token exists
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const options = {
      method,
      headers,
      credentials: 'include' // For cookies if using session-based auth
    };

    // Handle form data (for file uploads)
    if (isFormData && data instanceof FormData) {
      delete options.headers['Content-Type'];
      options.body = data;
    } else if (data) {
      options.body = JSON.stringify(data);
    }

    try {
      const response = await Promise.race([
        fetch(url, options),
        new Promise((_, reject) =>
          setTimeout(() => reject(new Error('Request timeout')), this.timeout)
        )
      ]);

      // Handle 401 Unauthorized - token might be expired
      if (response.status === 401) {
        try {
          const refreshed = await refreshToken();
          if (refreshed) {
            // Retry the request with new token
            return this.request(method, endpoint, data, isFormData);
          } else {
            clearAuth();
            window.location.href = '/pages/login.html';
            throw new Error('Unauthorized');
          }
        } catch (error) {
          clearAuth();
          window.location.href = '/pages/login.html';
          throw new Error('Session expired. Please log in again.');
        }
      }

      const responseData = await response.json();

      if (!response.ok) {
        throw {
          status: response.status,
          message: responseData.message || ERROR_MESSAGES.SERVER_ERROR,
          errors: responseData.errors || []
        };
      }

      return responseData;
    } catch (error) {
      if (error.message === 'Request timeout') {
        throw new Error('Request timeout. Please try again.');
      }
      throw error;
    }
  }

  // GET request
  get(endpoint) {
    return this.request('GET', endpoint);
  }

  // POST request
  post(endpoint, data) {
    return this.request('POST', endpoint, data);
  }

  // POST with form data (for file uploads)
  postFormData(endpoint, data) {
    return this.request('POST', endpoint, data, true);
  }

  // PUT request
  put(endpoint, data) {
    return this.request('PUT', endpoint, data);
  }

  // PUT with form data
  putFormData(endpoint, data) {
    return this.request('PUT', endpoint, data, true);
  }

  // DELETE request
  delete(endpoint) {
    return this.request('DELETE', endpoint);
  }

  // PATCH request
  patch(endpoint, data) {
    return this.request('PATCH', endpoint, data);
  }
}

// Create a singleton instance
const apiClient = new APIClient();

export default apiClient;
