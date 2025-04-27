using AutoMapper;
using Loans.Contracts.Data;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Loans.Contracts.Handlers;

public class CreateContractRequestedHandler : ICreateContractRequestedHandler
{
    private readonly IContractService _сontractService;
    private readonly ILogger<CreateContractRequestedHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;

    public CreateContractRequestedHandler(IContractService сontractService, ILogger<CreateContractRequestedHandler> logger,IMapper mapper,IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _mapper = mapper;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(CreateContractRequestedEvent request, CancellationToken cancellationToken)
    {
        var topic = _config["Kafka:Topics:CreateContractResult"];
        try
        {
            var contract = await _сontractService.CreateContractAsync(request, cancellationToken);
            var @event = _mapper.Map<DraftContractCreatedEvent>(contract);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _producer.PublishAsync(topic, jsonMessage);
            
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to create contract. OperationId: {request.OperationId}", request.OperationId);
            var @event = new CreateContractFailedEvent(request.OperationId, e.Message, e.InnerException?.Message);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _producer.PublishAsync(topic, jsonMessage);
        }
    }
}

