using realtor_notes_backend.users.model;
using realtor_notes_backend.users.repository;
using realtor_notes_backend.utils;

namespace realtor_notes_backend.users.service;

public interface ISessionService
{
    Task<Session> CreateNewSession(int userId, bool isTrustDevice, ClientPlatformInfo clientPlatformInfo);
    Task<Session?> GetSessionForDevice(int userId, string deviceId);
    Task<Session> CreateNewSessionFromSession(Session existingSession, ClientPlatformInfo platformInfo);
    Task SetSessionEntered(int userId, long sessionId);
    Task<Session?> GetByRefresh(string refresh);
    Task<Session?> GetSessionById(int userId, long sessionId);
}

public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    public async Task<Session> CreateNewSession(int userId, bool isTrustDevice, ClientPlatformInfo clientPlatformInfo)
    {
        Session session = new Session()
        {
            UserId = userId,
            Refresh = Utils.Random16Bytes(),
            EntryTime = DateTimeOffset.Now,
            DeviceId = isTrustDevice ? clientPlatformInfo.DeviceId : null,
            DeviceName = clientPlatformInfo.DeviceName,
            Platform = clientPlatformInfo.Platform
        };
        
        await sessionRepository.Save(session);
        return session;
    }

    public Task<Session?> GetSessionForDevice(int userId, string deviceId)
    {
        return sessionRepository.GetByTrustDevice(userId, deviceId);
    }

    public async Task<Session> CreateNewSessionFromSession(Session existingSession, ClientPlatformInfo platformInfo)
    {
        Session session = new Session()
        {
            Id = existingSession.Id,
            DeviceId = existingSession.DeviceId,
            UserId = existingSession.UserId,
            EntryTime = DateTimeOffset.Now,
            DeviceName = platformInfo.DeviceName,
            Platform = platformInfo.Platform,
            Refresh = Utils.Random16Bytes(),
            IsEntered = existingSession.IsEntered,
        };
        
        await sessionRepository.Save(session);
        return session;
    }

    public async Task SetSessionEntered(int userId, long sessionId)
    {
        var session = await sessionRepository.GetById(sessionId);
        if(session == null || session.UserId != userId)
            return;
        session.IsEntered = true;
        await sessionRepository.Save(session);
    }

    public Task<Session?> GetByRefresh(string refresh)
    {
        return sessionRepository.GetByRefresh(refresh);
    }

    public async Task<Session?> GetSessionById(int userId, long sessionId)
    {
        var session = await sessionRepository.GetById(sessionId);
        if(session != null && session.UserId != userId)
            session = null;
        return session;
    }
}