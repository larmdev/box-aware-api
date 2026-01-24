using Box.Domain.Entities;
using Box.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Box.Infrastructure.Interfaces;

namespace Box.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;

    public StudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Student>, int)> GetStudentsAsync(
        int offset,
        int limit)
    {
        var query = _db.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course);

        var total = await query.CountAsync();

        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }
}

