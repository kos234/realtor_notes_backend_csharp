using realtor_notes_backend.users.consts;
using realtor_notes_backend.users.model;
using realtor_notes_backend.users.service;

namespace realtor_notes_backend.users.dtos;

public record RegisterResponseDTO(User User, Session Session, bool IsFullAccess, string? TwoFaUrl, AuthActions[] NextActions)
{
    
}