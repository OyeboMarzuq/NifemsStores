import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class VendorOrdersPage {
  constructor() {
    this.orders = [];
    this.filteredOrders = [];
    this.filters = {
      status: '',
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

    await this.loadOrders();
    this.setupEventListeners();
    this.renderOrders();
  }

  async loadOrders() {
    try {
      const response = await apiCall('/api/vendor/orders?limit=100');
      this.orders = response.data || [];
      this.filteredOrders = [...this.orders];
    } catch (error) {
      console.error('Error loading orders:', error);
    }
  }

  setupEventListeners() {
    document.getElementById('status-filter').addEventListener('change', (e) => {
      this.filters.status = e.target.value;
      this.applyFilters();
    });

    document.getElementById('search-orders').addEventListener('input', (e) => {
      this.filters.search = e.target.value.toLowerCase();
      this.applyFilters();
    });
  }

  applyFilters() {
    let filtered = [...this.orders];

    if (this.filters.status) {
      filtered = filtered.filter(o => o.status === this.filters.status);
    }

    if (this.filters.search) {
      filtered = filtered.filter(o =>
        (o.orderNumber || o.id).toString().toLowerCase().includes(this.filters.search)
      );
    }

    this.filteredOrders = filtered;
    this.renderOrders();
  }

  renderOrders() {
    const container = document.getElementById('orders-container');

    if (this.filteredOrders.length === 0) {
      container.innerHTML = '<p style="text-align: center; color: #999; padding: 40px;">No orders found</p>';
      return;
    }

    container.innerHTML = this.filteredOrders.map(order => `
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
          <div>
            <span>Customer: ${sanitizeHtml(order.shippingAddress.firstName)} ${sanitizeHtml(order.shippingAddress.lastName)}</span>
            <br>
            <span style="font-size: 12px; color: #999;">Email: ${sanitizeHtml(order.shippingAddress.email)}</span>
          </div>
          <div style="text-align: right;">
            <div class="order-total">${formatCurrency(order.total)}</div>
            <select class="status-select" data-order-id="${order.id}" style="margin-top: 8px; padding: 4px 8px; border: 1px solid #ddd; border-radius: 4px;">
              <option value="pending" ${order.status === 'pending' ? 'selected' : ''}>Pending</option>
              <option value="processing" ${order.status === 'processing' ? 'selected' : ''}>Processing</option>
              <option value="shipped" ${order.status === 'shipped' ? 'selected' : ''}>Shipped</option>
              <option value="delivered" ${order.status === 'delivered' ? 'selected' : ''}>Delivered</option>
            </select>
          </div>
        </div>
      </div>
    `).join('');

    // Add status change handlers
    document.querySelectorAll('.status-select').forEach(select => {
      select.addEventListener('change', (e) => {
        this.updateOrderStatus(e.target.dataset.orderId, e.target.value);
      });
    });
  }

  async updateOrderStatus(orderId, status) {
    try {
      await apiCall(`/api/vendor/orders/${orderId}`, {
        method: 'PUT',
        body: JSON.stringify({ status })
      });

      // Reload orders
      await this.loadOrders();
      this.applyFilters();
      this.showSuccess('Order status updated');
    } catch (error) {
      console.error('Error updating order:', error);
      this.showError('Failed to update order status');
    }
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

  showSuccess(message) {
    const toast = document.createElement('div');
    toast.className = 'toast success';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
  }

  showError(message) {
    const toast = document.createElement('div');
    toast.className = 'toast error';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new VendorOrdersPage();
});
