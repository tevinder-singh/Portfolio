using FlavourVault.Notification.Contracts;
using FlavourVault.NotificationsService.Interfaces;
using FlavourVault.Results;
using FlavourVault.SharedCore.Extensions;
using FlavourVault.SharedCore.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FlavourVault.NotificationsService.SmsProviders;
internal sealed class ClickatellSmsProvider (ILogger<ClickatellSmsProvider> logger, ISecretProvider secretProvider, IHttpClientFactory httpClientFactory) : ISmsProvider
{
    public async Task<Result> SendAsync(SmsNotification sms, CancellationToken cancellationToken)
    {
        var connectionString = await secretProvider.GetAsync("ClickatellConnectionString");

        var urlStringBuilder = new StringBuilder(connectionString);
        urlStringBuilder.Append("&to=");
        urlStringBuilder.Append(sms.RecipientPhoneNumber);
        urlStringBuilder.Append("&text=");
        urlStringBuilder.Append(sms.Message);
        using var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, urlStringBuilder.ToString());

        var client = GetClient();
        var response = await client.SendAsync(requestMessage, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!IsResponseSuccessful(data))
                return Result.Failure("SmsError", data);

            return Result.Success();
        }
        else
        {
            return Result.Failure("SmsError", await response.Content.ReadAsStringAsync(cancellationToken));
        }        
    }

    private HttpClient GetClient()
    {
        return httpClientFactory.CreateClient("external");
    }

    private static bool IsResponseSuccessful(string response)
    {
        /* Successful API response:                             
            ID: < message ID >
            ID: 6422e12c1e10cdda68aec75276fae3f6

            Error response:
            ERR: < error code >, < error description >
            ERR: 105, Invalid Destination Address           
        */

        return response.IsNotEmpty() &&
                response.StartsWith("ID:", StringComparison.InvariantCultureIgnoreCase);
    }
}
