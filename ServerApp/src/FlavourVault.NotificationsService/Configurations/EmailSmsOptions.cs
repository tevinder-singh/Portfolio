using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FlavourVault.NotificationsService.Configurations;
public sealed class EmailSmsOptions
{
    public int? Port { get; set; }
    public string SmtpHostAddress { get; set; } = null!;
    public string RecipientEmailAddress { get; set; } = null!;
    public string SenderEmailAddress { get; set; } = null!;
    public bool UseSSL { get; set; }
}

public sealed class EmailSmsOptionsSetup : IConfigureOptions<EmailSmsOptions>
{
    private const string SectionName = "EmailSMS";
    private readonly IConfiguration _configuration;

    public EmailSmsOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(EmailSmsOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
