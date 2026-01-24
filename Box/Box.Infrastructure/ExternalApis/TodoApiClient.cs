using System.Net.Http.Json;
using Box.Application.Dtos;
using Box.Application.Interfaces;

namespace Box.Infrastructure.ExternalApis;

public class TodoApiClient : ITodoApiClient
{
    private readonly HttpClient _http;

    public TodoApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<TodoResponseDto> GetTodoAsync(string id)
    {
        return await _http.GetFromJsonAsync<TodoResponseDto>($"todos/{id}")
            ?? throw new Exception("Failed to fetch todo item");
    }
}
