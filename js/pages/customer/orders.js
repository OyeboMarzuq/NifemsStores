import { apiCall } from '../../utils/api.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class OrdersPage {
  constructor() {
    this.orders = [];
    this.filteredOrders = [];
    this.filters = {
      status: '',
      dateRange: '',
      search: ''
    };
    this.init();
  }

  async init() {
    await this.loadOrders();
    this.setupEventListeners();
    this.renderOrders();
  }

  async loadOrders() {
    try {
      const response = await apiCall('/api/orders');
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

    document.getElementById('date-filter').addEventListener('change', (e) => {
      this.filters.dateRange = e.target.value;
      this.applyFilters();
    });

    document.getElementById('search-orders').addEventListener('input', (e) => {
      this.filters.search = e.target.value.toLowerCase();
      this.applyFilters();
    });
  }

  applyFilters() {
    let filtered = [...this.orders];

    // Status filter
    if (this.filters.status) {
      filtered = filtered.filter(order => order.status === this.filters.status);
    }

    // Date filter
    if (this.filters.dateRange) {
      const now = new Date();
      let filterDate;

      switch (this.filters.dateRange) {
        case 'week':
          filterDate = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
          break;
        case 'month':
          filterDate = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
          break;
        case 'year':
          filterDate = new Date(now.getTime() - 365 * 24 * 60 * 60 * 1000);
          break;
      }

      if (filterDate) {
        filtered = filtered.filter(order => new Date(order.createdAt) >= filterDate);
      }
    }

    // Search filter
    if (this.filters.search) {
      filtered = filtered.filter(order =>
        (order.orderNumber || order.id).toString().toLowerCase().includes(this.filters.search)
      );
    }

    this.filteredOrders = filtered;
    this.renderOrders();
  }

  renderOrders() {
    const container = document.getElementById('orders-list');
    const noOrders = document.getElementById('no-orders');

    if (this.filteredOrders.length === 0) {
      container.innerHTML = '';
      noOrders.style.display = 'block';
      return;
    }

    noOrders.style.display = 'none';

    container.innerHTML = this.filteredOrders.map(order => `
      <div class="order-card">
        <div class="order-header">
          <div class="order-number">
            <strong>Order #${order.orderNumber || order.id}</strong>
            <span class="order-date">${new Date(order.createdAt).toLocaleDateString()}</span>
          </div>
          <div class="order-status">
            <span class="status-badge status-${order.status}">${this.formatStatus(order.status)}</span>
          </div>
        </div>

        <div class="order-items-summary">
          <div class="items-count">
            ${order.items.length} item${order.items.length !== 1 ? 's' : ''}
          </div>
          <div class="items-preview">
            ${order.items.slice(0, 3).map(item => `
              <div class="item-preview">
                <img src="${item.image || '/images/placeholder.jpg'}" alt="${sanitizeHtml(item.name)}">
              </div>
            `).join('')}
            ${order.items.length > 3 ? `<span class="more-items">+${order.items.length - 3}</span>` : ''}
          </div>
        </div>

        <div class="order-details">
          <div class="detail-row">
            <span>Total:</span>
            <strong>${formatCurrency(order.total)}</strong>
          </div>
          <div class="detail-row">
            <span>Delivery To:</span>
            <span>${sanitizeHtml(order.shippingAddress.city)}, ${sanitizeHtml(order.shippingAddress.state)}</span>
          </div>
        </div>

        <div class="order-actions">
          <a href="/pages/customer/order-details.html?orderId=${order.id}" class="btn btn-outline btn-sm">
            View Details
          </a>
          ${order.status === 'pending' ? `
            <button class="btn btn-danger btn-sm" onclick="if(confirm('Cancel this order?')) { location.href='#'; }">
              Cancel Order
            </button>
          ` : ''}
        </div>
      </div>
    `).join('');
  }

  formatStatus(status) {
    const statusMap = {
      'pending': 'Pending',
      'processing': 'Processing',
      'shipped': 'Shipped',
      'delivered': 'Delivered',
      'cancelled': 'Cancelled',
      'returned': 'Returned'
    };
    return statusMap[status] || status;
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new OrdersPage();
});
