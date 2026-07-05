using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SirnakSport.API.Middleware;
using SirnakSport.Application.Mappings;
using SirnakSport.Infrastructure;
using SirnakSport.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. INFRASTRUCTURE (DbContext, Repositories, Services)
// ============================================================
builder.Services.AddInfrastructure(builder.Configuration);

// ============================================================
// 2. AUTOMAPPER
// ============================================================
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ============================================================
// 3. JWT AUTHENTICATION
// ============================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ============================================================
// 4. CONTROLLERS
// ============================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ============================================================
// 5. SWAGGER (JWT Bearer desteği ile)
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Şırnak Üniversitesi Spor Tesisleri Rezervasyon API",
        Version = "v1",
        Description = "Şırnak Üniversitesi spor tesisleri için online rezervasyon sistemi API'si. " +
                      "JWT token ile kimlik doğrulama yapılmaktadır.",
        Contact = new OpenApiContact
        {
            Name = "Şırnak Üniversitesi BT Birimi",
            Email = "bt@sirnak.edu.tr"
        }
    });

    // JWT Bearer token tanımı
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ınızı giriniz.\n\nÖrnek: eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// 6. CORS (Flutter mobil uygulama için)
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================

// Global exception handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger (her ortamda aktif)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Şırnak Sport API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Şırnak Üniversitesi Spor Tesisleri API";
});

// CORS (HttpsRedirection kaldırıldı — dev ortamında HTTP üzerinden çalışıyoruz)
app.UseCors("AllowFlutter");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============================================================
// SEED DATA (Başlangıç verileri)
// ============================================================
await DataSeeder.SeedAsync(app.Services);

app.Run();
