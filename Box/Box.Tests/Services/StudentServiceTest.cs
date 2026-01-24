using Xunit;
using Microsoft.EntityFrameworkCore;
using Box.Infrastructure.Data;
using Box.Infrastructure.Repositories;
using Box.Application.Services;

public class StudentServiceTests
{
    [Fact]
    public async Task GetStudents_Returns_Students_With_Courses()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);
        DbSeeder.Seed(db);

        var repo = new StudentRepository(db);
        var service = new StudentService(repo);

        var result = await service.GetStudentsAsync(offset: 0, limit: 10);

        // Assert
        Assert.NotNull(result);
        if (result.Data != null)
        {
            Assert.True(result.Data.Items.Any());
            Assert.True(result.Data.Items.First().CourseCount > 0);
        }

    }
}
