using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.service;

namespace realtor_notes_backend.users.dtos;

public record MobileOnAuthResponse(string AccessToken, string RefreshToken, string DeviceId, AuthActions[] NextActions)
    : OnAuthResponse(AccessToken, NextActions)
{
}