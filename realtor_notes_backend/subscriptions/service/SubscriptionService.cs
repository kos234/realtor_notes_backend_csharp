using realtor_notes_backend.subscriptions.model;
using realtor_notes_backend.subscriptions.repository;

namespace realtor_notes_backend.subscriptions.service;

public class SubscriptionService (ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public Task<Subscription> GetTrialSubscription()
    {
        return subscriptionRepository.GetTrialSubscription();
    }

    public Task<Subscription?> GetSubscriptionById(int id)
    {
        return subscriptionRepository.GetSubscriptionById(id);
    }

    public Task<IReadOnlyCollection<Subscription>> GetSubscriptions()
    {
        return subscriptionRepository.GetSubscriptions();
    }
}