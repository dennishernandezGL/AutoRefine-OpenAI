import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:3000',
    timeout: 10000,
    headers: {
        'Content-Length': 'application/json',
    },
});

export default axiosInstance;