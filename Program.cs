using AzureDevOpsWebhook.Models;
using AzureDevOpsWebhook.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicionar suporte para controladores
builder.Services.AddControllers();

// Registrar o serviço AzureDevOpsService
builder.Services.AddSingleton<AzureDevOpsService>();
builder.Services.Configure<AzureDevOpsRestApi>(builder.Configuration.GetSection("AzureDevOpsRestApi"));

// Adicionar suporte para o Secret Manager
builder.Configuration.AddUserSecrets<Program>();

var xToken = builder.Configuration["XToken"];
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
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
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers(); // Isso permite que os controladores sejam mapeados
});

app.Run();