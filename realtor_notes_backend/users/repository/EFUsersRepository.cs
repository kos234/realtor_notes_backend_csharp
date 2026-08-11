using Microsoft.EntityFrameworkCore;
using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public class EFUsersRepository : IUserRepository
{
    private readonly UsersDBContext usersDBContext;

    public EFUsersRepository(UsersDBContext usersDbContext)
    {
        usersDBContext = usersDbContext;
    }
    
    public async Task Save(User user)
    {
        await usersDBContext.Users.AddAsync(user);
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await usersDBContext.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await usersDBContext.Users.Where(vl => vl.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByLogin(string login)
    {
        return await usersDBContext.Users.Where(vl => vl.Login == login).FirstOrDefaultAsync();
    }

    public async Task<bool> IsUnavailableLogin(string login)
    {
        return await usersDBContext.Users.Where(vl => vl.Login == login).CountAsync() != 0;
    }

    public async Task<bool> IsUnavailableEmail(string email)
    {
        return await usersDBContext.Users.Where(vl => vl.Email == email).CountAsync() != 0;
    }
}