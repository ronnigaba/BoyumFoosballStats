using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.View
{
    public partial class SeasonOverview : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _viewModel.WinRateFilterOptions = new List<PlayerPosition?> { PlayerPosition.Attacker, PlayerPosition.Defender, null };
            _viewModel.analysisHelper = new MatchAnalysisHelper();
            _viewModel.blobHelper = new AzureBlobStorageHelper();
            var allMatches = await _viewModel.blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
            _viewModel.AllMatches = allMatches;
            _viewModel.MatchesBySeason = _viewModel.analysisHelper.SortMatchesBySeason(allMatches);
            var viewModelMatchesBySeason = _viewModel.MatchesBySeason.OrderByDescending(x => x.Key).ToList();
            _viewModel.SeasonFilterOptions.Clear();
            foreach (var grouping in viewModelMatchesBySeason)
            {
                _viewModel.SeasonFilterOptions.Add(grouping.Key);
            }

            _viewModel.SeasonFilterValue = _viewModel.SeasonFilterOptions.First();
            _viewModel.Matches = viewModelMatchesBySeason.First().ToList();
            _viewModel.CalculateStats();
            _viewModel.CalculateTeamStatistics();
        }
    }
}
