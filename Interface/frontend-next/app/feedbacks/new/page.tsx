import { Navigation } from '@/components/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';
import { Suspense } from 'react';
import { cookies } from 'next/headers';

async function getCategories(cookieString: string) {
    'use cache';

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?pageSize=100`, {
        headers: { Cookie: cookieString },
        next: { tags: ['categories'] }
    });

    if (!res.ok) return { data: [] };
    return res.json();
}

async function FeedbackFormWrapper() {
    const cookieStore = await cookies();
    const cookieString = cookieStore.toString();

    const categories = await getCategories(cookieString);

    return (
        <FeedbackForm initialCategories={categories.data || []} />
    );
}

export default async function NewFeedbackPage() {
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Создание отзыва</h1>
                <div className="max-w-2xl">
                    <Suspense fallback={<div>Загрузка формы...</div>}>
                        <FeedbackFormWrapper />
                    </Suspense>
                </div>
            </main>
        </div>
    );
}