# NifemsStore - Quick Start Guide

## Getting Started in 5 Minutes

### 1. Update Your API URL

Edit `js/config.js` and change:
```javascript
const API_BASE_URL = 'https://your-api-domain.com';
```

Replace with your actual ASP.NET backend URL.

### 2. Test Login Page

Navigate to: `/pages/login.html`

Test accounts (once your backend provides them):
- Email: customer@example.com / Password: Test123!
- Email: vendor@example.com / Password: Test123!
- Email: admin@example.com / Password: Test123!

### 3. User Roles & Flows

**Customer Flow:**
1. Login → /pages/login.html
2. Browse products → /pages/customer/home.html
3. View product → /pages/customer/products.html
4. Add to cart → Product details page
5. Checkout → /pages/customer/checkout.html
6. Confirm order → /pages/customer/order-confirmation.html

**Vendor Flow:**
1. Login with vendor role → /pages/login.html
2. Dashboard → /pages/vendor/dashboard.html
3. Manage products → /pages/vendor/products.html
4. Manage orders → /pages/vendor/orders.html

**Admin Flow:**
Pages coming soon (structure ready)

---

## Key Files to Know

### Configuration
- **js/config.js** - API URL and settings

### Utilities
- **js/utils/api.js** - API calls wrapper
- **js/utils/auth.js** - Authentication helpers
- **js/utils/localStorage.js** - Cart and data storage
- **js/utils/validation.js** - Form validation

### Components
- **js/components/header.js** - Main navigation
- **js/components/footer.js** - Footer

### Styling
- **css/main.css** - Global styles
- **css/components.css** - Reusable components
- **css/customer.css** - Customer pages
- **css/vendor.css** - Vendor dashboard

---

## Common Tasks

### Add a New Customer Page

1. Create HTML in `pages/customer/new-page.html`
2. Create JS in `js/pages/customer/new-page.js`
3. Add styles to `css/customer.css`
4. Import header/footer in HTML:
```html
<div id="header-container"></div>
<script type="module" src="../../js/components/header.js"></script>
<script type="module" src="../../js/components/footer.js"></script>
<script type="module" src="../../js/pages/customer/new-page.js"></script>
```

### Make an API Call

```javascript
import { apiCall } from '../../utils/api.js';

const response = await apiCall('/api/endpoint');
// or with options
const response = await apiCall('/api/endpoint', {
  method: 'POST',
  body: JSON.stringify(data)
});
```

### Access User Data

```javascript
import { getFromLocalStorage } from '../../utils/localStorage.js';

const user = getFromLocalStorage('user');
const cart = getFromLocalStorage('cart');
```

### Format Values

```javascript
import { formatCurrency, formatDate } from '../../utils/formatter.js';

formatCurrency(100);  // $100.00
formatDate(new Date()); // MM/DD/YYYY
```

### Validate Input

```javascript
import { validateEmail, sanitizeHtml } from '../../utils/validation.js';

validateEmail('test@example.com'); // true/false
sanitizeHtml('<script>alert("xss")</script>'); // safe text
```

---

## Environment Setup

### Required Files
- HTML pages in `/pages`
- JavaScript in `/js`
- Styles in `/css`

### Browser Console
Open developer tools (F12) to see:
- API call logs
- Validation messages
- Error messages
- Debug information

### LocalStorage Data
Stored data includes:
- `user` - Current user info
- `token` - JWT authentication token
- `cart` - Shopping cart items
- `wishlist` - Favorite products
- `userData` - User profile data

Clear with: `localStorage.clear()`

---

## Testing Payment Methods

### Credit Card
Card Number: 4111 1111 1111 1111
Expiry: 12/25
CVV: 123

### Bank Transfer
Instructions display on checkout

### Cash on Delivery
No additional info needed

---

## Troubleshooting

### API Not Connecting
1. Check `js/config.js` API_BASE_URL
2. Verify backend is running
3. Check browser console for errors
4. Verify CORS is configured

### Cart Not Saving
1. Check localStorage is enabled
2. Open DevTools > Storage > LocalStorage
3. Clear and try again

### Login Not Working
1. Verify credentials are correct
2. Check API endpoint `/api/auth/login`
3. Verify response includes token

### Styles Not Applying
1. Check CSS file is linked
2. Verify breakpoint for device size
3. Check for CSS conflicts
4. Clear browser cache

---

## Performance Tips

1. **Images**: Optimize before uploading
2. **API calls**: Use pagination for lists
3. **Caching**: Use LocalStorage for user data
4. **CSS**: Already optimized and minified ready
5. **JavaScript**: Load only needed modules

---

## Security Reminders

1. Never expose API keys in frontend
2. Always validate user input
3. Use HTTPS in production
4. Configure CORS properly
5. Implement rate limiting
6. Set secure headers
7. Use httpOnly cookies for tokens (not localStorage)

---

## Deployment

### Vercel
```bash
npm install -g vercel
vercel
```

### Netlify
```bash
npm install -g netlify-cli
netlify deploy
```

### Manual
- Upload all files to web server
- Update API_BASE_URL for production
- Configure HTTPS
- Set up custom domain

---

## Documentation Files

- **README.md** - General overview
- **IMPLEMENTATION_GUIDE.md** - Technical details
- **SYSTEM_COMPLETION_GUIDE.md** - Complete architecture
- **PROJECT_SUMMARY.md** - Project statistics
- **QUICK_START.md** - This file

---

## Next Steps

1. Update API configuration
2. Connect to your backend
3. Test login/register
4. Test product browsing
5. Test shopping cart
6. Test checkout process
7. Test vendor dashboard
8. Deploy to production

---

## Support Resources

- Check inline code comments
- Review existing implementation patterns
- Check utility functions for usage
- Review API wrapper in api.js
- Look at similar pages for patterns

---

## Contact & Updates

For latest updates and support:
- Review documentation files
- Check code comments
- Follow existing patterns
- Test thoroughly before deploying

Good luck with your launch!
