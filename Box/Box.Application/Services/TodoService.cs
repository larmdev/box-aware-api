using Box.Application.Dtos;
using Box.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Box.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoApiClient _client;
    private readonly IConfiguration _config;
    private readonly string _baseUrl;

    public TodoService(IConfiguration config, ITodoApiClient client)
    {
        _config = config;
        _client = client;
        _baseUrl = _config["ExternalApis:TodoApi:BaseUrl"] ?? "";

    }
    
    public async Task<ApiWrapperDto<TodoResponseDto>> GetTodoAsync()
    {
        var random = new Random();
        int number = random.Next(1, 101);

        var todo = await _client.GetTodoAsync(number.ToString());

        return new ApiWrapperDto<TodoResponseDto>
        {
            Url = $"{_baseUrl}/todos/{number}",
            Method = "GET",
            Response = todo
        };
    }

        public async Task<ApiWrapperDto<TodoResponseDto>> GetTodoByIdAsync(int id)
    {
        var todo = await _client.GetTodoAsync(id.ToString());

        return new ApiWrapperDto<TodoResponseDto>
        {
            Url = $"{_baseUrl}/todos/{id}",
            Method = "GET",
            Response = todo
        };
    }
}
