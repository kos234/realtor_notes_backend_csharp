using realtor_notes_backend.users.model;
using realtor_notes_backend.users.repository;

namespace realtor_notes_backend.users.service;

public interface IUserService
{
    Task<bool> IsUnavailableLogin(string login);
    Task<bool> IsUnavailableEmail(string email);
    Task<User> CreateNewUser(string login, string email, string passwordHash);
    Task<User?> GetUserByLogin(string login);
    Task<User?> GetUserByEmail(string email);
    Task SetTwoFaSecret(int userId, string key);
    Task<User?> GetUserById(int userId);
    Task SetEmailChecked(int userId);
}

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public Task<bool> IsUnavailableLogin(string login)
    {
        return userRepository.IsUnavailableLogin(login);
    }

    public Task<bool> IsUnavailableEmail(string email)
    {
        return userRepository.IsUnavailableEmail(email);
    }
    
    public async Task<User> CreateNewUser(string login, string email, string passwordHash)
    {
        var user = new User()
        {
            Login = login,
            Email = email,
            PasswordHash = passwordHash,
            IsConfirmEmail = false,
            TwoFaSecret = null
        };
        await userRepository.Save(user);
        return user;
    }

    public Task<User?> GetUserByLogin(string login)
    {
        return userRepository.GetUserByLogin(login);
    }

    public Task<User?> GetUserByEmail(string email)
    {
        return userRepository.GetUserByEmail(email);
    }

    public async Task SetTwoFaSecret(int userId, string key)
    {
        var user = await GetUserById(userId);
        if(user == null)
            return;
        user.TwoFaSecret = key;
        await userRepository.Save(user);
    }

    public Task<User?> GetUserById(int userId)
    {
        return userRepository.GetUserById(userId);
    }

    public async Task SetEmailChecked(int userId)
    {
        var user = await GetUserById(userId);
        if(user == null)
            return;
        user.IsConfirmEmail = true;
        await userRepository.Save(user);
    }
}