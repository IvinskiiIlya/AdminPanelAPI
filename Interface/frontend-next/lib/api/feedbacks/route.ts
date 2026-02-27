import { NextRequest, NextResponse } from 'next/server';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

export async function GET(request: NextRequest) {
    const searchParams = request.nextUrl.searchParams;
    const cookie = request.headers.get('cookie') || '';

    const response = await fetch(`${API_URL}/Feedback?${searchParams}`, {
        headers: {
            'Cookie': cookie,
            'Content-Type': 'application/json',
        },
    });

    const data = await response.json();
    return NextResponse.json(data, { status: response.status });
}

export async function POST(request: NextRequest) {
    const body = await request.json();
    const cookie = request.headers.get('cookie') || '';

    console.log('Proxying POST to backend:', body);

    const response = await fetch(`${API_URL}/Feedback`, {
        method: 'POST',
        headers: {
            'Cookie': cookie,
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
    });

    const data = await response.json();
    return NextResponse.json(data, { status: response.status });
}