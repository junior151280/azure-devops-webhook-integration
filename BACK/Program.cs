using ADOWebhook.Back.Models;
using ADOWebhook.Back.Services;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicionar suporte para controladores
builder.Services.AddControllers();

// Registrar o serviço AzureDevOpsService
builder.Services.AddSingleton<AzureDevOpsService>();
builder.Services.AddSingleton<CosmosDBService>();
builder.Services.Configure<AzureDevOpsRestApi>(builder.Configuration.GetSection("AzureDevOpsRestApi"));
builder.Services.Configure<CosmosDB>(builder.Configuration.GetSection("CosmosDB"));

// Adicionar suporte para o Secret Manager
builder.Configuration.AddUserSecrets<Program>();

//var xToken = builder.Configuration["XToken"];
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});

builder.Logging.AddApplicationInsights();
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);


// Adicionar política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://adowebhook-front.azurewebsites.net")
                          .WithOrigins("https://localhost:7103")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configurar o roteamento para os controladores
app.UseRouting();

// Aplicar a política de CORS
app.UseCors("AllowSpecificOrigin");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers(); // Isso permite que os controladores sejam mapeados
});

app.Run();