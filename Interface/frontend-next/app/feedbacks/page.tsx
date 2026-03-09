import { Navigation } from '@/components/navigation';
import { FeedbacksTable } from '@/components/tables/feedbacks-table';
import { Suspense } from 'react';
import { cookies } from 'next/headers';

async function getFeedbacks(searchParams: {
    page?: string,
    pageSize?: string,
    sortColumn?: string;
    sortOrder?: string;
    searchTerm?: string;
    categoryId?: string;
    statusId?: string;
}) {
    const cookieStore = await cookies();
    const params = new URLSearchParams();

    if (searchParams.page) params.set('PageNumber', searchParams.page);
    if (searchParams.pageSize) params.set('PageSize', searchParams.pageSize);
    if (searchParams.sortColumn) params.set('SortColumn', searchParams.sortColumn);
    if (searchParams.sortOrder) params.set('SortOrder', searchParams.sortOrder);
    if (searchParams.searchTerm) params.set('SearchTerm', searchParams.searchTerm);
    if (searchParams.categoryId) params.set('CategoryId', searchParams.categoryId);
    if (searchParams.statusId) params.set('StatusId', searchParams.statusId);
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Feedback?${params}`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });
    
    if (!res.ok) {
        throw new Error('Failed to fetch');
    }
    
    return res.json();
}

async function getCategories() {
    const cookieStore = await cookies();
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?pageSize=100`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });
    
    if (!res.ok) {
        return { data: [] };
    }
    
    return res.json();
}

async function getStatuses() {
    const cookieStore = await cookies();
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Status`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });
    
    if (!res.ok) {
        return [];
    }
    
    return res.json();
}

export default async function FeedbacksPage({ searchParams }: {
    searchParams?: Promise<{
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
        categoryId?: string;
        statusId?: string;
    }>;
}) {
    const unwrappedSearchParams = await searchParams || {};
    
    const [feedbacks, categories, statuses] = await Promise.all([
        getFeedbacks(unwrappedSearchParams),
        getCategories(),
        getStatuses()
    ])

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Отзывы</h1>
                <Suspense fallback={<div>Загрузка таблицы...</div>}>
                    <FeedbacksTable
                        initialData={feedbacks}
                        initialSearchParams={unwrappedSearchParams}
                        initialCategories={categories.data || []}
                        initialStatuses={statuses}
                    />
                </Suspense>
            </main>
        </div>
    );
}