# NifemsStore - Multi-Vendor E-Commerce Platform

A modern, fully-featured e-commerce marketplace built with vanilla HTML/CSS/JavaScript and ASP.NET backend. Support for multiple user roles: Customers, Vendors, and Administrators.

## 🚀 Quick Start

### Prerequisites
- Web browser with ES6 support
- ASP.NET backend API (update base URL in config)
- Modern Node.js (if you want to use a build tool)

### Setup

1. **Clone the repository**
```bash
git clone <repository-url>
cd nifems-store
```

2. **Update API Configuration**
Edit `js/config.js` and set your API base URL:
```javascript
const API_CONFIG = {
  BASE_URL: 'https://your-api-url.com/api',
  // ... rest of config
};
```

3. **Serve the project**
```bash
# Using Python
python -m http.server 8000

# Using Node.js (http-server)
npx http-server

# Using PHP
php -S localhost:8000
```

4. **Open in browser**
Navigate to `http://localhost:8000` and start at `/pages/login.html`

## 📁 Project Structure

```
nifems-store/
├── pages/
│   ├── login.html              # User login
│   ├── register.html           # User registration
│   └── customer/
│       ├── home.html           # Product listing
│       ├── product-detail.html # Product details
│       ├── cart.html           # Shopping cart
│       ├── checkout.html       # Checkout process
│       └── orders.html         # Order history
├── js/
│   ├── config.js               # Global configuration
│   ├── utils/                  # Utility functions
│   │   ├── api.js             # API client
│   │   ├── auth.js            # Authentication
│   │   ├── localStorage.js    # Local storage helpers
│   │   ├── formatter.js       # Data formatting
│   │   └── validation.js      # Form validation
│   ├── components/             # Reusable components
│   │   ├── header.js          # Header component
│   │   └── footer.js          # Footer component
│   └── pages/                  # Page-specific logic
│       └── auth/
│           ├── login.js
│           └── register.js
├── css/
│   ├── main.css                # Global styles
│   ├── components.css          # Component styles
│   ├── layout.css              # Layout styles
│   ├── auth.css                # Authentication styles
│   └── customer.css            # Customer portal styles
├── IMPLEMENTATION_GUIDE.md     # Detailed implementation guide
└── README.md                   # This file
```

## ✨ Features

### ✅ Completed
- **Authentication**
  - User login with email/password
  - Multi-step user registration
  - Password strength validation
  - Session management with JWT tokens
  - Role-based access control

- **Design System**
  - Professional color scheme
  - Responsive layout (mobile-first)
  - Accessibility-first approach
  - Smooth animations and transitions
  - Dark-mode ready CSS variables

- **Utilities**
  - API client with error handling
  - Form validation framework
  - Currency and date formatting
  - Local storage management
  - Token refresh mechanism

### 🚧 In Progress
- Customer product listing with filters
- Product detail pages
- Shopping cart management
- Checkout process

### 📋 Planned
- Vendor dashboard
- Admin panel
- Order management
- Payment integration (Stripe, PayPal, Bank Transfer)
- Product reviews and ratings
- Wishlist/favorites
- Inventory management

## 🔑 Key Technologies

- **Frontend**: Vanilla HTML5, CSS3, ES6+ JavaScript
- **Backend**: ASP.NET Core
- **Storage**: LocalStorage (client-side) + API (server-side)
- **Authentication**: JWT tokens
- **Styling**: CSS variables, Flexbox, CSS Grid

## 🎨 Design System

