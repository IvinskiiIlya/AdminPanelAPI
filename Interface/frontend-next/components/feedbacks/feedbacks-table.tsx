'use client';

import { useEffect, useState, useCallback } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { MoreHorizontal, Pencil, Trash, Plus, ArrowUpDown, Search, X, Filter } from 'lucide-react';
import { toast } from 'sonner';
import { formatDate } from '@/lib/utils';
import { feedbackApi } from '@/lib/api/feedbacks';
import { categoryApi } from '@/lib/api/categories';
import { statusApi } from '@/lib/api/statuses';
import { Feedback, Category, Status } from '@/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';

export function FeedbacksTable() {
    const router = useRouter();
    const searchParams = useSearchParams();

    const [data, setData] = useState<Feedback[]>([]);
    const [loading, setLoading] = useState(true);
    const [totalPages, setTotalPages] = useState(1);
    const [totalRecords, setTotalRecords] = useState(0);

    const [categories, setCategories] = useState<Category[]>([]);
    const [statuses, setStatuses] = useState<Status[]>([]);
    const [loadingFilters, setLoadingFilters] = useState(true);

    const pageNumber = Number(searchParams.get('page')) || 1;
    const pageSize = Number(searchParams.get('pageSize')) || 10;
    const sortColumn = searchParams.get('sortColumn') || 'id';
    const sortOrderParam = searchParams.get('sortOrder');
    const sortOrder: 'asc' | 'desc' = sortOrderParam === 'desc' ? 'desc' : 'asc';
    const searchTerm = searchParams.get('searchTerm') || '';
    const categoryFilter = searchParams.get('categoryId');
    const statusFilter = searchParams.get('statusId');

    const [localSearch, setLocalSearch] = useState(searchTerm);
    const [localCategory, setLocalCategory] = useState(categoryFilter || 'all');
    const [localStatus, setLocalStatus] = useState(statusFilter || 'all');
    const [showFilters, setShowFilters] = useState(false);

    useEffect(() => {
        const loadFilters = async () => {
            try {
                setLoadingFilters(true);
                const [categoriesData, statusesData] = await Promise.all([
                    categoryApi.getAll({ pageSize: 100 }),
                    statusApi.getAll()
                ]);
                setCategories(categoriesData.data || []);
                setStatuses(statusesData);
            } catch (error) {
                console.error('Error loading filters:', error);
                toast.error('Ошибка при загрузке фильтров');
            } finally {
                setLoadingFilters(false);
            }
        };
        loadFilters();
    }, []);

    const updateParams = useCallback((updates: Record<string, string | number | null>) => {
        const params = new URLSearchParams(searchParams.toString());

        Object.entries(updates).forEach(([key, value]) => {
            if (value === null || value === '' || value === 'all') {
                params.delete(key);
            } else {
                params.set(key, String(value));
            }
        });

        router.push(`/feedbacks?${params.toString()}`);
    }, [router, searchParams]);

    const fetchData = useCallback(async () => {
        try {
            setLoading(true);

            const params = {
                pageNumber,
                pageSize,
                sortColumn,
                sortOrder,
                searchTerm: searchTerm || undefined,
                categoryId: categoryFilter ? parseInt(categoryFilter) : undefined,
                statusId: statusFilter ? parseInt(statusFilter) : undefined
            };

            console.log('Fetching feedbacks with params:', params);

            const response = await feedbackApi.getAll(params);

            const enrichedData = response.data.map(item => ({
                ...item,
                categoryName: categories.find(c => c.id === item.categoryId)?.name,
                statusName: statuses.find(s => s.id === item.statusId)?.name
            }));

            setData(enrichedData);
            setTotalPages(response.totalPages);
            setTotalRecords(response.totalRecords);
            
        } catch (error) {
            console.error('Error fetching feedbacks:', error);
            toast.error('Ошибка при загрузке отзывов');
            setData([]);
            
        } finally {
            setLoading(false);
        }
    }, [pageNumber, pageSize, sortColumn, sortOrder, searchTerm, categoryFilter, statusFilter, categories, statuses]);

    useEffect(() => {
        if (categories.length > 0 && statuses.length > 0) {
            fetchData();
        }
    }, [fetchData, categories, statuses]);

    const handleSearch = () => {
        updateParams({ searchTerm: localSearch || null, page: 1 });
    };

    const clearSearch = () => {
        setLocalSearch('');
        updateParams({ searchTerm: null, page: 1 });
    };

    const applyFilters = () => {
        updateParams({
            categoryId: localCategory !== 'all' ? localCategory : null,
            statusId: localStatus !== 'all' ? localStatus : null,
            page: 1
        });
        setShowFilters(false);
    };

    const clearFilters = () => {
        setLocalCategory('all');
        setLocalStatus('all');
        updateParams({
            categoryId: null,
            statusId: null,
            page: 1
        });
    };

    const handleSort = (column: string) => {
        const newOrder: 'asc' | 'desc' =
            sortColumn === column && sortOrder === 'asc' ? 'desc' : 'asc';
        updateParams({ sortColumn: column, sortOrder: newOrder, page: 1 });
    };

    const handlePageChange = (newPage: number) => {
        updateParams({ page: newPage });
    };

    const handlePageSizeChange = (newSize: string) => {
        updateParams({ pageSize: parseInt(newSize), page: 1 });
    };

    const handleDelete = async (id: number) => {
        if (confirm('Вы уверены, что хотите удалить этот отзыв?')) {
            try {
                await feedbackApi.delete(id);
                toast.success('Отзыв удален');
                fetchData();
            } catch (error) {
                console.error('Error deleting feedback:', error);
                toast.error('Ошибка при удалении');
            }
        }
    };

    const getStatusBadge = (statusName?: string) => {
        switch (statusName?.toLowerCase()) {
            case 'новый':
                return <Badge variant="default">Новый</Badge>;
            case 'в обработке':
                return <Badge variant="secondary">В обработке</Badge>;
            case 'решен':
                return <Badge variant="default" className="bg-green-500 hover:bg-green-600">Решен</Badge>;
            case 'отклонен':
                return <Badge variant="destructive">Отклонен</Badge>;
            default:
                return <Badge variant="outline">{statusName || 'Неизвестно'}</Badge>;
        }
    };

    return (
        <div className="space-y-4">
            <div className="flex flex-col gap-4">
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                    <div className="flex flex-1 items-center gap-2 max-w-md">
                        <div className="relative flex-1">
                            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                            <Input
                                placeholder="Поиск по сообщению..."
                                value={localSearch}
                                onChange={(e) => setLocalSearch(e.target.value)}
                                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                                className="pl-8 pr-8"
                            />
                            {localSearch && (
                                <button
                                    onClick={clearSearch}
                                    className="absolute right-2 top-2.5"
                                >
                                    <X className="h-4 w-4 text-muted-foreground" />
                                </button>
                            )}
                        </div>
                        <Button variant="secondary" onClick={handleSearch}>
                            Поиск
                        </Button>
                    </div>

                    <div className="flex items-center gap-2">
                        <Button
                            variant="outline"
                            onClick={() => setShowFilters(!showFilters)}
                            className="flex items-center gap-2"
                        >
                            <Filter className="h-4 w-4" />
                            Фильтры
                            {(categoryFilter || statusFilter) && (
                                <Badge variant="secondary" className="ml-1">
                                    Активны
                                </Badge>
                            )}
                        </Button>

                        <Link href="/feedbacks/new">
                            <Button>
                                <Plus className="mr-2 h-4 w-4" />
                                Добавить отзыв
                            </Button>
                        </Link>
                    </div>
                </div>

                {showFilters && (
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-lg">Фильтры</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                                <div className="space-y-2">
                                    <label className="text-sm font-medium">Категория</label>
                                    <Select
                                        value={localCategory}
                                        onValueChange={setLocalCategory}
                                        disabled={loadingFilters}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder="Все категории" />
                                        </SelectTrigger>
                                        <SelectContent>
                                            <SelectItem value="all">Все категории</SelectItem>
                                            {categories.map((cat) => (
                                                <SelectItem key={cat.id} value={String(cat.id)}>
                                                    {cat.name}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </div>

                                <div className="space-y-2">
                                    <label className="text-sm font-medium">Статус</label>
                                    <Select
                                        value={localStatus}
                                        onValueChange={setLocalStatus}
                                        disabled={loadingFilters}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder="Все статусы" />
                                        </SelectTrigger>
                                        <SelectContent>
                                            <SelectItem value="all">Все статусы</SelectItem>
                                            {statuses.map((status) => (
                                                <SelectItem key={status.id} value={String(status.id)}>
                                                    {status.name}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </div>

                                <div className="flex items-end gap-2">
                                    <Button onClick={applyFilters}>Применить</Button>
                                    <Button variant="outline" onClick={clearFilters}>
                                        Сбросить
                                    </Button>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                )}
            </div>

            <div className="rounded-md border">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-[80px]">
                                <Button
                                    variant="ghost"
                                    onClick={() => handleSort('id')}
                                    className="flex items-center gap-1"
                                >
                                    ID
                                    <ArrowUpDown className="h-4 w-4" />
                                    {sortColumn === 'id' && (
                                        <span className="text-xs ml-1">
                                            {sortOrder === 'asc' ? '↑' : '↓'}
                                        </span>
                                    )}
                                </Button>
                            </TableHead>
                            <TableHead>
                                <Button
                                    variant="ghost"
                                    onClick={() => handleSort('message')}
                                    className="flex items-center gap-1"
                                >
                                    Сообщение
                                    <ArrowUpDown className="h-4 w-4" />
                                    {sortColumn === 'message' && (
                                        <span className="text-xs ml-1">
                                            {sortOrder === 'asc' ? '↑' : '↓'}
                                        </span>
                                    )}
                                </Button>
                            </TableHead>
                            <TableHead>Категория</TableHead>
                            <TableHead>Статус</TableHead>
                            <TableHead>
                                <Button
                                    variant="ghost"
                                    onClick={() => handleSort('createdAt')}
                                    className="flex items-center gap-1"
                                >
                                    Дата создания
                                    <ArrowUpDown className="h-4 w-4" />
                                    {sortColumn === 'createdAt' && (
                                        <span className="text-xs ml-1">
                                            {sortOrder === 'asc' ? '↑' : '↓'}
                                        </span>
                                    )}
                                </Button>
                            </TableHead>
                            <TableHead className="w-[100px]">Действия</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {loading ? (
                            <TableRow>
                                <TableCell colSpan={6} className="text-center py-8">
                                    Загрузка...
                                </TableCell>
                            </TableRow>
                        ) : data.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={6} className="text-center py-8">
                                    Нет данных
                                </TableCell>
                            </TableRow>
                        ) : (
                            data.map((item) => (
                                <TableRow key={item.id}>
                                    <TableCell>{item.id}</TableCell>
                                    <TableCell className="max-w-md">
                                        <div className="truncate" title={item.message}>
                                            {item.message}
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <Badge variant="outline">
                                            {item.categoryName || `ID: ${item.categoryId}`}
                                        </Badge>
                                    </TableCell>
                                    <TableCell>
                                        {getStatusBadge(item.statusName)}
                                    </TableCell>
                                    <TableCell>{formatDate(item.createdAt)}</TableCell>
                                    <TableCell>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant="ghost" className="h-8 w-8 p-0">
                                                    <MoreHorizontal className="h-4 w-4" />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align="end">
                                                <DropdownMenuLabel>Действия</DropdownMenuLabel>
                                                <DropdownMenuItem asChild>
                                                    <Link href={`/feedbacks/${item.id}`}>
                                                        <Pencil className="mr-2 h-4 w-4" />
                                                        Редактировать
                                                    </Link>
                                                </DropdownMenuItem>
                                                <DropdownMenuSeparator />
                                                <DropdownMenuItem
                                                    className="text-red-600"
                                                    onClick={() => handleDelete(item.id)}
                                                >
                                                    <Trash className="mr-2 h-4 w-4" />
                                                    Удалить
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </div>

            <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                    <p className="text-sm text-muted-foreground">
                        Всего записей: {totalRecords}
                    </p>
                    <Select
                        value={String(pageSize)}
                        onValueChange={handlePageSizeChange}
                    >
                        <SelectTrigger className="h-8 w-[70px]">
                            <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                            {[5, 10, 20, 50, 100].map((size) => (
                                <SelectItem key={size} value={String(size)}>
                                    {size}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>

                <div className="flex items-center gap-2">
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handlePageChange(1)}
                        disabled={pageNumber === 1}
                    >
                        Первая
                    </Button>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handlePageChange(pageNumber - 1)}
                        disabled={pageNumber === 1}
                    >
                        Предыдущая
                    </Button>

                    <span className="text-sm">
                        Страница {pageNumber} из {totalPages}
                    </span>

                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handlePageChange(pageNumber + 1)}
                        disabled={pageNumber === totalPages}
                    >
                        Следующая
                    </Button>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handlePageChange(totalPages)}
                        disabled={pageNumber === totalPages}
                    >
                        Последняя
                    </Button>
                </div>
            </div>
        </div>
    );
}