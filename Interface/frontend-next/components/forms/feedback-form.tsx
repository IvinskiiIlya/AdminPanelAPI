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
import { Textarea } from '@/components/ui/textarea';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { feedbackSchema, FeedbackFormValues } from '@/lib/validations/feedback';
import { feedbackApi } from '@/lib/api/feedbacks/feedbacks';
import { Category, Feedback } from '@/index';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useState } from 'react';

interface FeedbackFormProps {
    initialData?: Feedback;
    initialCategories: Category[];
}

export function FeedbackForm({ initialData, initialCategories }: FeedbackFormProps) {
    const router = useRouter();
    const [isSubmitting, setIsSubmitting] = useState(false);

    const form = useForm<FeedbackFormValues>({
        resolver: zodResolver(feedbackSchema),
        defaultValues: {
            categoryId: initialData?.categoryId || 0,
            message: initialData?.message || '',
        },
    });

    async function onSubmit(data: FeedbackFormValues) {
        try {
            setIsSubmitting(true);

            if (initialData?.id) {
                await feedbackApi.update(initialData.id, {
                    categoryId: data.categoryId,
                    message: data.message
                });
                toast.success('Отзыв обновлен');
            }
            else {
                await feedbackApi.create({
                    categoryId: data.categoryId,
                    message: data.message
                });
                toast.success('Отзыв создан');
            }

            router.push('/feedbacks');
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
                <Card>
                    <CardHeader>
                        <CardTitle>{initialData ? 'Редактирование' : 'Новый'} отзыв</CardTitle>
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
                                            {initialCategories.map((category) => (
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