using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/create-contract", async ([FromBody] LoanApplicationRequest application, IPublisher publisher) =>
{
    var @event = new CreateContractRequestedEvent
    (application.ApplicationId,
        application.ClientId,
        application.DecisionId,
        application.LodgementDate,
        application.CreditProductId,
        application.LoanAmount,
        application.LoanTermMonths,
        application.InterestRate,
        application.LoanPurpose,
        application.LoanType,
        application.PaymentType,
        application.InitialPaymentAmount,
        application.Pawn,
        application.Insurance);

    await publisher.Publish(@event);
    Console.WriteLine("CreateContractRequestedEvent published.");
});

app.MapGet("/api/contract/{id}", async (Guid id, IContractService contractService) =>
    {
        var contract = await contractService.GetContractAsync(id);
        if (contract is null)
        {
            return Results.NotFound(); // Возвращаем HTTP-статус 404 Not Found
        }

        return Results.Ok(contract);
    }).WithName("GetContract").WithOpenApi();

app.Run();
