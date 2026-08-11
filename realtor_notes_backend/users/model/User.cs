namespace realtor_notes_backend.users.model;

public class User
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsConfirmEmail { get; set; } = false;
    public string? TwoFaSecret { get; set; }
}