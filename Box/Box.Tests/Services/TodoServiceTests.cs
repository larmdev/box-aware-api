using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Box.Application.Services;
using Box.Application.Interfaces;
using Box.Application.Dtos;

public class TodoServiceTests
{
    [Fact]
    public async Task GetTodoAsync_Returns_ApiWrapper_With_Response()
    {
        var mockClient = new Mock<ITodoApiClient>();
        var mockConfig = new Mock<IConfiguration>();

        mockClient
            .Setup(x => x.GetTodoAsync(It.IsAny<string>()))
            .ReturnsAsync(new TodoResponseDto
            {
                UserId = 1,
                Id = 1,
                Title = "test todo",
                Completed = false
            });

        var service = new TodoService(
            mockConfig.Object,
            mockClient.Object
        );

        var result = await service.GetTodoAsync();

        Assert.NotNull(result);
        Assert.Equal("GET", result.Method);
        Assert.NotNull(result.Response);

        mockClient.Verify(
            x => x.GetTodoAsync(It.IsAny<string>()),
            Times.Once
        );
    }
}
