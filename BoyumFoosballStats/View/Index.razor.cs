using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model.Enums;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.View
{
    public partial class Index : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _viewModel.WinRateFilterOptions = new List<PlayerPosition?> { PlayerPosition.Attacker, PlayerPosition.Defender, null };
            _viewModel.analysisHelper = new MatchAnalysisHelper();
            _viewModel.blobHelper = new AzureBlobStorageHelper();
            _viewModel.Matches = await _viewModel.blobHelper.GetMatches(AzureBlobStorageHelper.DefaultFileName);
            _viewModel.PlayerWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches);
            //_viewModel.TeamWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllTeams(_viewModel.Matches);
            _viewModel.TableSideWinRates = _viewModel.analysisHelper.CalculateWinRatesForTableSides(_viewModel.Matches);
            _viewModel.PlayerMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches);

        }
    }
}
