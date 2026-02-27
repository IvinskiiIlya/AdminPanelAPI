'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Navigation } from '@/components/layout/navigation';
import { CategoryForm } from '@/components/forms/category-form';
import { categoryApi } from '@/lib/api/categories';
import { Category } from '@/types';
import { toast } from 'sonner';

export default function EditCategoryPage() {
    const params = useParams();
    const router = useRouter();
    const [category, setCategory] = useState<Category | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCategory = async () => {
            try {
                const id = parseInt(params.id as string);
                const data = await categoryApi.getById(id);
                setCategory(data);
            } catch (error) {
                console.error('Error fetching category:', error);
                toast.error('Ошибка при загрузке категории');
                router.push('/categories');
            } finally {
                setLoading(false);
            }
        };

        fetchCategory();
    }, [params.id, router]);

    if (loading) {
        return (
            <div>
                <Navigation />
                <main className="p-8">
                    <div>Загрузка...</div>
                </main>
            </div>
        );
    }

    if (!category) {
        return (
            <div>
                <Navigation />
                <main className="p-8">
                    <div>Категория не найдена</div>
                </main>
            </div>
        );
    }

    return (
        <div>
            <Navigation />
            <main className="p-8">
                <h1 className="text-3xl font-bold mb-8">Редактирование категории</h1>
                <div className="max-w-2xl">
                    <CategoryForm initialData={category} />
                </div>
            </main>
        </div>
    );
}