using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Infrastructure;
using VaciSmart.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// A connection string NÃO fica no repositório: vem de user-secrets (desenvolvimento)
// ou da variável de ambiente ConnectionStrings__OracleFiap. Veja o README.
var connectionString = builder.Configuration.GetConnectionString("OracleFiap");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'OracleFiap' não configurada. " +
        "Rode: dotnet user-secrets set \"ConnectionStrings:OracleFiap\" " +
        "\"User Id=SEU_RM;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL\"");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString, oracle =>
        oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

builder.Services.AddControllers();

// Todos os erros saem em JSON no formato ProblemDetails (application/problem+json).
// Aqui só traduzimos o título padrão do erro de validação (400).
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails is ValidationProblemDetails)
        {
            context.ProblemDetails.Title = "Dados inválidos. Verifique os campos informados.";
        }
    });

// Swagger não é cobrado na entrega; fica ligado só para facilitar os testes manuais.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
