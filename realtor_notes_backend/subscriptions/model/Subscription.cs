using realtor_notes_backend.subscriptions.consts;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.subscriptions.model;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsTrial { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    public int PeriodCount { get; set; } = 1;
    
    public IReadOnlyList<Feature> Features { get; set; } = new List<Feature>();
}