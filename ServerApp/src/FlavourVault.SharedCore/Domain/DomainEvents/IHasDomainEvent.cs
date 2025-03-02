using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.SharedCore.Domain.DomainEvents;
public interface IHasDomainEvent
{
    Guid Id { get; }
    IList<DomainEvent> DomainEvents { get; }
}
