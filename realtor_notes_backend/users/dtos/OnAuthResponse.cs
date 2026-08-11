using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.service;

namespace realtor_notes_backend.users.dtos;

public record OnAuthResponse(string AccessToken, AuthActions[] NextActions)
{
    
}