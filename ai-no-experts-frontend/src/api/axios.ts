import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: '/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true
});

axiosInstance.interceptors.response.use(
    response => response,
    error => {
        if (error.response) {
            switch (error.response.status) {
                case 401:
                    console.error('Unauthorized access - possibly invalid credentials');
                    break;
                case 403:
                    console.error('Forbidden access');
                    break;
                default:
                    console.error('An error occurred:', error.message);
            }
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;