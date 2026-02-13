export class Footer {
  constructor() {
    this.footerElement = document.getElementById('footer');
  }

  render() {
    const currentYear = new Date().getFullYear();

    const html = `
      <footer id="app-footer" class="app-footer">
        <div class="footer-container">
          <!-- Footer Content -->
          <div class="footer-grid">
            <!-- About Section -->
            <div class="footer-section">
              <h4 class="footer-title">About NifemsStore</h4>
              <ul class="footer-list">
                <li><a href="#">About Us</a></li>
                <li><a href="#">Careers</a></li>
                <li><a href="#">Blog</a></li>
                <li><a href="#">Press</a></li>
              </ul>
            </div>

            <!-- Help Section -->
            <div class="footer-section">
              <h4 class="footer-title">Help & Support</h4>
              <ul class="footer-list">
                <li><a href="#">Contact Us</a></li>
                <li><a href="#">FAQ</a></li>
                <li><a href="#">Shipping Info</a></li>
                <li><a href="#">Returns</a></li>
              </ul>
            </div>

            <!-- Vendors Section -->
            <div class="footer-section">
              <h4 class="footer-title">For Vendors</h4>
              <ul class="footer-list">
                <li><a href="#">Become a Vendor</a></li>
                <li><a href="#">Seller Center</a></li>
                <li><a href="#">Seller Guide</a></li>
                <li><a href="#">Commission Rates</a></li>
              </ul>
            </div>

            <!-- Policy Section -->
            <div class="footer-section">
              <h4 class="footer-title">Legal</h4>
              <ul class="footer-list">
                <li><a href="#">Terms of Service</a></li>
                <li><a href="#">Privacy Policy</a></li>
                <li><a href="#">Cookie Policy</a></li>
                <li><a href="#">Sitemap</a></li>
              </ul>
            </div>

            <!-- Newsletter Section -->
            <div class="footer-section footer-newsletter">
              <h4 class="footer-title">Newsletter</h4>
              <p class="footer-description">Subscribe to get special offers and updates.</p>
              <form id="newsletter-form" class="newsletter-form">
                <input
                  type="email"
                  placeholder="Enter your email"
                  class="newsletter-input"
                  required
                >
                <button type="submit" class="btn btn-primary btn-sm">Subscribe</button>
              </form>
            </div>
          </div>

          <!-- Footer Bottom -->
          <div class="footer-bottom">
            <div class="footer-bottom-left">
              <p>&copy; ${currentYear} NifemsStore. All rights reserved.</p>
            </div>
            <div class="footer-bottom-right">
              <div class="social-links">
                <a href="#" class="social-link" title="Facebook">f</a>
                <a href="#" class="social-link" title="Twitter">𝕏</a>
                <a href="#" class="social-link" title="Instagram">📷</a>
                <a href="#" class="social-link" title="LinkedIn">in</a>
              </div>
            </div>
          </div>
        </div>
      </footer>
    `;

    const footerContainer = document.createElement('div');
    footerContainer.innerHTML = html;

    if (this.footerElement) {
      this.footerElement.replaceWith(footerContainer);
    }

    this.setupEventListeners();
  }

  setupEventListeners() {
    const newsletterForm = document.getElementById('newsletter-form');
    
    if (newsletterForm) {
      newsletterForm.addEventListener('submit', (e) => {
        e.preventDefault();
        this.handleNewsletterSubscribe();
      });
    }
  }

  handleNewsletterSubscribe() {
    const email = document.querySelector('.newsletter-input').value;
    
    // Here you would send the email to your backend
    console.log('[v0] Newsletter subscription:', email);
    
    // Show success message
    alert('Thank you for subscribing! Check your email for confirmation.');
    document.getElementById('newsletter-form').reset();
  }
}

// Initialize footer on page load
export function initFooter() {
  const footer = new Footer();
  footer.render();
}
