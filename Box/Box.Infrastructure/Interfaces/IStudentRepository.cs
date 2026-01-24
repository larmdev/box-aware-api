using Box.Domain.Entities;

namespace Box.Infrastructure.Interfaces;

public interface IStudentRepository
{
    Task<(List<Student> Items, int Total)> GetStudentsAsync(
        int offset,
        int limit);
}

