using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;

public interface ISessionRepository
{
    Task Save(Session session);
    Task<Session?> GetById(long id);
    Task<Session?> GetByRefresh(string refresh);
    Task<Session?> GetByTrustDevice(int userId, string deviceId);
}