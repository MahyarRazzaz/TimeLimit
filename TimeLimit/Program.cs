using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeLimit.Data;
using TimeLimit.Repositories;

var builder = WebApplication.CreateBuilder(args);

// پیکربندی Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // همچنان لاگ‌ها توی کنسول هم نمایش داده می‌شن
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // ذخیره لاگ در فایل روزانه
    .CreateLogger();

builder.Host.UseSerilog(); // جایگزین سیستم logging پیش‌فرض

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IRequestRepository, RequestRepository>();

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
app.MapControllers();

app.Run();

