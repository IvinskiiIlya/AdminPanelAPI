import { apiClient } from './axios';

export interface User {
    id: string;
    userName: string;
    email: string;
    roles?: string[];
}

export const userApi = {
    getAll: async (params?: any): Promise<User[]> => {
        const { data } = await apiClient.get('/User', { params });
        return data;
    },

    getById: async (id: string): Promise<User> => {
        const { data } = await apiClient.get(`/User/${id}`);
        return data;
    },
};