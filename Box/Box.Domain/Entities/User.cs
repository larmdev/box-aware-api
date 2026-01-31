using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Box.Domain.Entities;

[Index(nameof(UserId))]
public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = default!;
    public string PasswordSalt { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
