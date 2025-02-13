namespace FlavourVault.Recipes.BackgroundServices;

internal class OutboxProcessor: IOutboxProcessor
{
    private const int BatchSize = 1000;

    public async Task ProcessPendingNotifications()
    {
        //send messages to service bus in micro service architecture

        //send messages using mediatr for modular monolith
    }
}