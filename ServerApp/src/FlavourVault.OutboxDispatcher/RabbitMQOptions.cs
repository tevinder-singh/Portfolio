using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FlavourVault.OutboxDispatcher;
public sealed class RabbitMQOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Exchange { get; set; } = null!;        
    public string DeadLetterExchange { get; set; } = null!;    
    public int DeadLetterRetryWaitTime { get; set; } = 1000;
}

public sealed class RabbitMQOptionsSetup : IConfigureOptions<RabbitMQOptions>
{
    private const string SectionName = "RabbitMQ";
    private readonly IConfiguration _configuration;

    public RabbitMQOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RabbitMQOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
