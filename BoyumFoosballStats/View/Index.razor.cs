using BoyumFoosballStats.Controller;
using BoyumFoosballStats.Model;
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
            _viewModel.Matches = await _viewModel.blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
            _viewModel.OverallWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches);
            _viewModel.AttackerWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            _viewModel.DefenderWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
            _viewModel.TeamWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllTeams(_viewModel.Matches);
            _viewModel.TableSideWinRates = _viewModel.analysisHelper.CalculateWinRatesForTableSides(_viewModel.Matches);
            _viewModel.OverallMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches);
            _viewModel.AttackerMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            _viewModel.DefenderMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
        }
    }
}
