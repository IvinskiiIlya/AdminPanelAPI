'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Navigation } from '@/components/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';
import { feedbackApi } from '@/lib/api/feedbacks/feedbacks';
import { Feedback } from '@/index';
import { toast } from 'sonner';

export default function EditFeedbackPage() {
    const params = useParams();
    const router = useRouter();
    const [feedback, setFeedback] = useState<Feedback | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchFeedback = async () => {
            try {
                const id = parseInt(params.id as string);
                const data = await feedbackApi.getById(id);
                setFeedback(data);
                
            } catch (error) {
                toast.error('Ошибка при загрузке отзыва');
                router.push('/feedbacks');
                
            } finally {
                setLoading(false);
            }
        };

        fetchFeedback();
    }, [params.id, router]);

    if (loading) {
        return (
            <div>
                <Navigation />
                <main className="p-8">
                    <div className="text-center py-8">Загрузка...</div>
                </main>
            </div>
        );
    }

    if (!feedback) {
        return (
            <div>
                <Navigation />
                <main className="p-8">
                    <div className="text-center py-8">Отзыв не найден</div>
                </main>
            </div>
        );
    }

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Редактирование отзыва</h1>
                <div className="max-w-2xl">
                    <FeedbackForm initialData={feedback} />
                </div>
            </main>
        </div>
    );
}