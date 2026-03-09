import { Navigation } from '@/components/navigation';
import { CategoryForm } from '@/components/forms/category-form';
import { notFound } from 'next/navigation';
import { cookies } from 'next/headers';

async function getCategory(id: number) {
    const cookieStore = await cookies();
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/Category/${id}`, {
        headers: { Cookie: cookieStore.toString() },
        cache: 'no-store'
    });

    if (res.status === 404) return null;
    if (!res.ok) throw new Error('Failed to fetch');
    return res.json();
}

export default async function EditCategoryPage({ params }: { params: Promise<{ id: string }>; }) {
    const { id } = await params;
    const numericId = parseInt(id);
    if (isNaN(numericId)) notFound();

    const category = await getCategory(numericId);
    if (!category) notFound();

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