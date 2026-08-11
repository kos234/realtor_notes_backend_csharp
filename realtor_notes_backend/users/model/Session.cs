namespace realtor_notes_backend.users.model;

public class Session
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? Platform { get; set; }
    public required string Refresh { get; set; }
    public bool IsEntered { get; set; }
    public DateTimeOffset EntryTime { get; set; }
}