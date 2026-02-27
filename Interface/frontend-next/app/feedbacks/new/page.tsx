import { Navigation } from '@/components/layout/navigation';
import { FeedbackForm } from '@/components/forms/feedback-form';

export default function NewFeedbackPage() {
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Создание отзыва</h1>
                <div className="max-w-2xl">
                    <FeedbackForm />
                </div>
            </main>
        </div>
    );
}