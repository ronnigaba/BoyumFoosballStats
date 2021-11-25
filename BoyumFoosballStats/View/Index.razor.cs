using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BoyumFoosballStats.View
{
    public partial class Index : ComponentBase
    {
        protected bool ShowWinrateDrillDownChart = false;
        protected bool ShowMatchesPlayedDrillDownChart = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _viewModel.WinRateFilterOptions = new List<PlayerPosition?> { PlayerPosition.Attacker, PlayerPosition.Defender, null };
            _viewModel.analysisHelper = new MatchAnalysisHelper();
            _viewModel.blobHelper = new AzureBlobStorageHelper();
            _viewModel.Matches = await _viewModel.blobHelper.GetMatches(AzureBlobStorageHelper.DefaultFileName);
            _viewModel.OverallWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches);
            _viewModel.AttackerWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            _viewModel.DefenderWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllPlayers(_viewModel.Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
            //_viewModel.TeamWinrates = _viewModel.analysisHelper.CalculateWinRatesForAllTeams(_viewModel.Matches);
            _viewModel.TableSideWinRates = _viewModel.analysisHelper.CalculateWinRatesForTableSides(_viewModel.Matches);
            _viewModel.OverallMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches);
            _viewModel.AttackerMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches, PlayerPosition.Attacker).OrderBy(x => x.Key);
            _viewModel.DefenderMatchesPlayed = _viewModel.analysisHelper.CalculateMatchesPlayedForAllPlayers(_viewModel.Matches, PlayerPosition.Defender).OrderBy(x => x.Key);
        }

        protected void OnOverallWinrateSeriesClick(SeriesClickEventArgs args)
        {
            _viewModel.DrillDownAttackerWinrates = _viewModel.AttackerWinrates.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            _viewModel.DrillDownDefenderWinrates = _viewModel.DefenderWinrates.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ShowWinrateDrillDownChart = true;
        }

        protected void OnOverallMatchesPlayedSeriesClick(SeriesClickEventArgs args)
        {
            _viewModel.DrillDownAttackerMatchesPlayed = _viewModel.AttackerMatchesPlayed.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            _viewModel.DrillDownDefenderMatchesPlayed = _viewModel.DefenderMatchesPlayed.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ShowMatchesPlayedDrillDownChart = true;
        }

        protected void OnWinrateSeriesClick(SeriesClickEventArgs args)
        {
            _viewModel.DrillDownAttackerWinrates = null;
            _viewModel.DrillDownDefenderWinrates = null;
            ShowWinrateDrillDownChart = false;
        }

        protected void OnMatchesPlayedSeriesClick(SeriesClickEventArgs args)
        {
            _viewModel.DrillDownAttackerMatchesPlayed = null;
            _viewModel.DrillDownDefenderMatchesPlayed = null;
            ShowMatchesPlayedDrillDownChart = false;
        }
    }
}
