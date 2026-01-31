using Box.Application.Interfaces;
using Box.Domain.Entities;
using Box.Application.Dtos;
using Box.Application.Common;

namespace Box.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public StudentService(
        IStudentRepository repo,
        ICurrentUserService currentUser
        )
    {
        _repo = repo;
        _currentUser = currentUser;
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

    public async Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id)
    {

        string name = _currentUser.Name;
        Guid userId = _currentUser.UserId;

        var student = await _repo.GetStudentByIdAsync(id);

        if (student == null) return ApiResponse<StudentDto>.Error(500, "student id is not found");

        var item = new StudentDto()
        {
            Id = student.Id,
            StudentCode = student.StudentCode,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Age = student.Age
        };

        return ApiResponse<StudentDto>.Success(item);
    }


}
