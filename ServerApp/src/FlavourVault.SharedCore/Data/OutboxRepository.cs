using FlavourVault.SharedCore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlavourVault.SharedCore.Data;
public class OutboxRepository : IOutboxRepository
{
    private readonly DbContext _dbContext;        
    public OutboxRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<T>(T message)
    {
        var outboxMessage = new OutboxMessage
        (
            typeof(T).FullName,
            typeof(T).Name,
            JsonSerializer.Serialize(message)                
        );
        await _dbContext.AddAsync(outboxMessage);
    }
}
