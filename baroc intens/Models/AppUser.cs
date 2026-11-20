namespace baroc_intens.Models;

public sealed class AppUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string PasswordSalt { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
}
