using Microsoft.EntityFrameworkCore;
using VocabularyApp.Data;

var builder = WebApplication.CreateBuilder(args);

// 加入 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 設定資料庫連線（使用 SQLite）
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊控制器服務
builder.Services.AddControllers();

// 設定 CORS 策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// 開發環境啟用 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 設定路由
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseCors("AllowAll");

app.Run();