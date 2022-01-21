using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace BoyumFoosballStats.View
{
    public partial class MatchOverview
    {
        public List<Match> Matches = new List<Match>();
        private AzureBlobStorageHelper blobHelper;
        private RadzenDataGrid<Match> dataGrid;

        protected override async Task OnInitializedAsync()
        {
            blobHelper = new AzureBlobStorageHelper();
            Matches = await blobHelper.GetEntries<Match>(AzureBlobStorageHelper.DefaultMatchesFileName);
        }

        void CellRender(DataGridCellRenderEventArgs<Match> args)
        {
            var backgroundColor = "background-color:";
            var backgroundGreen = backgroundColor + "rgba(0, 171, 28, 0.3);";
            var backgroundRed = backgroundColor + "rgba(255, 99, 71, 0.3);";
            if (args.Column.Property == "Black.Attacker" || args.Column.Property == "Black.Defender" || args.Column.Property == "ScoreBlack")
            {
                args.Attributes.Add("style", $"{(args.Data.WinningTeam.Side == TableSide.Black ? backgroundGreen : backgroundRed)};");
            }
            if (args.Column.Property == "Gray.Attacker" || args.Column.Property == "Gray.Defender" || args.Column.Property == "ScoreGray")
            {
                args.Attributes.Add("style", $"{(args.Data.WinningTeam.Side == TableSide.Gray ? backgroundGreen : backgroundRed)};");
            }
        }

        private async void ConfirmDeleteEntry(Match match)
        {
            if (!await jsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this match?"))
            {
                return;
            }
            Matches.Remove(match);
            await blobHelper.RemoveEntry<Match>(match, AzureBlobStorageHelper.DefaultMatchesFileName);
            await dataGrid.Reload();
        }
    }
}
