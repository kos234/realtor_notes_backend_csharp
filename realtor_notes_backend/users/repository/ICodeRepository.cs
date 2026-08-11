using realtor_notes_backend.users.dtos;

namespace realtor_notes_backend.users.repository;

public interface ICodeRepository
{
    public Task SaveCode(int userId, string label, string code, int duration);
    public Task<CodeDTO?> GetCode(int userId, string label);
    public Task DeleteCode(int userId, string label);
}