using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Repositories;
using Obrazovashka.Services;
using System.Text;
using Obrazovashka.Data;
using Obrazovashka.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

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


// Регистрация сервисов и репозиториев
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();

// RabbitMQ
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>(sp =>
{
    try
    {
        return new RabbitMqService("localhost");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка подключения RabbitMQ: {ex.Message}");
        throw;
    }
});


// Добавляем конфигурацию JWT
builder.Configuration.AddIniFile("jwt_config.txt", optional: true, reloadOnChange: true);

var jwtKey = builder.Configuration["Key"];
var issuer = builder.Configuration["Issuer"];
var audience = builder.Configuration["Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new InvalidOperationException("JWT configuration is not properly set.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.UseCors("AllowAll");    

app.Run();