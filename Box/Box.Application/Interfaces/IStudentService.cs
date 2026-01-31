using Box.Domain.Entities;
using Box.Application.Dtos;
using Box.Application.Common;

namespace Box.Application.Interfaces;

public interface IStudentService
{
    Task<SearchResponse<StudentWithCourseDto>> GetStudentsAsync(
        int offset,
        int limit);

    Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id);
}