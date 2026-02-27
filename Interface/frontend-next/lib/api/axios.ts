import axios from 'axios';
import https from "https";

const isServer = typeof window === 'undefined';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

export const apiClient = axios.create({
    baseURL: API_URL,
    withCredentials: true,
    headers: {
        'Content-Type': 'application/json',
    },
    ...(isServer && {
        httpsAgent: new https.Agent({
            rejectUnauthorized: false
        })
    })
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            if (typeof window !== 'undefined') {
                window.location.href = '/login';
            }
        }
        return Promise.reject(error);
    }
);