import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, // Allow sending cookies with requests
});

axiosInstance.interceptors.response.use(
    response => response,
    error => {
        if (error.response && error.response.status === 401) {
            // Handle unauthorized error, e.g., redirect to login
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;