import { apiClient } from './axios';

export const authApi = {
    login: async (email: string, password: string): Promise<void> => {
        try {
            await apiClient.post('/auth/login', { email, password });
        } catch (error) {
            throw error;
        }
    },
};