import { apiClient } from './axios';
import { Feedback, FilterFeedbackDto, PagedResponse } from '@/types';
import { authApi } from './auth';

export const feedbackApi = {
    getAll: async (params?: FilterFeedbackDto): Promise<PagedResponse<Feedback>> => {
        const apiParams: any = {};
        if (params) {
            if (params.pageNumber) apiParams.pageNumber = params.pageNumber;
            if (params.pageSize) apiParams.pageSize = params.pageSize;
            if (params.sortColumn) apiParams.sortColumn = params.sortColumn;
            if (params.sortOrder) apiParams.sortOrder = params.sortOrder;
            if (params.searchTerm) apiParams.searchTerm = params.searchTerm;
            if (params.categoryId) apiParams.categoryId = params.categoryId;
            if (params.statusId) apiParams.statusId = params.statusId;
        }

        const { data } = await apiClient.get('/Feedback', { params: apiParams });
        return data;
    },

    getById: async (id: number): Promise<Feedback> => {
        const { data } = await apiClient.get(`/Feedback/${id}`);
        return data;
    },

    create: async (dto: { categoryId: number; message: string }): Promise<Feedback> => {
        console.log('Creating feedback with DTO:', dto);
        try {
            const userInfo = await authApi.getCurrentUser();
            console.log('Current user for feedback:', userInfo);

            const payload = {
                ...dto,
                userId: userInfo.id,
                statusId: 1
            };

            console.log('Sending payload to server:', payload);

            const { data } = await apiClient.post('/Feedback', payload);
            console.log('Create response:', data);
            return data;
            
        } catch (error) {
            console.error('Create error details:', error);
            throw error;
        }
    },

    update: async (id: number, dto: { categoryId: number; message: string }): Promise<void> => {
        console.log('Updating feedback:', { id, ...dto });
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
            console.error('Update error:', error);
            throw error;
        }
    },

    delete: async (id: number): Promise<void> => {
        try {
            await apiClient.delete(`/Feedback/${id}`);
        } catch (error) {
            console.error('Delete error:', error);
            throw error;
        }
    },
};