import { Navigation } from '@/components/navigation';
import { FeedbacksTable } from '@/components/feedbacks-table';
import { Suspense } from 'react';

export default function FeedbacksPage() {
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Отзывы</h1>
                <Suspense fallback={<div>Загрузка таблицы...</div>}>
                    <FeedbacksTable />
                </Suspense>
            </main>
        </div>
    );
}