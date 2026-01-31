namespace Box.Application.Common;
public class ApiResponse<T>
{
    public int Status { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; private set; }

    private ApiResponse(int status, string message, T? data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> Success(T data, int status = 200, string message = "Success")
        => new(status, message, data);
    public static ApiResponse<T> Success(T data, string message = "Success")
        => new(200, message, data);
    public static ApiResponse<T> Success(T data)
        => new(200, "Success", data);
    public static ApiResponse<T> Success()
        => new(200, "Success", default);
    public static ApiResponse<T> Error(int status = 500, string message = "Failed")
        => new(status, message, default);
    public static ApiResponse<T> Error(string message = "Failed")
        => new(400, message, default);
    public static ApiResponse<T> Error()
        => new(400, "Failed", default);
}
