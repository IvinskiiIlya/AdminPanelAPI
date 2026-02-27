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
import { categoryApi } from '@/lib/api/categories/categories';
import { Category } from '@/index';
import { useState } from 'react';

interface CategoryFormProps {
    initialData?: Category;
}

export function CategoryForm({ initialData }: CategoryFormProps) {
    const router = useRouter();
    const [isSubmitting, setIsSubmitting] = useState(false);

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
            setIsSubmitting(true);

            if (initialData?.id) {
                await categoryApi.update(initialData.id, {
                    name: data.name,
                    description: data.description
                });
                toast.success('Категория обновлена');
            } 
            else {
                if (!data.id) {
                    toast.error('Необходимо указать ID категории');
                    setIsSubmitting(false);
                    return;
                }
                await categoryApi.create({
                    id: data.id,
                    name: data.name,
                    description: data.description
                });
                toast.success('Категория создана');
            }

            router.push('/categories');
            router.refresh();
            
        } catch (error: any) {
            const errorMessage = error.response?.data?.message
                || error.response?.data
                || 'Ошибка при сохранении';

            toast.error(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
            
        } finally {
            setIsSubmitting(false);
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
                                        disabled={isSubmitting}
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
                                    disabled={isSubmitting}
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
                                    disabled={isSubmitting}
                                    {...field}
                                    value={field.value || ''}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <div className="flex gap-4">
                    <Button type="submit" disabled={isSubmitting}>
                        {isSubmitting ? 'Сохранение...' : (initialData ? 'Сохранить' : 'Создать')}
                    </Button>
                    <Button
                        type="button"
                        variant="outline"
                        onClick={() => router.push('/categories')}
                        disabled={isSubmitting}
                    >
                        Отмена
                    </Button>
                </div>
            </form>
        </Form>
    );
}