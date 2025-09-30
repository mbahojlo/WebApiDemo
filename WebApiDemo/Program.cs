using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebApiDemo.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<TopicsRequestExample>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IQuoteService, QuoteService>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()         
    .WriteTo.File("Logs/Log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); 
var app = builder.Build();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
