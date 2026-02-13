import { getToken, getUser, clearAuth, getUserRole, CONSTANTS } from '../utils/auth.js';
import { getCartCount } from '../utils/localStorage.js';

export class Header {
  constructor() {
    this.headerElement = document.getElementById('header');
    this.mobileMenuButton = null;
    this.userMenuButton = null;
    this.mobileMenu = null;
    this.userMenu = null;
  }

  render() {
    const isAuthenticated = !!getToken();
    const user = getUser();
    const userRole = getUserRole();
    const cartCount = getCartCount();

    const html = `
      <header id="app-header" class="app-header">
        <div class="header-container">
          <!-- Logo -->
          <div class="header-logo">
            <a href="/pages/customer/home.html" class="logo-link">
              <div class="logo-icon">🛍️</div>
              <span class="logo-text">NifemsStore</span>
            </a>
          </div>

          <!-- Search Bar (Customer only) -->
          ${isAuthenticated && userRole === CONSTANTS.ROLES.CUSTOMER ? `
            <div class="header-search">
              <input 
                type="text" 
                id="search-input" 
                placeholder="Search products..." 
                class="search-input"
              >
              <button class="search-btn" aria-label="Search">🔍</button>
            </div>
          ` : ''}

          <!-- Right Section -->
          <div class="header-right">
            ${isAuthenticated ? `
              <!-- Cart Icon (Customer only) -->
              ${userRole === CONSTANTS.ROLES.CUSTOMER ? `
                <a href="/pages/customer/cart.html" class="header-icon-link" title="Shopping Cart">
                  🛒
                  ${cartCount > 0 ? `<span class="cart-badge">${cartCount}</span>` : ''}
                </a>
              ` : ''}

              <!-- User Menu -->
              <div class="header-user-menu">
                <button id="user-menu-btn" class="header-user-btn" aria-label="User menu">
                  👤
                  <span class="user-name">${user?.firstName || 'User'}</span>
                </button>
                <div id="user-dropdown" class="header-dropdown hidden">
                  <a href="#profile" class="dropdown-item">👤 My Profile</a>
                  ${userRole === CONSTANTS.ROLES.VENDOR ? `
                    <a href="/pages/vendor/dashboard.html" class="dropdown-item">📊 Dashboard</a>
                    <a href="/pages/vendor/products.html" class="dropdown-item">📦 My Products</a>
                  ` : ''}
                  ${userRole === CONSTANTS.ROLES.ADMIN ? `
                    <a href="/pages/admin/dashboard.html" class="dropdown-item">⚙️ Admin Panel</a>
                  ` : ''}
                  <hr class="dropdown-divider">
                  <button id="logout-btn" class="dropdown-item dropdown-item-danger">🚪 Logout</button>
                </div>
              </div>
            ` : `
              <!-- Auth Links -->
              <a href="/pages/login.html" class="btn btn-secondary btn-sm">Sign In</a>
              <a href="/pages/register.html" class="btn btn-primary btn-sm">Register</a>
            `}

            <!-- Mobile Menu Toggle -->
            <button id="mobile-menu-btn" class="mobile-menu-btn" aria-label="Toggle menu">
              ☰
            </button>
          </div>
        </div>

        <!-- Mobile Menu -->
        <div id="mobile-menu" class="mobile-menu hidden">
          <nav class="mobile-nav">
            ${isAuthenticated ? `
              <a href="/pages/customer/home.html" class="mobile-nav-item">🏠 Home</a>
              ${userRole === CONSTANTS.ROLES.CUSTOMER ? `
                <a href="/pages/customer/orders.html" class="mobile-nav-item">📋 My Orders</a>
                <a href="/pages/customer/cart.html" class="mobile-nav-item">🛒 Cart</a>
              ` : ''}
              <hr>
              <button id="mobile-logout-btn" class="mobile-nav-item mobile-nav-danger">🚪 Logout</button>
            ` : `
              <a href="/pages/login.html" class="mobile-nav-item">Sign In</a>
              <a href="/pages/register.html" class="mobile-nav-item">Register</a>
            `}
          </nav>
        </div>
      </header>
    `;

    const headerContainer = document.createElement('div');
    headerContainer.innerHTML = html;
    
    if (this.headerElement) {
      this.headerElement.replaceWith(headerContainer);
    }

    this.setupEventListeners();
  }

  setupEventListeners() {
    // Mobile menu toggle
    this.mobileMenuButton = document.getElementById('mobile-menu-btn');
    this.mobileMenu = document.getElementById('mobile-menu');
    
    if (this.mobileMenuButton) {
      this.mobileMenuButton.addEventListener('click', () => {
        this.mobileMenu?.classList.toggle('hidden');
      });
    }

    // User menu dropdown
    this.userMenuButton = document.getElementById('user-menu-btn');
    this.userMenu = document.getElementById('user-dropdown');
    
    if (this.userMenuButton) {
      this.userMenuButton.addEventListener('click', () => {
        this.userMenu?.classList.toggle('hidden');
      });

      // Close menu when clicking outside
      document.addEventListener('click', (e) => {
        if (!e.target.closest('.header-user-menu')) {
          this.userMenu?.classList.add('hidden');
        }
      });
    }

    // Logout buttons
    const logoutBtn = document.getElementById('logout-btn');
    const mobileLogoutBtn = document.getElementById('mobile-logout-btn');

    [logoutBtn, mobileLogoutBtn].forEach(btn => {
      if (btn) {
        btn.addEventListener('click', () => this.logout());
      }
    });

    // Search functionality
    const searchInput = document.getElementById('search-input');
    const searchBtn = document.querySelector('.search-btn');

    if (searchBtn) {
      searchBtn.addEventListener('click', () => {
        this.handleSearch();
      });
    }

    if (searchInput) {
      searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
          this.handleSearch();
        }
      });
    }
  }

  handleSearch() {
    const searchInput = document.getElementById('search-input');
    const query = searchInput?.value.trim();

    if (!query) return;

    // Redirect to products page with search query
    window.location.href = `/pages/customer/home.html?search=${encodeURIComponent(query)}`;
  }

  logout() {
    clearAuth();
    window.location.href = '/pages/login.html';
  }
}

// Initialize header on page load
export function initHeader() {
  const header = new Header();
  header.render();
}
