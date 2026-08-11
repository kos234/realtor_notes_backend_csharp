using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.repository;

public interface ITwoFaRepository
{
    public Task SaveTwoFaKey(int userId, TwoFaDto key, int ttl);
    public Task DeleteTwoFaKey(int userId);
    public Task<TwoFaDto?> GetTwoFaKey(int userId);
}