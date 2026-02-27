import * as z from 'zod';

export const categorySchema = z.object({
    id: z.number()
        .min(1, 'ID должен быть положительным числом')
        .optional(),
    name: z.string()
        .min(2, 'Название должно содержать минимум 2 символа')
        .max(50, 'Название не может превышать 50 символов'),
    description: z.string()
        .max(200, 'Описание не может превышать 200 символов')
        .optional()
        .nullable(),
});

export type CategoryFormValues = z.infer<typeof categorySchema>;