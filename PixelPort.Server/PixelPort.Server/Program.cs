using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PixelPort.Server;
using PixelPort.Server.Data;
using PixelPort.Server.Repository;
using PixelPort.Server.Repository.IRepository;
using PixelPort.Server.Services;
using PixelPort.Server.Services.IServices;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<PixelPortDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCharacteristicRepository, ProductCharacteristicRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IHashing, Hashing>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL Angular приложения
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // важно для withCredentials
        });
});


var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Добавили поддержку cookies для Angular
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Ищем токен в cookie (для Angular)
                if (context.Request.Cookies.ContainsKey("auth_token"))
                {
                    context.Token = context.Request.Cookies["auth_token"];
                }
                // Или в header (для Swagger)
                else if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var header = context.Request.Headers["Authorization"].ToString();
                    if (header.StartsWith("Bearer "))
                    {
                        context.Token = header.Substring(7);
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options => // Bearer для тестирования через Swagger
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Введите JWT токен (для тестирования в Swagger)"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

//app.UseHttpsRedirection();

// Починить Авторизацию в свагере
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
