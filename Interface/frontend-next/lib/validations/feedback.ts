import * as z from 'zod';

export const feedbackSchema = z.object({
    categoryId: z.number().min(1, 'Категория обязательна'),
    message: z.string()
        .min(10, 'Сообщение должно содержать минимум 10 символов')
        .max(1000, 'Сообщение не может превышать 1000 символов'),
});

export type FeedbackFormValues = z.infer<typeof feedbackSchema>;