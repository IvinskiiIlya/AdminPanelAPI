import { apiClient } from '../axios';
import { Category, CreateCategoryDto, PagedResponse, PaginationParams } from '@/index';

export const categoryApi = {
    getAll: async (params?: PaginationParams): Promise<PagedResponse<Category>> => {
        const apiParams: any = {};

        if (params) {
            if (params.pageNumber) apiParams.pageNumber = params.pageNumber;
            if (params.pageSize) apiParams.pageSize = params.pageSize;
            if (params.sortColumn) apiParams.sortColumn = params.sortColumn;
            if (params.sortOrder) apiParams.sortOrder = params.sortOrder;
            if (params.searchTerm) apiParams.searchTerm = params.searchTerm;
        }

        const { data } = await apiClient.get('/Category', { params: apiParams });
        
        return data;
    },

    getById: async (id: number): Promise<Category> => {
        const { data } = await apiClient.get(`/Category/${id}`);
        return data;
    },

    create: async (dto: CreateCategoryDto): Promise<Category> => {
        const { data } = await apiClient.post('/Category', dto);
        return data;
    },

    update: async (id: number, dto: Omit<CreateCategoryDto, 'id'>): Promise<void> => {
        await apiClient.put(`/Category/${id}`, { ...dto, id });
    },

    delete: async (id: number): Promise<void> => {
        await apiClient.delete(`/Category/${id}`);
    },
};