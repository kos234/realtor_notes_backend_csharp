using realtor_notes_backend.subscriptions.model;

namespace realtor_notes_backend.subscriptions.repository;

public interface ISubscriptionRepository
{
    Task<Subscription> GetTrialSubscription();
    Task<Subscription?> GetSubscriptionById(int id);
    Task<IReadOnlyCollection<Subscription>> GetSubscriptions();
}