import { getFromLocalStorage } from '../../utils/localStorage.js';

export function renderVendorSidebar() {
  const user = getFromLocalStorage('user') || {};
  const container = document.getElementById('sidebar-container');
  
  if (!container) return;

  const currentPage = window.location.pathname.split('/').pop().replace('.html', '');

  const nav = [
    { page: 'dashboard', label: 'Dashboard', icon: '📊' },
    { page: 'products', label: 'Products', icon: '📦', badge: true },
    { page: 'orders', label: 'Orders', icon: '🛒', badge: true },
    { page: 'analytics', label: 'Analytics', icon: '📈' },
    { page: 'reviews', label: 'Reviews', icon: '⭐' },
    { page: 'settings', label: 'Settings', icon: '⚙️' }
  ];

  const navHtml = nav.map(item => `
    <li class="vendor-nav-item">
      <a href="/pages/vendor/${item.page}.html" class="vendor-nav-link ${currentPage === item.page ? 'active' : ''}">
        <span>${item.icon}</span>
        <span>${item.label}</span>
        ${item.badge ? '<span class="badge">0</span>' : ''}
      </a>
    </li>
  `).join('');

  container.innerHTML = `
    <div class="vendor-sidebar-header">
      <div class="vendor-profile">
        <div class="vendor-avatar">${user.storeName ? user.storeName.charAt(0).toUpperCase() : 'S'}</div>
        <div class="vendor-profile-info">
          <h3>${user.storeName || 'My Store'}</h3>
          <p>${user.email || 'store@example.com'}</p>
        </div>
      </div>
    </div>

    <nav class="vendor-nav">
      ${navHtml}
      <li class="vendor-nav-item" style="border-top: 1px solid #e0e0e0; margin-top: auto;">
        <a href="#" class="vendor-nav-link" id="logout-vendor-btn">
          <span>🚪</span>
          <span>Logout</span>
        </a>
      </li>
    </nav>
  `;

  // Add logout handler
  const logoutBtn = document.getElementById('logout-vendor-btn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', (e) => {
      e.preventDefault();
      if (confirm('Are you sure you want to logout?')) {
        localStorage.clear();
        window.location.href = '/pages/login.html';
      }
    });
  }
}

// Auto-render when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  renderVendorSidebar();
});
