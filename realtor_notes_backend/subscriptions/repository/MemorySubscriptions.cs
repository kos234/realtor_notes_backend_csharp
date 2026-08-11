using realtor_notes_backend.subscriptions.consts;
using realtor_notes_backend.subscriptions.model;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.subscriptions.repository;

public class MemorySubscriptions : ISubscriptionRepository
{
    private List<Subscription> subscriptions = new List<Subscription>()
    {
        new Subscription()
        {
            Id = 1,
            Name = "Пробная подписка",
            Description = "Используется для разработки",
            Price = 0,
            IsTrial = true,
            BillingPeriod = BillingPeriod.Month,
            PeriodCount = 1,
            Features = Enum.GetValues<Feature>()
        },
        new Subscription()
        {
            Id = 1,
            Name = "Только заметки",
            Description = "Только заметки и ничего больше",
            Price = 0,
            IsTrial = false,
            BillingPeriod = BillingPeriod.Month,
            PeriodCount = 1,
            Features = [Feature.CrudNotes]
        }
    };
    
    public Task<Subscription> GetTrialSubscription()
    {
        return Task.FromResult(subscriptions.First(x => x.IsTrial));
    }

    public Task<Subscription?> GetSubscriptionById(int id)
    {
        return Task.FromResult(subscriptions.FirstOrDefault(x => x.Id == id));
    }

    public Task<IReadOnlyCollection<Subscription>> GetSubscriptions()
    {
        return Task.FromResult((IReadOnlyCollection<Subscription>)subscriptions);
    }
}