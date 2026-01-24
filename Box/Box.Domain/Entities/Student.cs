namespace Box.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
