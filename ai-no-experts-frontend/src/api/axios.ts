import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
        'X-CSRF-Token': '<CSRF_TOKEN>'  // Include CSRF token for request validation
        'X-Content-Type-Options': 'nosniff', // Mitigate MIME type sniffing
        'Content-Security-Policy': "default-src 'self';" // Define valid sources for loading content
    },
});

// Optionally, add an interceptor to handle CSRF tokens or refresh them
axiosInstance.interceptors.request.use(config => {
    // Assume fetchCSRFToken() retrieves the CSRF token
    config.headers['X-CSRF-Token'] = fetchCSRFToken();
    return config;
}, error => {
    return Promise.reject(error);
});

export default axiosInstance;

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

export default axiosInstance;