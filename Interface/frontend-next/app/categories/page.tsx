import { Navigation } from '@/components/navigation';
import { CategoriesTable } from '@/components/tables/categories-table';
import { Suspense } from 'react';
import { cookies } from 'next/headers';

async function getCategories(searchParams: {
    page?: string;
    pageSize?: string;
    sortColumn?: string;
    sortOrder?: string;
    searchTerm?: string;
}) {
    const cookieStore = await cookies();
    const params = new URLSearchParams();

    if (searchParams.page) params.set('pageNumber', searchParams.page);
    if (searchParams.pageSize) params.set('pageSize', searchParams.pageSize);
    if (searchParams.sortColumn) params.set('sortColumn', searchParams.sortColumn);
    if (searchParams.sortOrder) params.set('sortOrder', searchParams.sortOrder);
    if (searchParams.searchTerm) params.set('searchTerm', searchParams.searchTerm);

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?${params}`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });

    if (!res.ok) throw new Error('Failed to fetch');
    return res.json();
}

export default async function CategoriesPage({ searchParams }: {
    searchParams?: Promise<{
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
    }>;
}) {
    const unwrappedSearchParams = await searchParams || {};
    const data = await getCategories(unwrappedSearchParams);

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Категории</h1>
                <Suspense fallback={<div>Загрузка таблицы...</div>}>
                    <CategoriesTable
                        initialData={data}
                        initialSearchParams={unwrappedSearchParams}
                    />
                </Suspense>
            </main>
        </div>
    );
}