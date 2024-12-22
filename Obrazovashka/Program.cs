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


// ����������� �������� � ������������
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
        Console.WriteLine($"������ ����������� RabbitMQ: {ex.Message}");
        throw;
    }
});


// ��������� ������������ JWT
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