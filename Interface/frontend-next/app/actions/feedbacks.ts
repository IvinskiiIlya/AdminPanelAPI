'use server';

import { revalidateTag } from 'next/cache';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

function getUserIdFromToken(token: string): string | null {
    try {
        const payload = token.split('.')[1];
        const decodedPayload = JSON.parse(atob(payload));

        const userId = decodedPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

        return userId || null;
    } catch (error) {
        return null;
    }
}

export async function deleteFeedback(id: number) {
    const cookieStore = await cookies();

    const response = await fetch(`${API_URL}/Feedback/${id}`, {
        method: 'DELETE',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
    });

    if (!response.ok) {
        const error = await response.json().catch(() => ({ message: 'Failed to delete feedback' }));
        throw new Error(error.message || 'Failed to delete feedback');
    }

    revalidateTag('feedbacks', 'max');
}

export async function createFeedback(formData: FormData) {
    const cookieStore = await cookies();

    const token = cookieStore.get('jwtToken')?.value;

    if (!token) {
        return { error: 'User not authenticated' };
    }

    const userId = getUserIdFromToken(token);

    if (!userId) {
        return { error: 'Invalid token - user ID not found' };
    }

    const categoryId = parseInt(formData.get('categoryId') as string);
    const message = formData.get('message') as string;
    
    const data = {
        categoryId,
        message,
        userId,
    };

    const response = await fetch(`${API_URL}/Feedback`, {
        method: 'POST',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const responseText = await response.text();

        try {
            const errorJson = JSON.parse(responseText);
            return { error: errorJson.message || errorJson.title || JSON.stringify(errorJson) };
        } catch {
            return { error: responseText || 'Failed to create feedback' };
        }
    }

    revalidateTag('feedbacks', 'max');
    redirect('/feedbacks');
}

export async function updateFeedback(id: number, formData: FormData) {
    const cookieStore = await cookies();

    const token = cookieStore.get('jwtToken')?.value;

    if (!token) {
        return { error: 'User not authenticated' };
    }

    const userId = getUserIdFromToken(token);

    if (!userId) {
        return { error: 'Invalid token - user ID not found' };
    }

    const categoryId = parseInt(formData.get('categoryId') as string);
    const message = formData.get('message') as string;
    
    const data = {
        id,
        categoryId,
        message,
        userId,
    };

    const response = await fetch(`${API_URL}/Feedback/${id}`, {
        method: 'PUT',
        headers: {
            'Cookie': cookieStore.toString(),
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const responseText = await response.text();

        try {
            const errorJson = JSON.parse(responseText);
            return { error: errorJson.message || errorJson.title || JSON.stringify(errorJson) };
        } catch {
            return { error: responseText || 'Failed to update feedback' };
        }
    }

    revalidateTag('feedbacks', 'max');
    redirect('/feedbacks');
}