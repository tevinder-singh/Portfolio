using FlavourVault.NotificationsService.Configurations;
using FlavourVault.NotificationsService.Consumers;
using FlavourVault.NotificationsService.EmailProviders;
using FlavourVault.NotificationsService.Interfaces;
using FlavourVault.NotificationsService.SmsProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlavourVault.NotificationsService;

public static class ServiceExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, 
        IConfiguration config,
        List<System.Reflection.Assembly> assemblies)
    {
        services.ConfigureOptions<EmailSmsOptionsSetup>();
        services.ConfigureOptions<SmtpEmailOptionsSetup>();
        services.ConfigureOptions<RabbitMQConsumerOptionsSetup>();

        // Add services to the container.                
        services.AddScoped<ISmsProvider, EmailSmsProvider>();
        services.AddScoped<IEmailProvider, SmtpEmailProvider>();

        //switch between rabbitmq and azure service bus
        //services.AddSingleton<IMessageConsumer, AzureServiceBusConsumer>();
        services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();

        //backgroud service to consume messages async        
        services.AddHostedService<ConsumerService>();

        assemblies.Add(typeof(ServiceExtensions).Assembly);        
        return services;
    }
}