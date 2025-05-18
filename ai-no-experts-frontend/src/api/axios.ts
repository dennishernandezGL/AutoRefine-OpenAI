import axios from 'axios';

// Axios instance creation with enhanced security headers
const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest',
        // Ensure all requests have CSRF protection
        'X-CSRF-Token': getCsrfToken(),
        // Adding other security headers if needed
        'X-Content-Type-Options': 'nosniff'
    },
});

// Extracted function to retrieve CSRF token
function getCsrfToken() {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta && meta.getAttribute('content');
}

export default axiosInstance;