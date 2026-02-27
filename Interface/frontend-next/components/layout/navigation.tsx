'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import {
    Home,
    FolderTree,
    Paperclip,
    MessageSquare,
    Users,
    Tag,
    Star,
    LogOut
} from 'lucide-react';
import { toast } from 'sonner';
import { logout } from '@/app/logout';

const routes = [
    { href: '/', label: 'Главная', icon: Home },
    { href: '/categories', label: 'Категории', icon: FolderTree },
    { href: '/feedbacks', label: 'Отзывы', icon: MessageSquare },
    { href: '/attachments', label: 'Вложения', icon: Paperclip },
    { href: '/users', label: 'Пользователи', icon: Users },
    { href: '/statuses', label: 'Статусы', icon: Tag },
    { href: '/roles', label: 'Роли', icon: Star },
];

export function Navigation() {
    const pathname = usePathname();

    const handleLogout = async () => {
        try {
            await logout();
            toast.success('Выход выполнен');
        } catch (error) {
            console.error('Logout error:', error);
            document.cookie = 'jwtToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
            window.location.href = '/login';
        }
    };

    return (
        <nav className="border-b">
            <div className="flex h-16 items-center px-8">
                <div className="flex gap-6">
                    {routes.map((route) => {
                        const Icon = route.icon;
                        return (
                            <Link
                                key={route.href}
                                href={route.href}
                                className={cn(
                                    'flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary',
                                    pathname === route.href
                                        ? 'text-black dark:text-white'
                                        : 'text-muted-foreground'
                                )}
                            >
                                <Icon className="h-4 w-4" />
                                {route.label}
                            </Link>
                        );
                    })}
                </div>
                <div className="ml-auto flex items-center gap-4">
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={handleLogout}
                        className="flex items-center gap-2 text-red-600 hover:text-red-700 hover:bg-red-50"
                    >
                        <LogOut className="h-4 w-4" />
                        Выйти
                    </Button>
                </div>
            </div>
        </nav>
    );
}