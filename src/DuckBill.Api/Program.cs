using DuckBill.Api.Endpoints;
using DuckBill.Domain.Interfaces;
using DuckBill.Application.Services;
using DuckBill.Infrastructure;
using DuckBill.Infrastructure.Repositories;
using DuckBill.Infrastructure.External;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Oracle DB
builder.Services.AddDbContext<DuckBillDbContext>(opt =>
{
    opt.UseOracle(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();
builder.Services.AddScoped<ITransacaoAtivoRepository, TransacaoAtivoRepository>();
builder.Services.AddScoped<ICotacaoAtivoRepository, CotacaoAtivoRepository>();

// Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<AtivoService>();
builder.Services.AddScoped<TransacaoAtivoService>();
builder.Services.AddScoped<RelatoriosService>();

builder.Services.AddHttpClient<ICambioService, AwesomeApiCambioService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Integrations:AwesomeApi:BaseUrl"] ?? "https://economia.awesomeapi.com.br/");
});

var app = builder.Build();

// Global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred.", details = ex.Message });
    }
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/swagger")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DuckBill API"));

app.MapCategorias().MapDespesas().MapAtivos().MapTransacoesAtivo().MapRelatorios().MapUsuarios();

app.Run();
