import { NextRequest, NextResponse } from 'next/server';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7109/api';

export async function GET(request: NextRequest, { params }: { params: { id: string } }) {
    const cookie = request.headers.get('cookie') || '';

    const response = await fetch(`${API_URL}/Feedback/${params.id}`, {
        headers: {
            'Cookie': cookie,
            'Content-Type': 'application/json',
        },
    });

    const data = await response.json();
    
    return NextResponse.json(data, { status: response.status });
}

export async function PUT(request: NextRequest, { params }: { params: { id: string } }) {
    const body = await request.json();
    const cookie = request.headers.get('cookie') || '';
    
    const response = await fetch(`${API_URL}/Feedback/${params.id}`, {
        method: 'PUT',
        headers: {
            'Cookie': cookie,
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
    });

    if (response.status === 204) {
        return new NextResponse(null, { status: 204 });
    }

    const data = await response.json();
    
    return NextResponse.json(data, { status: response.status });
}

export async function DELETE(request: NextRequest, { params }: { params: { id: string } }) {
    const cookie = request.headers.get('cookie') || '';

    const response = await fetch(`${API_URL}/Feedback/${params.id}`, {
        method: 'DELETE',
        headers: {
            'Cookie': cookie,
        },
    });

    if (response.status === 204) {
        return new NextResponse(null, { status: 204 });
    }

    const data = await response.json();
    
    return NextResponse.json(data, { status: response.status });
}