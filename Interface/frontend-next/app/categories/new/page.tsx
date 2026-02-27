import { Navigation } from '@/components/layout/navigation';
import { CategoryForm } from '@/components/forms/category-form';

export default function NewCategoryPage() {
    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Создание категории</h1>
                <div className="max-w-2xl">
                    <CategoryForm />
                </div>
            </main>
        </div>
    );
}