using Xunit;
using Box.Application.Services;

public class RankServiceTests
{
    [Fact]
    public void Process_Returns_Duplicated_And_Sorted_Data()
    {
        var service = new RankService();

        RankRequestDto dto = new RankRequestDto
        {
            P1 = "A,B,1,2,1,AA,3,5,BB,4,2,4,AA,B"
        };

        var result = service.Process(dto);
        Assert.True(result.Status == 200);

        if (result.Data != null)
        {
            Assert.Equal(5, result.Data.Count);
            Assert.Equal("AA", result.Data[0].Rank);
            Assert.Equal("B", result.Data[1].Rank);
            Assert.Equal("1", result.Data[2].Rank);
        }
    }
}
