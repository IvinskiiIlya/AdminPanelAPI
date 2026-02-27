import { apiClient } from './axios';

export interface UserInfo {
    id: string;
    name: string;
    roles: string[];
}

export const authApi = {
    login: async (email: string, password: string): Promise<void> => {
        try {
            await apiClient.post('/auth/login', { email, password });
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    },

    logout: async (): Promise<void> => {
        try {
            await apiClient.post('/auth/logout');
        } catch (error) {
            console.error('Logout API error:', error);
        }
    },

    getCurrentUser: async (): Promise<UserInfo> => {
        try {
            const { data } = await apiClient.get('/auth/userinfo');
            console.log('Current user info:', data);
            return data;
        } catch (error) {
            console.error('Get user info error:', error);
            throw error;
        }
    }
};