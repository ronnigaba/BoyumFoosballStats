using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;

namespace BoyumFoosballStats.View
{
    public partial class InputScores
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _viewModel.BlobStorageHelper = new AzureBlobStorageHelper();
            _viewModel.TeamStatisticsController = new TeamStatisticsController();
            _viewModel.EloRatings = await _viewModel.BlobStorageHelper.GetEntries<TeamStatistics>(AzureBlobStorageHelper.DefaultEloFileName);
        }
    }
}
