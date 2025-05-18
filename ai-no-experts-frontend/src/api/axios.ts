import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    // Adding withCredentials: true to ensure that cookies are sent with requests if the API requires authentication
    withCredentials: true, 
});

export default axiosInstance;