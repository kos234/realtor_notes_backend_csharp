using realtor_notes_backend.subscriptions.consts;
using realtor_notes_backend.subscriptions.model;
using realtor_notes_backend.subscriptions.service;
using realtor_notes_backend.users.model;
using realtor_notes_backend.users.repository;

namespace realtor_notes_backend.users.service;

public interface IUserSubscriptionService
{
    Task<UserSubscription?> GetActiveSubscription(int userId);
    Task<UserSubscription?> ActivateTrialSubscription(int userId);
}

public class UserSubscriptionService(
    ISubscriptionService subscriptionService,
    IUserSubscriptionRepository userSubscriptionRepository
) : IUserSubscriptionService
{
    
    public Task<UserSubscription?> GetActiveSubscription(int userId)
    {
        return userSubscriptionRepository.GetActiveSubscription(userId);
    }

    public async Task<UserSubscription?> ActivateTrialSubscription(int userId)
    {
        var res = await userSubscriptionRepository.IsUserHaveTrialSubscription(userId);
        if (res)
            return null;
        var trial = await subscriptionService.GetTrialSubscription();
        var subscription = new UserSubscription
        {
            SubscriptionId = trial.Id,
            Subscription = trial,
            UserId = userId,
            IsAutoRenewing = false,
            StartDate = DateTimeOffset.Now,
            ExpirationDate = GetExpirationDate(trial)
        };
        await userSubscriptionRepository.Save(subscription);
        return subscription;
    }

    private DateTimeOffset GetExpirationDate(Subscription subscription)
    {
        var timeNow = DateTimeOffset.Now;
        switch (subscription.BillingPeriod)
        {
            case BillingPeriod.Month: timeNow = timeNow.AddMonths(subscription.PeriodCount); break;
            case BillingPeriod.Year: timeNow = timeNow.AddYears(subscription.PeriodCount); break;
        }

        return timeNow;
    }
}