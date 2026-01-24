using Box.Application.Interfaces;
using Box.Domain.Entities;
using Box.Application.Dtos;
using Box.Application.Common;

namespace Box.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<SearchResponse<StudentWithCourseDto>> GetStudentsAsync(
            int offset,
            int limit)
    {
        var (students, total) = await _repo.GetStudentsAsync(offset, limit);

        var items = students.Select(s => new StudentWithCourseDto
        {
            StudentCode = s.StudentCode,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Age = s.Age,
            CourseCount = s.Enrollments.Count,
            Courses = s.Enrollments.Select(e => new CourseDto
            {
                CourseCode = e.Course.CourseCode,
                CourseName = e.Course.CourseName
            }).ToList()
        }).ToList();

        return SearchResponse<StudentWithCourseDto>.Success(
            items,
            total,
            offset,
            limit
        );
    }


}
