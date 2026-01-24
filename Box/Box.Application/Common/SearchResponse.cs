namespace Box.Application.Common;

public class SearchResponse<T>
{
    public SearchData<T>? Data { get; set; }
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ResponseError> Errors { get; set; } = new();

    public static SearchResponse<T> Success(
        List<T> items,
        int total,
        int offset,
        int limit)
    {
        return new SearchResponse<T>
        {
            Status = 200,
            Message = "Success",
            Data = new SearchData<T>(items, total, offset, limit)
        };
    }

    public static SearchResponse<T> Error(string message, int status = 500)
    {
        return new SearchResponse<T>
        {
            Status = status,
            Message = message,
            Errors =
            {
                new ResponseError { ErrorMessage = message }
            }
        };
    }
}

public class SearchData<T>
{
    public int Total { get; }
    public List<T> Items { get; }

    public int Offset { get; }
    public int Limit { get; }

    public int StartRow { get; }
    public int EndRow { get; }
    public int TotalPage { get; }
    public int CurrentPage { get; }

    public bool EnablePrevious { get; }
    public bool EnableNext { get; }

    public SearchData(List<T> items, int total, int offset, int limit)
    {
        Items = items;
        Total = total;
        Offset = offset;
        Limit = limit;

        CurrentPage = (offset / limit) + 1;
        TotalPage = (int)Math.Ceiling(total / (double)limit);

        StartRow = total == 0 ? 0 : offset + 1;
        EndRow = offset + items.Count;

        EnablePrevious = CurrentPage > 1;
        EnableNext = CurrentPage < TotalPage;
    }
}

public class ResponseError
{
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ListData<T>
{
    public int Total { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }

    public int TotalPage =>
        (int)Math.Ceiling((double)Total / Limit);

    public int CurrentPage => (Offset / Limit) + 1;

    public List<T> Items { get; set; } = new();
}

