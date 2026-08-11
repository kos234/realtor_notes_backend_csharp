using Microsoft.EntityFrameworkCore;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public class EFUserSubscriptionRepository :  IUserSubscriptionRepository
{
    private readonly UsersDBContext usersDBContext;

    public EFUserSubscriptionRepository(UsersDBContext usersDbContext)
    {
        usersDBContext = usersDbContext;
    }

    public async Task<bool> IsUserHaveTrialSubscription(int userId)
    {
        return await usersDBContext.UserSubscriptions.Where(vl => vl.IsTrial).CountAsync() != 0;
    }

    public async Task<UserSubscription?> GetActiveSubscription(int userId)
    {
        return await usersDBContext.UserSubscriptions
            .Where(vl => vl.UserId == userId && !vl.IsCanceled && vl.ExpirationDate >= DateTimeOffset.Now)
            .FirstOrDefaultAsync();
    }

    public async Task Save(UserSubscription userSubscription)
    {
        await usersDBContext.UserSubscriptions.AddAsync(userSubscription);
    }
}