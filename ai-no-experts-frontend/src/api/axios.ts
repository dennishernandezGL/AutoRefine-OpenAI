import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:5050/api',
    timeout: 10000,
    headers: {
        'Content-Length': 'application/json',
    },
});

export default axiosInstance;