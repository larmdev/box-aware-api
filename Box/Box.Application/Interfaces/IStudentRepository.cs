using Box.Domain.Entities;

namespace Box.Application.Interfaces;

public interface IStudentRepository
{
    Task<(List<Student> Items, int Total)> GetStudentsAsync(
        int offset,
        int limit
    );

    Task<Student?> GetStudentByIdAsync(int id);
}
