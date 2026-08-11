using realtor_notes_backend.subscriptions.model;
using realtor_notes_backend.subscriptions.repository;
using realtor_notes_backend.subscriptions.service;

namespace realtor_notes_backend.subscriptions;

public static class SubscriptionExtends
{
    public static void UseSubscriptionModule(this IHostApplicationBuilder app)
    {
        app.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        app.Services.AddScoped<ISubscriptionRepository, MemorySubscriptions>();
    }
}