import { Navigation } from '@/components/navigation';
import { CategoriesTable } from '@/components/categories-table';
import { Suspense } from 'react';

export default function CategoriesPage() {
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Категории</h1>
                <Suspense fallback={<div>Загрузка таблицы...</div>}>
                    <CategoriesTable />
                </Suspense>
            </main>
        </div>
    );
}