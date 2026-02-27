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
            throw error;
        }
    },

    logout: async (): Promise<void> => {
        try {
            await apiClient.post('/auth/logout');
        } catch (error) {
            throw error;
        }
    },

    getCurrentUser: async (): Promise<UserInfo> => {
        try {
            const { data } = await apiClient.get('/auth/userinfo');
            return data;
        } catch (error) {
            throw error;
        }
    }
};