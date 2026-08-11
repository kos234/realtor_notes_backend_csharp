using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public interface IUserSubscriptionRepository
{
    Task<bool> IsUserHaveTrialSubscription(int userId);
    Task<UserSubscription?> GetActiveSubscription(int userId);
    Task Save(UserSubscription userSubscription);
}