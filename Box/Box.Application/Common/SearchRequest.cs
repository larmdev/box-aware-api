public class SearchRequest
{
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 10;
    public int CurrentPage => (Offset / Limit) + 1;
}
