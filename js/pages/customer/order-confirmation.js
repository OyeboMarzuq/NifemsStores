import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class OrderConfirmationPage {
  constructor() {
    this.orderData = null;
    this.init();
  }

  async init() {
    const orderId = this.getOrderIdFromURL();
    if (!orderId) {
      window.location.href = '/pages/customer/home.html';
      return;
    }

    await this.loadOrderData(orderId);
    if (this.orderData) {
      this.renderConfirmation();
    }
  }

  getOrderIdFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get('orderId');
  }

  async loadOrderData(orderId) {
    try {
      const response = await apiCall(`/api/orders/${orderId}`);
      this.orderData = response.data;
    } catch (error) {
      console.error('Error loading order:', error);
      window.location.href = '/pages/customer/home.html';
    }
  }

  renderConfirmation() {
    const order = this.orderData;

    // Order details
    document.getElementById('order-number').textContent = order.orderNumber || order.id;
    document.getElementById('order-date').textContent = new Date(order.createdAt).toLocaleDateString();
    document.getElementById('order-status').textContent = order.status || 'Processing';

    // Shipping address
    const shipping = order.shippingAddress;
    document.getElementById('shipping-address').innerHTML = `
      <p>
        ${sanitizeHtml(shipping.firstName)} ${sanitizeHtml(shipping.lastName)}<br>
        ${sanitizeHtml(shipping.address)}<br>
        ${sanitizeHtml(shipping.city)}, ${sanitizeHtml(shipping.state)} ${sanitizeHtml(shipping.zip)}<br>
        ${sanitizeHtml(shipping.country)}<br>
        <strong>Phone:</strong> ${sanitizeHtml(shipping.phone)}<br>
        <strong>Email:</strong> ${sanitizeHtml(shipping.email)}
      </p>
    `;

    // Order items
    const itemsBody = document.getElementById('order-items');
    itemsBody.innerHTML = order.items.map(item => `
      <tr>
        <td>${sanitizeHtml(item.name)}</td>
        <td>${item.quantity}</td>
        <td>${formatCurrency(item.price)}</td>
        <td>${formatCurrency(item.price * item.quantity)}</td>
      </tr>
    `).join('');

    // Order summary
    document.getElementById('summary-subtotal').textContent = formatCurrency(order.subtotal);
    document.getElementById('summary-shipping').textContent = formatCurrency(order.shipping);
    document.getElementById('summary-tax').textContent = formatCurrency(order.tax);
    document.getElementById('summary-total').textContent = formatCurrency(order.total);

    // Payment method
    const methodText = {
      'credit-card': 'Credit/Debit Card',
      'bank-transfer': 'Bank Transfer',
      'cash-delivery': 'Cash on Delivery'
    };
    document.getElementById('payment-method').textContent = methodText[order.paymentMethod] || order.paymentMethod;
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new OrderConfirmationPage();
});
