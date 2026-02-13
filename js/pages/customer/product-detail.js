import { apiCall } from '../../utils/api.js';
import { getFromLocalStorage, saveToLocalStorage, addToCart } from '../../utils/localStorage.js';
import { formatCurrency } from '../../utils/formatter.js';
import { sanitizeHtml } from '../../utils/validation.js';

class ProductDetailPage {
  constructor() {
    this.product = null;
    this.quantity = 1;
    this.relatedProducts = [];
    this.init();
  }

  async init() {
    const productId = this.getProductIdFromURL();
    if (!productId) {
      window.location.href = '/pages/customer/products.html';
      return;
    }

    await this.loadProduct(productId);
    if (this.product) {
      this.setupEventListeners();
      this.renderProductDetails();
      this.loadRelatedProducts();
    }
  }

  getProductIdFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get('id');
  }

  async loadProduct(productId) {
    try {
      const response = await apiCall(`/api/products/${productId}`);
      this.product = response.data;
    } catch (error) {
      console.error('Error loading product:', error);
      window.location.href = '/pages/customer/products.html';
    }
  }

  setupEventListeners() {
    // Quantity controls
    document.getElementById('decrease-qty').addEventListener('click', () => {
      if (this.quantity > 1) {
        this.quantity--;
        document.getElementById('quantity').value = this.quantity;
      }
    });

    document.getElementById('increase-qty').addEventListener('click', () => {
      const max = this.product.stock;
      if (this.quantity < max) {
        this.quantity++;
        document.getElementById('quantity').value = this.quantity;
      }
    });

    document.getElementById('quantity').addEventListener('change', (e) => {
      let val = parseInt(e.target.value) || 1;
      val = Math.min(val, this.product.stock);
      val = Math.max(val, 1);
      this.quantity = val;
      e.target.value = val;
    });

    // Add to cart
    document.getElementById('add-to-cart').addEventListener('click', () => {
      this.addToCart();
    });

    // Buy now
    document.getElementById('buy-now').addEventListener('click', () => {
      this.addToCart();
      window.location.href = '/pages/customer/cart.html';
    });

    // Wishlist
    document.getElementById('wishlist-btn').addEventListener('click', (e) => {
      this.toggleWishlist(e.target);
    });

    // Image gallery
    const thumbnails = document.querySelectorAll('.thumbnail');
    thumbnails.forEach(thumb => {
      thumb.addEventListener('click', (e) => {
        const mainImage = document.getElementById('main-image');
        mainImage.src = e.target.src;
      });
    });

    // Tab buttons
    document.querySelectorAll('.tab-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const tabName = e.target.dataset.tab;
        this.switchTab(tabName);
      });
    });
  }

  renderProductDetails() {
    const product = this.product;

    // Breadcrumb
    document.getElementById('breadcrumb-name').textContent = product.name;

    // Images
    document.getElementById('main-image').src = product.imageUrl || '/images/placeholder.jpg';
    
    if (product.additionalImages && product.additionalImages.length > 0) {
      const thumbnailsContainer = document.getElementById('thumbnails-container');
      thumbnailsContainer.innerHTML = [product.imageUrl, ...product.additionalImages]
        .map((img, idx) => `
          <div class="thumbnail" style="cursor: pointer;">
            <img src="${img}" alt="Product image ${idx + 1}" class="thumbnail">
          </div>
        `).join('');

      document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.addEventListener('click', (e) => {
          document.getElementById('main-image').src = thumb.src;
        });
      });
    }

    // Discount badge
    if (product.discount > 0) {
      const badge = document.getElementById('discount-badge');
      badge.textContent = `-${product.discount}%`;
      badge.style.display = 'block';
    }

    // Vendor info
    document.getElementById('vendor-name').textContent = product.vendorName || 'Store';
    document.getElementById('vendor-stars').innerHTML = this.renderStars(product.vendorRating || 4.5);
    document.getElementById('vendor-reviews').textContent = `${product.vendorReviews || 0} reviews`;

    // Product title and rating
    document.getElementById('product-name').textContent = sanitizeHtml(product.name);
    document.getElementById('product-stars').innerHTML = this.renderStars(product.rating || 0);
    document.getElementById('review-count').textContent = `${product.reviewCount || 0} reviews`;

    // Price
    document.getElementById('current-price').textContent = formatCurrency(product.price);
    if (product.originalPrice && product.originalPrice > product.price) {
      document.getElementById('original-price').textContent = formatCurrency(product.originalPrice);
      document.getElementById('original-price').style.display = 'inline';
    }

    // Stock status
    const stockStatus = document.getElementById('stock-status');
    if (product.stock > 0) {
      stockStatus.textContent = product.stock <= 5 ? `Only ${product.stock} left!` : 'In Stock';
      stockStatus.className = 'stock-status in-stock';
    } else {
      stockStatus.textContent = 'Out of Stock';
      stockStatus.className = 'stock-status out-of-stock';
      document.getElementById('add-to-cart').disabled = true;
      document.getElementById('buy-now').disabled = true;
    }

    document.getElementById('available-stock').textContent = `${product.stock} available`;

    // Features
    if (product.features && product.features.length > 0) {
      document.getElementById('features-list').innerHTML = product.features
        .map(feature => `<li>${sanitizeHtml(feature)}</li>`)
        .join('');
    }

    // Meta information
    document.getElementById('sku').textContent = product.sku || 'N/A';
    document.getElementById('category').textContent = product.category || 'Uncategorized';
    document.getElementById('shipping-info').textContent = `$${product.shippingCost || 'Free'}`;

    // Description
    document.getElementById('product-description').innerHTML = sanitizeHtml(product.description || 'No description available');

    // Specifications
    if (product.specifications) {
      const specsBody = document.getElementById('specs-body');
      specsBody.innerHTML = Object.entries(product.specifications)
        .map(([key, value]) => `
          <tr>
            <td>${sanitizeHtml(key)}</td>
            <td>${sanitizeHtml(value)}</td>
          </tr>
        `).join('');
    }

    // Reviews
    this.renderReviews();

    // Check wishlist
    const wishlist = getFromLocalStorage('wishlist') || [];
    const wishlistBtn = document.getElementById('wishlist-btn');
    if (wishlist.includes(product.id)) {
      wishlistBtn.textContent = '♥';
      wishlistBtn.style.color = '#E63946';
    }
  }

  renderReviews() {
    const product = this.product;
    const reviews = product.reviews || [];

    // Rating distribution
    const distribution = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };
    reviews.forEach(review => {
      distribution[review.rating]++;
    });

    const total = reviews.length;
    document.getElementById('avg-rating').textContent = (product.rating || 0).toFixed(1);

    for (let i = 5; i >= 1; i--) {
      const percentage = total > 0 ? (distribution[i] / total) * 100 : 0;
      document.getElementById(`bar-${i}`).style.width = `${percentage}%`;
    }

    // Reviews list
    const reviewsList = document.getElementById('reviews-list');
    if (reviews.length === 0) {
      reviewsList.innerHTML = '<p>No reviews yet. Be the first to review this product!</p>';
      return;
    }

    reviewsList.innerHTML = reviews.map(review => `
      <div class="review-item">
        <div class="review-header">
          <div class="reviewer-info">
            <strong>${sanitizeHtml(review.reviewerName)}</strong>
            <span class="review-stars">${this.renderStars(review.rating)}</span>
          </div>
          <span class="review-date">${new Date(review.date).toLocaleDateString()}</span>
        </div>
        <div class="review-title">${sanitizeHtml(review.title)}</div>
        <div class="review-text">${sanitizeHtml(review.comment)}</div>
        ${review.images && review.images.length > 0 ? `
          <div class="review-images">
            ${review.images.map(img => `<img src="${img}" alt="Review image">`).join('')}
          </div>
        ` : ''}
      </div>
    `).join('');
  }

  async loadRelatedProducts() {
    try {
      const response = await apiCall(`/api/products?category=${this.product.category}&limit=4`);
      this.relatedProducts = (response.data || []).filter(p => p.id !== this.product.id).slice(0, 4);
      this.renderRelatedProducts();
    } catch (error) {
      console.error('Error loading related products:', error);
    }
  }

  renderRelatedProducts() {
    const grid = document.getElementById('related-products-grid');
    grid.innerHTML = this.relatedProducts.map(product => `
      <div class="product-card">
        <div class="product-image-container">
          <img src="${product.imageUrl}" alt="${sanitizeHtml(product.name)}">
          ${product.discount > 0 ? `<span class="discount-badge">-${product.discount}%</span>` : ''}
        </div>
        <div class="product-info">
          <h3>${sanitizeHtml(product.name)}</h3>
          <div class="product-rating">
            <div class="stars">${this.renderStars(product.rating || 0)}</div>
          </div>
          <div class="product-price">
            <span class="price">${formatCurrency(product.price)}</span>
          </div>
          <button class="btn btn-outline btn-sm" onclick="window.location.href='/pages/customer/product-detail.html?id=${product.id}'">
            View Details
          </button>
        </div>
      </div>
    `).join('');
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

  switchTab(tabName) {
    // Hide all panes
    document.querySelectorAll('.tab-pane').forEach(pane => {
      pane.classList.remove('active');
    });

    // Deactivate all buttons
    document.querySelectorAll('.tab-btn').forEach(btn => {
      btn.classList.remove('active');
    });

    // Show selected pane
    document.getElementById(tabName).classList.add('active');

    // Activate selected button
    document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');
  }

  addToCart() {
    addToCart({
      productId: this.product.id,
      name: this.product.name,
      price: this.product.price,
      image: this.product.imageUrl,
      vendor: this.product.vendorName,
      quantity: this.quantity
    });

    const toast = document.createElement('div');
    toast.className = 'toast success';
    toast.textContent = `${this.product.name} added to cart!`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
  }

  toggleWishlist(button) {
    const wishlist = getFromLocalStorage('wishlist') || [];
    const productId = this.product.id;
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
}

document.addEventListener('DOMContentLoaded', () => {
  new ProductDetailPage();
});
