'use server';

import { revalidateTag } from 'next/cache';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

export async function deleteCategory(id: number) {
    const cookieStore = await cookies();

    const response = await fetch(`${API_URL}/Category/${id}`, {
        method: 'DELETE',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to delete category');
    }

    revalidateTag('categories', 'max');
}

export async function createCategory(formData: FormData) {
    const cookieStore = await cookies();

    const data = {
        id: parseInt(formData.get('id') as string),
        name: formData.get('name'),
        description: formData.get('description'),
    };

    const response = await fetch(`${API_URL}/Category`, {
        method: 'POST',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to create category');
    }

    revalidateTag('categories', 'max');
    redirect('/categories');
}

export async function updateCategory(id: number, formData: FormData) {
    const cookieStore = await cookies();

    const data = {
        id,
        name: formData.get('name'),
        description: formData.get('description'),
    };

    const response = await fetch(`${API_URL}/Category/${id}`, {
        method: 'PUT',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to update category');
    }

    revalidateTag('categories', 'max');
    redirect('/categories');
}