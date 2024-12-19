using LibraryManagementSystem.BusinessLayer;
using LibraryManagementSystem.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using LibraryManagementSystem.Models.Enum;
using System.Data.SqlClient;
using System.Text;
using Microsoft.OpenApi.Any;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<UsersDAO>();


// Register SqlConnection as a scoped service, reading the connection string from appsettings.json
builder.Services.AddScoped<SqlConnection>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("LMSConnection");
    return new SqlConnection(connectionString);
});

// Singleton configuration service
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add controllers and endpoints to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
// Swagger Configuration with JWT Bearer Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryManagementSystem", Version = "v1" });

    c.MapType<UserRole>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(e => new OpenApiString(e.ToString())) // Ensuring the conversion to OpenApiString
                .ToList<IOpenApiAny>() // Convert to IOpenApiAny for Swagger compatibility
    });

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token in the text input below."
    });

    // Add JWT security requirement for all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // These should be in appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])) // SecretKey should be in appsettings.json
        };

        // Custom handling of unauthorized access
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // Prevents the default challenge
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = Newtonsoft.Json.JsonConvert.SerializeObject(new { message = "Please login first." });
                return context.Response.WriteAsync(result); // Custom message on 401 Unauthorized
            }
        };
    });

// Role-based Authorization Configuration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});
builder.Services.AddScoped<DatabaseHelper>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("LMSConnection");
    return new DatabaseHelper(connectionString);  // Pass the connection string to the DatabaseHelper
});
var app = builder.Build();

// Custom Middleware to catch Unauthorized Access (401) or Forbidden Access (403)
app.Use(async (context, next) =>
{
    await next.Invoke();  // Continue request pipeline

    // Check if the status code is 403 Forbidden
    if (context.Response.StatusCode == 403)
    {
        // Forbidden Access: Send a custom message
        context.Response.ContentType = "application/json";
        var result = Newtonsoft.Json.JsonConvert.SerializeObject(new { message = "Only admin can do this" });
        await context.Response.WriteAsync(result); // Send custom message for 403 Forbidden
    }
});

// Configure the HTTP request pipeline for Swagger and error handling
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryManagementSystem v1");
        c.RoutePrefix = "swagger"; // Optional: change Swagger route
    });
}


// Use Authentication and Authorization middleware
app.UseAuthentication(); // Enables JWT authentication
app.UseAuthorization();  // Enables role-based authorization

// Map controllers to handle incoming API requests
app.MapControllers();

// Run the application
app.Run();
