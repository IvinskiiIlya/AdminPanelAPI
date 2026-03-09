import { Navigation } from '@/components/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';
import { cookies } from 'next/headers';

async function getCategories() {
    const cookieStore = await cookies();
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category?pageSize=100`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });

    if (!res.ok) return { data: [] };
    return res.json();
}

export default async function NewFeedbackPage() {
    const categories = await getCategories();

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Создание отзыва</h1>
                <div className="max-w-2xl">
                    <FeedbackForm initialCategories={categories.data || []} />
                </div>
            </main>
        </div>
    );
}