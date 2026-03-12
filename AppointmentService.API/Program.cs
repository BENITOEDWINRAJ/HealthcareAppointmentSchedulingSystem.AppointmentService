/*using AppointmentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Ensure this directive is present
using System.Text;
using AppointmentService.Core.Repositories;
using AppointmentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var key = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

builder.Services.AddControllers();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this middleware to enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();*/
/*
using AppointmentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});*/

using AppointmentService.Infrastructure.Data;
using AppointmentService.Core.Repositories;
using AppointmentService.Infrastructure.Repositories;
using AppointmentService.API.Filters;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using Serilog;
using AppointmentService.Application.Handlers;
using AppointmentService.Infrastructure.Messaging;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Infrastructure.Services;
using AppointmentService.Application.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/Appointmentservice-log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
var configuration = builder.Configuration;

// -------------------------
// Add Services
// -------------------------

// Add Controllers + Global Exception Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// DbContext
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseInMemoryDatabase("AppointmentDb"));

// Dependency Injection
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IGetMyAppointmentsHandler, GetMyAppointmentsHandler>();
builder.Services.AddScoped<ICreateAppointmentHandler, CreateAppointmentHandler>();
builder.Services.AddScoped<ISearchAppointmentsHandler, SearchAppointmentsHandler>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddHttpClient<IAppointmentApiService, AppointmentApiService>();
builder.Services.AddScoped<IUpdateAppointmentHandler, UpdateAppointmentHandler>();
builder.Services.AddScoped<IDeleteAppointmentHandler,DeleteAppointmentHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IAppointmentApiService,AppointmentApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7001"); // PatientService or AppointmentService URL
});
// JWT Authentication
var key = configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT Key is missing in configuration");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key))
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// 8️⃣ Swagger + JWT Support
//
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AppointmentService API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token like: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
// -------------------------
// Build App
// -------------------------

var app = builder.Build();

// -------------------------
// Configure Middleware
// -------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
