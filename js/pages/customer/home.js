import apiClient from '../../utils/api.js';
import { API_CONFIG, CONSTANTS, PAGINATION } from '../../config.js';
import { requireAuth } from '../../utils/auth.js';
import { getCart, addToCart, getCartCount } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { initHeader } from '../../components/header.js';
import { initFooter } from '../../components/footer.js';

// Require authentication
requireAuth();

class HomePage {
  constructor() {
    this.currentPage = 1;
    this.pageSize = PAGINATION.PAGE_SIZE;
    this.products = [];
    this.totalProducts = 0;
    this.filters = {
      search: '',
      categories: [],
      priceMin: 0,
      priceMax: Infinity,
      ratings: [],
      availability: []
    };
    this.sortBy = 'newest';
    this.viewMode = 'grid';
  }

  async init() {
    console.log('[v0] Initializing home page');
    
    // Initialize header and footer
    initHeader();
    initFooter();

    this.setupEventListeners();
    this.setupFilters();
    await this.loadProducts();
  }

  setupEventListeners() {
    // Filter toggle
    document.getElementById('filter-toggle').addEventListener('click', () => {
      const sidebar = document.getElementById('filters-sidebar');
      sidebar.classList.toggle('visible');
    });

    // Close filters on outside click
    document.addEventListener('click', (e) => {
      const sidebar = document.getElementById('filters-sidebar');
      const toggle = document.getElementById('filter-toggle');
      if (!sidebar.contains(e.target) && !toggle.contains(e.target)) {
        sidebar.classList.remove('visible');
      }
    });

    // View toggle
    document.querySelectorAll('.view-toggle-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        document.querySelectorAll('.view-toggle-btn').forEach(b => b.classList.remove('active'));
        e.target.classList.add('active');
        this.viewMode = e.target.dataset.view;
        this.updateProductsView();
      });
    });

    // Sort select
    document.getElementById('sort-select').addEventListener('change', (e) => {
      this.sortBy = e.target.value;
      this.currentPage = 1;
      this.loadProducts();
    });

    // Search input
    document.getElementById('product-search').addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        this.filters.search = e.target.value.trim();
        this.currentPage = 1;
        this.loadProducts();
      }
    });

    // Reset filters button
    document.getElementById('reset-filters-btn').addEventListener('click', () => {
      this.resetFilters();
    });
  }

  setupFilters() {
    // Category filters
    document.querySelectorAll('input[name="category"]').forEach(checkbox => {
      checkbox.addEventListener('change', (e) => {
        if (e.target.checked) {
          this.filters.categories.push(e.target.value);
        } else {
          this.filters.categories = this.filters.categories.filter(c => c !== e.target.value);
        }
        this.currentPage = 1;
        this.loadProducts();
      });
    });

    // Price range filters
    document.getElementById('price-min').addEventListener('change', (e) => {
      this.filters.priceMin = parseInt(e.target.value) || 0;
      this.currentPage = 1;
      this.loadProducts();
    });

    document.getElementById('price-max').addEventListener('change', (e) => {
      this.filters.priceMax = parseInt(e.target.value) || Infinity;
      this.currentPage = 1;
      this.loadProducts();
    });

    // Rating filters
    document.querySelectorAll('input[name="rating"]').forEach(checkbox => {
      checkbox.addEventListener('change', (e) => {
        if (e.target.checked) {
          this.filters.ratings.push(parseInt(e.target.value));
        } else {
          this.filters.ratings = this.filters.ratings.filter(r => r !== parseInt(e.target.value));
        }
        this.currentPage = 1;
        this.loadProducts();
      });
    });

    // Availability filters
    document.querySelectorAll('input[name="availability"]').forEach(checkbox => {
      checkbox.addEventListener('change', (e) => {
        if (e.target.checked) {
          this.filters.availability.push(e.target.value);
        } else {
          this.filters.availability = this.filters.availability.filter(a => a !== e.target.value);
        }
        this.currentPage = 1;
        this.loadProducts();
      });
    });
  }

  async loadProducts() {
    console.log('[v0] Loading products with filters:', this.filters);
    
    try {
      // Show loading state
      const container = document.getElementById('products-container');
      container.innerHTML = `
        <div class="skeleton" style="aspect-ratio: 1; border-radius: var(--radius-lg);"></div>
        <div class="skeleton" style="aspect-ratio: 1; border-radius: var(--radius-lg);"></div>
        <div class="skeleton" style="aspect-ratio: 1; border-radius: var(--radius-lg);"></div>
        <div class="skeleton" style="aspect-ratio: 1; border-radius: var(--radius-lg);"></div>
      `;

      // Build query params
      const params = new URLSearchParams();
      params.append('page', this.currentPage);
      params.append('pageSize', this.pageSize);
      params.append('sortBy', this.sortBy);
      
      if (this.filters.search) params.append('search', this.filters.search);
      if (this.filters.categories.length) params.append('categories', this.filters.categories.join(','));
      if (this.filters.priceMin > 0) params.append('priceMin', this.filters.priceMin);
      if (this.filters.priceMax !== Infinity) params.append('priceMax', this.filters.priceMax);

      // Make API call
      const response = await apiClient.get(`${API_CONFIG.ENDPOINTS.GET_PRODUCTS}?${params.toString()}`);

      if (response.success) {
        this.products = response.data.items || [];
        this.totalProducts = response.data.total || 0;

        if (this.products.length === 0) {
          this.showEmptyState();
        } else {
          this.renderProducts();
          this.renderPagination();
          document.getElementById('empty-state').classList.add('hidden');
        }
      } else {
        this.showError('Failed to load products');
      }
    } catch (error) {
      console.error('[v0] Error loading products:', error);
      this.showError('Error loading products. Please try again.');
    }
  }

  renderProducts() {
    const container = document.getElementById('products-container');
    
    if (this.products.length === 0) {
      container.innerHTML = '<p>No products found</p>';
      return;
    }

    container.innerHTML = this.products.map(product => this.createProductCard(product)).join('');

    // Add event listeners
    this.setupProductCardListeners();
  }

  createProductCard(product) {
    const discount = product.originalPrice ? 
      Math.round(((product.originalPrice - product.price) / product.originalPrice) * 100) : 0;
    
    const cartCount = getCartCount();
    const isInCart = getCart().some(item => item.productId === product.id);

    return `
      <div class="product-card" data-product-id="${product.id}">
        <div class="product-image-container">
          <img src="${product.imageUrl || 'https://via.placeholder.com/200'}" alt="${product.name}" class="product-image">
          ${discount > 0 ? `<span class="product-badge sale">-${discount}%</span>` : ''}
          ${product.isNew ? '<span class="product-badge new">New</span>' : ''}
        </div>
        
        <div class="product-content">
          <div class="product-category">${product.category || 'Uncategorized'}</div>
          <h3 class="product-name">${product.name}</h3>
          
          <div class="product-rating">
            <span class="product-stars">${'★'.repeat(Math.floor(product.rating || 0))}${'☆'.repeat(5 - Math.floor(product.rating || 0))}</span>
            <span class="product-reviews-count">(${product.reviewCount || 0})</span>
          </div>
          
          <div class="product-price">
            <span class="product-price-current">${formatCurrency(product.price)}</span>
            ${product.originalPrice ? `<span class="product-price-original">${formatCurrency(product.originalPrice)}</span>` : ''}
          </div>
          
          <div class="product-action">
            <button class="product-btn primary add-to-cart-btn" data-product-id="${product.id}">
              Add to Cart
            </button>
            <button class="product-btn wishlist wishlist-btn" data-product-id="${product.id}" title="Add to wishlist">
              ♡
            </button>
          </div>
        </div>
      </div>
    `;
  }

  setupProductCardListeners() {
    // Add to cart buttons
    document.querySelectorAll('.add-to-cart-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        const productId = parseInt(btn.dataset.productId);
        const product = this.products.find(p => p.id === productId);
        
        if (product) {
          addToCart({
            productId: product.id,
            name: product.name,
            price: product.price,
            imageUrl: product.imageUrl,
            vendorId: product.vendorId
          });
          
          // Show feedback
          btn.textContent = '✓ Added';
          btn.disabled = true;
          setTimeout(() => {
            btn.textContent = 'Add to Cart';
            btn.disabled = false;
          }, 2000);

          // Update header cart count
          initHeader();
        }
      });
    });

    // Wishlist buttons
    document.querySelectorAll('.wishlist-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        btn.classList.toggle('active');
        btn.textContent = btn.classList.contains('active') ? '♥' : '♡';
      });
    });

    // Product card click to view details
    document.querySelectorAll('.product-card').forEach(card => {
      card.addEventListener('click', (e) => {
        if (!e.target.closest('.product-btn')) {
          const productId = card.dataset.productId;
          window.location.href = `/pages/customer/product-detail.html?id=${productId}`;
        }
      });
    });
  }

  renderPagination() {
    const totalPages = Math.ceil(this.totalProducts / this.pageSize);
    const container = document.getElementById('pagination-container');

    if (totalPages <= 1) {
      container.innerHTML = '';
      return;
    }

    let html = '';

    // Previous button
    if (this.currentPage > 1) {
      html += `<button class="pagination-item" data-page="${this.currentPage - 1}">← Previous</button>`;
    }

    // Page numbers
    const startPage = Math.max(1, this.currentPage - 2);
    const endPage = Math.min(totalPages, this.currentPage + 2);

    if (startPage > 1) {
      html += `<button class="pagination-item" data-page="1">1</button>`;
      if (startPage > 2) html += `<span class="pagination-item disabled">...</span>`;
    }

    for (let i = startPage; i <= endPage; i++) {
      html += `
        <button class="pagination-item ${i === this.currentPage ? 'active' : ''}" data-page="${i}">
          ${i}
        </button>
      `;
    }

    if (endPage < totalPages) {
      if (endPage < totalPages - 1) html += `<span class="pagination-item disabled">...</span>`;
      html += `<button class="pagination-item" data-page="${totalPages}">${totalPages}</button>`;
    }

    // Next button
    if (this.currentPage < totalPages) {
      html += `<button class="pagination-item" data-page="${this.currentPage + 1}">Next →</button>`;
    }

    container.innerHTML = html;

    // Add event listeners
    document.querySelectorAll('.pagination-item[data-page]').forEach(btn => {
      btn.addEventListener('click', (e) => {
        this.currentPage = parseInt(e.target.dataset.page);
        this.loadProducts();
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
    });
  }

  updateProductsView() {
    const container = document.getElementById('products-container');
    
    if (this.viewMode === 'list') {
      container.style.gridTemplateColumns = '1fr';
    } else {
      container.style.gridTemplateColumns = 'repeat(auto-fill, minmax(200px, 1fr))';
    }
  }

  showEmptyState() {
    document.getElementById('products-container').innerHTML = '';
    document.getElementById('empty-state').classList.remove('hidden');
    document.getElementById('pagination-container').innerHTML = '';
  }

  showError(message) {
    const container = document.getElementById('products-container');
    container.innerHTML = `
      <div class="alert alert-error" style="grid-column: 1 / -1;">
        <div class="alert-icon">⚠️</div>
        <div class="alert-content">${message}</div>
      </div>
    `;
  }

  resetFilters() {
    this.filters = {
      search: '',
      categories: [],
      priceMin: 0,
      priceMax: Infinity,
      ratings: [],
      availability: []
    };
    this.currentPage = 1;
    this.sortBy = 'newest';

    // Reset form inputs
    document.getElementById('filter-search').value = '';
    document.getElementById('price-min').value = '';
    document.getElementById('price-max').value = '';
    document.getElementById('product-search').value = '';
    document.getElementById('sort-select').value = 'newest';

    document.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
      checkbox.checked = false;
    });

    this.loadProducts();
  }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
  const homePage = new HomePage();
  homePage.init();
});
