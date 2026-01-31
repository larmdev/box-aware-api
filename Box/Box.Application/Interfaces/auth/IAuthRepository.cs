using Box.Domain.Entities;

namespace Box.Application.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserLoginAsync(string username);
    Task AddUserAsync(User user);
}