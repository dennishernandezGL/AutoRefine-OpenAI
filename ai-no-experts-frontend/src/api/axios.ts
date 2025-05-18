import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, // Enable sending credentials with requests to ensure proper session handling
});

export default axiosInstance;