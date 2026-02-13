import { getFromLocalStorage, saveToLocalStorage } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class CartPage {
  constructor() {
    this.cart = [];
    this.subtotal = 0;
    this.shipping = 0;
    this.tax = 0;
    this.discount = 0;
    this.total = 0;
    this.taxRate = 0.1; // 10% tax
    this.init();
  }

  init() {
    this.loadCart();
    this.setupEventListeners();
    this.renderCart();
    this.calculateTotals();
  }

  loadCart() {
    this.cart = getFromLocalStorage('cart') || [];
  }

  setupEventListeners() {
    document.getElementById('checkout-btn').addEventListener('click', () => {
      if (this.cart.length > 0) {
        window.location.href = '/pages/customer/checkout.html';
      }
    });

    document.getElementById('apply-promo').addEventListener('click', () => {
      this.applyPromoCode();
    });

    document.getElementById('promo-code').addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        this.applyPromoCode();
      }
    });
  }

  renderCart() {
    const container = document.getElementById('cart-items-container');
    const emptyCart = document.getElementById('empty-cart');

    if (this.cart.length === 0) {
      container.style.display = 'none';
      emptyCart.style.display = 'block';
      document.getElementById('checkout-btn').disabled = true;
      return;
    }

    container.style.display = 'block';
    emptyCart.style.display = 'none';
    document.getElementById('checkout-btn').disabled = false;

    container.innerHTML = this.cart.map((item, index) => `
      <div class="cart-item" data-index="${index}">
        <div class="item-image">
          <img src="${item.image || '/images/placeholder.jpg'}" alt="${sanitizeHtml(item.name)}">
        </div>

        <div class="item-details">
          <h3>${sanitizeHtml(item.name)}</h3>
          <p class="vendor-name">${sanitizeHtml(item.vendor)}</p>
          <p class="item-price">${formatCurrency(item.price)}</p>
        </div>

        <div class="item-quantity">
          <label>Quantity:</label>
          <div class="quantity-input">
            <button class="qty-btn decrease-item-qty" data-index="${index}">-</button>
            <input type="number" class="item-qty-input" value="${item.quantity}" min="1" data-index="${index}">
            <button class="qty-btn increase-item-qty" data-index="${index}">+</button>
          </div>
        </div>

        <div class="item-total">
          <div>${formatCurrency(item.price * item.quantity)}</div>
        </div>

        <button class="remove-item-btn" data-index="${index}" title="Remove from cart">
          ✕
        </button>
      </div>
    `).join('');

    // Add event listeners to cart items
    this.attachCartItemListeners();
  }

  attachCartItemListeners() {
    // Quantity buttons
    document.querySelectorAll('.decrease-item-qty').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.target.dataset.index);
        if (this.cart[index].quantity > 1) {
          this.cart[index].quantity--;
          this.saveCart();
          this.renderCart();
          this.calculateTotals();
        }
      });
    });

    document.querySelectorAll('.increase-item-qty').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.target.dataset.index);
        this.cart[index].quantity++;
        this.saveCart();
        this.renderCart();
        this.calculateTotals();
      });
    });

    document.querySelectorAll('.item-qty-input').forEach(input => {
      input.addEventListener('change', (e) => {
        const index = parseInt(e.target.dataset.index);
        let value = parseInt(e.target.value) || 1;
        value = Math.max(1, value);
        this.cart[index].quantity = value;
        this.saveCart();
        this.renderCart();
        this.calculateTotals();
      });
    });

    // Remove buttons
    document.querySelectorAll('.remove-item-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.target.dataset.index);
        this.cart.splice(index, 1);
        this.saveCart();
        this.renderCart();
        this.calculateTotals();
      });
    });
  }

  calculateTotals() {
    this.subtotal = this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    
    // Calculate shipping (simplified - could be based on location/weight)
    this.shipping = this.subtotal > 100 ? 0 : 10;
    
    // Calculate tax
    this.tax = this.subtotal * this.taxRate;
    
    // Calculate total
    this.total = this.subtotal + this.shipping + this.tax - this.discount;

    this.updateSummary();
  }

  updateSummary() {
    document.getElementById('subtotal').textContent = formatCurrency(this.subtotal);
    document.getElementById('shipping-cost').textContent = formatCurrency(this.shipping);
    document.getElementById('tax-amount').textContent = formatCurrency(this.tax);
    document.getElementById('total-price').textContent = formatCurrency(Math.max(0, this.total));

    if (this.discount > 0) {
      document.getElementById('discount-item').style.display = 'flex';
      document.getElementById('discount-amount').textContent = `-${formatCurrency(this.discount)}`;
    } else {
      document.getElementById('discount-item').style.display = 'none';
    }
  }

  applyPromoCode() {
    const promoInput = document.getElementById('promo-code');
    const code = promoInput.value.trim().toUpperCase();

    if (!code) {
      this.showError('Please enter a promo code');
      return;
    }

    // Mock promo codes
    const promoCodes = {
      'SAVE10': { discount: this.subtotal * 0.1, message: '10% discount applied!' },
      'SAVE20': { discount: this.subtotal * 0.2, message: '20% discount applied!' },
      'WELCOME': { discount: 25, message: '$25 off applied!' },
      'FREE': { discount: this.shipping, message: 'Free shipping applied!' }
    };

    const promo = promoCodes[code];
    if (promo) {
      this.discount = promo.discount;
      this.calculateTotals();
      this.showSuccess(promo.message);
      promoInput.disabled = true;
      document.getElementById('apply-promo').disabled = true;
    } else {
      this.showError('Invalid promo code');
      promoInput.focus();
    }
  }

  saveCart() {
    saveToLocalStorage('cart', this.cart);
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
  new CartPage();
});
