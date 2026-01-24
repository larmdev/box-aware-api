namespace Box.Domain.Entities;

public class Course
{
    public int Id { get; set; }                       
    public string CourseCode { get; set; } = default!;
    public string CourseName { get; set; } = default!;

    public List<Enrollment> Enrollments { get; set; } = new();
}
