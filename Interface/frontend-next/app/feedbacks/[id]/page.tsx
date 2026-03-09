import { Navigation } from '@/components/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';
import { notFound } from 'next/navigation';
import { cookies } from 'next/headers';

async function getFeedback(id: number) {
    const cookieStore = await cookies();
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Feedback/${id}`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });

    if (res.status === 404) return null;
    if (!res.ok) throw new Error('Failed to fetch');
    
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

export default async function EditFeedbackPage({ params }: { params: Promise<{ id: string }>; }) {
    const { id } = await params;
    const numericId = parseInt(id);
    if (isNaN(numericId)) notFound();

    const [feedback, categories] = await Promise.all([
        getFeedback(numericId),
        getCategories()
    ]);

    if (!feedback) notFound();

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Редактирование отзыва</h1>
                <div className="max-w-2xl">
                    <FeedbackForm
                        initialData={feedback}
                        initialCategories={categories.data || []}
                    />
                </div>
            </main>
        </div>
    );
}