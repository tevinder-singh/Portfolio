using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FlavourVault.NotificationsService.Configurations;
public sealed class SmtpEmailOptions
{
    public int? Port { get; set; }
    public string SmtpHostAddress { get; set; } = null!;    
    public bool UseSSL { get; set; }
}

public sealed class SmtpEmailOptionsSetup : IConfigureOptions<SmtpEmailOptions>
{
    private const string SectionName = "SmtpEmail";
    private readonly IConfiguration _configuration;

    public SmtpEmailOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(SmtpEmailOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
