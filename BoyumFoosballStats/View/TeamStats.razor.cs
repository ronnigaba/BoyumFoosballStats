using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.View
{
    public partial class TeamStats : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var blobHelper = new AzureBlobStorageHelper();
            var allMatches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
            var teamStatsController = new TeamStatisticsController();
            var teamStats = teamStatsController.CalculateTeamStats(allMatches);
            _viewModel.TeamStatistics = teamStats;
            await blobHelper.UploadList<TeamStatistics>(teamStats, AzureBlobStorageHelper.DefaultEloFileName, true);
        }
    }
}
