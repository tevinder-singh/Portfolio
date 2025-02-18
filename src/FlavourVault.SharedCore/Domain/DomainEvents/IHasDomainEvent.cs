using FlavourVault.SharedCore.Domain.Common;

namespace FlavourVault.SharedCore.Domain.DomainEvents;
public interface IHasDomainEvent
{
    IList<DomainEvent> DomainEvents { get; }
}