### Colors
- **Primary**: Professional Blue (#1E40AF)
- **Secondary**: Success Green (#10B981)
- **Accent**: Vibrant Orange (#F97316)
- **Neutrals**: Gray scale from white to charcoal

### Typography
- **Body**: System fonts (for performance)
- **Headings**: Bold weight, varied sizes
- **Code**: Monospace for technical content

### Spacing Scale
- xs: 4px, sm: 8px, md: 16px, lg: 24px
- xl: 32px, 2xl: 48px, 3xl: 64px

## 📱 Responsive Breakpoints

- **Mobile**: Default (< 480px)
- **Tablet**: 480px - 767px
- **Desktop**: 768px - 1023px
- **Wide**: 1024px+

## 🔐 Security Features

- JWT token-based authentication
- Automatic token refresh on expiration
- HTTP-only cookie support for tokens
- Form input validation (client + server)
- CSRF protection ready
- XSS protection (HTML sanitization)
- Password hashing on backend
- Role-based access control (RBAC)

## 📚 API Integration

### Authentication
```
POST   /api/auth/register     # User registration
POST   /api/auth/login        # User login
POST   /api/auth/logout       # User logout
POST   /api/auth/refresh-token # Refresh JWT token
GET    /api/auth/me           # Get current user
```

### Products
```
GET    /api/products                # List products
GET    /api/products/{id}           # Get product details
GET    /api/products/search?q=...   # Search products
GET    /api/categories              # Get categories
```

### Cart & Orders
```
POST   /api/cart/add                # Add to cart
DELETE /api/cart/remove/{itemId}    # Remove from cart
GET    /api/cart                    # Get cart
POST   /api/orders                  # Create order
GET    /api/orders                  # List user orders
GET    /api/orders/{id}             # Get order details
```

## 🚀 Development Workflow

1. **Create new page**
   - Add HTML file in `pages/` directory
   - Create corresponding CSS in `css/` directory
   - Create logic in `js/pages/` directory

2. **Add authentication check**
```javascript
import { requireAuth, requireRole } from '../../utils/auth.js';
import { CONSTANTS } from '../../utils/auth.js';

// Check if authenticated
if (!requireAuth()) return;

// Check for specific role
if (!requireRole(CONSTANTS.ROLES.VENDOR)) return;
```

3. **Use API client**
```javascript
import apiClient from '../../utils/api.js';
import { API_CONFIG } from '../../config.js';

const response = await apiClient.get(API_CONFIG.ENDPOINTS.GET_PRODUCTS);
```

4. **Manage cart**
```javascript
import { getCart, addToCart, removeFromCart } from '../../utils/localStorage.js';

const cart = getCart();
addToCart({ productId: 1, quantity: 2, price: 29.99 });
```

## 🧪 Testing

### Test Login
- Navigate to `/pages/login.html`
- Try different email formats
- Test password validation
- Verify error messages

### Test Registration
- Navigate to `/pages/register.html`
- Walk through 3-step process
- Check password strength meter
- Verify form validation
- Review step shows collected data

### Test Responsiveness
- Use DevTools device toolbar
- Test at various breakpoints
- Check mobile menu toggle
- Verify touch interactions

## 📖 Documentation

- **IMPLEMENTATION_GUIDE.md** - Detailed implementation guide with examples
- **API_DOCS.md** - (To be created) API endpoint documentation
- **CONTRIBUTING.md** - (To be created) Contribution guidelines
- **CHANGELOG.md** - (To be created) Version history

## 🤝 Contributing

To add features or fix bugs:

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

See CONTRIBUTING.md for detailed guidelines.

## 📄 License

This project is licensed under the MIT License - see LICENSE file for details.

## 🆘 Troubleshooting

### API calls not working
- Verify API_CONFIG.BASE_URL is correct
- Check CORS is enabled on backend
- Open DevTools Network tab to see requests

### Styles not loading
- Clear browser cache
- Check CSS file paths
- Verify CSS variables in main.css

### Authentication issues
- Check localStorage is enabled
- Verify token format in API response
- Look for 401 errors in Network tab

### Form validation not working
- Verify validation rules in page JS
- Check field names match HTML inputs
- Ensure error elements have correct IDs

## 📞 Support

For questions or issues:
1. Check IMPLEMENTATION_GUIDE.md
2. Review the code comments
3. Open an issue on GitHub
4. Contact development team

## 🗺️ Roadmap

### Q2 2026
- [ ] Complete customer portal
- [ ] Implement shopping cart
- [ ] Build checkout process
- [ ] Add order management

### Q3 2026
- [ ] Vendor dashboard
- [ ] Product management
- [ ] Sales analytics
- [ ] Vendor ratings

### Q4 2026
- [ ] Admin panel
- [ ] Payment gateway integration
- [ ] Advanced search and filters
- [ ] Customer reviews system

### Q1 2027
- [ ] Mobile app
- [ ] Inventory management
- [ ] Multi-language support
- [ ] Advanced analytics

## 🎯 Performance Targets

- **First Contentful Paint**: < 1.5s
- **Time to Interactive**: < 3s
- **Largest Contentful Paint**: < 2.5s
- **Cumulative Layout Shift**: < 0.1

---

**Version**: 1.0  
**Last Updated**: February 2026  
**Built for**: Modern Browsers (Chrome, Firefox, Safari, Edge)

Made with ❤️ by the NifemsStore Team
