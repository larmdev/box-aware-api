using Box.Domain.Entities;
using Box.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Box.Application.Interfaces;

namespace Box.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;

    public StudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Student> Items, int Total)> GetStudentsAsync(
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

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        var query = _db.Students
            .AsNoTracking()
            .Where(i => i.Id == id);
    
        var item = await query.FirstOrDefaultAsync();

        if (item == null) return null; 

        return item;
    }
}

