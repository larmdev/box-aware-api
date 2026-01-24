using Box.Application.Dtos;
namespace Box.Application.Interfaces;

public interface ITodoApiClient
{
    Task<TodoResponseDto> GetTodoAsync(string id);
}
