using Box.Application.Dtos;
using Box.Application.Common;

namespace Box.Application.Interfaces;
public interface IRankService
{
    ApiResponse<List<RankResponseDto>> Process(RankRequestDto request);
}