using Statistics.Data;
using Microsoft.EntityFrameworkCore;
using Statistics.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключение к PostgreSQL
string connectionString;
try
{
    connectionString = File.ReadAllText("db_config.txt").Trim();
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new Exception("Строка подключения пуста. Убедитесь, что db_config.txt заполнен.");
    }

    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка чтения строки подключения: {ex.Message}");
    Environment.Exit(1);
}


builder.Services.AddScoped<StatisticsService>();
builder.Services.AddSingleton<RabbitMqService>(sp => new RabbitMqService("localhost"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Инициализация RabbitMQ и его слушателя
var rabbitMqService = app.Services.GetRequiredService<RabbitMqService>();

rabbitMqService.ListenForMessages(async message =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var statisticsService = services.GetRequiredService<StatisticsService>();
        var logger = services.GetRequiredService<ILogger<RabbitMqService>>();

        try
        {
            await statisticsService.ProcessMessageAsync(message);
            logger.LogInformation("Статистика успешно обновлена: {Message}", message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки сообщения RabbitMQ: {Message}", message);
        }
    }
});



app.Run();
