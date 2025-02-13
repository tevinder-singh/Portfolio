namespace FlavourVault.Recipes.BackgroundServices
{
    internal interface IOutboxProcessor
    {
        Task ProcessPendingNotifications();
    }
}
