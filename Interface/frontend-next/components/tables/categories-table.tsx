'use client';

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
import { MoreHorizontal, Pencil, Trash, Plus, ArrowUpDown, Search, X } from 'lucide-react';
import { toast } from 'sonner';
import { categoryApi } from '@/lib/api/categories/categories';
import { Category } from '@/index';
import { useState, useEffect } from 'react';

interface CategoriesTableProps {
    initialData: {
        data: Category[];
        totalPages: number;
        totalRecords: number;
    };
    initialSearchParams: {
        page?: string;
        pageSize?: string;
        sortColumn?: string;
        sortOrder?: string;
        searchTerm?: string;
    };
}

export function CategoriesTable({ initialData, initialSearchParams }: CategoriesTableProps) {
    const router = useRouter();
    const searchParams = useSearchParams();

    const [data, setData] = useState(initialData.data);
    const [totalPages, setTotalPages] = useState(initialData.totalPages);
    const [totalRecords, setTotalRecords] = useState(initialData.totalRecords);
    const [loading, setLoading] = useState(false);

    const pageNumber = Number(searchParams.get('page')) || Number(initialSearchParams.page) || 1;
    const pageSize = Number(searchParams.get('pageSize')) || Number(initialSearchParams.pageSize) || 10;
    const sortColumn = searchParams.get('sortColumn') || initialSearchParams.sortColumn || 'id';
    const sortOrder = (searchParams.get('sortOrder') || initialSearchParams.sortOrder || 'asc') as 'asc' | 'desc';
    const searchTerm = searchParams.get('searchTerm') || initialSearchParams.searchTerm || '';

    const [localSearch, setLocalSearch] = useState(searchTerm);

    const updateParams = (updates: Record<string, string | number | null>) => {
        const params = new URLSearchParams(searchParams.toString());

        Object.entries(updates).forEach(([key, value]) => {
            if (value === null || value === '') {
                params.delete(key);
            } else {
                params.set(key, String(value));
            }
        });

        router.push(`/categories?${params.toString()}`);
    };

    const handleSearch = () => {
        updateParams({ searchTerm: localSearch || null, page: 1 });
    };

    const clearSearch = () => {
        setLocalSearch('');
        updateParams({ searchTerm: null, page: 1 });
    };

    const handleSort = (column: string) => {
        const newOrder = sortColumn === column && sortOrder === 'asc' ? 'desc' : 'asc';
        updateParams({ sortColumn: column, sortOrder: newOrder, page: 1 });
    };

    const handlePageChange = (newPage: number) => {
        updateParams({ page: newPage });
    };

    const handlePageSizeChange = (newSize: string) => {
        updateParams({ pageSize: parseInt(newSize), page: 1 });
    };

    const handleDelete = async (id: number) => {
        if (confirm('Вы уверены, что хотите удалить эту категорию?')) {
            try {
                await categoryApi.delete(id);
                toast.success('Категория удалена');
                router.refresh();
            } catch (error) {
                toast.error('Ошибка при удалении');
            }
        }
    };

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const params = {
                    pageNumber,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    searchTerm: searchTerm || undefined
                };
                const response = await categoryApi.getAll(params);
                setData(response.data);
                setTotalPages(response.totalPages);
                setTotalRecords(response.totalRecords);
                
            } catch (error) {
                toast.error('Ошибка при загрузке');
                
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [pageNumber, pageSize, sortColumn, sortOrder, searchTerm]);

    return (
        <div className="space-y-4">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div className="flex flex-1 items-center gap-2 max-w-md">
                    <div className="relative flex-1">
                        <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input
                            placeholder="Поиск по названию..."
                            value={localSearch}
                            onChange={(e) => setLocalSearch(e.target.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                            className="pl-8 pr-8"
                        />
                        {localSearch && (
                            <button onClick={clearSearch} className="absolute right-2 top-2.5">
                                <X className="h-4 w-4 text-muted-foreground" />
                            </button>
                        )}
                    </div>
                    <Button variant="secondary" onClick={handleSearch}>Поиск</Button>
                </div>

                <Link href="/categories/new">
                    <Button><Plus className="mr-2 h-4 w-4" />Добавить категорию</Button>
                </Link>
            </div>

            <div className="rounded-md border">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>
                                <Button variant="ghost" onClick={() => handleSort('id')} className="flex items-center gap-1">
                                    ID <ArrowUpDown className="h-4 w-4" />
                                    {sortColumn === 'id' && <span className="text-xs ml-1">{sortOrder === 'asc' ? '↑' : '↓'}</span>}
                                </Button>
                            </TableHead>
                            <TableHead>
                                <Button variant="ghost" onClick={() => handleSort('name')} className="flex items-center gap-1">
                                    Название <ArrowUpDown className="h-4 w-4" />
                                    {sortColumn === 'name' && <span className="text-xs ml-1">{sortOrder === 'asc' ? '↑' : '↓'}</span>}
                                </Button>
                            </TableHead>
                            <TableHead>Описание</TableHead>
                            <TableHead className="w-[100px]">Действия</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {loading ? (
                            <TableRow><TableCell colSpan={4} className="text-center py-8">Загрузка...</TableCell></TableRow>
                        ) : data.length === 0 ? (
                            <TableRow><TableCell colSpan={4} className="text-center py-8">Нет данных</TableCell></TableRow>
                        ) : (
                            data.map((item) => (
                                <TableRow key={item.id}>
                                    <TableCell>{item.id}</TableCell>
                                    <TableCell className="font-medium">{item.name}</TableCell>
                                    <TableCell>{item.description || '—'}</TableCell>
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
                                                    <Link href={`/categories/${item.id}`}>
                                                        <Pencil className="mr-2 h-4 w-4" />Редактировать
                                                    </Link>
                                                </DropdownMenuItem>
                                                <DropdownMenuSeparator />
                                                <DropdownMenuItem className="text-red-600" onClick={() => handleDelete(item.id)}>
                                                    <Trash className="mr-2 h-4 w-4" />Удалить
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
                    <p className="text-sm text-muted-foreground">Всего записей: {totalRecords}</p>
                    <Select value={String(pageSize)} onValueChange={handlePageSizeChange}>
                        <SelectTrigger className="h-8 w-[70px]">
                            <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                            {[5, 10, 20, 50, 100].map((size) => (
                                <SelectItem key={size} value={String(size)}>{size}</SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>

                <div className="flex items-center gap-2">
                    <Button variant="outline" size="sm" onClick={() => handlePageChange(1)} disabled={pageNumber === 1}>Первая</Button>
                    <Button variant="outline" size="sm" onClick={() => handlePageChange(pageNumber - 1)} disabled={pageNumber === 1}>Предыдущая</Button>
                    <span className="text-sm">Страница {pageNumber} из {totalPages}</span>
                    <Button variant="outline" size="sm" onClick={() => handlePageChange(pageNumber + 1)} disabled={pageNumber === totalPages}>Следующая</Button>
                    <Button variant="outline" size="sm" onClick={() => handlePageChange(totalPages)} disabled={pageNumber === totalPages}>Последняя</Button>
                </div>
            </div>
        </div>
    );
}