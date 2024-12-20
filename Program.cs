using LibraryAPI.DataAccess.Data;
using LibraryAPI.DataAccess.Interfaces;
using LibraryAPI.DataAccess.Repositories;
using LibraryAPI.BusinessLogic.Interfaces;
using LibraryAPI.BusinessLogic.Services;
using LibraryAPI.DataAccess.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using CRUDS.DataAccess.Models.Enum;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration to the builder to read from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LMSConnection"))
);

// Register DatabaseHelper, BookRepository, and BookService for dependency injection
builder.Services.AddScoped<DatabaseHelper>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("LMSConnection");
    return new DatabaseHelper(connectionString);  // Pass the connection string to the DatabaseHelper
});

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

// Optional: Add health checks for better API monitoring
builder.Services.AddHealthChecks();

// Register controllers
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.MapType<SearchBy>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetValues(typeof(SearchBy))
            .Cast<SearchBy>()
            .Select(e => new OpenApiString(e.ToString())) // Ensuring the conversion to OpenApiString
            .ToList<IOpenApiAny>() // Convert to IOpenApiAny for Swagger compatibility
    });
});

// Build the application
var app = builder.Build();

// Apply migrations on startup, handling pending model changes
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Check for pending migrations
    var pendingMigrations = dbContext.Database.GetPendingMigrations();

    if (pendingMigrations.Any())
    {
        // Log the pending migrations (Optional)
        Console.WriteLine("Applying the following pending migrations:");
        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine(migration);
        }

        // Apply migrations if any
        dbContext.Database.Migrate();
    }
    else
    {
        Console.WriteLine("No pending migrations to apply.");
    }
}

// Get the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("LMSConnection");

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
