using Statistics.Data;
using Microsoft.EntityFrameworkCore;
using Statistics.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ����������� � PostgreSQL
string connectionString;
try
{
    connectionString = File.ReadAllText("db_config.txt").Trim();
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new Exception("������ ����������� �����. ���������, ��� db_config.txt ��������.");
    }

    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"������ ������ ������ �����������: {ex.Message}");
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

// ������������� RabbitMQ � ��� ���������
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
            logger.LogInformation("���������� ������� ���������: {Message}", message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "������ ��������� ��������� RabbitMQ: {Message}", message);
        }
    }
});



app.Run();
