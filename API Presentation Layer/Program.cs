using Library_Management_System.BLL;
using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models.Enum;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IBorrowAndReturnBookServices, BorrowAndReturnBookServices>();
builder.Services.AddScoped<IBorrowAndReturnBooksDataAccessObject, BorrowAndReturnBooksDataAccessObject>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Register Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.MapType<TransactionTypes>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetValues(typeof(TransactionTypes))
            .Cast<TransactionTypes>()
            .Select(e => new OpenApiString(e.ToString())) // Ensuring the conversion to OpenApiString
            .ToList<IOpenApiAny>() // Convert to IOpenApiAny for Swagger compatibility
    });
});
// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Provides detailed error pages in development
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
