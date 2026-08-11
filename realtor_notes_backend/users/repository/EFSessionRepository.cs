using Microsoft.EntityFrameworkCore;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public class EFSessionRepository : ISessionRepository
{
    private readonly UsersDBContext usersDBContext;

    public EFSessionRepository(UsersDBContext usersDbContext)
    {
        usersDBContext = usersDbContext;
    }

    public async Task Save(Session session)
    {
        await usersDBContext.Sessions.AddAsync(session);
    }

    public async Task<Session?> GetById(long id)
    {
        return await usersDBContext.Sessions.FindAsync(id);
    }

    public async Task<Session?> GetByRefresh(string refresh)
    {
        return await usersDBContext.Sessions.Where(vl => vl.Refresh == refresh).FirstOrDefaultAsync();
    }

    public async Task<Session?> GetByTrustDevice(int userId, string deviceId)
    {
        return await usersDBContext.Sessions.Where(vl => vl.UserId == userId && vl.DeviceId == deviceId && vl.DeviceId != null).FirstOrDefaultAsync();
    }
}