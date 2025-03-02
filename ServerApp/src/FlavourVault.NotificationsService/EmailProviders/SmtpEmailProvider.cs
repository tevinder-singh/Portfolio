using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Configurations;
using FlavourVault.NotificationsService.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Extensions;
using FlavourVault.SharedCore.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FlavourVault.NotificationsService.EmailProviders;
internal sealed class SmtpEmailProvider : IEmailProvider
{
    private readonly ISecretProvider _secretProvider;
    private readonly SmtpEmailOptions _settings;
    private readonly ILogger<SmtpEmailProvider> _logger;

    public SmtpEmailProvider(ILogger<SmtpEmailProvider> logger, ISecretProvider secretProvider, IOptions<SmtpEmailOptions> options)
    {
        _logger = logger;
        _secretProvider = secretProvider;
        _settings = options.Value;
    }

    public async Task<Result> SendAsync(EmailNotification email, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient(_settings.SmtpHostAddress);
        await SetupSmtpClient(smtpClient);
        using var mailMessage = new MailMessage();        
        mailMessage.From = email.From;
        foreach (var item in email.To)
        {
            mailMessage.To.Add(item);
        }
            
        mailMessage.Subject = email.Subject;
        mailMessage.Body = email.Body;
        mailMessage.IsBodyHtml = email.IsBodyHtml;

        return Result.Success();
    }

    private async Task SetupSmtpClient(SmtpClient client)
    {
        if (_settings.Port.HasValue)
            client.Port = _settings.Port.Value;

        var username = await _secretProvider.GetAsync("SmtpEmail.UserName");
        if (username.IsNotEmpty())
        {
            var password = await _secretProvider.GetAsync("SmtpEmail.Password");
            client.Credentials = new NetworkCredential(username, password);
        }

        client.EnableSsl = _settings.UseSSL;
    }
}
