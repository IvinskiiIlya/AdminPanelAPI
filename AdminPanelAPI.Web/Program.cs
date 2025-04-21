using AdminPanelAPI.Repositories;
using AdminPanelAPI.Repositories.Users;
using AdminPanelAPI.Repositories.Admins;
using AdminPanelAPI.Repositories.Categories;
using AdminPanelAPI.Repositories.Feedbacks;
using AdminPanelAPI.Repositories.FeedbackCategories;
using AdminPanelAPI.Repositories.Responses;
using AdminPanelAPI.Repositories.Attachments;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Регистрация репозиториев (Dependency Injection)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackCategoryRepository, FeedbackCategoryRepository>();
builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();

// Другие сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();