import { Navigation } from '@/components/navigation';
import { CategoriesTable } from '@/components/tables/categories-table';
import { Suspense } from 'react';
import { cookies } from 'next/headers';
import { cacheTag } from 'next/cache';

async function getCategories(
    searchParams: {
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
    },
    cookieString: string
) {
    'use cache';
    cacheTag('categories');

    const params = new URLSearchParams();

    if (searchParams.page) params.set('pageNumber', searchParams.page);
    if (searchParams.pageSize) params.set('pageSize', searchParams.pageSize);
    if (searchParams.sortColumn) params.set('sortColumn', searchParams.sortColumn);
    if (searchParams.sortOrder) params.set('sortOrder', searchParams.sortOrder);
    if (searchParams.searchTerm) params.set('searchTerm', searchParams.searchTerm);

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?${params}`, {
        headers: { Cookie: cookieString },
    });

    if (!res.ok) throw new Error('Failed to fetch');
    return res.json();
}

async function CategoriesTableWrapper({ searchParamsPromise }: {
    searchParamsPromise: Promise<{
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
    }>
}) {
    const searchParams = await searchParamsPromise;
    const cookieStore = await cookies();
    const cookieString = cookieStore.toString();

    const data = await getCategories(searchParams, cookieString);

    return (
        <CategoriesTable
            initialData={data}
            initialSearchParams={searchParams}
        />
    );
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
    const searchParamsPromise = searchParams || Promise.resolve({});
    const paramsKey = JSON.stringify(await searchParamsPromise);

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Категории</h1>
                <Suspense key={paramsKey} fallback={<div>Загрузка таблицы...</div>}>
                    <CategoriesTableWrapper searchParamsPromise={searchParamsPromise} />
                </Suspense>
            </main>
        </div>
    );
}