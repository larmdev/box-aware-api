namespace Box.Application.Dtos;
public class StudentWithCourseDto
{
    public string StudentCode { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public int Age { get; set; }
    public int CourseCount { get; set; }

    public List<CourseDto> Courses { get; set; } = new();
}

public class CourseDto
{
    public string CourseCode { get; set; } = default!;
    public string CourseName { get; set; } = default!;
}
