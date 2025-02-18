using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;

namespace FlavourVault.OutboxDispatcher;
public class ResilienceOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public int DelayMilliseconds { get; set; } = 20;
    public bool UseJitter { get; set; } = true;
    public DelayBackoffType BackoffType { get; set; } = DelayBackoffType.Exponential;
}

public class ResilienceOptionsSetup : IConfigureOptions<ResilienceOptions>
{
    private const string SectionName = "Resilience";
    private readonly IConfiguration _configuration;

    public ResilienceOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ResilienceOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
