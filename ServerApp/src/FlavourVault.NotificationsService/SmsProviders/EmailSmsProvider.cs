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

namespace FlavourVault.NotificationsService.SmsProviders;
internal sealed class EmailSmsProvider : ISmsProvider
{
    private readonly ISecretProvider _secretProvider;
    private readonly EmailSmsOptions _settings;
    private readonly ILogger<EmailSmsProvider> _logger;

    public EmailSmsProvider(ILogger<EmailSmsProvider> logger, ISecretProvider secretProvider, IOptions<EmailSmsOptions> options)
    {
        _logger = logger;
        _secretProvider = secretProvider;
        _settings = options.Value;
    }

    public async Task<Result> SendAsync(SmsNotification sms, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient(_settings.SmtpHostAddress);        
        await SetupSmtpClient(smtpClient);
        using var mailMessage = new MailMessage();            
        mailMessage.From = new MailAddress(_settings.SenderEmailAddress);
        mailMessage.To.Add(new MailAddress(string.Format(_settings.RecipientEmailAddress, sms.RecipientPhoneNumber)));
        mailMessage.Body = sms.Message;
        smtpClient.SendAsync(mailMessage, cancellationToken);

        return Result.Success();
    }

    private async Task SetupSmtpClient(SmtpClient client)
    {
        if (_settings.Port.HasValue)
            client.Port = _settings.Port.Value;

        var username = await _secretProvider.GetAsync("EmailSMS.UserName");
        if (username.IsNotEmpty())
        {
            var password= await _secretProvider.GetAsync("EmailSMS.Password");
            client.Credentials = new NetworkCredential(username, password);
        }

        client.EnableSsl = _settings.UseSSL;
    }
}
