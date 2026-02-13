// API Configuration
const API_CONFIG = {
  // Change this to your ASP.NET backend URL
  BASE_URL: 'https://api.example.com/api',
  TIMEOUT: 10000,
  
  // Endpoints
  ENDPOINTS: {
    // Auth
    REGISTER: '/auth/register',
    LOGIN: '/auth/login',
    LOGOUT: '/auth/logout',
    REFRESH_TOKEN: '/auth/refresh-token',
    GET_ME: '/auth/me',
    
    // Products
    GET_PRODUCTS: '/products',
    GET_PRODUCT_DETAIL: '/products/{id}',
    SEARCH_PRODUCTS: '/products/search',
    GET_CATEGORIES: '/categories',
    
    // Cart & Orders
    ADD_TO_CART: '/cart/add',
    REMOVE_FROM_CART: '/cart/remove/{itemId}',
    GET_CART: '/cart',
    CREATE_ORDER: '/orders',
    GET_ORDERS: '/orders',
    GET_ORDER_DETAIL: '/orders/{id}',
    UPDATE_ORDER_STATUS: '/orders/{id}/status',
    
    // Payment
    PROCESS_PAYMENT: '/payments/process',
    VALIDATE_PAYMENT: '/payments/validate',
    GET_PAYMENT_METHODS: '/payment-methods',
    
    // Vendor
    GET_VENDORS: '/vendors',
    REGISTER_VENDOR: '/vendors/register',
    GET_VENDOR_DETAIL: '/vendors/{id}',
    UPDATE_VENDOR_SETTINGS: '/vendors/{id}/settings',
    GET_VENDOR_ORDERS: '/vendors/{id}/orders',
    GET_VENDOR_ANALYTICS: '/vendors/{id}/analytics',
    CREATE_PRODUCT: '/products',
    UPDATE_PRODUCT: '/products/{id}',
    DELETE_PRODUCT: '/products/{id}',
    
    // Admin
    GET_USERS: '/admin/users',
    UPDATE_USER: '/admin/users/{id}',
    GET_ADMIN_VENDORS: '/admin/vendors',
    UPDATE_VENDOR_STATUS: '/admin/vendors/{id}/status',
    GET_ADMIN_ORDERS: '/admin/orders',
    GET_ADMIN_ANALYTICS: '/admin/analytics'
  }
};

// Constants
const CONSTANTS = {
  TOKEN_KEY: 'auth_token',
  USER_KEY: 'user_info',
  CART_KEY: 'shopping_cart',
  USER_PREFERENCES_KEY: 'user_preferences',
  
  ROLES: {
    CUSTOMER: 'customer',
    VENDOR: 'vendor',
    ADMIN: 'admin'
  },
  
  ORDER_STATUS: {
    PENDING: 'pending',
    CONFIRMED: 'confirmed',
    PROCESSING: 'processing',
    SHIPPED: 'shipped',
    DELIVERED: 'delivered',
    CANCELLED: 'cancelled',
    REFUNDED: 'refunded'
  },
  
  PAYMENT_METHODS: {
    CARD: 'card',
    BANK_TRANSFER: 'bank_transfer',
    CASH_ON_DELIVERY: 'cash_on_delivery'
  },
  
  PRODUCT_STATUS: {
    ACTIVE: 'active',
    DRAFT: 'draft',
    ARCHIVED: 'archived'
  }
};

// Default pagination
const PAGINATION = {
  PAGE_SIZE: 20,
  MAX_PAGES: 10
};

// Error messages
const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Network error. Please check your connection and try again.',
  INVALID_CREDENTIALS: 'Invalid email or password.',
  USER_EXISTS: 'This email is already registered.',
  INVALID_EMAIL: 'Please enter a valid email address.',
  WEAK_PASSWORD: 'Password must be at least 8 characters with uppercase, lowercase, and numbers.',
  UNAUTHORIZED: 'You are not authorized to perform this action.',
  NOT_FOUND: 'The requested resource was not found.',
  SERVER_ERROR: 'Server error. Please try again later.',
  VALIDATION_ERROR: 'Please check your input and try again.',
  CART_ERROR: 'Error updating cart. Please try again.',
  PAYMENT_ERROR: 'Payment processing failed. Please try again.',
  ORDER_ERROR: 'Error creating order. Please try again.'
};

// Success messages
const SUCCESS_MESSAGES = {
  LOGIN_SUCCESS: 'Logged in successfully!',
  REGISTER_SUCCESS: 'Account created successfully! Please log in.',
  LOGOUT_SUCCESS: 'Logged out successfully.',
  PRODUCT_ADDED: 'Product added to cart!',
  PRODUCT_REMOVED: 'Product removed from cart.',
  ORDER_CREATED: 'Order created successfully!',
  PAYMENT_SUCCESS: 'Payment processed successfully!',
  PROFILE_UPDATED: 'Profile updated successfully!',
  PRODUCT_CREATED: 'Product created successfully!',
  PRODUCT_UPDATED: 'Product updated successfully!',
  PRODUCT_DELETED: 'Product deleted successfully!'
};

export { API_CONFIG, CONSTANTS, PAGINATION, ERROR_MESSAGES, SUCCESS_MESSAGES };
