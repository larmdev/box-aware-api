namespace Box.Application.Dtos;

public class ApiWrapperDto<T>
{
    public string Url { get; set; } = null!;
    public string Method { get; set; } = null!;
    public T Response { get; set; } = default!;
}
