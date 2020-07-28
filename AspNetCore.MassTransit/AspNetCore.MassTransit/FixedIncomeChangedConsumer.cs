using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCore.MassTransit
{
    internal class FixedIncomeChangedConsumer : IConsumer<FixedIncomeChangedEvent>
    {
        private readonly ILogger<FixedIncomeChangedConsumer> _logger;

        public FixedIncomeChangedConsumer(ILogger<FixedIncomeChangedConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<FixedIncomeChangedEvent> context)
        {
            _logger.LogInformation($"Instrument received: {JsonConvert.SerializeObject(context.Message)}");
            return Task.CompletedTask;
        }
    }

    internal class FixedIncomeChangedEvent
    {
        public int ProductType { get; set; }
        public int OperationType { get; set; }
        public int ProductId { get; set; }
        public DateTime RequestDate { get; set; }
        public Guid EventId { get; set; }
        public string SyncKey { get; set; }

    }
}