using AutoMapper;
using Loans.Contracts.Data;
using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Mappers;
using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Consumers;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Events.CalculateContractValues;
using Loans.Contracts.Kafka.Events.CreateDraftContract;
using Loans.Contracts.Kafka.Events.GetContractApproved;
using Loans.Contracts.Kafka.Handlers;
using Loans.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<LoanContractDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IContractService, ContractService>();

builder.Services.AddScoped<IEventHandler<CreateContractRequestedEvent>, CreateContractRequestedHandler>();
builder.Services.AddScoped<IEventHandler<FullLoanValueCalculatedEvent>, FullLoanValueCalculatedHandler>();
builder.Services.AddScoped<IEventHandler<ContractValuesCalculatedEvent>, ContractValuesCalculatedHandler>();
builder.Services.AddScoped<IEventHandler<ContractScheduleCalculatedEvent>, ContractScheduleCalculatedHandler>();
builder.Services.AddScoped<IEventHandler<ContractDetailsRequestedEvent>, GetFullContractHandler>();
builder.Services.AddScoped<IEventHandler<UpdateContractStatusEvent>, UpdateContractStatusHandler>();

builder.Services.AddHostedService<UpdateContractConsumer>();
builder.Services.AddHostedService<CreateContractConsumer>();

builder.Services.AddSingleton<KafkaProducerService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
