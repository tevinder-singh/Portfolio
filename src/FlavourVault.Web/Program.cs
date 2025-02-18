using Asp.Versioning;
using Asp.Versioning.Builder;
using FlavourVault.OutboxDispatcher;
using FlavourVault.OutboxDispatcher.Dispatchers;
using FlavourVault.OutboxDispatcher.Interfaces;
using FlavourVault.Recipes;
using FlavourVault.Security;
using FlavourVault.SharedCore.Domain.Common;
using FlavourVault.SharedCore.Extensions;
using FlavourVault.SharedCore.Interfaces;
using FlavourVault.SharedCore.Middleware;
using FlavourVault.SharedCore.RequestValidations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

List<Assembly> assemblies = [typeof(Program).Assembly];

services.AddHttpContextAccessor();
services.AddOpenApi();
services.AddEndpointsApiExplorer();

// Adds services for using Problem Details format
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddScoped<IUserContext, UserContext>();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["Auth:Audience"],
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:JwtSecret"])),
        };
    });

services.AddAuthorization();

services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

//Azure Service Bus
services.AddAzureClients(azureBuilder =>
{
    var topics = builder.Configuration["MessageBus:Topics"];
    azureBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("AzureServiceBusConnectionString"));
    if (topics != null)
    { 
        foreach (var topic in topics.Split(','))
        {
            azureBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                provider.GetService<ServiceBusClient>().CreateSender(topic!)
            )
            .WithName(topic);
        }
    }
});


//outbox services
builder.Services.ConfigureOptions<ResilienceOptionsSetup>();
services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
services.AddSingleton<IMessageBusDispatcher, AzureServiceBusDispatcher>();

//register domain specific services
services.AddRecipesDomainServices(builder.Configuration, assemblies);
services.AddSecurityDomainServices(builder.Configuration, assemblies);
services.AddAuditDomainServices(builder.Configuration, assemblies);

//add automapper
services.AddAutoMapper(assemblies);

//register fluent validations
services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

// Set up MediatR
builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssemblies(assemblies.ToArray());    
});

//validate MediatR commands with fluent validation
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<EnrichLogWithCorrelationId>();

app.UseSerilogRequestLogging();

app.UseHsts();
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();

//app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

//app.MapControllers();
app.Run();