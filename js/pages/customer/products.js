import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage, saveToLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency, truncateText } from '../../utils/formatter.js';
import { validateEmail, sanitizeHtml } from '../../utils/validation.js';

class ProductsPage {
  constructor() {
    this.currentPage = 1;
    this.itemsPerPage = 12;
    this.filters = {
      category: '',
      priceMin: 0,
      priceMax: 1000000,
      rating: 0,
      vendor: '',
      search: ''
    };
    this.products = [];
    this.allProducts = [];
    this.init();
  }

  async init() {
    this.setupEventListeners();
    await this.loadProducts();
    this.renderProducts();
    this.setupPagination();
  }

  setupEventListeners() {
    // Search functionality
    const searchInput = document.getElementById('product-search');
    if (searchInput) {
      searchInput.addEventListener('input', (e) => {
        this.filters.search = e.target.value.toLowerCase();
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Category filter
    const categorySelect = document.getElementById('category-filter');
    if (categorySelect) {
      categorySelect.addEventListener('change', (e) => {
        this.filters.category = e.target.value;
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Price range filter
    const priceMinInput = document.getElementById('price-min');
    const priceMaxInput = document.getElementById('price-max');
    if (priceMinInput && priceMaxInput) {
      priceMinInput.addEventListener('change', (e) => {
        this.filters.priceMin = parseFloat(e.target.value) || 0;
        this.currentPage = 1;
        this.applyFilters();
      });
      priceMaxInput.addEventListener('change', (e) => {
        this.filters.priceMax = parseFloat(e.target.value) || 1000000;
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Rating filter
    const ratingFilter = document.getElementById('rating-filter');
    if (ratingFilter) {
      ratingFilter.addEventListener('change', (e) => {
        this.filters.rating = parseFloat(e.target.value) || 0;
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Vendor filter
    const vendorSelect = document.getElementById('vendor-filter');
    if (vendorSelect) {
      vendorSelect.addEventListener('change', (e) => {
        this.filters.vendor = e.target.value;
        this.currentPage = 1;
        this.applyFilters();
      });
    }

    // Sort functionality
    const sortSelect = document.getElementById('sort-by');
    if (sortSelect) {
      sortSelect.addEventListener('change', (e) => {
        this.sortProducts(e.target.value);
      });
    }

    // Clear filters button
    const clearFiltersBtn = document.getElementById('clear-filters');
    if (clearFiltersBtn) {
      clearFiltersBtn.addEventListener('click', () => {
        this.clearFilters();
      });
    }
  }

  async loadProducts() {
    try {
      const response = await apiCall('/api/products?limit=100');
      this.allProducts = response.data || [];
      this.products = [...this.allProducts];
    } catch (error) {
      console.error('Error loading products:', error);
      this.showErrorMessage('Failed to load products. Please try again.');
    }
  }

  applyFilters() {
    let filtered = [...this.allProducts];

    // Search filter
    if (this.filters.search) {
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(this.filters.search) ||
        product.description.toLowerCase().includes(this.filters.search)
      );
    }

    // Category filter
    if (this.filters.category) {
      filtered = filtered.filter(product => product.category === this.filters.category);
    }

    // Price filter
    filtered = filtered.filter(product =>
      product.price >= this.filters.priceMin &&
      product.price <= this.filters.priceMax
    );

    // Rating filter
    if (this.filters.rating > 0) {
      filtered = filtered.filter(product => product.rating >= this.filters.rating);
    }

    // Vendor filter
    if (this.filters.vendor) {
      filtered = filtered.filter(product => product.vendorId === this.filters.vendor);
    }

    this.products = filtered;
    this.currentPage = 1;
    this.renderProducts();
  }

  sortProducts(sortBy) {
    switch (sortBy) {
      case 'price-low':
        this.products.sort((a, b) => a.price - b.price);
        break;
      case 'price-high':
        this.products.sort((a, b) => b.price - a.price);
        break;
      case 'rating':
        this.products.sort((a, b) => b.rating - a.rating);
        break;
      case 'newest':
        this.products.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        break;
      case 'popular':
        this.products.sort((a, b) => b.salesCount - a.salesCount);
        break;
      default:
        break;
    }
    this.currentPage = 1;
    this.renderProducts();
  }

  clearFilters() {
    this.filters = {
      category: '',
      priceMin: 0,
      priceMax: 1000000,
      rating: 0,
      vendor: '',
      search: ''
    };
    this.products = [...this.allProducts];
    this.currentPage = 1;

    // Reset form inputs
    document.getElementById('product-search').value = '';
    document.getElementById('category-filter').value = '';
    document.getElementById('price-min').value = '';
    document.getElementById('price-max').value = '';
    document.getElementById('rating-filter').value = '';
    document.getElementById('vendor-filter').value = '';
    document.getElementById('sort-by').value = 'newest';

    this.renderProducts();
  }

  renderProducts() {
    const productsContainer = document.getElementById('products-grid');
    if (!productsContainer) return;

    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    const paginatedProducts = this.products.slice(start, end);

    if (paginatedProducts.length === 0) {
      productsContainer.innerHTML = `
        <div class="no-products" style="grid-column: 1 / -1; text-align: center; padding: 40px;">
          <p style="font-size: 18px; color: #666;">No products found matching your criteria.</p>
        </div>
      `;
      return;
    }

    productsContainer.innerHTML = paginatedProducts.map(product => `
      <div class="product-card">
        <div class="product-image-container">
          <img src="${product.imageUrl || '/images/placeholder.jpg'}" alt="${sanitizeHtml(product.name)}" class="product-image">
          ${product.discount > 0 ? `<span class="discount-badge">-${product.discount}%</span>` : ''}
          ${product.isNew ? `<span class="new-badge">New</span>` : ''}
        </div>
        <div class="product-info">
          <div class="product-vendor" style="font-size: 12px; color: #999; margin-bottom: 5px;">
            ${sanitizeHtml(product.vendorName || 'Store')}
          </div>
          <h3 class="product-name">${sanitizeHtml(product.name)}</h3>
          <p class="product-description">${sanitizeHtml(truncateText(product.description, 60))}</p>
          <div class="product-rating">
            <div class="stars">
              ${this.renderStars(product.rating)}
            </div>
            <span class="rating-count">${product.reviewCount || 0}</span>
          </div>
          <div class="product-price-section">
            <div class="product-price">
              <span class="price">${formatCurrency(product.price)}</span>
              ${product.originalPrice ? `<span class="original-price">${formatCurrency(product.originalPrice)}</span>` : ''}
            </div>
          </div>
          <div class="product-actions">
            <button class="btn btn-outline btn-sm add-to-cart-btn" data-product-id="${product.id}">
              Add to Cart
            </button>
            <button class="btn btn-icon btn-sm wishlist-btn" data-product-id="${product.id}">
              ♡
            </button>
          </div>
        </div>
      </div>
    `).join('');

    // Add event listeners to add to cart buttons
    document.querySelectorAll('.add-to-cart-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const productId = e.target.dataset.productId;
        this.addToCart(productId);
      });
    });

    // Add event listeners to wishlist buttons
    document.querySelectorAll('.wishlist-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const productId = e.target.dataset.productId;
        this.toggleWishlist(productId, e.target);
      });
    });

    // Add click handlers to product cards for navigation
    document.querySelectorAll('.product-card').forEach(card => {
      card.addEventListener('click', (e) => {
        if (e.target.tagName !== 'BUTTON') {
          const productId = card.querySelector('.add-to-cart-btn').dataset.productId;
          window.location.href = `/pages/customer/product-detail.html?id=${productId}`;
        }
      });
      card.style.cursor = 'pointer';
    });
  }

  renderStars(rating) {
    let starsHtml = '';
    for (let i = 1; i <= 5; i++) {
      if (i <= rating) {
        starsHtml += '<span class="star filled">★</span>';
      } else if (i - rating < 1) {
        starsHtml += '<span class="star half">★</span>';
      } else {
        starsHtml += '<span class="star empty">★</span>';
      }
    }
    return starsHtml;
  }

  addToCart(productId) {
    const product = this.allProducts.find(p => p.id === productId);
    if (!product) return;

    const cart = getFromLocalStorage('cart') || [];
    const existingItem = cart.find(item => item.productId === productId);

    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      cart.push({
        productId: productId,
        name: product.name,
        price: product.price,
        image: product.imageUrl,
        vendor: product.vendorName,
        quantity: 1
      });
    }

    saveToLocalStorage('cart', cart);
    this.showSuccessMessage(`${product.name} added to cart!`);
    this.updateCartCount();
  }

  toggleWishlist(productId, button) {
    const wishlist = getFromLocalStorage('wishlist') || [];
    const index = wishlist.indexOf(productId);

    if (index > -1) {
      wishlist.splice(index, 1);
      button.textContent = '♡';
      button.style.color = '#ccc';
    } else {
      wishlist.push(productId);
      button.textContent = '♥';
      button.style.color = '#E63946';
    }

    saveToLocalStorage('wishlist', wishlist);
  }

  updateCartCount() {
    const cart = getFromLocalStorage('cart') || [];
    const cartCount = document.getElementById('cart-count');
    if (cartCount) {
      const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
      cartCount.textContent = totalItems;
    }
  }

  setupPagination() {
    const totalPages = Math.ceil(this.products.length / this.itemsPerPage);
    const paginationContainer = document.getElementById('pagination');

    if (!paginationContainer) return;

    let paginationHtml = '';

    if (this.currentPage > 1) {
      paginationHtml += `<button class="pagination-btn" data-page="1">First</button>`;
      paginationHtml += `<button class="pagination-btn" data-page="${this.currentPage - 1}">Previous</button>`;
    }

    for (let i = Math.max(1, this.currentPage - 2); i <= Math.min(totalPages, this.currentPage + 2); i++) {
      paginationHtml += `<button class="pagination-btn ${i === this.currentPage ? 'active' : ''}" data-page="${i}">${i}</button>`;
    }

    if (this.currentPage < totalPages) {
      paginationHtml += `<button class="pagination-btn" data-page="${this.currentPage + 1}">Next</button>`;
      paginationHtml += `<button class="pagination-btn" data-page="${totalPages}">Last</button>`;
    }

    paginationContainer.innerHTML = paginationHtml;

    document.querySelectorAll('.pagination-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        this.currentPage = parseInt(e.target.dataset.page);
        this.renderProducts();
        this.setupPagination();
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
    });
  }

  showSuccessMessage(message) {
    const toast = document.createElement('div');
    toast.className = 'toast success';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
  }

  showErrorMessage(message) {
    const alert = document.createElement('div');
    alert.className = 'alert alert-danger';
    alert.textContent = message;
    const container = document.querySelector('.products-container') || document.body;
    container.insertBefore(alert, container.firstChild);
  }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
  new ProductsPage();
});
