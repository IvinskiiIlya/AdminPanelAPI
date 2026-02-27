export interface PaginationParams {
    pageNumber?: number;
    pageSize?: number;
    sortColumn?: string;
    sortOrder?: 'asc' | 'desc';
    searchTerm?: string;
}

export interface PagedResponse<T> {
    data: T[];
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalRecords: number;
}

export interface Category {
    id: number;
    name: string;
    description?: string | null;
}

export interface CreateCategoryDto {
    id: number;
    name: string;
    description?: string | null;
}

export interface Status {
    id: number;
    name: string;
}

export interface Feedback {
    id: number;
    userId: string;
    categoryId: number;
    statusId: number;
    message: string;
    createdAt: string;
    categoryName?: string;
    statusName?: string;
    userName?: string;
}

export interface CreateFeedbackDto {
    userId: string;
    categoryId: number;
    statusId: number;
    message: string;
}

export interface UpdateFeedbackDto {
    id: number;
    userId: string;
    categoryId: number;
    statusId: number;
    message: string;
}

export interface FilterFeedbackDto extends PaginationParams {
    userId?: string;
    categoryId?: number;
    statusId?: number;
    message?: string;
    createdFrom?: string;
    createdTo?: string;
    searchTerm?: string;
}