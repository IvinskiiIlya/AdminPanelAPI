'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
    FormDescription,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { categorySchema, CategoryFormValues } from '@/lib/validations/category';
import { Category } from '@/index';
import { useTransition } from 'react';
import { createCategory, updateCategory } from '@/app/actions/categories';

interface CategoryFormProps {
    initialData?: Category;
}

export function CategoryForm({ initialData }: CategoryFormProps) {
    const router = useRouter();
    const [isPending, startTransition] = useTransition();

    const form = useForm<CategoryFormValues>({
        resolver: zodResolver(categorySchema),
        defaultValues: {
            id: initialData?.id || undefined,
            name: initialData?.name || '',
            description: initialData?.description || '',
        },
    });

    async function onSubmit(data: CategoryFormValues) {
        try {
            const formData = new FormData();

            if (initialData?.id) {
                formData.append('name', data.name);
                formData.append('description', data.description || '');

                startTransition(async () => {
                    await updateCategory(initialData.id, formData);
                });
            } else {
                formData.append('id', String(data.id));
                formData.append('name', data.name);
                formData.append('description', data.description || '');

                startTransition(async () => {
                    await createCategory(formData);
                });
            }
        } catch (error: any) {
            toast.error(error.message || 'Ошибка при сохранении');
        }
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
                {!initialData && (
                    <FormField
                        control={form.control}
                        name="id"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>ID категории *</FormLabel>
                                <FormControl>
                                    <Input
                                        type="number"
                                        placeholder="Введите ID категории"
                                        disabled={isPending}
                                        onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                        value={field.value || ''}
                                    />
                                </FormControl>
                                <FormDescription>
                                    Уникальный идентификатор категории
                                </FormDescription>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                )}

                <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Название *</FormLabel>
                            <FormControl>
                                <Input
                                    placeholder="Введите название категории"
                                    disabled={isPending}
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="description"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Описание</FormLabel>
                            <FormControl>
                                <Textarea
                                    placeholder="Введите описание категории"
                                    disabled={isPending}
                                    {...field}
                                    value={field.value || ''}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <div className="flex gap-4">
                    <Button type="submit" disabled={isPending}>
                        {isPending ? 'Сохранение...' : (initialData ? 'Сохранить' : 'Создать')}
                    </Button>
                    <Button
                        type="button"
                        variant="outline"
                        onClick={() => router.push('/categories')}
                        disabled={isPending}
                    >
                        Отмена
                    </Button>
                </div>
            </form>
        </Form>
    );
}