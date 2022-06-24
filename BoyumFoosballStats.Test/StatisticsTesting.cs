using System.Linq;
using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Xunit;

namespace BoyumFoosballStats.Test;

public class StatisticsTesting
{
    [Fact]
    public async void CalculatePlayerIdealMatch()
    {
        var player = Player.Ronni;
        var controller = new PlayerStatisticsController();
        var blobHelper = new AzureBlobStorageHelper();
        var matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        var result = controller.CalculatePlayerStatistics(matches, player);

        var idealOpponents = result.WinRateByOpponent.OrderByDescending(x => x.Value).Take(2).ToList();
        var idealMatch = $"The ideal match for {player} is played on a {result.WinRateByWeekday.MaxBy(x=>x.Value).Key} " +
                         $"with teammate {result.WinRateByTeammate.MaxBy(x=>x.Value).Key} against {idealOpponents.First().Key} and {idealOpponents.Last().Key}";
    }
}