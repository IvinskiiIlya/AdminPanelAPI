import { NextRequest, NextResponse } from 'next/server';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

export async function GET(request: NextRequest, { params }: { params: { id: string } }) {
    const cookie = request.headers.get('cookie') || '';

    const response = await fetch(`${API_URL}/Category/${params.id}`, {
        headers: {
            'Cookie': cookie,
            'Content-Type': 'application/json',
        },
    });

    const data = await response.json();
    
    return NextResponse.json(data, { status: response.status });
}