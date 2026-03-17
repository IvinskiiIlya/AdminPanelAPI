import { Navigation } from '@/components/navigation';
import { FeedbacksTable } from '@/components/tables/feedbacks-table';
import { Suspense } from 'react';
import { cookies } from 'next/headers';

async function getFeedbacks(
    searchParams: {
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
        categoryId?: string;
        statusId?: string;
    },
    cookieString: string
) {
    'use cache';

    const params = new URLSearchParams();

    if (searchParams.page) params.set('PageNumber', searchParams.page);
    if (searchParams.pageSize) params.set('PageSize', searchParams.pageSize);
    if (searchParams.sortColumn) params.set('SortColumn', searchParams.sortColumn);
    if (searchParams.sortOrder) params.set('SortOrder', searchParams.sortOrder);
    if (searchParams.searchTerm) params.set('SearchTerm', searchParams.searchTerm);
    if (searchParams.categoryId) params.set('CategoryId', searchParams.categoryId);
    if (searchParams.statusId) params.set('StatusId', searchParams.statusId);

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Feedback?${params}`, {
        headers: { Cookie: cookieString },
        next: { tags: ['feedbacks'] }
    });

    if (!res.ok) throw new Error('Failed to fetch');
    return res.json();
}

async function getCategories(cookieString: string) {
    'use cache';

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?pageSize=100`, {
        headers: { Cookie: cookieString },
        next: { tags: ['categories'] }
    });

    if (!res.ok) return { data: [] };
    return res.json();
}

async function getStatuses(cookieString: string) {
    'use cache';

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Status`, {
        headers: { Cookie: cookieString },
        next: { tags: ['statuses'] }
    });

    if (!res.ok) return [];

    const data = await res.json();

    if (data && Array.isArray(data.data)) {
        return data.data;
    }

    return [];
}

async function FeedbacksTableWrapper({ searchParams }: {
    searchParams: Promise<{
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
        categoryId?: string;
        statusId?: string;
    }>
}) {
    const unwrappedSearchParams = await searchParams || {};
    const cookieStore = await cookies();
    const cookieString = cookieStore.toString();

    const [feedbacks, categories, statuses] = await Promise.all([
        getFeedbacks(unwrappedSearchParams, cookieString),
        getCategories(cookieString),
        getStatuses(cookieString)
    ]);

    const statusesArray = Array.isArray(statuses) ? statuses : [];

    return (
        <FeedbacksTable
            initialData={feedbacks}
            initialSearchParams={unwrappedSearchParams}
            initialCategories={categories.data || []}
            initialStatuses={statusesArray}
        />
    );
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
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Отзывы</h1>
                <Suspense fallback={<div>Загрузка таблицы...</div>}>
                    <FeedbacksTableWrapper searchParams={searchParams || Promise.resolve({})} />
                </Suspense>
            </main>
        </div>
    );
}