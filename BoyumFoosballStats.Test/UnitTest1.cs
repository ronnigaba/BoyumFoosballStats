using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using Xunit;

namespace BoyumFoosballStats.Test;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        var sut = new PlayerStatisticsController();
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        sut.CalculatePlayerRatingUsingAlgorhitm(matches);
    }
}