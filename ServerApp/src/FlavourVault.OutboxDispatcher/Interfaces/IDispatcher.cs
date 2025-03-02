using FlavourVault.Results;
using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.OutboxDispatcher.Interfaces;
public interface IDispatcher
{
    Task<Result> DispatchAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);    
}