import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true // Allow sending cookies with requests
});

// Adding response interceptor for error logging
axiosInstance.interceptors.response.use(
    response => response,
    error => {
        console.error('API call failed:', error);
        return Promise.reject(error);
    }
);

export default axiosInstance;