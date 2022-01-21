using System.Security.Cryptography.X509Certificates;
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
            var blobHelper = new AzureBlobStorageHelper();
            var allMatches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
            var eloRatingHelper = new EloRatingHelper();
            var eloRatings = eloRatingHelper.CalculateTeamEloRatings(allMatches);
            _viewModel.EloRatings = eloRatings;
            await blobHelper.UploadList<TeamEloRating>(eloRatings, AzureBlobStorageHelper.DefaultEloFileName, true);
        }
    }
}
