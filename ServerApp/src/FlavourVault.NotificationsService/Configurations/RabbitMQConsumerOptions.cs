using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FlavourVault.NotificationsService.Configurations;
public sealed class RabbitMQConsumerOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}

public sealed class RabbitMQConsumerOptionsSetup : IConfigureOptions<RabbitMQConsumerOptions>
{
    private const string SectionName = "RabbitMQNotificationConsumer";
    private readonly IConfiguration _configuration;

    public RabbitMQConsumerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RabbitMQConsumerOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
