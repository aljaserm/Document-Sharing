using Application.DTOs;
using Application.Enums;
using Application.Utils;
using Infrastructure.Data;
using Infrastructure.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Net.Http.Headers;
using FluentValidation.AspNetCore;
using FluentValidation;
using MediatR;
using System.Linq;
using Application.Commands.Documents.GenerateShareLinkCommand;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Infrastructure")));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers().AddFluentValidation();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GenerateShareLinkCommand>());

// Configure CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition"); // Add any headers you need to expose
    });
});

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentLibrary API", Version = "v1" });
    c.MapType<FileType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(FileType)).Select(name => new OpenApiString(name) as IOpenApiAny).ToList()
    });
    c.MapType<TimeUnit>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(TimeUnit)).Select(name => new OpenApiString(name) as IOpenApiAny).ToList()
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocumentLibrary API v1"));
}

app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowAllOrigins");

// Add the middleware in the correct order
app.UseRouting(); // UseRouting should be called before UseEndpoints


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
