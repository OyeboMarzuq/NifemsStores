import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage, getFromLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';
import { redirectIfNotAuthenticated } from '../../utils/auth.js';

class VendorDashboard {
  constructor() {
    this.vendorData = null;
    this.stats = {};
    this.recentOrders = [];
    this.topProducts = [];
    this.init();
  }

  async init() {
    // Check authentication and vendor role
    const user = getFromLocalStorage('user');
    if (!user || user.role !== 'vendor') {
      window.location.href = '/pages/login.html';
      return;
    }

    await this.loadDashboardData();
    this.renderDashboard();
  }

  async loadDashboardData() {
    try {
      const [statsRes, ordersRes, productsRes] = await Promise.all([
        apiCall('/api/vendor/stats'),
        apiCall('/api/vendor/orders?limit=5'),
        apiCall('/api/vendor/products?limit=5&sort=sales')
      ]);

      this.stats = statsRes.data || {};
      this.recentOrders = ordersRes.data || [];
      this.topProducts = productsRes.data || [];
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    }
  }

  renderDashboard() {
    const user = getFromLocalStorage('user');
    
    // Update vendor info
    document.getElementById('vendor-name').textContent = user.storeName || 'My Store';
    document.getElementById('vendor-email').textContent = user.email;

    // Update stats
    document.getElementById('total-sales').textContent = formatCurrency(this.stats.totalSales || 0);
    document.getElementById('total-orders').textContent = this.stats.totalOrders || 0;
    document.getElementById('total-products').textContent = this.stats.totalProducts || 0;
    document.getElementById('avg-rating').textContent = (this.stats.averageRating || 0).toFixed(1);

    // Update badges
    document.getElementById('product-count').textContent = this.stats.totalProducts || 0;
    document.getElementById('order-count').textContent = this.stats.pendingOrders || 0;

    // Render recent orders
    this.renderRecentOrders();

    // Render top products
    this.renderTopProducts();

    // Setup event listeners
    this.setupEventListeners();
  }

  renderRecentOrders() {
    const container = document.getElementById('recent-orders');
    
    if (this.recentOrders.length === 0) {
      container.innerHTML = '<p style="text-align: center; color: #999;">No recent orders</p>';
      return;
    }

    container.innerHTML = this.recentOrders.map(order => `
      <div class="order-card">
        <div class="order-header">
          <div class="order-number">
            <strong>Order #${order.orderNumber}</strong>
            <span class="order-date">${new Date(order.createdAt).toLocaleDateString()}</span>
          </div>
          <div class="order-status">
            <span class="status-badge status-${order.status}">${this.formatStatus(order.status)}</span>
          </div>
        </div>

        <div class="order-items">
          ${order.items.map(item => `
            <div class="order-item">
              <span class="order-item-name">${sanitizeHtml(item.name)}</span>
              <span class="order-item-qty">x${item.quantity}</span>
              <span class="order-item-price">${formatCurrency(item.price * item.quantity)}</span>
            </div>
          `).join('')}
        </div>

        <div class="order-footer">
          <span>Customer: ${sanitizeHtml(order.shippingAddress.firstName)}</span>
          <span class="order-total">${formatCurrency(order.total)}</span>
          <a href="/pages/vendor/order-details.html?orderId=${order.id}" class="btn btn-outline btn-sm">
            Details
          </a>
        </div>
      </div>
    `).join('');
  }

  renderTopProducts() {
    const tbody = document.getElementById('top-products');
    
    if (this.topProducts.length === 0) {
      tbody.innerHTML = '<tr><td colspan="5" style="text-align: center; color: #999;">No products yet</td></tr>';
      return;
    }

    tbody.innerHTML = this.topProducts.map(product => `
      <tr>
        <td>
          <div class="product-cell">
            <img src="${product.imageUrl}" alt="${sanitizeHtml(product.name)}">
            <div class="product-info">
              <h4>${sanitizeHtml(product.name)}</h4>
              <p>${formatCurrency(product.price)}</p>
            </div>
          </div>
        </td>
        <td>${product.salesCount || 0}</td>
        <td>${formatCurrency(product.totalRevenue || 0)}</td>
        <td>${product.stock}</td>
        <td>
          <span class="product-status ${product.isActive ? 'active' : 'inactive'}">
            ${product.isActive ? 'Active' : 'Inactive'}
          </span>
        </td>
      </tr>
    `).join('');
  }

  formatStatus(status) {
    const statusMap = {
      'pending': 'Pending',
      'processing': 'Processing',
      'shipped': 'Shipped',
      'delivered': 'Delivered',
      'cancelled': 'Cancelled'
    };
    return statusMap[status] || status;
  }

  setupEventListeners() {
    document.querySelector('.btn-primary').addEventListener('click', () => {
      window.location.href = '/pages/vendor/add-product.html';
    });

    document.getElementById('logout-btn').addEventListener('click', (e) => {
      e.preventDefault();
      localStorage.clear();
      window.location.href = '/pages/login.html';
    });
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new VendorDashboard();
});
