using Box.Domain.Entities;

namespace Box.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Students.Any()) return;

        // Note: Data Seeding  
        // ===== Students =====
        var student1 = new Student
        {
            Id = 1,
            StudentCode = "ST001",
            FirstName = "Sompong",
            LastName = "Dodee",
            Age = 22
        };

        var student2 = new Student
        {
            Id = 2,
            StudentCode = "ST002",
            FirstName = "Somchai",
            LastName = "Jaidee",
            Age = 20
        };

        db.Students.AddRange(student1, student2);

        // ===== Courses =====
        var course1 = new Course
        {
            Id = 1,
            CourseCode = "CS101",
            CourseName = "Introduction to Programming"
        };

        var course2 = new Course
        {
            Id = 2,
            CourseCode = "CS102",
            CourseName = "Database System"
        };

        db.Courses.AddRange(course1, course2);

        // ===== Enrollments =====
        db.Enrollments.AddRange(
            new Enrollment
            {
                Id = 1,
                StudentId = 1,
                CourseId = 1
            },
            new Enrollment
            {
                Id = 2,
                StudentId = 1,
                CourseId = 2
            },
            new Enrollment
            {
                Id = 3,
                StudentId = 2,
                CourseId = 1
            }
        );

        db.SaveChanges();
    }
}
