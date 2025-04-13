using AutoMapper;
using Loans.Contracts.Data;
using Loans.Contracts.Data.Dto;
using Loans.Contracts.Handlers;
using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Mappers;
using Loans.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<LoanContractDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSingleton<KafkaProducerService>();

builder.Services.AddScoped<IContractService, ContractService>();

builder.Services.AddScoped<ICreateContractRequestedHandler, CreateContractRequestedHandler>();

builder.Services.AddHostedService<KafkaConsumerService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/create-contract", async ([FromBody] LoanApplicationRequest application, KafkaProducerService producer, IConfiguration config, IMapper mapper) =>
{
    var @event = mapper.Map<CreateContractRequestedEvent>(application);
    var jsonMessage = JsonConvert.SerializeObject(@event);
    var topic = config["Kafka:Topics:CreateContractRequested"];
    
    await producer.PublishAsync(topic, jsonMessage);
});

app.MapGet("/api/contract/{id}", async (Guid id, IContractService contractService) =>
    {
        var contract = await contractService.GetContractAsync(id);
        if (contract is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(contract);
    }).WithName("GetContract").WithOpenApi();

app.Run();
