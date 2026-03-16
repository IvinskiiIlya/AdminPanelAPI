import { Navigation } from '@/components/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';
import { notFound } from 'next/navigation';
import { Suspense } from 'react';
import { cookies } from 'next/headers';

async function getFeedback(id: number, cookieString: string) {
    'use cache';

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Feedback/${id}`, {
        headers: { Cookie: cookieString },
        next: { tags: [`feedback-${id}`] }
    });

    if (res.status === 404) return null;
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

async function FeedbackFormWrapper({ id }: { id: number }) {
    const cookieStore = await cookies();
    const cookieString = cookieStore.toString();

    const [feedback, categories] = await Promise.all([
        getFeedback(id, cookieString),
        getCategories(cookieString)
    ]);

    if (!feedback) notFound();

    return (
        <FeedbackForm
            initialData={feedback}
            initialCategories={categories.data || []}
        />
    );
}

export default async function EditFeedbackPage({ params }: { params: Promise<{ id: string }> }) {
    const { id } = await params;
    const numericId = parseInt(id);
    if (isNaN(numericId)) notFound();

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Редактирование отзыва</h1>
                <div className="max-w-2xl">
                    <Suspense fallback={<div>Загрузка формы...</div>}>
                        <FeedbackFormWrapper id={numericId} />
                    </Suspense>
                </div>
            </main>
        </div>
    );
}