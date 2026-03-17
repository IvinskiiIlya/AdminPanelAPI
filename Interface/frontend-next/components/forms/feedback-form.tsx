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
import { Category, Feedback } from '@/index';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useTransition } from 'react';
import { createFeedback, updateFeedback } from '@/app/actions/feedbacks';

interface FeedbackFormProps {
    initialData?: Feedback;
    initialCategories: Category[];
}

export function FeedbackForm({ initialData, initialCategories }: FeedbackFormProps) {
    const router = useRouter();
    const [isPending, startTransition] = useTransition();

    const form = useForm<FeedbackFormValues>({
        resolver: zodResolver(feedbackSchema),
        defaultValues: {
            categoryId: initialData?.categoryId || 0,
            message: initialData?.message || '',
        },
    });

    async function onSubmit(data: FeedbackFormValues) {
        const formData = new FormData();
        formData.append('categoryId', String(data.categoryId));
        formData.append('message', data.message);
        
        if (initialData?.statusId) {
            formData.append('statusId', String(initialData.statusId));
        }

        if (initialData?.id) {
            startTransition(async () => {
                const result = await updateFeedback(initialData.id, formData);
                if (result?.error) {
                    toast.error(result.error);
                }
            });
        } else {
            startTransition(async () => {
                const result = await createFeedback(formData);
                if (result?.error) {
                    toast.error(result.error);
                }
            });
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
                                        disabled={isPending}
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
                                            disabled={isPending}
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
                    <Button type="submit" disabled={isPending}>
                        {isPending ? 'Сохранение...' : (initialData ? 'Сохранить' : 'Создать')}
                    </Button>
                    <Button
                        type="button"
                        variant="outline"
                        onClick={() => router.push('/feedbacks')}
                        disabled={isPending}
                    >
                        Отмена
                    </Button>
                </div>
            </form>
        </Form>
    );
}