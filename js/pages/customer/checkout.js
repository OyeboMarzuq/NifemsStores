import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage, saveToLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { validateEmail, sanitizeHtml } from '../../utils/validation.js';

class CheckoutPage {
  constructor() {
    this.cart = [];
    this.orderData = {};
    this.subtotal = 0;
    this.shipping = 10;
    this.tax = 0;
    this.total = 0;
    this.currentStep = 1;
    this.init();
  }

  init() {
    this.loadCart();
    if (this.cart.length === 0) {
      window.location.href = '/pages/customer/cart.html';
      return;
    }

    this.setupEventListeners();
    this.renderOrderSummary();
    this.calculateTotals();
    this.populateUserData();
  }

  loadCart() {
    this.cart = getFromLocalStorage('cart') || [];
  }

  setupEventListeners() {
    // Shipping form
    document.getElementById('next-payment-btn').addEventListener('click', () => {
      if (this.validateShippingForm()) {
        this.saveShippingData();
        this.showPaymentStep();
      }
    });

    // Back button
    document.getElementById('back-shipping-btn').addEventListener('click', () => {
      this.showShippingStep();
    });

    // Payment method selection
    document.querySelectorAll('input[name="payment-method"]').forEach(radio => {
      radio.addEventListener('change', (e) => {
        this.switchPaymentMethod(e.target.value);
      });
    });

    // Card number formatting
    document.getElementById('card-number').addEventListener('input', (e) => {
      let value = e.target.value.replace(/\s/g, '').replace(/[^\d]/g, '');
      let formatted = value.replace(/(\d{4})(?=\d)/g, '$1 ');
      e.target.value = formatted;
    });

    // Expiry formatting
    document.getElementById('expiry').addEventListener('input', (e) => {
      let value = e.target.value.replace(/\D/g, '');
      if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2, 4);
      }
      e.target.value = value;
    });

    // Place order button
    document.getElementById('place-order-btn').addEventListener('click', () => {
      this.placeOrder();
    });

    // Same billing address checkbox
    document.getElementById('same-billing').addEventListener('change', (e) => {
      // In a full implementation, would toggle billing form visibility
    });
  }

  populateUserData() {
    const userData = getFromLocalStorage('userData');
    if (userData) {
      document.getElementById('first-name').value = userData.firstName || '';
      document.getElementById('last-name').value = userData.lastName || '';
      document.getElementById('email').value = userData.email || '';
      document.getElementById('phone').value = userData.phone || '';
      document.getElementById('address').value = userData.address || '';
      document.getElementById('city').value = userData.city || '';
      document.getElementById('state').value = userData.state || '';
      document.getElementById('zip').value = userData.zip || '';
      document.getElementById('country').value = userData.country || '';
    }
  }

  validateShippingForm() {
    const requiredFields = [
      'first-name', 'last-name', 'email', 'phone', 'address', 'city', 'state', 'zip', 'country'
    ];

    for (const field of requiredFields) {
      const input = document.getElementById(field);
      if (!input.value.trim()) {
        this.showError(`${input.previousElementSibling.textContent} is required`);
        input.focus();
        return false;
      }
    }

    // Validate email
    if (!validateEmail(document.getElementById('email').value)) {
      this.showError('Invalid email address');
      return false;
    }

    return true;
  }

  saveShippingData() {
    this.orderData.shipping = {
      firstName: document.getElementById('first-name').value,
      lastName: document.getElementById('last-name').value,
      email: document.getElementById('email').value,
      phone: document.getElementById('phone').value,
      address: document.getElementById('address').value,
      city: document.getElementById('city').value,
      state: document.getElementById('state').value,
      zip: document.getElementById('zip').value,
      country: document.getElementById('country').value
    };

    // Save to localStorage
    saveToLocalStorage('userData', this.orderData.shipping);
  }

  showPaymentStep() {
    document.getElementById('shipping-section').style.display = 'none';
    document.getElementById('payment-section').style.display = 'block';
    this.currentStep = 2;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  showShippingStep() {
    document.getElementById('shipping-section').style.display = 'block';
    document.getElementById('payment-section').style.display = 'none';
    this.currentStep = 1;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  switchPaymentMethod(method) {
    document.querySelectorAll('.payment-form').forEach(form => {
      form.style.display = 'none';
    });

    switch (method) {
      case 'credit-card':
        document.getElementById('credit-card-form').style.display = 'block';
        break;
      case 'bank-transfer':
        document.getElementById('bank-transfer-form').style.display = 'block';
        document.getElementById('transfer-amount').textContent = formatCurrency(this.total);
        break;
      case 'cash-delivery':
        document.getElementById('cod-form').style.display = 'block';
        break;
    }
  }

  renderOrderSummary() {
    const itemsContainer = document.getElementById('order-items');
    itemsContainer.innerHTML = this.cart.map(item => `
      <div class="summary-item-row">
        <div class="summary-item-info">
          <span class="item-name">${sanitizeHtml(item.name)}</span>
          <span class="item-qty">x${item.quantity}</span>
        </div>
        <span class="item-amount">${formatCurrency(item.price * item.quantity)}</span>
      </div>
    `).join('');
  }

  calculateTotals() {
    this.subtotal = this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    this.shipping = this.subtotal > 100 ? 0 : 10;
    this.tax = this.subtotal * 0.1;
    this.total = this.subtotal + this.shipping + this.tax;

    document.getElementById('summary-subtotal').textContent = formatCurrency(this.subtotal);
    document.getElementById('summary-shipping').textContent = formatCurrency(this.shipping);
    document.getElementById('summary-tax').textContent = formatCurrency(this.tax);
    document.getElementById('summary-total').textContent = formatCurrency(this.total);
  }

  async placeOrder() {
    const paymentMethod = document.querySelector('input[name="payment-method"]:checked').value;

    // Validate payment method
    if (!this.validatePaymentMethod(paymentMethod)) {
      return;
    }

    // Prepare order data
    const orderData = {
      shippingAddress: this.orderData.shipping,
      items: this.cart,
      paymentMethod: paymentMethod,
      subtotal: this.subtotal,
      shipping: this.shipping,
      tax: this.tax,
      total: this.total,
      paymentDetails: this.getPaymentDetails(paymentMethod)
    };

    try {
      const response = await apiCall('/api/orders', {
        method: 'POST',
        body: JSON.stringify(orderData)
      });

      if (response.success) {
        const orderId = response.data.orderId;
        saveToLocalStorage('lastOrderId', orderId);
        saveToLocalStorage('cart', []); // Clear cart
        
        window.location.href = `/pages/customer/order-confirmation.html?orderId=${orderId}`;
      }
    } catch (error) {
      this.showError('Failed to place order. Please try again.');
      console.error('Order placement error:', error);
    }
  }

  validatePaymentMethod(method) {
    switch (method) {
      case 'credit-card':
        return this.validateCreditCard();
      case 'bank-transfer':
        return true; // Bank transfer doesn't need validation here
      case 'cash-delivery':
        return true; // Cash on delivery doesn't need validation
      default:
        return false;
    }
  }

  validateCreditCard() {
    const cardNumber = document.getElementById('card-number').value.replace(/\s/g, '');
    const expiry = document.getElementById('expiry').value;
    const cvv = document.getElementById('cvv').value;
    const cardHolder = document.getElementById('card-holder').value;

    if (!cardHolder.trim()) {
      this.showError('Card holder name is required');
      return false;
    }

    if (cardNumber.length !== 16) {
      this.showError('Invalid card number');
      return false;
    }

    if (!expiry.match(/^\d{2}\/\d{2}$/)) {
      this.showError('Invalid expiry date (use MM/YY format)');
      return false;
    }

    if (cvv.length !== 3) {
      this.showError('Invalid CVV');
      return false;
    }

    return true;
  }

  getPaymentDetails(method) {
    switch (method) {
      case 'credit-card':
        return {
          cardHolder: document.getElementById('card-holder').value,
          cardNumber: document.getElementById('card-number').value.slice(-4), // Only last 4 digits
          expiry: document.getElementById('expiry').value
        };
      case 'bank-transfer':
        return {
          method: 'bank-transfer'
        };
      case 'cash-delivery':
        return {
          method: 'cash-delivery'
        };
      default:
        return {};
    }
  }

  showError(message) {
    const alert = document.createElement('div');
    alert.className = 'alert alert-danger';
    alert.textContent = message;
    const container = document.querySelector('.checkout-container');
    container.insertBefore(alert, container.firstChild);

    setTimeout(() => {
      alert.style.animation = 'fadeOut 0.3s ease-out';
      setTimeout(() => alert.remove(), 300);
    }, 5000);
  }

  showSuccess(message) {
    const toast = document.createElement('div');
    toast.className = 'toast success';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
  }
}

document.addEventListener('DOMContentLoaded', () => {
  new CheckoutPage();
});
