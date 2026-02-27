'use client';

import { useEffect, useState } from 'react';
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
import { Textarea } from '@/components/ui/textarea';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { feedbackSchema, FeedbackFormValues } from '@/lib/validations/feedback';
import { feedbackApi } from '@/lib/api/feedbacks';
import { categoryApi } from '@/lib/api/categories';
import { Category, Feedback } from '@/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

interface FeedbackFormProps {
    initialData?: Feedback;
}

export function FeedbackForm({ initialData }: FeedbackFormProps) {
    const router = useRouter();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);

    const form = useForm<FeedbackFormValues>({
        resolver: zodResolver(feedbackSchema),
        defaultValues: {
            categoryId: initialData?.categoryId || 0,
            message: initialData?.message || '',
        },
    });

    useEffect(() => {
        const loadCategories = async () => {
            try {
                setLoading(true);
                const categoriesData = await categoryApi.getAll({ pageSize: 100 });
                setCategories(categoriesData.data || []);
                
            } catch (error) {
                console.error('Error loading categories:', error);
                toast.error('Ошибка при загрузке категорий');
                
            } finally {
                setLoading(false);
            }
        };
        loadCategories();
    }, []);

    async function onSubmit(data: FeedbackFormValues) {
        try {
            setIsSubmitting(true);
            console.log('Submitting feedback form data:', data);

            if (initialData?.id) {
                await feedbackApi.update(initialData.id, {
                    categoryId: data.categoryId,
                    message: data.message
                });
                toast.success('Отзыв обновлен');
            } else {
                await feedbackApi.create({
                    categoryId: data.categoryId,
                    message: data.message
                });
                toast.success('Отзыв создан');
            }

            router.push('/feedbacks');
            router.refresh();
            
        } catch (error: any) {
            console.error('Form submission error:', error);

            const errorMessage = error.response?.data?.message
                || error.response?.data
                || 'Ошибка при сохранении';

            toast.error(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
            
        } finally {
            setIsSubmitting(false);
        }
    }

    if (loading) {
        return <div className="text-center py-8">Загрузка категорий...</div>;
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
                <Card>
                    <CardHeader>
                        <CardTitle>Новый отзыв</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                        <FormField
                            control={form.control}
                            name="categoryId"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Категория *</FormLabel>
                                    <Select
                                        disabled={isSubmitting}
                                        onValueChange={(value) => field.onChange(parseInt(value))}
                                        value={String(field.value)}
                                    >
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Выберите категорию" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {categories.map((category) => (
                                                <SelectItem key={category.id} value={String(category.id)}>
                                                    {category.name}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <FormField
                            control={form.control}
                            name="message"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Сообщение *</FormLabel>
                                    <FormControl>
                                        <Textarea
                                            placeholder="Введите текст отзыва"
                                            disabled={isSubmitting}
                                            className="min-h-[150px]"
                                            {...field}
                                        />
                                    </FormControl>
                                    <FormDescription>
                                        От 10 до 1000 символов
                                    </FormDescription>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                    </CardContent>
                </Card>

                <div className="flex gap-4">
                    <Button type="submit" disabled={isSubmitting}>
                        {isSubmitting ? 'Сохранение...' : (initialData ? 'Сохранить' : 'Создать')}
                    </Button>
                    <Button
                        type="button"
                        variant="outline"
                        onClick={() => router.push('/feedbacks')}
                        disabled={isSubmitting}
                    >
                        Отмена
                    </Button>
                </div>
            </form>
        </Form>
    );
}