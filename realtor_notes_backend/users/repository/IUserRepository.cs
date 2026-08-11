using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public interface IUserRepository
{
    Task Save(User user);
    Task<User?> GetUserById(int userId); 
    Task<User?> GetUserByEmail(string email); 
    Task<User?> GetUserByLogin(string login); 
    Task<bool> IsUnavailableLogin(string login);
    Task<bool> IsUnavailableEmail(string email);
}