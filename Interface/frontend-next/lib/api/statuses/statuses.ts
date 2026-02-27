import { apiClient } from '../axios';
import { Status, PagedResponse } from '@/index';

export const statusApi = {
    getAll: async (): Promise<Status[]> => {
        const { data } = await apiClient.get('/Status');
        return Array.isArray(data) ? data : data.data || [];
    },
};