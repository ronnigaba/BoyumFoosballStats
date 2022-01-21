using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.View
{
    public partial class PlayerDashboard : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _viewModel.PlayerFilterOption = Enum.GetValues<Player>().ToList();
            _viewModel.analysisHelper = new MatchAnalysisHelper();
            _viewModel.blobHelper = new AzureBlobStorageHelper();
            _viewModel.Matches = await _viewModel.blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        }
    }
}
