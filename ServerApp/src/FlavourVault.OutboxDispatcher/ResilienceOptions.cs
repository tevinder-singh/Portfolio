using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace FlavourVault.OutboxDispatcher;
public sealed class ResilienceOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public int DelayMilliseconds { get; set; } = 20;
    public bool UseJitter { get; set; } = true;
    public DelayBackoffType BackoffType { get; set; } = DelayBackoffType.Exponential;

    public ResiliencePipeline GetResiliencePipeline<T>(ILogger<T> logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = MaxRetryAttempts,
                Delay = TimeSpan.FromMilliseconds(DelayMilliseconds),
                BackoffType = BackoffType,
                UseJitter = UseJitter,
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                OnRetry = (args) =>
                {
                    logger.LogError("Retrying due to: {Message}, Attempt: {AttemptNumber}", args.Outcome.Exception?.Message, args.AttemptNumber);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.3,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(1),
                MinimumThroughput = 5,
                OnOpened = (args) =>
                {
                    logger.LogError("Circuit breaker triggered due to: {Message}", args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                },
                OnClosed = (args) =>
                {
                    logger.LogInformation("Circuit breaker reset.");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
    }
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
