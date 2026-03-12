using AppointmentService.Infrastructure.Data;
using AppointmentService.Core.Repositories;
using AppointmentService.Infrastructure.Repositories;
using AppointmentService.API.Filters;

using AppointmentService.Application.Handlers;
using AppointmentService.Application.Handlers.Interfaces;
using AppointmentService.Application.Interfaces;

using AppointmentService.Infrastructure.Services;
using AppointmentService.Infrastructure.Messaging;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//
// 1️⃣ Logging (Serilog)
//
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/appointmentservice-log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//
//  Global Exception Filter
//
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//
//  Database
//
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseInMemoryDatabase("AppointmentDB"));

//
//  Dependency Injection
//
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddScoped<ICreateAppointmentHandler, CreateAppointmentHandler>();
builder.Services.AddScoped<IGetMyAppointmentsHandler, GetMyAppointmentsHandler>();
builder.Services.AddScoped<ISearchAppointmentsHandler, SearchAppointmentsHandler>();
builder.Services.AddScoped<IUpdateAppointmentHandler, UpdateAppointmentHandler>();
builder.Services.AddScoped<IDeleteAppointmentHandler, DeleteAppointmentHandler>();

builder.Services.AddScoped<IJwtService, JwtService>();

//
//  HttpClient (PatientService Communication)
//
builder.Services.AddHttpClient<PatientServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/");
});

builder.Services.AddHttpClient<IAppointmentApiService, AppointmentApiService>();

//
//  Kafka Consumer
//
//builder.Services.AddHostedService<KafkaConsumerService>();

//builder.Services.AddHttpContextAccessor();

//
// JWT Authentication
//
var key = configuration["Jwt:Key"];

if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT Key missing in configuration.");
}

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

//
// Swagger + JWT Support
//
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Service API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your JWT token}",
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
            Array.Empty<string>()
        }
    });
});

//
// Build App
//
var app = builder.Build();

//
// Middleware Pipeline
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();