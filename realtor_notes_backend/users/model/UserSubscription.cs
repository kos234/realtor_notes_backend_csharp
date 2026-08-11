using realtor_notes_backend.subscriptions.model;

namespace realtor_notes_backend.users.model;

public class UserSubscription
{
    public long Id { get; set; }
    public int UserId { get; set; }
    
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; }
    
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }

    public bool IsAutoRenewing { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsTrial => Subscription?.IsTrial ?? false;
}