import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: process.env.REACT_APP_API_BASE_URL || '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    },
    withCredentials: true,
});

axiosInstance.interceptors.request.use(config => {
    // You can add token and other configurations here
    return config;
}, error => {
    return Promise.reject(error);
});

axiosInstance.interceptors.response.use(response => {
    return response;
}, error => {
    // Handle global response errors here
    if (error.response && error.response.status === 401) {
        // Handle authorization errors such as redirecting to login
    }
    return Promise.reject(error);
});

export default axiosInstance;
