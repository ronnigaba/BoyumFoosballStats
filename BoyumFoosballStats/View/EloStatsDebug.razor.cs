using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using Kezyma.EloRating;
using Microsoft.AspNetCore.Components;

namespace BoyumFoosballStats.View
{
    public partial class EloStatsDebug : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var analysisHelper = new MatchAnalysisHelper();
            var blobHelper = new AzureBlobStorageHelper();
            var allMatches = await blobHelper.GetMatches(AzureBlobStorageHelper.DefaultFileName);
            Dictionary<string, decimal> eloRatings = new Dictionary<string, decimal>();

            foreach (var match in allMatches)
            {
                var losingBase = match.LosingTeam as TeamBase;
                var winningBase = match.WinningTeam as TeamBase;
                decimal losingElo = 10;
                decimal winningElo = 10;
                var losingBaseGetTeamIdentifier = losingBase.GetTeamIdentifier;
                if (eloRatings.ContainsKey(losingBaseGetTeamIdentifier))
                {
                    losingElo = eloRatings[losingBaseGetTeamIdentifier];
                }

                var winningBaseGetTeamIdentifier = winningBase.GetTeamIdentifier;
                if (eloRatings.ContainsKey(winningBaseGetTeamIdentifier))
                {
                    winningElo = eloRatings[winningBaseGetTeamIdentifier];
                }

                //var newElo = EloCalculator.CalculateElo(winningElo, losingElo, (decimal)match.WinningScore / match.WinningScore, (decimal)match.LosingScore / match.WinningScore);
                var newElo = EloCalculator.CalculateElo(winningElo, losingElo, (decimal)EloCalculator.WIN, (decimal)EloCalculator.LOSE);
                eloRatings[winningBaseGetTeamIdentifier] = newElo[0];
                eloRatings[losingBaseGetTeamIdentifier] = newElo[1];
            }

            int unexpectedWins = 0;
            int expectedWins = 0;
            foreach (var match in allMatches)
            {
                var winningTeamGetTeamIdentifier = match.WinningTeam.GetTeamIdentifier;
                var losingTeamGetTeamIdentifier = match.LosingTeam.GetTeamIdentifier;
                var prediction = EloCalculator.PredictResult(eloRatings[winningTeamGetTeamIdentifier], eloRatings[losingTeamGetTeamIdentifier]);
                if (prediction[0] > prediction[1])
                {
                    expectedWins++;
                }
                else
                {
                    unexpectedWins++;
                }
            }
            
            _viewModel.EloRatings = eloRatings;
        }
    }
}
