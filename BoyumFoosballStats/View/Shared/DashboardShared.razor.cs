using BoyumFoosballStats.Viewmodel;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BoyumFoosballStats.View.Shared
{
    public partial class DashboardShared : ComponentBase
    {
        protected bool ShowWinrateDrillDownChart = false;
        protected bool ShowMatchesPlayedDrillDownChart = false;

        [Parameter]
        public IBaseDashboardViewModel? ViewModel { get; set; }

        protected void OnOverallWinrateSeriesClick(SeriesClickEventArgs args)
        {
            ViewModel.DrillDownAttackerWinrates = ViewModel.AttackerWinrates.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ViewModel.DrillDownDefenderWinrates = ViewModel.DefenderWinrates.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ShowWinrateDrillDownChart = true;
        }

        protected void OnOverallMatchesPlayedSeriesClick(SeriesClickEventArgs args)
        {
            ViewModel.DrillDownAttackerMatchesPlayed = ViewModel.AttackerMatchesPlayed.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ViewModel.DrillDownDefenderMatchesPlayed = ViewModel.DefenderMatchesPlayed.Where(x => x.Key == args.Category).OrderBy(x => x.Key);
            ShowMatchesPlayedDrillDownChart = true;
        }

        protected void OnWinrateSeriesClick(SeriesClickEventArgs args)
        {
            ViewModel.DrillDownAttackerWinrates = null;
            ViewModel.DrillDownDefenderWinrates = null;
            ShowWinrateDrillDownChart = false;
        }

        protected void OnMatchesPlayedSeriesClick(SeriesClickEventArgs args)
        {
            ViewModel.DrillDownAttackerMatchesPlayed = null;
            ViewModel.DrillDownDefenderMatchesPlayed = null;
            ShowMatchesPlayedDrillDownChart = false;
        }

        private string GenerateColClasses()
        {
            return "col-sm-12 col-md-12 col-lg-12 col-xl-6 col-xxl-6";
        }
    }
}
