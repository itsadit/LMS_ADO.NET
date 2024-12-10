using LibraryAPI.DataAccess.Data;
using LibraryAPI.DataAccess.Interfaces;
using LibraryAPI.DataAccess.Repositories;
using LibraryAPI.BusinessLogic.Interfaces;
using LibraryAPI.BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration to the builder to read from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DatabaseHelper, BookRepository, and BookService for dependency injection
builder.Services.AddScoped<DatabaseHelper>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DatabaseHelper(connectionString);  // Pass the connection string to the DatabaseHelper
});

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

// Optional: Add health checks for better API monitoring
builder.Services.AddHealthChecks();

// Register controllers
builder.Services.AddControllers();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect root URL ("/") to Swagger UI automatically
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger", permanent: false);
        return;
    }
    await next.Invoke();
});

app.UseHttpsRedirection();

// Map controllers to routes
app.MapControllers();

// Optional: Health check endpoint
app.MapHealthChecks("/health");

// Run the application
app.Run();
