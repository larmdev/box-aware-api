using Box.Application.Dtos;
namespace Box.Application.Interfaces;

public interface ITodoService
{
    Task<ApiWrapperDto<TodoResponseDto>> GetTodoAsync();
    Task<ApiWrapperDto<TodoResponseDto>> GetTodoByIdAsync(int id);

}
