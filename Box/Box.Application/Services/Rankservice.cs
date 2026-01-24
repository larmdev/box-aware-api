using Box.Application.Dtos;
using Box.Application.Interfaces;
using Box.Application.Common;

public class RankService : IRankService
{
    public ApiResponse<List<RankResponseDto>> Process(RankRequestDto request)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(request.P1))
            return ApiResponse<List<RankResponseDto>>.Error("p1 is required");

        if (request.P1.Length > 99)
            return ApiResponse<List<RankResponseDto>>.Error("p1 length must not exceed 99");

        // 1. Split
        var values = request.P1
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());

        // 2. เอาเฉพาะค่าที่ซ้ำ
        var duplicated = values
            .GroupBy(x => x)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .ToList();

        // 3. ตัวอักษร
        var letters = duplicated
            .Where(x => x.All(char.IsLetter))
            .OrderBy(x => x);

        // 4. ตัวเลข
        var numbers = duplicated
            .Where(x => int.TryParse(x, out _))
            .Select(int.Parse)
            .OrderBy(x => x)
            .Select(x => x.ToString());

        // 5. รวม + map response
        var result = letters
            .Concat(numbers)
            .Select(x => new RankResponseDto { Rank = x })
            .ToList();

        return ApiResponse<List<RankResponseDto>>.Success(result);
    }
}
