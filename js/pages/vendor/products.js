import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class VendorProductsPage {
  constructor() {
    this.products = [];
    this.filteredProducts = [];
    this.filters = {
      status: '',
      category: '',
      search: ''
    };
    this.init();
  }

  async init() {
    const user = getFromLocalStorage('user');
    if (!user || user.role !== 'vendor') {
      window.location.href = '/pages/login.html';
      return;
    }

    await this.loadProducts();
    this.setupEventListeners();
    this.renderProducts();
  }

  async loadProducts() {
    try {
      const response = await apiCall('/api/vendor/products?limit=100');
      this.products = response.data || [];
      this.filteredProducts = [...this.products];
    } catch (error) {
      console.error('Error loading products:', error);
    }
  }

  setupEventListeners() {
    document.getElementById('status-filter').addEventListener('change', (e) => {
      this.filters.status = e.target.value;
      this.applyFilters();
    });

    document.getElementById('category-filter').addEventListener('change', (e) => {
      this.filters.category = e.target.value;
      this.applyFilters();
    });

    document.getElementById('search-products').addEventListener('input', (e) => {
      this.filters.search = e.target.value.toLowerCase();
      this.applyFilters();
    });
  }

  applyFilters() {
    let filtered = [...this.products];

    if (this.filters.status) {
      filtered = filtered.filter(p => {
        if (this.filters.status === 'active') return p.isActive && p.stock > 0;
        if (this.filters.status === 'inactive') return !p.isActive || p.stock === 0;
        if (this.filters.status === 'draft') return p.isDraft;
        return true;
      });
    }

    if (this.filters.category) {
      filtered = filtered.filter(p => p.category === this.filters.category);
    }

    if (this.filters.search) {
      filtered = filtered.filter(p =>
        p.name.toLowerCase().includes(this.filters.search) ||
        p.sku.toLowerCase().includes(this.filters.search)
      );
    }

    this.filteredProducts = filtered;
    this.renderProducts();
  }

  renderProducts() {
    const tbody = document.getElementById('products-table-body');

    if (this.filteredProducts.length === 0) {
      tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 40px; color: #999;">No products found</td></tr>';
      return;
    }

    tbody.innerHTML = this.filteredProducts.map(product => `
      <tr>
        <td>
          <div class="product-cell">
            <img src="${product.imageUrl}" alt="${sanitizeHtml(product.name)}">
            <div class="product-info">
              <h4>${sanitizeHtml(product.name)}</h4>
              <p>SKU: ${product.sku}</p>
            </div>
          </div>
        </td>
        <td>${formatCurrency(product.price)}</td>
        <td>${product.stock}</td>
        <td>${product.salesCount || 0}</td>
        <td>
          <span class="product-status ${product.isActive ? 'active' : 'inactive'}">
            ${product.isActive ? 'Active' : 'Inactive'}
          </span>
        </td>
        <td>
          <div class="action-buttons">
            <a href="/pages/vendor/edit-product.html?id=${product.id}" class="btn btn-outline btn-sm">Edit</a>
            <button class="btn btn-danger btn-sm" onclick="if(confirm('Delete this product?')) { location.href='#'; }">Delete</button>
          </div>
        </td>
      </tr>
    `).join('');
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new VendorProductsPage();
});
