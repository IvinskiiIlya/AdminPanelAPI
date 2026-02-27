import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
    const token = request.cookies.get('jwtToken')?.value;
    const isAuthPage = request.nextUrl.pathname.startsWith('/login');
    const isPublicPath = request.nextUrl.pathname === '/login' || request.nextUrl.pathname === '/';

    // Если нет токена и страница не публичная - редирект на логин
    if (!token && !isPublicPath) {
        return NextResponse.redirect(new URL('/login', request.url));
    }

    // Если есть токен и мы на странице логина - редирект на главную
    if (token && isAuthPage) {
        return NextResponse.redirect(new URL('/', request.url));
    }

    return NextResponse.next();
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|favicon.ico|.*\\.).*)'],
};