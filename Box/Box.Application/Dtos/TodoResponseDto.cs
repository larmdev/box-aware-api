namespace Box.Application.Dtos;

public class TodoResponseDto
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool Completed { get; set; }
}
