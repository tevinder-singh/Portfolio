namespace FlavourVault.SharedCore.Data;
public interface IOutboxRepository
{
    Task Add<T>(T message);
}
