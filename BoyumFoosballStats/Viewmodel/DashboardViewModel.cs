using BoyumFoosballStats.Helper;
using BoyumFoosballStats.Model;
using BoyumFoosballStats.Model.Enums;

namespace BoyumFoosballStats.Viewmodel
{
    public class DashboardViewModel : IDashboardViewModel
    {
        public List<Match> Matches { get; set; } = new List<Match>();
        public AzureBlobStorageHelper blobHelper { get; set; }
        public MatchAnalysisHelper analysisHelper { get; set; }

        public IOrderedEnumerable<KeyValuePair<string, double>> PlayerWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> TeamWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> PlayerMatchesPlayed { get; set; }
        public Dictionary<string, double> TableSideWinRates { get; set; }

        public List<PlayerPosition?> WinRateFilterOptions { get; set; }
        
        public void CalculateWinRates(object value)
        {
            var positionFilter = value as PlayerPosition?;
            PlayerWinrates = analysisHelper.CalculateWinRatesForAllPlayers(Matches, positionFilter);
            PlayerMatchesPlayed = analysisHelper.CalculateMatchesPlayedForAllPlayers(Matches, positionFilter);
        }
    }

    public interface IDashboardViewModel
    {
        List<Match> Matches { get; set; }
        AzureBlobStorageHelper blobHelper { get; set; }
        MatchAnalysisHelper analysisHelper { get; set; }

        List<PlayerPosition?> WinRateFilterOptions { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> PlayerWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, double>> TeamWinrates { get; set; }
        public IOrderedEnumerable<KeyValuePair<string, int>> PlayerMatchesPlayed { get; set; }
        public Dictionary<string, double> TableSideWinRates { get; set; }

        void CalculateWinRates(object value);
    }
}
