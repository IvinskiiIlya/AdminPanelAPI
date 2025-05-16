using Data;
using Repositories.Users;
using Repositories.Admins;
using Repositories.Categories;
using Repositories.Feedbacks;
using Repositories.FeedbackCategories;
using Repositories.Responses;
using Repositories.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Admin Panel API",
        Version = "v1",
        Description = "API для управления административной панелью",
        Contact = new OpenApiContact { Name = "Developer", Email = "dev@example.com" }
    });
});

// Регистрация БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Регистрация репозиториев
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackCategoryRepository, FeedbackCategoryRepository>();
builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();

// Добавление CORS (настройте политику под ваш фронтенд)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Конвейер middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseRouting(); // Добавлено явное указание маршрутизации

app.UseCors("AllowAll"); // Применение политики CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();