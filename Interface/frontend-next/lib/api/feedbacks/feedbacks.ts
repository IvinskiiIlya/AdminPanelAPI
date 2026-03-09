import { apiClient } from '../axios';
import { Feedback, FilterFeedbackDto, PagedResponse } from '@/index';
import { authApi } from '../auth';

export const feedbackApi = {
    getAll: async (params?: FilterFeedbackDto): Promise<PagedResponse<Feedback>> => {
        try {
            const apiParams: any = {};

            if (params) {
                if (params.pageNumber !== undefined) apiParams.pageNumber = params.pageNumber;
                if (params.pageSize !== undefined) apiParams.pageSize = params.pageSize;
                if (params.sortColumn) apiParams.sortColumn = params.sortColumn;
                if (params.sortOrder) apiParams.sortOrder = params.sortOrder;
                if (params.searchTerm) apiParams.searchTerm = params.searchTerm;
                if (params.categoryId !== undefined) apiParams.categoryId = params.categoryId;
                if (params.statusId !== undefined) apiParams.statusId = params.statusId;
            }

            const { data } = await apiClient.get('/Feedback', { params: apiParams });

            return data;

        } catch (error) {
            console.error('API error in getAll:', error);
            throw error;
        }
    },

    getById: async (id: number): Promise<Feedback> => {
        try {
            const { data } = await apiClient.get(`/Feedback/${id}`);
            return data;
        } catch (error) {
            console.error('API error in getById:', error);
            throw error;
        }
    },

    create: async (dto: { categoryId: number; message: string }): Promise<Feedback> => {
        try {
            const userInfo = await authApi.getCurrentUser();

            const payload = {
                ...dto,
                userId: userInfo.id,
                statusId: 1
            };

            const { data } = await apiClient.post('/Feedback', payload);
            return data;

        } catch (error) {
            console.error('API error in create:', error);
            throw error;
        }
    },

    update: async (id: number, dto: { categoryId: number; message: string }): Promise<void> => {
        try {
            const userInfo = await authApi.getCurrentUser();

            const payload = {
                ...dto,
                id,
                userId: userInfo.id,
                statusId: 1
            };

            await apiClient.put(`/Feedback/${id}`, payload);

        } catch (error) {
            console.error('API error in update:', error);
            throw error;
        }
    },

    delete: async (id: number): Promise<void> => {
        try {
            await apiClient.delete(`/Feedback/${id}`);
        } catch (error) {
            console.error('API error in delete:', error);
            throw error;
        }
    },
};