import { CONSTANTS } from '../config.js';

/**
 * Get cart from localStorage
 */
export function getCart() {
  const cartJson = localStorage.getItem(CONSTANTS.CART_KEY);
  return cartJson ? JSON.parse(cartJson) : [];
}

/**
 * Set cart in localStorage
 */
export function setCart(cart) {
  localStorage.setItem(CONSTANTS.CART_KEY, JSON.stringify(cart));
}

/**
 * Add item to cart
 */
export function addToCart(item) {
  const cart = getCart();
  const existingItem = cart.find(
    c => c.productId === item.productId && c.vendorId === item.vendorId
  );

  if (existingItem) {
    existingItem.quantity += item.quantity || 1;
  } else {
    cart.push({
      ...item,
      quantity: item.quantity || 1,
      addedAt: new Date().toISOString()
    });
  }

  setCart(cart);
  return cart;
}

/**
 * Remove item from cart
 */
export function removeFromCart(productId, vendorId) {
  let cart = getCart();
  cart = cart.filter(item => !(item.productId === productId && item.vendorId === vendorId));
  setCart(cart);
  return cart;
}

/**
 * Update item quantity
 */
export function updateCartQuantity(productId, vendorId, quantity) {
  const cart = getCart();
  const item = cart.find(c => c.productId === productId && c.vendorId === vendorId);

  if (item) {
    if (quantity <= 0) {
      return removeFromCart(productId, vendorId);
    }
    item.quantity = quantity;
    setCart(cart);
  }

  return cart;
}

/**
 * Clear cart
 */
export function clearCart() {
  localStorage.removeItem(CONSTANTS.CART_KEY);
  return [];
}

/**
 * Get cart count
 */
export function getCartCount() {
  const cart = getCart();
  return cart.reduce((total, item) => total + item.quantity, 0);
}

/**
 * Get cart total
 */
export function getCartTotal() {
  const cart = getCart();
  return cart.reduce((total, item) => total + (item.price * item.quantity), 0);
}

/**
 * Get user preferences
 */
export function getPreferences() {
  const prefsJson = localStorage.getItem(CONSTANTS.USER_PREFERENCES_KEY);
  return prefsJson
    ? JSON.parse(prefsJson)
    : {
        theme: 'light',
        language: 'en',
        notifications: true
      };
}

/**
 * Set user preferences
 */
export function setPreferences(preferences) {
  const current = getPreferences();
  const updated = { ...current, ...preferences };
  localStorage.setItem(CONSTANTS.USER_PREFERENCES_KEY, JSON.stringify(updated));
  return updated;
}

/**
 * Store recent searches
 */
export function addRecentSearch(query) {
  const searches = JSON.parse(localStorage.getItem('recent_searches') || '[]');
  const filtered = searches.filter(s => s !== query);
  filtered.unshift(query);
  localStorage.setItem('recent_searches', JSON.stringify(filtered.slice(0, 10)));
}

/**
 * Get recent searches
 */
export function getRecentSearches() {
  return JSON.parse(localStorage.getItem('recent_searches') || '[]');
}

/**
 * Store temporary form data
 */
export function saveDraft(key, data) {
  localStorage.setItem(`draft_${key}`, JSON.stringify(data));
}

/**
 * Get temporary form data
 */
export function getDraft(key) {
  const draftJson = localStorage.getItem(`draft_${key}`);
  return draftJson ? JSON.parse(draftJson) : null;
}

/**
 * Clear temporary form data
 */
export function clearDraft(key) {
  localStorage.removeItem(`draft_${key}`);
}
