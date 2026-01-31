using Box.Domain.Entities;
using Box.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Box.Application.Interfaces;

namespace Box.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;

    public AuthRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserLoginAsync(string username)
    {
        var query = _db.Users
            .AsNoTracking()
            .Where(i => i.Username == username);

        var item = await query.FirstOrDefaultAsync();

        if (item == null) return null;

        return item;
    }

    public async Task AddUserAsync(User user)
    {
        try
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return;
        }
        catch (DbUpdateException ex)
        {
            // ดึง error จาก PostgreSQL
            var inner = ex.InnerException?.Message;
            throw new Exception(inner ?? ex.Message);
        }
    }
}

