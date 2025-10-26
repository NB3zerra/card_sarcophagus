using System.Reflection;
using CardSarcophagus.Infra.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CardSarcophagus API",
        Version = "v1",
        Description = "API for managing card scraping and related operations.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "support@cardsarcophagus.com",
        }
    });
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000") // React app URL
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Register scraping services
builder.Services.RegisterScrapingServices();

builder.Services.AddControllers();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CardSarcophagus API V1");
});
app.UseCors();
app.UseRouting();
app.MapControllers();
app.Run();
