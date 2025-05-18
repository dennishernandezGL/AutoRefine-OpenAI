import axios from 'axios';
import axiosRetry from 'axios-retry';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Configure retry mechanism on the axios instance
axiosRetry(axiosInstance, { retries: 3 });

export default axiosInstance;
